Option Strict On

Imports System.Data.OleDb
Imports System.Drawing.Printing

Public Class Form2

    '=== Variables globales ===
    Private idProduccionActual As Integer = -1
    Private cargarProduccion As Boolean = False

    'Guardado diferido: se agrupan los cambios y se guarda una sola vez tras una pausa
    Private guardadoPendiente As Boolean = False
    Private WithEvents timerGuardado As New System.Windows.Forms.Timer() With {.Interval = 700}

    'Codigo que tenia la celda antes de editarla (para saber si realmente cambio)
    Private codigoAntesDeEditar As String = ""

    ' Controla si dgvProduccionHechas muestra todo el historial o solo lo reciente.
    Private mostrarHistorialCompletoProduccion As Boolean = False
    Public dgvProductos As New DataGridView()

    'Variable para Impresion
    Private WithEvents impresionProduccion As New PrintDocument()
    Private impresora_Seleccionada As String = ""

    'Inicios del Form2
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Debug.WriteLine(">> Form2_Load")
        cargarProduccion = True

        ' Doble buffer: evita el parpadeo al limpiar/repoblar la lista lateral de producciones.
        ' DoubleBuffered es una propiedad protegida, por eso se activa via reflexion.
        GetType(DataGridView).GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).
            SetValue(dgvProduccionHechas, True, Nothing)

        'Iniciar Tabla de Produccion
        IniciarTablaProduccion()

        ' Se desactiva la seleccion multiple por el mismo motivo que en Form1:
        ' esta grilla es de carga secuencial, y una seleccion multiple accidental
        ' puede disparar CellEndEdit en filas no deseadas, pisando su cantidad real.
        dgvProduccion.MultiSelect = False

        'Cargar Columnas
        ConfigurarColumnas()

        'Cargar Empleados de Produccion
        CargarEmpleados()

        'Cargar lista de impresoras disponibles
        CargarImpresorasEnCombo(cbImpresora)

        'Cargar lista de producciones
        CargarListadoProduccion()
        'Lista configurada para no ser editable y elegir toda la fila 
        With dgvProduccionHechas
            .ReadOnly = True
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
        End With

        'Cargar ultima lista realizada
        Me.BeginInvoke(Sub()
                           CargarUltimaProduccion()
                       End Sub)

        For Each col As DataGridViewColumn In dgvProduccionHechas.Columns
            col.SortMode = DataGridViewColumnSortMode.NotSortable
        Next

        cargarProduccion = False

        'Label por default para el nombre de empleado
        lblEmpleadoID.Text = "Ingrese al Empleado para Produccion."
    End Sub

    'Estructura de la produccion
    Private Sub IniciarTablaProduccion()
        With dgvProduccion
            'Limpieza y creacion de columnas
            .Columns.Clear()
            .Columns.Add("codigo", "Código Producto")
            .Columns.Add("nombre", "Nombre Producto")
            .Columns.Add("cantidad", "Cantidad")
            .Columns.Add("subtotal", "Subtotal")

            'Solo "cantidad" es editable
            .Columns("nombre").ReadOnly = True
            .Columns("subtotal").ReadOnly = True

            'Precio unitario real (oculto): base para calcular el subtotal
            .Columns.Add("precio", "Precio")
            .Columns("precio").Visible = False

            'Botón para eliminar filas 
            Dim eliminarBtn As New DataGridViewButtonColumn()
            eliminarBtn.Name = "Eliminar"
            eliminarBtn.HeaderText = ""
            eliminarBtn.Text = "Eliminar"
            eliminarBtn.UseColumnTextForButtonValue = True
            .Columns.Add(eliminarBtn)
        End With
    End Sub

    'Carga de los empleados para su seleccion
    Private Sub CargarEmpleados()
        Try
            'Conexion a la base de datos para la obtencion de los empleados
            Using conexionLocal As New OleDbConnection(conexionDB.ConnectionString)
                conexionLocal.Open()

                'Consulta SQL para obtener empleados de la tabla empleados
                Dim query As String = "SELECT id_empleado, nombre_empleado FROM empleado ORDER BY nombre_empleado"
                Dim da As New OleDbDataAdapter(query, conexionLocal)
                Dim dt As New DataTable()
                da.Fill(dt)

                'Designacion de datos en el ComboBox
                cbIDEmpleado.DataSource = dt
                cbIDEmpleado.DisplayMember = "nombre_empleado" ' Nombre del Empleado
                cbIDEmpleado.ValueMember = "id_empleado"       ' Guardar ID del Empleado

                'Dejar sin seleccion por defecto
                cbIDEmpleado.SelectedIndex = -1

                'Mostrar texto guia por defecto
                lblEmpleadoID.Text = "Seleccionar el empleado para produccion."

            End Using

            'Catch para errores en la carga de empleados
        Catch ex As Exception
            MsgBox("Error al cargar empleados: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    'Autoactualizar la produccion al momento de realizar cambios
    Private Sub AutoActualizarProduccion()
        CargarListadoProduccion()
        dgvProduccionHechas.Refresh()
    End Sub

    'Texto de empleado al ser elegido
    Private Sub cbIDEmpleado_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbIDEmpleado.SelectedIndexChanged
        ' Evitar que se ejecute durante carga inicial
        If cargarProduccion OrElse idProduccionActual <= 0 Then Exit Sub

        ' Si hay selección válida
        If cbIDEmpleado.SelectedIndex >= 0 AndAlso cbIDEmpleado.SelectedValue IsNot Nothing Then
            lblEmpleadoID.Text = cbIDEmpleado.Text

            Try
                AbrirConexion()

                ' Actualizar el id_empleado en la producción actual
                Using cmdUpdate As New OleDbCommand("UPDATE produccion SET id_empleado = ? WHERE id_produccion = ?", conexionDB)
                    cmdUpdate.Parameters.AddWithValue("?", Convert.ToInt32(cbIDEmpleado.SelectedValue))
                    cmdUpdate.Parameters.AddWithValue("?", idProduccionActual)
                    cmdUpdate.ExecuteNonQuery()
                End Using

                ' Refrescar lista lateral
                CargarListadoProduccion()

            Catch ex As Exception
                MsgBox("Error al actualizar empleado: " & ex.Message, MsgBoxStyle.Critical)
            Finally
                CerrarConexion()
            End Try
        Else
            lblEmpleadoID.Text = "Sin empleado"
        End If
    End Sub

    'Marca que hay cambios y reinicia la cuenta regresiva del guardado
    Private Sub SolicitarGuardado()
        If cargarProduccion Then Exit Sub
        guardadoPendiente = True
        timerGuardado.Stop()
        timerGuardado.Start()
    End Sub

    Private Sub timerGuardado_Tick(sender As Object, e As EventArgs) Handles timerGuardado.Tick
        GuardarPendiente()
    End Sub

    'Guarda ahora lo que este pendiente (si no hay nada pendiente, no hace nada)
    Private Sub GuardarPendiente()
        timerGuardado.Stop()
        If Not guardadoPendiente Then Exit Sub
        guardadoPendiente = False
        Try
            GuardarProduccion()
        Catch ex As Exception
            MsgBox("Error durante autoguardado: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    'Antes de cerrar la ventana, no se pierde ningun cambio pendiente
    Private Sub Form2_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        GuardarPendiente()
    End Sub

    'Autoguardar al cambiar la fecha de produccion
    Private Sub dtpFechaProduccion_ValueChanged(sender As Object, e As EventArgs) Handles dtpFechaProduccion.ValueChanged
        If cargarProduccion Then Exit Sub
        GuardarProduccion()
    End Sub

    'Activado de autoguardado en cuanto se realizan cambios en el grid de produccion
    Private Sub dgvProduccion_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProduccion.CellValueChanged
        SolicitarGuardado()
    End Sub

    Private Sub dgvProduccion_RowsRemoved(sender As Object, e As DataGridViewRowsRemovedEventArgs) Handles dgvProduccion.RowsRemoved
        SolicitarGuardado()
    End Sub

    '=== Autoguardado cuando cambia el comentario ===
    Private Sub rtxtComentario_TextChanged(sender As Object, e As EventArgs) Handles rtxtComentario.TextChanged
        SolicitarGuardado()
    End Sub

    '=== Tabulacion de Codigo a Cantidad en el DataGrid
    '=== Tabulacion de Codigo a Cantidad en el DataGrid
    Private Sub dgvProduccion_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvProduccion.KeyDown
        If e.KeyCode = Keys.Enter AndAlso dgvProduccion.CurrentCell IsNot Nothing Then
            e.SuppressKeyPress = True ' Evita el beep y el salto de fila automático

            Dim filaActual As Integer = dgvProduccion.CurrentCell.RowIndex
            Dim colActual As Integer = dgvProduccion.CurrentCell.ColumnIndex

            If colActual = 0 Then
                ' Desde el codigo se pasa a la cantidad de la misma fila
                IrACantidad(filaActual)
            ElseIf colActual = 2 Then
                ' Desde la cantidad se pasa al final de la lista, listo para el siguiente producto
                IrAlFinalDeLaLista()
            End If
        End If
    End Sub

    '=== CONFIGURAR COLUMNAS DEL LISTADO LATERAL ===
    Private Sub ConfigurarColumnas()
        dgvProduccionHechas.Columns.Clear()
        dgvProduccionHechas.Columns.Add("fecha_produccion", "Fecha")
        dgvProduccionHechas.Columns.Add("nombre_empleado", "Empleado")
        dgvProduccionHechas.Columns.Add("total", "Total")

        dgvProduccionHechas.Columns("fecha_produccion").Width = 100
        dgvProduccionHechas.Columns("nombre_empleado").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        dgvProduccionHechas.Columns("total").Width = 90
    End Sub

    '=== CARGAR LISTADO DE PRODUCCIONES ===
    Private Sub CargarListadoProduccion()
        dgvProduccionHechas.SuspendLayout()
        Try
            AbrirConexion()

            Dim sql As String =
            "SELECT p.id_produccion, p.fecha_produccion, e.nombre_empleado, p.total " &
            "FROM produccion AS p " &
            "LEFT JOIN empleado AS e ON p.id_empleado = e.id_empleado "

            ' Por defecto solo se traen los ultimos 60 dias (mas rapido en PC modestas).
            ' El boton "Ver historial completo" permite alternar a traer todo cuando haga falta.
            If Not mostrarHistorialCompletoProduccion Then
                sql &= "WHERE p.fecha_produccion BETWEEN ? AND ? "
            End If

            sql &= "ORDER BY p.fecha_produccion DESC, p.id_produccion DESC"

            Using cmd As New OleDbCommand(sql, conexionDB)
                If Not mostrarHistorialCompletoProduccion Then
                    cmd.Parameters.AddWithValue("?", Date.Today.AddDays(-60))
                    cmd.Parameters.AddWithValue("?", Date.Today)
                End If

                Using reader = cmd.ExecuteReader()
                    dgvProduccionHechas.Rows.Clear()

                    While reader.Read()
                        Dim fechaProd As String = Convert.ToDateTime(reader("fecha_produccion")).ToShortDateString()
                        Dim nombreEmp As String = If(IsDBNull(reader("nombre_empleado")), "Sin empleado", reader("nombre_empleado").ToString())
                        Dim totalProd As String = String.Format("{0:N0}", reader("total")) & " $"

                        Dim filaIndex As Integer = dgvProduccionHechas.Rows.Add(fechaProd, nombreEmp, totalProd)
                        dgvProduccionHechas.Rows(filaIndex).Tag = reader("id_produccion")
                    End While
                End Using
            End Using

        Catch ex As Exception
            MsgBox("Error al cargar la lista de producciones: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
            dgvProduccionHechas.ResumeLayout()
        End Try
    End Sub

    '=== BOTÓN ACTUALIZAR ===
    Private Sub btnActualizarProduccion_Click(sender As Object, e As EventArgs) Handles btnActualizarProduccion.Click
        mostrarHistorialCompletoProduccion = Not mostrarHistorialCompletoProduccion

        If mostrarHistorialCompletoProduccion Then
            btnActualizarProduccion.Text = "Ver solo recientes"
        Else
            btnActualizarProduccion.Text = "Ver historial completo"
        End If

        ConfigurarColumnas()
        CargarListadoProduccion()
    End Sub

    '=== BOTON PARA ELIMINAR PRODUCCION
    Private Sub btnEliminarProduccion_Click(sender As Object, e As EventArgs) Handles btnEliminarProduccion.Click
        'Verificar si hay produccion cargada
        If idProduccionActual <= 0 Then
            MsgBox("No hay produccion cargada en pantalla.", MsgBoxStyle.Information)
            Exit Sub
        End If

        'Confirmacion de la eliminacion de la produccion
        Dim EliminarProduccion = MsgBox("Estas seguro de Eliminar la Produccion Nº " & idProduccionActual & "?", MsgBoxStyle.YesNo Or MsgBoxStyle.Question, "Confirmar Accion")
        If EliminarProduccion <> MsgBoxResult.Yes Then Exit Sub

        Dim idAEliminar As Integer = idProduccionActual
        Dim transaccion As OleDbTransaction = Nothing

        timerGuardado.Stop()
        guardadoPendiente = False

        'Mientras se elimina, se bloquea el autoguardado para que nada la vuelva a escribir
        cargarProduccion = True

        Try
            AbrirConexion()

            'Detalle y cabecera se eliminan juntos o no se elimina nada
            transaccion = conexionDB.BeginTransaction()

            Using cmdDetalle As New OleDbCommand("DELETE FROM produccion_detalle WHERE id_produccion = ?", conexionDB, transaccion)
                cmdDetalle.Parameters.AddWithValue("?", idAEliminar)
                cmdDetalle.ExecuteNonQuery()
            End Using

            Using cmdProduccion As New OleDbCommand("DELETE FROM produccion WHERE id_produccion = ?", conexionDB, transaccion)
                cmdProduccion.Parameters.AddWithValue("?", idAEliminar)
                cmdProduccion.ExecuteNonQuery()
            End Using

            transaccion.Commit()
            transaccion = Nothing

            'La produccion ya no existe: ningun guardado posterior debe apuntar a ella
            idProduccionActual = 0

            MsgBox("Produccion Eliminada", MsgBoxStyle.Information)

            'Buscar produccion Anterior
            Dim IDAnterior As Object = Nothing
            Using cmdAnterior As New OleDbCommand("SELECT MAX(id_produccion) FROM produccion WHERE id_produccion < ?", conexionDB)
                cmdAnterior.Parameters.AddWithValue("?", idAEliminar)
                IDAnterior = cmdAnterior.ExecuteScalar()
            End Using

            'Cargar produccion anterior
            If IDAnterior IsNot Nothing AndAlso IDAnterior IsNot DBNull.Value Then
                CargarProduccionID(Convert.ToInt32(IDAnterior))
            Else
                dgvProduccion.Rows.Clear()
                txtTotalProduccion.Text = "0 $"
                cbIDEmpleado.SelectedIndex = -1
                lblEmpleadoID.Text = "Seleccionar empleado para produccion."
                rtxtComentario.Clear()
                idProduccionActual = 0
            End If

            'Actualizar listado lateral
            CargarListadoProduccion()

        Catch ex As Exception
            'Si falla antes del Commit, se deshace todo y la produccion queda intacta
            Try
                If transaccion IsNot Nothing Then transaccion.Rollback()
            Catch
                'Si ni siquiera se pudo revertir, no hay mas accion posible aqui
            End Try
            MsgBox("Error al eliminar la produccion: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            cargarProduccion = False
            CerrarConexion()
        End Try
    End Sub

    '=== CARGAR LA ÚLTIMA PRODUCCIÓN AL ABRIR EL FORM ===
    Private Sub CargarUltimaProduccion()
        Try
            AbrirConexion()
            Using cmd As New OleDbCommand("SELECT TOP 1 id_produccion FROM produccion ORDER BY id_produccion DESC", conexionDB)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                    CargarProduccionID(Convert.ToInt32(result))
                End If
            End Using
        Catch ex As Exception
            MsgBox("Error al cargar la última producción: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try
    End Sub

    '=== EVENTO DE CLICK EN LISTA PARA CARGAR PRODUCCIÓN ===
    Private Sub dgvProduccionLista_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProduccionHechas.CellClick
        If e.RowIndex >= 0 AndAlso dgvProduccionHechas.Rows(e.RowIndex).Tag IsNot Nothing Then
            Dim idSeleccionado As Integer = Convert.ToInt32(dgvProduccionHechas.Rows(e.RowIndex).Tag)
            CargarProduccionID(idSeleccionado)
        End If
    End Sub

    'Indica si hay una produccion abierta donde guardar
    Public Function ProduccionAbierta() As Boolean
        Return idProduccionActual > 0
    End Function

    Public Sub GuardarProduccion()
        ' Si no existe producción creada aún, no guardar
        If idProduccionActual <= 0 Then Exit Sub

        If cargarProduccion Then Exit Sub

        timerGuardado.Stop()
        guardadoPendiente = False

        Dim transaccion As OleDbTransaction = Nothing

        Try
            AbrirConexion()

            Dim fecha As Date = dtpFechaProduccion.Value.Date

            ' Empleado puede ser NULL
            Dim idEmpleado As Object = DBNull.Value
            If cbIDEmpleado.SelectedValue IsNot Nothing AndAlso IsNumeric(cbIDEmpleado.SelectedValue) Then
                idEmpleado = Convert.ToInt32(cbIDEmpleado.SelectedValue)
            End If

            Dim comentario As String = rtxtComentario.Text
            Dim total As Integer = ObtenerTotalProduccion()

            ' A partir de aqui, todo se guarda junto o no se guarda nada
            transaccion = conexionDB.BeginTransaction()

            '=== SI YA EXISTE, ACTUALIZAR ===
            If idProduccionActual > 0 Then
                Using cmdActualizar As New OleDbCommand(
                "UPDATE produccion SET fecha_produccion = ?, id_empleado = ?, comentario = ?, total = ? 
                 WHERE id_produccion = ?", conexionDB, transaccion)

                    cmdActualizar.Parameters.AddWithValue("?", fecha)
                    cmdActualizar.Parameters.AddWithValue("?", idEmpleado)
                    cmdActualizar.Parameters.AddWithValue("?", comentario)
                    cmdActualizar.Parameters.AddWithValue("?", total)
                    cmdActualizar.Parameters.AddWithValue("?", idProduccionActual)

                    'Si no se actualizo ninguna fila, la produccion ya no existe (fue eliminada):
                    'se deshace todo para no crear detalles huerfanos.
                    If cmdActualizar.ExecuteNonQuery() = 0 Then
                        transaccion.Rollback()
                        transaccion = Nothing
                        Debug.WriteLine(">> GuardarProduccion: la produccion " & idProduccionActual & " ya no existe, no se guarda.")
                        Exit Sub
                    End If
                End Using

                'Eliminar detalles anteriores
                Using cmdBorrar As New OleDbCommand("DELETE FROM produccion_detalle WHERE id_produccion = ?", conexionDB, transaccion)
                    cmdBorrar.Parameters.AddWithValue("?", idProduccionActual)
                    cmdBorrar.ExecuteNonQuery()
                End Using

            End If

            '=== INSERTAR DETALLES ===
            For Each fila As DataGridViewRow In dgvProduccion.Rows
                If fila.IsNewRow Then Continue For

                If fila.Cells("codigo").Value Is Nothing OrElse
               fila.Cells("nombre").Value Is Nothing OrElse
               fila.Cells("cantidad").Value Is Nothing OrElse
               fila.Cells("subtotal").Value Is Nothing Then Continue For

                Dim codigo As String = fila.Cells("codigo").Value.ToString()
                Dim nombre As String = fila.Cells("nombre").Value.ToString()
                Dim cantidad As Integer = Convert.ToInt32(fila.Cells("cantidad").Value)
                Dim subtotal As Integer = LimpiarMonto(fila.Cells("subtotal").Value)

                ' Precio unitario fijo para que producciones antiguas no se alteren
                Dim precioUnitario As Integer = LimpiarMonto(fila.Cells("precio").Value)

                Using cmdDetalle As New OleDbCommand(
                "INSERT INTO produccion_detalle
                (id_produccion, codigo_producto, nombre_producto, cantidad, precio_unitario, subtotal)
                 VALUES (?,?,?,?,?,?)", conexionDB, transaccion)

                    cmdDetalle.Parameters.AddWithValue("?", idProduccionActual)
                    cmdDetalle.Parameters.AddWithValue("?", codigo)
                    cmdDetalle.Parameters.AddWithValue("?", nombre)
                    cmdDetalle.Parameters.AddWithValue("?", cantidad)
                    cmdDetalle.Parameters.AddWithValue("?", precioUnitario)
                    cmdDetalle.Parameters.AddWithValue("?", subtotal)

                    cmdDetalle.ExecuteNonQuery()
                End Using
            Next

            ' Todo salio bien: recien AHORA se confirman los cambios de forma definitiva
            transaccion.Commit()

            'Actualizar lista lateral
            CargarListadoProduccion()

        Catch ex As Exception
            ' Algo fallo: se deshace TODO lo hecho desde BeginTransaction (deja la produccion como estaba antes del guardado)
            Try
                If transaccion IsNot Nothing Then transaccion.Rollback()
            Catch
                ' Si ni siquiera se pudo revertir (ej. se perdio la conexion en pleno intento), no hay mas accion posible aqui
            End Try
            MsgBox("Error al guardar la producción. No se guardó ningún cambio parcial: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try
    End Sub

    'Calcular total general de la produccion
    Private Function ObtenerTotalProduccion() As Integer
        'El valor es 0 si no hay filas
        Dim total As Integer = 0

        For Each fila As DataGridViewRow In dgvProduccion.Rows
            If Not fila.IsNewRow Then
                total += LimpiarMonto(fila.Cells("subtotal").Value)
            End If
        Next

        Return total
    End Function

    'Actualizar total visual
    Public Sub CalcularTotalProduccion()
        Dim total = ObtenerTotalProduccion()
        txtTotalProduccion.Text = String.Format("{0:N0}", total) & " $"
    End Sub

    'Limpia los montos de los totales
    Private Function LimpiarMonto(monto As Object) As Integer
        Return CatalogoProductos.MontoEntero(monto)
    End Function

    Private Sub CargarProduccionID(idProduccion As Integer)
        Debug.WriteLine(">> CargarProduccionID con ID = " & idProduccion)

        GuardarPendiente()

        cargarProduccion = True
        dgvProduccion.Rows.Clear()

        Try
            AbrirConexion()

            'Cargar datos de produccion a traves de una consulta SQL
            Using cmdProduccion As New OleDbCommand("SELECT * FROM produccion WHERE id_produccion = ?", conexionDB)
                cmdProduccion.Parameters.AddWithValue("?", idProduccion)

                Using reader = cmdProduccion.ExecuteReader()
                    If reader.Read() Then
                        'Carga con el ID Produccion y Fecha de Produccion
                        idProduccionActual = idProduccion
                        dtpFechaProduccion.Value = Convert.ToDateTime(reader("fecha_produccion"))

                        Dim idEmpleado As Integer? = Nothing
                        If Not IsDBNull(reader("id_empleado")) Then idEmpleado = Convert.ToInt32(reader("id_empleado"))

                        If idEmpleado.HasValue AndAlso idEmpleado.Value > 0 Then
                            Dim empleadoDesignado As Boolean = False

                            'Designar directamente si el combobox posee los datos cargados
                            If cbIDEmpleado IsNot Nothing AndAlso cbIDEmpleado.DataSource IsNot Nothing Then
                                Dim dataEmpleado As DataTable = TryCast(cbIDEmpleado.DataSource, DataTable)

                                If dataEmpleado IsNot Nothing AndAlso dataEmpleado.Select("id_empleado = " & idEmpleado.Value).Length > 0 Then
                                    cbIDEmpleado.SelectedValue = idEmpleado.Value
                                    lblEmpleadoID.Text = cbIDEmpleado.Text
                                    empleadoDesignado = True
                                End If
                            End If
                            'Si no pudo ser designado, buscar en la base de datos
                            If Not empleadoDesignado Then
                                Using cmdEmpleado As New OleDbCommand("SELECT nombre_empleado FROM empleado WHERE id_empleado = ?", conexionDB)
                                    cmdEmpleado.Parameters.AddWithValue("?", idEmpleado.Value)
                                    Dim resultado = cmdEmpleado.ExecuteScalar()

                                    If resultado IsNot Nothing AndAlso Not IsDBNull(resultado) Then
                                        lblEmpleadoID.Text = resultado.ToString()
                                    Else
                                        lblEmpleadoID.Text = "Empleado no designado."
                                    End If
                                End Using
                            End If
                        Else
                            'Si no posee un empleado designado 
                            cbIDEmpleado.SelectedIndex = -1
                            lblEmpleadoID.Text = "Seleccionar empleado para produccion."
                        End If
                        'Carga de los comentarios y total de produccion
                        rtxtComentario.Text = reader("comentario").ToString()
                        txtTotalProduccion.Text = String.Format("{0:N0}", reader("total")) & " $"
                    End If
                End Using
            End Using

            'Cargar detalles de produccion a traves de una consulta SQL
            Using cmdDetalle As New OleDbCommand("SELECT codigo_producto, nombre_producto, cantidad, precio_unitario, subtotal FROM produccion_detalle WHERE id_produccion = ?", conexionDB)
                cmdDetalle.Parameters.AddWithValue("?", idProduccion)

                'Toma de datos para su posterior carga en el grid
                Using reader = cmdDetalle.ExecuteReader()
                    While reader.Read()
                        Dim codigo As Object = If(IsDBNull(reader("codigo_producto")), "", reader("codigo_producto"))
                        Dim nombre As Object = If(IsDBNull(reader("nombre_producto")), "", reader("nombre_producto"))
                        Dim cantidad As Integer = If(IsDBNull(reader("cantidad")), 0, Convert.ToInt32(reader("cantidad")))
                        Dim subtotal As Integer = If(IsDBNull(reader("subtotal")), 0, Convert.ToInt32(reader("subtotal")))

                        Dim precioUnit As Integer = If(IsDBNull(reader("precio_unitario")), 0, Convert.ToInt32(reader("precio_unitario")))
                        If precioUnit = 0 AndAlso cantidad > 0 Then precioUnit = subtotal \ cantidad

                        dgvProduccion.Rows.Add(codigo, nombre, cantidad, "$ " & String.Format("{0:N0}", subtotal), precioUnit)
                    End While
                End Using
            End Using

            'Captura de Errores y cierre de conexion
        Catch ex As Exception
            MsgBox("Error al cargar la producción: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
            cargarProduccion = False
        End Try
    End Sub

    Private Sub btnNuevaProduccion_Click(sender As Object, e As EventArgs) Handles btnNuevaProduccion.Click

        GuardarPendiente()

        Try
            AbrirConexion()

            ' Insertar nueva producción solo con fecha, total 0 y sin empleado
            Using cmdInsertar As New OleDbCommand("INSERT INTO produccion (fecha_produccion, id_empleado, total) VALUES (?, NULL, ?)", conexionDB)
                cmdInsertar.Parameters.AddWithValue("?", Date.Today)
                cmdInsertar.Parameters.AddWithValue("?", 0)
                cmdInsertar.ExecuteNonQuery()
            End Using

            ' Obtener el ID recién creado
            Using cmdID As New OleDbCommand("SELECT @@IDENTITY", conexionDB)
                idProduccionActual = Convert.ToInt32(cmdID.ExecuteScalar())
            End Using

            ' Resetear interfaz
            dgvProduccion.Rows.Clear()
            rtxtComentario.Clear()
            txtTotalProduccion.Text = "$ 0"
            cbIDEmpleado.SelectedIndex = -1
            lblEmpleadoID.Text = "Sin empleado"
            dtpFechaProduccion.Value = Date.Today

            ' Permitir edición en el grid
            dgvProduccion.ReadOnly = False

            ' Actualizar lista lateral
            CargarListadoProduccion()

        Catch ex As Exception
            MsgBox("Error al crear nueva producción: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try
    End Sub

    Private Sub btnSiguienteProduccion_Click(sender As Object, e As EventArgs) Handles btnSiguienteProduccion.Click
        Try
            AbrirConexion()

            Using cmd As New OleDbCommand("SELECT MIN(id_produccion) FROM produccion WHERE id_produccion > ?", conexionDB)
                cmd.Parameters.AddWithValue("?", idProduccionActual)
                Dim siguienteID = cmd.ExecuteScalar()

                If siguienteID IsNot DBNull.Value AndAlso siguienteID IsNot Nothing Then
                    CargarProduccionID(Convert.ToInt32(siguienteID))
                Else
                    MsgBox("No hay más producciones siguientes.", MsgBoxStyle.Information)
                End If
            End Using

        Catch ex As Exception
            MsgBox("Error al buscar producciones siguientes: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try
    End Sub

    Private Sub btnAnteriorProduccion_Click(sender As Object, e As EventArgs) Handles btnAnteriorProduccion.Click
        Try
            AbrirConexion()

            Using cmd As New OleDbCommand("SELECT MAX(id_produccion) FROM produccion WHERE id_produccion < ?", conexionDB)
                cmd.Parameters.AddWithValue("?", idProduccionActual)
                Dim anteriorID = cmd.ExecuteScalar()

                If anteriorID IsNot DBNull.Value AndAlso anteriorID IsNot Nothing Then
                    CargarProduccionID(Convert.ToInt32(anteriorID))
                Else
                    MsgBox("No hay más producciones anteriores.", MsgBoxStyle.Information)
                End If
            End Using

        Catch ex As Exception
            MsgBox("Error al buscar producciones anteriores: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try
    End Sub

    Private Sub dgvProduccion_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgvProduccion.CellBeginEdit
        'Sin produccion abierta no hay donde guardar: se avisa en vez de aceptar datos que se perderian
        If idProduccionActual <= 0 Then
            e.Cancel = True
            MsgBox("No hay una producción abierta. Pulsa «Nueva producción» antes de agregar productos.", MsgBoxStyle.Information)
            Exit Sub
        End If

        If e.ColumnIndex = 0 Then
            codigoAntesDeEditar = dgvProduccion.Rows(e.RowIndex).Cells(0).Value?.ToString()
        End If
    End Sub

    'Lleva el cursor a la cantidad de una fila
    Private Sub IrACantidad(fila As Integer)
        Try
            dgvProduccion.CurrentCell = dgvProduccion.Rows(fila).Cells("cantidad")
            dgvProduccion.BeginEdit(True)
        Catch ex As Exception
            Debug.WriteLine("Error al seleccionar Cantidad: " & ex.Message)
        End Try
    End Sub

    'Lleva el cursor al codigo de la ultima fila (si ya tiene un producto, crea una nueva)
    Private Sub IrAlFinalDeLaLista()
        Try
            Dim ultima As Integer = dgvProduccion.Rows.Count - 1
            If ultima < 0 OrElse Not String.IsNullOrWhiteSpace(dgvProduccion.Rows(ultima).Cells("codigo").Value?.ToString()) Then
                dgvProduccion.Rows.Add()
                ultima = dgvProduccion.Rows.Count - 1
            End If
            dgvProduccion.CurrentCell = dgvProduccion.Rows(ultima).Cells("codigo")
            dgvProduccion.BeginEdit(True)
        Catch ex As Exception
            Debug.WriteLine("Error al mover el foco al final de la lista: " & ex.Message)
        End Try
    End Sub

    Private Sub dgvProduccion_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProduccion.CellEndEdit
        ' Si editamos la columna de código
        If e.ColumnIndex = 0 Then
            Dim codigo As String = dgvProduccion.Rows(e.RowIndex).Cells(0).Value?.ToString()

            If Not String.IsNullOrEmpty(codigo) Then

                'Si el codigo no cambio y la fila ya esta cargada, no se consulta el catalogo ni se
                'toca el precio guardado: asi recorrer filas con Enter no altera el historial.
                Dim filaCargada As Boolean = Not String.IsNullOrWhiteSpace(dgvProduccion.Rows(e.RowIndex).Cells(1).Value?.ToString())
                If codigo = codigoAntesDeEditar AndAlso filaCargada Then
                    Me.BeginInvoke(Sub() IrACantidad(e.RowIndex))
                    Exit Sub
                End If

                Try
                    'Usar una conexión local y cerrarla manualmente (para evitar RCW)
                    Using conexionLocal As New OleDbConnection(conexionDB.ConnectionString)
                        conexionLocal.Open()

                        Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ? AND (productoActivo = True OR productoActivo IS NULL)", conexionLocal)
                            cmd.Parameters.AddWithValue("?", codigo)

                            Using reader As OleDbDataReader = cmd.ExecuteReader()
                                If reader.Read() Then
                                    Dim nombre As String = reader("nombre").ToString()
                                    Dim precioBoleta As Integer = CatalogoProductos.PrecioEntero(reader("precio_venta_boleta"))

                                    With dgvProduccion.Rows(e.RowIndex)
                                        .Cells(1).Value = nombre
                                        If IsDBNull(.Cells(2).Value) OrElse String.IsNullOrWhiteSpace(.Cells(2).Value?.ToString()) Then
                                            .Cells(2).Value = 1
                                        End If
                                        .Cells("precio").Value = precioBoleta
                                        Dim cantidadFila As Integer = 1
                                        Integer.TryParse(.Cells(2).Value?.ToString(), cantidadFila)
                                        .Cells(3).Value = "$ " & String.Format("{0:N0}", cantidadFila * precioBoleta)
                                    End With

                                    CalcularTotalProduccion()

                                    'Tras ingresar el codigo producto, se selecciona cantidad automaticamente
                                    Me.BeginInvoke(Sub() IrACantidad(e.RowIndex))
                                Else
                                    MsgBox("Código de producto no válido o inactivo: " & codigo, MsgBoxStyle.Exclamation)
                                    dgvProduccion.Rows.RemoveAt(e.RowIndex)
                                End If
                            End Using
                        End Using
                    End Using

                Catch ex As Exception
                    MsgBox("Error al consultar producto: " & ex.Message)
                End Try
            End If

            'Si editamos la columna de cantidad
        ElseIf e.ColumnIndex = 2 Then
            Try
                Dim fila = dgvProduccion.Rows(e.RowIndex)

                Dim cantidad As Integer = 0
                Integer.TryParse(fila.Cells(2).Value?.ToString(), cantidad)
                If cantidad < 0 Then cantidad = 0

                Dim preciounitario As Integer = 0
                Integer.TryParse(fila.Cells("precio").Value?.ToString(), preciounitario)
                fila.Cells(3).Value = "$ " & String.Format("{0:N0}", cantidad * preciounitario)

                CalcularTotalProduccion()

                'Mover foco al final de la lista, listo para el siguiente producto
                Me.BeginInvoke(Sub() IrAlFinalDeLaLista())

            Catch ex As Exception
                MsgBox("Error al recalcular el subototal de los productos: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub dgvProduccion_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProduccion.CellClick
        If e.ColumnIndex = dgvProduccion.Columns("Eliminar").Index AndAlso e.RowIndex >= 0 Then
            If Not dgvProduccion.Rows(e.RowIndex).IsNewRow Then
                dgvProduccion.Rows.RemoveAt(e.RowIndex)
                CalcularTotalProduccion()
                GuardarProduccion()
            End If
        End If
    End Sub

    Private Sub ValidarEntradaNumerica(sender As Object, e As KeyPressEventArgs)
        Dim txt As TextBox = CType(sender, TextBox)

        If Char.IsControl(e.KeyChar) Then Return
        If Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
            Return
        End If
        If txt.TextLength >= 5 Then
            e.Handled = True
        End If
    End Sub

    Private Sub dgvProduccion_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles dgvProduccion.EditingControlShowing
        If dgvProduccion.CurrentCell.ColumnIndex = 0 Or dgvProduccion.CurrentCell.ColumnIndex = 2 Then
            Dim txtEdit As TextBox = TryCast(e.Control, TextBox)
            If txtEdit IsNot Nothing Then
                RemoveHandler txtEdit.KeyPress, AddressOf ValidarEntradaNumerica
                AddHandler txtEdit.KeyPress, AddressOf ValidarEntradaNumerica
            End If
        End If
    End Sub

    Private Sub btnAgregarProduccion_Click(sender As Object, e As EventArgs) Handles btnAgregarProduccion.Click
        Using FormProducc_Productos As New FormProducc_Productos
            FormProducc_Productos.Owner = Me
            FormProducc_Productos.ShowDialog()
        End Using
    End Sub

    'Al cerrar la ventana se liberan los recursos de impresion
    Private Sub Form2_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        fuenteTitulo.Dispose()
        fuenteCuerpo.Dispose()
        impresionProduccion.Dispose()
        timerGuardado.Dispose()
    End Sub

    'Fuente, Cuerpo y posicion para impresion
    Dim fuenteTitulo As New Font("Consolas", 8, FontStyle.Bold)
    Dim fuenteCuerpo As New Font("Consolas", 7)
    Dim yPos As Integer = 0
    Private seImprimioEnVistaPrevia As Boolean = False

    '==SECCION DE IMPRESION DE PRODUCCION==
    Private Sub btnImprimirProduccion_Click(sender As Object, e As EventArgs) Handles btnImprimirProduccion.Click
        Try
            ' Configurar tamaño de papel (ancho 80mm ≈ 283, largo 297mm ≈ 1169)
            Dim tamañoPapel As New PaperSize("Ticket80mm", 283, 1169)
            impresionProduccion.DefaultPageSettings.PaperSize = tamañoPapel

            ' Margen mínimo para aprovechar mejor el espacio
            impresionProduccion.DefaultPageSettings.Margins = New Margins(5, 5, 5, 5)

            ' Especificar impresora (usar la seleccionada)
            ' Si hay una impresora seleccionada, usarla; de lo contrario, usar la predeterminada
            If Not String.IsNullOrEmpty(impresora_Seleccionada) Then
                impresionProduccion.PrinterSettings.PrinterName = impresora_Seleccionada
            End If

            seImprimioEnVistaPrevia = False

            Using vistaPrevia As New PrintPreviewDialog()
                vistaPrevia.Document = impresionProduccion
                vistaPrevia.WindowState = FormWindowState.Maximized
                vistaPrevia.PrintPreviewControl.Zoom = 2.0 'Escala de ventana del 200% CUIDADO!
                vistaPrevia.Text = "Vista Previa de Producción"
                vistaPrevia.ShowDialog()
            End Using

            'Si no se imprimio desde la vista previa, se ofrece imprimir ahora
            If Not seImprimioEnVistaPrevia Then
                Dim respuesta = MsgBox("¿Imprimir la producción ahora?",
                                       MsgBoxStyle.YesNo Or MsgBoxStyle.Question, "Imprimir producción")
                If respuesta = MsgBoxResult.Yes Then impresionProduccion.Print()
            End If

        Catch ex As Exception
            MsgBox("Error al mostrar la vista previa de la guia!" & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub impresionProduccion_BeginPrint(sender As Object, e As PrintEventArgs) Handles impresionProduccion.BeginPrint
        If e.PrintAction = PrintAction.PrintToPrinter Then seImprimioEnVistaPrevia = True
    End Sub

    Private Sub ImpresionProduccion_PrintPage(sender As Object, e As PrintPageEventArgs) Handles impresionProduccion.PrintPage
        'Elementos de la impresion, y margenes de impresion
        Dim g As Graphics = e.Graphics
        Dim margenIzq As Integer = e.MarginBounds.Left
        yPos = e.MarginBounds.Top
        Dim salto As Integer = CInt(fuenteCuerpo.GetHeight(g)) + 2

        '=== Encabezado de la guia ===
        g.DrawString("Produccion Interna", fuenteTitulo, Brushes.Black, margenIzq, yPos)
        yPos += salto

        g.DrawString("Empleado: " & lblEmpleadoID.Text, fuenteCuerpo, Brushes.Black, margenIzq, yPos)
        yPos += salto
        g.DrawString("Fecha: " & dtpFechaProduccion.Value.ToString("dd/MM/yyyy"), fuenteCuerpo, Brushes.Black, margenIzq, yPos)
        yPos += salto * 2

        '== Cabecera de Tabla ==
        g.DrawString("Código  Nombre Producto           Cant   Subtotal", fuenteCuerpo, Brushes.Black, margenIzq, yPos)
        yPos += salto
        g.DrawString("--------------------------------------------------", fuenteCuerpo, Brushes.Black, margenIzq, yPos)
        yPos += salto

        '== Cuerpo de la tabla ==

        For Each fila As DataGridViewRow In dgvProduccion.Rows
            If fila.IsNewRow Then Continue For

            Dim codigo As String = If(fila.Cells("codigo").Value, "").ToString().PadRight(6)
            Dim nombre As String = If(fila.Cells("nombre").Value, "").ToString().PadRight(25)
            Dim cantidad As String = If(fila.Cells("cantidad").Value, "0").ToString().PadLeft(4)
            Dim subtotal As Integer = LimpiarMonto(fila.Cells("subtotal").Value)

            Dim linea As String = $"{codigo}{nombre}{cantidad.PadLeft(6)}{subtotal.ToString("N0").PadLeft(8)}"
            g.DrawString(linea, fuenteCuerpo, Brushes.Black, margenIzq, yPos)
            yPos += salto
        Next

        '== Total de Produccion ==
        yPos += salto
        g.DrawString("--------------------------------------------------", fuenteCuerpo, Brushes.Black, margenIzq, yPos)
        yPos += salto
        g.DrawString("TOTAL:".PadLeft(33) & txtTotalProduccion.Text.Replace(" ", ""), fuenteTitulo, Brushes.Black, margenIzq, yPos)
        yPos += salto * 2

        '== Comentario de Produccion ==
        g.DrawString("COMENTARIO:", fuenteTitulo, Brushes.Black, margenIzq, yPos)
        yPos += salto

        Dim comentarioTexto As String = rtxtComentario.Text.Trim()
        If comentarioTexto <> "" Then
            ' Definir área de texto con márgenes
            Dim anchoTexto As Single = 280 ' ancho útil del ticket (ajústalo a tu impresora)
            Dim rectComentario As New RectangleF(margenIzq, yPos, anchoTexto, 9999)

            ' Configurar formato de texto
            Dim formato As New StringFormat()
            formato.Alignment = StringAlignment.Near
            formato.LineAlignment = StringAlignment.Near

            ' Dibujar comentario con ajuste automático de líneas
            g.DrawString(comentarioTexto, fuenteCuerpo, Brushes.Black, rectComentario, formato)

            ' Mover yPos hacia abajo según el texto
            Dim tamañoTexto As SizeF = g.MeasureString(comentarioTexto, fuenteCuerpo, CInt(anchoTexto))
            yPos += CInt(Math.Ceiling(tamañoTexto.Height))
        Else
            g.DrawString("(Sin comentario)", fuenteCuerpo, Brushes.Black, margenIzq, yPos)
            yPos += salto
        End If

    End Sub

    'Actualizar la impresora seleccionada
    Private Sub cbImpresora_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbImpresora.SelectedIndexChanged
        'Si no hay impresoras el combo queda deshabilitado con un aviso: ese texto no es una impresora
        If cbImpresora.Enabled AndAlso cbImpresora.SelectedIndex >= 0 Then
            impresora_Seleccionada = cbImpresora.SelectedItem.ToString()
        End If
    End Sub

End Class

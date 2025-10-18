Imports System.Data.OleDb
Imports System.Globalization

Public Class Form2

    '=== Variables globales ===
    Private autoGuardadoHabilitado As Boolean = True
    Private idProduccionActual As Integer = -1
    Private cargarProduccion As Boolean = False
    Public dgvProductos As New DataGridView()

    'Inicios del Form2
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Debug.WriteLine(">> Form2_Load")
        cargarProduccion = True

        'Iniciar Tabla de Produccion
        IniciarTablaProduccion()

        'Cargar Columnas
        ConfigurarColumnas()

        'Cargar Empleados de Produccion
        CargarEmpleados()

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

    'Texto de empleado al ser elegido
    Private Sub cbIDEmpleado_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbIDEmpleado.SelectedIndexChanged
        'Evitar que el codigo se ejecute al cargar los datos
        If cargarProduccion Then Exit Sub

        'Si ya existe un empleado seleccionado
        If cbIDEmpleado.SelectedIndex >= 0 AndAlso cbIDEmpleado.SelectedValue IsNot Nothing Then
            lblEmpleadoID.Text = cbIDEmpleado.Text
        Else
            lblEmpleadoID.Text = "Seleccionar el empleado para produccion."
        End If
    End Sub

    'Activado de autoguardado en cuanto se realizan cambios en el grid de produccion
    Private Sub dgvProduccion_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProduccion.CellValueChanged
        If cargarProduccion Then Exit Sub
        BeginInvoke(Sub()
                        Try
                            GuardarProduccion()
                        Catch ex As Exception
                            MsgBox("Error durante autoguardado: " & ex.Message, MsgBoxStyle.Critical)
                        End Try
                    End Sub)
    End Sub

    Private Sub dgvProduccion_RowsRemoved(sender As Object, e As DataGridViewRowsRemovedEventArgs) Handles dgvProduccion.RowsRemoved
        If cargarProduccion Then Exit Sub
        BeginInvoke(Sub()
                        Try
                            GuardarProduccion()
                        Catch ex As Exception
                            MsgBox("Error durante autoguardado: " & ex.Message, MsgBoxStyle.Critical)
                        End Try
                    End Sub)
    End Sub

    '=== Autoguardado cuando cambia el comentario ===
    Private Sub rtxtComentario_TextChanged(sender As Object, e As EventArgs) Handles rtxtComentario.TextChanged
        If cargarProduccion Then Exit Sub
        BeginInvoke(Sub()
                        Try
                            GuardarProduccion()
                        Catch ex As Exception
                            MsgBox("Error al guardar comentario: " & ex.Message, MsgBoxStyle.Critical)
                        End Try
                    End Sub)
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
        Try
            AbrirConexion()

            Dim sql As String =
            "SELECT p.id_produccion, p.fecha_produccion, e.nombre_empleado, p.total " &
            "FROM produccion AS p " &
            "INNER JOIN empleado AS e ON p.id_empleado = e.id_empleado " &
            "ORDER BY p.id_produccion DESC"

            Using cmd As New OleDbCommand(sql, conexionDB)
                Using reader = cmd.ExecuteReader()
                    dgvProduccionHechas.Rows.Clear()

                    While reader.Read()
                        Dim fechaProd As String = Convert.ToDateTime(reader("fecha_produccion")).ToShortDateString()
                        Dim nombreEmp As String = reader("nombre_empleado").ToString()
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
        End Try
    End Sub

    '=== BOTÓN ACTUALIZAR ===
    Private Sub btnActualizarProduccion_Click(sender As Object, e As EventArgs) Handles btnActualizarProduccion.Click
        ConfigurarColumnas()
        CargarListadoProduccion()
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

    Public Sub GuardarProduccion()
        'Si no hay empleado seleccionado o el grid está vacío, no hace nada
        If cbIDEmpleado.SelectedValue Is Nothing OrElse dgvProduccion.Rows.Count = 0 Then Exit Sub

        Try
            AbrirConexion()

            Dim fecha As Date = dtpFechaProduccion.Value
            Dim idEmpleado As Integer = Convert.ToInt32(cbIDEmpleado.SelectedValue)
            Dim comentario As String = rtxtComentario.Text
            Dim total As Integer = ObtenerTotalProduccion()

            'Actualizar la produccion existente o crear una nueva
            If idProduccionActual > 0 Then
                'Actualizar producción existente
                Using cmdActualizar As New OleDbCommand("UPDATE produccion SET fecha_produccion = ?, id_empleado = ?, comentario = ?, total = ? WHERE id_produccion = ?", conexionDB)
                    cmdActualizar.Parameters.AddWithValue("?", fecha)
                    cmdActualizar.Parameters.AddWithValue("?", idEmpleado)
                    cmdActualizar.Parameters.AddWithValue("?", comentario)
                    cmdActualizar.Parameters.AddWithValue("?", total)
                    cmdActualizar.Parameters.AddWithValue("?", idProduccionActual)
                    cmdActualizar.ExecuteNonQuery()
                End Using

                'Eliminar detalles antiguos
                Using cmdBorrar As New OleDbCommand("DELETE FROM produccion_detalle WHERE id_produccion = ?", conexionDB)
                    cmdBorrar.Parameters.AddWithValue("?", idProduccionActual)
                    cmdBorrar.ExecuteNonQuery()
                End Using
            Else
                'Crear nueva producción
                Using cmdInsertar As New OleDbCommand("INSERT INTO produccion (fecha_produccion, id_empleado, comentario, total) VALUES (?,?,?,?)", conexionDB)
                    cmdInsertar.Parameters.AddWithValue("", fecha)
                    cmdInsertar.Parameters.AddWithValue("", idEmpleado)
                    cmdInsertar.Parameters.AddWithValue("", comentario)
                    cmdInsertar.Parameters.AddWithValue("", total)
                    cmdInsertar.ExecuteNonQuery()

                    Using cmdID As New OleDbCommand("SELECT @@IDENTITY", conexionDB)
                        idProduccionActual = Convert.ToInt32(cmdID.ExecuteScalar())
                    End Using
                End Using
            End If

            'Insertar los detalles de producción
            For Each fila As DataGridViewRow In dgvProduccion.Rows
                If fila.IsNewRow Then Continue For

                If fila.Cells("codigo").Value Is Nothing OrElse
                   fila.Cells("nombre").Value Is Nothing OrElse
                   fila.Cells("cantidad").Value Is Nothing OrElse
                   fila.Cells("subtotal").Value Is Nothing Then Continue For

                Dim codigo = fila.Cells("codigo").Value.ToString()
                Dim nombre = fila.Cells("nombre").Value.ToString()
                Dim cantidad = Convert.ToInt32(fila.Cells("cantidad").Value)
                Dim subtotal = LimpiarMonto(fila.Cells("subtotal").Value)

                'Precio unitario para evitar el cambio de precios en guias antiguas
                Dim precioUnitario As Integer = 0
                If cantidad > 0 Then
                    precioUnitario = subtotal \ cantidad
                End If

                'Insertar detalles de produccion en la tabla detalle_produccion
                Using cmdDetalle As New OleDbCommand("INSERT INTO produccion_detalle (id_produccion, codigo_producto, nombre_producto, cantidad, precio_unitario, subtotal) VALUES (?,?,?,?,?,?)", conexionDB)
                    cmdDetalle.Parameters.AddWithValue("?", idProduccionActual)
                    cmdDetalle.Parameters.AddWithValue("?", codigo)
                    cmdDetalle.Parameters.AddWithValue("?", nombre)
                    cmdDetalle.Parameters.AddWithValue("?", cantidad)
                    cmdDetalle.Parameters.AddWithValue("?", precioUnitario)
                    cmdDetalle.Parameters.AddWithValue("?", subtotal)
                    cmdDetalle.ExecuteNonQuery()
                End Using
            Next

            'Actualizar listado lateral automáticamente
            CargarListadoProduccion()

            'Captura de errores y cierre de conexion
        Catch ex As Exception
            MsgBox("Error al guardar la producción: " & ex.Message, MsgBoxStyle.Critical)
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
                Dim subtotalTexto = fila.Cells("subtotal").Value?.ToString().Replace(".", "")
                total += Val(subtotalTexto)
            End If
        Next
        Return total
    End Function

    'Actualizar total visual
    Public Sub CalcularTotalProduccion()
        Dim total = ObtenerTotalProduccion()
        txtTotalProduccion.Text = String.Format("{0:N0}", total) & " $"
    End Sub

    'Limpiar los símbolos en montos
    Private Function LimpiarMonto(monto As Object) As Integer
        If monto Is Nothing OrElse String.IsNullOrWhiteSpace(monto.ToString()) Then Return 0
        Return Convert.ToInt32(monto.ToString().Replace(" $", "").Replace(".", ""))
    End Function

    Private Sub CargarProduccionID(idProduccion As Integer)
        Debug.WriteLine(">> CargarProduccionID con ID = " & idProduccion)

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

                        If idEmpleado.HasValue Then
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
                            cbIDEmpleado.SelectedIndex = 0
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

                        'Agregado de la fila en el grid
                        dgvProduccion.Rows.Add(codigo, nombre, cantidad, String.Format("{0:N0}", subtotal) & " $")
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
        'Limpieza de las columnas del grid
        dgvProduccion.Rows.Clear()

        'Limpiar la caja de comenarios
        rtxtComentario.Clear()
        txtTotalProduccion.Text = "0 $"
        idProduccionActual = 0

        'Fecha actual
        dtpFechaProduccion.Value = Date.Today

        cbIDEmpleado.SelectedIndex = -1
        lblEmpleadoID.Text = ""

        'Guardar nueva producción vacía
        GuardarProduccion()

        'Actualizar listado lateral
        CargarListadoProduccion()
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
    Private Sub dgvProduccion_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProduccion.CellEndEdit
        ' Si editamos la columna de código
        If e.ColumnIndex = 0 Then
            Dim codigo As String = dgvProduccion.Rows(e.RowIndex).Cells(0).Value?.ToString()

            If Not String.IsNullOrEmpty(codigo) Then

                Try
                    ' 🔹 Usar una conexión local y cerrarla manualmente (para evitar RCW)
                    Using conexionLocal As New OleDbConnection(conexionDB.ConnectionString)
                        conexionLocal.Open()

                        Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionLocal)
                            cmd.Parameters.AddWithValue("?", codigo)

                            Using reader As OleDbDataReader = cmd.ExecuteReader()
                                If reader.Read() Then
                                    Dim nombre As String = reader("nombre").ToString()
                                    Dim precioBoleta As Integer = Convert.ToInt32(reader("precio_venta_boleta"))
                                    Dim precioFormato As String = String.Format("{0:N0}", precioBoleta) & " $"

                                    With dgvProduccion.Rows(e.RowIndex)
                                        .Cells(1).Value = nombre
                                        .Cells(2).Value = 1
                                        .Cells(3).Value = precioFormato
                                    End With

                                    CalcularTotalProduccion()
                                    GuardarProduccion()
                                Else
                                    MsgBox("Código de producto no válido.", MsgBoxStyle.Exclamation)
                                    dgvProduccion.Rows.RemoveAt(e.RowIndex)
                                End If
                            End Using
                        End Using
                    End Using

                Catch ex As Exception
                    MsgBox("Error al consultar producto: " & ex.Message)
                End Try
            End If

            ' Si editamos la columna de cantidad
        ElseIf e.ColumnIndex = 2 Then
            Try
                Dim fila = dgvProduccion.Rows(e.RowIndex)
                Dim cantidadTexto As String = fila.Cells(2).Value?.ToString()
                Dim cantidad As Integer = 1
                Integer.TryParse(cantidadTexto, cantidad)
                If cantidad <= 0 Then cantidad = 1

                ' Obtener precio desde el subtotal anterior (dividido entre cantidad anterior)
                Dim subtotalTexto As String = fila.Cells(3).Value?.ToString().Replace(" $", "").Replace(".", "")
                Dim subtotalAnterior As Integer = 0
                Integer.TryParse(subtotalTexto, subtotalAnterior)

                Dim precioUnitario As Integer = subtotalAnterior
                If subtotalAnterior > 0 AndAlso cantidad > 0 Then
                    precioUnitario = subtotalAnterior \ 1 ' Si el valor anterior era 1 unidad
                End If

                ' Calcular nuevo subtotal
                Dim nuevoSubtotal As Integer = cantidad * precioUnitario
                fila.Cells(3).Value = String.Format("{0:N0}", nuevoSubtotal) & " $"

                CalcularTotalProduccion()
                GuardarProduccion()

            Catch ex As Exception
                MsgBox("Error al recalcular subtotal: " & ex.Message, MsgBoxStyle.Critical)
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
        Dim formProductosProduccion As New FormProducc_Productos()
        FormProducc_Productos.Owner = Me
        FormProducc_Productos.ShowDialog()
    End Sub

End Class
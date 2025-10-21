Imports System.Data.OleDb
Imports System.Globalization
Imports System.Linq.Expressions
Imports System.Text.RegularExpressions
Imports System.Drawing.Printing

Public Class Form1
    Private autoGuardadoHabilitado As Boolean = True
    Private idGuiaActual As Integer = -1
    Dim numeroGuia As Integer = 1
    Private cargarGuia As Boolean = False

    Private WithEvents documentoImpresion As New PrintDocument()

    'Creacion de lista de guias para los botones
    Dim listaGuias As New List(Of Integer)
    Dim indiceGuiaActual As Integer = -1

    'Carga del formulario para dar formato a la guia
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Debug.WriteLine(">> Form1_Load")
        cargarGuia = True

        'Preparar tabla de guia
        IniciarTabla()

        ConfigurarColumnasDgvGuias()

        'Cargar guias para seleccion
        GuiasListado()
        With dgvGuias
            .ReadOnly = True
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
        End With

        'Carga de lista de guias
        CargarListaGuias()

        'Carga del ComboBox de los ID Clientes
        CargarClientes()

        'Cargar ultima guia realizada
        CargarUltimaGuia()

        'Fecha del dia de hoy
        'dtpFecha.Value = Date.Today

        'Evitar de que las columnas organicen las filas
        For Each col As DataGridViewColumn In dgvProductos.Columns
            col.SortMode = DataGridViewColumnSortMode.NotSortable
        Next

        cargarGuia = False
    End Sub

    'Carga de Cliente para el ComboBox de Id Clientes
    Private Sub CargarClientes()
        Try
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
            conexionDB.Open()

            Dim cmd As New OleDbCommand("SELECT id_cliente, nombre_fantasia FROM cliente ORDER BY nombre_fantasia", conexionDB)
            Dim reader As OleDbDataReader = cmd.ExecuteReader()

            Dim dtClientes As New DataTable()
            dtClientes.Load(reader)

            cbCodCliente.DataSource = dtClientes
            cbCodCliente.DisplayMember = "nombre_fantasia"
            cbCodCliente.ValueMember = "id_cliente"

            'Colocar Texto vacio en caso de no haber Cliente ID
            cbCodCliente.SelectedIndex = -1

        Catch ex As Exception
            MsgBox("Error al cargar los clientes: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    Private Sub cbCodCliente_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbCodCliente.SelectedIndexChanged
        If Not cargarGuia AndAlso cbCodCliente.SelectedValue IsNot Nothing Then
            Try
                lblNombreCliente.Text = cbCodCliente.Text
                guardarGuia()
            Catch ex As Exception
                MsgBox("Error al seleccionar Cliente: " & ex.Message, MsgBoxStyle.Critical)
            End Try
        End If
    End Sub

    'DETECCION DE CAMBIOS EN EL GRID
    Private Sub dgvProductos_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProductos.CellValueChanged
        If cargarGuia Then Exit Sub
        BeginInvoke(Sub()
                        Try
                            guardarGuia()
                        Catch ex As Exception
                            MsgBox("Error durante el autoguardado o actualizacion: " & ex.Message, MsgBoxStyle.Critical)
                        End Try
                    End Sub)
    End Sub

    Private Sub dgvProductos_RowsRemoved(sender As Object, e As DataGridViewRowsRemovedEventArgs) Handles dgvProductos.RowsRemoved
        If cargarGuia Then Exit Sub
        BeginInvoke(Sub()
                        Try
                            guardarGuia()
                        Catch ex As Exception
                            MsgBox("Error durante el autoguardado o actualizacion:" & ex.Message, MsgBoxStyle.Critical)
                        End Try
                    End Sub)
    End Sub

    'Cargar Grid de Guias (derecha)
    Private Sub GuiasListado(Optional fechaInicio As Date = Nothing, Optional fechaFin As Date = Nothing)
        dgvGuias.Rows.Clear()

        Try
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
            conexionDB.Open()

            Dim sql As String = "SELECT g.id_guia, g.numero_guia, g.fecha, c.nombre_fantasia " &
                                "FROM guia g LEFT JOIN cliente c ON g.id_cliente = c.id_cliente "

            If fechaInicio <> Nothing AndAlso fechaFin <> Nothing Then
                sql &= "WHERE g.fecha BETWEEN ? AND ? "
            End If

            sql &= "ORDER BY g.fecha DESC"

            Using cmd As New OleDbCommand(sql, conexionDB)
                If fechaInicio <> Nothing AndAlso fechaFin <> Nothing Then
                    cmd.Parameters.AddWithValue("?", fechaInicio)
                    cmd.Parameters.AddWithValue("?", fechaFin)
                End If

                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim fechaGuia As String = Convert.ToDateTime(reader("fecha")).ToShortDateString()
                        Dim numeroGuia As String = reader("numero_guia").ToString()
                        Dim nombreCliente As String = If(IsDBNull(reader("nombre_fantasia")), "Sin Nombre", reader("nombre_fantasia").ToString())

                        Dim nuevaFila As Integer = dgvGuias.Rows.Add(fechaGuia, numeroGuia, nombreCliente)
                        dgvGuias.Rows(nuevaFila).Tag = reader("id_guia")
                    End While
                End Using
            End Using

        Catch ex As Exception
            MsgBox("Error al cargar la lista de guias:" & ex.Message, MsgBoxStyle.Critical)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

    End Sub

    Private Sub AutoActualizarGuias()
        ConfigurarColumnasDgvGuias()
        GuiasListado()

        'Selecciona automaticamente la guia actual de la lista lateral
        For Each fila As DataGridViewRow In dgvGuias.Rows
            If Convert.ToInt32(fila.Tag) = idGuiaActual Then
                fila.Selected = True
                dgvGuias.FirstDisplayedScrollingRowIndex = fila.Index
                Exit For
            End If
        Next
    End Sub

    Private Sub ConfigurarColumnasDgvGuias()
        dgvGuias.Columns.Clear()
        dgvGuias.Columns.Add("fecha", "Fecha")
        dgvGuias.Columns.Add("numero_guia", "N° Guía")
        dgvGuias.Columns.Add("nombre_cliente", "Cliente")

        dgvGuias.Columns("fecha").Width = 80
        dgvGuias.Columns("numero_guia").Width = 80
        dgvGuias.Columns("nombre_cliente").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
    End Sub

    Private Sub btnActualizarGuias_Click(sender As Object, e As EventArgs) Handles btnActualizarGuias.Click
        ConfigurarColumnasDgvGuias()
        GuiasListado()
    End Sub

    Private Sub dgvGuias_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvGuias.CellClick
        If e.RowIndex >= 0 AndAlso dgvGuias.Rows(e.RowIndex).Tag IsNot Nothing Then
            Dim idGuiaSeleccionada As Integer = Convert.ToInt32(dgvGuias.Rows(e.RowIndex).Tag)
            CargarGuiaID(idGuiaSeleccionada)
            CargarDetalleGuia(idGuiaSeleccionada)
        End If
    End Sub

    'Carga de lista de guias 
    Private Sub CargarListaGuias()
        Debug.WriteLine(">> CargarListaGuias")

        listaGuias.Clear()
        Try
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
            conexionDB.Open()

            Using cmd As New OleDbCommand("SELECT id_guia FROM guia ORDER BY numero_guia", conexionDB)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        listaGuias.Add(reader("id_guia"))
                    End While
                End Using
            End Using
        Catch ex As Exception
            MsgBox("Error al cargar la lista de guias: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    'Carga de Guia ultima vez usada
    Private Sub CargarUltimaGuia()
        Debug.WriteLine(">> CargarUltimaGuia")
        Try
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
            conexionDB.Open()

            Using cmd As New OleDbCommand("SELECT TOP 1 id_guia FROM guia ORDER BY id_guia DESC", conexionDB)
                Dim result = cmd.ExecuteScalar()
                If result IsNot DBNull.Value AndAlso result IsNot Nothing Then
                    Dim idUltimaGuia As Integer = Convert.ToInt32(result)
                    CargarGuiaID(result)
                End If
            End Using
        Catch ex As Exception
            MsgBox("Error al cargar la ultima pagina: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    'PARA LA CARGA DE DETALLES GUIA!
    Private Sub CargarDetalleGuia(idGuia As Integer)
        Debug.WriteLine(">> CargarndoDetalle con ID = " & idGuia)

        cargarGuia = True
        dgvProductos.Rows.Clear()

        Dim datosGuia As New List(Of Object())

        Try
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
            conexionDB.Open()

            Using cmd As New OleDbCommand("SELECT * FROM guia_detalle WHERE id_guia = ?", conexionDB)
                cmd.Parameters.AddWithValue("?", idGuia)

                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim codigo As Object = If(IsDBNull(reader("codigo_producto")), "", reader("codigo_producto"))
                        Dim nombre As Object = If(IsDBNull(reader("nombre_producto")), "", reader("nombre_producto"))
                        Dim precio As Integer = If(IsDBNull(reader("precio_venta_boleta")), 0, Convert.ToInt32(reader("precio_venta_boleta")))
                        Dim cantidad As Integer = If(IsDBNull(reader("cantidad")), 0, Convert.ToInt32(reader("cantidad")))
                        Dim subtotal As Integer = If(IsDBNull(reader("subtotal")), 0, Convert.ToInt32(reader("subtotal")))

                        datosGuia.Add(New Object() {
                          codigo,
                          nombre,
                          String.Format("{0:N0}", precio) & " $",
                          cantidad,
                          String.Format("{0:N0}", subtotal) & " $"
                        })
                    End While
                End Using
            End Using
        Catch ex As Exception
            MsgBox("Error al cargar la guia: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        For Each fila As Object() In datosGuia
            dgvProductos.Rows.Add(fila)
        Next

        cargarGuia = False
    End Sub

    'Buscar nombre de cliente ID
    Private Sub BuscarNombreClienteID(idCliente As Integer)
        Debug.WriteLine(">> BuscarNombreClienteID con ID = " & idCliente)
        Try
            Dim ruta As String = Application.StartupPath & "\DBApp\rjc_prod_f753.mdb"
            Dim conexionDatos As New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & ruta)

            conexionDatos.Open()

            Using cmd As New OleDbCommand("SELECT nombre_fantasia FROM cliente WHERE id_cliente = ?", conexionDatos)
                cmd.Parameters.AddWithValue("?", idCliente)
                Dim nombre = cmd.ExecuteScalar()
                lblNombreCliente.Text = If(nombre IsNot Nothing, nombre.ToString(), "Cliente no encontrado")
            End Using

            conexionDatos.Close()
        Catch ex As Exception
            lblNombreCliente.Text = "Error al buscar el cliente"
            Debug.WriteLine("Error en Busqueda del nombre con ID: " & ex.Message)
        End Try
    End Sub

    Private Sub btnSiguiente_Click(sender As Object, e As EventArgs) Handles btnSiguiente.Click
        Try
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
            conexionDB.Open()

            Using cmd As New OleDbCommand("SELECT MIN(id_guia) FROM guia WHERE id_guia > ?", conexionDB)
                cmd.Parameters.AddWithValue("?", idGuiaActual)
                Dim siguienteID = cmd.ExecuteScalar()

                If siguienteID IsNot DBNull.Value AndAlso siguienteID IsNot Nothing Then
                    CargarGuiaID(Convert.ToInt32(siguienteID))
                Else
                    MsgBox("No hay mas guias siguientes!")
                End If

                If Not Integer.TryParse(idGuiaActual.ToString(), Nothing) Then
                    MsgBox("El ID de la Guia no es valida.")
                    Exit Sub
                End If
            End Using

        Catch ex As Exception
            MsgBox("Error al buscar guias siguientes: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    Private Sub btnAnterior_Click(sender As Object, e As EventArgs) Handles btnAnterior.Click
        Try
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
            conexionDB.Open()

            Using cmd As New OleDbCommand("SELECT MAX(id_guia) FROM guia WHERE id_guia < ?", conexionDB)
                cmd.Parameters.AddWithValue("?", idGuiaActual)
                Dim anteriorID = cmd.ExecuteScalar()
                If anteriorID IsNot DBNull.Value AndAlso anteriorID IsNot Nothing Then
                    CargarGuiaID(Convert.ToInt32(anteriorID))
                Else
                    MsgBox("No hay mas guias anteriores!")
                End If

                If Not Integer.TryParse(idGuiaActual.ToString(), Nothing) Then
                    MsgBox("El ID de la Guia no es valida.")
                    Exit Sub
                End If
            End Using
        Catch ex As Exception
            MsgBox("Error al buscar guias anteriores: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    Public Sub CargarGuiaID(idGuia As Integer)
        Debug.WriteLine(">> CargarGuiaID con ID = " & idGuia)

        cargarGuia = True
        'Dim numeroGuiaActual As String = ""
        'Dim fechaActual As Date = Date.Today
        'Dim idClienteActual As Integer = 0
        'Dim totalActual As Integer = 0

        Try
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
            conexionDB.Open()

            Using cmdGuia As New OleDbCommand("SELECT * FROM guia WHERE id_guia = ?", conexionDB)
                cmdGuia.Parameters.AddWithValue("?", idGuia)

                Using reader = cmdGuia.ExecuteReader()
                    If reader.Read() Then
                        idGuiaActual = idGuia
                        txtNumeroGuia.Text = reader("numero_guia").ToString()
                        dtpFecha.Value = Convert.ToDateTime(reader("fecha"))
                        cbCodCliente.SelectedValue = Convert.ToInt32(reader("id_cliente"))
                        'txtCodigoCliente.Text = reader("id_cliente").ToString()
                        'BuscarNombreClienteID(Convert.ToInt32(txtCodigoCliente.Text))

                        lblNombreCliente.Text = cbCodCliente.Text

                        'Dim clienteID = Convert.ToInt32(reader("id_cliente"))
                        'Using cmdCliente As New OleDbCommand("SELECT nombre_fantasia FROM cliente WHERE id_cliente = ?", conexionDB)
                        'cmdCliente.Parameters.AddWithValue("?", clienteID)
                        'Dim 'nombre = cmdCliente.ExecuteScalar()
                        'lblNombreCliente.Text = If(nombre IsNot Nothing, nombre.ToString(), "Cliente no Encontrado")
                        'End Using

                        txtTotal.Text = String.Format("{0:N0}", reader("total")) & " $"
                    End If
                End Using
            End Using

            CargarDetalleGuia(idGuia)

        Catch ex As Exception
            MsgBox("Error al cargar la guia: " & ex.Message)
            Return
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    Private Sub dgvProductos_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProductos.CellEndEdit
        If e.ColumnIndex = 0 Then

            Dim codigo As String = dgvProductos.Rows(e.RowIndex).Cells(0).Value?.ToString()

            If Not String.IsNullOrEmpty(codigo) Then
                Try
                    Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                        cmd.Parameters.AddWithValue("?", codigo)
                        conexionDB.Open()

                        Using reader As OleDbDataReader = cmd.ExecuteReader()
                            If reader.Read() Then
                                Dim nombre As String = reader("nombre").ToString()
                                Dim precioBoleta As Integer = Convert.ToInt32(reader("precio_venta_boleta"))
                                Dim precioFormato As String = String.Format("{0:N0}", precioBoleta) & " $"

                                With dgvProductos.Rows(e.RowIndex)
                                    .Cells(1).Value = nombre
                                    .Cells(2).Value = precioFormato
                                    .Cells(3).Value = 1
                                    .Cells(4).Value = String.Format("{0:N0}", precioBoleta) & " $"
                                End With

                                CalcularTotal()

                            Else
                                MsgBox("Codigo de producto no valido: " & MsgBoxStyle.Exclamation)
                                dgvProductos.Rows.RemoveAt(e.RowIndex)
                            End If
                        End Using
                    End Using
                Catch ex As Exception
                    MsgBox("Error al consultar producto: " & ex.Message)
                Finally
                    If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
                End Try
            End If

        ElseIf e.ColumnIndex = 3 Then
            Dim cantidad = Val(dgvProductos.Rows(e.RowIndex).Cells(3).Value)

            Dim precioTexto = dgvProductos.Rows(e.RowIndex).Cells(2).Value?.ToString().Replace(" $", "").Replace(".", "")
            Dim precio = Val(precioTexto)

            Dim subtotal = cantidad * precio
            dgvProductos.Rows(e.RowIndex).Cells(4).Value = String.Format("{0:N0}", subtotal) & " $"

            CalcularTotal()
        End If
    End Sub

    'BOTON PARA NUEVAS GUIAS
    Private Sub btnNuevaGuia_Click(sender As Object, e As EventArgs) Handles btnNuevaGuia.Click
        dgvProductos.Rows.Clear()
        txtNumeroGuia.Text = ObtenerSiguienteNumeroGuia().ToString()
        lblNombreCliente.Text = ""
        idGuiaActual = 0

        cbCodCliente.SelectedIndex = -1
        lblNombreCliente.Text = ""

        'Esto permitira que la guia sea creada con la fecha actual de la PC
        dtpFecha.Value = Date.Today

        'Guardar la guia 
        guardarGuia()

        'Actualizar lista lateral
        AutoActualizarGuias()

    End Sub

    'PARA EL SIGUIENTE NUMERO DE LA GUIA (AHORA EN BASE AL MAS RECIENTE)
    Private Function ObtenerSiguienteNumeroGuia() As Integer
        Try

            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
            conexionDB.Open()

            Dim sql As String = "SELECT TOP 1 numero_guia FROM guia ORDER BY id_guia DESC"
            Using cmd As New OleDbCommand(sql, conexionDB)
                Dim result = cmd.ExecuteScalar()
                If result IsNot DBNull.Value AndAlso result IsNot Nothing Then
                    Return Convert.ToInt32(result) + 1
                Else
                    Return 1
                End If
            End Using

        Catch ex As Exception
            MsgBox("Error al obtener la siguiente guia: " & ex.Message, MsgBoxStyle.Critical)
            Return 1
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Function

    'Calcular el total de los subtotal
    Public Sub CalcularTotal()
        Dim total = ObtenerTotalGuia()
        Dim formato As New CultureInfo("es-ES")
        formato.NumberFormat.NumberGroupSeparator = "."
        txtTotal.Text = total.ToString("N0", formato)
    End Sub

    'OBTENER EL TOTAL DE LOS SUBTOTAL
    Private Function ObtenerTotalGuia() As Integer
        Dim total As Integer = 0

        For Each fila As DataGridViewRow In dgvProductos.Rows
            If Not fila.IsNewRow Then
                Dim subtotalTexto = fila.Cells(4).Value?.ToString().Replace(".", "")
                total += Val(subtotalTexto)
            End If
        Next
        Return total
    End Function

    'GUARDADO DE GUIAS NUEVO
    Public Sub guardarGuia()
        If cbCodCliente.SelectedValue Is Nothing OrElse dgvProductos.Rows.Count = 0 Then Exit Sub

        Try
            conexionDB.Open()

            Dim fecha As Date = dtpFecha.Value
            Dim total As Integer = ObtenerTotalGuia()
            Dim idCliente As Integer = Convert.ToInt32(cbCodCliente.SelectedValue)
            Dim numeroGuia As Integer = Convert.ToInt32(txtNumeroGuia.Text)

            'En caso de ya existir la guia, actualizar
            If idGuiaActual > 0 Then
                Using cmdUpdate As New OleDbCommand("UPDATE guia SET fecha = ?, total = ?, id_cliente = ?, numero_guia = ? WHERE id_guia = ?", conexionDB)
                    cmdUpdate.Parameters.AddWithValue("?", fecha)
                    cmdUpdate.Parameters.AddWithValue("?", total)
                    cmdUpdate.Parameters.AddWithValue("?", idCliente)
                    cmdUpdate.Parameters.AddWithValue("?", numeroGuia)
                    cmdUpdate.Parameters.AddWithValue("?", idGuiaActual)
                    cmdUpdate.ExecuteNonQuery()
                End Using

                'Eliminar Detalles antiguos en caso de cambios en la guia
                Using cmdDelete As New OleDbCommand("DELETE FROM guia_detalle WHERE id_guia = ?", conexionDB)
                    cmdDelete.Parameters.AddWithValue("?", idGuiaActual)
                    cmdDelete.ExecuteNonQuery()
                End Using

            Else
                'En caso de no existir, crear nueva guia
                Using cmdInsertar As New OleDbCommand("INSERT INTO guia (fecha, total, id_cliente, numero_guia) VALUES (?,?,?,?)", conexionDB)
                    cmdInsertar.Parameters.AddWithValue("?", fecha)
                    cmdInsertar.Parameters.AddWithValue("?", total)
                    cmdInsertar.Parameters.AddWithValue("?", idCliente)
                    cmdInsertar.Parameters.AddWithValue("?", numeroGuia)
                    cmdInsertar.ExecuteNonQuery()

                    'Obtencion de la ID de manera automatica
                    Using cmdID As New OleDbCommand("SELECT @@IDENTITY", conexionDB)
                        idGuiaActual = Convert.ToInt32(cmdID.ExecuteScalar())
                    End Using
                End Using
            End If

            'Insercion de nuevos detalles
            For Each fila As DataGridViewRow In dgvProductos.Rows

                If fila.IsNewRow Then Continue For

                If fila.Cells("codigo").Value Is Nothing OrElse
                   fila.Cells("nombre").Value Is Nothing OrElse
                   fila.Cells("cantidad").Value Is Nothing OrElse
                   fila.Cells("precio").Value Is Nothing OrElse
                   fila.Cells("subtotal").Value Is Nothing Then Continue For

                Dim codigo = fila.Cells("codigo").Value?.ToString()
                Dim nombre = fila.Cells("nombre").Value?.ToString()
                Dim cantidad = Convert.ToInt32(fila.Cells("cantidad").Value)
                Dim precioUnitario = LimpiarMonto(fila.Cells("precio").Value)
                Dim subtotal = LimpiarMonto(fila.Cells("subtotal").Value)

                Using cmdDetalle As New OleDbCommand("INSERT INTO guia_detalle (id_guia, codigo_producto, nombre_producto, cantidad, precio_venta_boleta, subtotal) VALUES (?,?,?,?,?,?)", conexionDB)
                    cmdDetalle.Parameters.AddWithValue("?", idGuiaActual)
                    cmdDetalle.Parameters.AddWithValue("?", codigo)
                    cmdDetalle.Parameters.AddWithValue("?", nombre)
                    cmdDetalle.Parameters.AddWithValue("?", cantidad)
                    cmdDetalle.Parameters.AddWithValue("?", precioUnitario)
                    cmdDetalle.Parameters.AddWithValue("?", subtotal)
                    cmdDetalle.ExecuteNonQuery()
                End Using
            Next

            'Actualizacion Automatica de la lista de guias
            AutoActualizarGuias()

        Catch ex As Exception
            MsgBox("Error al guardar/actualizar Guia: ", ex.Message, MsgBoxStyle.Critical)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    'LIMPIEZA DE LOS SIMBOLOS EN LOS INTEGER
    Private Function LimpiarMonto(monto As Object) As Integer
        If monto Is Nothing OrElse String.IsNullOrWhiteSpace(monto.ToString()) Then Return 0
        Return Convert.ToInt32(monto.ToString().Replace(" $", "").Replace(".", ""))
    End Function

    'inicializacion de la tabla guia
    Private Sub IniciarTabla()
        Debug.WriteLine(">> IniciarTabla")
        With dgvProductos
            'Agregado de las Columnas de Productos
            .Columns.Clear()
            .Columns.Add("codigo", "Codigo Producto")
            .Columns.Add("nombre", "Nombre Producto")
            .Columns.Add("precio", "Precio Producto")
            .Columns.Add("cantidad", "Cantidad")
            .Columns.Add("subtotal", "Sub. Total")

            'Colocar las Columnas en modo Read-Only
            .Columns("nombre").ReadOnly = True
            .Columns("precio").ReadOnly = True
            .Columns("subtotal").ReadOnly = True

            'Boton para eliminar las columnas (agregado como columna extra)
            Dim eliminarBtn As New DataGridViewButtonColumn()
            eliminarBtn.Name = "Eliminar"
            eliminarBtn.HeaderText = ""
            eliminarBtn.Text = "Eliminar"
            eliminarBtn.UseColumnTextForButtonValue = True
            .Columns.Add(eliminarBtn)
        End With
    End Sub

    Private Sub ValidarEntradaNumerica(sender As Object, e As KeyPressEventArgs)
        Dim txt As TextBox = CType(sender, TextBox)

        'Permitir Teclas de Retroceso y demas
        If Char.IsControl(e.KeyChar) Then Return

        'Permitir Solo numeros
        If Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
            Return
        End If

        'Limitar a los 5 caracteres 
        If txt.TextLength >= 5 Then
            e.Handled = True
        End If

    End Sub

    Private Sub dgvProductos_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles dgvProductos.EditingControlShowing
        ' Solo afecta para las columnas de Codigo y Cantidad
        If dgvProductos.CurrentCell.ColumnIndex = 0 Or dgvProductos.CurrentCell.ColumnIndex = 3 Then
            Dim txtEdit As TextBox = TryCast(e.Control, TextBox)

            If txtEdit IsNot Nothing Then
                'Evitar acumulacion de numeros
                RemoveHandler txtEdit.KeyPress, AddressOf ValidarEntradaNumerica
                AddHandler txtEdit.KeyPress, AddressOf ValidarEntradaNumerica
            End If
        End If
    End Sub

    Private Sub dgvProductos_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProductos.CellClick
        If e.ColumnIndex = dgvProductos.Columns("Eliminar").Index AndAlso e.RowIndex >= 0 Then
            If Not dgvProductos.Rows(e.RowIndex).IsNewRow Then
                dgvProductos.Rows.RemoveAt(e.RowIndex)
                CalcularTotal()
            End If
        End If
    End Sub

    Private Sub txtNumeroGuia_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtNumeroGuia.KeyPress
        'Solo Numero y borrado del mismo 
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub txtNumeroGuia_Leave(sender As Object, e As EventArgs) Handles txtNumeroGuia.Leave
        If Not cargarGuia Then
            Try
                Dim nuevoNumero As Integer
                If Integer.TryParse(txtNumeroGuia.Text, nuevoNumero) Then
                    'EN CASO DE EXISTIR UN NUMERO DE GUIA IGUAL
                    If GuiaNumeroIgual(nuevoNumero, idGuiaActual) Then
                        MsgBox("Ya existe una guia con ese numero!", MsgBoxStyle.Exclamation)
                        CargarGuiaID(idGuiaActual)
                        Exit Sub
                    End If

                    guardarGuia()
                Else
                    MsgBox("Numero de Guia invalido!", MsgBoxStyle.Exclamation)
                End If
            Catch ex As Exception
                MsgBox("Error al actualizar el numero de guia!" & ex.Message, MsgBoxStyle.Critical)
            End Try
        End If
    End Sub

    'EN CASO DE PRESIONAR ENTER AL CAMBIAR EL NUMERO DE GUIA
    Private Sub txtNumeroGuia_KeyDown(sender As Object, e As KeyEventArgs) Handles txtNumeroGuia.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            txtNumeroGuia_Leave(sender, e)
        End If
    End Sub

    'FUNCION EN CASO DE EXISTIR UN NUMERO DE GUIA IGUAL
    Private Function GuiaNumeroIgual(numeroGuia As Integer, Optional idExcluido As Integer = -1) As Boolean
        Try
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
            conexionDB.Open()

            Dim sql As String = "SELECT COUNT(*) FROM guia WHERE numero_guia = ?"
            If idExcluido > 0 Then
                sql &= " AND id_guia <> ?"
            End If

            Using cmd As New OleDbCommand(sql, conexionDB)
                cmd.Parameters.AddWithValue("?", numeroGuia)
                If idExcluido > 0 Then
                    cmd.Parameters.AddWithValue("?", idExcluido)
                End If

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                Return count > 0
            End Using

        Catch ex As Exception
            Debug.WriteLine("Error verificando numero de guia duplicada: " & ex.Message)
            Return False
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Function

    'BOTONES DE RETORNO Y AVANCE PARA VER GUIAS
    Private Sub btnAgregarProducto_Click(sender As Object, e As EventArgs) Handles btnAgregarProducto.Click
        Dim formProductos As New FormProductos()
        formProductos.ShowDialog() 'Mostrar el Formulario
    End Sub

    'SECCION PARA IMPRESION (Click para imprimir en papel!)
    Private Sub btnImprimir_Click(sender As Object, e As EventArgs) Handles btnImprimir.Click
        Try
            ' Tamaño del papel del ticket
            Dim tamañoPapel As New PaperSize("Ticket80mm", 283, 1169)
            documentoImpresion.DefaultPageSettings.PaperSize = tamañoPapel

            ' Margen para el espacio
            documentoImpresion.DefaultPageSettings.Margins = New Margins(5, 5, 5, 5)

            ' Fuente estandar para tickets
            fuenteCuerpo = New Font("Consolas", 7)

            ' Inicia la impresión
            ' (Aviso, en caso de desear ver el formato sin imprimir, comentar el codigo
            ' de abajo!)
            documentoImpresion.Print()

            Using vistaPrevia As New PrintPreviewDialog()
                vistaPrevia.Document = documentoImpresion
                vistaPrevia.WindowState = FormWindowState.Maximized
                vistaPrevia.PrintPreviewControl.Zoom = 2.0 'Escala de ventana del 200% CUIDADO!
                vistaPrevia.Text = "Vista Previa de Guia" & txtNumeroGuia.Text
                vistaPrevia.ShowDialog()
            End Using

            'Error en caso de no lograrse imprimir
        Catch ex As Exception
            MsgBox("Error al mostrar la vista previa de la guia!" & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    'Posicion y Fuente para la impresion
    Dim fuenteTitulo As New Font("Consolas", 8, FontStyle.Bold)
    Dim fuenteCuerpo As New Font("Consolas", 7)
    Dim yPos As Integer = 0

    'Impresion del Ticket
    Private Sub documentoImpresion_PrintPage(sender As Object, e As PrintPageEventArgs) Handles documentoImpresion.PrintPage
        'Elementos graficos, como de margenes con fuentes de la impresion
        Dim g As Graphics = e.Graphics
        Dim margenIzq As Integer = e.MarginBounds.Left
        yPos = e.MarginBounds.Top
        Dim fuenteTitulo As New Font("Consolas", 8, FontStyle.Bold)
        Dim fuenteCuerpo As New Font("Consolas", 7)
        Dim salto As Integer = fuenteCuerpo.GetHeight(g) + 2

        'OBTENER DATOS DEL CLIENTE 
        Dim razonSocial As String = ""
        Dim rutFormato As String = ""
        Dim direccionLocal As String = ""

        'CONEXION A LA BASE DE DATOS
        Try
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
            conexionDB.Open()

            Dim sqlCliente As String = "SELECT razon_social, rut, dvr, direccion, comuna FROM cliente WHERE id_cliente = ?"

            Using cmd As New OleDbCommand(sqlCliente, conexionDB)
                If cbCodCliente.SelectedValue IsNot Nothing Then
                    cmd.Parameters.AddWithValue("?", cbCodCliente.SelectedValue)
                Else
                    Exit Try
                End If

                Using reader = cmd.ExecuteReader()
                    If reader.Read() Then
                        razonSocial = reader("razon_social").ToString()
                        rutFormato = Format(CInt(reader("rut")), "##,###,###").Replace(",", ".") & " - " & reader("dvr").ToString()
                        direccionLocal = reader("direccion").ToString() & " - " & reader("comuna").ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            razonSocial = "Error de Cliente: No Encontrado! "
            rutFormato = "-"
            direccionLocal = "-"
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        ' ENCABEZADO FIJO
        g.DrawString("Guía de Despacho Interna", fuenteTitulo, Brushes.Black, margenIzq, yPos) : yPos += salto
        g.DrawString(razonSocial, fuenteCuerpo, Brushes.Black, margenIzq, yPos) : yPos += salto
        g.DrawString(rutFormato, fuenteCuerpo, Brushes.Black, margenIzq, yPos) : yPos += salto
        g.DrawString(direccionLocal, fuenteCuerpo, Brushes.Black, margenIzq, yPos) : yPos += salto
        g.DrawString(dtpFecha.Value.ToString("dd/mm/yyyy"), fuenteCuerpo, Brushes.Black, margenIzq, yPos) : yPos += salto
        yPos += salto

        ' N° Guía
        g.DrawString("N°Guia: " & txtNumeroGuia.Text, fuenteCuerpo, Brushes.Black, margenIzq, yPos) : yPos += salto
        yPos += salto

        ' CABECERA DE TABLA
        g.DrawString("Nombre                     Cant  Precio  Subtotal", fuenteCuerpo, Brushes.Black, margenIzq, yPos)
        yPos += salto

        ' CONTENIDO DE LA GUIA

        For Each fila As DataGridViewRow In dgvProductos.Rows
            If fila.IsNewRow Then Continue For
            ' Alineación formateada tipo consola
            Dim nombre As String = If(fila.Cells("nombre").Value IsNot Nothing, fila.Cells("nombre").Value.ToString(), "")
            Dim cantidad As Integer = Val(fila.Cells("cantidad").Value?.ToString())
            Dim precio As Integer = Val(fila.Cells("precio").Value?.ToString().Replace(" $", "").Replace(".", ""))
            Dim subtotal As Integer = Val(fila.Cells("subtotal").Value?.ToString().Replace(" $", "").Replace(".", ""))

            Dim linea As String = nombre.PadRight(26)
            linea &= cantidad.ToString().PadLeft(4) & " "
            linea &= precio.ToString("N0").PadLeft(8) & " "
            linea &= subtotal.ToString("N0").PadLeft(8)
            g.DrawString(linea, fuenteCuerpo, Brushes.Black, margenIzq, yPos)
            yPos += salto
        Next

        yPos += salto
        g.DrawString("--------------------------------------------------", fuenteCuerpo, Brushes.Black, margenIzq, yPos)
        yPos += salto
        g.DrawString("Total:".PadLeft(33) & txtTotal.Text.Replace(" ", ""), fuenteTitulo, Brushes.Black, margenIzq, yPos)
        yPos += salto
    End Sub

    'LLEVAR A LA VENTANA DE PRODUCCION
    Private Sub btnProduccion_Click(sender As Object, e As EventArgs) Handles btnProduccion.Click
        'En caso de existir una ventana abierta, no abrir otra
        For Each frm As Form In Application.OpenForms
            If TypeOf frm Is Form2 Then
                frm.Focus() 'Mover hacia el frente de la pantalla si la ventana ya esta abierta
                frm.WindowState = FormWindowState.Normal 'Mostrar la ventana de produccion con normalidad
                frm.BringToFront() 'Traer al frente de la pantalla
            End If
        Next

        'Si la ventana no esta abierta, abrir la misma
        Dim ventanaProduccion As New Form2() 'Declarar la ventana de produccion
        ventanaProduccion.Show() 'Mostrar la ventana de produccion
    End Sub
End Class

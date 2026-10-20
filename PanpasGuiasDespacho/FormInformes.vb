Option Strict On

Imports System.Data.OleDb
Imports System.Drawing.Printing
Imports System.Security.Cryptography
Imports System.Security.Policy
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

'Identificador de tipo de informe
Public Enum TipoInforme
    Ninguno = 0
    CorrelativoGuias = 1
    VentaPorLocales = 2
    VentaPorProductos = 3
    ProduccionProductos = 4
    ResumenProduccion = 5
End Enum

Public Class FormInformes

    'Deteccion del informe actual
    Private informeActual As TipoInforme = TipoInforme.Ninguno

    Private Sub FormInformes_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CenterToScreen()

        'Meses en el combobox de Meses y Años
        cmbMes.Items.AddRange(New Object() {
            "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
        })
        cmbMes.SelectedIndex = Date.Now.Month - 1

        'Años de combobox de Meses y Años
        For i As Integer = Date.Now.Year - 9 To Date.Now.Year
            cmbAnio.Items.Add(i)
        Next
        cmbAnio.SelectedItem = Date.Now.Year

        'Dejar "Entre fechas" como predeterminado
        rdbEntreFechas.Checked = True
        ActualizarEstadoFiltros()

        AddHandler rdbEntreFechas.CheckedChanged, AddressOf ModoFecha_Changed
        AddHandler rdbPorMes.CheckedChanged, AddressOf ModoFecha_Changed


    End Sub

    Private Function ValorCelda(fila As DataGridViewRow, ParamArray nombres() As String) As String
        For Each n In nombres
            If dgvResultados.Columns.Contains(n) Then
                Dim v = fila.Cells(n).Value
                If v IsNot Nothing AndAlso Not IsDBNull(v) Then
                    Return v.ToString()
                End If
            End If
        Next
        Return ""
    End Function

    Private Sub ModoFecha_Changed(sender As Object, e As EventArgs)
        ActualizarEstadoFiltros()
    End Sub

    'Habilitar o deshabilitar los campos seleccionados
    Private Sub ActualizarEstadoFiltros()
        If rdbEntreFechas.Checked Then
            dtpDesde.Enabled = True
            dtpHasta.Enabled = True
            cmbMes.Enabled = False
            cmbAnio.Enabled = False
        ElseIf rdbPorMes.Checked Then
            dtpDesde.Enabled = False
            dtpHasta.Enabled = False
            cmbMes.Enabled = True
            cmbAnio.Enabled = True
        End If
    End Sub

    Public Function ObtenerRangoFechas() As Tuple(Of Date, Date)
        Dim desde As Date
        Dim hasta As Date

        If rdbEntreFechas.Checked Then
            desde = dtpDesde.Value.Date
            hasta = dtpHasta.Value.Date
        ElseIf rdbPorMes.Checked Then
            If cmbMes.SelectedIndex < 0 OrElse cmbAnio.SelectedItem Is Nothing Then
                Throw New InvalidOperationException("Selecciona el mes y el año del informe.")
            End If

            Dim SeleccionMes As Integer = cmbMes.SelectedIndex + 1
            Dim SeleccionAnio As Integer = CInt(cmbAnio.SelectedItem)
            desde = New Date(SeleccionAnio, SeleccionMes, 1)
            hasta = desde.AddMonths(1).AddDays(-1)
        Else
            desde = DateTime.Now.Date
            hasta = DateTime.Now.Date
        End If
        Return Tuple.Create(desde, hasta)
    End Function

    'Botones de informes 
    Private Sub btnCorrelativoGuias_Click(sender As Object, e As EventArgs) Handles btnCorrelativoGuias.Click
        informeActual = TipoInforme.CorrelativoGuias

        Try
            Dim fechas As Tuple(Of Date, Date) = ObtenerRangoFechas()
            Dim desde As Date = fechas.Item1
            Dim hasta As Date = fechas.Item2

            Dim consultaSQL As String = "
            SELECT 
                g.numero_guia AS NumeroGuia,
                g.fecha AS Fecha,
                c.nombre_fantasia AS Cliente,
                g.total AS Total
            FROM guia AS g
            LEFT JOIN cliente AS c ON g.id_cliente = c.id_cliente
            WHERE g.fecha BETWEEN ? AND ?
            ORDER BY g.numero_guia;"

            AbrirConexion()

            Using cmd As New OleDb.OleDbCommand(consultaSQL, conexionDB)
                cmd.Parameters.AddWithValue("?", desde)
                cmd.Parameters.AddWithValue("?", hasta)

                Dim adaptar As New OleDb.OleDbDataAdapter(cmd)
                Dim tabla As New DataTable()
                adaptar.Fill(tabla)

                dgvResultados.DataSource = tabla
                'Entrega de formato de columnas de titulo
                FormatoGuiayProduccion()

                If dgvResultados.Columns.Contains("total") Then
                    dgvResultados.Columns("total").DefaultCellStyle.Format = "C0"
                    dgvResultados.Columns("total").DefaultCellStyle.FormatProvider = Globalization.CultureInfo.CreateSpecificCulture("es-CL")
                End If

                Dim totalGeneral As Decimal = 0
                For Each fila As DataGridViewRow In dgvResultados.Rows
                    If Not IsDBNull(fila.Cells("total").Value) AndAlso IsNumeric(fila.Cells("total").Value) Then
                        totalGeneral += Convert.ToDecimal(fila.Cells("total").Value)
                    End If
                Next
                txtTotalInforme.Text = totalGeneral.ToString("C0", Globalization.CultureInfo.CreateSpecificCulture("es-CL"))

                'Ventana de confirmacion
                MessageBox.Show($"Se encontraron {tabla.Rows.Count} guías entre {desde:d} y {hasta:d}.",
                            "Correlativo de Guias", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using

            CerrarConexion()

        Catch ex As Exception
            MessageBox.Show("Error al generar el informe: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnResumenProduccion_Click(sender As Object, e As EventArgs) Handles btnResumenProduccion.Click
        informeActual = TipoInforme.ResumenProduccion

        Dim selector As New FormResumenProduccion()

        If selector.ShowDialog() = DialogResult.OK Then

            ' Si se selecciona "Todos"
            If selector.EmpleadoID = 0 Then
                CargarTodaLaProduccion()
                Exit Sub
            End If

            ' Si se selecciona un empleado específico
            Dim IDEmpleado As Integer = selector.EmpleadoID
            CargarProduccionPorEmpleado(IDEmpleado)

        End If
    End Sub

    Private Sub CargarTodaLaProduccion()
        Try
            'Apertura a la DB
            AbrirConexion()

            'Se obtiene el rango de fecha a traves de los selectores de fecha
            Dim fechas = ObtenerRangoFechas()
            Dim desde As Date = fechas.Item1
            Dim hasta As Date = fechas.Item2

            'Se usa la siguiente consulta SQL para traer toda la producciones realizadas 
            Dim consultaSQL As String = "SELECT p.id_produccion,
                p.fecha_produccion,
                e.nombre_empleado,
                p.comentario,
                p.total
         FROM produccion AS p
         LEFT JOIN empleado AS e ON p.id_empleado = e.id_empleado
         WHERE p.fecha_produccion BETWEEN ? AND ?
         ORDER BY p.fecha_produccion DESC"

            'Se genera la tabla DataGrid para mostrar los datos consultados en la consulta SQL
            Dim tabla As New DataTable()

            'Se determina la fecha desde y hasta de las producciones
            Using cmd As New OleDbCommand(consultaSQL, conexionDB)
                cmd.Parameters.AddWithValue("?", desde)
                cmd.Parameters.AddWithValue("?", hasta)
                tabla.Load(cmd.ExecuteReader())
            End Using

            'Se muestra el resultado final
            dgvResultados.DataSource = tabla

            'Se entrega en los titulos de las columnas sus nombres determinados para su identificacion
            FormatoGuiayProduccion()

            'Se entrega el total de los subtotal, con el formato hispanico adecuado para su muestra adecuada
            Dim totalGeneral As Decimal = 0
            For Each fila As DataGridViewRow In dgvResultados.Rows
                If Not IsDBNull(fila.Cells("total").Value) Then
                    totalGeneral += Convert.ToDecimal(fila.Cells("total").Value)
                End If
            Next
            txtTotalInforme.Text = totalGeneral.ToString("C0", Globalization.CultureInfo.CreateSpecificCulture("es-CL"))

            MessageBox.Show($"Se encontraron {tabla.Rows.Count} registros de producción entre {desde:d} y {hasta:d}.",
                            "Producción por Empleado", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error al cargar produccion: " & ex.Message)
        Finally
            CerrarConexion()
        End Try
    End Sub

    Private Sub CargarProduccionPorEmpleado(idEmpleado As Integer)
        Try
            AbrirConexion()

            Dim rango = ObtenerRangoFechas()
            Dim desde As Date = rango.Item1
            Dim hasta As Date = rango.Item2

            Dim consultaSQL As String =
                    "SELECT 
                p.fecha_produccion,
                e.nombre_empleado,
                d.codigo_producto,
                d.nombre_producto,
                d.cantidad,
                d.precio_unitario,
                d.subtotal
             FROM (produccion AS p
             INNER JOIN produccion_detalle AS d 
                    ON p.id_produccion = d.id_produccion)
             LEFT JOIN empleado AS e 
                    ON p.id_empleado = e.id_empleado
             WHERE p.id_empleado = ?
               AND p.fecha_produccion BETWEEN ? AND ?
             ORDER BY p.fecha_produccion DESC"

            Dim tabla As New DataTable()

            Using cmd As New OleDbCommand(consultaSQL, conexionDB)
                cmd.Parameters.AddWithValue("?", idEmpleado)
                cmd.Parameters.AddWithValue("?", desde)
                cmd.Parameters.AddWithValue("?", hasta)
                tabla.Load(cmd.ExecuteReader())
            End Using

            dgvResultados.DataSource = tabla
            FormatoGuiayProduccion()

            ' Calcular total general
            Dim totalGeneral As Decimal = 0
            For Each fila As DataGridViewRow In dgvResultados.Rows
                If Not IsDBNull(fila.Cells("subtotal").Value) Then
                    totalGeneral += Convert.ToDecimal(fila.Cells("subtotal").Value)
                End If
            Next

            txtTotalInforme.Text =
            totalGeneral.ToString("C0", Globalization.CultureInfo.CreateSpecificCulture("es-CL"))

            ' Mensaje de aviso
            MessageBox.Show(
            $"Se encontraron {tabla.Rows.Count} registros de producción del empleado seleccionado entre {desde:d} y {hasta:d}.",
            "Producción por Empleado",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        )

        Catch ex As Exception
            MessageBox.Show("Error al cargar producción por empleado: " & ex.Message)
        Finally
            CerrarConexion()
        End Try
    End Sub

    Private Sub btnVentasLocales_Click(sender As Object, e As EventArgs) Handles btnVentasLocales.Click
        informeActual = TipoInforme.VentaPorLocales

        Dim selector As New FormLocal()

        If selector.ShowDialog() = DialogResult.OK Then
            If selector.LocalID = 0 Then
                CargarTodasLasGuias()
                Exit Sub
            End If
        End If

        Dim LocalID As Integer = selector.LocalID
        CargaInformeLocal(LocalID)

    End Sub

    Private Sub CargarTodasLasGuias()
        Try
            AbrirConexion()

            Dim fechas = ObtenerRangoFechas()
            Dim desde As Date = fechas.Item1
            Dim hasta As Date = fechas.Item2

            Dim consultaSQL As String = "SELECT g.numero_guia AS NumeroGuia,
                g.fecha AS Fecha,
                c.nombre_fantasia AS Cliente,
                g.total AS Total
                 FROM guia AS g
                 LEFT JOIN cliente AS c ON g.id_cliente = c.id_cliente
                 WHERE g.fecha BETWEEN ? AND ?
                 ORDER BY g.fecha DESC"

            Dim tabla As New DataTable()

            Using cmd As New OleDbCommand(consultaSQL, conexionDB)
                cmd.Parameters.AddWithValue("?", desde)
                cmd.Parameters.AddWithValue("?", hasta)
                tabla.Load(cmd.ExecuteReader())
            End Using

            dgvResultados.DataSource = tabla

            FormatoGuiayProduccion()

            Dim totalGeneral As Decimal = 0
            For Each fila As DataGridViewRow In dgvResultados.Rows
                If Not IsDBNull(fila.Cells("Total").Value) Then
                    totalGeneral += Convert.ToDecimal(fila.Cells("Total").Value)
                End If
            Next

            txtTotalInforme.Text = totalGeneral.ToString("C0", Globalization.CultureInfo.CreateSpecificCulture("es-CL"))

            MessageBox.Show($"Se encontraron {tabla.Rows.Count} guías entre {desde:d} y {hasta:d}.",
                            "Ventas por Locales", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error al cargar todas las guías: " & ex.Message)
        Finally
            CerrarConexion()
        End Try
    End Sub

    'Ejecucion del comando de consulta para el boton "Venta por Locales"
    Private Sub CargaInformeLocal(idCliente As Integer)
        Try
            AbrirConexion()

            Dim fechas = ObtenerRangoFechas()
            Dim desde As Date = fechas.Item1
            Dim hasta As Date = fechas.Item2

            Dim consultaSQL As String =
            "SELECT g.numero_guia, c.nombre_fantasia, g.fecha, g.total " &
            "FROM guia g INNER JOIN cliente c ON g.id_cliente = c.id_cliente " &
            "WHERE g.id_cliente = ? AND g.fecha BETWEEN ? AND ? " &
            "ORDER BY g.fecha DESC"

            Using cmd As New OleDbCommand(consultaSQL, conexionDB)

                cmd.Parameters.AddWithValue("?", idCliente)
                cmd.Parameters.AddWithValue("?", desde)
                cmd.Parameters.AddWithValue("?", hasta)

                Dim tabla As New DataTable()
                Using reader As OleDbDataReader = cmd.ExecuteReader()
                    tabla.Load(reader)
                End Using

                dgvResultados.DataSource = tabla
                FormatoGuiayProduccion()

                Dim totalGeneral As Decimal = 0
                For Each fila As DataGridViewRow In dgvResultados.Rows
                    If Not IsDBNull(fila.Cells("Total").Value) Then
                        totalGeneral += Convert.ToDecimal(fila.Cells("Total").Value)
                    End If
                Next

                txtTotalInforme.Text = totalGeneral.ToString("C0",
                Globalization.CultureInfo.CreateSpecificCulture("es-CL"))

                MessageBox.Show($"Se encontraron {tabla.Rows.Count} guías para este local entre {desde:d} y {hasta:d}.",
                "Ventas por Local", MessageBoxButtons.OK, MessageBoxIcon.Information)

            End Using

        Catch ex As Exception
            MessageBox.Show("Error al cargar informe: " & ex.Message)
        Finally
            CerrarConexion()
        End Try
    End Sub

    'Formato de celdas titulares para guias y produccion
    Private Sub FormatoGuiayProduccion()
        'Formato Guias
        If dgvResultados.Columns.Contains("NumeroGuia") Then
            dgvResultados.Columns("NumeroGuia").HeaderText = "Número de Guía"
        End If

        If dgvResultados.Columns.Contains("numero_guia") Then
            dgvResultados.Columns("numero_guia").HeaderText = "Numero de Guia"
        End If

        If dgvResultados.Columns.Contains("Fecha") Then
            dgvResultados.Columns("Fecha").HeaderText = "Fecha"
            dgvResultados.Columns("Fecha").DefaultCellStyle.Format = "dd/MM/yyyy"
        End If

        If dgvResultados.Columns.Contains("cantidad") Then
            dgvResultados.Columns("cantidad").HeaderText = "Cantidad"
        End If

        If dgvResultados.Columns.Contains("Cliente") Then
            dgvResultados.Columns("Cliente").HeaderText = "Cliente"
        End If

        If dgvResultados.Columns.Contains("nombre_fantasia") Then
            dgvResultados.Columns("nombre_fantasia").HeaderText = "Nombre de Fantasia"
        End If

        If dgvResultados.Columns.Contains("precio_unitario") Then
            dgvResultados.Columns("precio_unitario").HeaderText = "Precio Unitario"
            dgvResultados.Columns("precio_unitario").DefaultCellStyle.Format = "C0"
            dgvResultados.Columns("precio_unitario").DefaultCellStyle.FormatProvider =
                Globalization.CultureInfo.CreateSpecificCulture("es-CL")
        End If

        If dgvResultados.Columns.Contains("Total") Then
            dgvResultados.Columns("Total").HeaderText = "Total"
            dgvResultados.Columns("Total").DefaultCellStyle.Format = "C0"
            dgvResultados.Columns("Total").DefaultCellStyle.FormatProvider =
                Globalization.CultureInfo.CreateSpecificCulture("es-CL")
        End If

        'Formato Produccion
        If dgvResultados.Columns.Contains("id_produccion") Then
            dgvResultados.Columns("id_produccion").HeaderText = "ID Producción"
        End If

        If dgvResultados.Columns.Contains("codigo_producto") Then
            dgvResultados.Columns("codigo_producto").HeaderText = "Codigo de Producto"
        End If

        If dgvResultados.Columns.Contains("fecha_produccion") Then
            dgvResultados.Columns("fecha_produccion").HeaderText = "Fecha"
            dgvResultados.Columns("fecha_produccion").DefaultCellStyle.Format = "dd/MM/yyyy"
        End If

        If dgvResultados.Columns.Contains("nombre_producto") Then
            dgvResultados.Columns("nombre_producto").HeaderText = "Nombre de Producto"
        End If

        If dgvResultados.Columns.Contains("nombre_empleado") Then
            dgvResultados.Columns("nombre_empleado").HeaderText = "Empleado"
        End If

        If dgvResultados.Columns.Contains("comentario") Then
            dgvResultados.Columns("comentario").HeaderText = "Comentario"
        End If

        If dgvResultados.Columns.Contains("subtotal") Then
            dgvResultados.Columns("subtotal").HeaderText = "Sub.Total"
            dgvResultados.Columns("subtotal").DefaultCellStyle.Format = "C0"
            dgvResultados.Columns("subtotal").DefaultCellStyle.FormatProvider =
                Globalization.CultureInfo.CreateSpecificCulture("es-CL")
        End If

        If dgvResultados.Columns.Contains("total") Then
            dgvResultados.Columns("total").HeaderText = "Total"
            dgvResultados.Columns("total").DefaultCellStyle.Format = "C0"
            dgvResultados.Columns("total").DefaultCellStyle.FormatProvider =
                Globalization.CultureInfo.CreateSpecificCulture("es-CL")
        End If
    End Sub

    Private Sub rdbPorMes_CheckedChanged(sender As Object, e As EventArgs) Handles rdbPorMes.CheckedChanged
        If rdbPorMes.Checked Then
            rdbEntreFechas.Checked = False
        End If
        ActualizarEstadoFiltros()
    End Sub

    Private Sub rdbEntreFechas_CheckedChanged(sender As Object, e As EventArgs) Handles rdbEntreFechas.CheckedChanged
        If rdbEntreFechas.Checked Then
            rdbPorMes.Checked = False
        End If
        ActualizarEstadoFiltros()
    End Sub

    Private Sub btnVentasProductos_Click(sender As Object, e As EventArgs) Handles btnVentasProductos.Click
        informeActual = TipoInforme.VentaPorProductos

        Using form As New FormInformeProducto()

            If form.ShowDialog() <> DialogResult.OK Then Exit Sub

            If form.MostrarTodosLosProductos Then
                CargarVentasPorProducto()
                Exit Sub
            End If

            If form.CodigoSeleccionado <> "" Then
                CargarVentasPorProducto(form.CodigoSeleccionado)
            End If

        End Using
    End Sub

    Private Sub CargarVentasPorProducto(Optional codigoProducto As String = "")
        Try
            AbrirConexion()

            Dim rango = ObtenerRangoFechas()
            Dim desde As Date = rango.Item1
            Dim hasta As Date = rango.Item2

            Dim sql As String

            If codigoProducto = "" Then
                ' =========================
                ' TODOS LOS PRODUCTOS
                ' =========================
                sql = "SELECT
                        d.nombre_producto AS Producto,
                        c.nombre_fantasia AS Cliente,
                        SUM(d.cantidad) AS cantidad,
                        SUM(d.subtotal) AS subtotal
                    FROM (guia AS g
                        INNER JOIN guia_detalle AS d ON g.id_guia = d.id_guia)
                        LEFT JOIN cliente AS c ON g.id_cliente = c.id_cliente
                    WHERE g.fecha BETWEEN ? AND ?
                    GROUP BY
                        d.nombre_producto,
                        c.nombre_fantasia
                    ORDER BY
                        d.nombre_producto,
                        c.nombre_fantasia;"

            Else
                ' =========================
                ' PRODUCTO ESPECÍFICO
                ' =========================
                sql = "SELECT
                        c.nombre_fantasia AS Cliente,
                        d.nombre_producto AS Producto,
                        SUM(d.cantidad) AS cantidad,
                        SUM(d.subtotal) AS subtotal
                    FROM (guia AS g
                        INNER JOIN guia_detalle AS d ON g.id_guia = d.id_guia)
                        LEFT JOIN cliente AS c ON g.id_cliente = c.id_cliente
                    WHERE
                        d.codigo_producto = ?
                        AND g.fecha BETWEEN ? AND ?
                    GROUP BY
                        c.nombre_fantasia,
                        d.nombre_producto
                    ORDER BY
                        c.nombre_fantasia;"
            End If

            Dim tabla As New DataTable()

            Using cmd As New OleDbCommand(sql, conexionDB)

                If codigoProducto <> "" Then
                    cmd.Parameters.AddWithValue("?", codigoProducto)
                End If

                cmd.Parameters.AddWithValue("?", desde)
                cmd.Parameters.AddWithValue("?", hasta)

                tabla.Load(cmd.ExecuteReader())
            End Using

            dgvResultados.DataSource = tabla
            FormatearVentasPorProducto()
            CalcularTotalVentasProducto()

        Catch ex As Exception
            MessageBox.Show("Error al cargar ventas por productos: " & ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            CerrarConexion()
        End Try
    End Sub

    Private Sub CalcularTotalVentasProducto()
        Dim total As Decimal = 0

        For Each fila As DataGridViewRow In dgvResultados.Rows
            If fila.IsNewRow Then Continue For
            If Not IsDBNull(fila.Cells("subtotal").Value) Then
                total += Convert.ToDecimal(fila.Cells("subtotal").Value)
            End If
        Next

        txtTotalInforme.Text = total.ToString("C0",
        Globalization.CultureInfo.CreateSpecificCulture("es-CL"))
    End Sub

    Private Sub FormatearVentasPorProducto()

        With dgvResultados

            If .Columns.Contains("numero_guia") Then
                .Columns("numero_guia").HeaderText = "N° Guía"
            End If

            If .Columns.Contains("fecha") Then
                .Columns("fecha").HeaderText = "Fecha"
                .Columns("fecha").DefaultCellStyle.Format = "dd-MM-yyyy"
            End If

            If .Columns.Contains("Cliente") Then
                .Columns("Cliente").HeaderText = "Cliente"
            End If

            If .Columns.Contains("Producto") Then
                .Columns("Producto").HeaderText = "Producto"
            End If

            If .Columns.Contains("cantidad") Then
                .Columns("cantidad").HeaderText = "Cant."
            End If

            If .Columns.Contains("subtotal") Then
                .Columns("subtotal").HeaderText = "Subtotal"
                .Columns("subtotal").DefaultCellStyle.Format = "C0"
                .Columns("subtotal").DefaultCellStyle.FormatProvider =
                Globalization.CultureInfo.CreateSpecificCulture("es-CL")
            End If

            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        End With

    End Sub


    Private Sub btnProduccionProductos_Click(sender As Object, e As EventArgs) Handles btnProduccionProductos.Click
        informeActual = TipoInforme.ProduccionProductos

        Try
            Dim rango = ObtenerRangoFechas()
            Dim desde As Date = rango.Item1
            Dim hasta As Date = rango.Item2

            CargarProduccionProductos(desde, hasta)
        Catch ex As Exception
            MessageBox.Show("Error al generar el informe: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub CargarProduccionProductos(desde As Date, hasta As Date)
        Try
            AbrirConexion()

            Dim consultaSQL As String =
            "SELECT p.id_produccion, p.fecha_produccion, d.codigo_producto, d.nombre_producto, d.cantidad, d.precio_unitario, d.subtotal, e.nombre_empleado, p.comentario " &
            "FROM (produccion AS p INNER JOIN produccion_detalle AS d ON p.id_produccion = d.id_produccion) " &
            "LEFT JOIN empleado AS e ON p.id_empleado = e.id_empleado " &
            "WHERE p.fecha_produccion BETWEEN ? AND ? " &
            "ORDER BY p.fecha_produccion DESC"

            Dim tabla As New DataTable()

            Using cmd As New OleDb.OleDbCommand(consultaSQL, conexionDB)
                cmd.Parameters.AddWithValue("?", desde.Date)
                cmd.Parameters.AddWithValue("?", hasta.Date)
                Using reader As OleDb.OleDbDataReader = cmd.ExecuteReader()
                    tabla.Load(reader)
                End Using
            End Using

            If tabla.Columns.Contains("subtotal") Then
                Try
                    tabla.Columns("subtotal").DataType = GetType(Decimal)
                Catch ex As Exception
                    ' si falla, no frenar; se intentará formatear luego
                End Try
            End If
            If tabla.Columns.Contains("precio_unitario") Then
                Try
                    tabla.Columns("precio_unitario").DataType = GetType(Decimal)
                Catch ex As Exception
                End Try
            End If
            If tabla.Columns.Contains("cantidad") Then
                Try
                    tabla.Columns("cantidad").DataType = GetType(Integer)
                Catch ex As Exception
                End Try
            End If

            dgvResultados.DataSource = tabla

            ' Formatear columnas (ajusta nombres si tu DataTable usa otros)
            FormatoGuiayProduccion()

            ' Calcular total general (suma de subtotales)
            Dim totalGeneral As Decimal = 0
            For Each fila As DataGridViewRow In dgvResultados.Rows
                If Not fila.IsNewRow Then
                    Dim val = fila.Cells("subtotal").Value
                    If Not IsDBNull(val) Then
                        Dim dec As Decimal
                        If Decimal.TryParse(val.ToString(), dec) Then
                            totalGeneral += dec
                        End If
                    End If
                End If
            Next

            ' Mostrar total formateado en tu textbox
            txtTotalInforme.Text = totalGeneral.ToString("C0", Globalization.CultureInfo.CreateSpecificCulture("es-CL"))

            ' Mensaje informativo
            MessageBox.Show(
            $"Se encontraron {tabla.Rows.Count} registros de producción entre {desde:d} y {hasta:d}.",
            "Producción - Detallado",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        )

        Catch ex As Exception
            MessageBox.Show("Error al generar informe de producción (detallado): " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            CerrarConexion()
        End Try
    End Sub

    '==========================================
    'BOTON PARA IMPRESION DE INFORMES
    '==========================================
    Dim fuenteTitulo As New Font("Consolas", 8, FontStyle.Bold)
    Dim fuenteCuerpo As New Font("Consolas", 7)
    Dim yPos As Integer = 0

    Private Sub btnImprimirInforme_Click(sender As Object, e As EventArgs) Handles btnImprimirInforme.Click
        If informeActual = TipoInforme.Ninguno Then
            MessageBox.Show("Debe generar un informe antes de imprimir.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If dgvResultados.Rows.Count = 0 OrElse dgvResultados.DataSource Is Nothing Then
            MessageBox.Show("No hay datos para imprimir.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Try
            Dim tamañoPapel As New PaperSize("Ticket80mm", 283, 2000)
            documentoImpresion.DefaultPageSettings.PaperSize = tamañoPapel
            documentoImpresion.DefaultPageSettings.Margins = New Margins(5, 5, 5, 5)

            Using vistaPrevia As New PrintPreviewDialog()
                vistaPrevia.Document = documentoImpresion
                vistaPrevia.WindowState = FormWindowState.Maximized
                vistaPrevia.PrintPreviewControl.Zoom = 1.5
                vistaPrevia.ShowDialog()
            End Using

        Catch ex As Exception
            MsgBox("Error al imprimir: " & ex.Message)
        End Try
    End Sub

    '========================
    ' Eliminar Acentos de Futuros Tickes
    '========================
    Function TextoTicketSeguro(AcentoTexto As String) As String
        Dim AcentoReemplazar As New Dictionary(Of String, String) From {
        {"á", "a"}, {"é", "e"}, {"í", "i"}, {"ó", "o"}, {"ú", "u"},
        {"Á", "A"}, {"É", "E"}, {"Í", "I"}, {"Ó", "O"}, {"Ú", "U"},
        {"ñ", "n"}, {"Ñ", "N"}
    }

        For Each r In AcentoReemplazar
            AcentoTexto = AcentoTexto.Replace(r.Key, r.Value)
        Next

        Return AcentoTexto
    End Function

    '========================
    ' IMPRESION DE INFORMES DE TICKETS EN 80mm
    '========================
    Private Sub documentoImpresion_PrintPage(sender As Object, e As PrintPageEventArgs) Handles documentoImpresion.PrintPage
        Dim g = e.Graphics
        Dim margen = e.MarginBounds.Left
        yPos = e.MarginBounds.Top
        Dim salto As Integer = CInt(fuenteCuerpo.GetHeight(g)) + 2

        '=== ENCABEZADO DE PANPAS GENERICO ===
        g.DrawString("Panaderia y Pasteleria PANPAS", fuenteTitulo, Brushes.Black, margen, yPos)
        yPos += salto * 2

        Select Case informeActual
                '========================
                ' INFORME : CORRELATIVO DE GUIAS
                '========================
            Case TipoInforme.CorrelativoGuias

                ' ---- ANCHOS DE COLUMNA ----
                Dim anchoGuia As Integer = 6
                Dim anchoFecha As Integer = 6
                Dim anchoCliente As Integer = 20
                Dim anchoTotal As Integer = 11

                ' ---------------- ENCABEZADO -------------------
                g.DrawString(TextoTicketSeguro("Correlativo de Guias"), fuenteTitulo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("Desde: " & dtpDesde.Value.ToString("dd/MM/yyyy"), fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("Hasta: " & dtpHasta.Value.ToString("dd/MM/yyyy"), fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                yPos += salto

                ' ---------------- CABECERA TABLA -------------------
                Dim header As String =
                    "Guia".PadRight(anchoGuia) & " " &
                    "Fecha".PadRight(anchoFecha) & " " &
                    "Cliente".PadRight(anchoCliente) & " " &
                    "Total".PadLeft(anchoTotal)

                g.DrawString(header, fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString(New String("-"c, anchoGuia + anchoFecha + anchoCliente + anchoTotal + 3),
                             fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                ' ---------------- CONTENIDO -------------------
                Dim totalGeneral As Integer = 0

                For Each fila As DataGridViewRow In dgvResultados.Rows
                    If fila.IsNewRow Then Continue For

                    Dim guia As String = fila.Cells("NumeroGuia").Value.ToString().PadRight(anchoGuia)
                    Dim fecha As String = CDate(fila.Cells("Fecha").Value).ToString("dd-MM").PadRight(anchoFecha)

                    Dim cliente As String = fila.Cells("Cliente").Value.ToString()
                    If cliente.Length > anchoCliente Then cliente = cliente.Substring(0, anchoCliente)
                    cliente = cliente.PadRight(anchoCliente)

                    Dim monto As Integer = Convert.ToInt32(fila.Cells("Total").Value)
                    Dim montoTxt As String = ("$ " & monto.ToString("N0")).PadLeft(anchoTotal)

                    Dim linea As String = guia & " " & fecha & " " & cliente & " " & montoTxt

                    g.DrawString(linea, fuenteCuerpo, Brushes.Black, margen, yPos)
                    yPos += salto

                    totalGeneral += monto
                Next

                ' ---------------- TOTAL GENERAL -------------------
                g.DrawString(New String("-"c, anchoGuia + anchoFecha + anchoCliente + anchoTotal + 3),
                             fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                Dim totalGeneralTxt As String = "$ " & totalGeneral.ToString("N0")

                g.DrawString("TOTAL GENERAL: ".PadRight(25) & totalGeneralTxt,
                             fuenteTitulo, Brushes.Black, margen, yPos)

                yPos += salto

                '========================
                ' INFORME : VENTA POR LOCALES (Por Local y Todos los Locales)
                '========================
            Case TipoInforme.VentaPorLocales

                ' ----- ANCHOS DE COLUMNAS -----
                Dim anchoGuia As Integer = 6
                Dim anchoFecha As Integer = 6
                Dim anchoCliente As Integer = 20
                Dim anchoTotal As Integer = 11

                ' ----- ENCABEZADO -----
                g.DrawString(TextoTicketSeguro("VENTAS POR LOCAL"), fuenteTitulo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("Desde: " & dtpDesde.Value.ToString("dd-MM-yyyy"), fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("Hasta: " & dtpHasta.Value.ToString("dd-MM-yyyy"), fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto * 2

                ' ----- CABECERA TABLA -----
                Dim header As String =
                    "Guía".PadRight(anchoGuia) & " " &
                    "Fecha".PadRight(anchoFecha) & " " &
                    "Cliente".PadRight(anchoCliente) & " " &
                    "Total".PadLeft(anchoTotal)

                g.DrawString(header, fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString(New String("-"c, anchoGuia + anchoFecha + anchoCliente + anchoTotal + 3),
                             fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                ' ----- CONTENIDO -----
                Dim totalGeneral As Decimal = 0

                For Each fila As DataGridViewRow In dgvResultados.Rows
                    If fila.IsNewRow Then Continue For

                    Dim guia As String = ""

                    If dgvResultados.Columns.Contains("NumeroGuia") Then
                        guia = fila.Cells("NumeroGuia").Value.ToString()
                    ElseIf dgvResultados.Columns.Contains("numero_guia") Then
                        guia = fila.Cells("numero_guia").Value.ToString()
                    End If

                    guia = guia.PadRight(anchoGuia)

                    Dim fecha As String = CDate(fila.Cells("Fecha").Value).ToString("dd-MM").PadRight(anchoFecha)

                    ' Detectar columna cliente / nombre_fantasia
                    Dim cliente As String = ""
                    If dgvResultados.Columns.Contains("nombre_fantasia") Then
                        cliente = fila.Cells("nombre_fantasia").Value.ToString()
                    ElseIf dgvResultados.Columns.Contains("Cliente") Then
                        cliente = fila.Cells("Cliente").Value.ToString()
                    End If

                    If cliente.Length > anchoCliente Then
                        cliente = cliente.Substring(0, anchoCliente)
                    End If
                    cliente = cliente.PadRight(anchoCliente)

                    Dim monto As Decimal = Convert.ToDecimal(fila.Cells("Total").Value)
                    Dim montoTxt As String = ("$ " & monto.ToString("N0")).PadLeft(anchoTotal)

                    Dim linea As String =
                        guia & " " &
                        fecha & " " &
                        cliente & " " &
                        montoTxt

                    g.DrawString(linea, fuenteCuerpo, Brushes.Black, margen, yPos)
                    yPos += salto

                    totalGeneral += monto
                Next

                ' ----- TOTAL GENERAL -----
                g.DrawString(New String("-"c, anchoGuia + anchoFecha + anchoCliente + anchoTotal + 3),
                             fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("TOTAL GENERAL: ".PadRight(25) & "$ " & totalGeneral.ToString("N0"),
                             fuenteTitulo, Brushes.Black, margen, yPos)

                yPos += salto

                '========================
                ' INFORME : VENTA POR PRODUCTOS (Por producto y todos los productos)
                '========================
            Case TipoInforme.VentaPorProductos

                ' ==== ANCHOS ====
                Dim anchoCliente As Integer = 16
                Dim anchoProducto As Integer = 18
                Dim anchoCant As Integer = 4
                Dim anchoSub As Integer = 8

                ' ==== ENCABEZADO ====
                g.DrawString(TextoTicketSeguro("VENTAS POR PRODUCTO"), fuenteTitulo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("Desde: " & dtpDesde.Value.ToString("dd-MM-yyyy"),
                 fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("Hasta: " & dtpHasta.Value.ToString("dd-MM-yyyy"),
                 fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                ' ==== N° GUÍA (opcional) ====
                If dgvResultados.Columns.Contains("NumeroGuia") AndAlso dgvResultados.Rows.Count > 0 Then
                    Dim guia = dgvResultados.Rows(0).Cells("NumeroGuia").Value
                    If guia IsNot Nothing Then
                        yPos += salto
                        g.DrawString("N° Guía: " & guia.ToString(),
                         fuenteCuerpo, Brushes.Black, margen, yPos)
                        yPos += salto
                    End If
                End If

                yPos += salto

                ' ==== CABECERA TABLA ====
                Dim header As String =
                "Cliente".PadRight(anchoCliente) & " " &
                "Producto".PadRight(anchoProducto) & " " &
                "Cnt".PadLeft(anchoCant) & " " &
                "SubTot".PadLeft(anchoSub)

                g.DrawString(header, fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString(New String("-"c, anchoCliente + anchoProducto + anchoCant + anchoSub + 3),
                fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                ' ==== CONTENIDO ====
                Dim totalGeneral As Decimal = 0

                For Each fila As DataGridViewRow In dgvResultados.Rows
                    If fila.IsNewRow Then Continue For

                    ' Cliente
                    Dim cliente As String = ValorCelda(fila, "Cliente")
                    If cliente.Length > anchoCliente Then cliente = cliente.Substring(0, anchoCliente)
                    cliente = cliente.PadRight(anchoCliente)

                    ' Producto (SIN PRECIO)
                    ' Producto
                    Dim producto As String = ValorCelda(fila, "Producto", "nombre_producto")

                    If producto.Length > anchoProducto Then
                        producto = producto.Substring(0, anchoProducto)
                    End If
                    producto = producto.PadRight(anchoProducto)

                    ' Cantidad
                    Dim cant As String = ValorCelda(fila, "cantidad").PadLeft(anchoCant)

                    ' Subtotal
                    Dim subt As Decimal = Convert.ToDecimal(ValorCelda(fila, "subtotal"))
                    Dim subTxt As String = subt.ToString("N0").PadLeft(anchoSub)

                    totalGeneral += subt

                    Dim linea As String =
                    cliente & " " &
                    producto & " " &
                    cant & " " &
                    subTxt

                    g.DrawString(linea, fuenteCuerpo, Brushes.Black, margen, yPos)
                    yPos += salto
                Next

                ' ==== TOTAL ====
                yPos += salto
                g.DrawString(New String("-"c, 46), fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("TOTAL:".PadRight(30) & "$ " & totalGeneral.ToString("N0"),
                fuenteTitulo, Brushes.Black, margen, yPos)

                yPos += salto

                '========================
                ' INFORME : PRODUCCION DE PRODUCTOS
                '========================
            Case TipoInforme.ProduccionProductos

                ' ==== ANCHOS (80mm) ====
                Dim aProd As Integer = 18
                Dim aCant As Integer = 4
                Dim aSub As Integer = 8

                ' ==== ENCABEZADO ====
                g.DrawString(TextoTicketSeguro("PRODUCCION DE PRODUCTOS"), fuenteTitulo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("Desde: " & dtpDesde.Value.ToString("dd-MM-yyyy"),
                             fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("Hasta: " & dtpHasta.Value.ToString("dd-MM-yyyy"),
                             fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto * 2

                ' ==== AGRUPAR POR PRODUCCIÓN ====
                Dim producciones =
                    dgvResultados.Rows.
                    Cast(Of DataGridViewRow)().
                    Where(Function(r) Not r.IsNewRow).
                    GroupBy(Function(r) r.Cells("id_produccion").Value.ToString())

                Dim totalGeneral As Decimal = 0

                For Each prod In producciones

                    Dim filas = prod.ToList()

                    Dim idProd As String = prod.Key
                    Dim fecha As String = CDate(filas(0).Cells("fecha_produccion").Value).ToString("dd-MM-yyyy")
                    Dim empleado As String = filas(0).Cells("nombre_empleado").Value.ToString()
                    Dim comentario As String = ""

                    If dgvResultados.Columns.Contains("comentario") Then
                        comentario = filas(0).Cells("comentario").Value?.ToString().Trim()
                    End If

                    ' ===== CABECERA PRODUCCIÓN =====
                    g.DrawString("PRODUCCION N° " & idProd, fuenteTitulo, Brushes.Black, margen, yPos)
                    yPos += salto

                    g.DrawString("Empleado: " & empleado, fuenteCuerpo, Brushes.Black, margen, yPos)
                    yPos += salto

                    g.DrawString("Fecha: " & fecha, fuenteCuerpo, Brushes.Black, margen, yPos)
                    yPos += salto

                    ' ===== CABECERA TABLA =====
                    Dim header As String =
                        "Producto".PadRight(aProd) & " " &
                        "Cnt".PadLeft(aCant) & " " &
                        "SubTot".PadLeft(aSub)

                    g.DrawString(header, fuenteCuerpo, Brushes.Black, margen, yPos)
                    yPos += salto

                    g.DrawString(New String("-"c, aProd + aCant + aSub + 2),
                                 fuenteCuerpo, Brushes.Black, margen, yPos)
                    yPos += salto

                    ' ===== AGRUPAR PRODUCTOS =====
                    Dim productos =
                        filas.GroupBy(Function(r) r.Cells("nombre_producto").Value.ToString())

                    For Each p In productos
                        Dim nombre As String = p.Key
                        If nombre.Length > aProd Then nombre = nombre.Substring(0, aProd)
                        nombre = nombre.PadRight(aProd)

                        Dim cant As Integer = p.Sum(Function(r) Convert.ToInt32(r.Cells("cantidad").Value))
                        Dim subt As Decimal = p.Sum(Function(r) Convert.ToDecimal(r.Cells("subtotal").Value))

                        Dim linea As String =
                            nombre & " " &
                            cant.ToString().PadLeft(aCant) & " " &
                            subt.ToString("N0").PadLeft(aSub)

                        g.DrawString(linea, fuenteCuerpo, Brushes.Black, margen, yPos)
                        yPos += salto

                        totalGeneral += subt
                    Next

                    ' ===== COMENTARIO =====
                    If comentario <> "" Then
                        yPos += salto
                        g.DrawString("Comentario:", fuenteTitulo, Brushes.Black, margen, yPos)
                        yPos += salto

                        For Each linea In comentario.Split({Environment.NewLine}, StringSplitOptions.None)
                            g.DrawString(linea, fuenteCuerpo, Brushes.Black, margen, yPos)
                            yPos += salto
                        Next
                    End If

                    ' ===== SEPARADOR ENTRE PRODUCCIONES =====
                    yPos += salto
                    g.DrawString(New String("-"c, 46), fuenteCuerpo, Brushes.Black, margen, yPos)
                    yPos += salto
                Next

                ' ========= TOTAL GENERAL =========
                g.DrawString("TOTAL GENERAL:".PadRight(30) & "$ " & totalGeneral.ToString("N0"),
                             fuenteTitulo, Brushes.Black, margen, yPos)

                yPos += salto

                '========================
                ' INFORME : RESUMEN PRODUCCION (Por General y Empleado)
                '========================
            Case TipoInforme.ResumenProduccion

                '========================
                ' ENCABEZADO DE TICKET
                '========================
                g.DrawString(TextoTicketSeguro("RESUMEN DE PRODUCCION"), fuenteTitulo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("Desde: " & dtpDesde.Value.ToString("dd-MM-yyyy"), fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("Hasta: " & dtpHasta.Value.ToString("dd-MM-yyyy"), fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                '====================================================
                ' FORMATO A - PRODUCCIÓN POR EMPLEADO (DETALLADO)
                '====================================================
                If dgvResultados.Columns.Contains("codigo_producto") Then

                    ' Obtener nombre del empleado
                    Dim nombreEmpleado As String = ""
                    If dgvResultados.Rows.Count > 0 AndAlso dgvResultados.Columns.Contains("nombre_empleado") Then
                        nombreEmpleado = dgvResultados.Rows(0).Cells("nombre_empleado").Value.ToString()
                    End If

                    If nombreEmpleado <> "" Then
                        g.DrawString("Empleado: " & nombreEmpleado, fuenteTitulo, Brushes.Black, margen, yPos)
                        yPos += salto
                    End If

                    yPos += salto

                    ' ---- Anchos ----
                    Dim aFecha = 6, aProd = 26, aCant = 4, aPrecio = 8, aSub = 10

                    ' ---- Cabecera ----
                    Dim header =
                        "Fecha".PadRight(aFecha) & " " &
                        "Producto".PadRight(aProd) & " " &
                        "Cant".PadLeft(aCant) & " " &
                        "P.Unit".PadLeft(aPrecio) & " " &
                        "SubTot".PadLeft(aSub)

                    g.DrawString(header, fuenteCuerpo, Brushes.Black, margen, yPos)
                    yPos += salto

                    g.DrawString(New String("-"c, aFecha + aProd + aCant + aPrecio + aSub + 4),
                                 fuenteCuerpo, Brushes.Black, margen, yPos)
                    yPos += salto

                    Dim totalGeneral As Decimal = 0

                    For Each fila As DataGridViewRow In dgvResultados.Rows
                        If fila.IsNewRow Then Continue For

                        Dim fecha = CDate(fila.Cells("fecha_produccion").Value).ToString("dd-MM").PadRight(aFecha)

                        Dim prod = fila.Cells("nombre_producto").Value.ToString()
                        If prod.Length > aProd Then prod = prod.Substring(0, aProd)
                        prod = prod.PadRight(aProd)

                        Dim cant = fila.Cells("cantidad").Value.ToString().PadLeft(aCant)
                        Dim precio = Convert.ToDecimal(fila.Cells("precio_unitario").Value).ToString("N0").PadLeft(aPrecio)
                        Dim subt = Convert.ToDecimal(fila.Cells("subtotal").Value)
                        Dim subtTxt = subt.ToString("N0").PadLeft(aSub)

                        totalGeneral += subt

                        Dim linea =
                            fecha & " " & prod & " " &
                            cant & " " & precio & " " & subtTxt


                        g.DrawString(linea, fuenteCuerpo, Brushes.Black, margen, yPos)
                        yPos += salto
                    Next

                    yPos += salto
                    g.DrawString(New String("-"c, 55), fuenteCuerpo, Brushes.Black, margen, yPos)
                    yPos += salto

                    g.DrawString("TOTAL GENERAL:     $ " & totalGeneral.ToString("N0"),
                                 fuenteTitulo, Brushes.Black, margen, yPos)

                    '====================================================
                    ' FORMATO B - TODA LA PRODUCCIÓN (RESUMEN)
                    '====================================================
                Else

                    yPos += salto

                    Dim aFecha = 6, aEmp = 20, aTotal = 11

                    Dim header =
                        "Fecha".PadRight(aFecha) & " " &
                        "Empleado".PadRight(aEmp) & " " &
                        "Total".PadLeft(aTotal)

                    g.DrawString(header, fuenteCuerpo, Brushes.Black, margen, yPos)
                    yPos += salto

                    g.DrawString(New String("-"c, aFecha + aEmp + aTotal + 3),
                                 fuenteCuerpo, Brushes.Black, margen, yPos)
                    yPos += salto

                    Dim totalGeneral As Decimal = 0

                    For Each fila As DataGridViewRow In dgvResultados.Rows
                        If fila.IsNewRow Then Continue For

                        Dim fecha = CDate(fila.Cells("fecha_produccion").Value).ToString("dd-MM").PadRight(aFecha)

                        Dim emp = fila.Cells("nombre_empleado").Value.ToString()
                        If emp.Length > aEmp Then emp = emp.Substring(0, aEmp)
                        emp = emp.PadRight(aEmp)

                        Dim total = Convert.ToDecimal(fila.Cells("total").Value)
                        Dim totalTxt = ("$ " & total.ToString("N0")).PadLeft(aTotal)

                        totalGeneral += total

                        g.DrawString(fecha & " " & emp & " " & totalTxt,
                                     fuenteCuerpo, Brushes.Black, margen, yPos)
                        yPos += salto

                        ' ----- Comentario -----
                        If dgvResultados.Columns.Contains("comentario") Then
                            Dim comentario = fila.Cells("comentario").Value?.ToString().Trim()

                            If comentario <> "" Then
                                yPos += salto
                                g.DrawString(New String("-"c, 55), fuenteCuerpo, Brushes.Black, margen, yPos)
                                yPos += salto

                                g.DrawString("Comentario:", fuenteTitulo, Brushes.Black, margen, yPos)
                                yPos += salto

                                For Each linea In comentario.Split({Environment.NewLine}, StringSplitOptions.None)
                                    g.DrawString(linea, fuenteCuerpo, Brushes.Black, margen, yPos)
                                    yPos += salto
                                Next

                                yPos += salto
                                g.DrawString(New String("-"c, 55), fuenteCuerpo, Brushes.Black, margen, yPos)
                                yPos += salto
                            End If
                        End If
                    Next

                    yPos += salto
                    g.DrawString(New String("-"c, 55), fuenteCuerpo, Brushes.Black, margen, yPos)
                    yPos += salto

                    g.DrawString("TOTAL GENERAL:     $ " & totalGeneral.ToString("N0"),
                                 fuenteTitulo, Brushes.Black, margen, yPos)
                End If

            Case Else
                g.DrawString("Ningun Informe ha sido Seleccionado.", fuenteTitulo, Brushes.Black, margen, yPos)
        End Select

    End Sub

    Private Sub btnLimpiarTabla_Click(sender As Object, e As EventArgs) Handles btnLimpiarTabla.Click
        dgvResultados.DataSource = Nothing
        dgvResultados.Rows.Clear()
        dgvResultados.Columns.Clear()

        txtTotalInforme.Text = ""
    End Sub

End Class
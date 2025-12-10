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
            WHERE g.fecha BETWEEN #" & desde.ToString("MM/dd/yyyy") & "# 
            AND #" & hasta.ToString("MM/dd/yyyy") & "#
            ORDER BY g.numero_guia;"

            AbrirConexion()

            Using cmd As New OleDb.OleDbCommand(consultaSQL, conexionDB)
                cmd.Parameters.AddWithValue("@Desde", desde)
                cmd.Parameters.AddWithValue("@Hasta", hasta)

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
                            "Correlativo de Guías", MessageBoxButtons.OK, MessageBoxIcon.Information)
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

            If selector.ShowDialog() = DialogResult.OK Then

                If selector.EmpleadoID = 0 Then
                    CargarTodaLaProduccion()
                    Exit Sub
                End If

                Dim IDEmpleado As Integer = selector.EmpleadoID
                CargarProduccionPorEmpleado(IDEmpleado)

            End If

        End If
    End Sub

    Private Sub CargarTodaLaProduccion()
        Try
            AbrirConexion()

            Dim fechas = ObtenerRangoFechas()
            Dim desde As Date = fechas.Item1
            Dim hasta As Date = fechas.Item2

            Dim consultaSQL As String = "SELECT p.id_produccion,
                p.fecha_produccion,
                e.nombre_empleado,
                p.comentario,
                p.total
         FROM produccion AS p
         LEFT JOIN empleado AS e ON p.id_empleado = e.id_empleado
         WHERE p.fecha_produccion BETWEEN ? AND ?
         ORDER BY p.fecha_produccion DESC"

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

    'Venta por productos de guias
    Private Sub BuscarProductoEnGuias(codigoProd As String)
        Try
            AbrirConexion()

            Dim fechas = ObtenerRangoFechas()
            Dim desde As Date = fechas.Item1
            Dim hasta As Date = fechas.Item2

            Dim sql As String =
            "SELECT g.numero_guia AS NumeroGuia, g.fecha AS Fecha, " &
            "c.nombre_fantasia AS Cliente, d.nombre_producto AS Producto, " &
            "d.cantidad AS Cantidad, d.subtotal AS subtotal " &
            "FROM (guia AS g " &
            "INNER JOIN guia_detalle AS d ON g.id_guia = d.id_guia) " &
            "LEFT JOIN cliente AS c ON g.id_cliente = c.id_cliente " &
            "WHERE d.codigo_producto = ? AND g.fecha BETWEEN ? AND ? " &
            "ORDER BY g.fecha DESC"


            Using cmd As New OleDbCommand(sql, conexionDB)
                cmd.Parameters.AddWithValue("?", codigoProd)
                cmd.Parameters.AddWithValue("?", desde)
                cmd.Parameters.AddWithValue("?", hasta)

                Dim tabla As New DataTable()
                tabla.Load(cmd.ExecuteReader())

                dgvResultados.DataSource = tabla

                FormatoGuiayProduccion()

                'Sumatoria
                Dim total As Decimal = 0
                For Each row As DataGridViewRow In dgvResultados.Rows
                    If Not IsDBNull(row.Cells("subtotal").Value) Then
                        total += Convert.ToDecimal(row.Cells("subtotal").Value)
                    End If
                Next

                txtTotalInforme.Text = total.ToString("C0",
                Globalization.CultureInfo.CreateSpecificCulture("es-CL"))
            End Using

        Catch ex As Exception
            MessageBox.Show("Error al generar informe: " & ex.Message)
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

        Dim form As New FormInformeProducto()

        If form.ShowDialog() = DialogResult.OK Then

            ' Si seleccionó todos:
            If form.MostrarTodosLosProductos Then
                CargarVentasProductosAgrupado()
                informeActual = TipoInforme.VentaPorProductos
                Exit Sub
            End If

            ' Si seleccionó un producto específico
            If form.CodigoSeleccionado <> "" Then
                BuscarProductoEnGuias(form.CodigoSeleccionado)
                informeActual = TipoInforme.VentaPorProductos
            End If

        End If
    End Sub

    Private Sub CargarVentasProductosAgrupado()
        Try
            AbrirConexion()

            Dim fechas = ObtenerRangoFechas()
            Dim desde As Date = fechas.Item1
            Dim hasta As Date = fechas.Item2

            Dim sql As String =
            "SELECT 
            d.codigo_producto,
            d.nombre_producto,
            SUM(d.cantidad) AS CantidadTotal,
            SUM(d.subtotal) AS TotalVendido
        FROM guia AS g
        INNER JOIN guia_detalle AS d ON g.id_guia = d.id_guia
        WHERE g.fecha BETWEEN ? AND ?
        GROUP BY d.codigo_producto, d.nombre_producto
        ORDER BY d.nombre_producto;"

            Dim tabla As New DataTable()

            Using cmd As New OleDbCommand(sql, conexionDB)
                cmd.Parameters.AddWithValue("?", desde)
                cmd.Parameters.AddWithValue("?", hasta)
                tabla.Load(cmd.ExecuteReader())
            End Using

            dgvResultados.DataSource = tabla

            dgvResultados.Columns("codigo_producto").HeaderText = "Código"
            dgvResultados.Columns("nombre_producto").HeaderText = "Producto"
            dgvResultados.Columns("CantidadTotal").HeaderText = "Cantidad"
            dgvResultados.Columns("TotalVendido").HeaderText = "Total Vendido"

            dgvResultados.Columns("TotalVendido").DefaultCellStyle.Format = "C0"
            dgvResultados.Columns("TotalVendido").DefaultCellStyle.FormatProvider =
                Globalization.CultureInfo.CreateSpecificCulture("es-CL")

            Dim totalGeneral As Decimal = 0
            For Each row As DataGridViewRow In dgvResultados.Rows
                If Not IsDBNull(row.Cells("TotalVendido").Value) Then
                    totalGeneral += Convert.ToDecimal(row.Cells("TotalVendido").Value)
                End If
            Next

            txtTotalInforme.Text = totalGeneral.ToString("C0",
                Globalization.CultureInfo.CreateSpecificCulture("es-CL"))

            MessageBox.Show("Se generó el informe de todos los productos.",
                            "Ventas por Productos",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error al cargar informe agrupado: " & ex.Message)
        Finally
            CerrarConexion()
        End Try
    End Sub

    Private Sub btnProduccionProductos_Click(sender As Object, e As EventArgs) Handles btnProduccionProductos.Click
        informeActual = TipoInforme.ProduccionProductos

        Dim rango = ObtenerRangoFechas()
        Dim desde As Date = rango.Item1
        Dim hasta As Date = rango.Item2

        CargarProduccionProductos(desde, hasta)

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

    Private Sub documentoImpresion_PrintPage(sender As Object, e As PrintPageEventArgs) Handles documentoImpresion.PrintPage
        Dim g = e.Graphics
        Dim margen = e.MarginBounds.Left
        yPos = e.MarginBounds.Top
        Dim salto = fuenteCuerpo.GetHeight(g) + 2

        g.DrawString("Panadería y Pastelería PANPAS", fuenteTitulo, Brushes.Black, margen, yPos)
        yPos += salto * 2

        Select Case informeActual

            Case TipoInforme.CorrelativoGuias

                yPos = e.MarginBounds.Top

                ' ---- ANCHOS DE COLUMNA ----
                Dim anchoGuia As Integer = 6
                Dim anchoFecha As Integer = 6
                Dim anchoCliente As Integer = 20
                Dim anchoTotal As Integer = 11

                ' ---------------- ENCABEZADO -------------------
                g.DrawString("Panadería y Pastelería PANPAS", fuenteTitulo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("Correlativo de Guías", fuenteTitulo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("Desde: " & dtpDesde.Value.ToString("dd/MM/yyyy"), fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                g.DrawString("Hasta: " & dtpHasta.Value.ToString("dd/MM/yyyy"), fuenteCuerpo, Brushes.Black, margen, yPos)
                yPos += salto

                yPos += salto

                ' ---------------- CABECERA TABLA -------------------
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

            Case TipoInforme.VentaPorLocales
                g.DrawString("IMPRIMIENDO: VENTA POR LOCALES", fuenteTitulo, Brushes.Black, margen, yPos)

            Case TipoInforme.VentaPorProductos
                g.DrawString("IMPRIMIENDO: VENTA POR PRODUCTOS", fuenteTitulo, Brushes.Black, margen, yPos)

            Case TipoInforme.ProduccionProductos
                g.DrawString("IMPRIMIENDO: PRODUCCION PRODUCTOS", fuenteTitulo, Brushes.Black, margen, yPos)

            Case TipoInforme.ResumenProduccion
                g.DrawString("IMPRIMIENDO: RESUMEN PRODUCCION", fuenteTitulo, Brushes.Black, margen, yPos)

            Case Else
                g.DrawString("NINGUN INFORME SELECCIONADO", fuenteTitulo, Brushes.Black, margen, yPos)

        End Select

    End Sub

    Private Sub btnLimpiarTabla_Click(sender As Object, e As EventArgs) Handles btnLimpiarTabla.Click
        dgvResultados.DataSource = Nothing
        dgvResultados.Rows.Clear()
        dgvResultados.Columns.Clear()

        txtTotalInforme.Text = ""
    End Sub

End Class
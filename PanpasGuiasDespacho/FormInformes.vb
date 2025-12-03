Imports System.Data.OleDb
Imports System.Security.Cryptography
Imports System.Security.Policy
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class FormInformes

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

    Private Function ObtenerRangoFechas() As Tuple(Of Date, Date)
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
        Dim selector As New FormResumenProduccion()

        If selector.ShowDialog() = DialogResult.OK Then
            Dim IDEmpleado As Integer = selector.EmpleadoID

            If IDEmpleado > 0 Then
                CargarProduccionPorEmpleado(IDEmpleado)
            End If
        End If

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
            d.codigo_producto,
            d.nombre_producto,
            d.cantidad,
            d.precio_unitario,
            d.subtotal
         FROM produccion AS p
         INNER JOIN produccion_detalle AS d 
                ON p.id_produccion = d.id_produccion
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

            ' FORMATEO DE COLUMNAS
            If dgvResultados.Columns.Contains("precio_unitario") Then
                dgvResultados.Columns("precio_unitario").DefaultCellStyle.Format = "C0"
                dgvResultados.Columns("precio_unitario").DefaultCellStyle.FormatProvider =
                Globalization.CultureInfo.CreateSpecificCulture("es-CL")
            End If

            If dgvResultados.Columns.Contains("subtotal") Then
                dgvResultados.Columns("subtotal").DefaultCellStyle.Format = "C0"
                dgvResultados.Columns("subtotal").DefaultCellStyle.FormatProvider =
                Globalization.CultureInfo.CreateSpecificCulture("es-CL")
            End If

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
        Dim selector As New FormLocal()

        If selector.ShowDialog() = DialogResult.OK Then
            Dim IDLocal As Integer = selector.LocalID

            If IDLocal > 0 Then
                CargaInformeLocal(IDLocal)
            End If
        End If
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

                If dgvResultados.Columns.Contains("Total") Then
                    dgvResultados.Columns("Total").DefaultCellStyle.Format = "C0"
                    dgvResultados.Columns("Total").DefaultCellStyle.FormatProvider =
                    Globalization.CultureInfo.CreateSpecificCulture("es-CL")
                End If

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
            "d.cantidad AS Cantidad, d.subtotal AS Subtotal " &
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

                'Formato del grid
                If dgvResultados.Columns.Contains("Subtotal") Then
                    dgvResultados.Columns("Subtotal").DefaultCellStyle.Format = "C0"
                    dgvResultados.Columns("Subtotal").DefaultCellStyle.FormatProvider =
                    Globalization.CultureInfo.CreateSpecificCulture("es-CL")
                End If

                'Sumatoria
                Dim total As Decimal = 0
                For Each row As DataGridViewRow In dgvResultados.Rows
                    If Not IsDBNull(row.Cells("Subtotal").Value) Then
                        total += Convert.ToDecimal(row.Cells("Subtotal").Value)
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
        Dim f As New FormInformeProducto()

        If f.ShowDialog() = DialogResult.OK Then
            If f.CodigoSeleccionado <> "" Then
                BuscarProductoEnGuias(f.CodigoSeleccionado)
            End If
        End If
    End Sub
End Class
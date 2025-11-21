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
        Try
            'Obtener el rango de las producciones con las fechas
            Dim rango = ObtenerRangoFechas()
            Dim desde As Date = rango.Item1
            Dim hasta As Date = rango.Item2

            Dim consultaSQL As String = "SELECT p.id_produccion, p.fecha_produccion, e.nombre_empleado, p.comentario, p.total " &
            "FROM produccion AS p " &
            "LEFT JOIN empleado AS e ON p.id_empleado = e.id_empleado " &
            "WHERE p.fecha_produccion BETWEEN @desde AND @hasta " &
            "ORDER BY p.fecha_produccion ASC"

            AbrirConexion()

            Using cmd As New OleDb.OleDbCommand(consultaSQL, conexionDB)
                cmd.Parameters.AddWithValue("@Desde", desde)
                cmd.Parameters.AddWithValue("@Hasta", hasta)

                Dim adaptar As New OleDb.OleDbDataAdapter(cmd)
                Dim tabla As New DataTable()
                adaptar.Fill(tabla)

                dgvResultados.DataSource = tabla
                'Entrega de formato en las celdas de titulos
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

                MessageBox.Show($"Se encontraron {tabla.Rows.Count} producciones entre {desde:d} y {hasta:d}.",
                                "Resumen de Produccion", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using

            CerrarConexion()

        Catch ex As Exception
            MessageBox.Show("Error al generar el resumen de produccion: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

        If dgvResultados.Columns.Contains("Cliente") Then
            dgvResultados.Columns("Cliente").HeaderText = "Cliente"
        End If

        If dgvResultados.Columns.Contains("nombre_fantasia") Then
            dgvResultados.Columns("nombre_fantasia").HeaderText = "Nombre de Fantasia"
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

        If dgvResultados.Columns.Contains("fecha_produccion") Then
            dgvResultados.Columns("fecha_produccion").HeaderText = "Fecha"
            dgvResultados.Columns("fecha_produccion").DefaultCellStyle.Format = "dd/MM/yyyy"
        End If

        If dgvResultados.Columns.Contains("nombre_empleado") Then
            dgvResultados.Columns("nombre_empleado").HeaderText = "Empleado"
        End If

        If dgvResultados.Columns.Contains("comentario") Then
            dgvResultados.Columns("comentario").HeaderText = "Comentario"
        End If

        If dgvResultados.Columns.Contains("total") Then
            dgvResultados.Columns("total").HeaderText = "Total"
            dgvResultados.Columns("total").DefaultCellStyle.Format = "C0"
            dgvResultados.Columns("total").DefaultCellStyle.FormatProvider =
                Globalization.CultureInfo.CreateSpecificCulture("es-CL")
        End If
    End Sub

    Private Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        dtpDesde.Value = Date.Now
        dtpHasta.Value = Date.Now
        cmbMes.SelectedIndex = Date.Now.Month - 1
        cmbAnio.SelectedItem = Date.Now.Year
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


End Class
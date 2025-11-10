Imports System.Data.OleDb
Imports System.Security.Cryptography
Imports System.Security.Policy

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
            rdbPorMes.Checked = False
        ElseIf rdbPorMes.Checked Then
            dtpDesde.Enabled = False
            dtpHasta.Enabled = False
            cmbMes.Enabled = True
            cmbAnio.Enabled = True
            rdbEntreFechas.Checked = False
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

            Using cmd As New OleDb.OleDbCommand(consultaSQL, conexionDB)
                cmd.Parameters.AddWithValue("@Desde", desde)
                cmd.Parameters.AddWithValue("@Hasta", hasta)

                Dim adaptar As New OleDb.OleDbDataAdapter(cmd)
                Dim tabla As New DataTable()
                adaptar.Fill(tabla)

                dgvResultados.DataSource = tabla

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

        Catch ex As Exception
            MessageBox.Show("Error al generar  el informe: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        dtpDesde.Value = Date.Now
        dtpHasta.Value = Date.Now
        cmbMes.SelectedIndex = Date.Now.Month - 1
        cmbAnio.SelectedItem = Date.Now.Year
    End Sub

End Class
Imports System.Data.OleDb

Public Class FormLocal

    Public Property LocalID As Integer = -1

    Private Sub FormLocal_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarLocal()
    End Sub

    Private Sub CargarLocal()
        Try
            AbrirConexion()

            Dim consultaSQL As String = "SELECT id_cliente, nombre_fantasia FROM cliente ORDER BY nombre_fantasia"
            Dim cmd As New OleDbCommand(consultaSQL, conexionDB)
            Dim reader As OleDbDataReader = cmd.ExecuteReader()

            Dim tabla As New DataTable()
            tabla.Load(reader)

            cmbLocales.DataSource = tabla
            cmbLocales.DisplayMember = "nombre_fantasia"
            cmbLocales.ValueMember = "id_cliente"
            cmbLocales.SelectedIndex = -1

            CerrarConexion()

        Catch ex As Exception
            MessageBox.Show("Error al cargar locales: " & ex.Message)
        End Try
    End Sub

    Private Sub btnAceptar_Click(sender As Object, e As EventArgs) Handles btnAceptar.Click
        If cmbLocales.SelectedIndex = -1 Then
            MessageBox.Show("Debe de seleccionar un Local.")
            Exit Sub
        End If

        LocalID = CInt(cmbLocales.SelectedValue)
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnTodoLocales_Click(sender As Object, e As EventArgs) Handles btnTodoLocales.Click
        LocalID = 0
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub
End Class
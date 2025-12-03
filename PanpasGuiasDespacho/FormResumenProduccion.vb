Imports System.Data.OleDb

Public Class FormResumenProduccion

    Public Property EmpleadoID As Integer = -1

    Private Sub FormResumenProduccion_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarEmpleado()
    End Sub

    Private Sub CargarEmpleado()
        Try
            AbrirConexion()

            Dim consultaSQL As String = "SELECT id_empleado, nombre_empleado FROM empleado ORDER BY nombre_empleado"

            Using cmd As New OleDbCommand(consultaSQL, conexionDB)
                Dim tabla As New DataTable()
                tabla.Load(cmd.ExecuteReader())

                cbEmpleado.DataSource = tabla
                cbEmpleado.DisplayMember = "nombre_empleado"
                cbEmpleado.ValueMember = "id_empleado"
                cbEmpleado.SelectedIndex = -1
            End Using

        Catch ex As Exception
            MessageBox.Show("Error al cargar los empleados: " & ex.Message)
        Finally
            CerrarConexion()
        End Try
    End Sub

    Private Sub btnEmpBuscar_Click(sender As Object, e As EventArgs) Handles btnEmpBuscar.Click
        If cbEmpleado.SelectedIndex = -1 Then
            MessageBox.Show("Debe seleccionar a un empleado.")
            Exit Sub
        End If

        EmpleadoID = CInt(cbEmpleado.SelectedValue)
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub
End Class
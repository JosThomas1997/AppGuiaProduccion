Option Strict On
Public Class FormLoginAdmin

    Private Sub FormLoginAdmin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CenterToScreen()
        txtClave.Focus()
    End Sub

    Private Sub txtClave_KeyDown(sender As Object, e As KeyEventArgs) Handles txtClave.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            btnIngresar.PerformClick()
        End If
    End Sub

    Private Sub btnIngresar_Click(sender As Object, e As EventArgs) Handles btnIngresar.Click
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Public ReadOnly Property ClaveIngresada As String
        Get
            Return txtClave.Text
        End Get
    End Property

End Class

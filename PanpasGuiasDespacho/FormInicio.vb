Imports System.Security.Cryptography

Public Class FormInicio

    Private Sub FormInicio_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CenterToScreen()
    End Sub

    Private Sub btnGuiaProduccion_Click(sender As Object, e As EventArgs) Handles btnGuiaProduccion.Click
        Dim formAbierto As Boolean = False

        For Each form As Form In Application.OpenForms
            If TypeOf form Is Form1 Then
                formAbierto = True
                form.BringToFront()
                Exit For
            End If
        Next

        If formAbierto Then
            MessageBox.Show("Guia y Produccion se encuentra en ejecución.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            Dim form As New Form1()
            form.Show()
        End If
    End Sub

    Private Sub btnInformes_Click(sender As Object, e As EventArgs) Handles btnInformes.Click
        Dim formAbierto As Boolean = False

        For Each form As Form In Application.OpenForms
            If TypeOf form Is FormInformes Then
                formAbierto = True
                form.BringToFront()
                Exit For
            End If
        Next

        If formAbierto Then
            MessageBox.Show("Informes se encuentra en ejecucion.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            Dim form As New FormInformes
            form.Show()
        End If
    End Sub

End Class
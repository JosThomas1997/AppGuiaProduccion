Option Strict On

Imports System.Security.Cryptography
Imports System.Text

Module Seguridad

    ' IMPORTANTE: La contrseña no se guarda en texto plano, solamente en HASH.
    Private Const HashClaveAdmin As String = "vrx9NjR4gQDHjhAEVAb3e1DvYQNpXk/HVaSVxdu3Xyo="
    ' Intentos maximos de ingreso de clave antes de bloquear el acceso.
    Private Const MaxIntentos As Integer = 3

    ' Validacion de contraseña de CRUD 
    Public Function ValidarAccesoAdmin() As Boolean
        Dim intentos As Integer = 0

        While intentos < MaxIntentos
            Using frmLogin As New FormLoginAdmin()
                If frmLogin.ShowDialog() <> DialogResult.OK Then
                    Return False ' El usuario presiono Cancelar
                End If

                'Si la clave es correcta, se permite el acceso al CRUD
                If CalcularHash(frmLogin.ClaveIngresada) = HashClaveAdmin Then
                    Return True
                End If
            End Using

            'Si la clave es incorrecta, se incrementa el contador de intentos y se muestra un mensaje de error
            intentos += 1
            If intentos < MaxIntentos Then
                MsgBox("Contraseña incorrecta. Intento Nº " & intentos & " de " & MaxIntentos & ".", MsgBoxStyle.Exclamation, "Intente de nuevo!")
            End If
        End While

        MsgBox("Se alcanzó el número máximo de intentos. Acceso denegado.", MsgBoxStyle.Critical, "Acceso denegado")
        Return False
    End Function

    ' Calculo de HASH de la contraseña ingresada
    Public Function CalcularHash(texto As String) As String
        Using sha As SHA256 = SHA256.Create()
            Dim bytesEntrada = Encoding.UTF8.GetBytes(texto)
            Dim bytesHash = sha.ComputeHash(bytesEntrada)
            Return Convert.ToBase64String(bytesHash)
        End Using
    End Function

End Module

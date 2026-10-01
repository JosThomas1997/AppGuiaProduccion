Imports System.Data.OleDb

''' <summary>
''' Formulario para reactivar productos inactivos
''' Se usa cuando se detecta que el catálogo no tiene productos activos
''' </summary>
Public Class FormReactivarProductos

    Private Sub FormReactivarProductos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CenterToScreen()
        ActualizarEstado()
    End Sub

    ''' <summary>
    ''' Verifica y actualiza el estado de productos
    ''' </summary>
    Private Sub ActualizarEstado()
        Try
            ' Verificar cuántos productos esperados están inactivos
            Dim inactivos As Integer = UtilReactivarProductos.VerificarProductosInactivos()

            If inactivos > 0 Then
                lblEstado.Text = $"Se encontraron {inactivos} productos inactivos que DEBERÍAN estar activos."
                lblEstado.ForeColor = Color.Red
                btnReactivar.Enabled = True
            Else
                lblEstado.Text = "Todos los productos esperados están activos. ✓"
                lblEstado.ForeColor = Color.Green
                btnReactivar.Enabled = False
            End If

        Catch ex As Exception
            lblEstado.Text = $"Error al verificar: {ex.Message}"
            lblEstado.ForeColor = Color.Red
        End Try
    End Sub

    ''' <summary>
    ''' Reactiva los productos esperados (hardcodeados en FormProductos)
    ''' </summary>
    Private Sub btnReactivar_Click(sender As Object, e As EventArgs) Handles btnReactivar.Click

        Dim confirmacion = MessageBox.Show(
            "¿Deseas reactivar los productos del catálogo?" & vbCrLf & vbCrLf &
            "Se reactivarán:" & vbCrLf &
            "- Galletas (1304, 1305, 1310, 1311, 1313)" & vbCrLf &
            "- Empanadas (1410, 1411, 1414, 9012)" & vbCrLf &
            "- Masas Dulces, Kuchen, Pasteles, etc." & vbCrLf & vbCrLf &
            "Total de productos a reactivar: ~50",
            "Confirmar reactivación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If confirmacion = DialogResult.No Then
            Return
        End If

        Try
            btnReactivar.Enabled = False
            btnReactivar.Text = "Procesando..."
            Application.DoEvents()

            Dim afectados As Integer = UtilReactivarProductos.ReactivarProductosEsperados()

            If afectados > 0 Then
                MessageBox.Show(
                    $"✓ Se reactivaron {afectados} productos correctamente." & vbCrLf & vbCrLf &
                    "Ahora deberían aparecer en los formularios de producción y guías.",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information)
                ActualizarEstado()
            Else
                MessageBox.Show(
                    "No se realizaron cambios. Los productos podrían ya estar activos.",
                    "Sin cambios",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            btnReactivar.Enabled = True
            btnReactivar.Text = "Reactivar Productos"
        End Try

    End Sub

    ''' <summary>
    ''' Reactiva TODOS los productos (opción nuclear)
    ''' </summary>
    Private Sub btnReactivarTodos_Click(sender As Object, e As EventArgs) Handles btnReactivarTodos.Click

        Dim confirmacion = MessageBox.Show(
            "⚠️ ADVERTENCIA: Esto reactivará TODOS los productos en la base de datos." & vbCrLf &
            "Incluyendo productos que podrían estar desactivados intencionalmente." & vbCrLf & vbCrLf &
            "¿Estás seguro de que deseas continuar?",
            "Confirmar reactivación TOTAL",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning)

        If confirmacion = DialogResult.No Then
            Return
        End If

        Try
            btnReactivarTodos.Enabled = False
            btnReactivarTodos.Text = "Procesando..."
            Application.DoEvents()

            Dim afectados As Integer = UtilReactivarProductos.ReactivarTodos()

            MessageBox.Show(
                $"✓ Se reactivaron {afectados} productos.",
                "Completado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)
            ActualizarEstado()

        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            btnReactivarTodos.Enabled = True
            btnReactivarTodos.Text = "Reactivar TODOS"
        End Try

    End Sub

    Private Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        Me.Close()
    End Sub

End Class

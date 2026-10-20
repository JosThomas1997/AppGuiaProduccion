Option Strict On

Imports System.Drawing.Printing

Public Module PrinterHelper
    ''' <summary>
    ''' Obtiene la lista de impresoras disponibles en la computadora
    ''' </summary>
    ''' <returns>Array con los nombres de las impresoras</returns>
    Public Function ObtenerImpresorasDisponibles() As String()
        Try
            Dim impresoras As New List(Of String)()

            ' Agregar la impresora predeterminada primero
            Dim impresoras_instaladas = PrinterSettings.InstalledPrinters
            For Each impresora As String In impresoras_instaladas
                impresoras.Add(impresora)
            Next

            Return impresoras.ToArray()
        Catch ex As Exception
            Return New String() {}
        End Try
    End Function

    ''' <summary>
    ''' Llena un ComboBox con las impresoras disponibles
    ''' </summary>
    ''' <param name="comboBox">El ComboBox a llenar</param>
    ''' <param name="impresora_Predeterminada">Nombre de la impresora a seleccionar por defecto (opcional)</param>
    Public Sub CargarImpresorasEnCombo(comboBox As ComboBox, Optional impresora_Predeterminada As String = "")
        Try
            comboBox.Items.Clear()

            Dim impresoras = ObtenerImpresorasDisponibles()

            If impresoras.Length = 0 Then
                comboBox.Items.Add("No hay impresoras disponibles")
                comboBox.Enabled = False
                comboBox.SelectedIndex = 0
                Return
            End If

            ' Agregar todas las impresoras
            For Each impresora As String In impresoras
                comboBox.Items.Add(impresora)
            Next

            ' Seleccionar la impresora especificada o la predeterminada
            If Not String.IsNullOrEmpty(impresora_Predeterminada) AndAlso comboBox.Items.Contains(impresora_Predeterminada) Then
                comboBox.SelectedItem = impresora_Predeterminada
            Else
                ' Intentar seleccionar la impresora predeterminada del sistema
                Dim impresora_Sistema As New PrinterSettings()
                If comboBox.Items.Contains(impresora_Sistema.PrinterName) Then
                    comboBox.SelectedItem = impresora_Sistema.PrinterName
                Else
                    ' Si no, seleccionar la primera
                    comboBox.SelectedIndex = 0
                End If
            End If

            comboBox.Enabled = True
        Catch ex As Exception
            comboBox.Items.Clear()
            comboBox.Items.Add("Error al cargar impresoras")
            comboBox.Enabled = False
            comboBox.SelectedIndex = 0
        End Try
    End Sub
End Module

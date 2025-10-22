Imports System.Security.AccessControl
Imports System.Threading
Imports System.Windows.Forms

Module Main
    <STAThread()>
    Sub Main()
        'Declaracion de elementos para instancia unica
        Dim mutex As Mutex = Nothing
        Dim AppNombre As String = "Global\PanpasGuiasDespacho"

        'Creacion de Mutex para instancia unica (Evitar mutiples aperturas del programa.)
        Try
            'Si el programa detecta que la aplicacion ya se encuentra abierta, mostrar mensaje
            mutex = New Mutex(True, AppNombre, createdNew:=Nothing)
            If Not mutex.WaitOne(0, False) Then
                MessageBox.Show("El programa se encuentra ejecutado!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            'Si no hay otra instancia abierta, iniciar la aplicacion
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            Application.Run(New Form1())

        Finally
            'Cuando el programa se cierre, liberar el Mutex para futuras aperturas
            If mutex IsNot Nothing Then
                mutex.ReleaseMutex()
                mutex.Dispose()
            End If
        End Try
    End Sub
End Module

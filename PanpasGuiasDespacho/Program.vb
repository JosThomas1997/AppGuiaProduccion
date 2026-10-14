Imports System.Threading

Module Program
    <STAThread()>
    Sub Main()
        Dim mutex As New Mutex(True, "PanpasAppMutex", createdNew:=False)

        If mutex.WaitOne(TimeSpan.Zero, True) Then
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)

            'Respaldo del estado con que se abre el dia (una vez por dia)
            Respaldos.CrearRespaldoAutomatico("inicio", False)

            Application.Run(New FormInicio())
            CerrarConexionApp() ' Recien aqui se cierra la conexion de verdad, al salir del programa

            'Respaldo del estado final de la jornada (se actualiza en cada cierre)
            Data.OleDb.OleDbConnection.ReleaseObjectPool()
            Respaldos.CrearRespaldoAutomatico("cierre", True)

            mutex.ReleaseMutex()
        Else
            MessageBox.Show("El programa ya se encuentra en ejecución.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If
    End Sub
End Module
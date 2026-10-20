' Los siguientes eventos están disponibles para MyApplication:
' Inicio: Se genera cuando se inicia la aplicación, antes de que se cree el formulario de inicio.
' Apagado: Se genera después de haberse cerrado todos los formularios de aplicación.  Este evento no se genera si la aplicación termina de forma anómala.
' UnhandledException: Se genera si la aplicación encuentra una excepción no controlada.
' StartupNextInstance: Se genera cuando se inicia una aplicación de instancia única y dicha aplicación está ya activa. 
' NetworkAvailabilityChanged: Se genera cuando se conecta o desconecta la conexión de red.
Option Strict On

Namespace My
    ' Eventos disponibles: Startup, Shutdown, UnhandledException, StartupNextInstance, NetworkAvailabilityChanged
    Partial Friend Class MyApplication

        ' Al abrir el programa: respaldo del estado con que empieza el dia (una vez por dia)
        Private Sub MyApplication_Startup(sender As Object, e As ApplicationServices.StartupEventArgs) Handles Me.Startup
            Respaldos.CrearRespaldoAutomatico("inicio", False)
        End Sub

        ' Al cerrar el programa: se cierra la conexion de verdad y se guarda el estado final de la jornada
        Private Sub MyApplication_Shutdown(sender As Object, e As EventArgs) Handles Me.Shutdown
            CerrarConexionApp()
            System.Data.OleDb.OleDbConnection.ReleaseObjectPool()
            Respaldos.CrearRespaldoAutomatico("cierre", True)
        End Sub

    End Class
End Namespace
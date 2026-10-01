Imports System.Data.OleDb

Module Conexion
    'Ruta para la DB
    Public rutaDB As String = Application.StartupPath & "\DBApp\rjc_prod_f753.mdb"
    Public conexionDB As New OleDbConnection("Provider = Microsoft.Jet.OLEDB.4.0;Data Source=" & rutaDB)

    'Abrir Conexion (no hace nada si ya esta abierta)
    Public Sub AbrirConexion()
        Try
            If conexionDB.State = ConnectionState.Closed Then
                conexionDB.Open()
            End If
        Catch ex As Exception
            MessageBox.Show("Error al abrir la Conexion: " & ex.Message)
        End Try
    End Sub

    ' "Cerrar" Conexion: a proposito, ya NO cierra la conexion en cada llamado.
    ' Antes, cada operacion abria y cerraba la conexion por su cuenta (decenas de veces
    ' por sesion), lo cual tiene un costo real en PC modestas. Ahora la conexion se
    ' mantiene abierta y se reutiliza durante toda la sesion del programa; recien se
    ' cierra de verdad al salir de la aplicacion, con CerrarConexionApp() (ver Program.vb).
    Public Sub CerrarConexion()
        ' Intencionalmente vacio. Se deja el metodo (en vez de borrarlo) para no tener
        ' que tocar cada formulario que ya lo llama.
    End Sub

    ''' <summary>
    ''' Cierra la conexion de verdad. Llamar SOLO al salir de la aplicacion (Program.vb).
    ''' </summary>
    Public Sub CerrarConexionApp()
        Try
            If conexionDB.State = ConnectionState.Open Then
                conexionDB.Close()
            End If
        Catch ex As Exception
            MessageBox.Show("Error al cerrar la Conexion: " & ex.Message)
        End Try
    End Sub
End Module

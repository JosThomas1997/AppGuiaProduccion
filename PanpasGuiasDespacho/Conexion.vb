Imports System.Data.OleDb

Module Conexion
    'Ruta para la DB
    Public rutaDB As String = Application.StartupPath & "\DBApp\rjc_prod_f753.mdb"
    Public conexionDB As New OleDbConnection("Provider = Microsoft.Jet.OLEDB.4.0;Data Source=" & rutaDB)

    'Abrir Conexion (no hace nada si ya esta abierta)
    Public Sub AbrirConexion()
        Try
            'Una conexion rota se cierra primero para poder reabrirla
            If conexionDB.State = ConnectionState.Broken Then conexionDB.Close()
            If conexionDB.State = ConnectionState.Closed Then
                conexionDB.Open()
            End If
        Catch ex As Exception
            MessageBox.Show("Error al abrir la Conexion: " & ex.Message)
        End Try
    End Sub

    Public Sub CerrarConexion()

    End Sub

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

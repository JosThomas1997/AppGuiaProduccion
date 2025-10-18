Imports System.Data.OleDb

Module Conexion
    'Ruta para la DB
    Public rutaDB As String = Application.StartupPath & "\DBApp\rjc_prod_f753.mdb"
    Public conexionDB As New OleDbConnection("Provider = Microsoft.Jet.OLEDB.4.0;Data Source=" & rutaDB)

    'Abrir Conexion
    Public Sub AbrirConexion()
        Try
            If conexionDB.State = ConnectionState.Closed Then
                conexionDB.Open()
            End If
        Catch ex As Exception
            MessageBox.Show("Error al abrir la Conexion: " & ex.Message)
        End Try
    End Sub

    'Cerrar Conexion
    Public Sub CerrarConexion()
        Try
            If conexionDB.State = ConnectionState.Open Then
                conexionDB.Close()
            End If
        Catch ex As Exception
            MessageBox.Show("Error al Cerrar la Conexion: " & ex.Message)
        End Try
    End Sub
End Module

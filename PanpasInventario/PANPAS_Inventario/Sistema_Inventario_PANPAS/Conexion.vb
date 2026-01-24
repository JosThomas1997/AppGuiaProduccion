Imports System.Data.OleDb

Module Conexion

    Public conexionDB As OleDbConnection

    Private ReadOnly rutaDB As String =
        Application.StartupPath & "\InventarioDB\InventarioPANPAS.mdb"

    Public Sub AbrirConexionDB()

        If conexionDB Is Nothing Then
            conexionDB = New OleDbConnection(
                "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & rutaDB)
        End If

        If conexionDB.State <> ConnectionState.Open Then
            conexionDB.Open()
        End If

    End Sub

    Public Sub CerrarConexionDB()

        If conexionDB IsNot Nothing AndAlso
           conexionDB.State = ConnectionState.Open Then
            conexionDB.Close()
        End If

    End Sub

End Module

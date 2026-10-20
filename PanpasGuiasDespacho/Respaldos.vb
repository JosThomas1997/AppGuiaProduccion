Option Strict On

Imports System.IO

Module Respaldos
    ' Cuantos respaldos automaticos se conservan (los mas nuevos). Con 2 por dia, 20 = 10 dias.
    Private Const MaximoRespaldosAutomaticos As Integer = 20

    ' Copia la base a la carpeta "Respaldos" que esta junto a ella.
    '   etiqueta: "inicio" (una vez al dia, no pisa) o "cierre" (pisa la del dia).
    ' Nunca debe impedir que el programa abra o cierre: cualquier error se ignora.
    Public Sub CrearRespaldoAutomatico(etiqueta As String, pisarSiExiste As Boolean)
        Try
            If Not File.Exists(rutaDB) Then Return

            Dim carpeta As String = Path.Combine(Path.GetDirectoryName(rutaDB), "Respaldos")
            Directory.CreateDirectory(carpeta)

            Dim nombreBase As String = Path.GetFileNameWithoutExtension(rutaDB)
            Dim destino As String = Path.Combine(carpeta,
                nombreBase & " - auto " & Date.Today.ToString("yyyy-MM-dd") & " " & etiqueta & ".mdb")

            If File.Exists(destino) AndAlso Not pisarSiExiste Then Return

            File.Copy(rutaDB, destino, True)
            File.SetLastWriteTime(destino, Date.Now)
            LimpiarRespaldosAntiguos(carpeta, nombreBase)

        Catch ex As Exception
            Debug.WriteLine("No se pudo crear el respaldo automatico: " & ex.Message)
        End Try
    End Sub

    ' Borra los respaldos automaticos mas viejos. Solo toca archivos "<base> - auto *.mdb"
    ' dentro de la carpeta Respaldos: jamas la base ni los respaldos manuales.
    Private Sub LimpiarRespaldosAntiguos(carpeta As String, nombreBase As String)
        Dim archivos As FileInfo() = New DirectoryInfo(carpeta).GetFiles(nombreBase & " - auto *.mdb")
        Array.Sort(archivos, Function(a, b) String.Compare(b.Name, a.Name, StringComparison.Ordinal))

        For i As Integer = MaximoRespaldosAutomaticos To archivos.Length - 1
            Try
                archivos(i).Delete()
            Catch
                'Si un archivo no se puede borrar, se deja
            End Try
        Next
    End Sub
End Module
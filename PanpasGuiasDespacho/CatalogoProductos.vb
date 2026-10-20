Option Strict On

Imports System.Data.OleDb
Module CatalogoProductos
    ' Catalogo centralizado de tipos de torta y sus codigos de producto.
    ' Antes esta misma lista estaba duplicada en FormProductos.vb, FormProducc_Productos.vb
    ' y FormInformeProducto.vb. Ahora vive en un solo lugar: para agregar, quitar o corregir
    ' un tipo de torta (o sus codigos), solo hay que editar aqui. 

    Public ReadOnly tiposTorta As String() = {
        "Holandesas", "Yoghurt", "Dulce de Leche", "Chocolate", "Piña", "Selva Negra",
        "Hoja Manjar", "Lucuma", "Tres Leches", "Mocka", "Panqueque", "Durazno",
        "Bizcocho Manjar", "Panpas"
    }

    ' Convierte un precio de la base a entero. Nulo, vacio o invalido = 0.
    Public Function PrecioEntero(valor As Object) As Integer
        If valor Is Nothing OrElse IsDBNull(valor) Then Return 0
        Try
            Return Convert.ToInt32(Convert.ToDecimal(valor))
        Catch
            Return 0
        End Try
    End Function

    ' Convierte un monto mostrado ("$ 1.234", "$ 1,234", "1234") a entero, sin depender de la
    ' configuracion regional: solo se conservan los digitos. Vacio o invalido = 0.
    Public Function MontoEntero(monto As Object) As Integer
        If monto Is Nothing OrElse IsDBNull(monto) Then Return 0

        'Si ya es un numero, no se pasa por texto (un decimal "1500,00" se leeria como 150000)
        If TypeOf monto Is Integer Then Return CInt(monto)
        If TypeOf monto Is Long OrElse TypeOf monto Is Decimal OrElse TypeOf monto Is Double Then
            Try
                Return Convert.ToInt32(monto)
            Catch
                Return 0
            End Try
        End If

        Dim digitos As New System.Text.StringBuilder()
        For Each c As Char In monto.ToString()
            If Char.IsDigit(c) Then digitos.Append(c)
        Next

        Dim valor As Integer
        If digitos.Length > 0 AndAlso Integer.TryParse(digitos.ToString(), valor) Then Return valor
        Return 0
    End Function

    ' Tortas de un tipo (subcategoria).
    Public Function ObtenerTortasPorSubcategoria(subcategoria As String, soloActivos As Boolean) As DataTable
        Dim sql As String = "SELECT codigo, nombre, precio_venta_boleta FROM producto " &
                            "WHERE categoria = 'Tortas' AND subcategoria = ? "
        If soloActivos Then sql &= "AND (productoActivo = True OR productoActivo IS NULL) "
        sql &= "ORDER BY codigo"
        Return EjecutarConsultaProductos(sql, subcategoria)
    End Function

    ' Productos de una categoria. soloActivos = True oculta los desactivados
    ' (paneles de Guias y Produccion); False los incluye (Informes).
    Public Function ObtenerProductosPorCategoria(categoria As String, soloActivos As Boolean) As DataTable
        Dim sql As String = "SELECT codigo, nombre, precio_venta_boleta FROM producto WHERE categoria = ? "
        If soloActivos Then sql &= "AND (productoActivo = True OR productoActivo IS NULL) "
        sql &= "ORDER BY codigo"
        Return EjecutarConsultaProductos(sql, categoria)
    End Function

    ' Productos de "Otros" y los que aun no tienen categoria.
    Public Function ObtenerProductosOtros(soloActivos As Boolean) As DataTable
        Dim sql As String = "SELECT codigo, nombre, precio_venta_boleta FROM producto " &
                            "WHERE (categoria = 'Otros' OR categoria IS NULL OR categoria = '') "
        If soloActivos Then sql &= "AND (productoActivo = True OR productoActivo IS NULL) "
        sql &= "ORDER BY nombre"
        Return EjecutarConsultaProductos(sql, Nothing)
    End Function

    Private Function EjecutarConsultaProductos(sql As String, parametro As String) As DataTable
        Dim tabla As New DataTable()
        AbrirConexion()
        Try
            Using cmd As New OleDbCommand(sql, conexionDB)
                If parametro IsNot Nothing Then cmd.Parameters.AddWithValue("?", parametro)
                Using reader As OleDbDataReader = cmd.ExecuteReader()
                    tabla.Load(reader)
                End Using
            End Using
        Finally
            CerrarConexion()
        End Try
        Return tabla
    End Function

End Module
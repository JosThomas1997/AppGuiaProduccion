Imports System.Data.OleDb

Public Class FormEditarPrecios

    Private codigoSeleccionado As String = Nothing
    Private productoSeleccionadoActivo As Boolean = True

    Private Sub FormEditarPrecios_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CenterToScreen()
        CargarProductos()
        CargarCategoriasDisponibles()
    End Sub

    Private Sub CargarCategoriasDisponibles()
        Try
            AbrirConexion()

            cbCategoriaNueva.Items.Clear()
            Using cmd As New OleDbCommand("SELECT DISTINCT categoria FROM producto WHERE categoria IS NOT NULL ORDER BY categoria", conexionDB)
                Using reader As OleDbDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        cbCategoriaNueva.Items.Add(reader("categoria").ToString())
                    End While
                End Using
            End Using

            cbSubcategoriaNueva.Items.Clear()
            Using cmd As New OleDbCommand("SELECT DISTINCT subcategoria FROM producto WHERE subcategoria IS NOT NULL ORDER BY subcategoria", conexionDB)
                Using reader As OleDbDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        cbSubcategoriaNueva.Items.Add(reader("subcategoria").ToString())
                    End While
                End Using
            End Using

        Catch ex As Exception
            MsgBox("Error al cargar las categorías: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try
    End Sub

    Private Sub cbCategoriaNueva_TextChanged(sender As Object, e As EventArgs) Handles cbCategoriaNueva.TextChanged
        Dim esTorta As Boolean = cbCategoriaNueva.Text.Trim().Equals("Tortas", StringComparison.OrdinalIgnoreCase)
        cbSubcategoriaNueva.Enabled = esTorta
        If Not esTorta Then cbSubcategoriaNueva.Text = ""
    End Sub

    Private Sub CargarProductos(Optional filtro As String = "")
        dgvResultados.SuspendLayout()
        Try
            AbrirConexion()

            Dim sql As String = "SELECT codigo, nombre, precio_venta_boleta, productoActivo FROM producto WHERE 1=1 "
            If chkMostrarInactivos.Checked Then
                'Marcado: solo los inactivos
                sql &= "AND productoActivo = ? "
            Else
                'Desmarcado: solo los activos. Se incluye "OR IS NULL" para los productos antiguos
                'con la columna en blanco, que se tratan como activos.
                sql &= "AND (productoActivo = ? OR productoActivo IS NULL) "
            End If
            If Not String.IsNullOrWhiteSpace(filtro) Then
                sql &= "AND (codigo LIKE ? OR nombre LIKE ?) "
            End If
            sql &= "ORDER BY nombre"

            Using cmd As New OleDbCommand(sql, conexionDB)
                'El orden de los parametros debe calzar con el orden de los "?" en el SQL de arriba
                cmd.Parameters.AddWithValue("?", Not chkMostrarInactivos.Checked)
                If Not String.IsNullOrWhiteSpace(filtro) Then
                    cmd.Parameters.AddWithValue("?", "%" & filtro & "%")
                    cmd.Parameters.AddWithValue("?", "%" & filtro & "%")
                End If

                Using reader As OleDbDataReader = cmd.ExecuteReader()
                    dgvResultados.Rows.Clear()
                    While reader.Read()
                        Dim codigo As String = reader("codigo").ToString()
                        Dim nombre As String = reader("nombre").ToString()
                        Dim precio As Integer = CatalogoProductos.PrecioEntero(reader("precio_venta_boleta"))

                        ' Si productoActivo viene en NULL (producto antiguo, de antes de que
                        ' existiera esta columna), se trata como activo por defecto en vez
                        ' de fallar -- para no dejar la lista entera sin mostrar nada.
                        Dim estaActivo As Boolean = IsDBNull(reader("productoActivo")) OrElse Convert.ToBoolean(reader("productoActivo"))
                        Dim textoEstado As String = If(estaActivo, "Activo", "Inactivo")

                        Dim filaIndex As Integer = dgvResultados.Rows.Add(codigo, nombre, String.Format("$ {0:N0}", precio), textoEstado)
                        dgvResultados.Rows(filaIndex).Tag = estaActivo
                    End While
                End Using
            End Using

        Catch ex As Exception
            MsgBox("Error al cargar los productos: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
            dgvResultados.ResumeLayout()
        End Try
    End Sub

    Private Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        CargarProductos(txtBuscar.Text.Trim())
    End Sub

    Private Sub txtBuscar_TextChanged(sender As Object, e As EventArgs) Handles txtBuscar.TextChanged
        CargarProductos(txtBuscar.Text.Trim())
    End Sub

    Private Sub txtBuscar_KeyDown(sender As Object, e As KeyEventArgs) Handles txtBuscar.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            CargarProductos(txtBuscar.Text.Trim())
        End If
    End Sub

    Private Sub chkMostrarInactivos_CheckedChanged(sender As Object, e As EventArgs) Handles chkMostrarInactivos.CheckedChanged
        CargarProductos(txtBuscar.Text.Trim())
    End Sub

    Private Sub dgvResultados_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvResultados.CellClick
        If e.RowIndex < 0 Then Exit Sub

        Dim fila = dgvResultados.Rows(e.RowIndex)
        codigoSeleccionado = fila.Cells("codigo").Value.ToString()
        productoSeleccionadoActivo = Convert.ToBoolean(fila.Tag)

        lblProductoSeleccionado.Text = "Producto: " & fila.Cells("nombre").Value.ToString() & " (Código " & codigoSeleccionado & ")"

        txtNuevoNombreEditar.Text = fila.Cells("nombre").Value.ToString()

        ' Extraer precio de forma más robusta: remover símbolos de formato
        Dim precioFormato As String = fila.Cells("precio").Value.ToString()
        ' Remover $ y espacios, luego puntos de formato (1.234 -> 1234)
        Dim precioLimpio As String = CatalogoProductos.MontoEntero(precioFormato).ToString()
        txtNuevoPrecio.Text = precioLimpio

        btnGuardar.Enabled = True
        btnEliminarReactivar.Enabled = True
        btnEliminarReactivar.Text = If(productoSeleccionadoActivo, "Eliminar producto", "Reactivar producto")
    End Sub

    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If String.IsNullOrEmpty(codigoSeleccionado) Then
            MsgBox("Primero selecciona un producto de la lista.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        Dim nombreNuevo As String = txtNuevoNombreEditar.Text.Trim()
        If String.IsNullOrEmpty(nombreNuevo) Then
            MsgBox("El nombre no puede quedar vacío.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        Dim nuevoPrecio As Integer
        If Not Integer.TryParse(txtNuevoPrecio.Text.Trim(), nuevoPrecio) OrElse nuevoPrecio < 0 Then
            MsgBox("El precio debe ser un número entero válido (sin puntos ni símbolos).", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        Dim confirmacion = MsgBox(
            "¿Confirmas actualizar el producto " & codigoSeleccionado & "?" & vbCrLf &
            "Nombre: " & nombreNuevo & vbCrLf &
            "Precio: $ " & nuevoPrecio.ToString("N0") & vbCrLf & vbCrLf &
            "Este cambio se aplicará de inmediato a nuevas guías y producciones.",
            MsgBoxStyle.YesNo Or MsgBoxStyle.Question, "Confirmar cambios")

        If confirmacion <> MsgBoxResult.Yes Then Exit Sub

        Try
            AbrirConexion()

            Using cmd As New OleDbCommand("UPDATE producto SET nombre = ?, precio_venta_boleta = ? WHERE codigo = ?", conexionDB)
                cmd.Parameters.AddWithValue("?", nombreNuevo)
                cmd.Parameters.AddWithValue("?", nuevoPrecio)
                cmd.Parameters.AddWithValue("?", codigoSeleccionado)
                Dim filasAfectadas As Integer = cmd.ExecuteNonQuery()

                If filasAfectadas = 0 Then
                    MsgBox("No se encontró el producto con código " & codigoSeleccionado & ". No se modificó nada.", MsgBoxStyle.Exclamation)
                Else
                    MsgBox("Producto actualizado correctamente.", MsgBoxStyle.Information)
                End If
            End Using

        Catch ex As Exception
            MsgBox("Error al actualizar el producto: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try

        LimpiarSeleccion()
        CargarProductos(txtBuscar.Text.Trim())
    End Sub

    ''' <summary>
    ''' Elimina o reactiva el producto seleccionado, segun su estado actual.
    ''' Si el producto ya se uso en alguna guia o produccion, no se borra de verdad
    ''' (para no romper el historial) -- en su lugar se marca como inactivo.
    ''' </summary>
    Private Sub btnEliminarReactivar_Click(sender As Object, e As EventArgs) Handles btnEliminarReactivar.Click
        If String.IsNullOrEmpty(codigoSeleccionado) Then
            MsgBox("Primero selecciona un producto de la lista.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        ' === CASO: reactivar un producto inactivo ===
        If Not productoSeleccionadoActivo Then
            Dim confirmarReactivar = MsgBox(
                "¿Confirmas reactivar el producto " & codigoSeleccionado & "? Volverá a aparecer en los formularios de selección.",
                MsgBoxStyle.YesNo Or MsgBoxStyle.Question, "Confirmar reactivación")

            If confirmarReactivar <> MsgBoxResult.Yes Then Exit Sub

            Try
                AbrirConexion()
                Using cmd As New OleDbCommand("UPDATE producto SET productoActivo = ? WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", True)
                    cmd.Parameters.AddWithValue("?", codigoSeleccionado)
                    cmd.ExecuteNonQuery()
                End Using
                MsgBox("Producto reactivado.", MsgBoxStyle.Information)
            Catch ex As Exception
                MsgBox("Error al reactivar el producto: " & ex.Message, MsgBoxStyle.Critical)
            Finally
                CerrarConexion()
            End Try

            LimpiarSeleccion()
            CargarProductos(txtBuscar.Text.Trim())
            Exit Sub
        End If

        ' === CASO: eliminar (o desactivar) un producto activo ===
        Try
            AbrirConexion()

            ' Revisar si el codigo aparece en el historial de guias o producciones
            Dim tieneHistorial As Boolean = False

            Using cmdGuia As New OleDbCommand("SELECT COUNT(*) FROM guia_detalle WHERE codigo_producto = ?", conexionDB)
                cmdGuia.Parameters.AddWithValue("?", codigoSeleccionado)
                If Convert.ToInt32(cmdGuia.ExecuteScalar()) > 0 Then tieneHistorial = True
            End Using

            If Not tieneHistorial Then
                Using cmdProduccion As New OleDbCommand("SELECT COUNT(*) FROM produccion_detalle WHERE codigo_producto = ?", conexionDB)
                    cmdProduccion.Parameters.AddWithValue("?", codigoSeleccionado)
                    If Convert.ToInt32(cmdProduccion.ExecuteScalar()) > 0 Then tieneHistorial = True
                End Using
            End If

            If tieneHistorial Then
                ' Tiene historial: no se borra de verdad, se desactiva para no romper guias/producciones antiguas
                Dim confirmarDesactivar = MsgBox(
                    "El producto " & codigoSeleccionado & " ya aparece en guías o producciones anteriores." & vbCrLf &
                    "Para no alterar ese historial, no se puede eliminar de verdad." & vbCrLf & vbCrLf &
                    "¿Deseas marcarlo como INACTIVO en su lugar? Dejará de aparecer en los formularios de selección, pero el historial existente no se toca.",
                    MsgBoxStyle.YesNo Or MsgBoxStyle.Question, "Producto con historial")

                If confirmarDesactivar <> MsgBoxResult.Yes Then Exit Sub

                Using cmd As New OleDbCommand("UPDATE producto SET productoActivo = ? WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", False)
                    cmd.Parameters.AddWithValue("?", codigoSeleccionado)
                    cmd.ExecuteNonQuery()
                End Using
                MsgBox("Producto marcado como inactivo.", MsgBoxStyle.Information)

            Else
                ' Sin historial: se puede eliminar de verdad sin riesgo
                Dim confirmarEliminar = MsgBox(
                    "El producto " & codigoSeleccionado & " no tiene historial de uso en guías ni producciones." & vbCrLf &
                    "¿Confirmas ELIMINARLO definitivamente? Esta acción no se puede deshacer.",
                    MsgBoxStyle.YesNo Or MsgBoxStyle.Question, "Confirmar eliminación definitiva")

                If confirmarEliminar <> MsgBoxResult.Yes Then Exit Sub

                Using cmd As New OleDbCommand("DELETE FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigoSeleccionado)
                    cmd.ExecuteNonQuery()
                End Using
                MsgBox("Producto eliminado definitivamente.", MsgBoxStyle.Information)
            End If

        Catch ex As Exception
            MsgBox("Error al eliminar/desactivar el producto: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try

        LimpiarSeleccion()
        CargarProductos(txtBuscar.Text.Trim())
    End Sub

    Private Sub LimpiarSeleccion()
        codigoSeleccionado = Nothing
        productoSeleccionadoActivo = True
        btnGuardar.Enabled = False
        btnEliminarReactivar.Enabled = False
        btnEliminarReactivar.Text = "Eliminar producto"
        lblProductoSeleccionado.Text = "Selecciona un producto de la lista para editarlo"
        txtNuevoNombreEditar.Clear()
        txtNuevoPrecio.Clear()
    End Sub

    Private Sub btnAgregarProducto_Click(sender As Object, e As EventArgs) Handles btnAgregarProducto.Click
        Dim codigoNuevo As String = txtCodigoNuevo.Text.Trim()
        Dim nombreNuevo As String = txtNombreNuevo.Text.Trim()
        Dim categoriaNueva As String = cbCategoriaNueva.Text.Trim()

        If String.IsNullOrEmpty(codigoNuevo) OrElse String.IsNullOrEmpty(nombreNuevo) Then
            MsgBox("Debes ingresar código y nombre del producto.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        If String.IsNullOrEmpty(categoriaNueva) Then
            MsgBox("Debes elegir una categoría de la lista. El producto aparecerá en el panel de esa categoría.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        Dim esTorta As Boolean = categoriaNueva.Equals("Tortas", StringComparison.OrdinalIgnoreCase)
        Dim subcategoriaNueva As String = cbSubcategoriaNueva.Text.Trim()

        If esTorta AndAlso String.IsNullOrEmpty(subcategoriaNueva) Then
            MsgBox("La categoría ""Tortas"" necesita también un sabor (subcategoría).", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        Dim precioNuevo As Integer
        If Not Integer.TryParse(txtPrecioNuevoProducto.Text.Trim(), precioNuevo) OrElse precioNuevo < 0 Then
            MsgBox("El precio debe ser un número entero válido (sin puntos ni símbolos).", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        Try
            AbrirConexion()

            ' Verificar que el codigo no exista ya (evita choques con codigos existentes)
            Using cmdVerificar As New OleDbCommand("SELECT COUNT(*) FROM producto WHERE codigo = ?", conexionDB)
                cmdVerificar.Parameters.AddWithValue("?", codigoNuevo)
                Dim yaExiste As Integer = Convert.ToInt32(cmdVerificar.ExecuteScalar())

                If yaExiste > 0 Then
                    MsgBox("Ya existe un producto con el código " & codigoNuevo & ". Elige un código distinto.", MsgBoxStyle.Exclamation)
                    Exit Sub
                End If
            End Using

            Dim resumenSabor As String = If(esTorta, " (sabor: " & subcategoriaNueva & ")", "")
            Dim confirmacion = MsgBox(
                "¿Confirmas crear el producto """ & nombreNuevo & """ con código " & codigoNuevo & "?" & vbCrLf &
                "Categoría: " & categoriaNueva & resumenSabor & vbCrLf &
                "Precio: $ " & precioNuevo.ToString("N0"),
                MsgBoxStyle.YesNo Or MsgBoxStyle.Question, "Confirmar producto nuevo")

            If confirmacion <> MsgBoxResult.Yes Then Exit Sub

            ' El producto nuevo se crea activo por defecto (True = productoActivo).
            ' La subcategoria queda en NULL para todo lo que no sea "Tortas".
            Dim valorSubcategoria As Object = If(esTorta, CType(subcategoriaNueva, Object), DBNull.Value)

            ' Usar transacción para garantizar que se inserta completamente o no se inserta nada
            Dim transaction As OleDbTransaction = Nothing
            Try
                transaction = conexionDB.BeginTransaction()

                Using cmdInsertar As New OleDbCommand("INSERT INTO producto (codigo, nombre, precio_venta_boleta, productoActivo, categoria, subcategoria) VALUES (?, ?, ?, ?, ?, ?)", conexionDB, transaction)
                    cmdInsertar.Parameters.AddWithValue("?", codigoNuevo)
                    cmdInsertar.Parameters.AddWithValue("?", nombreNuevo)
                    cmdInsertar.Parameters.AddWithValue("?", precioNuevo)
                    cmdInsertar.Parameters.AddWithValue("?", True)
                    cmdInsertar.Parameters.AddWithValue("?", categoriaNueva)
                    cmdInsertar.Parameters.AddWithValue("?", valorSubcategoria)
                    cmdInsertar.ExecuteNonQuery()
                End Using

                transaction.Commit()
                Dim aviso As String = "Producto agregado correctamente. Aparecerá en el panel """ & categoriaNueva & """ para los empleados."

                If esTorta Then
                    'Un sabor nuevo necesita estar en la lista de tipos de torta para tener su boton
                    Dim saborConocido As Boolean = Array.Exists(CatalogoProductos.tiposTorta,
                        Function(t) t.Equals(subcategoriaNueva, StringComparison.OrdinalIgnoreCase))
                    If Not saborConocido Then
                        aviso &= vbCrLf & vbCrLf & "Aviso: el sabor """ & subcategoriaNueva & """ no tiene boton todavia. " &
                                 "Para que aparezca, agregalo a la lista tiposTorta en CatalogoProductos.vb."
                    End If
                End If

                MsgBox(aviso, MsgBoxStyle.Information)

            Catch ex As Exception
                If transaction IsNot Nothing Then
                    transaction.Rollback()
                End If
                Throw ' Re-lanzar excepción para ser manejada por catch externo
            End Try

        Catch ex As Exception
            MsgBox("Error al agregar el producto: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try

        ' Limpiar el formulario y refrescar la lista y las categorías disponibles
        txtCodigoNuevo.Clear()
        txtNombreNuevo.Clear()
        txtPrecioNuevoProducto.Clear()
        cbCategoriaNueva.Text = ""
        cbSubcategoriaNueva.Text = ""
        CargarProductos(txtBuscar.Text.Trim())
        CargarCategoriasDisponibles()
    End Sub

    ''' <summary>
    ''' Abre el formulario para reactivar productos
    ''' Permite gestionar la activación/desactivación de productos en lote
    ''' </summary>
    Private Sub btnReactivarProductos_Click(sender As Object, e As EventArgs)
        Dim formAbierto As Boolean = False

        ' Verifica si el formulario ya está abierto
        For Each form As Form In Application.OpenForms
            If TypeOf form Is FormReactivarProductos Then
                formAbierto = True
                form.BringToFront()
                Exit For
            End If
        Next

        If formAbierto Then
            MessageBox.Show("Reactivar Productos se encuentra en ejecución.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            Dim form As New FormReactivarProductos()
            form.Show()
        End If
    End Sub

End Class

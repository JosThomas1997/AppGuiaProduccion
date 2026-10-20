Option Strict On

Imports System.IO
Imports System.Data.OleDb

Public Class FormProducc_Productos

    Private Sub FormProducc_Productos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciarPanelProductos()
    End Sub

    Private Sub IniciarPanelProductos()
        panelIzquierdo.Controls.Clear()

        'Ubicacion de los iconos en las carpetas
        Dim carpetaIconos As String = Path.Combine(Application.StartupPath, "Images")

        Dim productos As (Nombre As String, Archivo As String)() = {
            ("Tortas", "tortas.png"),
            ("Empanadas", "empanadas.png"),
            ("Masas Dulces", "masasdulces.png"),
            ("Kuchen", "kuchen.png"),
            ("Galletas", "galletas.png"),
            ("Cocadas", "cocadas.png"),
            ("Alfajores", "alfajores.png"),
            ("Brazos de Reina", "brazosdereina.png"),
            ("Queques y Muffins", "quequesymuffins.png"),
            ("Pasteles", "pasteles.png"),
            ("Materia Prima", "placeholder.png"),
            ("Pan de Completo", "pancompleto.png"),
            ("Mote con Huesillo", "motehuesillo.png"),
            ("Pan de Pascua", "panpascua.png"),
            ("Otros", "placeholder.png")
        }
        Dim posicionY As Integer = 10

        'Botones para los productos
        For Each ico In productos
            Dim rutaIcono As String = Path.Combine(carpetaIconos, ico.Archivo)

            'Boton con diseño personalizado
            Dim button As New Button()
            button.Name = "Button" & ico.Nombre.Replace(" ", "")
            button.Text = ico.Nombre
            button.FlatStyle = FlatStyle.Flat
            button.BackColor = Color.White
            button.Size = New Size(120, 100)
            button.Location = New Point(5, posicionY)
            button.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            button.FlatAppearance.BorderSize = 0
            button.Tag = ico.Nombre 'Almacenar el nombre del producto en el Tag


            'Carga de las imagenes de los botones
            If File.Exists(rutaIcono) Then
                Using img As Image = Image.FromFile(rutaIcono)
                    button.Image = New Bitmap(img, New Size(64, 64))
                End Using
            Else
                'En caso de no existir un icono
                button.Text &= vbCrLf & "(Sin Icono)"
            End If

            'Conectar el evento del click de los iconos
            AddHandler button.Click, AddressOf btnPanelIzquierdo_Click

            'Agregar el boton al panel
            panelIzquierdo.Controls.Add(button)

            'Incrementar posicion vertical
            posicionY += button.Height + 8
        Next

        'Activar el Scroll para los iconos
        panelIzquierdo.AutoScroll = True
    End Sub

    'Evento click en los botones de los productos
    Private Sub btnPanelIzquierdo_Click(sender As Object, e As EventArgs)
        Dim boton As Button = CType(sender, Button)
        Dim nombreProducto As String = boton.Tag.ToString().Trim()

        Select Case nombreProducto
            Case "Tortas"
                btnTortas_Click(sender, e)
            Case "Otros"
                MostrarProductosOtros()
            Case Else
                'Cualquier otra categoria se consulta directamente en la base
                MostrarProductosPorCategoria(nombreProducto)
        End Select

        'Traer panel al frente y hacerlo visible
        dgvProducProductos.Visible = True
        dgvProducProductos.BringToFront()
    End Sub

    'Productos de la categoria "Otros" y los que aun no tienen categoria
    Private Sub MostrarProductosOtros()
        Try
            MostrarEnGrid(CatalogoProductos.ObtenerProductosOtros(True), "Otros")
        Catch ex As Exception
            LimpiarPanelCentro()
            MsgBox("Error al cargar productos: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    'Productos activos de una categoria (consulta por la columna "categoria" de la base)
    Private Sub MostrarProductosPorCategoria(categoria As String)
        Try
            MostrarEnGrid(CatalogoProductos.ObtenerProductosPorCategoria(categoria, True), categoria)
        Catch ex As Exception
            LimpiarPanelCentro()
            MsgBox("Error al cargar productos: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    'Dibuja los productos en la grilla y avisa si la categoria esta vacia
    Private Sub MostrarEnGrid(tabla As DataTable, nombreCategoria As String, Optional conservarPanelCentral As Boolean = False)
        If conservarPanelCentral Then
            dgvProducProductos.Columns.Clear()
            dgvProducProductos.Rows.Clear()
        Else
            LimpiarPanelCentro()
        End If

        dgvProducProductos.Columns.Add("codigo", "Codigo")
        dgvProducProductos.Columns.Add("nombre", "Nombre")
        dgvProducProductos.Columns.Add("precio", "Precio")

        For Each col As DataGridViewColumn In dgvProducProductos.Columns
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Next

        For Each fila As DataRow In tabla.Rows
            Dim precio As Integer = If(IsDBNull(fila("precio_venta_boleta")), 0, Convert.ToInt32(fila("precio_venta_boleta")))
            dgvProducProductos.Rows.Add(fila("codigo").ToString(), fila("nombre").ToString(), String.Format("$ {0:N0}", precio))
        Next

        If dgvProducProductos.Rows.Count = 0 Then
            MessageBox.Show("No hay productos activos en la categoria: " & nombreCategoria,
                            "!", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        dgvProducProductos.Visible = True
        dgvProducProductos.BringToFront()
    End Sub

    '==================================================================
    'BOTONES - PRODUCTOS DEL PANEL IZQUIERDO
    '==================================================================

    Private Sub btnTortas_Click(sender As Object, e As EventArgs)
        LimpiarPanelCentro()

        'Un boton por cada tipo de torta
        Dim posY As Integer = 10
        For Each nombreTipo In CatalogoProductos.tiposTorta
            Dim botonTipo As New Button()
            botonTipo.Text = nombreTipo
            botonTipo.Tag = nombreTipo
            botonTipo.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            botonTipo.Size = New Size(180, 40)
            botonTipo.Location = New Point(10, posY)
            botonTipo.BackColor = Color.FromArgb(255, 242, 204)
            botonTipo.FlatStyle = FlatStyle.Flat
            botonTipo.FlatAppearance.BorderSize = 1

            AddHandler botonTipo.Click, AddressOf MostrarTortasTipo
            panelCentral.Controls.Add(botonTipo)
            posY += 50
        Next

        panelCentral.AutoScroll = True
    End Sub

    Private Sub MostrarTortasTipo(sender As Object, e As EventArgs)
        Dim tipo As String = CType(sender, Button).Tag.ToString()
        Try
            MostrarEnGrid(CatalogoProductos.ObtenerTortasPorSubcategoria(tipo, True), tipo, True)
        Catch ex As Exception
            MsgBox("Error al cargar la torta: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    'Limpieza del panel central ante cambio de producto
    Private Sub LimpiarPanelCentro()
        panelCentral.Controls.Clear()
        dgvProducProductos.Columns.Clear()
        dgvProducProductos.Rows.Clear()
    End Sub

    'Doble click en las celdas de los productos para ser agregados al grid
    Private Sub dgvProducProductos_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProducProductos.CellDoubleClick
        If e.RowIndex < 0 Then Exit Sub

        Try
            'Asegurar de que la fila elegida haya de sido valida.
            If e.RowIndex < 0 Then Exit Sub

            'Tomar los datos de la fila seleccionada 
            Dim codigo As String = dgvProducProductos.Rows(e.RowIndex).Cells("codigo").Value.ToString()
            Dim nombre As String = dgvProducProductos.Rows(e.RowIndex).Cells("nombre").Value.ToString()

            'Limpiar el precio de venta boleta y convertir el mismo a entero
            Dim precioTexto As String = dgvProducProductos.Rows(e.RowIndex).Cells("precio").Value.ToString()
            precioTexto = CatalogoProductos.MontoEntero(precioTexto).ToString()

            Dim precio As Integer
            If Not Integer.TryParse(precioTexto, precio) Then
                MsgBox("El precio no es valido: " & precioTexto, MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            'Asegurar de que el formulario haya sido abierto en el Form2
            If TypeOf Owner Is Form2 Then
                Dim formProduccion As Form2 = CType(Owner, Form2)

                'Sin produccion abierta no hay donde guardar el producto
                If Not formProduccion.ProduccionAbierta() Then
                    MsgBox("No hay una producción abierta. Cierra esta ventana y pulsa «Nueva producción» primero.", MsgBoxStyle.Information)
                    Exit Sub
                End If

                'Agregar los productos al grid del Form2 (la quinta columna es el precio unitario real, oculta)
                formProduccion.dgvProduccion.Rows.Add(
                    codigo,
                    nombre,
                    1,
                    String.Format("$ {0:N0}", precio),
                    precio
                )

                'Calcular el total y guardar automaticamente los productos agregados
                formProduccion.CalcularTotalProduccion()
                formProduccion.GuardarProduccion()
            End If

            'Al agregar el producto, cerrar el formulario
            Me.Close()
        Catch ex As Exception
            MsgBox("Error al agregar el producto: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

End Class
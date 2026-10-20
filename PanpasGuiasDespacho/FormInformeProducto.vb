Option Strict On

Imports System.Data.OleDb
Imports System.IO

Public Class FormInformeProducto

    Public MostrarTodosLosProductos As Boolean = False

    Private Sub FormInformeProducto_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciarPanelProductos()
        AddHandler txtCodigoProducto.KeyDown, AddressOf txtCodigoProducto_KeyDown
    End Sub

    Private Sub IniciarPanelProductos()
        panelIconos.Controls.Clear()

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

        Dim btnAncho As Integer = 120
        Dim btnAlto As Integer = 100
        Dim btnMargen As Integer = 10

        Dim columnas As Integer = Math.Max(1, panelIconos.Width \ (btnAncho + btnMargen))

        Dim ancho As Integer = 0
        Dim alto As Integer = 0
        Dim columna As Integer = 0

        For Each ico In productos
            Dim rutaIconos As String = Path.Combine(carpetaIconos, ico.Archivo)

            Dim btn As New Button()
            btn.Size = New Size(btnAncho, btnAlto)
            btn.Location = New Point(ancho, alto)
            btn.FlatStyle = FlatStyle.Flat
            btn.FlatAppearance.BorderSize = 0
            btn.BackColor = Color.White
            btn.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            btn.Text = ico.Nombre
            btn.Tag = ico.Nombre
            btn.TextImageRelation = TextImageRelation.ImageAboveText

            If File.Exists(rutaIconos) Then
                Using img As Image = Image.FromFile(rutaIconos)
                    btn.Image = New Bitmap(img, New Size(64, 64))
                End Using
            End If

            AddHandler btn.Click, AddressOf btnpanelIconos_Click
            panelIconos.Controls.Add(btn)

            columna += 1
            If columna >= columnas Then
                columna = 0
                ancho = 0
                alto += btnAlto + btnMargen
            Else
                ancho += btnAncho + btnMargen
            End If

            panelIconos.AutoScroll = True
        Next

    End Sub

    Private Sub btnpanelIconos_Click(sender As Object, e As EventArgs)
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
        dgvInformeProductos.Visible = True
        dgvInformeProductos.BringToFront()
    End Sub

    'Productos de la categoria "Otros" y los que aun no tienen categoria (incluye inactivos)
    Private Sub MostrarProductosOtros()
        Try
            MostrarEnGrid(CatalogoProductos.ObtenerProductosOtros(False), "Otros")
        Catch ex As Exception
            LimpiarPanelCentro()
            MsgBox("Error al cargar productos: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    'Todos los productos de una categoria (incluye inactivos: un informe debe ver el historial)
    Private Sub MostrarProductosPorCategoria(categoria As String)
        Try
            MostrarEnGrid(CatalogoProductos.ObtenerProductosPorCategoria(categoria, False), categoria)
        Catch ex As Exception
            LimpiarPanelCentro()
            MsgBox("Error al cargar productos: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    'Dibuja los productos en la grilla y avisa si la categoria esta vacia
    Private Sub MostrarEnGrid(tabla As DataTable, nombreCategoria As String, Optional conservarPanelCentral As Boolean = False)
        If conservarPanelCentral Then
            dgvInformeProductos.Columns.Clear()
            dgvInformeProductos.Rows.Clear()
        Else
            LimpiarPanelCentro()
        End If

        dgvInformeProductos.Columns.Add("codigo", "Codigo")
        dgvInformeProductos.Columns.Add("nombre", "Nombre")
        dgvInformeProductos.Columns.Add("precio", "Precio")

        For Each col As DataGridViewColumn In dgvInformeProductos.Columns
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Next

        For Each fila As DataRow In tabla.Rows
            Dim precio As Integer = If(IsDBNull(fila("precio_venta_boleta")), 0, Convert.ToInt32(fila("precio_venta_boleta")))
            dgvInformeProductos.Rows.Add(fila("codigo").ToString(), fila("nombre").ToString(), String.Format("$ {0:N0}", precio))
        Next

        If dgvInformeProductos.Rows.Count = 0 Then
            MessageBox.Show("No hay productos en la categoria: " & nombreCategoria,
                            "!", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        dgvInformeProductos.Visible = True
        dgvInformeProductos.BringToFront()
    End Sub

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
            MostrarEnGrid(CatalogoProductos.ObtenerTortasPorSubcategoria(tipo, False), tipo, True)
        Catch ex As Exception
            MsgBox("Error al cargar la torta: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub LimpiarPanelCentro()
        panelCentral.Controls.Clear()
        dgvInformeProductos.Columns.Clear()
        dgvInformeProductos.Rows.Clear()
    End Sub

    Public Property CodigoSeleccionado As String = ""

    Private Sub dgvInformeProductos_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvInformeProductos.CellDoubleClick
        If e.RowIndex < 0 Then Exit Sub

        CodigoSeleccionado = dgvInformeProductos.Rows(e.RowIndex).Cells("codigo").Value.ToString()

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub txtCodigoProducto_KeyDown(sender As Object, e As KeyEventArgs) Handles txtCodigoProducto.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            BuscarProductoCodigo(txtCodigoProducto.Text.Trim())
        End If
    End Sub

    Private Sub BuscarProductoCodigo(codigo As String)
        If codigo = "" Then Return

        Try
            AbrirConexion()

            Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                cmd.Parameters.AddWithValue("?", codigo)

                Using reader As OleDbDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        dgvInformeProductos.Columns.Clear()
                        dgvInformeProductos.Rows.Clear()

                        dgvInformeProductos.Columns.Add("codigo", "Codigo")
                        dgvInformeProductos.Columns.Add("nombre", "Nombre")
                        dgvInformeProductos.Columns.Add("precio", "Precio")

                        dgvInformeProductos.Rows.Add(
                            codigo,
                            reader("nombre").ToString(),
                            String.Format("$ {0:N0}", CatalogoProductos.PrecioEntero(reader("precio_venta_boleta")))
                        )

                        dgvInformeProductos.Visible = True
                        dgvInformeProductos.BringToFront()
                    Else
                        MessageBox.Show("No existe el producto con el codigo ingresado.")
                    End If
                End Using
            End Using

        Catch ex As Exception
            MsgBox("Error al buscar el producto: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try
    End Sub

    Private Sub btnTodosLosProductos_Click(sender As Object, e As EventArgs) Handles btnTodosLosProductos.Click
        MostrarTodosLosProductos = True
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

End Class
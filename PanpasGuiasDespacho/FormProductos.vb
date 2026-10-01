Imports System.IO
Imports System.Drawing
Imports System.Runtime.Remoting.Channels
Imports System.Windows.Forms.VisualStyles
Imports System.Data.OleDb
Imports System.Linq.Expressions

Public Class FormProductos
    Private Sub FormProductos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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
            ("Pan de Pascua", "panpascua.png")
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
                AddHandler boton.Click, AddressOf btnTortas_Click
            Case "Galletas"
                MostrarCodigoProducto({"1304", "1305", "1310", "1311", "1313"})
            Case "Empanadas"
                MostrarCodigoProducto({"1410", "1411", "1414", "9012"})
            Case "Masas Dulces"
                MostrarCodigoProducto({"1502", "1503", "1504", "1505"})
            Case "Kuchen"
                MostrarCodigoProducto({"1200", "1201", "1202", "1203", "1206", "1207", "8014", "8015"})
            Case "Cocadas"
                MostrarCodigoProducto({"1309"})
            Case "Alfajores"
                MostrarCodigoProducto({"1300", "1302", "1303", "1319"})
            Case "Brazos de Reina"
                MostrarCodigoProducto({"1211"})
            Case "Queques y Muffins"
                MostrarCodigoProducto({"1307", "1316", "9013"})
            Case "Pasteles"
                MostrarCodigoProducto({"8000", "8006", "8001", "8007", "8017", "8009", "8003", "8010", "8004", "8011", "8005", "8013"})
            Case "Pan de Completo"
                MostrarCodigoProducto({"1402"})
            Case "Materia Prima"
                MostrarCodigoProducto({"9000", "10007"})
            Case "Mote con Huesillo"
                MostrarCodigoProducto({"1514"})
            Case "Pan de Pascua"
                MostrarCodigoProducto({"1513", "1509"})
            Case Else
                MessageBox.Show("Aun no ha sido definido productos para:  " & nombreProducto,
                                "!", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Select

        'Traer panel al frente y hacerlo visible
        dgvProductos.Visible = True
        dgvProductos.BringToFront()
    End Sub

    '==================================================================
    'BOTONES - PRODUCTOS DEL PANEL IZQUIERDO
    '==================================================================

    Private Sub btnTortas_Click(sender As Object, e As EventArgs)
        'Limpieza de los paneles y datagrid para los datos de tortas
        panelCentral.Controls.Clear()
        dgvProductos.Rows.Clear()
        dgvProductos.Columns.Clear()

        'Creacion de los botones por tipo de torta
        ' El catalogo de tipos de torta ahora vive centralizado en CatalogoProductos.vb
        Dim tipoTortas = CatalogoProductos.tipoTortas

        'Creacion de los botones de tipoTorta
        Dim posY As Integer = 10
        For Each tipo In tipoTortas
            Dim botonTipo As New Button()
            botonTipo.Text = tipo.Texto
            botonTipo.Tag = tipo.Codigo
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

    'Mostrar las Tortas por tipo 
    Private Sub MostrarTortasTipo(sender As Object, e As EventArgs)
        Dim botonTipo As Button = CType(sender, Button)
        Dim tortaCodigo As String() = CType(botonTipo.Tag, String())

        dgvProductos.Columns.Clear()
        dgvProductos.Rows.Clear()

        'Vista del DataGridView
        dgvProductos.Columns.Add("codigo", "Codigo")
        dgvProductos.Columns.Add("nombre", "Nombre")
        dgvProductos.Columns.Add("precio", "Precio")

        For Each col As DataGridViewColumn In dgvProductos.Columns
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Next

        'Conexion con Consulta SQL para la obtencion de las tortas
        Try
            AbrirConexion()

            If tortaCodigo.Length > 0 Then
                ' Una sola consulta con IN (...) en vez de una consulta por cada codigo
                Dim placeholders As New System.Text.StringBuilder()
                For i As Integer = 0 To tortaCodigo.Length - 1
                    If i > 0 Then placeholders.Append(",")
                    placeholders.Append("?")
                Next

                Dim sql As String = "SELECT codigo, nombre, precio_venta_boleta FROM producto WHERE codigo IN (" & placeholders.ToString() & ") ORDER BY codigo"

                Using cmd As New OleDbCommand(sql, conexionDB)
                    For Each codigo As String In tortaCodigo
                        cmd.Parameters.AddWithValue("?", codigo)
                    Next

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim codigoLeido As String = reader("codigo").ToString()
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As Integer = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvProductos.Rows.Add(codigoLeido, nombre, String.Format("$ {0:N0}", precio))
                        End While
                    End Using
                End Using
            End If
        Catch ex As Exception
            MsgBox("Error al cargar la torta: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try

        'Traer panel al frente  y hacerlo visible
        dgvProductos.Visible = True
        dgvProductos.BringToFront()
    End Sub

    'Limpieza del panel central ante cambio de producto
    Private Sub LimpiarPanelCentro()
        panelCentral.Controls.Clear()
        dgvProductos.Columns.Clear()
        dgvProductos.Rows.Clear()
    End Sub

    'Carga de los productos por codigo
    Private Sub MostrarCodigoProducto(codigos() As String)
        'Para el caso de tortas, se limpia el panel central de los botones
        LimpiarPanelCentro()
        'Limpieza de las columnas y filas del datagrid
        dgvProductos.Columns.Clear()
        dgvProductos.Rows.Clear()

        'Creacion de las columnas del grid
        If dgvProductos.Columns.Count = 0 Then
            dgvProductos.Columns.Add("codigo", "Codigo")
            dgvProductos.Columns.Add("nombre", "Nombre")
            dgvProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            Next
        End If

        Try
            AbrirConexion()

            If codigos.Length > 0 Then
                ' Una sola consulta con IN (...) en vez de una consulta por cada codigo
                Dim placeholders As New System.Text.StringBuilder()
                For i As Integer = 0 To codigos.Length - 1
                    If i > 0 Then placeholders.Append(",")
                    placeholders.Append("?")
                Next

                Dim sql As String = "SELECT codigo, nombre, precio_venta_boleta FROM producto WHERE codigo IN (" & placeholders.ToString() & ") ORDER BY codigo"

                Using cmd As New OleDbCommand(sql, conexionDB)
                    For Each codigo As String In codigos
                        cmd.Parameters.AddWithValue("?", codigo)
                    Next

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim codigoLeido As String = reader("codigo").ToString()
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As Integer = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvProductos.Rows.Add(codigoLeido, nombre, String.Format("$ {0:N0}", precio))
                        End While
                    End Using
                End Using
            End If

        Catch ex As Exception
            MsgBox("Error al cargar productos: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try

        dgvProductos.Visible = True
        dgvProductos.BringToFront()
    End Sub


    'Doble click en las celdas de los productos para ser agregados al grid
    Private Sub dgvProductos_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProductos.CellDoubleClick
        If e.RowIndex < 0 Then Exit Sub

        Try
            'Asegurar de que la fila elegida haya de sido valida.
            If e.RowIndex < 0 Then Exit Sub

            'Tomar los datos de la fila seleccionada 
            Dim codigo As String = dgvProductos.Rows(e.RowIndex).Cells("codigo").Value.ToString()
            Dim nombre As String = dgvProductos.Rows(e.RowIndex).Cells("nombre").Value.ToString()

            'Limpiar el precio de venta boleta y convertir el mismo a entero
            Dim precioTexto As String = dgvProductos.Rows(e.RowIndex).Cells("precio").Value.ToString()
            precioTexto = precioTexto.Replace("$ ", "").Replace(" ", "").Replace(".", "").Trim()

            Dim precio As Integer
            If Not Integer.TryParse(precioTexto, precio) Then
                MsgBox("El precio no es valido: " & precioTexto, MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            'Asegurar de que el formulario haya sido abierto en el Form2
            If TypeOf Owner Is Form1 Then
                Dim formGuias As Form1 = CType(Owner, Form1)

                ' Agregar producto al DataGridView principal
                Dim filaIndex As Integer = formGuias.dgvGuiaProductos.Rows.Add()
                Dim nuevaFila = formGuias.dgvGuiaProductos.Rows(filaIndex)

                nuevaFila.Cells("codigo").Value = codigo
                nuevaFila.Cells("nombre").Value = nombre
                nuevaFila.Cells("cantidad").Value = 1
                nuevaFila.Cells("precio").Value = String.Format("$ {0:N0}", precio)
                nuevaFila.Cells("subtotal").Value = String.Format("$ {0:N0}", precio)

                ' Recalcular y guardar automáticamente
                formGuias.CalcularTotal()
                formGuias.guardarGuia()
            Else
                MsgBox("Este formulario solo se puede usar desde el módulo de Guías.", MsgBoxStyle.Information)
            End If

            ' Cerrar el formulario después de agregar el producto
            Me.Close()

        Catch ex As Exception
            MsgBox("Error al agregar el producto: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

End Class
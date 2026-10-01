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
            ("Pan de Pascua", "panpascua.png")
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
        dgvInformeProductos.Visible = True
        dgvInformeProductos.BringToFront()
    End Sub

    Private Sub MostrarCodigoProducto(codigos() As String)
        'Para el caso de tortas, se limpia el panel central de los botones
        LimpiarPanelCentro()
        'Limpieza de las columnas y filas del datagrid
        dgvInformeProductos.Columns.Clear()
        dgvInformeProductos.Rows.Clear()

        'Creacion de las columnas del grid
        If dgvInformeProductos.Columns.Count = 0 Then
            dgvInformeProductos.Columns.Add("codigo", "Codigo")
            dgvInformeProductos.Columns.Add("nombre", "Nombre")
            dgvInformeProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvInformeProductos.Columns
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
                            dgvInformeProductos.Rows.Add(codigoLeido, nombre, String.Format("$ {0:N0}", precio))
                        End While
                    End Using
                End Using
            End If

        Catch ex As Exception
            MsgBox("Error al cargar productos: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try

        dgvInformeProductos.Visible = True
        dgvInformeProductos.BringToFront()
    End Sub

    Private Sub btnTortas_Click(sender As Object, e As EventArgs)
        'Limpieza de los paneles y datagrid para los datos de tortas
        panelCentral.Controls.Clear()
        dgvInformeProductos.Rows.Clear()
        dgvInformeProductos.Columns.Clear()

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

    Private Sub MostrarTortasTipo(sender As Object, e As EventArgs)
        Dim botonTipo As Button = CType(sender, Button)
        Dim tortaCodigo As String() = CType(botonTipo.Tag, String())

        dgvInformeProductos.Columns.Clear()
        dgvInformeProductos.Rows.Clear()

        'Vista del DataGridView
        dgvInformeProductos.Columns.Add("codigo", "Codigo")
        dgvInformeProductos.Columns.Add("nombre", "Nombre")
        dgvInformeProductos.Columns.Add("precio", "Precio")

        For Each col As DataGridViewColumn In dgvInformeProductos.Columns
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
                            dgvInformeProductos.Rows.Add(codigoLeido, nombre, String.Format("$ {0:N0}", precio))
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
        dgvInformeProductos.Visible = True
        dgvInformeProductos.BringToFront()
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
                            String.Format("$ {0:N0}", Convert.ToInt32(reader("precio_venta_boleta")))
                        )

                        dgvInformeProductos.Visible = True
                        dgvInformeProductos.BringToFront()
                    Else
                        MessageBox.Show("No existe el producto con el codigo ingresado.")
                    End If
                End Using
            End Using

        Catch ex As Exception
            MsgBox("Error al buscar el producto:", ex.Message)
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
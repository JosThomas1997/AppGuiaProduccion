Imports System.Windows.Forms
Imports System.Data.OleDb
Public Class FormProductos

    Private panelIzquierdo As New FlowLayoutPanel()
    Private dgvListarProductos As New DataGridView()
    'El panel central es exclusivo para las tortas
    Private panelCentro As New Panel()
    Private conexionDB As OleDbConnection = Conexion.conexionDB
    'Para el centrado de los botones de tortas
    Private layoutTortas As New TableLayoutPanel()
    'Layout para la reforma del centro
    Private layoutPrincipal As New TableLayoutPanel()

    Public Sub FormProductos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Size = New Size(1000, 600)
        Me.Text = "Seleccionar Producto"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False

        'Panel Principal para el layout
        layoutPrincipal.Dock = DockStyle.Fill
        layoutPrincipal.ColumnCount = 3
        layoutPrincipal.RowCount = 1
        layoutPrincipal.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 160))
        layoutPrincipal.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 220))
        layoutPrincipal.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 100))
        Me.Controls.Add(layoutPrincipal)

        'Panel Izquierdo
        panelIzquierdo.Dock = DockStyle.Left
        panelIzquierdo.Width = 150
        panelIzquierdo.FlowDirection = FlowDirection.TopDown
        panelIzquierdo.WrapContents = False
        panelIzquierdo.AutoScroll = True
        panelIzquierdo.BackColor = Color.LightGray
        Me.Controls.Add(panelIzquierdo)

        'Panel Centro
        panelCentro.Dock = DockStyle.Fill
        panelCentro.BackColor = Color.WhiteSmoke
        panelCentro.Visible = False
        layoutPrincipal.Controls.Add(panelCentro, 1, 0)

        'Layout para las Tortas
        layoutTortas.Dock = DockStyle.Fill
        layoutTortas.ColumnCount = 1
        layoutTortas.RowCount = 0
        layoutTortas.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        layoutTortas.AutoScroll = True
        layoutTortas.BackColor = Color.WhiteSmoke

        'Panel Derecho(Datagrid para agregar productos)
        dgvListarProductos.Dock = DockStyle.Fill
        dgvListarProductos.ReadOnly = True
        dgvListarProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvListarProductos.MultiSelect = False
        dgvListarProductos.AllowUserToAddRows = False
        AddHandler dgvListarProductos.CellDoubleClick, AddressOf SeleccionarProducto
        layoutPrincipal.Controls.Add(dgvListarProductos, 2, 0)

        panelCentro.Controls.Add(layoutTortas)

        'Icono para Tortas
        Dim picTortas As New PictureBox With {
                .Image = Image.FromFile(Application.StartupPath & "\Images\tortas.png"),
                .SizeMode = PictureBoxSizeMode.Zoom,
                .Size = New Size(120, 120),
                .Cursor = Cursors.Hand,
                .Margin = New Padding(10)
            }
        AddHandler picTortas.Click, AddressOf MostrarTortas
        panelIzquierdo.Controls.Add(picTortas)

        'Icono de Empanadas
        Dim picEmpanadas As New PictureBox With {
                .Image = Image.FromFile(Application.StartupPath & "\Images\empanadas.png"),
                .SizeMode = PictureBoxSizeMode.Zoom,
                .Size = New Size(120, 120),
                .Cursor = Cursors.Hand,
                .Margin = New Padding(10)
            }
        AddHandler picEmpanadas.Click, AddressOf MostrarEmpanadas
        panelIzquierdo.Controls.Add(picEmpanadas)

        'Icono de Masas Dulces
        Dim picMasasDulces As New PictureBox With {
                .Image = Image.FromFile(Application.StartupPath & "\Images\masasdulces.png"),
                .SizeMode = PictureBoxSizeMode.Zoom,
                .Size = New Size(120, 120),
                .Cursor = Cursors.Hand,
                .Margin = New Padding(10)
            }
        AddHandler picMasasDulces.Click, AddressOf MostrarMasasDulces
        panelIzquierdo.Controls.Add(picMasasDulces)

        'Icono de Kuchen
        Dim picKuchen As New PictureBox With {
                .Image = Image.FromFile(Application.StartupPath & "\Images\kuchen.png"),
                .SizeMode = PictureBoxSizeMode.Zoom,
                .Size = New Size(120, 120),
                .Cursor = Cursors.Hand,
                .Margin = New Padding(10)
            }
        AddHandler picKuchen.Click, AddressOf MostrarKuchen
        panelIzquierdo.Controls.Add(picKuchen)

        'Icono de Galletas
        Dim picGalletas As New PictureBox With {
                .Image = Image.FromFile(Application.StartupPath & "\Images\galletas.png"),
                .SizeMode = PictureBoxSizeMode.Zoom,
                .Size = New Size(120, 120),
                .Cursor = Cursors.Hand,
                .Margin = New Padding(10)
            }
        AddHandler picGalletas.Click, AddressOf MostrarGalletas
        panelIzquierdo.Controls.Add(picGalletas)

        'Icono de Cocadas
        Dim picCocadas As New PictureBox With {
                .Image = Image.FromFile(Application.StartupPath & "\Images\cocadas.png"),
                .SizeMode = PictureBoxSizeMode.Zoom,
                .Size = New Size(120, 120),
                .Cursor = Cursors.Hand,
                .Margin = New Padding(10)
            }
        AddHandler picCocadas.Click, AddressOf MostrarCocadas
        panelIzquierdo.Controls.Add(picCocadas)

        'Icono de Alfajores
        Dim picAlfajores As New PictureBox With {
                .Image = Image.FromFile(Application.StartupPath & "\Images\alfajores.png"),
                .SizeMode = PictureBoxSizeMode.Zoom,
                .Size = New Size(120, 120),
                .Cursor = Cursors.Hand,
                .Margin = New Padding(10)
            }
        AddHandler picAlfajores.Click, AddressOf MostrarAlfajores
        panelIzquierdo.Controls.Add(picAlfajores)

        'Icono de Brazo de Reina
        Dim picBrazoReina As New PictureBox With {
                .Image = Image.FromFile(Application.StartupPath & "\Images\brazosdereina.png"),
                .SizeMode = PictureBoxSizeMode.Zoom,
                .Size = New Size(120, 120),
                .Cursor = Cursors.Hand,
                .Margin = New Padding(10)
            }
        AddHandler picBrazoReina.Click, AddressOf MostrarBrazoReina
        panelIzquierdo.Controls.Add(picBrazoReina)

        'Icono de Queques y Muffin
        Dim picQuequeMuffin As New PictureBox With {
                .Image = Image.FromFile(Application.StartupPath & "\Images\quequesymuffins.png"),
                .SizeMode = PictureBoxSizeMode.Zoom,
                .Size = New Size(120, 120),
                .Cursor = Cursors.Hand,
                .Margin = New Padding(10)
            }
        AddHandler picQuequeMuffin.Click, AddressOf MostrarQuequeMuffin
        panelIzquierdo.Controls.Add(picQuequeMuffin)

        'Icono de Pasteles
        Dim picPasteles As New PictureBox With {
                .Image = Image.FromFile(Application.StartupPath & "\Images\pasteles.png"),
                .SizeMode = PictureBoxSizeMode.Zoom,
                .Size = New Size(120, 120),
                .Cursor = Cursors.Hand,
                .Margin = New Padding(10)
            }
        AddHandler picPasteles.Click, AddressOf MostrarPasteles
        panelIzquierdo.Controls.Add(picPasteles)

    End Sub

    'LIMPIAR ELEMENTOS DEL CENTRO
    Private Sub LimpiarPanelCentro()
        layoutTortas.Controls.Clear()
        layoutTortas.RowStyles.Clear()
        layoutTortas.RowCount = 0
    End Sub

    'Mostrar Tortas
    Private Sub MostrarTortas(sender As Object, e As EventArgs)
        LimpiarPanelCentro()
        panelCentro.Visible = True
        panelCentro.BringToFront()
        dgvListarProductos.Visible = True

        'BOTON PARA TORTAS HOLANDESAS
        layoutTortas.Controls.Clear()
        layoutTortas.RowStyles.Clear()
        layoutTortas.RowCount = 0
        panelCentro.BringToFront()

        Dim btnHolandesas As New Button With {
                .Text = "Holandesas",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnHolandesas.Click, AddressOf MostrarTortasHolandesas

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnHolandesas, 0, layoutTortas.RowCount - 1)

        'BOTON PARA TORTAS DE YOGHURT
        Dim btnYoghurt As New Button With {
                .Text = "Yoghurt",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnYoghurt.Click, AddressOf MostrarTortasYoghurt

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnYoghurt, 0, layoutTortas.RowCount - 1)

        'BOTON PARA TORTAS DE DULCE DE LECHE
        Dim btnDulceDeLeche As New Button With {
                .Text = "Dulce de Leche",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnDulceDeLeche.Click, AddressOf MostrarTortasDulceDeLeche

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnDulceDeLeche, 0, layoutTortas.RowCount - 1)

        'BOTON PARA TORTAS DE CHOCOLATE
        Dim btnChocolate As New Button With {
                .Text = "Chocolate",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnChocolate.Click, AddressOf MostrarTortasChocolate

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnChocolate, 0, layoutTortas.RowCount - 1)

        'BOTON PARA TORTAS DE PIÑA
        Dim btnPinia As New Button With {
                .Text = "Piña",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnPinia.Click, AddressOf MostrarTortasPinia

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnPinia, 0, layoutTortas.RowCount - 1)

        'BOTON PARA TORTAS DE SELVA NEGRA
        Dim btnSelvaNegra As New Button With {
                .Text = "Selva Negra",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnSelvaNegra.Click, AddressOf MostrarTortasSelvaNegra

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnSelvaNegra, 0, layoutTortas.RowCount - 1)

        'BOTON PARA TORTAS DE HOJA MANJAR
        Dim btnHojaManjar As New Button With {
                .Text = "Hoja Manjar",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnHojaManjar.Click, AddressOf MostrarTortasHojaManjar

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnHojaManjar, 0, layoutTortas.RowCount - 1)

        'BOTON PARA TORTAS DE LUCUMA
        Dim btnLucuma As New Button With {
                .Text = "Lucuma",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnLucuma.Click, AddressOf MostrarTortasLucuma

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnLucuma, 0, layoutTortas.RowCount - 1)

        'BOTON PARA TORTAS DE TRES LECHES
        Dim btnTresLeches As New Button With {
                .Text = "Tres Leches",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnTresLeches.Click, AddressOf MostrarTortasTresLeches

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnTresLeches, 0, layoutTortas.RowCount - 1)

        'BOTON PARA TORTAS DE MOCKA
        Dim btnMocka As New Button With {
                .Text = "Mocka",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnMocka.Click, AddressOf MostrarTortasMocka

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnMocka, 0, layoutTortas.RowCount - 1)

        'BOTON PARA TORTAS DE PANQUEQUE
        Dim btnPanqueque As New Button With {
                .Text = "Panqueque",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnPanqueque.Click, AddressOf MostrarTortasPanqueque

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnPanqueque, 0, layoutTortas.RowCount - 1)

        'BOTON PARA TORTAS DE DURAZNO
        Dim btnDurazno As New Button With {
                .Text = "Durazno",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnDurazno.Click, AddressOf MostrarTortasDurazno

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnDurazno, 0, layoutTortas.RowCount - 1)

        'BOTON PARA TORTAS DE BANDAS
        Dim btnBandas As New Button With {
                .Text = "Bandas",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnBandas.Click, AddressOf MostrarTortasBandas

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnBandas, 0, layoutTortas.RowCount - 1)

        'BOTON PARA TORTAS DE BIZCOCHO MANJAR
        Dim btnBizManjar As New Button With {
                .Text = "Biz. Manjar",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnBizManjar.Click, AddressOf MostrarTortasBizManjar

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnBizManjar, 0, layoutTortas.RowCount - 1)

        'BOTON PARA TORTAS DE PANPAS
        Dim btnPanpas As New Button With {
                .Text = "Panpas",
                .Width = 200,
                .Height = 40,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .BackColor = Color.LightPink,
                .Margin = New Padding(10)
        }
        AddHandler btnPanpas.Click, AddressOf MostrarTortasPanpas

        layoutTortas.RowCount += 1
        layoutTortas.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutTortas.Controls.Add(btnPanpas, 0, layoutTortas.RowCount - 1)
    End Sub

    Private Sub anchoDataGrid()
        dgvListarProductos.Columns("codigo").FillWeight = 20
        dgvListarProductos.Columns("nombre").FillWeight = 60
        dgvListarProductos.Columns("precio").FillWeight = 20
    End Sub

    'MOSTRAR TORTAS DE PANPAS
    Private Sub MostrarTortasPanpas(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta de Bandas)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta de Bizcocho Manjar)
        Dim codigoPanpas As String() = {"1096"}

        Try
            conexionDB.Open()
            For Each codigo As String In codigoPanpas
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas De Panpas: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub

    'MOSTRAR TORTAS DE BIZCOCHO MANJAR
    Private Sub MostrarTortasBizManjar(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta de Bandas)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta de Bizcocho Manjar)
        Dim codigoBizManjar As String() = {"1025", "1035", "1045", "1055", "1065", "1075", "1085"}

        Try
            conexionDB.Open()
            For Each codigo As String In codigoBizManjar
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas De Bizcocho Manjar: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub

    'MOSTRAR TORTAS DE BANDAS
    Private Sub MostrarTortasBandas(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta de Bandas)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta de Bandas)
        Dim codigoBandas As String() = {"1094", "1095"}

        Try
            conexionDB.Open()
            For Each codigo As String In codigoBandas
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas De Bandas: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub

    'MOSTRAR TORTAS DE DURAZNO
    Private Sub MostrarTortasDurazno(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta de Durazno)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta de Durazno)
        Dim codigoDurazno As String() = {"1026", "1036", "1046", "1056", "1066", "1076", "1086"}

        Try
            conexionDB.Open()
            For Each codigo As String In codigoDurazno
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas De Durazno: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub

    'MOSTRAR TORTAS DE PANQUEQUE
    Private Sub MostrarTortasPanqueque(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta de Panqueque)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta de Panqueque)
        Dim codigoPanqueque As String() = {"1114", "1116"}

        Try
            conexionDB.Open()
            For Each codigo As String In codigoPanqueque
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas De Panqueque: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub

    'MOSTRAR TORTAS DE MOCKA
    Private Sub MostrarTortasMocka(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta de Mocka)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta de Mocka)
        Dim codigoMocka As String() = {"1024", "1034", "1044", "1054", "1064", "1074", "1084"}

        Try
            conexionDB.Open()
            For Each codigo As String In codigoMocka
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas De Mocka: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub

    'MOSTRAR TORTAS DE TRES LECHES
    Private Sub MostrarTortasTresLeches(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta de Tres Leches)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta de Tres Leches)
        Dim codigoTresLeches As String() = {"1110", "1111", "1112"}

        Try
            conexionDB.Open()
            For Each codigo As String In codigoTresLeches
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas De Tres Leches: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub

    'MOSTRAR TORTAS DE LUCUMA
    Private Sub MostrarTortasLucuma(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta de Lucuma)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta de Lucuma)
        Dim codigoLucuma As String() = {"1023", "1033", "1043", "1053", "1063", "1073", "1083"}

        Try
            conexionDB.Open()
            For Each codigo As String In codigoLucuma
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas Lucuma: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub

    'MOSTRAR TORTAS DE HOJA MANJAR
    Private Sub MostrarTortasHojaManjar(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta de Hoja Manjar)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta de Selva Negra)
        Dim codigoHojaManjar As String() = {"1100", "1101", "1102", "1103"}

        Try
            conexionDB.Open()
            For Each codigo As String In codigoHojaManjar
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas Hoja Manjar: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub

    'MOSTRAR TORTAS DE SELVA NEGRA
    Private Sub MostrarTortasSelvaNegra(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta de Selva Negra)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta de Selva Negra)
        Dim codigoSelvaNegra As String() = {"1022", "1032", "1042", "1052", "1062", "1072", "1082"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigoSelvaNegra
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas de Selva Negra: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub

    'MOSTRAR TORTAS DE PIÑA
    Private Sub MostrarTortasPinia(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta de Piña)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta de Piña)
        Dim codigoPinia As String() = {"1021", "1031", "1041", "1051", "1061", "1071", "1081"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigoPinia
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas de Piña: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub

    'MOSTRAR TORTAS DE CHOCOLATE
    Private Sub MostrarTortasChocolate(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta Chocolate)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta Chocolate)
        Dim codigoChocolate As String() = {"1027", "1037", "1047", "1057", "1067", "1077", "1087"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigoChocolate
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas de Chocolate: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub

    'MOSTRAR TORTAS DE DULCE DE LECHE
    Private Sub MostrarTortasDulceDeLeche(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta Dulce de Leche)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta Dulce de Leche)
        Dim codigoDulceDeLeche As String() = {"1028", "1038", "1048", "1058", "1068", "1078", "1088"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigoDulceDeLeche
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas de Yoghurt: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub

    'MOSTRAR TORTAS DE YOGHURT
    Private Sub MostrarTortasYoghurt(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta Yoghurt)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta Yoghurt)
        Dim codigoYoghurt As String() = {"1009"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigoYoghurt
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas de Yoghurt: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub

    'MOSTRAR TORTAS HOLANDESAS
    Private Sub MostrarTortasHolandesas(sender As Object, e As EventArgs)
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Columnas (Torta Holandesas)
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvListarProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                'AJUSTE DE ANCHO DE LAS COLUMNAS DE TORTAS
                anchoDataGrid()
            Next
        End If

        'Codigo (Torta Holandesas)
        Dim codigoHolandesas As String() = {"1010", "1011", "1012", "1013"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigoHolandesas
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As String = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar las Tortas: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try

        'Asegurar la visibilidad de tortas
        panelCentro.Visible = True
        dgvListarProductos.Visible = True
        dgvListarProductos.BringToFront()
    End Sub


    'Mostrar Pasteles
    Private Sub MostrarPasteles(sender As Object, e As EventArgs)
        LimpiarPanelCentro()
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Crear columnas 
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            'Tomar provecho del ancho del Grid por completo
            dgvListarProductos.Columns("codigo").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("nombre").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("precio").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            anchoDataGrid()
        End If

        'Codigo de Pasteles
        Dim codigoPasteles As String() = {"8000", "8006", "8001", "8007", "8017", "8009", "8003", "8010", "8004", "8011", "8005", "8013"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigoPasteles
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then

                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As Integer = Convert.ToInt32(reader("precio_venta_boleta"))

                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next

        Catch ex As Exception
            MsgBox("Error al cargar los Pasteles: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    'Mostrar Queques y Muffins
    Private Sub MostrarQuequeMuffin(sender As Object, e As EventArgs)
        LimpiarPanelCentro()
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Crear columnas 
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            'Tomar provecho del ancho del Grid por completo
            dgvListarProductos.Columns("codigo").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("nombre").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("precio").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            anchoDataGrid()
        End If

        'Codigo de Queques y Muffins
        Dim codigoQuequeMuffin As String() = {"1307", "1316"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigoQuequeMuffin
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then

                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As Integer = Convert.ToInt32(reader("precio_venta_boleta"))

                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next

        Catch ex As Exception
            MsgBox("Error al cargar los brazos de reina: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    'Mostrar Brazo de Reina
    Private Sub MostrarBrazoReina(sender As Object, e As EventArgs)
        LimpiarPanelCentro()
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Crear columnas 
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            'Tomar provecho del ancho del Grid por completo
            dgvListarProductos.Columns("codigo").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("nombre").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("precio").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            anchoDataGrid()
        End If

        'Codigo de Brazo de Reina
        Dim codigoBrazoReina As String() = {"1211"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigoBrazoReina
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then

                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As Integer = Convert.ToInt32(reader("precio_venta_boleta"))

                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next

        Catch ex As Exception
            MsgBox("Error al cargar los brazos de reina: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    'Mostrar Alfajores
    Private Sub MostrarAlfajores(sender As Object, e As EventArgs)
        LimpiarPanelCentro()
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Crear columnas 
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            'Tomar provecho del ancho del Grid por completo
            dgvListarProductos.Columns("codigo").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("nombre").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("precio").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            anchoDataGrid()
        End If

        'Codigo de Alfajores
        Dim codigoAlfajores As String() = {"1300", "1302", "1303", "1319"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigoAlfajores
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then

                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As Integer = Convert.ToInt32(reader("precio_venta_boleta"))

                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next

        Catch ex As Exception
            MsgBox("Error al cargar los Alfajores: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    'Mostrar Cocadas
    Private Sub MostrarCocadas(sender As Object, e As EventArgs)
        LimpiarPanelCentro()
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Crear columnas 
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            'Tomar provecho del ancho del Grid por completo
            dgvListarProductos.Columns("codigo").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("nombre").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("precio").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            anchoDataGrid()
        End If

        'Codigo de Cocadas
        Dim codigoCocadas As String() = {"1309"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigoCocadas
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then

                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As Integer = Convert.ToInt32(reader("precio_venta_boleta"))

                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next

        Catch ex As Exception
            MsgBox("Error al cargar las Cocadas: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    'Mostrar Galletas
    Private Sub MostrarGalletas(sender As Object, e As EventArgs)
        LimpiarPanelCentro()
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Crear columnas 
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            'Tomar provecho del ancho del Grid por completo
            dgvListarProductos.Columns("codigo").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("nombre").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("precio").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            anchoDataGrid()
        End If

        'Codigo de Galletas
        Dim codigoGalletas As String() = {"1304", "1305", "1310", "1311", "1313"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigoGalletas
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then

                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As Integer = Convert.ToInt32(reader("precio_venta_boleta"))

                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next

        Catch ex As Exception
            MsgBox("Error al cargar las Galletas: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    'Mostrar Kuchen
    Private Sub MostrarKuchen(sender As Object, e As EventArgs)
        LimpiarPanelCentro()
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Crear columnas 
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            'Tomar provecho del ancho del Grid por completo
            dgvListarProductos.Columns("codigo").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("nombre").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("precio").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            anchoDataGrid()
        End If

        'Codigo de Kuchen
        Dim codigoKuchen As String() = {"1200", "1201", "1202", "1203", "1206", "1207", "8014", "8015"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigoKuchen
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then

                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As Integer = Convert.ToInt32(reader("precio_venta_boleta"))

                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next

        Catch ex As Exception
            MsgBox("Error al cargar los Kuchen: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    'Mostrar Masas Dulces
    Private Sub MostrarMasasDulces(sender As Object, e As EventArgs)
        LimpiarPanelCentro()
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Crear columnas 
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            'Tomar provecho del ancho del GRid por completo
            dgvListarProductos.Columns("codigo").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("nombre").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("precio").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            anchoDataGrid()
        End If

        'Codigo de Masa Dulce
        Dim codigoMasasDulces As String() = {"1502", "1503", "1504", "1505"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigoMasasDulces
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then

                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As Integer = Convert.ToInt32(reader("precio_venta_boleta"))

                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next

        Catch ex As Exception
            MsgBox("Error al cargar las masas dulces: " & ex.Message)
        Finally
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    'Mostrar Empanadas 
    Private Sub MostrarEmpanadas(sender As Object, e As EventArgs)
        LimpiarPanelCentro()
        dgvListarProductos.Columns.Clear()
        dgvListarProductos.Rows.Clear()

        'Agregado de columnas antes del Try-Catch
        If dgvListarProductos.Columns.Count = 0 Then
            dgvListarProductos.Columns.Add("codigo", "Codigo")
            dgvListarProductos.Columns.Add("nombre", "Nombre")
            dgvListarProductos.Columns.Add("precio", "Precio")

            'Expandir las columnas al maximo de la ventana
            dgvListarProductos.Columns("codigo").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("nombre").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            dgvListarProductos.Columns("precio").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            anchoDataGrid()
        End If


        'Codigos de Empanadas
        Dim codigosEmpanadas As String() = {"1410", "1411", "1414"}

        Try
            conexionDB.Open()

            For Each codigo As String In codigosEmpanadas
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As Integer = Convert.ToInt32(reader("precio_venta_boleta"))

                            dgvListarProductos.Rows.Add(codigo, nombre, precio)
                        End If
                    End Using
                End Using
            Next

        Catch ex As Exception
            MsgBox("Error al cargar los productos: " & ex.Message)
        Finally
            'Cerrar conexion para hacer click en el otro icono de los productos
            If conexionDB.State = ConnectionState.Open Then conexionDB.Close()
        End Try
    End Sub

    Private Sub SeleccionarProducto(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 Then
            Dim fila = dgvListarProductos.Rows(e.RowIndex)
            Dim codigo = fila.Cells("codigo").Value.ToString()
            Dim nombre = fila.Cells("nombre").Value.ToString()
            Dim precio = Convert.ToInt32(fila.Cells("precio").Value)

            'Agregar al formulario de guia
            Dim dgv = Form1.dgvProductos
            Dim filaIndex = dgv.Rows.Add()
            Dim nuevaFila = dgv.Rows(filaIndex)

            nuevaFila.Cells("codigo").Value = codigo
            nuevaFila.Cells("nombre").Value = nombre
            nuevaFila.Cells("precio").Value = String.Format("{0:N0}", precio) & " $"
            nuevaFila.Cells("cantidad").Value = 1
            nuevaFila.Cells("subtotal").Value = String.Format("{0:N0}", precio) & " $"

            Form1.CalcularTotal()
            Me.Close()
        End If
    End Sub

End Class
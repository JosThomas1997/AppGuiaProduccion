Imports System.IO
Imports System.Drawing
Imports System.Runtime.Remoting.Channels
Imports System.Windows.Forms.VisualStyles
Imports System.Data.OleDb
Imports System.Linq.Expressions

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
            ("Pan de Completo", "pancompleto.png")
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
                MostrarCodigoProducto({"1410", "1411", "1414"})
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
                MostrarCodigoProducto({"1307", "1316"})
            Case "Pasteles"
                MostrarCodigoProducto({"8000", "8006", "8001", "8007", "8017", "8009", "8003", "8010", "8004", "8011", "8005", "8013"})
            Case "Pan de Completo"
                MostrarCodigoProducto({"1402"})
            Case "Materia Prima"
                MostrarCodigoProducto({"9000", "10007"})
            Case Else
                MessageBox.Show("Aun no ha sido definido productos para:  " & nombreProducto,
                                "!", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Select

        'Traer panel al frente y hacerlo visible
        dgvProducProductos.Visible = True
        dgvProducProductos.BringToFront()
    End Sub



    '==================================================================
    'BOTONES - PRODUCTOS DEL PANEL IZQUIERDO
    '==================================================================

    Private Sub btnTortas_Click(sender As Object, e As EventArgs)
        'Limpieza de los paneles y datagrid para los datos de tortas
        panelCentral.Controls.Clear()
        dgvProducProductos.Rows.Clear()
        dgvProducProductos.Columns.Clear()

        'Creacion de los botones por tipo de torta
        Dim tipoTortas As (Texto As String, Codigo As String())() = {
        ("Holandesas", {"1010", "1011", "1012", "1013"}),
        ("Yoghurt", {"1009"}),
        ("Dulce de Leche", {"1028", "1038", "1048", "1058", "1068", "1078", "1088"}),
        ("Chocolate", {"1027", "1037", "1047", "1057", "1067", "1077", "1087"}),
        ("Piña", {"1021", "1031", "1041", "1051", "1061", "1071", "1081"}),
        ("Selva Negra", {"1022", "1032", "1042", "1052", "1062", "1072", "1082"}),
        ("Hoja Manjar", {"1100", "1101", "1102", "1103"}),
        ("Lucuma", {"1023", "1033", "1043", "1053", "1063", "1073", "1083"}),
        ("Tres Leches", {"1110", "1111", "1112"}),
        ("Mocka", {"1024", "1034", "1044", "1054", "1064", "1074", "1084"}),
        ("Panqueque", {"1114", "1116"}),
        ("Durazno", {"1026", "1036", "1046", "1056", "1066", "1076", "1086"}),
        ("Bizcocho Manjar", {"1025", "1035", "1045", "1055", "1065", "1075", "1085"}),
        ("Panpas", {"1096"})
    }

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

        dgvProducProductos.Columns.Clear()
        dgvProducProductos.Rows.Clear()

        'Vista del DataGridView
        dgvProducProductos.Columns.Add("codigo", "Codigo")
        dgvProducProductos.Columns.Add("nombre", "Nombre")
        dgvProducProductos.Columns.Add("precio", "Precio")

        For Each col As DataGridViewColumn In dgvProducProductos.Columns
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Next

        'Conexion con Consulta SQL para la obtencion de las tortas
        Try
            AbrirConexion()
            For Each codigo As String In tortaCodigo
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As Integer = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvProducProductos.Rows.Add(codigo, nombre, String.Format("{0:N0}", precio) & " $")
                        End If
                    End Using
                End Using
            Next
        Catch ex As Exception
            MsgBox("Error al cargar la torta: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try

        'Traer panel al frente  y hacerlo visible
        dgvProducProductos.Visible = True
        dgvProducProductos.BringToFront()
    End Sub

    'Limpieza del panel central ante cambio de producto
    Private Sub LimpiarPanelCentro()
        panelCentral.Controls.Clear()
        dgvProducProductos.Columns.Clear()
        dgvProducProductos.Rows.Clear()
    End Sub

    'Carga de los productos por codigo
    Private Sub MostrarCodigoProducto(codigos() As String)
        'Para el caso de tortas, se limpia el panel central de los botones
        LimpiarPanelCentro()
        'Limpieza de las columnas y filas del datagrid
        dgvProducProductos.Columns.Clear()
        dgvProducProductos.Rows.Clear()

        'Creacion de las columnas del grid
        If dgvProducProductos.Columns.Count = 0 Then
            dgvProducProductos.Columns.Add("codigo", "Codigo")
            dgvProducProductos.Columns.Add("nombre", "Nombre")
            dgvProducProductos.Columns.Add("precio", "Precio")

            For Each col As DataGridViewColumn In dgvProducProductos.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            Next
        End If

        Try
            AbrirConexion()

            For Each codigo As String In codigos
                Using cmd As New OleDbCommand("SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?", conexionDB)
                    cmd.Parameters.AddWithValue("?", codigo)
                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim nombre As String = reader("nombre").ToString()
                            Dim precio As Integer = Convert.ToInt32(reader("precio_venta_boleta"))
                            dgvProducProductos.Rows.Add(codigo, nombre, String.Format("{0:N0}", precio) & " $")
                        End If
                    End Using
                End Using
            Next

        Catch ex As Exception
            MsgBox("Error al cargar productos: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            CerrarConexion()
        End Try

        dgvProducProductos.Visible = True
        dgvProducProductos.BringToFront()
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
            precioTexto = precioTexto.Replace(" $", "").Replace(".", "").Trim()

            Dim precio As Integer
            If Not Integer.TryParse(precioTexto, precio) Then
                MsgBox("El precio no es valido: " & precioTexto, MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            'Asegurar de que el formulario haya sido abierto en el Form2
            If TypeOf Owner Is Form2 Then
                Dim formProduccion As Form2 = CType(Owner, Form2)

                'Agregar los productos al grid del Form2
                formProduccion.dgvProduccion.Rows.Add(
                codigo,
                nombre,
                1,
                String.Format("{0:N0}", precio) & " $"
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
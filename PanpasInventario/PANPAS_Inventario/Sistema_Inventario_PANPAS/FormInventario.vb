Imports System.Data.OleDb

Public Class FormInventario

    Private Sub FormInventario_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler txtVentaPesos.KeyPress, AddressOf SoloDecimal
        AddHandler txtPrecioPan.KeyPress, AddressOf SoloDecimal
        AddHandler txtLlegoPan.KeyPress, AddressOf SoloDecimal
        AddHandler txtSalidaPan.KeyPress, AddressOf SoloDecimal

        AbrirConexionDB()

        BaseGridinventario()

        CargarPasteleros()
        CargarPastelerosDia()
        CargarOCrearInventario()

        CargarInventarioCabecera(InventarioActual)
        CargarInventarioDetalle(InventarioActual)

        CalcularTotalesInventario()

        CargarListaInventarios()
    End Sub

    Private Sub FormInventario_FormClosing(sender As Object, e As FormClosingEventArgs) _
    Handles MyBase.FormClosing
        CerrarConexionDB()
    End Sub

    Private Sub BaseGridinventario()

        'Limpieza del Grid y establecimiento de las columnas a utilizar
        dgvInventario.Columns.Clear()
        dgvInventario.AllowUserToAddRows = False
        dgvInventario.AllowUserToDeleteRows = False
        dgvInventario.RowHeadersVisible = False
        dgvInventario.AutoGenerateColumns = False
        dgvInventario.AllowUserToOrderColumns = False

        'Elementos para la columnas de pastelero (ComboBox)
        Dim colPastelero As New DataGridViewComboBoxColumn()
        colPastelero.Name = "id_pastelero"
        colPastelero.HeaderText = "Pastelero"
        colPastelero.FlatStyle = FlatStyle.Flat
        colPastelero.Width = 150

        'ID producto (No a la vista)
        Dim colIdProducto As New DataGridViewTextBoxColumn()
        colIdProducto.Name = "id_producto"
        colIdProducto.Visible = False
        dgvInventario.Columns.Add(colIdProducto)

        'Codigo Producto (Solo lectura)
        Dim colCodigo As New DataGridViewTextBoxColumn()
        colCodigo.Name = "codigo"
        colCodigo.HeaderText = "Codigo"
        colCodigo.ReadOnly = True
        dgvInventario.Columns.Add(colCodigo)

        'Agregado de columnas al Grid
        dgvInventario.Columns("codigo").ReadOnly = True
        dgvInventario.Columns.Add("nombre", "Producto")
        dgvInventario.Columns.Add("precio", "Precio")
        dgvInventario.Columns.Add("inventario", "Inventario Anterior")
        dgvInventario.Columns.Add("llegada", "Llegada (+)")
        dgvInventario.Columns.Add("despacho", "Despacho (-)")

        'Columnas de pastelero (ComboBox)
        dgvInventario.Columns.Add(colPastelero)

        dgvInventario.Columns.Add("bajas", "Bajas (-)")
        dgvInventario.Columns.Add("ofertas", "Ofertas (-)")
        dgvInventario.Columns.Add("caja1", "Caja 1 (-)")
        dgvInventario.Columns.Add("caja2", "Caja 2 (-)")

        dgvInventario.Columns.Add("inventario_final", "Inventario Final (-)")
        dgvInventario.Columns.Add("total", "Total")

        'Configuracion de las columnas en modo lectura
        dgvInventario.Columns("nombre").ReadOnly = True
        dgvInventario.Columns("precio").ReadOnly = True
        dgvInventario.Columns("inventario_final").ReadOnly = False 'Ahora editable
        dgvInventario.Columns("total").ReadOnly = True

        'Formato para monto de total y precio
        dgvInventario.Columns("precio").DefaultCellStyle.Format = "N0"
        dgvInventario.Columns("total").DefaultCellStyle.Format = "N0"
    End Sub

    'Tabulacion de Enter en celdas laterales e inferiores
    Private Sub dgvInventario_KeyDown(sender As Object, e As KeyEventArgs) _
    Handles dgvInventario.KeyDown

        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            MoverCeldaDerecha()
        End If

    End Sub

    'Movimiento de celda activa a la derecha o a la siguiente fila
    Private Sub MoverCeldaDerecha()

        dgvInventario.EndEdit()

        If dgvInventario.CurrentCell Is Nothing Then Exit Sub

        Dim fila As Integer = dgvInventario.CurrentCell.RowIndex
        Dim colActual As Integer = dgvInventario.CurrentCell.ColumnIndex

        ' Columnas en orden de Edicion
        Dim columnasEditables As String() =
        {"inventario", "llegada", "despacho", "id_pastelero", "bajas", "ofertas", "caja1", "caja2"}

        Dim colNombre As String =
        dgvInventario.Columns(colActual).Name

        Dim indiceActual As Integer =
        Array.IndexOf(columnasEditables, colNombre)

        ' Si la columna no está en la lista, no se ejecuta
        If indiceActual = -1 Then Exit Sub

        ' Si hay columna a la derecha, editar la misma fila
        If indiceActual < columnasEditables.Length - 1 Then

            Dim siguienteCol As String =
            columnasEditables(indiceActual + 1)

            dgvInventario.CurrentCell =
            dgvInventario.Rows(fila).Cells(siguienteCol)

        Else
            ' Una vez que la edicion termina, se pasa a la siguiente fila
            If fila < dgvInventario.Rows.Count - 1 Then
                dgvInventario.CurrentCell =
                    dgvInventario.Rows(fila + 1).Cells("codigo")
            End If
        End If

    End Sub

    'insercion de datos existentes de inventario anterior, al nuevo inventario creado, para facilitar la edicion
    'de inventarios diarios
    Private Sub CopiarDesdeGridAlNuevoInventario(idNuevo As Integer)

        Dim cn As OleDbConnection = Conexion.conexionDB
        If cn.State <> ConnectionState.Open Then cn.Open()

        For Each row As DataGridViewRow In dgvInventario.Rows

            If row.IsNewRow Then Continue For
            If row.Cells("id_producto").Value Is Nothing Then Continue For

            Dim idProducto As Integer = CInt(row.Cells("id_producto").Value)

            Dim inventario As Integer = LeerEntero(row.Cells("inventario"))
            Dim llegada As Integer = LeerEntero(row.Cells("llegada"))
            Dim despacho As Integer = LeerEntero(row.Cells("despacho"))
            Dim bajas As Integer = LeerEntero(row.Cells("bajas"))
            Dim ofertas As Integer = LeerEntero(row.Cells("ofertas"))
            Dim caja1 As Integer = LeerEntero(row.Cells("caja1"))
            Dim caja2 As Integer = LeerEntero(row.Cells("caja2"))

            Dim inventarioFinal As Integer =
            inventario + llegada - despacho - bajas - ofertas - caja1 - caja2

            Dim pasteleroCell = row.Cells("id_pastelero").Value
            Dim tienePastelero As Boolean = Not (pasteleroCell Is Nothing OrElse IsDBNull(pasteleroCell))
            Dim pasteleroId As Integer = 0
            If tienePastelero Then
                pasteleroId = Convert.ToInt32(pasteleroCell)
            End If

            Dim tieneMovimiento As Boolean =
            llegada <> 0 OrElse despacho <> 0 OrElse bajas <> 0 OrElse ofertas <> 0 OrElse caja1 <> 0 OrElse caja2 <> 0

            Dim tieneInventario As Boolean = inventario <> 0 OrElse inventarioFinal <> 0

            ' Insertar sólo si hay datos relevantes
            If Not (tieneMovimiento OrElse tieneInventario OrElse tienePastelero) Then
                Continue For
            End If

            Dim sql As String =
        "INSERT INTO inventario_detalle
        (id_inventario, id_producto,
         inventario_inicial, llegada, despacho, bajas, ofertas,
         caja1, caja2, id_pastelero)
        VALUES (?,?,?,?,?,?,?,?,?,?)"

            Using cmd As New OleDbCommand(sql, cn)
                cmd.Parameters.AddWithValue("@p1", idNuevo)
                cmd.Parameters.AddWithValue("@p2", idProducto)
                cmd.Parameters.AddWithValue("@p3", inventarioFinal)
                cmd.Parameters.AddWithValue("@p4", 0)
                cmd.Parameters.AddWithValue("@p5", 0)
                cmd.Parameters.AddWithValue("@p6", 0)
                cmd.Parameters.AddWithValue("@p7", 0)
                cmd.Parameters.AddWithValue("@p8", 0)
                cmd.Parameters.AddWithValue("@p9", 0)
                If tienePastelero Then
                    cmd.Parameters.AddWithValue("@p10", pasteleroId)
                Else
                    cmd.Parameters.AddWithValue("@p10", DBNull.Value)
                End If

                cmd.ExecuteNonQuery()
            End Using

        Next

    End Sub

    'Insercion de producto al finalizar la edicion de codigo y autocompletarse con los datos
    Private Sub dgvInventario_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) _
    Handles dgvInventario.CellEndEdit

        dgvInventario.CommitEdit(DataGridViewDataErrorContexts.Commit)

        If Not GuardarAuto Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub

        RecalcularFila(e.RowIndex)
        GuardadoPendiente = True

    End Sub

    Private Sub dgvInventario_CurrentCellDirtyStateChanged(
    sender As Object,
    e As EventArgs
) Handles dgvInventario.CurrentCellDirtyStateChanged

        If TypeOf dgvInventario.CurrentCell Is DataGridViewComboBoxCell Then
            dgvInventario.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If

    End Sub

    'Limpieza de la fila en caso de que el codigo quede vacio o se elimine el producto
    Private Sub LimpiarFila(fila As Integer)

        Dim row = dgvInventario.Rows(fila)
        If row.IsNewRow Then Exit Sub

        row.Cells("nombre").Value = Nothing
        row.Cells("precio").Value = Nothing
        row.Cells("inventario").Value = Nothing
        row.Cells("llegada").Value = Nothing
        row.Cells("despacho").Value = Nothing
        row.Cells("id_pastelero").Value = DBNull.Value
        row.Cells("bajas").Value = Nothing
        row.Cells("ofertas").Value = Nothing
        row.Cells("caja1").Value = Nothing
        row.Cells("caja2").Value = Nothing
        row.Cells("inventario_final").Value = Nothing
        row.Cells("total").Value = Nothing

        ' Reset visual
        row.DefaultCellStyle.BackColor = Color.White
        row.Cells("total").Style.ForeColor = Color.Black

    End Sub

    Private CargandoInventario As Boolean = False

    'Recalculo de la fila al cambiar algun valor numerico
    Private Sub dgvInventario_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) _
    Handles dgvInventario.CellValueChanged

        If CargandoInventario Then Exit Sub
        If Not GuardarAuto Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub

        RecalcularFila(e.RowIndex)
        GuardadoPendiente = True
    End Sub

    'Validacion de datos ingresados en celdas numericas
    Private Sub dgvInventario_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) _
    Handles dgvInventario.CellValidating

        Dim col As String = dgvInventario.Columns(e.ColumnIndex).Name

        If col = "llegada" OrElse col = "despacho" OrElse
       col = "bajas" OrElse col = "ofertas" OrElse
       col = "caja1" OrElse col = "caja2" Then

            Dim texto As String = e.FormattedValue.ToString().Trim()

            ' Permitir vacio y se considera 0
            If texto = "" Then Exit Sub

            Dim valor As Integer
            If Not Integer.TryParse(texto, valor) OrElse valor < 0 Then
                e.Cancel = True
            End If

        End If

    End Sub

    Private Sub Saquitos_TextChanged(sender As Object, e As EventArgs) _
        Handles txtProdBerlines.TextChanged,
                txtInventarioInicialSaquitos.TextChanged,
                txtIngresoSaquitos.TextChanged,
                txtSalidaSaquitos.TextChanged,
                txtInventarioFinalSaquitos.TextChanged

        RecalcularSaquitos()
    End Sub

    Private Sub CalcularPanVenta()
        If Not GuardarAuto Then Exit Sub

        Dim ventaPesos As Decimal = LeerDecimal(txtVentaPesos)
        Dim precioPan As Decimal = LeerDecimal(txtPrecioPan)
        Dim llegoPan As Decimal = LeerDecimal(txtLlegoPan)
        Dim salidaPan As Decimal = LeerDecimal(txtSalidaPan)

        Dim ventaKg As Decimal = 0D
        If precioPan > 0 Then
            ventaKg = ventaPesos / precioPan
        End If

        txtVentaPanKg.Text = ventaKg.ToString("0.00")

        Dim totalPan As Decimal =
        ventaKg -
        llegoPan +
        salidaPan

        txtTotalPan.Text = totalPan.ToString("0.00")
    End Sub

    Private Sub CalcularKilosEnVenta()
        If Not GuardarAuto Then Exit Sub

        Dim ventaPesos As Decimal = LeerDecimal(txtVentaPesos)
        Dim precioPan As Decimal = LeerDecimal(txtPrecioPan)

        If precioPan <= 0 Then
            txtKilosVenta.Text = "0"
            Exit Sub
        End If

        Dim kilosReales As Decimal = ventaPesos / precioPan
        Dim kilosEnteros As Integer = Math.Floor(kilosReales)

        txtKilosVenta.Text = kilosEnteros.ToString()
    End Sub

    Private Sub CalcularSaquitosLabel()
        If Not GuardarAuto Then Exit Sub

        Dim produccionTotal As Decimal = LeerDecimal(txtProduccionTotal)

        If produccionTotal <= 0 Then
            lblSaquitos.Text = "0"
            Exit Sub
        End If

        Dim saquitos As Decimal = produccionTotal / 2D
        lblSaquitos.Text = saquitos.ToString("N1")
    End Sub

    Private Sub ValoresClave_TextChanged(sender As Object, e As EventArgs) _
    Handles txtProduccionTotal.TextChanged,
        txtVentaPesos.TextChanged,
        txtPrecioPan.TextChanged,
        txtLlegoPan.TextChanged,
        txtSalidaPan.TextChanged

        If Not GuardarAuto Then Exit Sub

        RecalcularPan()
        CalcularRendimiento()
        CalcularSaquitosLabel()
        GuardarInventarioCabecera()

        GuardadoPendiente = True
    End Sub

    Private Sub RecalcularSaquitos()
        If Not GuardarAuto Then Exit Sub

        Dim produccionBerlines As Decimal = LeerDecimal(txtProdBerlines)
        Dim inicial As Decimal = LeerDecimal(txtInventarioInicialSaquitos)
        Dim ingreso As Decimal = LeerDecimal(txtIngresoSaquitos)
        Dim salida As Decimal = LeerDecimal(txtSalidaSaquitos)
        Dim invFinal As Decimal = LeerDecimal(txtInventarioFinalSaquitos)

        ' Formula de Berlines
        Dim saquitoBerlin As Decimal =
        (produccionBerlines * 0.119D) / 25D

        txtBerlinesSaquitos.Text = saquitoBerlin.ToString("N2")

        ' Produccion Total
        Dim produccionTotal As Decimal =
        inicial + ingreso - salida - saquitoBerlin - invFinal

        txtProduccionTotal.Text = produccionTotal.ToString("N2")

    End Sub

    'Funcion para la lectura de Decimales 
    Private Function LeerDecimal(txt As TextBox) As Decimal
        If txt Is Nothing OrElse txt.Text.Trim() = "" Then Return 0D

        Dim s As String = txt.Text.Trim()
        Dim resultado As Decimal

        ' Intentar con la cultura actual (acepta coma en es-ES)
        If Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.CurrentCulture, resultado) Then
            Return resultado
        End If

        ' Intentar con InvariantCulture después de normalizar separador decimal (soporta "1,23" y "1.23")
        Dim sInvariant As String = s.Replace(",", ".")
        If Decimal.TryParse(sInvariant, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, resultado) Then
            Return resultado
        End If

        ' Último recurso: reemplazar puntos por comas y probar con cultura actual
        Dim sAlternate As String = s.Replace(".", ",")
        If Decimal.TryParse(sAlternate, Globalization.NumberStyles.Any, Globalization.CultureInfo.CurrentCulture, resultado) Then
            Return resultado
        End If

        Return 0D
    End Function

    'Funcion para leer valores enteros de celdas, en caso de estar vacias, retorna 0
    Private Function LeerEntero(celda As DataGridViewCell) As Integer
        If celda.Value Is Nothing OrElse celda.Value.ToString().Trim() = "" Then
            Return 0
        End If
        Return Convert.ToInt32(celda.Value)
    End Function

    Private Function LeerDecimalCelda(valor As Object) As Decimal

        If valor Is Nothing OrElse IsDBNull(valor) Then
            Return 0D
        End If

        Dim resultado As Decimal

        If Decimal.TryParse(valor.ToString().Replace(",", "."),
        Globalization.NumberStyles.Any,
        Globalization.CultureInfo.InvariantCulture,
        resultado) Then

            Return resultado
        End If

        Return 0D

    End Function

    Private Sub RecalcularTotalesGenerales()
        If Not GuardarAuto Then Exit Sub

        Dim totalDespacho As Integer = 0

        For Each row As DataGridViewRow In dgvInventario.Rows
            If row.IsNewRow Then Continue For

            totalDespacho += LeerEntero(row.Cells("despacho"))
        Next
    End Sub

    'TEMPORAL: Manejo de errores en el DataGridView para evitar excepciones molestos
    Private Sub dgvInventario_DataError(
    sender As Object,
    e As DataGridViewDataErrorEventArgs
    ) Handles dgvInventario.DataError

        e.ThrowException = False
    End Sub

    'Cargar los pasteleros en el grid del combobox creado para tal fin
    Private Sub CargarPasteleros()

        Dim dt As New DataTable()
        Dim sql As String =
        "SELECT id_pastelero, nombre_pastelero 
         FROM pastelero 
         ORDER BY nombre_pastelero"

        Using cmd As New OleDbCommand(sql, Conexion.conexionDB)
            Using da As New OleDbDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        Dim comboCol =
        CType(dgvInventario.Columns("id_pastelero"), DataGridViewComboBoxColumn)

        comboCol.DataSource = dt
        comboCol.DisplayMember = "nombre_pastelero"
        comboCol.ValueMember = "id_pastelero"
        comboCol.ValueType = GetType(Integer)
        comboCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton

    End Sub

    'Codigos fijos para la lista de productos de inventario
    Private ListaCodigosFijos As String() = {
    "1300", "1302", "1500", "1211", "1303", "1309", "1505", "1304", "1410", "1411",
    "1310", "1311", "1206", "1207", "1305", "1502", "1316", "1404", "11204", "11203",
    "8014", "1200", "1201", "8015", "1202", "1203", "11205", "11206", "11207", "11208", "11209",
    "11210", "1319", "1100", "1101", "1102", "1103", "1010", "1011", "1012", "1013",
    "1114", "1110", "1111", "1009", "1504", "1503", "1416", "1205", "1402", "1517", "1518", "11100", "11110"
    }

    Private Sub CargarProductosFijosEnGrid()

        dgvInventario.Rows.Clear()

        Dim sql As String =
    "SELECT id_producto, codigo, nombre, precio_venta_boleta
     FROM producto
     WHERE codigo = ?"

        For Each codigo As String In ListaCodigosFijos

            Using cmd As New OleDbCommand(sql, Conexion.conexionDB)
                cmd.Parameters.AddWithValue("@codigo", codigo)

                Using dr = cmd.ExecuteReader()
                    If dr.Read() Then

                        Dim fila As Integer = dgvInventario.Rows.Add()

                        With dgvInventario.Rows(fila)
                            .Cells("id_producto").Value = dr("id_producto")
                            .Cells("codigo").Value = dr("codigo")
                            .Cells("nombre").Value = dr("nombre")
                            .Cells("precio").Value = dr("precio_venta_boleta")

                            ' Todos los productos poseen valor cero
                            .Cells("inventario").Value = 0
                            .Cells("llegada").Value = 0
                            .Cells("despacho").Value = 0
                            .Cells("bajas").Value = 0
                            .Cells("ofertas").Value = 0
                            .Cells("caja1").Value = 0
                            .Cells("caja2").Value = 0
                            .Cells("inventario_final").Value = 0 ' ← CAMBIO: inicializar a 0 (editable)
                            .Cells("id_pastelero").Value = DBNull.Value
                        End With

                        RecalcularFila(fila)

                    End If
                End Using
            End Using

        Next

    End Sub

    Private Const RENDIMIENTO_OBJETIVO As Decimal = 32.5D

    Private Sub CalcularRendimiento()
        ' Evitar cálculo mientras se está cargando el inventario (protege de recursividad),
        ' pero permitir el cálculo cuando GuardarAuto = False por motivos de guardado/creación.
        If CargandoInventario Then Exit Sub

        Dim produccionTotal As Decimal = LeerDecimal(txtProduccionTotal)
        Dim totalPan As Decimal = LeerDecimal(txtTotalPan)

        If totalPan <= 0 Then
            txtRendimiento.Text = "0.0"
            txtFaltante.Text = ""
            txtRendimiento.BackColor = Color.White
            Exit Sub
        End If

        ' Rendimiento real (igual a FileMaker)
        Dim rendimiento As Decimal = (produccionTotal / totalPan) * 1000D
        rendimiento = Math.Round(rendimiento, 1, MidpointRounding.AwayFromZero)

        txtRendimiento.Text = rendimiento.ToString("0.0")

        If rendimiento < RENDIMIENTO_OBJETIVO Then

            Dim faltante As Decimal = RENDIMIENTO_OBJETIVO - rendimiento
            faltante = Math.Round(faltante, 1, MidpointRounding.AwayFromZero)

            txtFaltante.Text = faltante.ToString("0.0")
            txtRendimiento.BackColor = Color.MistyRose

        Else
            txtFaltante.Text = ""
            txtRendimiento.BackColor = Color.LightGreen
        End If

    End Sub

    Private Sub Rendimiento_TextChanged(sender As Object, e As EventArgs) _
    Handles txtProduccionTotal.TextChanged,
        txtTotalPan.TextChanged

        CalcularRendimiento()

    End Sub

    'Calculo para la venta de pan, salida y entrada de pan
    Private Sub RecalcularPan()
        If Not GuardarAuto Then Exit Sub

        Dim ventaPesos As Decimal = LeerDecimal(txtVentaPesos)
        Dim precioPan As Decimal = LeerDecimal(txtPrecioPan)
        Dim llego As Integer = CInt(LeerDecimal(txtLlegoPan))
        Dim salida As Integer = CInt(LeerDecimal(txtSalidaPan))

        If precioPan <= 0 Then
            txtVentaPanKg.Text = "0"
            txtTotalPan.Text = "0"
            Exit Sub
        End If

        Dim kilosReales As Decimal = ventaPesos / precioPan
        Dim kilosEnteros As Integer = CInt(Math.Floor(kilosReales))

        txtVentaPanKg.Text = kilosReales.ToString("N2")
        txtKilosVenta.Text = kilosEnteros.ToString()

        Dim totalPan As Integer = kilosEnteros - llego + salida
        txtTotalPan.Text = totalPan.ToString()
        CalcularRendimiento()
    End Sub

    'Recalcular las filas del grid de Inventario
    Private Sub RecalcularFila(fila As Integer)

        If Not GuardarAuto Then Exit Sub

        'Obtencion de la fila a calcular
        Dim row = dgvInventario.Rows(fila)

        'Lectura de los valores necesarios para el calculo
        Dim inventario As Integer = LeerEntero(row.Cells("inventario"))
        Dim llegada As Integer = LeerEntero(row.Cells("llegada"))
        Dim despacho As Integer = LeerEntero(row.Cells("despacho"))
        Dim bajas As Integer = LeerEntero(row.Cells("bajas"))
        Dim ofertas As Integer = LeerEntero(row.Cells("ofertas"))
        Dim caja1 As Integer = LeerEntero(row.Cells("caja1"))
        Dim caja2 As Integer = LeerEntero(row.Cells("caja2"))
        Dim inventarioFinalAjuste As Integer = LeerEntero(row.Cells("inventario_final"))

        ' Nueva fórmula: Inventario (+) + Llegada (+) - Despacho (-) - Bajas (-) - Ofertas (-) - Caja1 (-) - Caja2 (-) - Inventario Final (-)
        Dim totalInventario As Integer =
            inventario + llegada - despacho - bajas - ofertas - caja1 - caja2 - inventarioFinalAjuste

        'Entregar los valores calculados a las celdas correspondientes
        row.Cells("total").Value = totalInventario

        'Ajustar a colores los valores negativos como positivos
        ColorearTotal(fila)
    End Sub

    'En caso de que exista un exceso de stock, se torna verde, en caso de faltar stock, se torna rojo
    Private Sub ColorearTotal(fila As Integer)

        Dim row = dgvInventario.Rows(fila)
        If row.IsNewRow Then Exit Sub

        Dim celda = row.Cells("total")

        Dim valor As Integer = 0
        If celda.Value IsNot Nothing Then
            Integer.TryParse(celda.Value.ToString(), valor)
        End If

        ' Reset de estilo
        celda.Style.ForeColor = Color.Black

        ' Aplicar color según regla
        If valor > 0 Then
            celda.Style.ForeColor = Color.Green
        ElseIf valor < 0 Then
            celda.Style.ForeColor = Color.Red
            'Esto es en el caso de que el inventario final necesite de ser resaltado en rojo
            'dgvInventario.Rows(fila).DefaultCellStyle.BackColor = Color.MistyRose
        End If

    End Sub

    Private Sub dgvInventario_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles dgvInventario.EditingControlShowing
        Dim txt As TextBox = TryCast(e.Control, TextBox)
        If txt Is Nothing Then Exit Sub

        RemoveHandler txt.KeyPress, AddressOf SoloNumeros
        AddHandler txt.KeyPress, AddressOf SoloNumeros
    End Sub

    'Validar exclusivamente numeros en ciertas columnas
    Private Sub SoloNumeros(sender As Object, e As KeyPressEventArgs)
        If Not Char.IsDigit(e.KeyChar) AndAlso
           e.KeyChar <> ChrW(Keys.Back) Then
            e.Handled = True
        End If
    End Sub

    'Validar exclusivamente decimales en los calculos de saquitos
    Private Sub SoloDecimal(sender As Object, e As KeyPressEventArgs)
        If Not Char.IsDigit(e.KeyChar) AndAlso
       e.KeyChar <> "."c AndAlso
       e.KeyChar <> ","c AndAlso
       e.KeyChar <> ChrW(Keys.Back) Then
            e.Handled = True
        End If
    End Sub

    Private Sub btnAnteriorInventario_Click(sender As Object, e As EventArgs) Handles btnAnteriorInventario.Click

        Dim sql As String =
    "SELECT MAX(id_inventario)
     FROM inventario
     WHERE id_inventario < ?"

        Using cmd As New OleDbCommand(sql, Conexion.conexionDB)
            cmd.Parameters.AddWithValue("@id", InventarioActual)

            Dim result = cmd.ExecuteScalar()

            If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                CargarInventarioCompleto(CInt(result))
            End If
        End Using

    End Sub

    Private Sub btnSiguienteInventario_Click(sender As Object, e As EventArgs) Handles btnSiguienteInventario.Click

        Dim sql As String =
    "SELECT MIN(id_inventario)
     FROM inventario
     WHERE id_inventario > ?"

        Using cmd As New OleDbCommand(sql, Conexion.conexionDB)
            cmd.Parameters.AddWithValue("@id", InventarioActual)

            Dim result = cmd.ExecuteScalar()

            If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                CargarInventarioCompleto(CInt(result))
            End If
        End Using

    End Sub

    Private Function ExisteInventarioAnterior() As Boolean

        Dim sql As String = "SELECT COUNT(*) FROM inventario"

        Using cmd As New OleDbCommand(sql, Conexion.conexionDB)
            Return CInt(cmd.ExecuteScalar()) > 0
        End Using

    End Function

    Private Sub btnNuevoInventario_Click(sender As Object, e As EventArgs) Handles btnNuevoInventario.Click

        dgvInventario.EndEdit()
        dgvInventario.CommitEdit(DataGridViewDataErrorContexts.Commit)

        If GuardadoPendiente Then
            GuardarInventarioCabecera()
            GuardarInventarioDetalle()
            GuardadoPendiente = False
        End If

        dgvInventario.CommitEdit(DataGridViewDataErrorContexts.Commit)

        GuardadoPendiente = False

        ' Calculo y recalculo de los productos
        CalcularTotalesInventario()

        ' Guardar productos y detalles
        dgvInventario.EndEdit()

        If GuardadoPendiente Then
            GuardarInventarioDetalle()
            GuardarInventarioCabecera()
            GuardadoPendiente = False
        End If

        GuardarAuto = False
        CargandoInventario = True

        Dim esPrimero As Boolean = Not ExisteInventarioAnterior()

        ' Crear nuevo inventario
        CrearNuevoInventario()

        If Not esPrimero Then
            ' Flujo normal
            CopiarDesdeGridAlNuevoInventario(InventarioActual)
        Else
            ' PRIMER INVENTARIO → no copiar nada
            ' Solo cargar productos base en 0
            CargarProductosFijosEnGrid()
        End If

        ' Limpiar cabecera
        LimpiarCabeceraInventario()

        ' Cargar nuevo inventario
        CargarInventarioDetalle(InventarioActual)

        CalcularTotalesInventario()
        CargarListaInventarios()

        CargandoInventario = False
        GuardarAuto = True
    End Sub

    Private Sub LimpiarCabeceraInventario()

        txtProduccionTotal.Clear()
        txtProdBerlines.Clear()
        txtVentaPesos.Clear()
        txtPrecioPan.Clear()
        txtVentaPanKg.Clear()
        txtLlegoPan.Clear()
        txtSalidaPan.Clear()
        txtTotalPan.Clear()
        txtKilosVenta.Clear()
        txtRendimiento.Clear()
        txtFaltante.Clear()

        txtInventarioInicialSaquitos.Clear()
        txtIngresoSaquitos.Clear()
        txtSalidaSaquitos.Clear()
        txtInventarioFinalSaquitos.Clear()

        cbPasteleroDia.SelectedIndex = -1

    End Sub

    Private Sub CargarInventarioCompleto(id As Integer)

        GuardarAuto = False
        CargandoInventario = True

        InventarioActual = id

        CargarInventarioCabecera(id)
        CargarInventarioDetalle(id)
        CalcularTotalesInventario()

        CargandoInventario = False
        GuardarAuto = True

    End Sub

    Private Sub CargarListaInventarios()

        Dim dt As New DataTable()

        Dim sql As String =
        "SELECT id_inventario, fecha_inventario
         FROM inventario
         ORDER BY id_inventario DESC"

        Using da As New OleDbDataAdapter(sql, Conexion.conexionDB)
            da.Fill(dt)
        End Using

        ' Eliminar filas sin id_inventario para evitar celdas vacías en el grid
        For i As Integer = dt.Rows.Count - 1 To 0 Step -1
            If dt.Rows(i).IsNull("id_inventario") Then
                dt.Rows.RemoveAt(i)
            End If
        Next

        dgvListaInventarios.DataSource = dt

        ' Ajustes de encabezados y formato para claridad
        If dgvListaInventarios.Columns.Contains("id_inventario") Then
            dgvListaInventarios.Columns("id_inventario").HeaderText = "ID. Inv"
            dgvListaInventarios.Columns("id_inventario").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        End If

        If dgvListaInventarios.Columns.Contains("fecha_inventario") Then
            dgvListaInventarios.Columns("fecha_inventario").HeaderText = "Fecha Inv."
            dgvListaInventarios.Columns("fecha_inventario").DefaultCellStyle.Format = "dd/MM/yyyy"
        End If

        dgvListaInventarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvListaInventarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvListaInventarios.MultiSelect = False
        dgvListaInventarios.ReadOnly = True
        dgvListaInventarios.AllowUserToAddRows = False
        dgvInventario.AllowUserToOrderColumns = False
        dgvListaInventarios.ClearSelection()

    End Sub

    Private Sub cbPasteleroDia_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbPasteleroDia.SelectedIndexChanged
        If Not GuardarAuto Or InventarioActual = 0 Then Exit Sub

        ' Guardar inmediatamente la selección del pastelero del día
        GuardarInventarioCabecera()
    End Sub

    Private Sub dgvListaInventarios_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvListaInventarios.CellClick

        If e.RowIndex < 0 Then Exit Sub

        Dim cell = dgvListaInventarios.Rows(e.RowIndex).Cells("id_inventario")
        If cell Is Nothing OrElse cell.Value Is Nothing OrElse IsDBNull(cell.Value) Then
            ' fila vacía o celda sin valor → no hacer nada
            Exit Sub
        End If

        Dim idInventario As Integer
        If Integer.TryParse(Convert.ToString(cell.Value), idInventario) Then
            CargarInventarioCompleto(idInventario)
        End If

    End Sub

    Private Sub CargarPastelerosDia()

        Dim dt As New DataTable
        Dim sql As String = "SELECT id_pastelero, nombre_pastelero FROM pastelero ORDER BY nombre_pastelero"

        Using cmd As New OleDbCommand(sql, Conexion.conexionDB)
            Using da As New OleDbDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        cbPasteleroDia.DataSource = dt
        cbPasteleroDia.DisplayMember = "nombre_pastelero"
        cbPasteleroDia.ValueMember = "id_pastelero"
        cbPasteleroDia.SelectedIndex = -1

    End Sub

    '===========================
    '==AUTOGUARDADO INVENTARIO==
    '===========================
    Private GuardadoPendiente As Boolean = False
    Private GuardarAuto As Boolean = True
    Private InventarioActual As Integer = 0

    Private Sub tmrGuardar_Tick(sender As Object, e As EventArgs) Handles tmrGuardar.Tick

        If Not GuardadoPendiente Then Exit Sub

        GuardadoPendiente = False
        GuardarInventarioDetalle()

    End Sub

    Private Sub GuardarInventarioDetalle()

        CalcularTotalesInventario()

        If InventarioActual = 0 Then Exit Sub

        GuardarAuto = False

        Dim cn As OleDbConnection = Conexion.conexionDB
        If cn.State <> ConnectionState.Open Then cn.Open()

        For Each row As DataGridViewRow In dgvInventario.Rows

            If row.IsNewRow Then Continue For
            If row.Cells("id_producto").Value Is Nothing Then Continue For

            Dim inventario As Integer = LeerEntero(row.Cells("inventario"))
            Dim llegada As Integer = LeerEntero(row.Cells("llegada"))
            Dim despacho As Integer = LeerEntero(row.Cells("despacho"))
            Dim bajas As Integer = LeerEntero(row.Cells("bajas"))
            Dim ofertas As Integer = LeerEntero(row.Cells("ofertas"))
            Dim caja1 As Integer = LeerEntero(row.Cells("caja1"))
            Dim caja2 As Integer = LeerEntero(row.Cells("caja2"))

            Dim idProducto As Integer = CInt(row.Cells("id_producto").Value)

            Dim pasteleroCell = row.Cells("id_pastelero").Value
            Dim tienePastelero As Boolean = Not (pasteleroCell Is Nothing OrElse IsDBNull(pasteleroCell))
            Dim pasteleroId As Integer = 0
            If tienePastelero Then
                pasteleroId = Convert.ToInt32(pasteleroCell)
            End If

            Dim tieneMovimiento As Boolean =
            llegada <> 0 OrElse despacho <> 0 OrElse bajas <> 0 OrElse ofertas <> 0 OrElse caja1 <> 0 OrElse caja2 <> 0

            Dim tieneInventario As Boolean = inventario <> 0

            Dim existeRegistro As Boolean = ExisteDetalleInventario(InventarioActual, idProducto)

            If existeRegistro Then
                ' Si ya existe y no tiene datos relevantes → eliminar para evitar "basura"
                If Not tieneMovimiento AndAlso Not tieneInventario AndAlso Not tienePastelero Then
                    Dim sqlDel As String = "DELETE FROM inventario_detalle WHERE id_inventario = ? AND id_producto = ?"
                    Using cmdDel As New OleDbCommand(sqlDel, cn)
                        cmdDel.Parameters.AddWithValue("@p1", InventarioActual)
                        cmdDel.Parameters.AddWithValue("@p2", idProducto)
                        cmdDel.ExecuteNonQuery()
                    End Using
                    Continue For
                End If

                ' UPDATE
                Dim sqlUpd As String =
            "UPDATE inventario_detalle SET
                inventario_inicial = ?,
                llegada = ?,
                despacho = ?,
                bajas = ?,
                ofertas = ?,
                caja1 = ?,
                caja2 = ?,
                id_pastelero = ?
            WHERE id_inventario = ?
            AND id_producto = ?"

                Using cmd As New OleDbCommand(sqlUpd, cn)
                    cmd.Parameters.AddWithValue("@p1", inventario)
                    cmd.Parameters.AddWithValue("@p2", llegada)
                    cmd.Parameters.AddWithValue("@p3", despacho)
                    cmd.Parameters.AddWithValue("@p4", bajas)
                    cmd.Parameters.AddWithValue("@p5", ofertas)
                    cmd.Parameters.AddWithValue("@p6", caja1)
                    cmd.Parameters.AddWithValue("@p7", caja2)

                    If tienePastelero Then
                        cmd.Parameters.AddWithValue("@p8", pasteleroId)
                    Else
                        cmd.Parameters.AddWithValue("@p8", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValue("@p9", InventarioActual)
                    cmd.Parameters.AddWithValue("@p10", idProducto)

                    cmd.ExecuteNonQuery()
                End Using

            Else
                ' INSERT sólo si hay datos relevantes
                If Not (tieneMovimiento OrElse tieneInventario OrElse tienePastelero) Then
                    Continue For
                End If

                Dim sqlIns As String =
            "INSERT INTO inventario_detalle
            (id_inventario, id_producto,
             inventario_inicial, llegada, despacho, bajas, ofertas,
             caja1, caja2, id_pastelero)
            VALUES (?,?,?,?,?,?,?,?,?,?)"

                Using cmd As New OleDbCommand(sqlIns, cn)
                    cmd.Parameters.AddWithValue("@p1", InventarioActual)
                    cmd.Parameters.AddWithValue("@p2", idProducto)
                    cmd.Parameters.AddWithValue("@p3", inventario)
                    cmd.Parameters.AddWithValue("@p4", llegada)
                    cmd.Parameters.AddWithValue("@p5", despacho)
                    cmd.Parameters.AddWithValue("@p6", bajas)
                    cmd.Parameters.AddWithValue("@p7", ofertas)
                    cmd.Parameters.AddWithValue("@p8", caja1)
                    cmd.Parameters.AddWithValue("@p9", caja2)

                    If tienePastelero Then
                        cmd.Parameters.AddWithValue("@p10", pasteleroId)
                    Else
                        cmd.Parameters.AddWithValue("@p10", DBNull.Value)
                    End If

                    cmd.ExecuteNonQuery()
                End Using

            End If

        Next

        GuardarAuto = True

    End Sub

    Private Function ExisteAlgúnDetalle(idInventario As Integer) As Boolean

        Dim sql As String =
        "SELECT COUNT(*) FROM inventario_detalle WHERE id_inventario = ?"

        Using cmd As New OleDbCommand(sql, Conexion.conexionDB)
            cmd.Parameters.AddWithValue("@id", idInventario)
            Return CInt(cmd.ExecuteScalar()) > 0
        End Using

    End Function

    Private Function ExisteDetalleInventario(idInventario As Integer, idProducto As Integer) As Boolean

        Dim sql As String =
    "SELECT COUNT(*) 
     FROM inventario_detalle 
     WHERE id_inventario = ? AND id_producto = ?"

        Using cmd As New OleDbCommand(sql, Conexion.conexionDB)
            cmd.Parameters.AddWithValue("@inv", idInventario)
            cmd.Parameters.AddWithValue("@prod", idProducto)

            Return CInt(cmd.ExecuteScalar()) > 0
        End Using

    End Function

    Private Sub GuardarInventarioCabecera()

        If Not GuardarAuto OrElse InventarioActual = 0 Then Exit Sub

        Dim sql As String =
            "UPDATE inventario SET
            id_pastelero_dia = ?,
            produccion_pastelero = ?,
            produccion_berlin = ?,
            venta_pesos = ?,
            precio_pan = ?,
            venta_pan_kg = ?,
            llego_pan = ?,
            salida_pan_manual = ?,
            total_pan = ?,
            kilos_venta = ?,
            rendimiento = ?,
            faltante = ?,
            saq_inv_inicial = ?,
            saq_ingreso = ?,
            saq_salida = ?,
            saq_inv_final = ?
        WHERE id_inventario = ?"

        Using cmd As New OleDbCommand(sql, Conexion.conexionDB)

            cmd.Parameters.AddWithValue("@id_pastelero",
            If(cbPasteleroDia.SelectedIndex = -1,
               DBNull.Value,
               cbPasteleroDia.SelectedValue))

            cmd.Parameters.AddWithValue("@p1", LeerDecimal(txtProduccionTotal))
            cmd.Parameters.AddWithValue("@p2", LeerDecimal(txtProdBerlines))
            cmd.Parameters.AddWithValue("@p3", LeerDecimal(txtVentaPesos))
            cmd.Parameters.AddWithValue("@p4", LeerDecimal(txtPrecioPan))
            cmd.Parameters.AddWithValue("@p5", LeerDecimal(txtVentaPanKg))
            cmd.Parameters.AddWithValue("@p6", LeerDecimal(txtLlegoPan))
            cmd.Parameters.AddWithValue("@p7", LeerDecimal(txtSalidaPan))
            cmd.Parameters.AddWithValue("@p8", LeerDecimal(txtTotalPan))
            cmd.Parameters.AddWithValue("@p9", LeerDecimal(txtKilosVenta))
            cmd.Parameters.AddWithValue("@p10", LeerDecimal(txtRendimiento))
            cmd.Parameters.AddWithValue("@p11", LeerDecimal(txtFaltante))
            cmd.Parameters.AddWithValue("@p12", LeerDecimal(txtInventarioInicialSaquitos))
            cmd.Parameters.AddWithValue("@p13", LeerDecimal(txtIngresoSaquitos))
            cmd.Parameters.AddWithValue("@p14", LeerDecimal(txtSalidaSaquitos))
            cmd.Parameters.AddWithValue("@p15", LeerDecimal(txtInventarioFinalSaquitos))
            cmd.Parameters.AddWithValue("@p16", InventarioActual)

            cmd.ExecuteNonQuery()
        End Using

    End Sub

    Private Sub CargarOCrearInventario()

        If ExisteInventarioAnterior() Then

            Dim sql As String =
        "SELECT MAX(id_inventario) FROM inventario"

            Using cmd As New OleDbCommand(sql, Conexion.conexionDB)
                InventarioActual = CInt(cmd.ExecuteScalar())
            End Using

        Else
            CrearNuevoInventario()
        End If

    End Sub

    Private Sub CargarInventarioDetalle(idInventario As Integer)
        CargandoInventario = True
        GuardarAuto = False

        ' SIEMPRE limpiar y cargar productos base PRIMERO
        dgvInventario.Rows.Clear()
        CargarProductosFijosEnGrid()

        Dim sql As String = "
    SELECT p.id_producto, p.codigo, p.nombre, p.precio_venta_boleta,
           d.inventario_inicial, d.llegada, d.despacho, d.bajas, 
           d.ofertas, d.caja1, d.caja2, d.id_pastelero
    FROM inventario_detalle d
    INNER JOIN producto p ON d.id_producto = p.id_producto
    WHERE d.id_inventario = ?"

        Dim dt As New DataTable()

        Using cmd As New OleDbCommand(sql, Conexion.conexionDB)
            cmd.Parameters.AddWithValue("@id", idInventario)
            Using dr As OleDbDataReader = cmd.ExecuteReader()
                dt.Load(dr)
            End Using
        End Using

        ' Si NO hay datos en BD → PRIMERA VEZ → mantener productos base en 0
        If dt.Rows.Count = 0 Then
            ' Ya están cargados por CargarProductosFijosEnGrid()
            ' Solo recalcular filas
            For Each row As DataGridViewRow In dgvInventario.Rows
                If Not row.IsNewRow Then
                    RecalcularFila(row.Index)
                End If
            Next
            GuardarAuto = True
            CargandoInventario = False
            Exit Sub
        End If

        ' ACTUALIZAR productos existentes con datos de BD
        For Each dr As DataRow In dt.Rows
            Dim idProducto As Integer = CInt(dr("id_producto"))

            ' Buscar fila en grid
            For Each row As DataGridViewRow In dgvInventario.Rows
                If row.IsNewRow Then Continue For
                If row.Cells("id_producto").Value Is Nothing Then Continue For

                If CInt(row.Cells("id_producto").Value) = idProducto Then
                    row.Cells("inventario").Value = If(IsDBNull(dr("inventario_inicial")), 0, dr("inventario_inicial"))
                    row.Cells("llegada").Value = If(IsDBNull(dr("llegada")), 0, dr("llegada"))
                    row.Cells("despacho").Value = If(IsDBNull(dr("despacho")), 0, dr("despacho"))
                    row.Cells("bajas").Value = If(IsDBNull(dr("bajas")), 0, dr("bajas"))
                    row.Cells("ofertas").Value = If(IsDBNull(dr("ofertas")), 0, dr("ofertas"))
                    row.Cells("caja1").Value = If(IsDBNull(dr("caja1")), 0, dr("caja1"))
                    row.Cells("caja2").Value = If(IsDBNull(dr("caja2")), 0, dr("caja2"))
                    ' Forzar tipo entero para que el ComboBoxColumn muestre el DisplayMember en lugar del raw value
                    row.Cells("id_pastelero").Value = If(IsDBNull(dr("id_pastelero")), DBNull.Value, Convert.ToInt32(dr("id_pastelero")))
                    Exit For
                End If
            Next
        Next

        ' Recalcular TODAS las filas
        For Each row As DataGridViewRow In dgvInventario.Rows
            If Not row.IsNewRow Then
                RecalcularFila(row.Index)
            End If
        Next

        GuardarAuto = True
        CargandoInventario = False
    End Sub

    Private Sub CalcularTotalesInventario()
        If Not GuardarAuto Then Exit Sub

        ' recalcula filas
        For Each row As DataGridViewRow In dgvInventario.Rows
            If Not row.IsNewRow Then
                RecalcularFila(row.Index)
            End If
        Next

        ' recalcula totales generales
        RecalcularTotalesGenerales()
        RecalcularPan()
        RecalcularSaquitos()
        CalcularRendimiento()
        CalcularSaquitosLabel()
    End Sub

    Private Sub CargarInventarioCabecera(idInventario As Integer)

        GuardarAuto = False

        Dim sql As String =
    "SELECT *
     FROM inventario
     WHERE id_inventario = ?"

        Using cmd As New OleDbCommand(sql, Conexion.conexionDB)
            cmd.Parameters.AddWithValue("@id", idInventario)

            Using dr = cmd.ExecuteReader()
                If dr.Read() Then

                    txtProduccionTotal.Text = dr("produccion_pastelero").ToString()
                    txtProdBerlines.Text = dr("produccion_berlin").ToString()
                    txtVentaPesos.Text = dr("venta_pesos").ToString()
                    txtPrecioPan.Text = dr("precio_pan").ToString()
                    txtVentaPanKg.Text = dr("venta_pan_kg").ToString()
                    txtLlegoPan.Text = dr("llego_pan").ToString()
                    txtSalidaPan.Text = dr("salida_pan_manual").ToString()
                    txtTotalPan.Text = dr("total_pan").ToString()
                    txtKilosVenta.Text = dr("kilos_venta").ToString()
                    txtRendimiento.Text = dr("rendimiento").ToString()
                    txtFaltante.Text = dr("faltante").ToString()

                    txtInventarioInicialSaquitos.Text = dr("saq_inv_inicial").ToString()
                    txtIngresoSaquitos.Text = dr("saq_ingreso").ToString()
                    txtSalidaSaquitos.Text = dr("saq_salida").ToString()
                    txtInventarioFinalSaquitos.Text = dr("saq_inv_final").ToString()
                    If IsDBNull(dr("id_pastelero_dia")) Then
                        cbPasteleroDia.SelectedIndex = -1
                    Else
                        cbPasteleroDia.SelectedValue = Convert.ToInt32(dr("id_pastelero_dia"))
                    End If
                End If
            End Using
        End Using

        GuardarAuto = True

    End Sub

    Private Sub CrearNuevoInventario()

        Dim sql As String =
        "INSERT INTO inventario 
        (fecha_inventario, produccion_pastelero, fecha_creacion)
        VALUES (Date(), 0, Now())"

        Using cmd As New OleDbCommand(sql, Conexion.conexionDB)
            cmd.ExecuteNonQuery()

            cmd.CommandText = "SELECT @@IDENTITY"
            InventarioActual = CInt(cmd.ExecuteScalar())
        End Using

    End Sub

End Class
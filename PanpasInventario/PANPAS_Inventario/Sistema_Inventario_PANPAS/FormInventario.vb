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
    End Sub



    Private Sub FormInventario_FormClosing(sender As Object, e As FormClosingEventArgs) _
    Handles MyBase.FormClosing
        CerrarConexionDB()
    End Sub

    Private Sub BaseGridinventario()

        'Limpieza del Grid y establecimiento de las columnas a utilizar
        dgvInventario.Columns.Clear()
        dgvInventario.AllowUserToAddRows = True
        dgvInventario.AllowUserToDeleteRows = False
        dgvInventario.RowHeadersVisible = False
        dgvInventario.AutoGenerateColumns = False

        'Elementos para la columnas de pastelero (ComboBox)
        Dim colPastelero As New DataGridViewComboBoxColumn()
        colPastelero.Name = "id_pastelero"
        colPastelero.HeaderText = "Pastelero"
        colPastelero.FlatStyle = FlatStyle.Flat
        colPastelero.Width = 150

        Dim colIdProducto As New DataGridViewTextBoxColumn()
        colIdProducto.Name = "id_producto"
        colIdProducto.Visible = False
        dgvInventario.Columns.Add(colIdProducto)

        'Agregado de columnas al Grid
        dgvInventario.Columns.Add("codigo", "Codigo")
        dgvInventario.Columns.Add("nombre", "Producto")
        dgvInventario.Columns.Add("precio", "Precio")
        dgvInventario.Columns.Add("inventario", "Inventario")

        dgvInventario.Columns.Add("llegada", "Llegada (+)")
        dgvInventario.Columns.Add("despacho", "Despacho (-)")

        'Columnas de pastelero (ComboBox)
        dgvInventario.Columns.Add(colPastelero)

        dgvInventario.Columns.Add("bajas", "Bajas (-)")
        dgvInventario.Columns.Add("ofertas", "Ofertas (-)")
        dgvInventario.Columns.Add("caja1", "Caja 1 (-)")
        dgvInventario.Columns.Add("caja2", "Caja 2 (-)")

        dgvInventario.Columns.Add("inventario_final", "Inventario Final")
        dgvInventario.Columns.Add("total", "Total")

        'Configuracion de las columnas en modo lectura
        dgvInventario.Columns("nombre").ReadOnly = True
        dgvInventario.Columns("precio").ReadOnly = True
        'dgvInventario.Columns("inventario").ReadOnly = True
        dgvInventario.Columns("inventario_final").ReadOnly = True
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
            dgvInventario.CurrentCell =
            dgvInventario.Rows(fila + 1).Cells("codigo")
        End If

    End Sub

    'Insercion de producto al finalizar la edicion de codigo y autocompletarse con los datos
    Private Sub dgvInventario_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) _
    Handles dgvInventario.CellEndEdit

        dgvInventario.CommitEdit(DataGridViewDataErrorContexts.Commit)

        Dim colNombre As String = dgvInventario.Columns(e.ColumnIndex).Name

        If colNombre = "codigo" Then

            Dim codigo As String =
                If(dgvInventario.Rows(e.RowIndex).Cells("codigo").Value, "").ToString().Trim()

            If codigo = "" Then
                LimpiarFila(e.RowIndex)
                Exit Sub
            End If
            CargarProductoPorCodigo(e.RowIndex)
        Else
            RecalcularFila(e.RowIndex)
        End If

        If Not GuardarAuto Then Exit Sub
        GuardadoPendiente = True
        If e.RowIndex < 0 Then Exit Sub

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
        If txt.Text.Trim = "" Then Return 0D

        Dim valor As Decimal
        If Decimal.TryParse(txt.Text.Replace(",", "."), valor) Then
            Return valor
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

    'Funcion para verificar si hay productos en el grid
    Private Function ProductoExistente() As Boolean
        For Each fila As DataGridViewRow In dgvInventario.Rows
            If Not fila.IsNewRow AndAlso
           fila.Cells("codigo").Value IsNot Nothing Then
                Return True
            End If
        Next
        Return False
    End Function

    'Verificacion del codigo ya existente en el grid
    Private Function CodigoExistente(codigo As String, filaActual As Integer) As Boolean

        For Each fila As DataGridViewRow In dgvInventario.Rows

            If fila.Index <> filaActual AndAlso
           Not fila.IsNewRow AndAlso
           fila.Cells("codigo").Value IsNot Nothing AndAlso
           fila.Cells("codigo").Value.ToString() = codigo Then

                Return True
            End If
        Next
        Return False

    End Function

    Private Function LeerDecimalCelda(valor As Object) As Decimal

        If valor Is Nothing OrElse IsDBNull(valor) Then
            Return 0D
        End If

        If IsNumeric(valor) Then
            Return Convert.ToDecimal(valor)
        End If

        Dim texto As String = valor.ToString().Trim()

        If texto = "" Then Return 0D

        Decimal.TryParse(texto.Replace(",", "."), Globalization.NumberStyles.Any,
                     Globalization.CultureInfo.InvariantCulture, Nothing)

        Return Convert.ToDecimal(texto)

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

    'Cargar los productos en el grid a partir del codigo ingresado, si este ya existe, no es agregado al grid
    Private Sub CargarProductoPorCodigo(fila As Integer)

        Dim codigo As String = dgvInventario.Rows(fila).Cells("codigo").Value?.ToString()

        If CodigoExistente(codigo, fila) Then
            MessageBox.Show("Este producto ya ha sido ingresado!", "Producto Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            dgvInventario.Rows(fila).Cells("codigo").Value = Nothing
            dgvInventario.CurrentCell = dgvInventario.Rows(fila).Cells("codigo")
            Exit Sub
        End If

        If String.IsNullOrWhiteSpace(codigo) Then Exit Sub

        Dim sql As String =
        "SELECT id_producto, nombre, precio_venta_boleta
         FROM producto
         WHERE codigo = ?"

        Using cmd As New OleDb.OleDbCommand(sql, Conexion.conexionDB)
            cmd.Parameters.AddWithValue("@codigo", codigo)

            Using dr = cmd.ExecuteReader()
                If dr.Read() Then
                    dgvInventario.Rows(fila).Cells("id_producto").Value = dr("id_producto")
                    dgvInventario.Rows(fila).Cells("nombre").Value = dr("nombre")
                    dgvInventario.Rows(fila).Cells("precio").Value = dr("precio_venta_boleta")

                    ' Iniciar columnas con valores de cero
                    dgvInventario.Rows(fila).Cells("inventario").Value = 0
                    dgvInventario.Rows(fila).Cells("llegada").Value = 0
                    dgvInventario.Rows(fila).Cells("despacho").Value = 0
                    dgvInventario.Rows(fila).Cells("id_pastelero").Value = DBNull.Value
                    dgvInventario.Rows(fila).Cells("bajas").Value = 0
                    dgvInventario.Rows(fila).Cells("ofertas").Value = 0
                    dgvInventario.Rows(fila).Cells("caja1").Value = 0
                    dgvInventario.Rows(fila).Cells("caja2").Value = 0

                    RecalcularFila(fila)
                    GuardadoPendiente = True
                Else
                    MessageBox.Show("Código de producto no encontrado",
                                "Inventario",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning)

                    dgvInventario.Rows(fila).Cells("codigo").Value = Nothing
                End If
            End Using
        End Using

    End Sub

    Private Const RENDIMIENTO_OBJETIVO As Decimal = 32.5D

    Private Sub CalcularRendimiento()
        If Not GuardarAuto Then Exit Sub

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

        'Total = Total de los movimientos realizados
        Dim totalInventario As Integer =
            llegada - despacho - bajas - ofertas - caja1 - caja2

        'inventario final = Inventario inicial + Total de movimientos realizados
        Dim inventarioFinal As Integer =
            inventario + totalInventario

        'Entregar los valores calculados a las celdas correspondientes
        row.Cells("total").Value = totalInventario
        row.Cells("inventario_final").Value = inventarioFinal

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

    Private Sub btnNuevoInventario_Click(sender As Object, e As EventArgs) Handles btnNuevoInventario.Click
        GuardarAuto = False
        CargandoInventario = True

        CrearNuevoInventario()

        dgvInventario.Rows.Clear()

        LimpiarCabeceraInventario()

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

        If InventarioActual = 0 Then Exit Sub

        GuardarAuto = False

        Dim cn As OleDbConnection = Conexion.conexionDB

        If cn.State <> ConnectionState.Open Then
            cn.Open()
        End If

        For Each row As DataGridViewRow In dgvInventario.Rows

            If row.IsNewRow Then Continue For
            If row.Cells("id_producto").Value Is Nothing Then Continue For

            Dim idProducto As Integer = CInt(row.Cells("id_producto").Value)

            Dim sql As String

            If ExisteDetalleInventario(InventarioActual, idProducto) Then
                ' UPDATE
                sql =
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
            Else
                ' INSERT
                sql =
        "INSERT INTO inventario_detalle
        (id_inventario, id_producto,
         inventario_inicial, llegada, despacho, bajas, ofertas,
         caja1, caja2, id_pastelero)
        VALUES (?,?,?,?,?,?,?,?,?,?)"
            End If

            Using cmd As New OleDbCommand(sql, Conexion.conexionDB)

                If ExisteDetalleInventario(InventarioActual, idProducto) Then
                    ' UPDATE (orden correcto)
                    cmd.Parameters.AddWithValue("@invini", LeerDecimalCelda(row.Cells("inventario").Value))
                    cmd.Parameters.AddWithValue("@lleg", LeerDecimalCelda(row.Cells("llegada").Value))
                    cmd.Parameters.AddWithValue("@desp", LeerDecimalCelda(row.Cells("despacho").Value))
                    cmd.Parameters.AddWithValue("@baj", LeerDecimalCelda(row.Cells("bajas").Value))
                    cmd.Parameters.AddWithValue("@ofe", LeerDecimalCelda(row.Cells("ofertas").Value))
                    cmd.Parameters.AddWithValue("@c1", LeerDecimalCelda(row.Cells("caja1").Value))
                    cmd.Parameters.AddWithValue("@c2", LeerDecimalCelda(row.Cells("caja2").Value))
                    cmd.Parameters.AddWithValue("@past", If(IsDBNull(row.Cells("id_pastelero").Value), DBNull.Value, row.Cells("id_pastelero").Value))
                    cmd.Parameters.AddWithValue("@inv", InventarioActual)
                    cmd.Parameters.AddWithValue("@prod", idProducto)

                Else
                    ' INSERT (ORDEN EXACTO AL SQL)
                    cmd.Parameters.AddWithValue("@inv", InventarioActual)
                    cmd.Parameters.AddWithValue("@prod", idProducto)
                    cmd.Parameters.AddWithValue("@invini", LeerDecimalCelda(row.Cells("inventario").Value))
                    cmd.Parameters.AddWithValue("@lleg", LeerDecimalCelda(row.Cells("llegada").Value))
                    cmd.Parameters.AddWithValue("@desp", LeerDecimalCelda(row.Cells("despacho").Value))
                    cmd.Parameters.AddWithValue("@baj", LeerDecimalCelda(row.Cells("bajas").Value))
                    cmd.Parameters.AddWithValue("@ofe", LeerDecimalCelda(row.Cells("ofertas").Value))
                    cmd.Parameters.AddWithValue("@c1", LeerDecimalCelda(row.Cells("caja1").Value))
                    cmd.Parameters.AddWithValue("@c2", LeerDecimalCelda(row.Cells("caja2").Value))
                    cmd.Parameters.AddWithValue("@past", If(IsDBNull(row.Cells("id_pastelero").Value), DBNull.Value, row.Cells("id_pastelero").Value))
                End If

                cmd.ExecuteNonQuery()
            End Using

        Next

        GuardarAuto = True

    End Sub

    Private Function ObtenerIdProducto(codigo As String) As Integer

        Dim sql As String =
    "SELECT id_producto FROM producto WHERE codigo = ?"

        Using cmd As New OleDbCommand(sql, Conexion.conexionDB)
            cmd.Parameters.AddWithValue("@codigo", codigo)

            Dim resultado = cmd.ExecuteScalar()

            If resultado Is Nothing OrElse IsDBNull(resultado) Then
                Throw New Exception("Producto no existe en base de datos.")
            End If

            Return CInt(resultado)
        End Using

        Return 0

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

        Dim sql As String =
        "SELECT id_inventario 
         FROM inventario 
         WHERE fecha_inventario = Date()"

        Using cmd As New OleDbCommand(sql, Conexion.conexionDB)
            Dim resultado = cmd.ExecuteScalar()

            If resultado IsNot Nothing Then
                InventarioActual = CInt(resultado)
            Else
                CrearNuevoInventario()
            End If

        End Using

    End Sub

    Private Sub CargarInventarioDetalle(idInventario As Integer)

        CargandoInventario = True
        GuardarAuto = False

        dgvInventario.Rows.Clear()

        Dim sql As String =
        "SELECT p.id_producto,
               p.codigo,
               p.nombre,
               p.precio_venta_boleta,
               d.inventario_inicial,
               d.llegada,
               d.despacho,
               d.bajas,
               d.ofertas,
               d.caja1,
               d.caja2,
               d.id_pastelero
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

        For Each row As DataRow In dt.Rows

            Dim fila As Integer = dgvInventario.Rows.Add()

            With dgvInventario.Rows(fila)
                .Cells("id_producto").Value = row("id_producto")
                .Cells("codigo").Value = row("codigo")
                .Cells("nombre").Value = row("nombre")
                .Cells("precio").Value = row("precio_venta_boleta")

                .Cells("inventario").Value = row("inventario_inicial")
                .Cells("llegada").Value = row("llegada")
                .Cells("despacho").Value = row("despacho")
                .Cells("bajas").Value = row("bajas")
                .Cells("ofertas").Value = row("ofertas")
                .Cells("id_pastelero").Value =
                If(IsDBNull(row("id_pastelero")),
                   DBNull.Value,
                   Convert.ToInt32(row("id_pastelero")))

                .Cells("caja1").Value = row("caja1")
                .Cells("caja2").Value = row("caja2")
            End With

            RecalcularFila(fila)
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

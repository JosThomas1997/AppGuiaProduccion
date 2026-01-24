Public Class FormInventario

    Private Sub FormInventario_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AbrirConexionDB()
        BaseGridinventario()
        CargarPasteleros()
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
        colPastelero.DisplayMember = "nombre_pastelero"
        colPastelero.ValueMember = "id_pastelero"
        colPastelero.FlatStyle = FlatStyle.Flat
        colPastelero.Width = 150

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

    'Recalculo de la fila al cambiar algun valor numerico
    Private Sub dgvInventario_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) _
        Handles dgvInventario.CellValueChanged

        If e.RowIndex < 0 Then Exit Sub

        Dim col As String = dgvInventario.Columns(e.ColumnIndex).Name

        If col = "inventario" OrElse
       col = "llegada" OrElse
       col = "despacho" OrElse
       col = "bajas" OrElse
       col = "ofertas" Then

            RecalcularFila(e.RowIndex)
        End If
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

        EvaluarRendimiento(totalPan)

    End Sub

    Private Sub EvaluarRendimiento(totalPan As Decimal)

        If totalPan >= 65D Then
            txtTotalPan.BackColor = Color.LightGreen
            txtTotalPan.ForeColor = Color.Black
        Else
            txtTotalPan.BackColor = Color.MistyRose
            txtTotalPan.ForeColor = Color.DarkRed
        End If

    End Sub

    Private Sub txtPan_TextChanged(sender As Object, e As EventArgs) _
    Handles txtVentaPesos.TextChanged,
            txtPrecioPan.TextChanged,
            txtLlegoPan.TextChanged,
            txtSalidaPan.TextChanged

        CalcularPanVenta()

    End Sub

    Private Sub RecalcularSaquitos()

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

    'Cargar los pasteleros en el grid del combobox creado para tal fin
    Private Sub CargarPasteleros()
        Dim dt As New DataTable()
        Dim sql As String = "SELECT id_pastelero, nombre_pastelero FROM pastelero ORDER BY nombre_pastelero"

        Using cmd As New OleDb.OleDbCommand(sql, Conexion.conexionDB)
            Using da As New OleDb.OleDbDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        Dim comboCol As DataGridViewComboBoxColumn =
        CType(dgvInventario.Columns("id_pastelero"), DataGridViewComboBoxColumn)

        comboCol.DataSource = dt
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
        "SELECT nombre, precio_venta_boleta FROM producto WHERE codigo = ?"

        Using cmd As New OleDb.OleDbCommand(sql, Conexion.conexionDB)
            cmd.Parameters.AddWithValue("@codigo", codigo)

            Using dr = cmd.ExecuteReader()
                If dr.Read() Then

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

    Private Sub RecalcularFila(fila As Integer)

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

End Class

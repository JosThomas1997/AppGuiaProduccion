<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormInventario
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.lblFechaInventario = New System.Windows.Forms.Label()
        Me.DateTimePicker1 = New System.Windows.Forms.DateTimePicker()
        Me.dgvInventario = New System.Windows.Forms.DataGridView()
        Me.btnNuevoInventario = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cbPasteleroDia = New System.Windows.Forms.ComboBox()
        Me.btnSiguienteInventario = New System.Windows.Forms.Button()
        Me.btnAnteriorInventario = New System.Windows.Forms.Button()
        Me.gbInventariosExistentes = New System.Windows.Forms.GroupBox()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.gbSaquitosBerlines = New System.Windows.Forms.GroupBox()
        Me.lblSaquitos = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtProduccionTotal = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtInventarioFinalSaquitos = New System.Windows.Forms.TextBox()
        Me.txtBerlinesSaquitos = New System.Windows.Forms.TextBox()
        Me.txtSalidaSaquitos = New System.Windows.Forms.TextBox()
        Me.txtIngresoSaquitos = New System.Windows.Forms.TextBox()
        Me.txtInventarioInicialSaquitos = New System.Windows.Forms.TextBox()
        Me.txtProdBerlines = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.gbVentaPan = New System.Windows.Forms.GroupBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.txtTotalPan = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtSalidaPan = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtLlegoPan = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txtVentaPanKg = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtPrecioPan = New System.Windows.Forms.TextBox()
        Me.txtVentaPesos = New System.Windows.Forms.TextBox()
        Me.gbRendimiento = New System.Windows.Forms.GroupBox()
        Me.txtKilosVenta = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.txtFaltante = New System.Windows.Forms.TextBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.txtRendimiento = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.tmrGuardar = New System.Windows.Forms.Timer(Me.components)
        CType(Me.dgvInventario, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbInventariosExistentes.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbSaquitosBerlines.SuspendLayout()
        Me.gbVentaPan.SuspendLayout()
        Me.gbRendimiento.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblFechaInventario
        '
        Me.lblFechaInventario.AutoSize = True
        Me.lblFechaInventario.Location = New System.Drawing.Point(12, 9)
        Me.lblFechaInventario.Name = "lblFechaInventario"
        Me.lblFechaInventario.Size = New System.Drawing.Size(105, 13)
        Me.lblFechaInventario.TabIndex = 0
        Me.lblFechaInventario.Text = "Fecha de Inventario:"
        '
        'DateTimePicker1
        '
        Me.DateTimePicker1.Location = New System.Drawing.Point(12, 25)
        Me.DateTimePicker1.Name = "DateTimePicker1"
        Me.DateTimePicker1.Size = New System.Drawing.Size(200, 20)
        Me.DateTimePicker1.TabIndex = 1
        '
        'dgvInventario
        '
        Me.dgvInventario.AllowUserToAddRows = False
        Me.dgvInventario.AllowUserToDeleteRows = False
        Me.dgvInventario.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvInventario.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvInventario.Location = New System.Drawing.Point(12, 51)
        Me.dgvInventario.Name = "dgvInventario"
        Me.dgvInventario.RowHeadersVisible = False
        Me.dgvInventario.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.dgvInventario.Size = New System.Drawing.Size(1240, 358)
        Me.dgvInventario.TabIndex = 2
        '
        'btnNuevoInventario
        '
        Me.btnNuevoInventario.Location = New System.Drawing.Point(12, 415)
        Me.btnNuevoInventario.Name = "btnNuevoInventario"
        Me.btnNuevoInventario.Size = New System.Drawing.Size(104, 23)
        Me.btnNuevoInventario.TabIndex = 3
        Me.btnNuevoInventario.Text = "Nuevo Inventario"
        Me.btnNuevoInventario.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(215, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(111, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Producción Pastelero:"
        '
        'cbPasteleroDia
        '
        Me.cbPasteleroDia.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbPasteleroDia.FormattingEnabled = True
        Me.cbPasteleroDia.Location = New System.Drawing.Point(218, 24)
        Me.cbPasteleroDia.Name = "cbPasteleroDia"
        Me.cbPasteleroDia.Size = New System.Drawing.Size(121, 21)
        Me.cbPasteleroDia.TabIndex = 5
        '
        'btnSiguienteInventario
        '
        Me.btnSiguienteInventario.Location = New System.Drawing.Point(654, 415)
        Me.btnSiguienteInventario.Name = "btnSiguienteInventario"
        Me.btnSiguienteInventario.Size = New System.Drawing.Size(110, 23)
        Me.btnSiguienteInventario.TabIndex = 6
        Me.btnSiguienteInventario.Text = "Siguiente Inventario"
        Me.btnSiguienteInventario.UseVisualStyleBackColor = True
        '
        'btnAnteriorInventario
        '
        Me.btnAnteriorInventario.Location = New System.Drawing.Point(546, 415)
        Me.btnAnteriorInventario.Name = "btnAnteriorInventario"
        Me.btnAnteriorInventario.Size = New System.Drawing.Size(102, 23)
        Me.btnAnteriorInventario.TabIndex = 7
        Me.btnAnteriorInventario.Text = "Inventario Anterior"
        Me.btnAnteriorInventario.UseVisualStyleBackColor = True
        '
        'gbInventariosExistentes
        '
        Me.gbInventariosExistentes.Controls.Add(Me.DataGridView1)
        Me.gbInventariosExistentes.Location = New System.Drawing.Point(15, 444)
        Me.gbInventariosExistentes.Name = "gbInventariosExistentes"
        Me.gbInventariosExistentes.Size = New System.Drawing.Size(357, 225)
        Me.gbInventariosExistentes.TabIndex = 8
        Me.gbInventariosExistentes.TabStop = False
        Me.gbInventariosExistentes.Text = "Inventarios Existentes"
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(6, 19)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(345, 200)
        Me.DataGridView1.TabIndex = 0
        '
        'gbSaquitosBerlines
        '
        Me.gbSaquitosBerlines.Controls.Add(Me.lblSaquitos)
        Me.gbSaquitosBerlines.Controls.Add(Me.Label17)
        Me.gbSaquitosBerlines.Controls.Add(Me.Label8)
        Me.gbSaquitosBerlines.Controls.Add(Me.txtProduccionTotal)
        Me.gbSaquitosBerlines.Controls.Add(Me.Label7)
        Me.gbSaquitosBerlines.Controls.Add(Me.Label6)
        Me.gbSaquitosBerlines.Controls.Add(Me.Label5)
        Me.gbSaquitosBerlines.Controls.Add(Me.Label4)
        Me.gbSaquitosBerlines.Controls.Add(Me.Label3)
        Me.gbSaquitosBerlines.Controls.Add(Me.txtInventarioFinalSaquitos)
        Me.gbSaquitosBerlines.Controls.Add(Me.txtBerlinesSaquitos)
        Me.gbSaquitosBerlines.Controls.Add(Me.txtSalidaSaquitos)
        Me.gbSaquitosBerlines.Controls.Add(Me.txtIngresoSaquitos)
        Me.gbSaquitosBerlines.Controls.Add(Me.txtInventarioInicialSaquitos)
        Me.gbSaquitosBerlines.Controls.Add(Me.txtProdBerlines)
        Me.gbSaquitosBerlines.Controls.Add(Me.Label2)
        Me.gbSaquitosBerlines.Location = New System.Drawing.Point(378, 444)
        Me.gbSaquitosBerlines.Name = "gbSaquitosBerlines"
        Me.gbSaquitosBerlines.Size = New System.Drawing.Size(370, 225)
        Me.gbSaquitosBerlines.TabIndex = 9
        Me.gbSaquitosBerlines.TabStop = False
        Me.gbSaquitosBerlines.Text = "Saquitos / Berlines"
        '
        'lblSaquitos
        '
        Me.lblSaquitos.AutoSize = True
        Me.lblSaquitos.Location = New System.Drawing.Point(308, 183)
        Me.lblSaquitos.Name = "lblSaquitos"
        Me.lblSaquitos.Size = New System.Drawing.Size(10, 13)
        Me.lblSaquitos.TabIndex = 15
        Me.lblSaquitos.Text = "-"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.Location = New System.Drawing.Point(273, 183)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(29, 13)
        Me.Label17.TabIndex = 14
        Me.Label17.Text = "QQ:"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(167, 202)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(91, 13)
        Me.Label8.TabIndex = 13
        Me.Label8.Text = "Produccion Total:"
        '
        'txtProduccionTotal
        '
        Me.txtProduccionTotal.Location = New System.Drawing.Point(264, 199)
        Me.txtProduccionTotal.Name = "txtProduccionTotal"
        Me.txtProduccionTotal.ReadOnly = True
        Me.txtProduccionTotal.Size = New System.Drawing.Size(100, 20)
        Me.txtProduccionTotal.TabIndex = 12
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(45, 162)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(50, 13)
        Me.Label7.TabIndex = 11
        Me.Label7.Text = "Inv. Final"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(51, 136)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(47, 13)
        Me.Label6.TabIndex = 10
        Me.Label6.Text = "Berlines:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(15, 110)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(83, 13)
        Me.Label5.TabIndex = 9
        Me.Label5.Text = "Salida Saquitos:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(9, 84)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(89, 13)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Ingreso Saquitos:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(40, 58)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(58, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Inv. Inicial:"
        '
        'txtInventarioFinalSaquitos
        '
        Me.txtInventarioFinalSaquitos.Location = New System.Drawing.Point(101, 159)
        Me.txtInventarioFinalSaquitos.Name = "txtInventarioFinalSaquitos"
        Me.txtInventarioFinalSaquitos.Size = New System.Drawing.Size(100, 20)
        Me.txtInventarioFinalSaquitos.TabIndex = 6
        '
        'txtBerlinesSaquitos
        '
        Me.txtBerlinesSaquitos.Location = New System.Drawing.Point(101, 133)
        Me.txtBerlinesSaquitos.Name = "txtBerlinesSaquitos"
        Me.txtBerlinesSaquitos.ReadOnly = True
        Me.txtBerlinesSaquitos.Size = New System.Drawing.Size(100, 20)
        Me.txtBerlinesSaquitos.TabIndex = 5
        '
        'txtSalidaSaquitos
        '
        Me.txtSalidaSaquitos.Location = New System.Drawing.Point(101, 107)
        Me.txtSalidaSaquitos.Name = "txtSalidaSaquitos"
        Me.txtSalidaSaquitos.Size = New System.Drawing.Size(100, 20)
        Me.txtSalidaSaquitos.TabIndex = 4
        '
        'txtIngresoSaquitos
        '
        Me.txtIngresoSaquitos.Location = New System.Drawing.Point(101, 81)
        Me.txtIngresoSaquitos.Name = "txtIngresoSaquitos"
        Me.txtIngresoSaquitos.Size = New System.Drawing.Size(100, 20)
        Me.txtIngresoSaquitos.TabIndex = 3
        '
        'txtInventarioInicialSaquitos
        '
        Me.txtInventarioInicialSaquitos.Location = New System.Drawing.Point(101, 55)
        Me.txtInventarioInicialSaquitos.Name = "txtInventarioInicialSaquitos"
        Me.txtInventarioInicialSaquitos.Size = New System.Drawing.Size(100, 20)
        Me.txtInventarioInicialSaquitos.TabIndex = 2
        '
        'txtProdBerlines
        '
        Me.txtProdBerlines.Location = New System.Drawing.Point(264, 19)
        Me.txtProdBerlines.Name = "txtProdBerlines"
        Me.txtProdBerlines.Size = New System.Drawing.Size(100, 20)
        Me.txtProdBerlines.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(183, 22)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(75, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Prod. Berlines:"
        '
        'gbVentaPan
        '
        Me.gbVentaPan.Controls.Add(Me.Label14)
        Me.gbVentaPan.Controls.Add(Me.txtTotalPan)
        Me.gbVentaPan.Controls.Add(Me.Label13)
        Me.gbVentaPan.Controls.Add(Me.txtSalidaPan)
        Me.gbVentaPan.Controls.Add(Me.Label12)
        Me.gbVentaPan.Controls.Add(Me.txtLlegoPan)
        Me.gbVentaPan.Controls.Add(Me.Label11)
        Me.gbVentaPan.Controls.Add(Me.txtVentaPanKg)
        Me.gbVentaPan.Controls.Add(Me.Label10)
        Me.gbVentaPan.Controls.Add(Me.Label9)
        Me.gbVentaPan.Controls.Add(Me.txtPrecioPan)
        Me.gbVentaPan.Controls.Add(Me.txtVentaPesos)
        Me.gbVentaPan.Location = New System.Drawing.Point(754, 444)
        Me.gbVentaPan.Name = "gbVentaPan"
        Me.gbVentaPan.Size = New System.Drawing.Size(272, 225)
        Me.gbVentaPan.TabIndex = 10
        Me.gbVentaPan.TabStop = False
        Me.gbVentaPan.Text = "Venta Pan"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(104, 198)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(56, 13)
        Me.Label14.TabIndex = 11
        Me.Label14.Text = "Total Pan:"
        '
        'txtTotalPan
        '
        Me.txtTotalPan.Location = New System.Drawing.Point(166, 195)
        Me.txtTotalPan.Name = "txtTotalPan"
        Me.txtTotalPan.ReadOnly = True
        Me.txtTotalPan.Size = New System.Drawing.Size(100, 20)
        Me.txtTotalPan.TabIndex = 10
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(49, 148)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(61, 13)
        Me.Label13.TabIndex = 9
        Me.Label13.Text = "Salida Pan:"
        '
        'txtSalidaPan
        '
        Me.txtSalidaPan.Location = New System.Drawing.Point(116, 145)
        Me.txtSalidaPan.Name = "txtSalidaPan"
        Me.txtSalidaPan.Size = New System.Drawing.Size(100, 20)
        Me.txtSalidaPan.TabIndex = 8
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(52, 122)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(58, 13)
        Me.Label12.TabIndex = 7
        Me.Label12.Text = "Llego Pan:"
        '
        'txtLlegoPan
        '
        Me.txtLlegoPan.Location = New System.Drawing.Point(116, 119)
        Me.txtLlegoPan.Name = "txtLlegoPan"
        Me.txtLlegoPan.Size = New System.Drawing.Size(100, 20)
        Me.txtLlegoPan.TabIndex = 6
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(50, 96)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(60, 13)
        Me.Label11.TabIndex = 5
        Me.Label11.Text = "Venta Pan:"
        '
        'txtVentaPanKg
        '
        Me.txtVentaPanKg.Location = New System.Drawing.Point(116, 93)
        Me.txtVentaPanKg.Name = "txtVentaPanKg"
        Me.txtVentaPanKg.ReadOnly = True
        Me.txtVentaPanKg.Size = New System.Drawing.Size(100, 20)
        Me.txtVentaPanKg.TabIndex = 4
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(188, 26)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(62, 13)
        Me.Label10.TabIndex = 3
        Me.Label10.Text = "Precio Pan:"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(23, 26)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(72, 13)
        Me.Label9.TabIndex = 2
        Me.Label9.Text = "Venta Pan $ :"
        '
        'txtPrecioPan
        '
        Me.txtPrecioPan.Location = New System.Drawing.Point(167, 42)
        Me.txtPrecioPan.Name = "txtPrecioPan"
        Me.txtPrecioPan.Size = New System.Drawing.Size(100, 20)
        Me.txtPrecioPan.TabIndex = 1
        '
        'txtVentaPesos
        '
        Me.txtVentaPesos.Location = New System.Drawing.Point(7, 42)
        Me.txtVentaPesos.Name = "txtVentaPesos"
        Me.txtVentaPesos.Size = New System.Drawing.Size(100, 20)
        Me.txtVentaPesos.TabIndex = 0
        '
        'gbRendimiento
        '
        Me.gbRendimiento.Controls.Add(Me.txtKilosVenta)
        Me.gbRendimiento.Controls.Add(Me.Label18)
        Me.gbRendimiento.Controls.Add(Me.txtFaltante)
        Me.gbRendimiento.Controls.Add(Me.Label16)
        Me.gbRendimiento.Controls.Add(Me.txtRendimiento)
        Me.gbRendimiento.Controls.Add(Me.Label15)
        Me.gbRendimiento.Location = New System.Drawing.Point(1032, 444)
        Me.gbRendimiento.Name = "gbRendimiento"
        Me.gbRendimiento.Size = New System.Drawing.Size(220, 225)
        Me.gbRendimiento.TabIndex = 11
        Me.gbRendimiento.TabStop = False
        Me.gbRendimiento.Text = "Rendimiento"
        '
        'txtKilosVenta
        '
        Me.txtKilosVenta.Location = New System.Drawing.Point(48, 180)
        Me.txtKilosVenta.Name = "txtKilosVenta"
        Me.txtKilosVenta.ReadOnly = True
        Me.txtKilosVenta.Size = New System.Drawing.Size(133, 20)
        Me.txtKilosVenta.TabIndex = 5
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Italic)
        Me.Label18.Location = New System.Drawing.Point(33, 153)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(157, 25)
        Me.Label18.TabIndex = 4
        Me.Label18.Text = "Kilos en Venta:"
        '
        'txtFaltante
        '
        Me.txtFaltante.Location = New System.Drawing.Point(48, 116)
        Me.txtFaltante.Name = "txtFaltante"
        Me.txtFaltante.ReadOnly = True
        Me.txtFaltante.Size = New System.Drawing.Size(133, 20)
        Me.txtFaltante.TabIndex = 3
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Italic)
        Me.Label16.Location = New System.Drawing.Point(68, 88)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(96, 25)
        Me.Label16.TabIndex = 2
        Me.Label16.Text = "Faltante:"
        '
        'txtRendimiento
        '
        Me.txtRendimiento.Location = New System.Drawing.Point(48, 54)
        Me.txtRendimiento.Name = "txtRendimiento"
        Me.txtRendimiento.ReadOnly = True
        Me.txtRendimiento.Size = New System.Drawing.Size(133, 20)
        Me.txtRendimiento.TabIndex = 1
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(43, 26)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(138, 25)
        Me.Label15.TabIndex = 0
        Me.Label15.Text = "Rendimiento:"
        '
        'tmrGuardar
        '
        Me.tmrGuardar.Enabled = True
        Me.tmrGuardar.Interval = 1000
        '
        'FormInventario
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1264, 681)
        Me.Controls.Add(Me.gbRendimiento)
        Me.Controls.Add(Me.gbVentaPan)
        Me.Controls.Add(Me.gbSaquitosBerlines)
        Me.Controls.Add(Me.gbInventariosExistentes)
        Me.Controls.Add(Me.btnAnteriorInventario)
        Me.Controls.Add(Me.btnSiguienteInventario)
        Me.Controls.Add(Me.cbPasteleroDia)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnNuevoInventario)
        Me.Controls.Add(Me.dgvInventario)
        Me.Controls.Add(Me.DateTimePicker1)
        Me.Controls.Add(Me.lblFechaInventario)
        Me.Name = "FormInventario"
        Me.Text = "Inventario PANPAS"
        CType(Me.dgvInventario, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbInventariosExistentes.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbSaquitosBerlines.ResumeLayout(False)
        Me.gbSaquitosBerlines.PerformLayout()
        Me.gbVentaPan.ResumeLayout(False)
        Me.gbVentaPan.PerformLayout()
        Me.gbRendimiento.ResumeLayout(False)
        Me.gbRendimiento.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblFechaInventario As Label
    Friend WithEvents DateTimePicker1 As DateTimePicker
    Friend WithEvents dgvInventario As DataGridView
    Friend WithEvents btnNuevoInventario As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents cbPasteleroDia As ComboBox
    Friend WithEvents btnSiguienteInventario As Button
    Friend WithEvents btnAnteriorInventario As Button
    Friend WithEvents gbInventariosExistentes As GroupBox
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents gbSaquitosBerlines As GroupBox
    Friend WithEvents txtProdBerlines As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents txtInventarioFinalSaquitos As TextBox
    Friend WithEvents txtBerlinesSaquitos As TextBox
    Friend WithEvents txtSalidaSaquitos As TextBox
    Friend WithEvents txtIngresoSaquitos As TextBox
    Friend WithEvents txtInventarioInicialSaquitos As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents txtProduccionTotal As TextBox
    Friend WithEvents gbVentaPan As GroupBox
    Friend WithEvents Label9 As Label
    Friend WithEvents txtPrecioPan As TextBox
    Friend WithEvents txtVentaPesos As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents txtVentaPanKg As TextBox
    Friend WithEvents Label12 As Label
    Friend WithEvents txtLlegoPan As TextBox
    Friend WithEvents Label14 As Label
    Friend WithEvents txtTotalPan As TextBox
    Friend WithEvents Label13 As Label
    Friend WithEvents txtSalidaPan As TextBox
    Friend WithEvents gbRendimiento As GroupBox
    Friend WithEvents txtRendimiento As TextBox
    Friend WithEvents Label15 As Label
    Friend WithEvents txtFaltante As TextBox
    Friend WithEvents Label16 As Label
    Friend WithEvents Label17 As Label
    Friend WithEvents lblSaquitos As Label
    Friend WithEvents txtKilosVenta As TextBox
    Friend WithEvents Label18 As Label
    Friend WithEvents tmrGuardar As Timer
End Class

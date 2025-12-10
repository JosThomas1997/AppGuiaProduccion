<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormInformes
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormInformes))
        Me.dgvResultados = New System.Windows.Forms.DataGridView()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.grpFechasDesdeHasta = New System.Windows.Forms.GroupBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.dtpHasta = New System.Windows.Forms.DateTimePicker()
        Me.dtpDesde = New System.Windows.Forms.DateTimePicker()
        Me.rdbEntreFechas = New System.Windows.Forms.RadioButton()
        Me.grpFechasMesAnio = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cmbAnio = New System.Windows.Forms.ComboBox()
        Me.cmbMes = New System.Windows.Forms.ComboBox()
        Me.rdbPorMes = New System.Windows.Forms.RadioButton()
        Me.btnVentasLocales = New System.Windows.Forms.Button()
        Me.btnCorrelativoGuias = New System.Windows.Forms.Button()
        Me.btnVentasProductos = New System.Windows.Forms.Button()
        Me.btnResumenProduccion = New System.Windows.Forms.Button()
        Me.btnProduccionProductos = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtTotalInforme = New System.Windows.Forms.TextBox()
        Me.btnLimpiarTabla = New System.Windows.Forms.Button()
        Me.btnImprimirInforme = New System.Windows.Forms.Button()
        Me.documentoImpresion = New System.Drawing.Printing.PrintDocument()
        CType(Me.dgvResultados, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpFechasDesdeHasta.SuspendLayout()
        Me.grpFechasMesAnio.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgvResultados
        '
        Me.dgvResultados.AllowUserToAddRows = False
        Me.dgvResultados.AllowUserToDeleteRows = False
        Me.dgvResultados.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvResultados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvResultados.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.dgvResultados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvResultados.Location = New System.Drawing.Point(12, 306)
        Me.dgvResultados.Name = "dgvResultados"
        Me.dgvResultados.ReadOnly = True
        Me.dgvResultados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvResultados.Size = New System.Drawing.Size(912, 260)
        Me.dgvResultados.TabIndex = 0
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(299, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(327, 143)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'grpFechasDesdeHasta
        '
        Me.grpFechasDesdeHasta.Controls.Add(Me.Label2)
        Me.grpFechasDesdeHasta.Controls.Add(Me.Label1)
        Me.grpFechasDesdeHasta.Controls.Add(Me.dtpHasta)
        Me.grpFechasDesdeHasta.Controls.Add(Me.dtpDesde)
        Me.grpFechasDesdeHasta.Controls.Add(Me.rdbEntreFechas)
        Me.grpFechasDesdeHasta.Location = New System.Drawing.Point(12, 12)
        Me.grpFechasDesdeHasta.Name = "grpFechasDesdeHasta"
        Me.grpFechasDesdeHasta.Size = New System.Drawing.Size(281, 143)
        Me.grpFechasDesdeHasta.TabIndex = 2
        Me.grpFechasDesdeHasta.TabStop = False
        Me.grpFechasDesdeHasta.Text = "Fecha Desde y Hasta"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(117, 73)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(38, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Hasta:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(117, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Desde:"
        '
        'dtpHasta
        '
        Me.dtpHasta.Location = New System.Drawing.Point(39, 89)
        Me.dtpHasta.Name = "dtpHasta"
        Me.dtpHasta.Size = New System.Drawing.Size(200, 20)
        Me.dtpHasta.TabIndex = 2
        '
        'dtpDesde
        '
        Me.dtpDesde.Location = New System.Drawing.Point(39, 38)
        Me.dtpDesde.Name = "dtpDesde"
        Me.dtpDesde.Size = New System.Drawing.Size(200, 20)
        Me.dtpDesde.TabIndex = 1
        '
        'rdbEntreFechas
        '
        Me.rdbEntreFechas.AutoSize = True
        Me.rdbEntreFechas.Location = New System.Drawing.Point(91, 120)
        Me.rdbEntreFechas.Name = "rdbEntreFechas"
        Me.rdbEntreFechas.Size = New System.Drawing.Size(88, 17)
        Me.rdbEntreFechas.TabIndex = 0
        Me.rdbEntreFechas.TabStop = True
        Me.rdbEntreFechas.Text = "Entre Fechas"
        Me.rdbEntreFechas.UseVisualStyleBackColor = True
        '
        'grpFechasMesAnio
        '
        Me.grpFechasMesAnio.Controls.Add(Me.Label4)
        Me.grpFechasMesAnio.Controls.Add(Me.Label3)
        Me.grpFechasMesAnio.Controls.Add(Me.cmbAnio)
        Me.grpFechasMesAnio.Controls.Add(Me.cmbMes)
        Me.grpFechasMesAnio.Controls.Add(Me.rdbPorMes)
        Me.grpFechasMesAnio.Location = New System.Drawing.Point(632, 12)
        Me.grpFechasMesAnio.Name = "grpFechasMesAnio"
        Me.grpFechasMesAnio.Size = New System.Drawing.Size(292, 143)
        Me.grpFechasMesAnio.TabIndex = 5
        Me.grpFechasMesAnio.TabStop = False
        Me.grpFechasMesAnio.Text = "Fecha por Mes y Año"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(120, 72)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(29, 13)
        Me.Label4.TabIndex = 4
        Me.Label4.Text = "Año:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(120, 21)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(30, 13)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Mes:"
        '
        'cmbAnio
        '
        Me.cmbAnio.FormattingEnabled = True
        Me.cmbAnio.Location = New System.Drawing.Point(79, 88)
        Me.cmbAnio.Name = "cmbAnio"
        Me.cmbAnio.Size = New System.Drawing.Size(121, 21)
        Me.cmbAnio.TabIndex = 2
        '
        'cmbMes
        '
        Me.cmbMes.FormattingEnabled = True
        Me.cmbMes.Location = New System.Drawing.Point(79, 37)
        Me.cmbMes.Name = "cmbMes"
        Me.cmbMes.Size = New System.Drawing.Size(121, 21)
        Me.cmbMes.TabIndex = 1
        '
        'rdbPorMes
        '
        Me.rdbPorMes.AutoSize = True
        Me.rdbPorMes.Location = New System.Drawing.Point(97, 120)
        Me.rdbPorMes.Name = "rdbPorMes"
        Me.rdbPorMes.Size = New System.Drawing.Size(75, 17)
        Me.rdbPorMes.TabIndex = 0
        Me.rdbPorMes.TabStop = True
        Me.rdbPorMes.Text = "Mes y Año"
        Me.rdbPorMes.UseVisualStyleBackColor = True
        '
        'btnVentasLocales
        '
        Me.btnVentasLocales.Location = New System.Drawing.Point(281, 206)
        Me.btnVentasLocales.Name = "btnVentasLocales"
        Me.btnVentasLocales.Size = New System.Drawing.Size(118, 80)
        Me.btnVentasLocales.TabIndex = 6
        Me.btnVentasLocales.Text = "Ventas por locales"
        Me.btnVentasLocales.UseVisualStyleBackColor = True
        '
        'btnCorrelativoGuias
        '
        Me.btnCorrelativoGuias.Location = New System.Drawing.Point(405, 206)
        Me.btnCorrelativoGuias.Name = "btnCorrelativoGuias"
        Me.btnCorrelativoGuias.Size = New System.Drawing.Size(118, 80)
        Me.btnCorrelativoGuias.TabIndex = 7
        Me.btnCorrelativoGuias.Text = "Correlativo de guias"
        Me.btnCorrelativoGuias.UseVisualStyleBackColor = True
        '
        'btnVentasProductos
        '
        Me.btnVentasProductos.Location = New System.Drawing.Point(529, 206)
        Me.btnVentasProductos.Name = "btnVentasProductos"
        Me.btnVentasProductos.Size = New System.Drawing.Size(118, 80)
        Me.btnVentasProductos.TabIndex = 8
        Me.btnVentasProductos.Text = "Ventas por producto"
        Me.btnVentasProductos.UseVisualStyleBackColor = True
        '
        'btnResumenProduccion
        '
        Me.btnResumenProduccion.Location = New System.Drawing.Point(157, 206)
        Me.btnResumenProduccion.Name = "btnResumenProduccion"
        Me.btnResumenProduccion.Size = New System.Drawing.Size(118, 80)
        Me.btnResumenProduccion.TabIndex = 9
        Me.btnResumenProduccion.Text = "Resumen produccion"
        Me.btnResumenProduccion.UseVisualStyleBackColor = True
        '
        'btnProduccionProductos
        '
        Me.btnProduccionProductos.Location = New System.Drawing.Point(653, 206)
        Me.btnProduccionProductos.Name = "btnProduccionProductos"
        Me.btnProduccionProductos.Size = New System.Drawing.Size(118, 80)
        Me.btnProduccionProductos.TabIndex = 10
        Me.btnProduccionProductos.Text = "Produccion productos"
        Me.btnProduccionProductos.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(784, 587)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(34, 13)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "Total:"
        '
        'txtTotalInforme
        '
        Me.txtTotalInforme.Location = New System.Drawing.Point(824, 584)
        Me.txtTotalInforme.Name = "txtTotalInforme"
        Me.txtTotalInforme.Size = New System.Drawing.Size(100, 20)
        Me.txtTotalInforme.TabIndex = 12
        '
        'btnLimpiarTabla
        '
        Me.btnLimpiarTabla.Location = New System.Drawing.Point(405, 163)
        Me.btnLimpiarTabla.Name = "btnLimpiarTabla"
        Me.btnLimpiarTabla.Size = New System.Drawing.Size(118, 27)
        Me.btnLimpiarTabla.TabIndex = 13
        Me.btnLimpiarTabla.Text = "Limpiar Tabla"
        Me.btnLimpiarTabla.UseVisualStyleBackColor = True
        '
        'btnImprimirInforme
        '
        Me.btnImprimirInforme.Location = New System.Drawing.Point(653, 582)
        Me.btnImprimirInforme.Name = "btnImprimirInforme"
        Me.btnImprimirInforme.Size = New System.Drawing.Size(118, 23)
        Me.btnImprimirInforme.TabIndex = 14
        Me.btnImprimirInforme.Text = "Imprimir Informe"
        Me.btnImprimirInforme.UseVisualStyleBackColor = True
        '
        'FormInformes
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.ClientSize = New System.Drawing.Size(936, 616)
        Me.Controls.Add(Me.btnImprimirInforme)
        Me.Controls.Add(Me.btnLimpiarTabla)
        Me.Controls.Add(Me.txtTotalInforme)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.btnProduccionProductos)
        Me.Controls.Add(Me.btnResumenProduccion)
        Me.Controls.Add(Me.btnVentasProductos)
        Me.Controls.Add(Me.btnCorrelativoGuias)
        Me.Controls.Add(Me.btnVentasLocales)
        Me.Controls.Add(Me.grpFechasMesAnio)
        Me.Controls.Add(Me.grpFechasDesdeHasta)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.dgvResultados)
        Me.Name = "FormInformes"
        Me.Text = "Informes"
        CType(Me.dgvResultados, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpFechasDesdeHasta.ResumeLayout(False)
        Me.grpFechasDesdeHasta.PerformLayout()
        Me.grpFechasMesAnio.ResumeLayout(False)
        Me.grpFechasMesAnio.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dgvResultados As DataGridView
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents grpFechasDesdeHasta As GroupBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents dtpHasta As DateTimePicker
    Friend WithEvents dtpDesde As DateTimePicker
    Friend WithEvents rdbEntreFechas As RadioButton
    Friend WithEvents grpFechasMesAnio As GroupBox
    Friend WithEvents rdbPorMes As RadioButton
    Friend WithEvents cmbAnio As ComboBox
    Friend WithEvents cmbMes As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents btnVentasLocales As Button
    Friend WithEvents btnCorrelativoGuias As Button
    Friend WithEvents btnVentasProductos As Button
    Friend WithEvents btnResumenProduccion As Button
    Friend WithEvents btnProduccionProductos As Button
    Friend WithEvents Label5 As Label
    Friend WithEvents txtTotalInforme As TextBox
    Friend WithEvents btnLimpiarTabla As Button
    Friend WithEvents btnImprimirInforme As Button
    Friend WithEvents documentoImpresion As Printing.PrintDocument
End Class

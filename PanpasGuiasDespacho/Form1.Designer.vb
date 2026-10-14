<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblNumeroGuia = New System.Windows.Forms.Label()
        Me.txtNumeroGuia = New System.Windows.Forms.TextBox()
        Me.lblFecha = New System.Windows.Forms.Label()
        Me.lblCodigoCliente = New System.Windows.Forms.Label()
        Me.lblNombreCliente = New System.Windows.Forms.Label()
        Me.dgvGuiaProductos = New System.Windows.Forms.DataGridView()
        Me.txtTotal = New System.Windows.Forms.TextBox()
        Me.lblTotalPrecio = New System.Windows.Forms.Label()
        Me.btnAgregarProducto = New System.Windows.Forms.Button()
        Me.dtpFecha = New System.Windows.Forms.DateTimePicker()
        Me.btnNuevaGuia = New System.Windows.Forms.Button()
        Me.btnAnterior = New System.Windows.Forms.Button()
        Me.btnSiguiente = New System.Windows.Forms.Button()
        Me.dgvGuias = New System.Windows.Forms.DataGridView()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnActualizarGuias = New System.Windows.Forms.Button()
        Me.PrintDocument1 = New System.Drawing.Printing.PrintDocument()
        Me.btnImprimir = New System.Windows.Forms.Button()
        Me.cbCodCliente = New System.Windows.Forms.ComboBox()
        Me.btnProduccion = New System.Windows.Forms.Button()
        Me.btnEliminarGuia = New System.Windows.Forms.Button()
        Me.cbImpresora = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        CType(Me.dgvGuiaProductos, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvGuias, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblNumeroGuia
        '
        Me.lblNumeroGuia.AutoSize = True
        Me.lblNumeroGuia.Location = New System.Drawing.Point(42, 9)
        Me.lblNumeroGuia.Name = "lblNumeroGuia"
        Me.lblNumeroGuia.Size = New System.Drawing.Size(44, 13)
        Me.lblNumeroGuia.TabIndex = 0
        Me.lblNumeroGuia.Text = "Nª Guia"
        '
        'txtNumeroGuia
        '
        Me.txtNumeroGuia.Location = New System.Drawing.Point(12, 25)
        Me.txtNumeroGuia.Name = "txtNumeroGuia"
        Me.txtNumeroGuia.Size = New System.Drawing.Size(100, 20)
        Me.txtNumeroGuia.TabIndex = 1
        '
        'lblFecha
        '
        Me.lblFecha.AutoSize = True
        Me.lblFecha.Location = New System.Drawing.Point(200, 9)
        Me.lblFecha.Name = "lblFecha"
        Me.lblFecha.Size = New System.Drawing.Size(37, 13)
        Me.lblFecha.TabIndex = 2
        Me.lblFecha.Text = "Fecha"
        '
        'lblCodigoCliente
        '
        Me.lblCodigoCliente.AutoSize = True
        Me.lblCodigoCliente.Location = New System.Drawing.Point(358, 9)
        Me.lblCodigoCliente.Name = "lblCodigoCliente"
        Me.lblCodigoCliente.Size = New System.Drawing.Size(61, 13)
        Me.lblCodigoCliente.TabIndex = 5
        Me.lblCodigoCliente.Text = "Cod.Cliente"
        '
        'lblNombreCliente
        '
        Me.lblNombreCliente.AutoSize = True
        Me.lblNombreCliente.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNombreCliente.Location = New System.Drawing.Point(456, 19)
        Me.lblNombreCliente.Name = "lblNombreCliente"
        Me.lblNombreCliente.Size = New System.Drawing.Size(240, 25)
        Me.lblNombreCliente.TabIndex = 6
        Me.lblNombreCliente.Text = "INGRESAR ID CLIENTE"
        '
        'dgvGuiaProductos
        '
        Me.dgvGuiaProductos.AllowUserToOrderColumns = True
        Me.dgvGuiaProductos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvGuiaProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvGuiaProductos.Location = New System.Drawing.Point(12, 61)
        Me.dgvGuiaProductos.Name = "dgvGuiaProductos"
        Me.dgvGuiaProductos.Size = New System.Drawing.Size(787, 400)
        Me.dgvGuiaProductos.TabIndex = 7
        '
        'txtTotal
        '
        Me.txtTotal.Location = New System.Drawing.Point(687, 472)
        Me.txtTotal.Name = "txtTotal"
        Me.txtTotal.ReadOnly = True
        Me.txtTotal.Size = New System.Drawing.Size(100, 20)
        Me.txtTotal.TabIndex = 14
        '
        'lblTotalPrecio
        '
        Me.lblTotalPrecio.AutoSize = True
        Me.lblTotalPrecio.Location = New System.Drawing.Point(650, 475)
        Me.lblTotalPrecio.Name = "lblTotalPrecio"
        Me.lblTotalPrecio.Size = New System.Drawing.Size(34, 13)
        Me.lblTotalPrecio.TabIndex = 15
        Me.lblTotalPrecio.Text = "Total:"
        '
        'btnAgregarProducto
        '
        Me.btnAgregarProducto.Location = New System.Drawing.Point(12, 472)
        Me.btnAgregarProducto.Name = "btnAgregarProducto"
        Me.btnAgregarProducto.Size = New System.Drawing.Size(74, 23)
        Me.btnAgregarProducto.TabIndex = 20
        Me.btnAgregarProducto.Text = "Agregar"
        Me.btnAgregarProducto.UseVisualStyleBackColor = True
        '
        'dtpFecha
        '
        Me.dtpFecha.Location = New System.Drawing.Point(121, 25)
        Me.dtpFecha.Name = "dtpFecha"
        Me.dtpFecha.Size = New System.Drawing.Size(200, 20)
        Me.dtpFecha.TabIndex = 21
        '
        'btnNuevaGuia
        '
        Me.btnNuevaGuia.Location = New System.Drawing.Point(93, 472)
        Me.btnNuevaGuia.Name = "btnNuevaGuia"
        Me.btnNuevaGuia.Size = New System.Drawing.Size(75, 23)
        Me.btnNuevaGuia.TabIndex = 22
        Me.btnNuevaGuia.Text = "Nueva Guia"
        Me.btnNuevaGuia.UseVisualStyleBackColor = True
        '
        'btnAnterior
        '
        Me.btnAnterior.Location = New System.Drawing.Point(305, 472)
        Me.btnAnterior.Name = "btnAnterior"
        Me.btnAnterior.Size = New System.Drawing.Size(100, 23)
        Me.btnAnterior.TabIndex = 23
        Me.btnAnterior.Text = "Guia Anterior"
        Me.btnAnterior.UseVisualStyleBackColor = True
        '
        'btnSiguiente
        '
        Me.btnSiguiente.Location = New System.Drawing.Point(411, 472)
        Me.btnSiguiente.Name = "btnSiguiente"
        Me.btnSiguiente.Size = New System.Drawing.Size(100, 23)
        Me.btnSiguiente.TabIndex = 24
        Me.btnSiguiente.Text = "Guia Siguiente"
        Me.btnSiguiente.UseVisualStyleBackColor = True
        '
        'dgvGuias
        '
        Me.dgvGuias.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvGuias.Location = New System.Drawing.Point(815, 61)
        Me.dgvGuias.Name = "dgvGuias"
        Me.dgvGuias.Size = New System.Drawing.Size(303, 400)
        Me.dgvGuias.TabIndex = 25
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(810, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(69, 25)
        Me.Label1.TabIndex = 26
        Me.Label1.Text = "Guias:"
        '
        'btnActualizarGuias
        '
        Me.btnActualizarGuias.Location = New System.Drawing.Point(985, 472)
        Me.btnActualizarGuias.Name = "btnActualizarGuias"
        Me.btnActualizarGuias.Size = New System.Drawing.Size(140, 23)
        Me.btnActualizarGuias.TabIndex = 27
        Me.btnActualizarGuias.Text = "Ver historial completo"
        Me.btnActualizarGuias.UseVisualStyleBackColor = True
        '
        'btnImprimir
        '
        Me.btnImprimir.Location = New System.Drawing.Point(567, 472)
        Me.btnImprimir.Name = "btnImprimir"
        Me.btnImprimir.Size = New System.Drawing.Size(75, 23)
        Me.btnImprimir.TabIndex = 28
        Me.btnImprimir.Text = "Imprimir"
        Me.btnImprimir.UseVisualStyleBackColor = True
        '
        'cbCodCliente
        '
        Me.cbCodCliente.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cbCodCliente.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbCodCliente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbCodCliente.FormattingEnabled = True
        Me.cbCodCliente.Location = New System.Drawing.Point(327, 25)
        Me.cbCodCliente.Name = "cbCodCliente"
        Me.cbCodCliente.Size = New System.Drawing.Size(121, 21)
        Me.cbCodCliente.TabIndex = 29
        '
        'btnProduccion
        '
        Me.btnProduccion.BackColor = System.Drawing.Color.DodgerBlue
        Me.btnProduccion.Location = New System.Drawing.Point(815, 472)
        Me.btnProduccion.Name = "btnProduccion"
        Me.btnProduccion.Size = New System.Drawing.Size(115, 23)
        Me.btnProduccion.TabIndex = 30
        Me.btnProduccion.Text = "Produccion"
        Me.btnProduccion.UseVisualStyleBackColor = False
        '
        'btnEliminarGuia
        '
        Me.btnEliminarGuia.Location = New System.Drawing.Point(174, 472)
        Me.btnEliminarGuia.Name = "btnEliminarGuia"
        Me.btnEliminarGuia.Size = New System.Drawing.Size(81, 23)
        Me.btnEliminarGuia.TabIndex = 31
        Me.btnEliminarGuia.Text = "Eliminar Guia"
        Me.btnEliminarGuia.UseVisualStyleBackColor = True
        '
        'cbImpresora
        '
        Me.cbImpresora.FormattingEnabled = True
        Me.cbImpresora.Location = New System.Drawing.Point(966, 34)
        Me.cbImpresora.Name = "cbImpresora"
        Me.cbImpresora.Size = New System.Drawing.Size(152, 21)
        Me.cbImpresora.TabIndex = 32
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(999, 11)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(85, 20)
        Me.Label2.TabIndex = 33
        Me.Label2.Text = "Impresora:"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.LightGreen
        Me.ClientSize = New System.Drawing.Size(1132, 504)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.cbImpresora)
        Me.Controls.Add(Me.btnEliminarGuia)
        Me.Controls.Add(Me.btnProduccion)
        Me.Controls.Add(Me.cbCodCliente)
        Me.Controls.Add(Me.btnImprimir)
        Me.Controls.Add(Me.btnActualizarGuias)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.dgvGuias)
        Me.Controls.Add(Me.btnSiguiente)
        Me.Controls.Add(Me.btnAnterior)
        Me.Controls.Add(Me.btnNuevaGuia)
        Me.Controls.Add(Me.dtpFecha)
        Me.Controls.Add(Me.btnAgregarProducto)
        Me.Controls.Add(Me.lblTotalPrecio)
        Me.Controls.Add(Me.txtTotal)
        Me.Controls.Add(Me.dgvGuiaProductos)
        Me.Controls.Add(Me.lblNombreCliente)
        Me.Controls.Add(Me.lblCodigoCliente)
        Me.Controls.Add(Me.lblFecha)
        Me.Controls.Add(Me.txtNumeroGuia)
        Me.Controls.Add(Me.lblNumeroGuia)
        Me.Name = "Form1"
        Me.Text = "Guias PANPAS"
        CType(Me.dgvGuiaProductos, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvGuias, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblNumeroGuia As Label
    Friend WithEvents txtNumeroGuia As TextBox
    Friend WithEvents lblFecha As Label
    Friend WithEvents lblCodigoCliente As Label
    Friend WithEvents lblNombreCliente As Label
    Friend WithEvents dgvGuiaProductos As DataGridView
    Friend WithEvents txtTotal As TextBox
    Friend WithEvents lblTotalPrecio As Label
    Friend WithEvents btnAgregarProducto As Button
    Friend WithEvents dtpFecha As DateTimePicker
    Friend WithEvents btnNuevaGuia As Button
    Friend WithEvents btnAnterior As Button
    Friend WithEvents btnSiguiente As Button
    Friend WithEvents dgvGuias As DataGridView
    Friend WithEvents Label1 As Label
    Friend WithEvents btnActualizarGuias As Button
    Friend WithEvents PrintDocument1 As Printing.PrintDocument
    Friend WithEvents btnImprimir As Button
    Friend WithEvents cbCodCliente As ComboBox
    Friend WithEvents btnProduccion As Button
    Friend WithEvents btnEliminarGuia As Button
    Friend WithEvents cbImpresora As ComboBox
    Friend WithEvents Label2 As Label
End Class

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormInformeProducto
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
        Me.panelIconos = New System.Windows.Forms.Panel()
        Me.panelCodProducto = New System.Windows.Forms.Panel()
        Me.txtCodigoProducto = New System.Windows.Forms.TextBox()
        Me.lblCodigoProducto = New System.Windows.Forms.Label()
        Me.panelCentral = New System.Windows.Forms.Panel()
        Me.dgvInformeProductos = New System.Windows.Forms.DataGridView()
        Me.btnTodosLosProductos = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.panelCodProducto.SuspendLayout()
        CType(Me.dgvInformeProductos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panelIconos
        '
        Me.panelIconos.AutoScroll = True
        Me.panelIconos.Location = New System.Drawing.Point(12, 216)
        Me.panelIconos.Name = "panelIconos"
        Me.panelIconos.Size = New System.Drawing.Size(383, 276)
        Me.panelIconos.TabIndex = 0
        '
        'panelCodProducto
        '
        Me.panelCodProducto.Controls.Add(Me.txtCodigoProducto)
        Me.panelCodProducto.Controls.Add(Me.lblCodigoProducto)
        Me.panelCodProducto.Location = New System.Drawing.Point(12, 12)
        Me.panelCodProducto.Name = "panelCodProducto"
        Me.panelCodProducto.Size = New System.Drawing.Size(383, 111)
        Me.panelCodProducto.TabIndex = 1
        '
        'txtCodigoProducto
        '
        Me.txtCodigoProducto.Location = New System.Drawing.Point(85, 63)
        Me.txtCodigoProducto.Name = "txtCodigoProducto"
        Me.txtCodigoProducto.Size = New System.Drawing.Size(208, 20)
        Me.txtCodigoProducto.TabIndex = 1
        '
        'lblCodigoProducto
        '
        Me.lblCodigoProducto.AutoSize = True
        Me.lblCodigoProducto.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCodigoProducto.Location = New System.Drawing.Point(59, 25)
        Me.lblCodigoProducto.Name = "lblCodigoProducto"
        Me.lblCodigoProducto.Size = New System.Drawing.Size(257, 24)
        Me.lblCodigoProducto.TabIndex = 0
        Me.lblCodigoProducto.Text = "Ingresar Codigo de Producto:"
        '
        'panelCentral
        '
        Me.panelCentral.Location = New System.Drawing.Point(401, 12)
        Me.panelCentral.Name = "panelCentral"
        Me.panelCentral.Size = New System.Drawing.Size(353, 480)
        Me.panelCentral.TabIndex = 2
        '
        'dgvInformeProductos
        '
        Me.dgvInformeProductos.AllowUserToAddRows = False
        Me.dgvInformeProductos.AllowUserToDeleteRows = False
        Me.dgvInformeProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvInformeProductos.Location = New System.Drawing.Point(760, 12)
        Me.dgvInformeProductos.Name = "dgvInformeProductos"
        Me.dgvInformeProductos.ReadOnly = True
        Me.dgvInformeProductos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvInformeProductos.Size = New System.Drawing.Size(361, 480)
        Me.dgvInformeProductos.TabIndex = 3
        '
        'btnTodosLosProductos
        '
        Me.btnTodosLosProductos.Location = New System.Drawing.Point(141, 163)
        Me.btnTodosLosProductos.Name = "btnTodosLosProductos"
        Me.btnTodosLosProductos.Size = New System.Drawing.Size(114, 38)
        Me.btnTodosLosProductos.TabIndex = 4
        Me.btnTodosLosProductos.Text = "Todos los Productos"
        Me.btnTodosLosProductos.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(52, 137)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(316, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "En caso de necesitar todos los productos, haga click en el boton:"
        '
        'FormInformeProducto
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1133, 504)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnTodosLosProductos)
        Me.Controls.Add(Me.dgvInformeProductos)
        Me.Controls.Add(Me.panelCentral)
        Me.Controls.Add(Me.panelCodProducto)
        Me.Controls.Add(Me.panelIconos)
        Me.Name = "FormInformeProducto"
        Me.Text = "Seleccionar Producto"
        Me.panelCodProducto.ResumeLayout(False)
        Me.panelCodProducto.PerformLayout()
        CType(Me.dgvInformeProductos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents panelIconos As Panel
    Friend WithEvents panelCodProducto As Panel
    Friend WithEvents panelCentral As Panel
    Friend WithEvents dgvInformeProductos As DataGridView
    Friend WithEvents txtCodigoProducto As TextBox
    Friend WithEvents lblCodigoProducto As Label
    Friend WithEvents btnTodosLosProductos As Button
    Friend WithEvents Label1 As Label
End Class

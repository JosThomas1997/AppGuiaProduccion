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
        Me.panelCodProducto.SuspendLayout()
        CType(Me.dgvInformeProductos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panelIconos
        '
        Me.panelIconos.AutoScroll = True
        Me.panelIconos.Location = New System.Drawing.Point(12, 129)
        Me.panelIconos.Name = "panelIconos"
        Me.panelIconos.Size = New System.Drawing.Size(383, 363)
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
        Me.txtCodigoProducto.Location = New System.Drawing.Point(83, 67)
        Me.txtCodigoProducto.Name = "txtCodigoProducto"
        Me.txtCodigoProducto.Size = New System.Drawing.Size(208, 20)
        Me.txtCodigoProducto.TabIndex = 1
        '
        'lblCodigoProducto
        '
        Me.lblCodigoProducto.AutoSize = True
        Me.lblCodigoProducto.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCodigoProducto.Location = New System.Drawing.Point(57, 29)
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
        Me.dgvInformeProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvInformeProductos.Location = New System.Drawing.Point(760, 12)
        Me.dgvInformeProductos.Name = "dgvInformeProductos"
        Me.dgvInformeProductos.Size = New System.Drawing.Size(361, 480)
        Me.dgvInformeProductos.TabIndex = 3
        '
        'FormInformeProducto
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1133, 504)
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

    End Sub

    Friend WithEvents panelIconos As Panel
    Friend WithEvents panelCodProducto As Panel
    Friend WithEvents panelCentral As Panel
    Friend WithEvents dgvInformeProductos As DataGridView
    Friend WithEvents txtCodigoProducto As TextBox
    Friend WithEvents lblCodigoProducto As Label
End Class

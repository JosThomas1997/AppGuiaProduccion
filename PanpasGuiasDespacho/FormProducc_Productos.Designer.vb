<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormProducc_Productos
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
        Me.panelIzquierdo = New System.Windows.Forms.FlowLayoutPanel()
        Me.panelCentral = New System.Windows.Forms.Panel()
        Me.dgvProducProductos = New System.Windows.Forms.DataGridView()
        CType(Me.dgvProducProductos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panelIzquierdo
        '
        Me.panelIzquierdo.AutoScroll = True
        Me.panelIzquierdo.BackColor = System.Drawing.Color.WhiteSmoke
        Me.panelIzquierdo.Dock = System.Windows.Forms.DockStyle.Left
        Me.panelIzquierdo.Location = New System.Drawing.Point(0, 0)
        Me.panelIzquierdo.Name = "panelIzquierdo"
        Me.panelIzquierdo.Size = New System.Drawing.Size(383, 504)
        Me.panelIzquierdo.TabIndex = 0
        '
        'panelCentral
        '
        Me.panelCentral.Dock = System.Windows.Forms.DockStyle.Fill
        Me.panelCentral.Location = New System.Drawing.Point(383, 0)
        Me.panelCentral.Name = "panelCentral"
        Me.panelCentral.Size = New System.Drawing.Size(750, 504)
        Me.panelCentral.TabIndex = 1
        '
        'dgvProducProductos
        '
        Me.dgvProducProductos.AllowUserToOrderColumns = True
        Me.dgvProducProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvProducProductos.Dock = System.Windows.Forms.DockStyle.Right
        Me.dgvProducProductos.Location = New System.Drawing.Point(802, 0)
        Me.dgvProducProductos.Name = "dgvProducProductos"
        Me.dgvProducProductos.ReadOnly = True
        Me.dgvProducProductos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvProducProductos.Size = New System.Drawing.Size(331, 504)
        Me.dgvProducProductos.TabIndex = 2
        '
        'FormProducc_Productos
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1133, 504)
        Me.Controls.Add(Me.dgvProducProductos)
        Me.Controls.Add(Me.panelCentral)
        Me.Controls.Add(Me.panelIzquierdo)
        Me.Name = "FormProducc_Productos"
        Me.Text = "Agregar Productos"
        CType(Me.dgvProducProductos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents panelIzquierdo As FlowLayoutPanel
    Friend WithEvents panelCentral As Panel
    Friend WithEvents dgvProducProductos As DataGridView
End Class

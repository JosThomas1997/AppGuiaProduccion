<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormProductos
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
        Me.panelIzquierdo = New System.Windows.Forms.FlowLayoutPanel()
        Me.panelCentral = New System.Windows.Forms.Panel()
        Me.dgvProductos = New System.Windows.Forms.DataGridView()
        CType(Me.dgvProductos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panelIzquierdo
        '
        Me.panelIzquierdo.AutoScroll = True
        Me.panelIzquierdo.Dock = System.Windows.Forms.DockStyle.Left
        Me.panelIzquierdo.Location = New System.Drawing.Point(0, 0)
        Me.panelIzquierdo.Name = "panelIzquierdo"
        Me.panelIzquierdo.Size = New System.Drawing.Size(383, 504)
        Me.panelIzquierdo.TabIndex = 0
        '
        'panelCentral
        '
        Me.panelCentral.Location = New System.Drawing.Point(389, 0)
        Me.panelCentral.Name = "panelCentral"
        Me.panelCentral.Size = New System.Drawing.Size(407, 504)
        Me.panelCentral.TabIndex = 1
        '
        'dgvProductos
        '
        Me.dgvProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvProductos.Dock = System.Windows.Forms.DockStyle.Right
        Me.dgvProductos.Location = New System.Drawing.Point(802, 0)
        Me.dgvProductos.Name = "dgvProductos"
        Me.dgvProductos.Size = New System.Drawing.Size(331, 504)
        Me.dgvProductos.TabIndex = 2
        '
        'FormProductos
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1133, 504)
        Me.Controls.Add(Me.dgvProductos)
        Me.Controls.Add(Me.panelCentral)
        Me.Controls.Add(Me.panelIzquierdo)
        Me.Name = "FormProductos"
        Me.Text = "Agregar Productos"
        CType(Me.dgvProductos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents panelIzquierdo As FlowLayoutPanel
    Friend WithEvents panelCentral As Panel
    Friend WithEvents dgvProductos As DataGridView
End Class

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormReactivarProductos
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it with the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.lblTitulo = New System.Windows.Forms.Label()
        Me.lblEstado = New System.Windows.Forms.Label()
        Me.btnReactivar = New System.Windows.Forms.Button()
        Me.btnReactivarTodos = New System.Windows.Forms.Button()
        Me.btnCancelar = New System.Windows.Forms.Button()
        Me.lblDescripcion = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblTitulo
        '
        Me.lblTitulo.AutoSize = True
        Me.lblTitulo.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitulo.Location = New System.Drawing.Point(12, 9)
        Me.lblTitulo.Name = "lblTitulo"
        Me.lblTitulo.Size = New System.Drawing.Size(274, 25)
        Me.lblTitulo.TabIndex = 0
        Me.lblTitulo.Text = "Reactivar Productos Inactivos"
        '
        'lblEstado
        '
        Me.lblEstado.AutoSize = True
        Me.lblEstado.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblEstado.Location = New System.Drawing.Point(12, 50)
        Me.lblEstado.Name = "lblEstado"
        Me.lblEstado.Size = New System.Drawing.Size(171, 19)
        Me.lblEstado.TabIndex = 1
        Me.lblEstado.Text = "Verificando estado de BD..."
        '
        'btnReactivar
        '
        Me.btnReactivar.BackColor = System.Drawing.Color.FromArgb(CType(CType(76, Byte), Integer), CType(CType(175, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.btnReactivar.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnReactivar.ForeColor = System.Drawing.Color.White
        Me.btnReactivar.Location = New System.Drawing.Point(12, 130)
        Me.btnReactivar.Name = "btnReactivar"
        Me.btnReactivar.Size = New System.Drawing.Size(200, 40)
        Me.btnReactivar.TabIndex = 3
        Me.btnReactivar.Text = "Reactivar Productos"
        Me.btnReactivar.UseVisualStyleBackColor = False
        '
        'btnReactivarTodos
        '
        Me.btnReactivarTodos.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(152, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnReactivarTodos.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnReactivarTodos.ForeColor = System.Drawing.Color.White
        Me.btnReactivarTodos.Location = New System.Drawing.Point(230, 130)
        Me.btnReactivarTodos.Name = "btnReactivarTodos"
        Me.btnReactivarTodos.Size = New System.Drawing.Size(200, 40)
        Me.btnReactivarTodos.TabIndex = 4
        Me.btnReactivarTodos.Text = "Reactivar TODOS"
        Me.btnReactivarTodos.UseVisualStyleBackColor = False
        '
        'btnCancelar
        '
        Me.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancelar.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.btnCancelar.Location = New System.Drawing.Point(182, 176)
        Me.btnCancelar.Name = "btnCancelar"
        Me.btnCancelar.Size = New System.Drawing.Size(75, 40)
        Me.btnCancelar.TabIndex = 5
        Me.btnCancelar.Text = "Cerrar"
        Me.btnCancelar.UseVisualStyleBackColor = True
        '
        'lblDescripcion
        '
        Me.lblDescripcion.AutoSize = True
        Me.lblDescripcion.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblDescripcion.ForeColor = System.Drawing.Color.Gray
        Me.lblDescripcion.Location = New System.Drawing.Point(12, 80)
        Me.lblDescripcion.Name = "lblDescripcion"
        Me.lblDescripcion.Size = New System.Drawing.Size(311, 30)
        Me.lblDescripcion.TabIndex = 2
        Me.lblDescripcion.Text = "Se han detectado productos inactivos en la base de datos." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Usa los botones de aba" &
    "jo para reactivarlos."
        '
        'FormReactivarProductos
        '
        Me.AcceptButton = Me.btnReactivar
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancelar
        Me.ClientSize = New System.Drawing.Size(442, 224)
        Me.Controls.Add(Me.btnCancelar)
        Me.Controls.Add(Me.btnReactivarTodos)
        Me.Controls.Add(Me.btnReactivar)
        Me.Controls.Add(Me.lblDescripcion)
        Me.Controls.Add(Me.lblEstado)
        Me.Controls.Add(Me.lblTitulo)
        Me.Name = "FormReactivarProductos"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Reactivar Productos"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitulo As Label
    Friend WithEvents lblEstado As Label
    Friend WithEvents btnReactivar As Button
    Friend WithEvents btnReactivarTodos As Button
    Friend WithEvents btnCancelar As Button
    Friend WithEvents lblDescripcion As Label

End Class

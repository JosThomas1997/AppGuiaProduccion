<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormResumenProduccion
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
        Me.cbEmpleado = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnEmpBuscar = New System.Windows.Forms.Button()
        Me.btnTodoProduccion = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'cbEmpleado
        '
        Me.cbEmpleado.FormattingEnabled = True
        Me.cbEmpleado.Location = New System.Drawing.Point(57, 54)
        Me.cbEmpleado.Name = "cbEmpleado"
        Me.cbEmpleado.Size = New System.Drawing.Size(158, 21)
        Me.cbEmpleado.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(76, 26)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(124, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Seleccione al Empleado:"
        '
        'btnEmpBuscar
        '
        Me.btnEmpBuscar.Location = New System.Drawing.Point(12, 93)
        Me.btnEmpBuscar.Name = "btnEmpBuscar"
        Me.btnEmpBuscar.Size = New System.Drawing.Size(121, 23)
        Me.btnEmpBuscar.TabIndex = 2
        Me.btnEmpBuscar.Text = "Buscar Empleado"
        Me.btnEmpBuscar.UseVisualStyleBackColor = True
        '
        'btnTodoProduccion
        '
        Me.btnTodoProduccion.Location = New System.Drawing.Point(139, 93)
        Me.btnTodoProduccion.Name = "btnTodoProduccion"
        Me.btnTodoProduccion.Size = New System.Drawing.Size(126, 23)
        Me.btnTodoProduccion.TabIndex = 3
        Me.btnTodoProduccion.Text = "Toda la Produccion"
        Me.btnTodoProduccion.UseVisualStyleBackColor = True
        '
        'FormResumenProduccion
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(277, 128)
        Me.Controls.Add(Me.btnTodoProduccion)
        Me.Controls.Add(Me.btnEmpBuscar)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cbEmpleado)
        Me.Name = "FormResumenProduccion"
        Me.Text = "Seleccionar Empleado"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cbEmpleado As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents btnEmpBuscar As Button
    Friend WithEvents btnTodoProduccion As Button
End Class

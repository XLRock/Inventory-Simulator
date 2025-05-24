<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CartForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.RemoveAllBtn = New System.Windows.Forms.Button()
        Me.confirmbtn = New System.Windows.Forms.Button()
        Me.ReturnBtn = New System.Windows.Forms.Button()
        Me.RemoveBtn = New System.Windows.Forms.Button()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.subTotallb = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'RemoveAllBtn
        '
        Me.RemoveAllBtn.Dock = System.Windows.Forms.DockStyle.Left
        Me.RemoveAllBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black
        Me.RemoveAllBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red
        Me.RemoveAllBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.RemoveAllBtn.ForeColor = System.Drawing.Color.White
        Me.RemoveAllBtn.Location = New System.Drawing.Point(154, 0)
        Me.RemoveAllBtn.Name = "RemoveAllBtn"
        Me.RemoveAllBtn.Size = New System.Drawing.Size(154, 41)
        Me.RemoveAllBtn.TabIndex = 7
        Me.RemoveAllBtn.Text = "Remove all items"
        Me.RemoveAllBtn.UseVisualStyleBackColor = True
        '
        'confirmbtn
        '
        Me.confirmbtn.BackColor = System.Drawing.Color.LightSlateGray
        Me.confirmbtn.Dock = System.Windows.Forms.DockStyle.Right
        Me.confirmbtn.FlatAppearance.BorderColor = System.Drawing.Color.Black
        Me.confirmbtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red
        Me.confirmbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.confirmbtn.ForeColor = System.Drawing.Color.White
        Me.confirmbtn.Location = New System.Drawing.Point(598, 0)
        Me.confirmbtn.Name = "confirmbtn"
        Me.confirmbtn.Size = New System.Drawing.Size(148, 41)
        Me.confirmbtn.TabIndex = 2
        Me.confirmbtn.Text = "Proceed To Checkout"
        Me.confirmbtn.UseVisualStyleBackColor = False
        '
        'ReturnBtn
        '
        Me.ReturnBtn.Dock = System.Windows.Forms.DockStyle.Left
        Me.ReturnBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black
        Me.ReturnBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red
        Me.ReturnBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ReturnBtn.ForeColor = System.Drawing.Color.White
        Me.ReturnBtn.Location = New System.Drawing.Point(308, 0)
        Me.ReturnBtn.Name = "ReturnBtn"
        Me.ReturnBtn.Size = New System.Drawing.Size(154, 41)
        Me.ReturnBtn.TabIndex = 6
        Me.ReturnBtn.Text = "Return to previous page"
        Me.ReturnBtn.UseVisualStyleBackColor = True
        '
        'RemoveBtn
        '
        Me.RemoveBtn.Dock = System.Windows.Forms.DockStyle.Left
        Me.RemoveBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black
        Me.RemoveBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red
        Me.RemoveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.RemoveBtn.ForeColor = System.Drawing.Color.White
        Me.RemoveBtn.Location = New System.Drawing.Point(0, 0)
        Me.RemoveBtn.Name = "RemoveBtn"
        Me.RemoveBtn.Size = New System.Drawing.Size(154, 41)
        Me.RemoveBtn.TabIndex = 1
        Me.RemoveBtn.Text = "Remove Item"
        Me.RemoveBtn.UseVisualStyleBackColor = True
        '
        'ListView1
        '
        Me.ListView1.FullRowSelect = True
        Me.ListView1.GridLines = True
        Me.ListView1.HideSelection = False
        Me.ListView1.Location = New System.Drawing.Point(52, 50)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(746, 210)
        Me.ListView1.TabIndex = 0
        Me.ListView1.UseCompatibleStateImageBehavior = False
        '
        'subTotallb
        '
        Me.subTotallb.AutoSize = True
        Me.subTotallb.ForeColor = System.Drawing.Color.White
        Me.subTotallb.Location = New System.Drawing.Point(468, 14)
        Me.subTotallb.Name = "subTotallb"
        Me.subTotallb.Size = New System.Drawing.Size(46, 13)
        Me.subTotallb.TabIndex = 9
        Me.subTotallb.Text = "Subtotal"
        Me.subTotallb.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.LightSlateGray
        Me.Panel1.Controls.Add(Me.ReturnBtn)
        Me.Panel1.Controls.Add(Me.subTotallb)
        Me.Panel1.Controls.Add(Me.confirmbtn)
        Me.Panel1.Controls.Add(Me.RemoveAllBtn)
        Me.Panel1.Controls.Add(Me.RemoveBtn)
        Me.Panel1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Panel1.Location = New System.Drawing.Point(52, 257)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(746, 41)
        Me.Panel1.TabIndex = 0
        '
        'CartForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.LightGray
        Me.ClientSize = New System.Drawing.Size(857, 356)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.ListView1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "CartForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Cart"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents RemoveAllBtn As Button
    Friend WithEvents confirmbtn As Button
    Friend WithEvents ReturnBtn As Button
    Friend WithEvents RemoveBtn As Button
    Friend WithEvents ListView1 As ListView
    Friend WithEvents subTotallb As Label
    Friend WithEvents Panel1 As Panel
End Class

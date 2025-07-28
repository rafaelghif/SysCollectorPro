<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SysCollectoPro
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SysCollectoPro))
        Me.LblSite = New System.Windows.Forms.Label()
        Me.TxtSite = New System.Windows.Forms.TextBox()
        Me.LblZone = New System.Windows.Forms.Label()
        Me.TxtZone = New System.Windows.Forms.TextBox()
        Me.BtnExecute = New System.Windows.Forms.Button()
        Me.TxtPosition = New System.Windows.Forms.TextBox()
        Me.LblPosition = New System.Windows.Forms.Label()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.LblCopyright = New System.Windows.Forms.ToolStripStatusLabel()
        Me.BgWorkerExport = New System.ComponentModel.BackgroundWorker()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'LblSite
        '
        Me.LblSite.AutoSize = True
        Me.LblSite.Location = New System.Drawing.Point(17, 16)
        Me.LblSite.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblSite.Name = "LblSite"
        Me.LblSite.Size = New System.Drawing.Size(30, 16)
        Me.LblSite.TabIndex = 0
        Me.LblSite.Text = "Site"
        '
        'TxtSite
        '
        Me.TxtSite.Location = New System.Drawing.Point(17, 37)
        Me.TxtSite.Margin = New System.Windows.Forms.Padding(4)
        Me.TxtSite.Name = "TxtSite"
        Me.TxtSite.Size = New System.Drawing.Size(301, 22)
        Me.TxtSite.TabIndex = 1
        '
        'LblZone
        '
        Me.LblZone.AutoSize = True
        Me.LblZone.Location = New System.Drawing.Point(17, 70)
        Me.LblZone.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblZone.Name = "LblZone"
        Me.LblZone.Size = New System.Drawing.Size(38, 16)
        Me.LblZone.TabIndex = 2
        Me.LblZone.Text = "Zone"
        '
        'TxtZone
        '
        Me.TxtZone.Location = New System.Drawing.Point(17, 90)
        Me.TxtZone.Margin = New System.Windows.Forms.Padding(4)
        Me.TxtZone.Name = "TxtZone"
        Me.TxtZone.Size = New System.Drawing.Size(301, 22)
        Me.TxtZone.TabIndex = 3
        '
        'BtnExecute
        '
        Me.BtnExecute.Location = New System.Drawing.Point(68, 175)
        Me.BtnExecute.Margin = New System.Windows.Forms.Padding(4)
        Me.BtnExecute.Name = "BtnExecute"
        Me.BtnExecute.Size = New System.Drawing.Size(189, 34)
        Me.BtnExecute.TabIndex = 4
        Me.BtnExecute.Text = "Execute"
        Me.BtnExecute.UseVisualStyleBackColor = True
        '
        'TxtPosition
        '
        Me.TxtPosition.Location = New System.Drawing.Point(20, 145)
        Me.TxtPosition.Margin = New System.Windows.Forms.Padding(4)
        Me.TxtPosition.Name = "TxtPosition"
        Me.TxtPosition.Size = New System.Drawing.Size(301, 22)
        Me.TxtPosition.TabIndex = 6
        '
        'LblPosition
        '
        Me.LblPosition.AutoSize = True
        Me.LblPosition.Location = New System.Drawing.Point(20, 125)
        Me.LblPosition.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblPosition.Name = "LblPosition"
        Me.LblPosition.Size = New System.Drawing.Size(55, 16)
        Me.LblPosition.TabIndex = 5
        Me.LblPosition.Text = "Position"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LblCopyright})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 225)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(341, 22)
        Me.StatusStrip1.TabIndex = 7
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'LblCopyright
        '
        Me.LblCopyright.Name = "LblCopyright"
        Me.LblCopyright.Size = New System.Drawing.Size(90, 17)
        Me.LblCopyright.Text = "© 2025 Flontive"
        '
        'SysCollectoPro
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(341, 247)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.TxtPosition)
        Me.Controls.Add(Me.LblPosition)
        Me.Controls.Add(Me.BtnExecute)
        Me.Controls.Add(Me.TxtZone)
        Me.Controls.Add(Me.LblZone)
        Me.Controls.Add(Me.TxtSite)
        Me.Controls.Add(Me.LblSite)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "SysCollectoPro"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SysCollectorPro"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LblSite As Label
    Friend WithEvents TxtSite As TextBox
    Friend WithEvents LblZone As Label
    Friend WithEvents TxtZone As TextBox
    Friend WithEvents BtnExecute As Button
    Friend WithEvents TxtPosition As TextBox
    Friend WithEvents LblPosition As Label
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents LblCopyright As ToolStripStatusLabel
    Friend WithEvents BgWorkerExport As System.ComponentModel.BackgroundWorker
End Class

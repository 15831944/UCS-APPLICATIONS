<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form_DPI
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
        Me.components = New System.ComponentModel.Container()
        Me.OpenCSV = New System.Windows.Forms.OpenFileDialog()
        Me.btnCancelImport = New System.Windows.Forms.Button()
        Me.tbPg_InsertPoints = New System.Windows.Forms.TabPage()
        Me.lblPassedLayers = New System.Windows.Forms.Label()
        Me.lblEntPassed = New System.Windows.Forms.Label()
        Me.btn_ProcessData = New System.Windows.Forms.Button()
        Me.tbPg_Layer = New System.Windows.Forms.TabPage()
        Me.lbl_ValidatingLayers = New System.Windows.Forms.Label()
        Me.btn_AddMissingLayers = New System.Windows.Forms.Button()
        Me.lstBox_Layers = New System.Windows.Forms.ListBox()
        Me.tbPg_CheckBlockOrStyle = New System.Windows.Forms.TabPage()
        Me.lbl_ValidatingEntities = New System.Windows.Forms.Label()
        Me.btn_AddMissingEntities = New System.Windows.Forms.Button()
        Me.lstBox_Entities = New System.Windows.Forms.ListBox()
        Me.tbPg_CSV = New System.Windows.Forms.TabPage()
        Me.btn_ChangeCSV = New System.Windows.Forms.Button()
        Me.txtBox_TargetDrawingCSV = New System.Windows.Forms.TextBox()
        Me.txtBox_CSVFileName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lbl_CSVFileName = New System.Windows.Forms.Label()
        Me.btn_GetCSV = New System.Windows.Forms.Button()
        Me.tbPg_Setup = New System.Windows.Forms.TabPage()
        Me.txtBox_CurrentGCS = New System.Windows.Forms.TextBox()
        Me.txtBox_TargetDrawing = New System.Windows.Forms.TextBox()
        Me.lbl_CurrentGCS = New System.Windows.Forms.Label()
        Me.lbl_SourceGCS = New System.Windows.Forms.Label()
        Me.lbl_TargetDrawing = New System.Windows.Forms.Label()
        Me.cmbBox_SourceGCS = New System.Windows.Forms.ComboBox()
        Me.tabCtrMain = New System.Windows.Forms.TabControl()
        Me.DPIToolTips = New System.Windows.Forms.ToolTip(Me.components)
        Me.tbPg_InsertPoints.SuspendLayout()
        Me.tbPg_Layer.SuspendLayout()
        Me.tbPg_CheckBlockOrStyle.SuspendLayout()
        Me.tbPg_CSV.SuspendLayout()
        Me.tbPg_Setup.SuspendLayout()
        Me.tabCtrMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnCancelImport
        '
        Me.btnCancelImport.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancelImport.Location = New System.Drawing.Point(9, 221)
        Me.btnCancelImport.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnCancelImport.Name = "btnCancelImport"
        Me.btnCancelImport.Size = New System.Drawing.Size(446, 53)
        Me.btnCancelImport.TabIndex = 9
        Me.btnCancelImport.Text = "Pause to run Design Center (DC) to Add Missing" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Layer/Block/Style then type ""DPI""" & _
    " again"
        Me.btnCancelImport.UseVisualStyleBackColor = True
        '
        'tbPg_InsertPoints
        '
        Me.tbPg_InsertPoints.Controls.Add(Me.lblPassedLayers)
        Me.tbPg_InsertPoints.Controls.Add(Me.lblEntPassed)
        Me.tbPg_InsertPoints.Controls.Add(Me.btn_ProcessData)
        Me.tbPg_InsertPoints.Location = New System.Drawing.Point(4, 22)
        Me.tbPg_InsertPoints.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.tbPg_InsertPoints.Name = "tbPg_InsertPoints"
        Me.tbPg_InsertPoints.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.tbPg_InsertPoints.Size = New System.Drawing.Size(440, 176)
        Me.tbPg_InsertPoints.TabIndex = 5
        Me.tbPg_InsertPoints.Text = "Insert Points"
        Me.tbPg_InsertPoints.UseVisualStyleBackColor = True
        '
        'lblPassedLayers
        '
        Me.lblPassedLayers.AutoSize = True
        Me.lblPassedLayers.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPassedLayers.ForeColor = System.Drawing.Color.Black
        Me.lblPassedLayers.Location = New System.Drawing.Point(11, 43)
        Me.lblPassedLayers.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblPassedLayers.Name = "lblPassedLayers"
        Me.lblPassedLayers.Size = New System.Drawing.Size(97, 15)
        Me.lblPassedLayers.TabIndex = 9
        Me.lblPassedLayers.Text = "Layer(s) Check..."
        Me.lblPassedLayers.Visible = False
        '
        'lblEntPassed
        '
        Me.lblEntPassed.AutoSize = True
        Me.lblEntPassed.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEntPassed.ForeColor = System.Drawing.Color.Black
        Me.lblEntPassed.Location = New System.Drawing.Point(11, 15)
        Me.lblEntPassed.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblEntPassed.Name = "lblEntPassed"
        Me.lblEntPassed.Size = New System.Drawing.Size(222, 15)
        Me.lblEntPassed.TabIndex = 8
        Me.lblEntPassed.Text = "Block Ref(S) or MTEXT Style(s) Check..."
        Me.lblEntPassed.Visible = False
        '
        'btn_ProcessData
        '
        Me.btn_ProcessData.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_ProcessData.Location = New System.Drawing.Point(79, 108)
        Me.btn_ProcessData.Name = "btn_ProcessData"
        Me.btn_ProcessData.Size = New System.Drawing.Size(270, 55)
        Me.btn_ProcessData.TabIndex = 7
        Me.btn_ProcessData.Text = "Add Data to Drawing"
        Me.btn_ProcessData.UseVisualStyleBackColor = True
        Me.btn_ProcessData.Visible = False
        '
        'tbPg_Layer
        '
        Me.tbPg_Layer.Controls.Add(Me.lbl_ValidatingLayers)
        Me.tbPg_Layer.Controls.Add(Me.btn_AddMissingLayers)
        Me.tbPg_Layer.Controls.Add(Me.lstBox_Layers)
        Me.tbPg_Layer.Location = New System.Drawing.Point(4, 22)
        Me.tbPg_Layer.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.tbPg_Layer.Name = "tbPg_Layer"
        Me.tbPg_Layer.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.tbPg_Layer.Size = New System.Drawing.Size(440, 176)
        Me.tbPg_Layer.TabIndex = 4
        Me.tbPg_Layer.Text = "Check Layer(s)"
        Me.tbPg_Layer.UseVisualStyleBackColor = True
        '
        'lbl_ValidatingLayers
        '
        Me.lbl_ValidatingLayers.AutoSize = True
        Me.lbl_ValidatingLayers.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbl_ValidatingLayers.Location = New System.Drawing.Point(8, 129)
        Me.lbl_ValidatingLayers.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl_ValidatingLayers.Name = "lbl_ValidatingLayers"
        Me.lbl_ValidatingLayers.Size = New System.Drawing.Size(98, 13)
        Me.lbl_ValidatingLayers.TabIndex = 6
        Me.lbl_ValidatingLayers.Text = "Block Reference(s)"
        Me.lbl_ValidatingLayers.Visible = False
        '
        'btn_AddMissingLayers
        '
        Me.btn_AddMissingLayers.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_AddMissingLayers.Location = New System.Drawing.Point(366, 134)
        Me.btn_AddMissingLayers.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btn_AddMissingLayers.Name = "btn_AddMissingLayers"
        Me.btn_AddMissingLayers.Size = New System.Drawing.Size(65, 30)
        Me.btn_AddMissingLayers.TabIndex = 5
        Me.btn_AddMissingLayers.Text = "Refresh"
        Me.btn_AddMissingLayers.UseVisualStyleBackColor = True
        Me.btn_AddMissingLayers.Visible = False
        '
        'lstBox_Layers
        '
        Me.lstBox_Layers.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstBox_Layers.FormattingEnabled = True
        Me.lstBox_Layers.ItemHeight = 15
        Me.lstBox_Layers.Location = New System.Drawing.Point(5, 11)
        Me.lstBox_Layers.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.lstBox_Layers.Name = "lstBox_Layers"
        Me.lstBox_Layers.Size = New System.Drawing.Size(432, 109)
        Me.lstBox_Layers.TabIndex = 4
        '
        'tbPg_CheckBlockOrStyle
        '
        Me.tbPg_CheckBlockOrStyle.Controls.Add(Me.lbl_ValidatingEntities)
        Me.tbPg_CheckBlockOrStyle.Controls.Add(Me.btn_AddMissingEntities)
        Me.tbPg_CheckBlockOrStyle.Controls.Add(Me.lstBox_Entities)
        Me.tbPg_CheckBlockOrStyle.Location = New System.Drawing.Point(4, 22)
        Me.tbPg_CheckBlockOrStyle.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.tbPg_CheckBlockOrStyle.Name = "tbPg_CheckBlockOrStyle"
        Me.tbPg_CheckBlockOrStyle.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.tbPg_CheckBlockOrStyle.Size = New System.Drawing.Size(440, 176)
        Me.tbPg_CheckBlockOrStyle.TabIndex = 2
        Me.tbPg_CheckBlockOrStyle.Text = "Check Block(s) or Style(s)"
        Me.tbPg_CheckBlockOrStyle.UseVisualStyleBackColor = True
        '
        'lbl_ValidatingEntities
        '
        Me.lbl_ValidatingEntities.AutoSize = True
        Me.lbl_ValidatingEntities.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbl_ValidatingEntities.Location = New System.Drawing.Point(8, 129)
        Me.lbl_ValidatingEntities.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl_ValidatingEntities.Name = "lbl_ValidatingEntities"
        Me.lbl_ValidatingEntities.Size = New System.Drawing.Size(98, 13)
        Me.lbl_ValidatingEntities.TabIndex = 6
        Me.lbl_ValidatingEntities.Text = "Block Reference(s)"
        Me.lbl_ValidatingEntities.Visible = False
        '
        'btn_AddMissingEntities
        '
        Me.btn_AddMissingEntities.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_AddMissingEntities.Location = New System.Drawing.Point(366, 134)
        Me.btn_AddMissingEntities.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btn_AddMissingEntities.Name = "btn_AddMissingEntities"
        Me.btn_AddMissingEntities.Size = New System.Drawing.Size(65, 30)
        Me.btn_AddMissingEntities.TabIndex = 5
        Me.btn_AddMissingEntities.Text = "Refresh"
        Me.btn_AddMissingEntities.UseVisualStyleBackColor = True
        Me.btn_AddMissingEntities.Visible = False
        '
        'lstBox_Entities
        '
        Me.lstBox_Entities.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstBox_Entities.FormattingEnabled = True
        Me.lstBox_Entities.ItemHeight = 15
        Me.lstBox_Entities.Location = New System.Drawing.Point(6, 12)
        Me.lstBox_Entities.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.lstBox_Entities.Name = "lstBox_Entities"
        Me.lstBox_Entities.Size = New System.Drawing.Size(432, 109)
        Me.lstBox_Entities.TabIndex = 4
        '
        'tbPg_CSV
        '
        Me.tbPg_CSV.Controls.Add(Me.btn_ChangeCSV)
        Me.tbPg_CSV.Controls.Add(Me.txtBox_TargetDrawingCSV)
        Me.tbPg_CSV.Controls.Add(Me.txtBox_CSVFileName)
        Me.tbPg_CSV.Controls.Add(Me.Label1)
        Me.tbPg_CSV.Controls.Add(Me.lbl_CSVFileName)
        Me.tbPg_CSV.Controls.Add(Me.btn_GetCSV)
        Me.tbPg_CSV.Location = New System.Drawing.Point(4, 22)
        Me.tbPg_CSV.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.tbPg_CSV.Name = "tbPg_CSV"
        Me.tbPg_CSV.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.tbPg_CSV.Size = New System.Drawing.Size(440, 176)
        Me.tbPg_CSV.TabIndex = 1
        Me.tbPg_CSV.Text = "Get Data from CSV"
        Me.tbPg_CSV.UseVisualStyleBackColor = True
        '
        'btn_ChangeCSV
        '
        Me.btn_ChangeCSV.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_ChangeCSV.Location = New System.Drawing.Point(6, 97)
        Me.btn_ChangeCSV.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btn_ChangeCSV.Name = "btn_ChangeCSV"
        Me.btn_ChangeCSV.Size = New System.Drawing.Size(432, 24)
        Me.btn_ChangeCSV.TabIndex = 9
        Me.btn_ChangeCSV.Text = "Change Data File (*.csv) to use"
        Me.btn_ChangeCSV.UseVisualStyleBackColor = True
        '
        'txtBox_TargetDrawingCSV
        '
        Me.txtBox_TargetDrawingCSV.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBox_TargetDrawingCSV.Location = New System.Drawing.Point(6, 149)
        Me.txtBox_TargetDrawingCSV.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtBox_TargetDrawingCSV.Name = "txtBox_TargetDrawingCSV"
        Me.txtBox_TargetDrawingCSV.ReadOnly = True
        Me.txtBox_TargetDrawingCSV.Size = New System.Drawing.Size(433, 23)
        Me.txtBox_TargetDrawingCSV.TabIndex = 7
        '
        'txtBox_CSVFileName
        '
        Me.txtBox_CSVFileName.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBox_CSVFileName.Location = New System.Drawing.Point(6, 61)
        Me.txtBox_CSVFileName.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtBox_CSVFileName.Name = "txtBox_CSVFileName"
        Me.txtBox_CSVFileName.ReadOnly = True
        Me.txtBox_CSVFileName.Size = New System.Drawing.Size(433, 23)
        Me.txtBox_CSVFileName.TabIndex = 4
        Me.txtBox_CSVFileName.Text = "CSV to use"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(4, 130)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(139, 13)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Drawing to Import Data into:"
        '
        'lbl_CSVFileName
        '
        Me.lbl_CSVFileName.AutoSize = True
        Me.lbl_CSVFileName.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbl_CSVFileName.Location = New System.Drawing.Point(4, 42)
        Me.lbl_CSVFileName.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl_CSVFileName.Name = "lbl_CSVFileName"
        Me.lbl_CSVFileName.Size = New System.Drawing.Size(131, 13)
        Me.lbl_CSVFileName.TabIndex = 5
        Me.lbl_CSVFileName.Text = "Name of CSV File To Use:"
        '
        'btn_GetCSV
        '
        Me.btn_GetCSV.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_GetCSV.Location = New System.Drawing.Point(6, 10)
        Me.btn_GetCSV.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btn_GetCSV.Name = "btn_GetCSV"
        Me.btn_GetCSV.Size = New System.Drawing.Size(432, 26)
        Me.btn_GetCSV.TabIndex = 3
        Me.btn_GetCSV.Text = "Select Data File (*.csv) to use"
        Me.btn_GetCSV.UseVisualStyleBackColor = True
        '
        'tbPg_Setup
        '
        Me.tbPg_Setup.Controls.Add(Me.txtBox_CurrentGCS)
        Me.tbPg_Setup.Controls.Add(Me.txtBox_TargetDrawing)
        Me.tbPg_Setup.Controls.Add(Me.lbl_CurrentGCS)
        Me.tbPg_Setup.Controls.Add(Me.lbl_SourceGCS)
        Me.tbPg_Setup.Controls.Add(Me.lbl_TargetDrawing)
        Me.tbPg_Setup.Controls.Add(Me.cmbBox_SourceGCS)
        Me.tbPg_Setup.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbPg_Setup.Location = New System.Drawing.Point(4, 22)
        Me.tbPg_Setup.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.tbPg_Setup.Name = "tbPg_Setup"
        Me.tbPg_Setup.Padding = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.tbPg_Setup.Size = New System.Drawing.Size(440, 176)
        Me.tbPg_Setup.TabIndex = 0
        Me.tbPg_Setup.Text = "Setup"
        Me.tbPg_Setup.UseVisualStyleBackColor = True
        '
        'txtBox_CurrentGCS
        '
        Me.txtBox_CurrentGCS.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBox_CurrentGCS.Location = New System.Drawing.Point(274, 17)
        Me.txtBox_CurrentGCS.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtBox_CurrentGCS.Name = "txtBox_CurrentGCS"
        Me.txtBox_CurrentGCS.ReadOnly = True
        Me.txtBox_CurrentGCS.Size = New System.Drawing.Size(73, 23)
        Me.txtBox_CurrentGCS.TabIndex = 0
        '
        'txtBox_TargetDrawing
        '
        Me.txtBox_TargetDrawing.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBox_TargetDrawing.Location = New System.Drawing.Point(6, 136)
        Me.txtBox_TargetDrawing.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtBox_TargetDrawing.Name = "txtBox_TargetDrawing"
        Me.txtBox_TargetDrawing.ReadOnly = True
        Me.txtBox_TargetDrawing.Size = New System.Drawing.Size(432, 23)
        Me.txtBox_TargetDrawing.TabIndex = 2
        '
        'lbl_CurrentGCS
        '
        Me.lbl_CurrentGCS.AutoSize = True
        Me.lbl_CurrentGCS.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbl_CurrentGCS.Location = New System.Drawing.Point(11, 19)
        Me.lbl_CurrentGCS.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl_CurrentGCS.Name = "lbl_CurrentGCS"
        Me.lbl_CurrentGCS.Size = New System.Drawing.Size(251, 15)
        Me.lbl_CurrentGCS.TabIndex = 3
        Me.lbl_CurrentGCS.Text = "Drawing's current Global Coordinate System:"
        '
        'lbl_SourceGCS
        '
        Me.lbl_SourceGCS.AutoSize = True
        Me.lbl_SourceGCS.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbl_SourceGCS.Location = New System.Drawing.Point(11, 54)
        Me.lbl_SourceGCS.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl_SourceGCS.Name = "lbl_SourceGCS"
        Me.lbl_SourceGCS.Size = New System.Drawing.Size(232, 15)
        Me.lbl_SourceGCS.TabIndex = 4
        Me.lbl_SourceGCS.Text = "Source Data's Global Coordinate System:"
        Me.lbl_SourceGCS.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lbl_TargetDrawing
        '
        Me.lbl_TargetDrawing.AutoSize = True
        Me.lbl_TargetDrawing.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbl_TargetDrawing.Location = New System.Drawing.Point(4, 114)
        Me.lbl_TargetDrawing.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbl_TargetDrawing.Name = "lbl_TargetDrawing"
        Me.lbl_TargetDrawing.Size = New System.Drawing.Size(159, 15)
        Me.lbl_TargetDrawing.TabIndex = 6
        Me.lbl_TargetDrawing.Text = "Drawing to Import Data into:"
        '
        'cmbBox_SourceGCS
        '
        Me.cmbBox_SourceGCS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBox_SourceGCS.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbBox_SourceGCS.FormattingEnabled = True
        Me.cmbBox_SourceGCS.Items.AddRange(New Object() {"LL83", "WGS84", "LL27"})
        Me.cmbBox_SourceGCS.Location = New System.Drawing.Point(274, 50)
        Me.cmbBox_SourceGCS.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.cmbBox_SourceGCS.MaxDropDownItems = 3
        Me.cmbBox_SourceGCS.Name = "cmbBox_SourceGCS"
        Me.cmbBox_SourceGCS.Size = New System.Drawing.Size(73, 25)
        Me.cmbBox_SourceGCS.TabIndex = 1
        '
        'tabCtrMain
        '
        Me.tabCtrMain.Controls.Add(Me.tbPg_Setup)
        Me.tabCtrMain.Controls.Add(Me.tbPg_CSV)
        Me.tabCtrMain.Controls.Add(Me.tbPg_CheckBlockOrStyle)
        Me.tabCtrMain.Controls.Add(Me.tbPg_Layer)
        Me.tabCtrMain.Controls.Add(Me.tbPg_InsertPoints)
        Me.tabCtrMain.Location = New System.Drawing.Point(9, 10)
        Me.tabCtrMain.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.tabCtrMain.Name = "tabCtrMain"
        Me.tabCtrMain.SelectedIndex = 0
        Me.tabCtrMain.Size = New System.Drawing.Size(448, 202)
        Me.tabCtrMain.TabIndex = 8
        '
        'Form_DPI
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(474, 303)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnCancelImport)
        Me.Controls.Add(Me.tabCtrMain)
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(482, 330)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(482, 330)
        Me.Name = "Form_DPI"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "AutoCAD Map 3D Point Data Import"
        Me.tbPg_InsertPoints.ResumeLayout(False)
        Me.tbPg_InsertPoints.PerformLayout()
        Me.tbPg_Layer.ResumeLayout(False)
        Me.tbPg_Layer.PerformLayout()
        Me.tbPg_CheckBlockOrStyle.ResumeLayout(False)
        Me.tbPg_CheckBlockOrStyle.PerformLayout()
        Me.tbPg_CSV.ResumeLayout(False)
        Me.tbPg_CSV.PerformLayout()
        Me.tbPg_Setup.ResumeLayout(False)
        Me.tbPg_Setup.PerformLayout()
        Me.tabCtrMain.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents OpenCSV As Windows.Forms.OpenFileDialog
    Friend WithEvents btnCancelImport As Windows.Forms.Button
    Friend WithEvents tbPg_InsertPoints As Windows.Forms.TabPage
    Friend WithEvents lblPassedLayers As Windows.Forms.Label
    Friend WithEvents lblEntPassed As Windows.Forms.Label
    Friend WithEvents btn_ProcessData As Windows.Forms.Button
    Friend WithEvents tbPg_Layer As Windows.Forms.TabPage
    Friend WithEvents lbl_ValidatingLayers As Windows.Forms.Label
    Friend WithEvents btn_AddMissingLayers As Windows.Forms.Button
    Friend WithEvents lstBox_Layers As Windows.Forms.ListBox
    Friend WithEvents tbPg_CheckBlockOrStyle As Windows.Forms.TabPage
    Friend WithEvents lbl_ValidatingEntities As Windows.Forms.Label
    Friend WithEvents btn_AddMissingEntities As Windows.Forms.Button
    Friend WithEvents lstBox_Entities As Windows.Forms.ListBox
    Friend WithEvents tbPg_CSV As Windows.Forms.TabPage
    Friend WithEvents btn_ChangeCSV As Windows.Forms.Button
    Friend WithEvents txtBox_TargetDrawingCSV As Windows.Forms.TextBox
    Friend WithEvents txtBox_CSVFileName As Windows.Forms.TextBox
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents lbl_CSVFileName As Windows.Forms.Label
    Friend WithEvents btn_GetCSV As Windows.Forms.Button
    Friend WithEvents tbPg_Setup As Windows.Forms.TabPage
    Friend WithEvents txtBox_CurrentGCS As Windows.Forms.TextBox
    Friend WithEvents txtBox_TargetDrawing As Windows.Forms.TextBox
    Friend WithEvents lbl_CurrentGCS As Windows.Forms.Label
    Friend WithEvents lbl_SourceGCS As Windows.Forms.Label
    Friend WithEvents lbl_TargetDrawing As Windows.Forms.Label
    Friend WithEvents cmbBox_SourceGCS As Windows.Forms.ComboBox
    Friend WithEvents tabCtrMain As Windows.Forms.TabControl
    Friend WithEvents DPIToolTips As Windows.Forms.ToolTip
End Class

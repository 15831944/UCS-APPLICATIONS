Imports System
Imports System.Data.Linq
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.EditorInput

Imports Autodesk.Gis.Map
Imports Autodesk.Gis.Map.ObjectData
Imports Autodesk.Gis.Map.Constants
Imports Autodesk.Gis.Map.Utilities

Public Class Form_DPI

    ' Load TextBoxes with current Source and Destination GSC plus verify a Drawing is open (though using a Modal Autodesk Dialog
    ' should prevent any command line calls without a drawing open
    Private Sub Form_DPI_OnLoad(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        ' Set LL83 as the default Source Global Coordinate System (99% of our Survey and GPS data comes in this format)
        cmbBox_SourceGCS.SelectedItem = "LL83"
        txtBox_CurrentGCS.Text = WinForm_DPI.MyCommands.destinationGCS

        ' Ask AutoCAD for the name of the active/current drawing
        Dim cDWG As String = Application.GetSystemVariable("DWGNAME")
        ' Avoid memory fault/crash by attempting to reference void data/drawing
        If cDWG <> "" Then
            txtBox_TargetDrawing.Text = cDWG
            txtBox_TargetDrawingCSV.Text = cDWG
        Else
            txtBox_TargetDrawing.Text = "Please click the REFRESH button below"
        End If
        DPIToolTips.SetToolTip(btnCancelImport, "Pause Importer so you can do things such as add missing Layers " & vbCrLf &
                "with Design Center or to close the Importer to work with the new Points you have added to your drawing. " & vbCrLf &
                "(Note that DPI remains running in the background until you exit or restart AutoCAD.")

    End Sub

    ' Allow the User to exit the Modal dialog box in case they made a mistake
    Private Sub btnCancelImport_Click(sender As Object, e As EventArgs) Handles btnCancelImport.Click
        Me.Visible = False
    End Sub

    ' Duplicate of Open a CSV for those Users not sure about what is or is not loaded for Data
    Private Sub btn_ChangeCSV_Click(sender As Object, e As EventArgs) Handles btn_ChangeCSV.Click
        ClearOutOldData()
        Me.tabCtrMain.SelectedTab = tbPg_CSV
        btn_GetCSV_Click(sender, e)
    End Sub

    ' Locate desired data source (CSV file) and load it into an in-memory Windows DataTable
    Private Sub btn_GetCSV_Click(sender As System.Object, e As System.EventArgs) Handles btn_GetCSV.Click
        ' Verify that User has not cleared the default source GCS of "LL83"
        If cmbBox_SourceGCS.Text = "" Then
            MsgBox("Please select a " & ControlChars.Quote & "Source Data's Global Coordinate System " & ControlChars.Quote & "from the dropdown list above.")
            Exit Sub
        End If

        ' Clear out any previous DataTable
        If WinForm_DPI.MyCommands.dt IsNot Nothing Then
            WinForm_DPI.MyCommands.dt = Nothing
        End If

        ' Since most projects are on \\manv-file01 (which is mapped on login to M:), open FileDialogBox with...
        OpenCSV.InitialDirectory = "M:\"
        ' The Excel file generates a | delimited CSV file with "Points-" in the title, hence:
        OpenCSV.Title = "Select the " & ControlChars.Quote & "Points-* " & ControlChars.Quote & "file to open"
        OpenCSV.Filter = "CSV Files (*.csv)|*.csv"

        ' Display the Open File Dialog window
        If OpenCSV.ShowDialog = Windows.Forms.DialogResult.OK Then
            ' Strip off Path information and place the file name in:
            txtBox_CSVFileName.Text = OpenCSV.SafeFileName
            ' Create the Shared DataTable (pass ByRef to external Modules/Classes)
            WinForm_DPI.MyCommands.dt = New System.Data.DataTable("Current-Data")
            ' Load the new DataTable from the | delimited CSV file
            WinForm_DPI.MyCommands.dt = Util_CSV.CreateDataTableFromCSV(OpenCSV.FileName)

            '---- Validations ------
            If WinForm_DPI.MyCommands.isBlockRef Then
                tbPg_CheckBlockOrStyle.Text = "Check Block Ref(s)"
                ' Check to make sure the drawing has the desired Block Reference(s) in its BlockTable
                CheckForBlockRefs(WinForm_DPI.MyCommands.dt)
            Else
                tbPg_CheckBlockOrStyle.Text = "Check MTEXT Style(s)"
                ' Check to make sure all MTEXT Style(s) are in the DWG
                CheckForMTEXTStyle(WinForm_DPI.MyCommands.dt)
            End If
            ' Check to make sure the drawing has the desired Layer(s) in its LayerTable
            CheckForLayers(WinForm_DPI.MyCommands.dt)
        End If
    End Sub

    ' Check to make sure the drawing has the desired Block Reference(s) in its BlockTable
    Public Sub CheckForBlockRefs(ByRef dt As System.Data.DataTable)

        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = doc.Editor
        Dim db As Database = doc.Database

        ' Find all the Block Reference(s) in the DataTable using LINQ-to-DataSet
        Dim query = (From row In dt Select row.Field(Of String)("BLOCK_NAME")).Distinct

        Try
            Using trans As Transaction = db.TransactionManager.StartTransaction()
                Dim bt As BlockTable = DirectCast(trans.GetObject(db.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)

                ' Check the results of our query against the Block Table
                For Each blockName In query
                    ' The LINQ-to-Dataset query seems to return both the Column Name and an empty end-of-table object so
                    If blockName <> "BLOCK_NAME" And blockName <> "" Then
                        ' If it exists, add the Block Name to our ListBox of Block Refs and...
                        If bt.Has(blockName) Then
                            lstBox_Entities.Items.Add("Found: " & blockName)
                            ' ...flag that we found one of # Block Refs in our dt (DataTable)
                            WinForm_DPI.MyCommands.foundAllBlockRefs = True
                        Else ' Not found:
                            lstBox_Entities.Items.Add("Please add: " & blockName)
                            ed.WriteMessage("Please add the missing Block named: " & blockName & vbCrLf)
                            WinForm_DPI.MyCommands.foundAllBlockRefs = False
                        End If
                    End If
                Next
                trans.Commit()
            End Using
        Catch ex As Autodesk.AutoCAD.Runtime.Exception
            MsgBox("Error occured in: Form_DPI.CheckForBlockRefs(). Exception: " & ex.ToString())
        End Try

        ' Examine our flag and...
        If WinForm_DPI.MyCommands.foundAllBlockRefs Then
            With lblEntPassed
                .Visible = True
                .ForeColor = Drawing.Color.DarkGreen
                .Text = "Passed 'Check for Block Reference(s)'"
            End With
            lbl_ValidatingEntities.Text = "All Block Reference(s) " & vbCrLf & "found in DWG"
            lbl_ValidatingEntities.Visible = True
            btn_AddMissingEntities.Visible = False
        Else
            lbl_ValidatingEntities.Text = "Please click " & ControlChars.Quote & "Refresh" & ControlChars.Quote & " after " & vbCrLf & "adding the missing Block(s):"
            lbl_ValidatingEntities.Visible = True
            btn_AddMissingEntities.Visible = True
            With lblEntPassed
                .Visible = True
                .ForeColor = Drawing.Color.Red
                .Text = "FAILED - Please see the 'Check Block Reference(s)' tab"
            End With
        End If
        Me.tabCtrMain.SelectedTab = tbPg_InsertPoints
    End Sub

    ' Make sure that the User added the missing Block Reference(s) or MTEXT Styles
    Private Sub btn_AddMissingEntities_Click(sender As Object, e As EventArgs) Handles btn_AddMissingEntities.Click
        '---- Validations ------
        ' Reset our flags and notify User we are...
        lbl_ValidatingEntities.Visible = False
        btn_AddMissingEntities.Visible = False
        If WinForm_DPI.MyCommands.isBlockRef Then
            lbl_ValidatingEntities.Text = "Checking Block Reference(s)..."
            lbl_ValidatingEntities.Visible = True
            lstBox_Entities.Items.Clear()
            ' ...making sure they added the missing Block Reference(s).
            CheckForBlockRefs(WinForm_DPI.MyCommands.dt)
        Else
            lbl_ValidatingEntities.Text = "Checking MTEXT Style(s)..."
            lbl_ValidatingEntities.Visible = True
            lstBox_Entities.Items.Clear()
            ' ...making sure they added the missing Block Reference(s).
            CheckForMTEXTStyle(WinForm_DPI.MyCommands.dt)
        End If
    End Sub

    ' Check to make sure the drawing has the desired Layer(s) in its LayerTable
    Public Sub CheckForLayers(ByRef dt As System.Data.DataTable)

        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = doc.Editor
        Dim db As Database = doc.Database

        ' Find all the Layers(s) in the DataTable using LINQ-to-DataSet
        Dim query = (From row In dt Select row.Field(Of String)("LAYER_NAME")).Distinct

        Try
            Using trans As Transaction = db.TransactionManager.StartTransaction()
                ' Open the Layer Table of the drawing's database
                Dim pLayerTable As LayerTable = DirectCast(trans.GetObject(db.LayerTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), LayerTable)

                ' Check the results of our query against the Layer Table
                For Each layerName In query
                    ' The LINQ-to-Dataset query seems to return both the Column Name and an empty end-of-table object so
                    If layerName <> "LAYER_NAME" And layerName <> "" Then
                        ' If it exists, add the Layer Name to our ListBox of Layer(s) and...
                        If pLayerTable.Has(layerName) Then
                            lstBox_Layers.Items.Add("Found: " & layerName)
                            ' ...flag that we found one of # Layer(s) in our dt (DataTable)
                            WinForm_DPI.MyCommands.foundAllLayers = True
                        Else ' Not found:
                            lstBox_Layers.Items.Add("Please add: " & layerName)
                            ed.WriteMessage("Please add the missing Layer named: " & layerName & vbCrLf)
                            WinForm_DPI.MyCommands.foundAllLayers = False
                        End If
                    End If
                Next
                ' Close our transaction even though we didn't change anything
                trans.Commit()
            End Using
        Catch ex As Autodesk.AutoCAD.Runtime.Exception
            MsgBox("Error occured in: Form_DPI.CheckForLayers(). Exception: " & ex.ToString())
        End Try

        ' Examine our flag and...
        If WinForm_DPI.MyCommands.foundAllLayers Then
            lbl_ValidatingLayers.Text = "All Layer(s) " & vbCrLf & "found in DWG"
            lbl_ValidatingLayers.Visible = True
            btn_AddMissingLayers.Visible = False
            With lblPassedLayers
                .Visible = True
                .ForeColor = Drawing.Color.DarkGreen
                .Text = "Passed 'Check for Layer(s)'"
            End With
            btn_ProcessData.Visible = True
        Else
            lbl_ValidatingLayers.Text = "Please click " & ControlChars.Quote & "Refresh" & ControlChars.Quote & " after " & vbCrLf & "adding the missing Layer(s):"
            lbl_ValidatingLayers.Visible = True
            btn_AddMissingLayers.Visible = True
            With lblPassedLayers
                .Visible = True
                .ForeColor = Drawing.Color.Red
                .Text = "FAILED - Please see the 'Check Layer(s)' tab'"
            End With
        End If
        Me.tabCtrMain.SelectedTab = tbPg_InsertPoints
    End Sub

    ' Make sure that the User added the missing Layer(s)
    Private Sub btn_AddMissingLayers_Click(sender As System.Object, e As System.EventArgs) Handles btn_AddMissingLayers.Click
        ' Reset our flags and notify User we are...
        lbl_ValidatingLayers.Visible = False
        btn_AddMissingLayers.Visible = False
        lbl_ValidatingLayers.Text = "Checking Block Reference(s)..."
        lbl_ValidatingLayers.Visible = True
        lstBox_Layers.Items.Clear()
        ' ...making sure they added the missing Layer(s).
        CheckForLayers(WinForm_DPI.MyCommands.dt)
    End Sub

    ' MTEXT Style(s) Processing
    Public Sub CheckForMTEXTStyle(ByRef dt As System.Data.DataTable)

        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = doc.Editor
        Dim db As Database = doc.Database

        ' Find all the Layers(s) in the DataTable using LINQ-to-DataSet
        Dim query = (From row In dt Select row.Field(Of String)("STYLE")).Distinct

        Try
            Using trans As Transaction = db.TransactionManager.StartTransaction()
                Dim textStyleTable As TextStyleTable = trans.GetObject(db.TextStyleTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
                For Each styleName In query
                    If textStyleTable.Has(styleName) Then
                        lstBox_Entities.Items.Add("Found: " & styleName)
                        WinForm_DPI.MyCommands.foundAllStyles = True
                    Else
                        lstBox_Entities.Items.Add("Please add: " & styleName)
                        ed.WriteMessage("Please add the missing MTEXT Style named: " & styleName & vbCrLf)
                        WinForm_DPI.MyCommands.foundAllStyles = False
                    End If
                Next
                trans.Commit()
            End Using
        Catch ex As Autodesk.AutoCAD.Runtime.Exception
            MsgBox("Error occured in: Form_DPI.CheckForMTEXTStyle(). Exception: " & ex.ToString())
        End Try

        If WinForm_DPI.MyCommands.foundAllStyles Then
            With lblEntPassed
                .Visible = True
                .ForeColor = Drawing.Color.DarkGreen
                .Text = "Passed 'Check for MTEXT Style(s)'"
            End With
            lbl_ValidatingEntities.Text = "All MTEXT Style(s) " & vbCrLf & "found in DWG"
            lbl_ValidatingEntities.Visible = True
            btn_AddMissingEntities.Visible = False
        Else
            lbl_ValidatingEntities.Text = "Please click " & ControlChars.Quote & "Refresh" & ControlChars.Quote & " after " & vbCrLf & "adding the missing Style(s):"
            lbl_ValidatingEntities.Visible = True
            btn_AddMissingEntities.Visible = True
            With lblEntPassed
                .Visible = True
                .ForeColor = Drawing.Color.Red
                .Text = "FAILED - Please see the 'Check for MTEXT Style(s)' tab"
            End With
        End If
        Me.tabCtrMain.SelectedTab = tbPg_InsertPoints
    End Sub

    ' MAIN Sub: Process validated data
    Private Sub btn_ProcessData_Click(sender As Object, e As EventArgs) Handles btn_ProcessData.Click

        If Not Application.DocumentManager.Count > 0 Then
            txtBox_TargetDrawing.Text = "Please open a drawing first"
            MsgBox("Please open a drawing first and make sure the Source and Destination Global Coordinate Systems are correct.")
            Exit Sub
        End If

        If cmbBox_SourceGCS.SelectedText = "" Then
            WinForm_DPI.MyCommands.sourceGCS = "LL83"
        Else
            WinForm_DPI.MyCommands.sourceGCS = cmbBox_SourceGCS.SelectedItem
        End If

        If WinForm_DPI.MyCommands.isBlockRef Then
            CheckForBlockRefs(WinForm_DPI.MyCommands.dt)
            If Not WinForm_DPI.MyCommands.foundAllBlockRefs Then
                MsgBox("Please verify that you have added all missing Blocks to the drawing: " & txtBox_TargetDrawing.Text)
                Exit Sub
            End If
        Else
            CheckForMTEXTStyle(WinForm_DPI.MyCommands.dt)
            If Not WinForm_DPI.MyCommands.foundAllStyles Then
                MsgBox("Please verify that you have added all missing MTEXT Styles to the drawing: " & txtBox_TargetDrawing.Text)
                Exit Sub
            End If
        End If

        If Not WinForm_DPI.MyCommands.foundAllLayers Then
            MsgBox("Please verify that you have added all missing Layers to the drawing: " & txtBox_TargetDrawing.Text)
            Exit Sub
        End If

        ' Turn off Object Snap just in case...
        Application.SetSystemVariable("SNAPMODE", 0)

        If WinForm_DPI.MyCommands.dt.Rows.Count > 0 Then
            ' See if we can actually create an Object Data Table always called "MORE_INFO" from a System.Data.DataTable
            If WinForm_DPI.MyCommands.objDataCount > 0 Then
                Dim ODFieldDefs(WinForm_DPI.MyCommands.objDataCount - 1, 2) As String
                ' FIELD_NAME# = ODFieldDefs(#,0)
                ' OD_TYPE# = ODFieldDefs(#,1)
                ' OBJECT_DATA# = ODFieldDefs(#,2)
                For ODItem As Integer = 0 To WinForm_DPI.MyCommands.objDataCount - 1
                    ODFieldDefs(ODItem, 0) = CStr(WinForm_DPI.MyCommands.dt.Rows(0).Item("FIELD_NAME" & ODItem + 1))
                    ODFieldDefs(ODItem, 1) = CStr(WinForm_DPI.MyCommands.dt.Rows(0).Item("OD_TYPE" & ODItem + 1))
                    ODFieldDefs(ODItem, 2) = CStr(WinForm_DPI.MyCommands.dt.Rows(0).Item("OBJECT_DATA" & ODItem + 1))
                Next

                Dim acCurDb As Autodesk.AutoCAD.DatabaseServices.Database = Application.DocumentManager.MdiActiveDocument.Database
                Dim mapApp As MapApplication = HostMapApplicationServices.Application
                Dim activeProject As Project.ProjectModel = mapApp.ActiveProject

                Util_ObjectData.CreateTable(ODFieldDefs, acCurDb, mapApp, activeProject)
            End If
        Else
            MsgBox("Please verify that you have loaded a CSV file and/or the CSV File Name is correct. Currently set to: " & txtBox_CSVFileName.Text)
            Exit Sub
        End If
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = doc.Editor
        Dim db As Database = doc.Database
        Dim entObjID As ObjectId = Nothing
        Dim countOfObjectsInserted As Integer = 0

        For Each row As System.Data.DataRow In WinForm_DPI.MyCommands.dt.Rows
            Try
                Using trans As Transaction = db.TransactionManager.StartTransaction()
                    entObjID = Util_Entity.InsertEntity(row)
                    trans.Commit()
                    countOfObjectsInserted = countOfObjectsInserted + 1
                End Using
                If WinForm_DPI.MyCommands.objDataCount > 0 Then
                    Using trans As Transaction = db.TransactionManager.StartTransaction()
                        Dim ODFieldDefs(WinForm_DPI.MyCommands.objDataCount - 1, 2) As String
                        ' FIELD_NAME# = ODFieldDefs(#,0)
                        ' OD_TYPE# = ODFieldDefs(#,1)
                        ' OBJECT_DATA# = ODFieldDefs(#,2)
                        For ODItem As Integer = 0 To WinForm_DPI.MyCommands.objDataCount - 1
                            ODFieldDefs(ODItem, 0) = CStr(row.Item("FIELD_NAME" & ODItem + 1))
                            ODFieldDefs(ODItem, 1) = CStr(row.Item("OD_TYPE" & ODItem + 1))
                            ODFieldDefs(ODItem, 2) = CStr(row.Item("OBJECT_DATA" & ODItem + 1))
                        Next

                        Dim acCurDb As Autodesk.AutoCAD.DatabaseServices.Database = Application.DocumentManager.MdiActiveDocument.Database
                        Dim mapApp As MapApplication = HostMapApplicationServices.Application
                        Dim activeProject As Project.ProjectModel = mapApp.ActiveProject
                        Util_ObjectData.AddFieldDefs(ODFieldDefs, entObjID)
                        trans.Commit()
                    End Using
                End If
            Catch ex As Autodesk.AutoCAD.Runtime.Exception
                MsgBox("Error occured in: Form_DPI.btn_ProcessData_Click() while processing WinForm_DPI.MyCommands.dt System.Data.DataRows. Exception: " & ex.ToString())
            End Try
        Next
        ' Clean up and empty all data/Buttons/TextBoxs/Labels
        ClearOutOldData()

        ' Let the user know what happened
        ed.WriteMessage(CStr(countOfObjectsInserted) & " objects were inserted into your drawing.")
        Me.Visible = False
        MsgBox(CStr(countOfObjectsInserted) & " objects were inserted into your drawing.")

    End Sub

    ' Clean up and empty all data/Buttons/TextBoxs/Labels
    Private Sub ClearOutOldData()

        ' ---- Block Ref(s) or MTEXT Style(s) ----
        lbl_ValidatingEntities.Visible = False
        btn_AddMissingEntities.Visible = False
        lstBox_Entities.Items.Clear()

        ' ---- Layer(s) ----
        lbl_ValidatingLayers.Visible = False
        btn_AddMissingLayers.Visible = False
        lblPassedLayers.Visible = False
        lstBox_Layers.Items.Clear()

        ' ---- CSV Info ----
        txtBox_CSVFileName.Text = "CSV to Use"

        ' ---- Wipe data loaded into DataTable from CSV file ----
        WinForm_DPI.MyCommands.dt.Clear()

        ' ---- Reset Insert Data tab ----
        btn_ProcessData.Visible = False
        lblEntPassed.Visible = False
        lblPassedLayers.Visible = False

        ' ---- Reset data Counters -----
        WinForm_DPI.MyCommands.attribCount = 0
        WinForm_DPI.MyCommands.objDataCount = 0
        WinForm_DPI.MyCommands.isBlockRef = True
        WinForm_DPI.MyCommands.foundAllBlockRefs = False
        WinForm_DPI.MyCommands.foundAllStyles = False
        WinForm_DPI.MyCommands.foundAllLayers = False

        ' ---- Finally, select the Setup tab ----
        Me.tabCtrMain.SelectedTab = tbPg_Setup

    End Sub
End Class
' (C) Copyright 2015 by  Steven H. Brubaker
' PURPOSE: Process AutoCAD MAP and CIVIL 3D Object Data Tables and Assignment/Update of Object Data on Entities

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry

Imports Autodesk.Gis.Map
Imports Autodesk.Gis.Map.ObjectData
Imports Autodesk.Gis.Map.Utilities
Imports Autodesk.Gis.Map.Project

Module Util_ObjectData

    ' Creates a Table with at least one Field Definition as required by AutoCAD ManagedMapAPI
    Public Sub CreateTable(ByRef ODFieldDefs(,) As String, ByRef db As Autodesk.AutoCAD.DatabaseServices.Database, ByRef mapApp As MapApplication, ByRef activeProject As Project.ProjectModel)

        Dim tableList As ObjectData.Tables = activeProject.ODTables
        Dim table As ObjectData.Table = Nothing
        Dim tabDefs As FieldDefinitions = Nothing
        Dim def1 As FieldDefinition = Nothing

        If tableList.IsTableDefined("MORE_INFO") Then
            ' Remove old Object Data Table
            RemoveTable(tableList, "MORE_INFO")
        End If

        ' Create the new (or replacement) Object Data Table
        tabDefs = mapApp.ActiveProject.MapUtility.NewODFieldDefinitions()

        ' FIELD_NAME# = ODFieldDefs(0)
        ' OD_TYPE# = ODFieldDefs(1)
        ' OBJECT_DATA# = ODFieldDefs(2)
        For ODCount As Integer = 0 To WinForm_DPI.MyCommands.objDataCount - 1
            Select Case ODFieldDefs(ODCount, 1)
                Case "Integer"
                    def1 = FieldDefinition.Create(ODFieldDefs(ODCount, 0), "Int Type Default", 0)
                    tabDefs.AddColumn(def1, ODCount)
                Case "Real"
                    def1 = FieldDefinition.Create(ODFieldDefs(ODCount, 0), "Real Type Default", 0.0)
                    tabDefs.AddColumn(def1, ODCount)
                Case "Point"
                    def1 = FieldDefinition.Create(ODFieldDefs(ODCount, 0), "Point Type Default", New Point3d(0.0, 0.0, 0.0))
                    tabDefs.AddColumn(def1, ODCount)
                Case "Character"
                    def1 = FieldDefinition.Create(ODFieldDefs(ODCount, 0), "String Type Default", "Intentionally left blank")
                    tabDefs.AddColumn(def1, ODCount)
                Case Else
                    def1 = FieldDefinition.Create("ERROR", "Invalid Object Data found: ", DateTime.Now.ToString("MMM-dd-yyyy"))
                    tabDefs.AddColumn(def1, ODCount)
            End Select
        Next
        tableList.Add("MORE_INFO", tabDefs, "Data Importer Generated Table - Created: " & DateTime.Now.ToString("MMM-dd-yyyy"), True)
    End Sub

    Public Sub AddFieldDefs(ByRef ODFieldDefs(,) As String, ByRef id As ObjectId)
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database
        Dim mapApp As MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
        Dim projModel As ProjectModel = mapApp.ActiveProject
        Dim table As ObjectData.Table
        Dim tables As ObjectData.Tables = projModel.ODTables

        Using lck As DocumentLock = doc.LockDocument()
            Using trans As Transaction = db.TransactionManager.StartTransaction()
                Try
                    Dim dbObj As DBObject = trans.GetObject(id, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                    table = tables("MORE_INFO")
                    Dim odrecords As ObjectData.Records = table.GetObjectTableRecords(Convert.ToUInt32(0), id, Constants.OpenMode.OpenForRead, False)
                    Dim odRecord As ObjectData.Record = odrecords.Item(0)
                    odRecord = Autodesk.Gis.Map.ObjectData.Record.Create()
                    table.InitRecord(odRecord)
                    Dim val As MapValue
                    For ODCount As Integer = 0 To WinForm_DPI.MyCommands.objDataCount - 1
                        Select Case ODFieldDefs(ODCount, 1)
                            Case "Integer"
                                val = odRecord(ODCount)
                                val.Assign(CInt(ODFieldDefs(ODCount, 2)))
                            Case "Real"
                                val = odRecord(ODCount)
                                val.Assign(CDbl(ODFieldDefs(ODCount, 2)))
                            Case "Character"
                                val = odRecord(ODCount)
                                val.Assign(ODFieldDefs(ODCount, 2))
                            Case "Point"
                                val = odRecord(ODCount)
                                val.Assign(ConvertStringTo3dPoint(ODFieldDefs(ODCount, 2)))
                        End Select
                    Next
                    table.AddRecord(odRecord, id)
                    trans.Commit()
                Catch exc As Autodesk.Gis.Map.MapException
                    MsgBox("Error : " + exc.Message.ToString())
                End Try
            End Using
        End Using
    End Sub

    Public Function ConvertStringTo3dPoint(ByVal convertee As String) As Autodesk.AutoCAD.Geometry.Point3d
        Dim tmp As String() = convertee.Split(New Char() {","c})
        If tmp.Length <> 0 Then
            Return New Point3d(Convert.ToDouble(tmp(0)), Convert.ToDouble(tmp(1)), Convert.ToDouble(tmp(2)))
        End If
    End Function

    ' Removes the Table named tableName.
    Public Function RemoveTable(ByVal tables As Tables, ByVal tableName As String) As Boolean
        Try
            tables.RemoveTable(tableName)
            Return True
        Catch err As MapException
            Return False
        End Try
    End Function

End Module

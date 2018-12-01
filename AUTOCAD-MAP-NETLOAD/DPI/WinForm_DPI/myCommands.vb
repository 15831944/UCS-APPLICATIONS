' (C) Copyright 2015 by Steven H. Brubaker
' Purpose: Process Survey Data and insert Blocks or MTEXT into a Georeferenced AutoCAD drawing
' Information about the DataTable generated from the CSV file that was created by the "Data Importer.xlsm" workbook using VBA.

Imports System
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.EditorInput

' This line is not mandatory, but improves loading performances
<Assembly: CommandClass(GetType(WinForm_DPI.MyCommands))>

Namespace WinForm_DPI
    Public Class MyCommands

        ' Windows Form for Data Points Importer
        Friend Shared m_ps As Form_DPI = Nothing

        ' DataTable created from Points-xxxxxxxx-DateCreated.csv. See Comments at top for Column information
        Friend Shared dt As System.Data.DataTable

        ' Control flags
        Friend Shared isBlockRef As Boolean = True          ' Points are for BlockRef Data (TRUE) or MTEXT (FALSE)
        Friend Shared foundAllBlockRefs As Boolean = False  ' All Data File (CSV) Block Ref(s) found in DWG
        Friend Shared foundAllStyles As Boolean = False     ' All Data File (CSV) MTEXT Style(s) found in DWG
        Friend Shared foundAllLayers As Boolean = False     ' All Data File (CSV) Layer(s) found in DWG

        ' Shared Data for use in processing the Block References or MTEXT objects
        Friend Shared attribCount As Integer = 0            ' Number of ATTRIBUTE(s) Data File (CSV) = ATTRIBUTE# Columns in DataTable
        Friend Shared objDataCount As Integer = 0           ' Number of Object Data FIELD(s) Data File (CSV) = FIELD_NAME# Columns in DataTable
        Friend Shared sourceGCS As String = Nothing         ' LL83 99% of the time. GCS = Global Coordinate System of data provided by Survey Crew or GPS
        Friend Shared destinationGCS As String = Nothing    ' MA83F or PA83... most of the time. GCS of DWG.

        <CommandMethod("DPI_RUN")>
        Public Sub MyCommand()
            ' Verify the current drawing has a Global Coordinate System (GCS) assigned
            If Util_GIS.GIS_Assignment_Check() Then
                If m_ps Is Nothing Then
                    m_ps = New Form_DPI
                End If
                Application.ShowModalDialog(m_ps)
            End If
        End Sub
    End Class
End Namespace

' TODO: Format Text size on Form_DPI

' MTEXT 
' Column Name   Index
' LATITUDE      0
' LONGITUDE     1
' MTEXT         2
' LAYER_NAME    3
' STYLE         4

' BLOCKS
' Column Name   Index
' LATITUDE	    0
' LONGITUDE	    1
' ATTRIBUTE1	2
' ATTRIBUTE2	3
' ATTRIBUTE3	4
' ATTRIBUTE4	5
' ATTRIBUTE5	6
' ATTRIBUTE6	7
' ATTRIBUTE7	8
' ATTRIBUTE8	9
' ATTRIBUTE9	10
' ATTRIBUTE10	11
' ATTRIBUTE11	12
' ATTRIBUTE12	13
' ELEVATION	    14
' Block Name	15
' Layer Name	16

' OBJECT DATA
' Column Name
' ================
' TABLE_NAME
' FIELD_NAME1...12
' OD_TYPE1...12
' OBJECT_DATA1...12
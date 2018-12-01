' (C) Copyright 2015 by  Steven H. Brubaker
' Purpose: Static Geographic Information Systems (GIS) Handling Subs and Functions for Point Data Importing Processing
' including reprojection and Object Data Table creation and attachment to inserted objects.

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.Gis.Map.Platform
Imports OSGeo.MapGuide

Module Util_GIS

    ' Check to make sure the current DWG has been assigned a Global Coordinate System
    Public Function GIS_Assignment_Check() As Boolean
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = doc.Editor

        Dim wkt As String
        ' Autodesk.Gis.Map.Platform.AcMapMap function required to obtain details of 
        ' the currently assigned Coordinate System. Specifically the abbreviations
        ' most AutoCAD Map/Civil 3D see such as "MA83F" or "LL83".
        wkt = AcMapMap.GetCurrentMap().GetMapSRS()

        If wkt.Length = 0 Then
            ed.WriteMessage("NO COORDINATE SYSTEM ASSIGNED. Please assign a Global Coordinate System.")
            ' Execute the built-in AutoCAD Map/Civil 3D "ASSIGN GLOBAL COORDINATE SYSTEM..." command
            doc.SendStringToExecute("ADESETCRDSYS ", False, False, True)
            GIS_Assignment_Check = False
        Else
            ' Extract the AutoCAD abbreviation name such as "MA83F"
            Dim factory As OSGeo.MapGuide.MgCoordinateSystemFactory = New OSGeo.MapGuide.MgCoordinateSystemFactory()
            Dim cs As String = factory.ConvertWktToCoordinateSystemCode(wkt) ' ACAD Dictionary lookup for abbreviated GCS name such as MA83F
            WinForm_DPI.MyCommands.destinationGCS = cs ' Assign to Global destinationGCS variable for use in Form/Palette TextBox/ComboBox
            GIS_Assignment_Check = True
        End If
    End Function

    Public Function ConvertPointGCS(ByVal sourceGCS As String, ByVal destGCS As String, ByRef sourcePoint3d As Autodesk.AutoCAD.Geometry.Point3d) As Autodesk.AutoCAD.Geometry.Point3d
        ' AutoCAD Map and GIS API Coordinate Object creation factory used to...
        Dim factory As MgCoordinateSystemFactory = Nothing
        ' Access the Autodesk catalog of Coordinate Systems such as NAD83 based projection into MA83 which is converted to a...
        Dim catalog As MgCoordinateSystemCatalog = Nothing
        ' Dictionary to look-up...
        Dim coordDic As MgCoordinateSystemDictionary = Nothing
        ' a source Global Coordinate System (GCS), such as LL83 or WGS84, and ...
        Dim coordIn As MgCoordinateSystem = Nothing
        ' a destination GCS, such as MA83F or RI83F, so we can...
        Dim coordOut As MgCoordinateSystem = Nothing
        ' transform (Project) an AutoCAD 3D Point from the source GCS (99% of the time it will be LL83) to...
        Dim coordTransform As MgCoordinateSystemTransform = Nothing
        ' our AutoCAD Map/Civil 3D Geographical 3d Point using the drawings Assigned GCS such as MA83F:
        Dim destCoord As MgCoordinate = Nothing

        Try
            factory = New MgCoordinateSystemFactory()
            catalog = factory.GetCatalog()
            coordDic = catalog.GetCoordinateSystemDictionary()
            coordIn = coordDic.GetCoordinateSystem(sourceGCS)
            coordOut = coordDic.GetCoordinateSystem(destGCS)
            coordTransform = factory.GetTransform(coordIn, coordOut)

            '(X, Y, Z) = (Longitude, Latitude, Elevation) in GEO API (Yes, that is right: the GEO API is LONGITUDE = X and LATITUDE = Y,
            ' even though we think of LL83 as "LATitude, LONGitude 83")
            destCoord = coordTransform.Transform(sourcePoint3d.X, sourcePoint3d.Y, sourcePoint3d.Z)
            ConvertPointGCS = New Autodesk.AutoCAD.Geometry.Point3d(destCoord.GetX(), destCoord.GetY(), destCoord.GetZ())
        Catch
            MsgBox("Error occured in converting Global Coordinate Systems. Source: Util_GIS.ConvertPointGCS()")
        End Try
    End Function

End Module

' (C) Copyright 2015 by  Steven H. Brubaker
' Purpose: Utility functions to place desired Blocks, Blocks with Attributes and MTEXT into Model Space

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput

Module Util_Entity

    Public Function InsertEntity(ByRef row As System.Data.DataRow) As ObjectId
        ' Open the Drawing's Database
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database

        '... examine and process each row to create and insert each Block Reference 
        ' followed by Adding ATTRIBUTES and then OBJECT DATA
        ' If the CVS is for Block References, then...
        If WinForm_DPI.MyCommands.isBlockRef Then
            ' Get the name of the current Block Reference in this Row
            Dim blockRefName As String = CStr(row.Item("BLOCK_NAME"))
            ' Verify Block Reference's Name is not blank or the name of the column from the DataTable
            If blockRefName <> "BLOCK_NAME" And blockRefName <> "" Then
                ' Create a new 3D Point based on Row's Longitude, Latitude and Elevation then...
                Dim location As Point3d = Nothing
                ' convert the 3D Point to the current drawing's Global Coordinate System (GCS)
                location = ConvertPointGCS(WinForm_DPI.MyCommands.sourceGCS, WinForm_DPI.MyCommands.destinationGCS, New Point3d(CDbl(row.Item("LONGITUDE")), CDbl(row.Item("LATITUDE")), CDbl(row.Item("ELEVATION"))))
                ' Build a 0 indexed array of strings for our Attribute(s)
                Dim attribs(WinForm_DPI.MyCommands.attribCount - 1) As String
                For i As Integer = 0 To WinForm_DPI.MyCommands.attribCount - 1
                    attribs(i) = CStr(row.Item("ATTRIBUTE" & (i + 1)))
                Next i
                ' Get the Layer to place the block on
                Dim blkLayer As String = CStr(row.Item("LAYER_NAME"))
                ' and finally, create and insert the block at the correct Geographic location
                InsertEntity = InsertBlockWithAttributes(blockRefName, location, attribs, WinForm_DPI.MyCommands.attribCount, blkLayer)
            End If
        Else
            If CheckForTextStyle(CStr(row.Item("STYLE"))) Then
                Using trans As Transaction = db.TransactionManager.StartTransaction()
                    Dim blkTbl As BlockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead)
                    Dim blkTblRec As BlockTableRecord = trans.GetObject(blkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)

                    Using acMTEXT As MText = New MText()
                        acMTEXT.Contents = CStr(row.Item("MTEXT"))
                        acMTEXT.Location = ConvertPointGCS(WinForm_DPI.MyCommands.sourceGCS, WinForm_DPI.MyCommands.destinationGCS, New Point3d(CDbl(row.Item("LONGITUDE")), CDbl(row.Item("LATITUDE")), 0.0))
                        acMTEXT.Layer = CStr(row.Item("LAYER_NAME"))
                        acMTEXT.TextStyleId = GetTextStyleID(CStr(row.Item("STYLE")))
                        blkTblRec.AppendEntity(acMTEXT)
                        trans.AddNewlyCreatedDBObject(acMTEXT, True)
                        InsertEntity = acMTEXT.ObjectId
                    End Using
                    trans.Commit()
                End Using
            Else
                InsertEntity = ObjectId.Null
            End If
        End If
    End Function

    Public Function InsertBlockWithAttributes(ByVal blkName As String, ByVal blkPoint3d As Point3d, ByRef attribs() As String, ByVal numberAttributes As Integer, ByVal blkLayer As String) As ObjectId
        ' Get the current database and start a transaction
        Dim acCurDb As Database = Application.DocumentManager.MdiActiveDocument.Database

        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
            ' Open the Block table for read
            Dim acBlkTbl As BlockTable
            acBlkTbl = CType(acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead), BlockTable)

            Dim blkRecId As ObjectId = ObjectId.Null
            ' This should have been caught earlier but rather than crash with a NullException error, we will check again
            If acBlkTbl.Has(blkName) Then
                blkRecId = acBlkTbl(blkName)
            Else
                MsgBox("Please use INSERT or DESIGN CENTER (DC) to add this block " & blkName & " to your drawing.")
                InsertBlockWithAttributes = ObjectId.Null
                Exit Function
            End If

            ' Create and insert the new block reference
            If blkRecId <> ObjectId.Null Then
                Dim acBlkTblRec As BlockTableRecord
                acBlkTblRec = CType(acTrans.GetObject(blkRecId, OpenMode.ForRead), BlockTableRecord)

                Using acBlkRef As New BlockReference(blkPoint3d, acBlkTblRec.Id)

                    Dim acCurSpaceBlkTblRec As BlockTableRecord
                    acCurSpaceBlkTblRec = DirectCast(acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite), BlockTableRecord)

                    acBlkRef.Layer = blkLayer

                    acCurSpaceBlkTblRec.AppendEntity(acBlkRef)
                    acTrans.AddNewlyCreatedDBObject(acBlkRef, True)
                    InsertBlockWithAttributes = acBlkRef.ObjectId
                    ' Verify block table record has attribute definitions associated with it
                    If acBlkTblRec.HasAttributeDefinitions Then
                        Dim currentAttributeNr As Integer = 0
                        ' Add attributes from the block table record
                        For Each objID As ObjectId In acBlkTblRec
                            ' Yes, this is convoluted but you need to identify the previously created object AFTER it has actually been 
                            ' written to the Drawing's Database in the BlockTable as a NEW BlockTableRecord before you can actually
                            ' access it...
                            Dim dbObj As DBObject = acTrans.GetObject(objID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
                            ' and because the Block can have multiple types of Objects attached to it, you need to find which Object
                            ' is actually an AttributeDefinition:
                            If TypeOf dbObj Is AttributeDefinition Then
                                Dim acAtt As AttributeDefinition = CType(dbObj, AttributeDefinition)
                                ' before you can cycle through AttributeDefinitions to...
                                If Not acAtt.Constant Then
                                    Using acAttRef As New AttributeReference
                                        ' actually set your AttributeReference to the desired...
                                        acAttRef.SetAttributeFromBlock(acAtt, acBlkRef.BlockTransform)
                                        acAttRef.Position = acAtt.Position.TransformBy(acBlkRef.BlockTransform)
                                        ' value for your BlockRef:
                                        If currentAttributeNr <= numberAttributes Then
                                            acAttRef.TextString = attribs(currentAttributeNr)
                                            currentAttributeNr = currentAttributeNr + 1
                                        Else
                                            acAttRef.TextString = acAtt.TextString
                                        End If
                                        acBlkRef.AttributeCollection.AppendAttribute(acAttRef)
                                        acTrans.AddNewlyCreatedDBObject(acAttRef, True)
                                    End Using
                                End If
                            End If
                        Next
                    End If
                End Using
                acTrans.Commit()
            Else
                InsertBlockWithAttributes = ObjectId.Null
            End If
        End Using
    End Function

    Public Function CheckForTextStyle(ByVal myTextStyleName As String) As Boolean
        If Not GetTextStyleID(myTextStyleName) = ObjectId.Null Then
            CheckForTextStyle = True
        End If
    End Function

    Public Function GetTextStyleID(ByVal myTextStyleName As String) As ObjectId
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database

        Try
            Using trans As Transaction = db.TransactionManager.StartTransaction()
                Dim textStyleTable As TextStyleTable = trans.GetObject(db.TextStyleTableId, OpenMode.ForRead)
                If textStyleTable.Has(myTextStyleName) Then
                    Dim textStyleID As ObjectId = textStyleTable(myTextStyleName)
                    GetTextStyleID = textStyleID
                Else
                    GetTextStyleID = ObjectId.Null
                    MsgBox("Please use DESIGN CENTER (DC) to add this Text Style " & myTextStyleName & " to your drawing.")
                End If
                trans.Commit()
            End Using
        Catch ex As Exception
            MsgBox("Error occured in: Util.Entity.GetTextStyleID(). Exception: " & ex.ToString())
        End Try
    End Function

End Module

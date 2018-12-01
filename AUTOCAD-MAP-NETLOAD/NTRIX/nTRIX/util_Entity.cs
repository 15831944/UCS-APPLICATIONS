// (C) Copyright 2017 by Steven H. Brubaker
// Purpose: Utility functions to place desired Blocks, Blocks with Attributes and MTEXT into Model Space

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System.Data;

namespace nTRIX
{
    class util_Entity
    {
        // Select all DBTEXT ("TEXT") and DBPOINT ("POINT") objects on Layer 0
        private static ObjectIdCollection GetDXFEntities()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            // AutoCAD Selection Filtering using a Typed Values array with
            // old style DXFCode or the newer enumerated 'DxfCode.xxx'
            TypedValue[] tvs = new TypedValue[3];

            // Only select objects on LAYER 0 -and-
            tvs.SetValue(new TypedValue((int)DxfCode.LayerName, "0"), 0);

            // Only select objects that are DTEXT or POINTS -and-
            tvs.SetValue(new TypedValue(0, "TEXT,POINT"), 1);

            // Only if the object is in MODEL Space
            tvs.SetValue(new TypedValue(410, "Model"), 2);

            // Create the filter...
            SelectionFilter sf = new SelectionFilter(tvs);

            // Execute our filtered selection on all objects in the DWG
            PromptSelectionResult psr = ed.SelectAll(sf);

            // If any objects were caught in our filter...
            if (psr.Status == PromptStatus.OK)
            {
                // return our new collection of objects or
                return new ObjectIdCollection(psr.Value.GetObjectIds());
            }
            else // return an empty collection to avoid throwing an exception/error
            {
                return new ObjectIdCollection();
            }
        } // END ObjectIdCollection()

        // Cycle though filtered DWG Objects [util_Entity.GetDXFEntities()] and
        // assign correct Block [util_ExcelReader.ReadInDXFDataTable] to 
        // the position of DBTEXT object 
        public static void ProcessDXFEntities()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Create a filtered Collection with ObjectId's
                ObjectIdCollection ents = GetDXFEntities();

                // Cycle through our collection (even if there are no Entities (AutoCAD Objects)) to...
                foreach (ObjectId objID in ents)
                {
                    // Use DTEXT objectId's to assign the appropriate Block with data
                    if (objID.ObjectClass.DxfName == "TEXT")
                    {
                        DBText dbTxt = tr.GetObject(objID, OpenMode.ForRead) as DBText;

                        if(dbTxt.TextString == "DW")
                        {
                            dbTxt.UpgradeOpen();
                            dbTxt.TextString = "(MATERIAL) DRIVWAY";
                            dbTxt.Height = 1.5;
                            dbTxt.Layer = "SHOTPOINTS-CHECK";
                            dbTxt.DowngradeOpen();
                        }

                        // Cycle over the Edge of Pavement/Road/Sidewalks annotations...
                        if (dbTxt.TextString.StartsWith("SW") 
                            || dbTxt.TextString.StartsWith("EO")
                            || dbTxt.TextString.StartsWith("FO")
                            || dbTxt.TextString.StartsWith("PK"))
                        {
                            // ... and move them to the SHOTPOINTS Layer
                            dbTxt.UpgradeOpen();

                            // If MyCommands.moveToUsedShotpointsLayer == false, don't move to
                            if (MyCommands.moveToShotpointsLayer)
                            {
                                dbTxt.Layer = "SHOTPOINTS";
                                dbTxt.DowngradeOpen();
                            }
                            else
                            {
                                dbTxt.Layer = "SHOTPOINTS-USED";
                                dbTxt.DowngradeOpen();
                            }
                        }
                        else
                        {
                            // We need to check the Field Crew Annotations before running it
                            // against the nTRIX.xlsx list of annotations
                            string crewAnno = dbTxt.TextString;

                            // To deal with Crew Annotations that inculde "_"
                            if (dbTxt.TextString.Contains("_"))
                            {
                                // Split DBTEXT.TextString into 1 to 3 pieces
                                string[] attribVals = dbTxt.TextString.Split('_');

                                // If DBTEXT.TextString has an "_"...
                                if (!string.IsNullOrEmpty(attribVals[1]))
                                {
                                    switch (attribVals[0])
                                    {
                                        case "PARKING":
                                        case "PARKINGMETER":
                                        case "P-E":
                                        case "P-JO":
                                        case "PM":
                                        case "PM1":
                                        case "PM2":
                                        case "POLE-ELEC.":
                                        case "POLE-JO":
                                        case "POLE-SE":
                                        case "POLE-ST":
                                        case "P-T":
                                        case "TREE":
                                            // ...only use the first part that 'should' match the 
                                            // Excel DXF value "Annotation to use" column
                                            crewAnno = attribVals[0];
                                            break;
                                    }
                                }
                            }

                            // Look-up the DBTEXT annotation using our parsed crewAnno value
                            var results = (from rw in MyCommands.lkAbbrevTbl.AsEnumerable()
                                           where rw.Field<string>("ANNO") == crewAnno
                                           select rw).FirstOrDefault();

                            // If we found a matching Annotation...
                            if (results != null)
                            {
                                // Get the location of the DTEXT Annotation (Point3D)
                                // to use as the insertion point of the new Block
                                Point3d loc = dbTxt.Position;

                                Double rotation;
                                if (MyCommands.useCurrentUCS_Rotation == false)
                                {
                                    // Use the DBText's Rotation to set the rotation of the new BlockRef
                                    rotation = dbTxt.Rotation;
                                }
                                else
                                {
                                    CoordinateSystem3d UCS = ed.CurrentUserCoordinateSystem.CoordinateSystem3d;
                                    Plane OcsPlane = new Plane(Point3d.Origin, UCS.Zaxis);
                                    CoordinateSystem3d OCS = Matrix3d.PlaneToWorld(OcsPlane).CoordinateSystem3d;
                                    rotation = OCS.Xaxis.GetAngleTo(UCS.Xaxis, UCS.Zaxis);
                                }

                                // ...we insert the appropriate Block ONLY
                                // if we already ran util_ExcelReader.ValidateLayers & 
                                // util_ExcelReader.ValidateBlocks
                                InsertBlock(loc, dbTxt.TextString,
                                    results.Field<string>("BLOCK"),
                                    results.Field<string>("DYN_VAL"),
                                    results.Field<string>("LAYER"),
                                    results.Field<string>("PROP_NAME"),
                                    rotation);

                                // ... and move them to the SHOTPOINTS Layer
                                dbTxt.UpgradeOpen();

                                // If MyCommands.moveToUsedShotpointsLayer == false, don't move to
                                if (MyCommands.moveToShotpointsLayer)
                                {
                                    dbTxt.Layer = "SHOTPOINTS";
                                    dbTxt.DowngradeOpen();
                                }
                                else
                                {
                                    dbTxt.Layer = "SHOTPOINTS-USED";
                                    dbTxt.DowngradeOpen();
                                }
                            }
                            else //...we could not find a match and...
                            {
                                // ...move the DBTEXT to the SHOTPOINTS-CHECK Layer 
                                // for the Drafter to decide what to do 
                                dbTxt.UpgradeOpen();
                                dbTxt.Layer = "SHOTPOINTS-CHECK";
                                dbTxt.DowngradeOpen();
                            }
                        }
                    }

                    // We leave the LINES in the DXF alone and move all the LAYER 0 POINTS
                    // onto the SHOTPOINTS Layer for the Drafter to use as they see fit
                    if (objID.ObjectClass.DxfName == "POINT")
                    {
                        DBPoint pnt = tr.GetObject(objID, OpenMode.ForWrite) as DBPoint;

                        // If MyCommands.moveToUsedShotpointsLayer == false, don't move to
                        if (MyCommands.moveToShotpointsLayer)
                        {
                            pnt.Layer = "SHOTPOINTS";
                        }
                        else
                        {
                            pnt.Layer = "SHOTPOINTS-USED";
                        }
                    }
                }
                tr.Commit();
            }
        } // END ProcessDXFEntities

        // Note that a lot of checking has been eliminated by
        // util_ExcelReader.ValidateLayers & util_ExcelReader.ValidateBlocks BUT
        // you MUST call those methods before using InsertBlock!
        public static void InsertBlock(
            Point3d loc,
            string anno,
            string blockName,
            string dyn_val,
            string layer,
            string prop_name,
            double blkRotation)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Open BlockTable for Read
                BlockTable blkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                // Create a new ID for the new Block so we can access any Dynamic or 
                // Attribute properties after creation and insertion into the drawing
                ObjectId blkRecId = ObjectId.Null;

                if (blkTbl.Has(blockName))
                {
                    // Use the correct BlockName from our Excel look-up tab (System.Data.DataTable)
                    blkRecId = blkTbl[blockName];
                }
                else
                {
                    ed.WriteMessage("Please use INSERT or DESIGN CENTER (DC) to add this block " + blockName + " to your drawing.\n");
                    return;
                }

                if (blkRecId != ObjectId.Null)
                {
                    // Get a reference to a Block Table Record for our new Block named in nTRIX.xlsx
                    BlockTableRecord blkTblRec = tr.GetObject(blkRecId, OpenMode.ForWrite) as BlockTableRecord;

                    using (BlockReference blkRef = new BlockReference(loc, blkTblRec.Id))
                    {
                        // Open the [Model Space] BlockTable and get a 
                        BlockTableRecord acCurSpaceBlkTblRec = tr.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                        // Assign the correct Layer
                        blkRef.Layer = layer;
                        blkRef.Rotation = blkRotation;

                        // Append the new Block
                        acCurSpaceBlkTblRec.AppendEntity(blkRef);

                        // and insert it into the DWG's BlockTableRecord
                        tr.AddNewlyCreatedDBObject(blkRef, true);

                        // Set any Dynamic Blocks to display the correct symbol
                        // (aka -> set the Property to the correct Dynamic_Value of nTRIX.xlsx
                        // such as MH-VZ uses "Verizon" as the "OWNER" DynamicBlockReferenceProperty
                        if (blkRef.IsDynamicBlock)
                        {
                            // Property Collection (DynamicBlockReferencePropertyCollection) is the 
                            // list of choices on a Dynamic Block that a Drafter can pick from
                            DynamicBlockReferencePropertyCollection props = blkRef.DynamicBlockReferencePropertyCollection;

                            // Initialize the count of Properties (aka Choices) available to the Drafter
                            int propValues = 0;

                            // Cycle through the list and find the one that matches the Dynamic_Value of nTRIX.xlsx
                            foreach (DynamicBlockReferenceProperty prop in props)
                            {
                                // Use a zero based array to hold our values (Choice's numeric value)
                                object[] values = prop.GetAllowedValues();
                                propValues = values.Length - 1;

                                // Set property (Choice) to desired value (ie "Verizon" for MH-VZ)
                                if (prop.PropertyName == prop_name && !prop.ReadOnly)
                                {
                                    for (int i = 0; i < propValues; i++)
                                    {
                                        if (dyn_val == values[i].ToString())
                                        {
                                            // Example: "Verizon" for MH-VZ
                                            prop.Value = values[i];
                                        }
                                    }
                                }
                            }
                        }

                        // Parse and add any defined Block Attributes (For example: P-E_123 or TREE_6) 
                        if (blkTblRec.HasAttributeDefinitions)
                        {
                            // Split the ANNO from the Field Crew by the '_" character
                            // to get the desired Block Attribute string value to use
                            string[] attribVals = anno.Split('_');

                            // Intialize our Block Attributes counter in case this Block has Attributes
                            // Notes: array.Split() removes the "_" and keeps the start.
                            int numberAttributes = attribVals.Count();

                            // In case  the Field Crew add too many "_"'s
                            if(numberAttributes >= 4)
                            {
                                // Only use the first Splits. Example: PM2_123_124_125
                                //      attribVals[0] = PM2     keep
                                //      attribVals[1] = 123     keep 
                                //      attribVals[2] = 124     keep
                                //      attribVals[3] = 125     by setting
                                numberAttributes = 3;
                                //      we ignore attribVals[3] = 125
                            }

                            // In case the Field Crew misses the second attribute on:
                            //      PARKING METER - DOUBLE
                            //      JOINT-POLE-D
                            //      POLE2
                            if (blockName == "JOINT-POLE-D" || blockName == "POLE2" || blockName == "PARKING METER - DOUBLE")
                            {
                                if(numberAttributes == 2)
                                {
                                    Array.Resize(ref attribVals, 3);
                                    if(blockName == "JOINT-POLE-D" || blockName == "POLE2")
                                    {
                                        attribVals[2] = "(E)NT";
                                        numberAttributes = 3;
                                    }
                                    else
                                    {
                                        attribVals[2] = "";
                                        numberAttributes = 3;
                                    }
                                }
                            }

                            // Check to see if there are any Field Crew notes (_xxxx or _xxx_xxx)
                            if (numberAttributes > 1)
                            {
                                // Track our Block Attribute(s)
                                int currentAttribNR = 1;

                                // Because the Block can have multiple types of Objects attached to it, 
                                // you need to find which Object is actually an AttributeDefinition...
                                foreach (ObjectId acobjID in blkTblRec)
                                {
                                    // Get a reference to our DBObject (AutoCAD Database Object)
                                    DBObject dbObj = tr.GetObject(acobjID, OpenMode.ForRead);

                                    // See if our DBObject is a Block Attribute (AttributeDefinition)
                                    if (dbObj.GetType() == typeof(AttributeDefinition))
                                    {
                                        // (Cast) our DBObject to a Block Attribute (AttributeDefinition)
                                        AttributeDefinition acAtt = (AttributeDefinition)dbObj;

                                        // Make sure we are not trying to modify a Constant (READ ONLY)
                                        // Block Attribute (AttributeDefinition)
                                        if (!acAtt.Constant)
                                        {
                                            // Get a reference to our Block Attribute (AttributeDefinition)'s
                                            using (AttributeReference acAttRef = new AttributeReference())
                                            {
                                                // Use the default orientation of the Block Attribute 
                                                acAttRef.SetAttributeFromBlock(acAtt, blkRef.BlockTransform);

                                                // Use the default position/location of the Block Attribute
                                                acAttRef.Position = acAtt.Position.TransformBy(blkRef.BlockTransform);

                                                // Assign the Field Crew note (attribVals[#]) and increment counter
                                                if (currentAttribNR <= numberAttributes)
                                                {
                                                    switch (blockName)
                                                    {
                                                        case "TREE W-ATTRIB":
                                                            acAttRef.TextString = attribVals[currentAttribNR] + "\"";
                                                            break;
                                                        case "SOLE-OWNED-POLE-D":
                                                        case "JOINT-POLE-D":
                                                        case "POLE2":
                                                            acAttRef.TextString = "P." + attribVals[currentAttribNR];
                                                            currentAttribNR = currentAttribNR + 1;
                                                            break;
                                                        default:
                                                            acAttRef.TextString = attribVals[currentAttribNR];
                                                            currentAttribNR = currentAttribNR + 1;
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    // Use the default Attribute
                                                    acAttRef.TextString = acAtt.TextString;
                                                }

                                                // Commit our changes to the Block Reference and...
                                                blkRef.AttributeCollection.AppendAttribute(acAttRef);
                                                // ...add the new Attributes to the Block Reference
                                                tr.AddNewlyCreatedDBObject(acAttRef, true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                tr.Commit();
            }
        } // END InsertBlock

    } // END util_Entity Class
} // END nTRIX Namespace

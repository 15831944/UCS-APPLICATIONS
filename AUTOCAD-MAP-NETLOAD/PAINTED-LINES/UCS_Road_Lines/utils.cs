// (C) Copyright 2016 by Steven H. Brubaker
// Purpose: See comments below

using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Colors;

namespace UCS_Road_Lines
{
    public class utils
    {   
        // ************************************************************************
        // DEFAULTS in case the User forgot to click on an image
        // of the Road Line Style desired
        // ************************************************************************
        // Style to be assigned based on User selection of a picture (embedded WPF)
        // of the road on the UserControl (PaletteSet)
        static public string roadStyle = "1L-NM";
        // Currently Active Style for Tooltips
        static public string currentlyActiveStyle = "Current Style: None selected";
        // Number of Lanes
        static public int nrLanes = 1;
        // Width of Lane(s) on desired Road Line Style in FEET
        // Minimum = 6.7', Max = 15.0'
        static public double laneWidth = 11.0;
        // Shoulder widths on desired Road Line Style
        static public double rightShoulderWidth = -0.5;
        static public double leftShoulderWidth = 0.5;
        // Median widths on desired Road Line Style
        static public double leftMedianWidth = 0.5;
        static public double rightMedianWidth = -0.5;
        // Draw EOP
        static public bool drawEOP = true;
        // UC Synergetic Standard MUTCD Road Global Width
        static public double whiteStripeLineWidth = 0.41;
        static public double yellowStripLineWidth = 0.33;
        /*
         Name                               Color   Global Width    LineType
         MUTCD-LINE-DASHED-WHITE            9       0.41            ACAD_ISO03W100
         MUTCD-LINE-DASHED-YELLOW           9       0.33            ACAD_ISO03W100
         MUTCD-LINE-SOLID-DOUBLE-YELLOW     52      0.33            CONTINUOUS
         MUTCD-LINE-SOLID-WHITE             9       0.41            CONTINUOUS
         T-EOP                              8       0.0             CONTINUOUS
         T-ROAD-CNTR                        2       0.0             CENTER                    
        */
        // ************************************************************************

        // CheckLayers: Validate that required Layers and Linetypes 
        // are present in the drawing. Add if not present.
        public static void CheckLayers(string layerToCheck, short colorToUse, string lineTypeToAssign)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Open the Layer table for read
                LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                LayerTableRecord ltr;

                if (lt.Has(layerToCheck) == false)
                {
                    ltr = new LayerTableRecord();
                    
                    // Assign the layer the ACI color
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, colorToUse);

                    // Name the new Layer
                    ltr.Name = layerToCheck;

                    // Upgrade the Layer table for write
                    lt.UpgradeOpen();

                    // Append the new layer to the Layer table and the transaction
                    lt.Add(ltr);
                    tr.AddNewlyCreatedDBObject(ltr, true);
                }
                else
                {
                    ltr = tr.GetObject(lt[layerToCheck], OpenMode.ForRead) as LayerTableRecord;
                }

                // Open the Line Type table for read
                LinetypeTable linTbl = tr.GetObject(db.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;

                if (linTbl.Has(lineTypeToAssign) == false)
                {
                    // Load desired Linetype
                    db.LoadLineTypeFile(lineTypeToAssign, "acad.lin");
                }

                ltr.UpgradeOpen();
                ltr.LinetypeObjectId = linTbl[lineTypeToAssign];

                tr.Commit();
            } // End Trans
                

        } // end CheckLayers()

        // Convert2DPolyToLWPoly: Convert to "LWPOLYLINE" for performance reasons
        public static Polyline Convert2DPolyToLWPoly(Entity ent)
        {
            // Select the currently open AutoCAD drawing and...
            Document doc = Application.DocumentManager.MdiActiveDocument;
            // Open the Drawing Database of Objects in the current drawing
            Database db = doc.Database;
            try
            {
                Polyline poly = new Polyline();
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    // Open the Block table for read
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace],
                                                    OpenMode.ForWrite) as BlockTableRecord;

                    Polyline2d poly2d = (Polyline2d)ent;
                    poly2d.UpgradeOpen();
                    poly.ConvertFrom(poly2d, false);

                    btr.AppendEntity(poly);
                    tr.AddNewlyCreatedDBObject(poly, true);

                    tr.Commit();
                } // End Transaction
                return poly;
            }
            catch
            {
                throw new ApplicationException("2D Polyline to LWPolyline conversion failure");
            }            
        } // End Convert2DPolyToLWPoly()

        // OffsetRoadLine: Offset an LWPOLYLINE as required by the selected Road Line Style
        public static void OffsetRoadLine(Polyline poly, Double offsetBy, string layerToUse)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blkTbl = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord blkTblRec = trans.GetObject(blkTbl[BlockTableRecord.ModelSpace], 
                                                            OpenMode.ForWrite) as BlockTableRecord;
                // Assign LWPOLYLINE to desired Layer
                poly.Layer = layerToUse;
                poly.ColorIndex = 256;

                // Adjust LWPOLYLINE to correct GLOBAL WIDTH
                switch (layerToUse)
                {
                    case "MUTCD-LINE-SOLID-DOUBLE-YELLOW":
                        {
                            poly.ConstantWidth = yellowStripLineWidth;
                            break;
                        }
                    case "MUTCD-LINE-DASHED-YELLOW":
                        {
                            poly.ConstantWidth = yellowStripLineWidth;
                            break;
                        }
                    case "MUTCD-LINE-SOLID-WHITE":
                        {
                            poly.ConstantWidth = whiteStripeLineWidth;
                            break;
                        }
                    case "MUTCD-LINE-DASHED-WHITE":
                        {
                            poly.ConstantWidth = whiteStripeLineWidth;
                            break;
                        }
                    default:
                        {
                            poly.ConstantWidth = 0.0;
                            break;
                        }
                }

                // Negative Double sets new polyline to North-Right of existing Polyline
                // Positive Double sets new polyline to South-Left of existing Polyline
                DBObjectCollection DBObjColl = poly.GetOffsetCurves(offsetBy);

                foreach (Entity ent in DBObjColl)
                {
                    // Add each offset object to Block Table Record and...
                    blkTblRec.AppendEntity(ent);
                    // ...to the AutoCAD Database (aka - the current .dwg) 
                    trans.AddNewlyCreatedDBObject(ent, true);
                }
                trans.Commit();
            }
        } // End OffsetRoadLine()

    } // End class
} // End namespace

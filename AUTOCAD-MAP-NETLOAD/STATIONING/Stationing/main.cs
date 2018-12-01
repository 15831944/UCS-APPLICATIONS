/*
 * Stationing
 * Purpose: Emulate the Civil 3D Intelligent Stationing Object in AutoCAD/AutoCAD Map
 * using a Polyline and/or GIS provided road Centerlines
 * (C) Copyright 2017 Steven H. Brubaker
 */

using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Colors;

[assembly: CommandClass(typeof(Stationing.Main))]

namespace Stationing
{
    public class Main
    {
        // Default values based on Civil 3D defaults
        // Annotations
        public static string prefix = "";
        public static string postfix = "+00";
        public static double txtHeight = 3.0;
        // Tick marks
        public static double tickLength = 4.0;
        public static double incrementDistance = 100.0;

        [CommandMethod("Stationing")]
        public static void DrawTicksAndAnnotate()
        {
            // Standard ACAD.NET handles to current...
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            
            // ... the actual database of objects in the DWG...
            Database db = doc.Database;
            
            // ... the Editor so I can display/write comments to the console
            Editor ed = doc.Editor;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                PromptEntityOptions pmtOpts = new PromptEntityOptions("\nSelect a polyline (PLINE): ");
                pmtOpts.SetRejectMessage("You must use a Polyline since it best models roads.");
                pmtOpts.AddAllowedClass(typeof(Polyline), true);
                pmtOpts.AllowNone = false;

                PromptEntityResult pmtResults = ed.GetEntity(pmtOpts);

                if (pmtResults.Status == PromptStatus.OK)
                {
                    // Create a handle to the selected Polyline
                    Polyline poly = (Polyline)tr.GetObject(pmtResults.ObjectId, OpenMode.ForRead);
            
                    // Get the length of the selected poly...
                    double length = poly.Length;
                    
                    // ... to make sure it is long enough for at least one station
                    int numberOfStations = 0;
                    if (length / incrementDistance > 1.0)
                    {
                        numberOfStations = (int)(length / incrementDistance);
                    }
                    else
                    {
                        ed.WriteMessage("Your selected Polyline is too short. You need at least a " +
                            incrementDistance + "' or longer Polyline for Stationing.");
                        return;
                    }
                    
                    // Check to make sure we have the right layer to assign out Alignment Ticks & Text to
                    CheckLayers("C-ALIGNMENT", 8, "CONTINUOUS");
                    
                    // Open ACAD Database Table (aka BlockTableRecord)
                    var curSpace = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                    
                    // Create a DBText object for modification as we iterate through our stations 
                    // and annotate them with prefix and postfix
                    DBText txt = null;
                    
                    // Initialize our Station Distance tracking variable to the desired Tick mark increment distance
                    double stationingDistance = incrementDistance;

                    for (int i = 0; i < numberOfStations; i++)
                    {
                        // Draw first (zero/start) tick mark and annotate with prefix and postfix and then...
                        if (i == 0)
                        {
                            // See other Tick/Text comments below for details about the following lines of code
                            Point3d p0 = poly.GetPoint3dAt(0);
                            Vector3d angl = poly.GetFirstDerivative(poly.GetParameterAtPoint(p0));
                            angl = angl.GetNormal() * tickLength;
                            angl = angl.TransformBy(Matrix3d.Rotation(Math.PI / 2.0, poly.Normal, Point3d.Origin));
                            Line lin0 = new Line(p0 - angl, p0 + angl);
                            lin0.Layer = "C-ALIGNMENT";
                            curSpace.AppendEntity(lin0);
                            tr.AddNewlyCreatedDBObject(lin0, true);
                    
                            // See other Tick/Text comments below for details about the following lines of code
                            txt = new DBText();
                            txt.Height = txtHeight;
                            txt.Layer = "C-ALIGNMENT";
                            txt.Position = p0 + (2 * angl);
                            txt.VerticalMode = TextVerticalMode.TextVerticalMid;
                            txt.TextString = prefix + "0" + postfix;
                            txt.AlignmentPoint = p0 + (2 * angl);

                            if (lin0.Angle <= Math.PI / 2 || lin0.Angle > (3 * Math.PI) / 2)
                            {
                                txt.HorizontalMode = TextHorizontalMode.TextLeft;
                                txt.Rotation = lin0.Angle;
                            }
                            else
                            {
                                txt.HorizontalMode = TextHorizontalMode.TextRight;
                                txt.Rotation = lin0.Angle + Math.PI;
                            }

                            curSpace.AppendEntity(txt);
                            tr.AddNewlyCreatedDBObject(txt, true);
                        }

                        // ... create tick marks for all the other stations
                        Point3d p1 = poly.GetPointAtDist(stationingDistance);
                        
                        // Find vector angle from point to next point
                        Vector3d ang = poly.GetFirstDerivative(poly.GetParameterAtPoint(p1));
                        
                        // Scale the vector by tickLength (default is 4 units length like Civil 3D 2017 Alignments)
                        ang = ang.GetNormal() * tickLength;
                        
                        // Transformation Matrix application to Rotate the vector so it is perpendicular to the
                        // target point on the selected Polyline object
                        ang = ang.TransformBy(Matrix3d.Rotation(Math.PI / 2.0, poly.Normal, Point3d.Origin));
                        
                        // Create a line by subtracting and adding the vector to the point (aka Transformation Matirx to 
                        // Displace the points for the new lime perpendicular to the selected Polyline)
                        Line line = new Line(p1 - ang, p1 + ang);
                        line.Layer = "C-ALIGNMENT";
                        
                        // Standard ACAD.NET database (sic DWG) code to add & save new line
                        curSpace.AppendEntity(line);
                        tr.AddNewlyCreatedDBObject(line, true);

                        // Stationing annotation text
                        txt = new DBText();
                        txt.Height = txtHeight;
                        txt.Layer = "C-ALIGNMENT";
                        
                        // Place (Displace) text twice as far away as the length (and 'direction' since "ang" is a vector)
                        // of our tick mark
                        txt.Position = p1 + (2 * ang);
                        
                        // Place a grip at middle of our text so we can align it...
                        txt.VerticalMode = TextVerticalMode.TextVerticalMid;
                        
                        // ...to our tick mark and Displace it away from the end of the tick mark
                        txt.AlignmentPoint = p1 + (2 * ang);
                        
                        // Format the current Station Distance so it appears as 1+00, 2+00, etc
                        txt.TextString = prefix + (int)(stationingDistance / incrementDistance) + postfix;
                        
                        // ACAD uses RADIANS, hence the Math.PI. The goal here is to Rotate the text to the most readable angle
                        if (line.Angle <= Math.PI / 2 || line.Angle > (3 * Math.PI) / 2)
                        {
                            // From 0 to 180 degrees, we want the alignment/rotation point to the left of the text so...
                            txt.HorizontalMode = TextHorizontalMode.TextLeft;
                            // ...that it is easy to read when it is perpendicular to the polyline at the current station tick
                            txt.Rotation = line.Angle;
                        }
                        else
                        {
                            // otherwise we need to rotate the text by 180 degrees and align to the right so it is easy to read
                            txt.HorizontalMode = TextHorizontalMode.TextRight;
                            txt.Rotation = line.Angle + Math.PI;
                        }
                        
                        // Standard ACAD.NET database (sic DWG) code to add & save new text
                        curSpace.AppendEntity(txt);
                        tr.AddNewlyCreatedDBObject(txt, true);
                        
                        // Increment our Stationing Distance for the next tick mark ##' along the selected polyline
                        stationingDistance = stationingDistance + incrementDistance;
                    }
                }
                tr.Commit();
            }
        }

        // CheckLayers: Validate that required Layers and Linetypes 
        // are present in the drawing. Add if not present.
        public static void CheckLayers(string layerToCheck, short colorToUse, string lineTypeToAssign)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
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
                    ltr.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByAci, colorToUse);

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
    }       
}
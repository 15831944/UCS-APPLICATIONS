// (C) Copyright 2016 by Steven H. Brubaker
// Purpose: Use GIS Centerline data to automate the creation of Road Lines
// for use in Traffic Management Plans for municipalities that will not accept
// Aerial Imagery as a background.

using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

[assembly: CommandClass(typeof(UCS_Road_Lines.ACAD_Commands))]

namespace UCS_Road_Lines
{
    public class ACAD_Commands
    {
        public static Autodesk.AutoCAD.Windows.PaletteSet _ps = null;

        [CommandMethod("Road_Style_Picker")]
        public void MyCommand()
        {
            if (_ps != null)
            {
                _ps.Visible = true;
            }
            else
            {
                _ps = new Autodesk.AutoCAD.Windows.PaletteSet("UCS Road Centerline Tool");
                _ps.Size = new Size(600, Math.Max(350,_ps.Size.Height));
                _ps.Visible = true;
                _ps.DockEnabled = (Autodesk.AutoCAD.Windows.DockSides)
                    ((int)Autodesk.AutoCAD.Windows.DockSides.Left +
                    (int)Autodesk.AutoCAD.Windows.DockSides.Right);
                _ps.Dock = Autodesk.AutoCAD.Windows.DockSides.Left;
                _ps.KeepFocus = false;

                Help helpUC = new UCS_Road_Lines.Help();
                ElementHost helpEH = new ElementHost();
                helpEH.AutoSize = true;
                helpEH.Dock = DockStyle.Fill;
                helpEH.Child = helpUC;
                helpEH.Name = "How to";
                
                _ps.Add("How to...", helpEH);

                OneLaneRoadUserControl oneLane = new OneLaneRoadUserControl();
                ElementHost singleLanes = new ElementHost();
                singleLanes.AutoSize = true;
                singleLanes.Dock = DockStyle.Fill;
                singleLanes.Child = oneLane;
                singleLanes.Name = "EH_1L";
                _ps.Add("One Lane Roads", singleLanes);

                TwoLaneRoadsUserControl twoLane = new TwoLaneRoadsUserControl();
                ElementHost twoLanes = new ElementHost();
                twoLanes.AutoSize = true;
                twoLanes.Dock = DockStyle.Fill;
                twoLanes.Child = twoLane;
                twoLanes.Name = "EH_2L";
                _ps.Add("Two Lane Roads", twoLanes);

                ThreeLaneRoads threeLane = new UCS_Road_Lines.ThreeLaneRoads();
                ElementHost threeLanes = new ElementHost();
                threeLanes.AutoSize = true;
                threeLanes.Dock = DockStyle.Fill;
                threeLanes.Child = threeLane;
                threeLane.Name = "EH_3L";
                _ps.Add("Three Lane Roads", threeLanes);

                FourLaneRoads fourLane = new UCS_Road_Lines.FourLaneRoads();
                ElementHost fourLanesEH = new ElementHost();
                fourLanesEH.AutoSize = true;
                fourLanesEH.Dock = DockStyle.Fill;
                fourLanesEH.Child = fourLane;
                fourLane.Name = "EH_4L";
                _ps.Add("Four Lane Roads", fourLanesEH);

                FiveLaneRoads fiveLane = new UCS_Road_Lines.FiveLaneRoads();
                ElementHost fiveLaneEH = new ElementHost();
                fiveLaneEH.AutoSize = true;
                fiveLaneEH.Dock = DockStyle.Fill;
                fiveLaneEH.Child = fiveLane;
                fiveLane.Name = "EH_5L";
                _ps.Add("Five Lane Roads", fiveLaneEH);

                SixLaneRoads sixLane = new UCS_Road_Lines.SixLaneRoads();
                ElementHost sixLaneEH = new ElementHost();
                sixLaneEH.AutoSize = true;
                sixLaneEH.Dock = DockStyle.Fill;
                sixLaneEH.Child = sixLane;
                sixLane.Name = "EH_6L";
                _ps.Add("Six Lane Roads", sixLaneEH);
            }
        }

        [CommandMethod("Paint_Road_Lines")]
        public void PolyOffset()
        {
            // Select the currently open AutoCAD drawing and...
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            // Open the Drawing Database of Objects in the current drawing
            Database db = doc.Database;

            // Provide feedback to the client (plus Developer during debug)
            Editor ed = doc.Editor;

            // Multi-Selection Set generation
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Polyline poly = new Polyline();

                // Ask client to select Road Centerline(s) to convert
                PromptSelectionResult SSPrompt = doc.Editor.GetSelection();

                // Verify they actually selected anything
                if (SSPrompt.Status == PromptStatus.OK)
                {
                    // Create a Selection Set incase the client selected more than one Object
                    SelectionSet SSet = SSPrompt.Value;

                    // Cycle through the selected objects to...
                    foreach (SelectedObject SSObj in SSet)
                    {
                        // Verify that we have a valid Object
                        if (SSObj != null)
                        {
                            // Convert selected Object(s) to Entity and open it to READ ONLY to see what it is
                            Entity ent = trans.GetObject(SSObj.ObjectId, OpenMode.ForWrite) as Entity;

                            // If it is not a blank (null) record in the database for the retrieved
                            // ObjectID in the database (db) of the currently open drawing (Document (doc))
                            if (ent != null)
                            {
                                // Check dwg for required layers
                                /*
                                 Name                               Color   Global Width    LineType
                                 MUTCD-LINE-DASHED-WHITE            9       0.41            ACAD_ISO03W100
                                 MUTCD-LINE-DASHED-YELLOW           9       0.33            ACAD_ISO03W100
                                 MUTCD-LINE-SOLID-DOUBLE-YELLOW     52      0.33            CONTINUOUS
                                 MUTCD-LINE-SOLID-WHITE             9       0.41            CONTINUOUS
                                 T-EOP                              8       0.0             CONTINUOUS
                                 T-ROAD-CNTR                        2       0.0             CENTER
                                 */
                                utils.CheckLayers("MUTCD-LINE-DASHED-WHITE", 9, "ACAD_ISO03W100");
                                utils.CheckLayers("MUTCD-LINE-DASHED-YELLOW", 52, "ACAD_ISO03W100");
                                utils.CheckLayers("MUTCD-LINE-SOLID-DOUBLE-YELLOW", 52, "CONTINUOUS");
                                utils.CheckLayers("MUTCD-LINE-SOLID-WHITE", 9, "CONTINUOUS");
                                utils.CheckLayers("T-EOP", 8, "CONTINUOUS");
                                utils.CheckLayers("T-ROAD-CNTR", 2, "CENTER");


                                // Extract the full (string) name of the Object Type
                                // since SWITCH does not support Object Types as a parameter
                                string typeEnt = ent.GetType().ToString();

                                // Determine if we have our POLYLINE Centerline for the road or
                                // if Conversion can be done to make it a POLYLINE. (Note: AutoCAD Polylines 
                                // support arc's and Global Width's required to simulate roads and their MUTCD 
                                // compliant painted lane markings.)
                                switch (typeEnt)
                                {
                                    // LWPOLYLINE (Lightweight Polyline): Used with OFFSET to draw & mark our simulated road
                                    case "Autodesk.AutoCAD.DatabaseServices.Polyline":
                                        {
                                            poly = (Polyline)ent;
                                            break;
                                        }

                                    // POLYLINE2D (Heavy Polyline): Could be used 'as is' but will be converted to 
                                    // the newer "LWPOLYLINE" for performance reasons
                                    case "Autodesk.AutoCAD.DatabaseServices.Polyline2d":
                                        {
                                            poly = utils.Convert2DPolyToLWPoly(ent);
                                            break;
                                        }

                                    // LINE: Create a new polyline and delete existing line
                                    case "Autodesk.AutoCAD.DatabaseServices.Line":
                                        {
                                            using (Transaction acTrans = db.TransactionManager.StartTransaction())
                                            {
                                                // Cast our Entity ent to a LINE
                                                Line lin = (Line)ent;

                                                // Open the Block table for read
                                                BlockTable acBlkTbl;
                                                acBlkTbl = acTrans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                                                // Open the Block table record Model space for write
                                                BlockTableRecord acBlkTblRec;
                                                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                                                OpenMode.ForWrite) as BlockTableRecord;

                                                // Create the replacement polyline that supports GLOBAL WIDTH for MUTCD Road Lines
                                                using (Polyline acPoly = new Polyline())
                                                {
                                                    acPoly.AddVertexAt(0, new Point2d(lin.StartPoint.X, lin.StartPoint.Y), 0, 0, 0);
                                                    acPoly.AddVertexAt(1, new Point2d(lin.EndPoint.X, lin.EndPoint.Y), 0, 0, 0);

                                                    // Add the new object to the block table record and the transaction
                                                    acBlkTblRec.AppendEntity(acPoly);
                                                    acTrans.AddNewlyCreatedDBObject(acPoly, true);
                                                    poly = acPoly;
                                                }
                                                // Save the new object to the database
                                                acTrans.Commit();
                                            }
                                            break;
                                        }

                                    // Not an Entity that can be converted quickly or directly to an LWPOLYLINE
                                    default:
                                        {
                                            ed.WriteMessage("You have selected an object that cannot be quickly " + 
                                                "converted to a POLYLINE such as an ARC, CIRCLE, 3D POLYLINE or BLOCK. " + 
                                                "\nExcept for BLOCKS, you can run 'DRAWING CLEANUP...' (Command: MAPCLEAN) " +
                                                "to convert them to LWPOLYLINE's that are needed by this application." );
                                            break;
                                        }
                                } // End Switch

                                if(poly != null)
                                {
                                    switch (utils.nrLanes)
                                    {                                        
                                        case 1:
                                            {
                                                road_styles.OneLaneRoads(poly);
                                                break;
                                            }
                                        case 2:
                                            {
                                                road_styles.TwoLaneRoads(poly);
                                                break;
                                            }
                                        case 3:
                                            {
                                                road_styles.ThreeLaneRoads(poly);
                                                break;
                                            }
                                        case 4:
                                            {
                                                road_styles.FourLaneRoads(poly);
                                                break;
                                            }
                                        case 5:
                                            {
                                                road_styles.FiveLaneRoads(poly);
                                                break;
                                            }
                                        case 6:
                                            {
                                                road_styles.SixLaneRoads(poly);
                                                break;
                                            }
                                        default:
                                            {
                                                ed.WriteMessage("Ooops... Something went wrong with the number of lanes.");
                                                break;
                                            }
                                    } // End Switch(utils.nrLanes)
                                    poly.Layer = "T-ROAD-CNTR";
                                    poly.ConstantWidth = 0.0;
                                } // End If(poly != null)
                            } // End If (Ent != null)
                        } // End if (SSObj != null)
                    } // End Foreach
                    trans.Commit();
                } // End If (Verify they actually selected anything)
            } // End Using (Multi-select set generation)
        } // End PolyOffset()

        [CommandMethod("CLW")]
        public void ChangeLaneWidth()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            utils.laneWidth = GetDoubleValue(utils.laneWidth, "Lane Width");
            if(utils.laneWidth > 15.0)
            {
                if (utils.roadStyle != "1L-NM " || utils.roadStyle != "2L-NM" || 
                    utils.roadStyle != "2L-CL" || utils.roadStyle != "2L-DY")
                {
                    utils.laneWidth = 15.0;
                    ed.WriteMessage("\nLane Width reduced to US normal max of 15'.");
                }
            }

            if(utils.laneWidth < 6.7)
            {
                utils.laneWidth = 6.7;
                ed.WriteMessage("\nLane Width increased to US normal min of 6.7' (but 8' would really be a better choice).");
            }
        } // End ChangeLaneWidth()

        [CommandMethod("MEOP")]
        public void ChangeMedianWidth()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                bool bEOPStatus = true;
                PromptKeywordOptions pKeyWord = new PromptKeywordOptions("");
                pKeyWord.Message = "\nChange Left, Right or Both EOP/Shoulder Widths or leave 'as is' (Default) ";
                pKeyWord.AllowNone = true;
                pKeyWord.Keywords.Add("Left");
                pKeyWord.Keywords.Add("Right");
                pKeyWord.Keywords.Add("Both");
                pKeyWord.Keywords.Add("Default");
                pKeyWord.Keywords.Default = "Default";

                PromptResult pKeyRes = doc.Editor.GetKeywords(pKeyWord);

                if (pKeyRes.Status == PromptStatus.OK)
                {
                    switch (pKeyRes.StringResult)
                    {
                        case "Left":
                            {
                                utils.leftMedianWidth = GetDoubleValue(utils.leftMedianWidth, "\nLeft Median Width");
                                break;
                            }
                        case "Right":
                            {
                                double tempRight = Math.Abs(utils.rightMedianWidth);
                                utils.rightMedianWidth = GetDoubleValue(tempRight, "\nRight Median Width") * -1.0;
                                break;
                            }
                        case "Both":
                            {
                                double tempRight = 0.5;
                                tempRight = GetDoubleValue(tempRight, "\nUpdate both Median width's to");
                                utils.leftMedianWidth = tempRight;
                                utils.rightMedianWidth = tempRight * -1.0;
                                break;
                            }
                        default:
                            {
                                bEOPStatus = false;
                                if (utils.drawEOP)
                                {
                                    ed.WriteMessage("\nMedian widths not changed.\nLeft Median = " + Math.Abs(utils.leftMedianWidth).ToString() +
                                        " | | Right Median = " + Math.Abs(utils.rightMedianWidth).ToString());
                                }
                                else
                                {
                                    ed.WriteMessage("\nEdge of Pavement and Median lines have been turned off. Use DEOP to turn them back on.");
                                }
                                break;
                            }
                    }
                    if (bEOPStatus)
                    {
                        // Update Command Console with current EOP Width/Status
                        if (utils.drawEOP)
                        {
                            ed.WriteMessage("\nLeft Median = " + Math.Abs(utils.leftMedianWidth).ToString() +
                                " | | Right Median = " + Math.Abs(utils.rightMedianWidth).ToString());
                        }
                        else
                        {
                            ed.WriteMessage("\nEdge of Pavement and Median lines have been turned off. Use DEOP to turn them back on.");
                        }
                    }
                }
                tr.Commit();
            }
        } // End ChangeMedianWidth()

        [CommandMethod("CEOP")]
        public void ChangeEOPWidth()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                bool bEOPStatus = true;
                PromptKeywordOptions pKeyWord = new PromptKeywordOptions("");
                pKeyWord.Message = "\nChange Left, Right or Both EOP/Shoulder Widths or leave 'as is' (Default) ";
                pKeyWord.AllowNone = true;
                pKeyWord.Keywords.Add("Left");
                pKeyWord.Keywords.Add("Right");
                pKeyWord.Keywords.Add("Both");
                pKeyWord.Keywords.Add("Default");
                pKeyWord.Keywords.Default = "Default";

                PromptResult pKeyRes = doc.Editor.GetKeywords(pKeyWord);

                if (pKeyRes.Status == PromptStatus.OK)
                {
                    switch(pKeyRes.StringResult)
                    {
                        case "Left":
                            {
                                utils.leftShoulderWidth = GetDoubleValue(utils.leftShoulderWidth, "\nLeft EOP Width");
                                break;
                            }
                        case "Right":
                            {
                                double tempRight = Math.Abs(utils.rightShoulderWidth);
                                utils.rightShoulderWidth = GetDoubleValue(tempRight, "\nRight EOP Width") * -1.0;
                                break;
                            }
                        case "Both":
                            {
                                double tempRight = 0.5;
                                tempRight = GetDoubleValue(tempRight, "\nUpdate both EOP width's to");
                                utils.leftShoulderWidth = tempRight;
                                utils.rightShoulderWidth = tempRight * -1.0;
                                break;
                            }
                        default:
                            {
                                bEOPStatus = false;
                                if (utils.drawEOP)
                                {
                                    ed.WriteMessage("\nEOP widths not changed.\nLeft EOP = " + Math.Abs(utils.leftShoulderWidth).ToString() +
                                        " | | Right EOP = " + Math.Abs(utils.rightShoulderWidth).ToString());
                                }
                                else
                                {
                                    ed.WriteMessage("\nEdge of Pavement lines have been turned off. Use DEOP to turn them back on.");
                                }
                                break;
                            }
                    }
                    if (bEOPStatus)
                    {
                        // Update Command Console with current EOP Width/Status
                        if (utils.drawEOP)
                        {
                            ed.WriteMessage("\nLeft EOP = " + Math.Abs(utils.leftShoulderWidth).ToString() +
                                " | | Right EOP = " + Math.Abs(utils.rightShoulderWidth).ToString());
                        }
                        else
                        {
                            ed.WriteMessage("\nEdge of Pavement lines have been turned off. Use DEOP to turn them back on.");
                        }
                    }
                }
                tr.Commit();
            }
        } // End ChangeEOPWidth()

        [CommandMethod("DEOP")]
        public void DrawEOPs()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                PromptKeywordOptions pKeyWord = new PromptKeywordOptions("");
                pKeyWord.Message = "Draw EOP lines ";
                pKeyWord.AllowNone = true;
                pKeyWord.Keywords.Add("Yes");
                pKeyWord.Keywords.Add("No");
                pKeyWord.Keywords.Default = "Yes";

                PromptResult pKeyRes = doc.Editor.GetKeywords(pKeyWord);

                if (pKeyRes.Status == PromptStatus.OK)
                {
                    switch (pKeyRes.StringResult)
                    {
                        case "No":
                            {
                                utils.drawEOP = false;
                                ed.WriteMessage("\nEdge of Pavement lines have been turned off. Use DEOP again to turn them back on.");
                                break;
                            }
                        case "Yes":
                            {
                                utils.drawEOP = true;
                                ed.WriteMessage("\nLeft EOP = " + Math.Abs(utils.leftShoulderWidth).ToString() +
                                        " | | Right EOP = " + Math.Abs(utils.rightShoulderWidth).ToString());
                                break;
                            }
                        default:
                            {
                                if (utils.drawEOP)
                                {
                                    ed.WriteMessage("\nLeft EOP = " + Math.Abs(utils.leftShoulderWidth).ToString() +
                                        " | | Right EOP = " + Math.Abs(utils.rightShoulderWidth).ToString());
                                }
                                else
                                {
                                    ed.WriteMessage("\nEdge of Pavement lines have been turned off. Use CEOP again to turn them back on.");
                                }
                                break;
                            }
                    }
                }
                tr.Commit();
            }
        } // End ChangeEOPWidth()


        public double GetDoubleValue(double dblValue, string dblToChanged)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;

            PromptDoubleOptions pDbleOpts = new PromptDoubleOptions("Enter the new amount ");
            pDbleOpts.AllowZero = true;
            pDbleOpts.AllowNegative = false;
            pDbleOpts.AllowNone = false;
            pDbleOpts.DefaultValue = dblValue;

            PromptDoubleResult pDblRes = ed.GetDouble(pDbleOpts);
            if (pDblRes.Status == PromptStatus.OK)
            {
                ed.WriteMessage(dblToChanged + " = " + Math.Abs(pDblRes.Value).ToString());
                return pDblRes.Value;
            }
            else
            {
                ed.WriteMessage(dblToChanged + " = " + Math.Abs(dblValue).ToString());
                return dblValue;
            }
        }
    } // End Class
} // End namespace

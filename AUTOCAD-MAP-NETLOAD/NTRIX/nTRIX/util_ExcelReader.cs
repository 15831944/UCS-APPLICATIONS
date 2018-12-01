/* (C) Copyright 2017 by Steven H. Brubaker

Purpose: Read Microsoft Excel Worksheet to get Names of Blocks and Layers per UCS Standards
to use in util_Entity to place Blocks on DXF Shotpoints.

Credit and great appreciation goes to: https://github.com/JanKallman/EPPlus - I have used EPPlus
(aka: OfficeOpenXml) in place of my Visual LISP routines to read from Excel Worksheets.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace nTRIX
{
    class util_ExcelReader
    {
        // Layers to check and display to Drafter if any are missing
        public static List<string> missingLayers = new List<string>();
        // Blocks to check and display to Drafter if any are missing
        public static List<string> missingBlocks = new List<string>();

        // Check current DWG for required Layers
        public static void ValidateLayers(string fileName, string wsTabToUse)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (ExcelPackage p = new ExcelPackage())
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    p.Load(stream);

                    ExcelWorksheet ws = p.Workbook.Worksheets[wsTabToUse];

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        // Open the Layer table for read
                        LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;

                        int rowIndex = 2;
                        string layerToCheck = ws.Cells[rowIndex, 1].Value.ToString();
                        while (layerToCheck != "ZZZ-END-ZZZ")
                        {
                            // Check if Layer is in DWG Layer Table
                            if (lt.Has(layerToCheck) != true)
                            {
                                missingLayers.Add(layerToCheck);
                                ed.WriteMessage("Please add this Layer: " + layerToCheck + "\n");
                            }
                            rowIndex++;
                            layerToCheck = ws.Cells[rowIndex, 1].Value.ToString();
                        }
                        tr.Commit();
                    }
                }
            }
        }

        // Check current DWG for required Blocks
        public static void ValidateBlocks(string fileName, string wsTabToUse)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (ExcelPackage p = new ExcelPackage())
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    p.Load(stream);

                    ExcelWorksheet ws = p.Workbook.Worksheets[wsTabToUse];

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                        int rowIndex = 2;

                        string blockToCheck = ws.Cells[rowIndex, 1].Value.ToString();
                        while (blockToCheck != "ZZZ-END-ZZZ")
                        {
                            // Check if Block is in DWG Block Table
                            if (bt.Has(blockToCheck) != true)
                            {
                                missingBlocks.Add(blockToCheck);
                                ed.WriteMessage("Please add this Block: " + blockToCheck + "\n");
                            }
                            rowIndex++;
                            blockToCheck = ws.Cells[rowIndex, 1].Value.ToString();
                        }
                        tr.Commit();
                    }
                }
            }
        }

        // Read in Excel DXF tab into look-up DataTable "lkAbbrevTbl"
        public static void ReadInDXFDataTable(string fileName, string wsTabToUse)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (ExcelPackage p = new ExcelPackage())
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    p.Load(stream);

                    ExcelWorksheet ws = p.Workbook.Worksheets[wsTabToUse];

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        // Check to see if DataTable already exists
                        if (!MyCommands.lkAbbrevTbl.Columns.Contains("ANNO"))
                        {
                            // Field Crew Annotation = Type of structure that will be used...
                            MyCommands.lkAbbrevTbl.Columns.Add("ANNO", typeof(string)); // COLUMN 1
                            // ...with this Block in the DWG
                            MyCommands.lkAbbrevTbl.Columns.Add("BLOCK", typeof(string)); // COLUMN 2
                            // If not "NA", this is the Value to use to set the Dynamic Block...
                            MyCommands.lkAbbrevTbl.Columns.Add("DYN_VAL", typeof(string)); // COLUMN 3
                            // ...on this Layer
                            MyCommands.lkAbbrevTbl.Columns.Add("LAYER", typeof(string)); // COLUMN 4
                            // If the Block has ATTRIBUTES, this Cell is used to identify the first...
                            MyCommands.lkAbbrevTbl.Columns.Add("ATTRIB1", typeof(string)); // COLUMN 5
                            // ...and this identifies the second (if applicable).
                            MyCommands.lkAbbrevTbl.Columns.Add("ATTRIB2", typeof(string)); // COLUMN 6
                            // If this is a DYNAMIC BLOCK, this is the Property Name of the
                            // the DynamicBlockReferenceProperty used to control visibilty for
                            // the nTRIX Blocks (ie -> MH-VZ == Verizon)
                            MyCommands.lkAbbrevTbl.Columns.Add("PROP_NAME", typeof(string)); // COLUMN 7

                            System.Data.DataRow row;

                            int rowIndex = 2;
                            string rowToAdd = ws.Cells[rowIndex, 1].Value.ToString();
                            while (rowToAdd != "ZZZ-END-ZZZ")
                            {
                                row = MyCommands.lkAbbrevTbl.NewRow();
                                row["ANNO"] = ws.Cells[rowIndex, 1].Value.ToString();
                                row["BLOCK"] = ws.Cells[rowIndex, 2].Value.ToString();
                                row["DYN_VAL"] = ws.Cells[rowIndex, 3].Value.ToString();
                                row["LAYER"] = ws.Cells[rowIndex, 4].Value.ToString();
                                row["ATTRIB1"] = ws.Cells[rowIndex, 5].Value.ToString();
                                row["ATTRIB2"] = ws.Cells[rowIndex, 6].Value.ToString();
                                row["PROP_NAME"] = ws.Cells[rowIndex, 7].Value.ToString();

                                MyCommands.lkAbbrevTbl.Rows.Add(row);

                                rowIndex++;
                                rowToAdd = ws.Cells[rowIndex, 1].Value.ToString();
                            }
                        }
                    }
                }
            }
        }

    }
}

// (C) Copyright 2017 Steven H. Brubaker
// Purpose: Improvment and replacement of UCS's CAD Dept's TRIX LISP
// to convert DTEXT objects provided by UCS's Field Crew in a DXF format
// into the appropriate Block Reference and/or annotation

using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

[assembly: CommandClass(typeof(nTRIX.MyCommands))]

namespace nTRIX
{
    public class MyCommands
    {
        // Modal Dialog Form for nTRIX Application
        public static nTrixMainForm m_ps = null;

        // In-memory Database of DXF abbreviations. Used for look-up of appropriate Block & Layer to
        // use when placing the Block into the DWG using a Layer 0 DBTEXT Object's insertion point.
        public static System.Data.DataTable lkAbbrevTbl = new System.Data.DataTable();

        // Path to Excel File called "nTRIX.XLSX"
        public static string ntrixFilePath = "N:\\CADD\\CAD STANDARDS - UNIVERSAL\\LISPS\\NTRIX\\nTRIX.xlsx";

        // Path to Excel File called "nTRIX.XLSX"
        public static string tempnTRIXFile = "";

        // Allow Drafters to manually move SHOTPOINTS (used as a self-tracking QC technique)
        public static Boolean moveToShotpointsLayer = true;

        // Allow Drafters to manually move SHOTPOINTS (used as a self-tracking QC technique)
        public static Boolean useOldTRIXLayersAndBlocks = false;

        // Allow Drafters to use the current UCS to set the Rotation/Orientation of the Blocks
        public static Boolean useCurrentUCS_Rotation = true;

        // Tracking List<string> variables to make sure the DWG has the
        // required LAYERs and BLOCKs
        /*
         * util_ExcelReader.missingLayers
         * util_ExcelReader.missingBlocks
         */

        [CommandMethod("nTRIX")]
        public void MyCommand()
        {
            // Standard ACAD.NET handles to current...
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            // ... the actual database of objects in the DWG...
            Database db = doc.Database;
            // ... the Editor so I can display/write comments to the console
            Editor ed = doc.Editor;

            // Display the Windows Form for the Drater to interact with the 
            // AutoCAD hosted application
            if (m_ps == null)
            {
                m_ps = new nTrixMainForm();
            }
            Application.ShowModalDialog(m_ps);
        }
    }
}

// (C) Copyright 2017 by Steven H. Brubaker

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace nTRIX
{
    public partial class nTrixMainForm : Form
    {
        string fileName = "";
        public nTrixMainForm()
        {
            InitializeComponent();
            if (!File.Exists(MyCommands.ntrixFilePath))
            {
                if(openExcel.ShowDialog() == DialogResult.OK)
                {
                    MyCommands.ntrixFilePath = openExcel.FileName;
                }
            }
        }

        private void btnRunNTRIX_Click(object sender, EventArgs e)
        {
            CheckState cState = cbUsedShotPoints.CheckState;
            switch (cState)
            {
                case CheckState.Checked:
                    MyCommands.moveToShotpointsLayer = false;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    MyCommands.moveToShotpointsLayer = true;
                    break;
            }

            CheckState useOldTRIX = cb_oTRIX.CheckState;
            switch (useOldTRIX)
            {
                case CheckState.Checked:
                    MyCommands.useOldTRIXLayersAndBlocks = true;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    MyCommands.useOldTRIXLayersAndBlocks = false;
                    break;
            }

            if (File.Exists(MyCommands.ntrixFilePath))
            {
                try
                {
                    // Add unique identifier to the temp file (this avoids the 'File in use' Exception)
                    fileName = System.Net.Dns.GetHostName() + "-nTRIX.xls";
                    string targetPath = @"N:\CADD\CAD STANDARDS - UNIVERSAL\Lisps\NTRIX\TEMP";
                    MyCommands.tempnTRIXFile = Path.Combine(targetPath, fileName);

                    // If necessary, create a new target folder
                    if (!Directory.Exists(targetPath))
                    {
                        Directory.CreateDirectory(targetPath);
                    }

                    // Copy our temp file to another location and 
                    // overwrite the destination file if it already exists.
                    File.Copy(MyCommands.ntrixFilePath, MyCommands.tempnTRIXFile, true);

                }
                catch
                {
                    // Warn Drafter that the App is...
                    string message = "Unable to locate nTRIX.xls. Please check: \n N:\\CADD\\CAD STANDARDS - UNIVERSAL\\LISPS\\NTRIX\\nTRIX.xlsx";
                    string caption = "nTRIX.xlx Not Found";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result = MessageBox.Show(message, caption, buttons);
                    Close();
                }

                if (MyCommands.useOldTRIXLayersAndBlocks == false)
                {
                    // Check DWG for required Layers
                    util_ExcelReader.ValidateLayers(MyCommands.tempnTRIXFile, "LAYERS");

                    // Check DWG for required Blocks
                    util_ExcelReader.ValidateBlocks(MyCommands.tempnTRIXFile, "BLOCKS");
                }
                else
                {
                    // Check DWG for required Layers
                    util_ExcelReader.ValidateLayers(MyCommands.tempnTRIXFile, "oTRIX_LAYERS");

                    // Check DWG for required Blocks
                    util_ExcelReader.ValidateBlocks(MyCommands.tempnTRIXFile, "oTRIX_BLOCKS");
                }
            }
            else
            {
                // Warn Drafter that they are...
                string message = "Unable to locate nTRIX.xls. Please check: \n N:\\CADD\\CAD STANDARDS - UNIVERSAL\\LISPS\\NTRIX\\nTRIX.xlsx";
                string caption = "nTRIX.xlx Not Found";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result = MessageBox.Show(message, caption, buttons);
                CleanUpTempFile();
                MyCommands.lkAbbrevTbl = new System.Data.DataTable(); 
                Close();
            }

            // ValidateBlocks and ValidateLayers (above) instantiated the variables 
            // missingBlocks and missingLayers (ie -> not null) so we only have to check their Count
            if (util_ExcelReader.missingBlocks.Count == 0 && util_ExcelReader.missingLayers.Count == 0)
            {
                if (MyCommands.useOldTRIXLayersAndBlocks == false)
                {
                    util_ExcelReader.ReadInDXFDataTable(MyCommands.tempnTRIXFile, "DXF");
                    util_Entity.ProcessDXFEntities();
                    CleanUpTempFile();
                    MyCommands.lkAbbrevTbl = new System.Data.DataTable();
                    Close();
                }
                else
                {
                    util_ExcelReader.ReadInDXFDataTable(MyCommands.tempnTRIXFile, "oTRIX_DXF");
                    util_Entity.ProcessDXFEntities();
                    CleanUpTempFile();
                    MyCommands.lkAbbrevTbl = new System.Data.DataTable();
                    Close();
                }
            }
            else
            {
                // Warn Drafter that they are...
                string message = "You are missing required Layers and/or Blocks in this drawing. Exiting nTRIX now. Check AutoCAD CMD Line for list (Press F2 for CMD Window)";
                string caption = "Missing Layers and/or Blocks";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result = MessageBox.Show(message, caption, buttons);
                CleanUpTempFile();
                MyCommands.lkAbbrevTbl = new System.Data.DataTable();
                Close();
            }
        }

        private void CleanUpTempFile()
        {
            // Delete a file by using File class static method...
            if (System.IO.File.Exists(MyCommands.tempnTRIXFile))
            {
                // IOException possible if the file is already
                // opened by another process so...
                try
                {
                    System.IO.File.Delete(MyCommands.tempnTRIXFile);
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }

        }

        private void rBtnAlignBlocks_CheckedChanged(object sender, EventArgs e)
        {
            foreach(Control ctr in this.gBoxAlignBlock.Controls)
            {
                if (ctr is RadioButton)
                {
                    if(rBtnUCS.Checked)
                    {
                        MyCommands.useCurrentUCS_Rotation = true;
                    }
                    else if(rBtnDBText.Checked)
                    {
                        MyCommands.useCurrentUCS_Rotation = false;
                    }
                }
            }
        }
    }
}

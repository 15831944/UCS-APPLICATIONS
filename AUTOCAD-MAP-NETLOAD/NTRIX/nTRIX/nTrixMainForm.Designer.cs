namespace nTRIX
{
    partial class nTrixMainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(nTrixMainForm));
            this.openExcel = new System.Windows.Forms.OpenFileDialog();
            this.lblUsedShotpoints = new System.Windows.Forms.Label();
            this.cbUsedShotPoints = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRunNTRIX = new System.Windows.Forms.Button();
            this.cb_oTRIX = new System.Windows.Forms.CheckBox();
            this.lbl_oTRIX = new System.Windows.Forms.Label();
            this.rBtnUCS = new System.Windows.Forms.RadioButton();
            this.rBtnDBText = new System.Windows.Forms.RadioButton();
            this.gBoxAlignBlock = new System.Windows.Forms.GroupBox();
            this.gBoxAlignBlock.SuspendLayout();
            this.SuspendLayout();
            // 
            // openExcel
            // 
            this.openExcel.DefaultExt = "xlsx";
            this.openExcel.FileName = "nTRIX.xlsx";
            this.openExcel.Filter = "\"Excel files (*.xlxs)|*.xlsx";
            this.openExcel.InitialDirectory = "N:\\CADD\\CAD STANDARDS - UNIVERSAL\\Lisps\\NTRIX";
            this.openExcel.Title = "Locate nTRIX required Excel spreadsheet";
            // 
            // lblUsedShotpoints
            // 
            this.lblUsedShotpoints.AutoSize = true;
            this.lblUsedShotpoints.Location = new System.Drawing.Point(16, 12);
            this.lblUsedShotpoints.Name = "lblUsedShotpoints";
            this.lblUsedShotpoints.Size = new System.Drawing.Size(185, 13);
            this.lblUsedShotpoints.TabIndex = 0;
            this.lblUsedShotpoints.Text = "Use the \"SHOTPOINTS-USED\" layer";
            // 
            // cbUsedShotPoints
            // 
            this.cbUsedShotPoints.AutoSize = true;
            this.cbUsedShotPoints.Checked = true;
            this.cbUsedShotPoints.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUsedShotPoints.Location = new System.Drawing.Point(218, 13);
            this.cbUsedShotPoints.Name = "cbUsedShotPoints";
            this.cbUsedShotPoints.Size = new System.Drawing.Size(15, 14);
            this.cbUsedShotPoints.TabIndex = 1;
            this.cbUsedShotPoints.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Location = new System.Drawing.Point(16, 152);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(256, 128);
            this.label1.TabIndex = 2;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // btnRunNTRIX
            // 
            this.btnRunNTRIX.Font = new System.Drawing.Font("Calibri", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRunNTRIX.Location = new System.Drawing.Point(16, 292);
            this.btnRunNTRIX.Name = "btnRunNTRIX";
            this.btnRunNTRIX.Size = new System.Drawing.Size(253, 54);
            this.btnRunNTRIX.TabIndex = 3;
            this.btnRunNTRIX.Text = "Run nTRIX on my drawing";
            this.btnRunNTRIX.UseVisualStyleBackColor = true;
            this.btnRunNTRIX.Click += new System.EventHandler(this.btnRunNTRIX_Click);
            // 
            // cb_oTRIX
            // 
            this.cb_oTRIX.AutoSize = true;
            this.cb_oTRIX.Location = new System.Drawing.Point(218, 41);
            this.cb_oTRIX.Margin = new System.Windows.Forms.Padding(2);
            this.cb_oTRIX.Name = "cb_oTRIX";
            this.cb_oTRIX.Size = new System.Drawing.Size(15, 14);
            this.cb_oTRIX.TabIndex = 4;
            this.cb_oTRIX.UseVisualStyleBackColor = true;
            // 
            // lbl_oTRIX
            // 
            this.lbl_oTRIX.AutoSize = true;
            this.lbl_oTRIX.Location = new System.Drawing.Point(16, 41);
            this.lbl_oTRIX.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_oTRIX.Name = "lbl_oTRIX";
            this.lbl_oTRIX.Size = new System.Drawing.Size(163, 13);
            this.lbl_oTRIX.TabIndex = 5;
            this.lbl_oTRIX.Text = "Use Old TRIX Layers and Blocks";
            // 
            // rBtnUCS
            // 
            this.rBtnUCS.AutoSize = true;
            this.rBtnUCS.Checked = true;
            this.rBtnUCS.Location = new System.Drawing.Point(15, 23);
            this.rBtnUCS.Name = "rBtnUCS";
            this.rBtnUCS.Size = new System.Drawing.Size(100, 17);
            this.rBtnUCS.TabIndex = 8;
            this.rBtnUCS.TabStop = true;
            this.rBtnUCS.Text = "My current UCS";
            this.rBtnUCS.UseVisualStyleBackColor = true;
            this.rBtnUCS.CheckedChanged += new System.EventHandler(this.rBtnAlignBlocks_CheckedChanged);
            // 
            // rBtnDBText
            // 
            this.rBtnDBText.AutoSize = true;
            this.rBtnDBText.Location = new System.Drawing.Point(15, 51);
            this.rBtnDBText.Name = "rBtnDBText";
            this.rBtnDBText.Size = new System.Drawing.Size(94, 17);
            this.rBtnDBText.TabIndex = 9;
            this.rBtnDBText.Text = "Shotpoint Text";
            this.rBtnDBText.UseVisualStyleBackColor = true;
            this.rBtnDBText.CheckedChanged += new System.EventHandler(this.rBtnAlignBlocks_CheckedChanged);
            // 
            // gBoxAlignBlock
            // 
            this.gBoxAlignBlock.Controls.Add(this.rBtnDBText);
            this.gBoxAlignBlock.Controls.Add(this.rBtnUCS);
            this.gBoxAlignBlock.Location = new System.Drawing.Point(19, 66);
            this.gBoxAlignBlock.Name = "gBoxAlignBlock";
            this.gBoxAlignBlock.Size = new System.Drawing.Size(250, 83);
            this.gBoxAlignBlock.TabIndex = 10;
            this.gBoxAlignBlock.TabStop = false;
            this.gBoxAlignBlock.Text = "Align Blocks to:";
            // 
            // nTrixMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 356);
            this.Controls.Add(this.gBoxAlignBlock);
            this.Controls.Add(this.lbl_oTRIX);
            this.Controls.Add(this.cb_oTRIX);
            this.Controls.Add(this.btnRunNTRIX);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbUsedShotPoints);
            this.Controls.Add(this.lblUsedShotpoints);
            this.MinimumSize = new System.Drawing.Size(305, 395);
            this.Name = "nTrixMainForm";
            this.Text = "nTRIX - Shotpoint Processing App";
            this.gBoxAlignBlock.ResumeLayout(false);
            this.gBoxAlignBlock.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openExcel;
        private System.Windows.Forms.Label lblUsedShotpoints;
        private System.Windows.Forms.CheckBox cbUsedShotPoints;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRunNTRIX;
        private System.Windows.Forms.CheckBox cb_oTRIX;
        private System.Windows.Forms.Label lbl_oTRIX;
        private System.Windows.Forms.RadioButton rBtnUCS;
        private System.Windows.Forms.RadioButton rBtnDBText;
        private System.Windows.Forms.GroupBox gBoxAlignBlock;
    }
}
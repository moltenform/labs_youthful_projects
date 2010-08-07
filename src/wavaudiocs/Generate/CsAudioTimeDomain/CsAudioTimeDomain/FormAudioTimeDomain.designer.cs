
namespace CsGeneralBitmap
{
    partial class FormAudioTimeDomain
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
            this.txtExpression = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.tbParam1 = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.lblParam1 = new System.Windows.Forms.Label();
            this.tbParam2 = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.lblParam2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSaveWav = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdvSetParamRange = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tbParam3 = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.lblParam3 = new System.Windows.Forms.Label();
            this.tbParam4 = new System.Windows.Forms.TrackBar();
            this.label10 = new System.Windows.Forms.Label();
            this.lblParam4 = new System.Windows.Forms.Label();
            this.btnHearResults = new System.Windows.Forms.Button();
            this.btnHelpPlay1 = new System.Windows.Forms.Button();
            this.btnHelpPlay2 = new System.Windows.Forms.Button();
            this.btnHelpPlay3 = new System.Windows.Forms.Button();
            this.btnHelpPlay4 = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.scintilla1 = new ScintillaNet.Scintilla();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam2)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scintilla1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtExpression
            // 
            this.txtExpression.Location = new System.Drawing.Point(808, 481);
            this.txtExpression.Multiline = true;
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExpression.Size = new System.Drawing.Size(64, 40);
            this.txtExpression.TabIndex = 0;
            this.txtExpression.Text = "p = r*p*(1-p);";
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(761, 70);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(135, 50);
            this.btnGo.TabIndex = 1;
            this.btnGo.Text = "Run";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // tbParam1
            // 
            this.tbParam1.LargeChange = 10;
            this.tbParam1.Location = new System.Drawing.Point(52, 539);
            this.tbParam1.Maximum = 10000;
            this.tbParam1.Name = "tbParam1";
            this.tbParam1.Size = new System.Drawing.Size(401, 45);
            this.tbParam1.TabIndex = 3;
            this.tbParam1.Scroll += new System.EventHandler(this.tbParam1_Scroll);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 539);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "c1";
            // 
            // lblParam1
            // 
            this.lblParam1.AutoSize = true;
            this.lblParam1.Location = new System.Drawing.Point(10, 552);
            this.lblParam1.Name = "lblParam1";
            this.lblParam1.Size = new System.Drawing.Size(15, 13);
            this.lblParam1.TabIndex = 4;
            this.lblParam1.Text = " x";
            this.lblParam1.Click += new System.EventHandler(this.lblParam1_Click);
            // 
            // tbParam2
            // 
            this.tbParam2.LargeChange = 10;
            this.tbParam2.Location = new System.Drawing.Point(52, 572);
            this.tbParam2.Maximum = 10000;
            this.tbParam2.Name = "tbParam2";
            this.tbParam2.Size = new System.Drawing.Size(401, 45);
            this.tbParam2.TabIndex = 3;
            this.tbParam2.Scroll += new System.EventHandler(this.tbParam2_Scroll);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 572);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(19, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "c2";
            // 
            // lblParam2
            // 
            this.lblParam2.AutoSize = true;
            this.lblParam2.Location = new System.Drawing.Point(10, 585);
            this.lblParam2.Name = "lblParam2";
            this.lblParam2.Size = new System.Drawing.Size(15, 13);
            this.lblParam2.TabIndex = 4;
            this.lblParam2.Text = "x ";
            this.lblParam2.Click += new System.EventHandler(this.lblParam2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(821, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Ben Fisher, 2010";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.advancedToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(908, 24);
            this.menuStrip1.TabIndex = 14;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileNew,
            this.mnuFileOpen,
            this.mnuFileSave,
            this.mnuFileSaveWav,
            this.toolStripSeparator1,
            this.mnuFileExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // mnuFileNew
            // 
            this.mnuFileNew.Name = "mnuFileNew";
            this.mnuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.mnuFileNew.Size = new System.Drawing.Size(203, 22);
            this.mnuFileNew.Text = "New";
            this.mnuFileNew.Click += new System.EventHandler(this.mnuFileNew_Click);
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Name = "mnuFileOpen";
            this.mnuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mnuFileOpen.Size = new System.Drawing.Size(203, 22);
            this.mnuFileOpen.Text = "Open...";
            this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
            // 
            // mnuFileSave
            // 
            this.mnuFileSave.Name = "mnuFileSave";
            this.mnuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnuFileSave.Size = new System.Drawing.Size(203, 22);
            this.mnuFileSave.Text = "Save As...";
            this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
            // 
            // mnuFileSaveWav
            // 
            this.mnuFileSaveWav.Name = "mnuFileSaveWav";
            this.mnuFileSaveWav.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.mnuFileSaveWav.Size = new System.Drawing.Size(203, 22);
            this.mnuFileSaveWav.Text = "Save wav...";
            this.mnuFileSaveWav.Click += new System.EventHandler(this.mnuFileSaveWav_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(200, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Size = new System.Drawing.Size(203, 22);
            this.mnuFileExit.Text = "Exit";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAdvSetParamRange});
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.advancedToolStripMenuItem.Text = "Advanced";
            // 
            // mnuAdvSetParamRange
            // 
            this.mnuAdvSetParamRange.Name = "mnuAdvSetParamRange";
            this.mnuAdvSetParamRange.Size = new System.Drawing.Size(177, 22);
            this.mnuAdvSetParamRange.Text = "Set max c1 and c2...";
            this.mnuAdvSetParamRange.Click += new System.EventHandler(this.mnuAdvSetParamRange_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(107, 22);
            this.mnuHelpAbout.Text = "About";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // tbParam3
            // 
            this.tbParam3.LargeChange = 10;
            this.tbParam3.Location = new System.Drawing.Point(506, 540);
            this.tbParam3.Maximum = 10000;
            this.tbParam3.Name = "tbParam3";
            this.tbParam3.Size = new System.Drawing.Size(401, 45);
            this.tbParam3.TabIndex = 3;
            this.tbParam3.Scroll += new System.EventHandler(this.tbParam3_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(466, 539);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "c3";
            // 
            // lblParam3
            // 
            this.lblParam3.AutoSize = true;
            this.lblParam3.Location = new System.Drawing.Point(464, 552);
            this.lblParam3.Name = "lblParam3";
            this.lblParam3.Size = new System.Drawing.Size(15, 13);
            this.lblParam3.TabIndex = 4;
            this.lblParam3.Text = " x";
            this.lblParam3.Click += new System.EventHandler(this.lblParam3_Click);
            // 
            // tbParam4
            // 
            this.tbParam4.LargeChange = 10;
            this.tbParam4.Location = new System.Drawing.Point(506, 572);
            this.tbParam4.Maximum = 10000;
            this.tbParam4.Name = "tbParam4";
            this.tbParam4.Size = new System.Drawing.Size(401, 45);
            this.tbParam4.TabIndex = 3;
            this.tbParam4.Scroll += new System.EventHandler(this.tbParam4_Scroll);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(466, 572);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(19, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "c4";
            // 
            // lblParam4
            // 
            this.lblParam4.AutoSize = true;
            this.lblParam4.Location = new System.Drawing.Point(464, 585);
            this.lblParam4.Name = "lblParam4";
            this.lblParam4.Size = new System.Drawing.Size(15, 13);
            this.lblParam4.TabIndex = 4;
            this.lblParam4.Text = " x";
            this.lblParam4.Click += new System.EventHandler(this.lblParam4_Click);
            // 
            // btnHearResults
            // 
            this.btnHearResults.Location = new System.Drawing.Point(761, 147);
            this.btnHearResults.Name = "btnHearResults";
            this.btnHearResults.Size = new System.Drawing.Size(135, 50);
            this.btnHearResults.TabIndex = 1;
            this.btnHearResults.Text = "Hear Results";
            this.btnHearResults.UseVisualStyleBackColor = true;
            this.btnHearResults.Click += new System.EventHandler(this.btnHearResults_Click);
            // 
            // btnHelpPlay1
            // 
            this.btnHelpPlay1.Location = new System.Drawing.Point(808, 254);
            this.btnHelpPlay1.Name = "btnHelpPlay1";
            this.btnHelpPlay1.Size = new System.Drawing.Size(75, 23);
            this.btnHelpPlay1.TabIndex = 15;
            this.btnHelpPlay1.Text = "bHelpPlay1";
            this.btnHelpPlay1.UseVisualStyleBackColor = true;
            this.btnHelpPlay1.Click += new System.EventHandler(this.btnHelpPlay1_Click);
            // 
            // btnHelpPlay2
            // 
            this.btnHelpPlay2.Location = new System.Drawing.Point(808, 283);
            this.btnHelpPlay2.Name = "btnHelpPlay2";
            this.btnHelpPlay2.Size = new System.Drawing.Size(75, 23);
            this.btnHelpPlay2.TabIndex = 15;
            this.btnHelpPlay2.Text = "bHelpPlay1";
            this.btnHelpPlay2.UseVisualStyleBackColor = true;
            // 
            // btnHelpPlay3
            // 
            this.btnHelpPlay3.Location = new System.Drawing.Point(808, 312);
            this.btnHelpPlay3.Name = "btnHelpPlay3";
            this.btnHelpPlay3.Size = new System.Drawing.Size(75, 23);
            this.btnHelpPlay3.TabIndex = 15;
            this.btnHelpPlay3.Text = "bHelpPlay1";
            this.btnHelpPlay3.UseVisualStyleBackColor = true;
            // 
            // btnHelpPlay4
            // 
            this.btnHelpPlay4.Location = new System.Drawing.Point(808, 341);
            this.btnHelpPlay4.Name = "btnHelpPlay4";
            this.btnHelpPlay4.Size = new System.Drawing.Size(75, 23);
            this.btnHelpPlay4.TabIndex = 15;
            this.btnHelpPlay4.Text = "bHelpPlay1";
            this.btnHelpPlay4.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(808, 216);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 15;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // scintilla1
            // 
            this.scintilla1.Location = new System.Drawing.Point(0, 27);
            this.scintilla1.Name = "scintilla1";
            this.scintilla1.Size = new System.Drawing.Size(755, 494);
            this.scintilla1.Styles.BraceBad.FontName = "Verdana";
            this.scintilla1.Styles.BraceLight.FontName = "Verdana";
            this.scintilla1.Styles.ControlChar.FontName = "Verdana";
            this.scintilla1.Styles.Default.FontName = "Verdana";
            this.scintilla1.Styles.IndentGuide.FontName = "Verdana";
            this.scintilla1.Styles.LastPredefined.FontName = "Verdana";
            this.scintilla1.Styles.LineNumber.FontName = "Verdana";
            this.scintilla1.Styles.Max.FontName = "Verdana";
            this.scintilla1.TabIndex = 16;
            // 
            // FormAudioTimeDomain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 623);
            this.Controls.Add(this.scintilla1);
            this.Controls.Add(this.btnHelpPlay4);
            this.Controls.Add(this.btnHelpPlay3);
            this.Controls.Add(this.btnHelpPlay2);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnHelpPlay1);
            this.Controls.Add(this.lblParam2);
            this.Controls.Add(this.lblParam4);
            this.Controls.Add(this.lblParam3);
            this.Controls.Add(this.lblParam1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbParam2);
            this.Controls.Add(this.tbParam4);
            this.Controls.Add(this.tbParam3);
            this.Controls.Add(this.tbParam1);
            this.Controls.Add(this.btnHearResults);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtExpression);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormAudioTimeDomain";
            this.Text = "CsBifcuration";
            ((System.ComponentModel.ISupportInitialize)(this.tbParam1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam2)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scintilla1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtExpression;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TrackBar tbParam1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblParam1;
        private System.Windows.Forms.TrackBar tbParam2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblParam2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem advancedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem mnuFileNew;
        private System.Windows.Forms.TrackBar tbParam3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblParam3;
        private System.Windows.Forms.TrackBar tbParam4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblParam4;
        private System.Windows.Forms.ToolStripMenuItem mnuAdvSetParamRange;
        private System.Windows.Forms.Button btnHearResults;
        private System.Windows.Forms.Button btnHelpPlay1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSaveWav;
        private System.Windows.Forms.Button btnHelpPlay2;
        private System.Windows.Forms.Button btnHelpPlay3;
        private System.Windows.Forms.Button btnHelpPlay4;
        private System.Windows.Forms.Button btnStop;
        private ScintillaNet.Scintilla scintilla1;
    }
}


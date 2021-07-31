namespace CsDownloadVid
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.lblMain = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.csDownloadVidToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.audioFromVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitMediaIntoPartsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.joinTwoVideosLosslessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.helpAndTutorialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEncodeCustom = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMain
            // 
            this.lblMain.Location = new System.Drawing.Point(16, 172);
            this.lblMain.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMain.Name = "lblMain";
            this.lblMain.Size = new System.Drawing.Size(565, 142);
            this.lblMain.TabIndex = 0;
            this.lblMain.Text = resources.GetString("lblMain.Text");
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.csDownloadVidToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(581, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // csDownloadVidToolStripMenuItem
            // 
            this.csDownloadVidToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadVideoToolStripMenuItem,
            this.toolStripMenuItem1,
            this.audioFromVideoToolStripMenuItem,
            this.splitMediaIntoPartsToolStripMenuItem,
            this.joinTwoVideosLosslessToolStripMenuItem,
            this.mnuEncodeCustom,
            this.toolStripMenuItem2,
            this.helpAndTutorialsToolStripMenuItem});
            this.csDownloadVidToolStripMenuItem.Name = "csDownloadVidToolStripMenuItem";
            this.csDownloadVidToolStripMenuItem.Size = new System.Drawing.Size(54, 24);
            this.csDownloadVidToolStripMenuItem.Text = "Start";
            // 
            // downloadVideoToolStripMenuItem
            // 
            this.downloadVideoToolStripMenuItem.Name = "downloadVideoToolStripMenuItem";
            this.downloadVideoToolStripMenuItem.Size = new System.Drawing.Size(401, 26);
            this.downloadVideoToolStripMenuItem.Text = "Download a video...";
            this.downloadVideoToolStripMenuItem.Click += new System.EventHandler(this.mnuOpenFormGetVideo_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(398, 6);
            // 
            // audioFromVideoToolStripMenuItem
            // 
            this.audioFromVideoToolStripMenuItem.Name = "audioFromVideoToolStripMenuItem";
            this.audioFromVideoToolStripMenuItem.Size = new System.Drawing.Size(401, 26);
            this.audioFromVideoToolStripMenuItem.Text = "Separate audio and video (Lossless)...";
            this.audioFromVideoToolStripMenuItem.Click += new System.EventHandler(this.mnuOpenFormAudioFromVideo_Click);
            // 
            // splitMediaIntoPartsToolStripMenuItem
            // 
            this.splitMediaIntoPartsToolStripMenuItem.Name = "splitMediaIntoPartsToolStripMenuItem";
            this.splitMediaIntoPartsToolStripMenuItem.Size = new System.Drawing.Size(401, 26);
            this.splitMediaIntoPartsToolStripMenuItem.Text = "Split a video or a song into pieces (Lossless)...";
            this.splitMediaIntoPartsToolStripMenuItem.Click += new System.EventHandler(this.mnuOpenFormMediaSplit_Click);
            // 
            // joinTwoVideosLosslessToolStripMenuItem
            // 
            this.joinTwoVideosLosslessToolStripMenuItem.Name = "joinTwoVideosLosslessToolStripMenuItem";
            this.joinTwoVideosLosslessToolStripMenuItem.Size = new System.Drawing.Size(401, 26);
            this.joinTwoVideosLosslessToolStripMenuItem.Text = "Join the pieces of a video or a song (Lossless)...";
            this.joinTwoVideosLosslessToolStripMenuItem.Click += new System.EventHandler(this.mnuOpenFormMediaJoin_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(398, 6);
            // 
            // helpAndTutorialsToolStripMenuItem
            // 
            this.helpAndTutorialsToolStripMenuItem.Name = "helpAndTutorialsToolStripMenuItem";
            this.helpAndTutorialsToolStripMenuItem.Size = new System.Drawing.Size(401, 26);
            this.helpAndTutorialsToolStripMenuItem.Text = "Online info and tutorials...";
            this.helpAndTutorialsToolStripMenuItem.Click += new System.EventHandler(this.mnuOpenHelpWebsite_Click);
            // 
            // mnuEncodeCustom
            // 
            this.mnuEncodeCustom.Name = "mnuEncodeCustom";
            this.mnuEncodeCustom.Size = new System.Drawing.Size(401, 26);
            this.mnuEncodeCustom.Text = "Encode video or audio...";
            this.mnuEncodeCustom.Click += new System.EventHandler(this.mnuEncodeCustom_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 325);
            this.Controls.Add(this.lblMain);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormMain";
            this.Text = "CsDownloadVid";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMain;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem csDownloadVidToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadVideoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem audioFromVideoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem splitMediaIntoPartsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem joinTwoVideosLosslessToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem helpAndTutorialsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuEncodeCustom;
    }
}
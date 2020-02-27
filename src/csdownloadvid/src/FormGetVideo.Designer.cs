namespace CsDownloadVid
{
    partial class FormGetVideo
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
            this.chkShowAdvanced = new System.Windows.Forms.CheckBox();
            this.lbl1Basic = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btnNextStepIsToChooseQuality = new System.Windows.Forms.Button();
            this.panelChooseQuality = new System.Windows.Forms.Panel();
            this.btnNextStepIsToChooseOutput = new System.Windows.Forms.Button();
            this.listBoxFmts = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.panelAdvanced = new System.Windows.Forms.Panel();
            this.txtAdditionalArgs = new System.Windows.Forms.TextBox();
            this.txtWaitBetween = new System.Windows.Forms.TextBox();
            this.txtFilenamePattern = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblNamePattern = new System.Windows.Forms.Label();
            this.btnEncode = new System.Windows.Forms.Button();
            this.btnGetPlaylist = new System.Windows.Forms.Button();
            this.chkDashToM4a = new System.Windows.Forms.CheckBox();
            this.lblEnterUrlsAdvanced = new System.Windows.Forms.Label();
            this.panelChooseOutput = new System.Windows.Forms.Panel();
            this.btnSaveTo = new System.Windows.Forms.Button();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblShortStatus = new System.Windows.Forms.Label();
            this.btnGetUpdates = new System.Windows.Forms.Button();
            this.cbUsePytube = new System.Windows.Forms.CheckBox();
            this.btnDownloadFromWeb = new System.Windows.Forms.Button();
            this.panelChooseQuality.SuspendLayout();
            this.panelAdvanced.SuspendLayout();
            this.panelChooseOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkShowAdvanced
            // 
            this.chkShowAdvanced.AutoSize = true;
            this.chkShowAdvanced.Location = new System.Drawing.Point(521, 12);
            this.chkShowAdvanced.Name = "chkShowAdvanced";
            this.chkShowAdvanced.Size = new System.Drawing.Size(121, 17);
            this.chkShowAdvanced.TabIndex = 0;
            this.chkShowAdvanced.Text = "Advanced options...";
            this.chkShowAdvanced.UseVisualStyleBackColor = true;
            this.chkShowAdvanced.CheckedChanged += new System.EventHandler(this.chkShowAdvanced_CheckedChanged);
            // 
            // lbl1Basic
            // 
            this.lbl1Basic.AutoSize = true;
            this.lbl1Basic.Location = new System.Drawing.Point(13, 42);
            this.lbl1Basic.Name = "lbl1Basic";
            this.lbl1Basic.Size = new System.Drawing.Size(135, 13);
            this.lbl1Basic.TabIndex = 1;
            this.lbl1Basic.Text = "Step 1: Enter URL of video";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(16, 68);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(499, 20);
            this.txtUrl.TabIndex = 2;
            // 
            // btnNextStepIsToChooseQuality
            // 
            this.btnNextStepIsToChooseQuality.Location = new System.Drawing.Point(546, 65);
            this.btnNextStepIsToChooseQuality.Name = "btnNextStepIsToChooseQuality";
            this.btnNextStepIsToChooseQuality.Size = new System.Drawing.Size(132, 23);
            this.btnNextStepIsToChooseQuality.TabIndex = 3;
            this.btnNextStepIsToChooseQuality.Text = "Go to next step";
            this.btnNextStepIsToChooseQuality.UseVisualStyleBackColor = true;
            this.btnNextStepIsToChooseQuality.Click += new System.EventHandler(this.btnNextToChooseQuality_Click);
            // 
            // panelChooseQuality
            // 
            this.panelChooseQuality.Controls.Add(this.btnNextStepIsToChooseOutput);
            this.panelChooseQuality.Controls.Add(this.listBoxFmts);
            this.panelChooseQuality.Controls.Add(this.label2);
            this.panelChooseQuality.Location = new System.Drawing.Point(2, 117);
            this.panelChooseQuality.Name = "panelChooseQuality";
            this.panelChooseQuality.Size = new System.Drawing.Size(713, 206);
            this.panelChooseQuality.TabIndex = 4;
            // 
            // btnNextStepIsToChooseOutput
            // 
            this.btnNextStepIsToChooseOutput.Location = new System.Drawing.Point(544, 168);
            this.btnNextStepIsToChooseOutput.Name = "btnNextStepIsToChooseOutput";
            this.btnNextStepIsToChooseOutput.Size = new System.Drawing.Size(132, 23);
            this.btnNextStepIsToChooseOutput.TabIndex = 4;
            this.btnNextStepIsToChooseOutput.Text = "Go to next step";
            this.btnNextStepIsToChooseOutput.UseVisualStyleBackColor = true;
            this.btnNextStepIsToChooseOutput.Click += new System.EventHandler(this.btnNextStepIsToChooseOutput_Click);
            // 
            // listBoxFmts
            // 
            this.listBoxFmts.FormattingEnabled = true;
            this.listBoxFmts.Location = new System.Drawing.Point(14, 31);
            this.listBoxFmts.Name = "listBoxFmts";
            this.listBoxFmts.ScrollAlwaysVisible = true;
            this.listBoxFmts.Size = new System.Drawing.Size(499, 160);
            this.listBoxFmts.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Step 2: Choose download quality";
            // 
            // txtStatus
            // 
            this.txtStatus.BackColor = System.Drawing.SystemColors.Control;
            this.txtStatus.Location = new System.Drawing.Point(16, 513);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(499, 113);
            this.txtStatus.TabIndex = 14;
            // 
            // panelAdvanced
            // 
            this.panelAdvanced.Controls.Add(this.btnDownloadFromWeb);
            this.panelAdvanced.Controls.Add(this.txtAdditionalArgs);
            this.panelAdvanced.Controls.Add(this.txtWaitBetween);
            this.panelAdvanced.Controls.Add(this.txtFilenamePattern);
            this.panelAdvanced.Controls.Add(this.label5);
            this.panelAdvanced.Controls.Add(this.label6);
            this.panelAdvanced.Controls.Add(this.label4);
            this.panelAdvanced.Controls.Add(this.lblNamePattern);
            this.panelAdvanced.Controls.Add(this.btnEncode);
            this.panelAdvanced.Controls.Add(this.btnGetPlaylist);
            this.panelAdvanced.Controls.Add(this.chkDashToM4a);
            this.panelAdvanced.Location = new System.Drawing.Point(588, 380);
            this.panelAdvanced.Name = "panelAdvanced";
            this.panelAdvanced.Size = new System.Drawing.Size(225, 246);
            this.panelAdvanced.TabIndex = 15;
            // 
            // txtAdditionalArgs
            // 
            this.txtAdditionalArgs.Location = new System.Drawing.Point(19, 188);
            this.txtAdditionalArgs.Name = "txtAdditionalArgs";
            this.txtAdditionalArgs.Size = new System.Drawing.Size(116, 20);
            this.txtAdditionalArgs.TabIndex = 41;
            // 
            // txtWaitBetween
            // 
            this.txtWaitBetween.Location = new System.Drawing.Point(19, 153);
            this.txtWaitBetween.Name = "txtWaitBetween";
            this.txtWaitBetween.Size = new System.Drawing.Size(116, 20);
            this.txtWaitBetween.TabIndex = 41;
            this.txtWaitBetween.Text = "5";
            // 
            // txtFilenamePattern
            // 
            this.txtFilenamePattern.Location = new System.Drawing.Point(19, 114);
            this.txtFilenamePattern.Name = "txtFilenamePattern";
            this.txtFilenamePattern.Size = new System.Drawing.Size(203, 20);
            this.txtFilenamePattern.TabIndex = 40;
            this.txtFilenamePattern.Text = "%(uploader)s @ %(title)s [%(id)s].%(ext)s";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 176);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 39;
            this.label5.Text = "Additional args";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 39;
            this.label6.Text = "Advanced opts:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(156, 13);
            this.label4.TabIndex = 39;
            this.label4.Text = "Wait between dloads (seconds)";
            // 
            // lblNamePattern
            // 
            this.lblNamePattern.AutoSize = true;
            this.lblNamePattern.Location = new System.Drawing.Point(17, 80);
            this.lblNamePattern.Name = "lblNamePattern";
            this.lblNamePattern.Size = new System.Drawing.Size(85, 13);
            this.lblNamePattern.TabIndex = 39;
            this.lblNamePattern.Text = "Filename pattern";
            // 
            // btnEncode
            // 
            this.btnEncode.Location = new System.Drawing.Point(79, 214);
            this.btnEncode.Name = "btnEncode";
            this.btnEncode.Size = new System.Drawing.Size(56, 26);
            this.btnEncode.TabIndex = 38;
            this.btnEncode.Text = "Encode";
            this.btnEncode.UseVisualStyleBackColor = true;
            this.btnEncode.Click += new System.EventHandler(this.btnEncode_Click);
            // 
            // btnGetPlaylist
            // 
            this.btnGetPlaylist.Location = new System.Drawing.Point(3, 214);
            this.btnGetPlaylist.Name = "btnGetPlaylist";
            this.btnGetPlaylist.Size = new System.Drawing.Size(73, 26);
            this.btnGetPlaylist.TabIndex = 38;
            this.btnGetPlaylist.Text = "Get playlist";
            this.btnGetPlaylist.UseVisualStyleBackColor = true;
            this.btnGetPlaylist.Click += new System.EventHandler(this.btnGetPlaylist_Click);
            // 
            // chkDashToM4a
            // 
            this.chkDashToM4a.AutoSize = true;
            this.chkDashToM4a.Checked = true;
            this.chkDashToM4a.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDashToM4a.Location = new System.Drawing.Point(19, 48);
            this.chkDashToM4a.Name = "chkDashToM4a";
            this.chkDashToM4a.Size = new System.Drawing.Size(185, 17);
            this.chkDashToM4a.TabIndex = 0;
            this.chkDashToM4a.Text = "fix DASH audio after downloading";
            this.chkDashToM4a.UseVisualStyleBackColor = true;
            // 
            // lblEnterUrlsAdvanced
            // 
            this.lblEnterUrlsAdvanced.AutoSize = true;
            this.lblEnterUrlsAdvanced.Location = new System.Drawing.Point(51, 42);
            this.lblEnterUrlsAdvanced.Name = "lblEnterUrlsAdvanced";
            this.lblEnterUrlsAdvanced.Size = new System.Drawing.Size(579, 13);
            this.lblEnterUrlsAdvanced.TabIndex = 28;
            this.lblEnterUrlsAdvanced.Text = "Enter URL, list of URLs separated by |, path of a .url file, path of a .txt file " +
    "containing urls, or a directory containing .url files:";
            // 
            // panelChooseOutput
            // 
            this.panelChooseOutput.Controls.Add(this.btnSaveTo);
            this.panelChooseOutput.Controls.Add(this.txtOutputDir);
            this.panelChooseOutput.Controls.Add(this.btnCancel);
            this.panelChooseOutput.Controls.Add(this.btnDownload);
            this.panelChooseOutput.Controls.Add(this.label1);
            this.panelChooseOutput.Location = new System.Drawing.Point(2, 339);
            this.panelChooseOutput.Name = "panelChooseOutput";
            this.panelChooseOutput.Size = new System.Drawing.Size(572, 146);
            this.panelChooseOutput.TabIndex = 30;
            // 
            // btnSaveTo
            // 
            this.btnSaveTo.Location = new System.Drawing.Point(437, 39);
            this.btnSaveTo.Name = "btnSaveTo";
            this.btnSaveTo.Size = new System.Drawing.Size(76, 23);
            this.btnSaveTo.TabIndex = 35;
            this.btnSaveTo.Text = "Browse...";
            this.btnSaveTo.UseVisualStyleBackColor = true;
            this.btnSaveTo.Click += new System.EventHandler(this.btnSaveTo_Click);
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.BackColor = System.Drawing.SystemColors.Control;
            this.txtOutputDir.Location = new System.Drawing.Point(14, 41);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.ReadOnly = true;
            this.txtOutputDir.Size = new System.Drawing.Size(417, 20);
            this.txtOutputDir.TabIndex = 34;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(133, 77);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(111, 57);
            this.btnCancel.TabIndex = 30;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(14, 77);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(107, 57);
            this.btnDownload.TabIndex = 30;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Step 3: Choose output location";
            // 
            // lblShortStatus
            // 
            this.lblShortStatus.AutoSize = true;
            this.lblShortStatus.Location = new System.Drawing.Point(24, 497);
            this.lblShortStatus.Name = "lblShortStatus";
            this.lblShortStatus.Size = new System.Drawing.Size(78, 13);
            this.lblShortStatus.TabIndex = 31;
            this.lblShortStatus.Text = "Current status: ";
            // 
            // btnGetUpdates
            // 
            this.btnGetUpdates.Location = new System.Drawing.Point(696, 65);
            this.btnGetUpdates.Name = "btnGetUpdates";
            this.btnGetUpdates.Size = new System.Drawing.Size(117, 23);
            this.btnGetUpdates.TabIndex = 3;
            this.btnGetUpdates.Text = "Get updates";
            this.btnGetUpdates.UseVisualStyleBackColor = true;
            this.btnGetUpdates.Click += new System.EventHandler(this.btnGetUpdates_Click);
            // 
            // cbUsePytube
            // 
            this.cbUsePytube.AutoSize = true;
            this.cbUsePytube.Location = new System.Drawing.Point(667, 12);
            this.cbUsePytube.Name = "cbUsePytube";
            this.cbUsePytube.Size = new System.Drawing.Size(148, 17);
            this.cbUsePytube.TabIndex = 0;
            this.cbUsePytube.Text = "Use pytube instead of ytdl";
            this.cbUsePytube.UseVisualStyleBackColor = true;
            this.cbUsePytube.CheckedChanged += new System.EventHandler(this.chkShowAdvanced_CheckedChanged);
            // 
            // btnDownloadFromWeb
            // 
            this.btnDownloadFromWeb.Location = new System.Drawing.Point(141, 214);
            this.btnDownloadFromWeb.Name = "btnDownloadFromWeb";
            this.btnDownloadFromWeb.Size = new System.Drawing.Size(81, 26);
            this.btnDownloadFromWeb.TabIndex = 42;
            this.btnDownloadFromWeb.Text = "From Web";
            this.btnDownloadFromWeb.UseVisualStyleBackColor = true;
            this.btnDownloadFromWeb.Click += new System.EventHandler(this.btnDownloadFromWeb_Click);
            // 
            // FormGetVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 654);
            this.Controls.Add(this.lblShortStatus);
            this.Controls.Add(this.panelChooseOutput);
            this.Controls.Add(this.lblEnterUrlsAdvanced);
            this.Controls.Add(this.panelAdvanced);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.panelChooseQuality);
            this.Controls.Add(this.btnGetUpdates);
            this.Controls.Add(this.btnNextStepIsToChooseQuality);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.lbl1Basic);
            this.Controls.Add(this.cbUsePytube);
            this.Controls.Add(this.chkShowAdvanced);
            this.Name = "FormGetVideo";
            this.Text = "Download Video";
            this.panelChooseQuality.ResumeLayout(false);
            this.panelChooseQuality.PerformLayout();
            this.panelAdvanced.ResumeLayout(false);
            this.panelAdvanced.PerformLayout();
            this.panelChooseOutput.ResumeLayout(false);
            this.panelChooseOutput.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkShowAdvanced;
        private System.Windows.Forms.Label lbl1Basic;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button btnNextStepIsToChooseQuality;
        private System.Windows.Forms.Panel panelChooseQuality;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxFmts;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Panel panelAdvanced;
        private System.Windows.Forms.CheckBox chkDashToM4a;
        private System.Windows.Forms.Label lblEnterUrlsAdvanced;
        private System.Windows.Forms.Panel panelChooseOutput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblNamePattern;
        private System.Windows.Forms.TextBox txtFilenamePattern;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.Button btnSaveTo;
        private System.Windows.Forms.Button btnNextStepIsToChooseOutput;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtWaitBetween;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtAdditionalArgs;
        private System.Windows.Forms.Label lblShortStatus;
        private System.Windows.Forms.Button btnGetUpdates;
        private System.Windows.Forms.Button btnGetPlaylist;
        private System.Windows.Forms.CheckBox cbUsePytube;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnEncode;
        private System.Windows.Forms.Button btnDownloadFromWeb;
    }
}
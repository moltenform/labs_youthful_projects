namespace rbcpy
{
    partial class FormSyncMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSyncMain));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPickDest = new System.Windows.Forms.Button();
            this.btnPickSrc = new System.Windows.Forms.Button();
            this.panelAdvancedSettings = new System.Windows.Forms.Panel();
            this.chkCompensateDST = new System.Windows.Forms.CheckBox();
            this.chkFatTimes = new System.Windows.Forms.CheckBox();
            this.chkSymlinkNotTarget = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtnWaitBetweenRetries = new System.Windows.Forms.TextBox();
            this.txtCustom = new System.Windows.Forms.TextBox();
            this.txtnRetries = new System.Windows.Forms.TextBox();
            this.txtnThreads = new System.Windows.Forms.TextBox();
            this.txtIpg = new System.Windows.Forms.TextBox();
            this.txtDirCopyFlags = new System.Windows.Forms.TextBox();
            this.txtCopyFlags = new System.Windows.Forms.TextBox();
            this.chkCopySubdirs = new System.Windows.Forms.CheckBox();
            this.chkAdvanced = new System.Windows.Forms.CheckBox();
            this.chkMirror = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtExcludeFiles = new System.Windows.Forms.TextBox();
            this.txtDestShowValid = new System.Windows.Forms.Label();
            this.txtSrcShowValid = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtExcludeDirs = new System.Windows.Forms.TextBox();
            this.txtDest = new System.Windows.Forms.TextBox();
            this.txtSrc = new System.Windows.Forms.TextBox();
            this.listBoxConfigs = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnShowCmd = new System.Windows.Forms.Button();
            this.btnOpenWinmerge = new System.Windows.Forms.Button();
            this.btnPreviewRun = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCopyCmd = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSwapSrcDest = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWinMergePath = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuManuallyDeleted = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.runTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelAdvancedSettings.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 213F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.listBoxConfigs, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 28);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(889, 526);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnPickDest);
            this.panel1.Controls.Add(this.btnPickSrc);
            this.panel1.Controls.Add(this.panelAdvancedSettings);
            this.panel1.Controls.Add(this.chkCopySubdirs);
            this.panel1.Controls.Add(this.chkAdvanced);
            this.panel1.Controls.Add(this.chkMirror);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtExcludeFiles);
            this.panel1.Controls.Add(this.txtDestShowValid);
            this.panel1.Controls.Add(this.txtSrcShowValid);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtExcludeDirs);
            this.panel1.Controls.Add(this.txtDest);
            this.panel1.Controls.Add(this.txtSrc);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(217, 4);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(668, 456);
            this.panel1.TabIndex = 0;
            // 
            // btnPickDest
            // 
            this.btnPickDest.Location = new System.Drawing.Point(535, 48);
            this.btnPickDest.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPickDest.Name = "btnPickDest";
            this.btnPickDest.Size = new System.Drawing.Size(41, 25);
            this.btnPickDest.TabIndex = 13;
            this.btnPickDest.Text = "...";
            this.btnPickDest.UseVisualStyleBackColor = true;
            this.btnPickDest.Click += new System.EventHandler(this.btnPickDest_Click);
            // 
            // btnPickSrc
            // 
            this.btnPickSrc.Location = new System.Drawing.Point(535, 20);
            this.btnPickSrc.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPickSrc.Name = "btnPickSrc";
            this.btnPickSrc.Size = new System.Drawing.Size(41, 25);
            this.btnPickSrc.TabIndex = 12;
            this.btnPickSrc.Text = "...";
            this.btnPickSrc.UseVisualStyleBackColor = true;
            this.btnPickSrc.Click += new System.EventHandler(this.btnPickSrc_Click);
            // 
            // panelAdvancedSettings
            // 
            this.panelAdvancedSettings.Controls.Add(this.chkCompensateDST);
            this.panelAdvancedSettings.Controls.Add(this.chkFatTimes);
            this.panelAdvancedSettings.Controls.Add(this.chkSymlinkNotTarget);
            this.panelAdvancedSettings.Controls.Add(this.label13);
            this.panelAdvancedSettings.Controls.Add(this.label15);
            this.panelAdvancedSettings.Controls.Add(this.label12);
            this.panelAdvancedSettings.Controls.Add(this.label14);
            this.panelAdvancedSettings.Controls.Add(this.label11);
            this.panelAdvancedSettings.Controls.Add(this.label10);
            this.panelAdvancedSettings.Controls.Add(this.label9);
            this.panelAdvancedSettings.Controls.Add(this.txtnWaitBetweenRetries);
            this.panelAdvancedSettings.Controls.Add(this.txtCustom);
            this.panelAdvancedSettings.Controls.Add(this.txtnRetries);
            this.panelAdvancedSettings.Controls.Add(this.txtnThreads);
            this.panelAdvancedSettings.Controls.Add(this.txtIpg);
            this.panelAdvancedSettings.Controls.Add(this.txtDirCopyFlags);
            this.panelAdvancedSettings.Controls.Add(this.txtCopyFlags);
            this.panelAdvancedSettings.Location = new System.Drawing.Point(21, 245);
            this.panelAdvancedSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelAdvancedSettings.Name = "panelAdvancedSettings";
            this.panelAdvancedSettings.Size = new System.Drawing.Size(656, 206);
            this.panelAdvancedSettings.TabIndex = 0;
            // 
            // chkCompensateDST
            // 
            this.chkCompensateDST.AutoSize = true;
            this.chkCompensateDST.Location = new System.Drawing.Point(428, 144);
            this.chkCompensateDST.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkCompensateDST.Name = "chkCompensateDST";
            this.chkCompensateDST.Size = new System.Drawing.Size(151, 21);
            this.chkCompensateDST.TabIndex = 39;
            this.chkCompensateDST.Text = "Adjust 1 hr for DST";
            this.chkCompensateDST.UseVisualStyleBackColor = true;
            // 
            // chkFatTimes
            // 
            this.chkFatTimes.AutoSize = true;
            this.chkFatTimes.Location = new System.Drawing.Point(203, 145);
            this.chkFatTimes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkFatTimes.Name = "chkFatTimes";
            this.chkFatTimes.Size = new System.Drawing.Size(223, 21);
            this.chkFatTimes.TabIndex = 38;
            this.chkFatTimes.Text = "2 sec granularity for FAT times";
            this.chkFatTimes.UseVisualStyleBackColor = true;
            // 
            // chkSymlinkNotTarget
            // 
            this.chkSymlinkNotTarget.AutoSize = true;
            this.chkSymlinkNotTarget.Location = new System.Drawing.Point(8, 145);
            this.chkSymlinkNotTarget.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkSymlinkNotTarget.Name = "chkSymlinkNotTarget";
            this.chkSymlinkNotTarget.Size = new System.Drawing.Size(184, 21);
            this.chkSymlinkNotTarget.TabIndex = 37;
            this.chkSymlinkNotTarget.Text = "Copy symlinks not target";
            this.chkSymlinkNotTarget.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(333, 84);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(141, 17);
            this.label13.TabIndex = 0;
            this.label13.Text = "Wait between retries:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(195, 116);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(59, 17);
            this.label15.TabIndex = 0;
            this.label15.Text = "Custom:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(195, 84);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 17);
            this.label12.TabIndex = 0;
            this.label12.Text = "Retries:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(4, 116);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 17);
            this.label14.TabIndex = 0;
            this.label14.Text = "Threads:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 84);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(115, 17);
            this.label11.TabIndex = 0;
            this.label11.Text = "Inter-packet gap:";
            this.toolTip1.SetToolTip(this.label11, "Inter-Packet Gap (ms), to free bandwidth on slow lines");
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 52);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(201, 17);
            this.label10.TabIndex = 0;
            this.label10.Text = "Copy dir metadata, default: DA";
            this.toolTip1.SetToolTip(this.label10, "D=Data, A=Attributes, T=Timestamps");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 21);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(212, 17);
            this.label9.TabIndex = 0;
            this.label9.Text = "Copy file metadata, default: DAT";
            this.toolTip1.SetToolTip(this.label9, "D=Data, A=Attributes, T=Timestamps,S=Security=NTFS ACLs, O=Owner info, U=aUditing" +
        " info");
            // 
            // txtnWaitBetweenRetries
            // 
            this.txtnWaitBetweenRetries.Location = new System.Drawing.Point(481, 80);
            this.txtnWaitBetweenRetries.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtnWaitBetweenRetries.Name = "txtnWaitBetweenRetries";
            this.txtnWaitBetweenRetries.Size = new System.Drawing.Size(77, 22);
            this.txtnWaitBetweenRetries.TabIndex = 34;
            // 
            // txtCustom
            // 
            this.txtCustom.Location = new System.Drawing.Point(263, 112);
            this.txtCustom.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCustom.Name = "txtCustom";
            this.txtCustom.Size = new System.Drawing.Size(296, 22);
            this.txtCustom.TabIndex = 36;
            // 
            // txtnRetries
            // 
            this.txtnRetries.Location = new System.Drawing.Point(256, 80);
            this.txtnRetries.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtnRetries.Name = "txtnRetries";
            this.txtnRetries.Size = new System.Drawing.Size(72, 22);
            this.txtnRetries.TabIndex = 33;
            // 
            // txtnThreads
            // 
            this.txtnThreads.Location = new System.Drawing.Point(129, 112);
            this.txtnThreads.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtnThreads.Name = "txtnThreads";
            this.txtnThreads.Size = new System.Drawing.Size(52, 22);
            this.txtnThreads.TabIndex = 35;
            // 
            // txtIpg
            // 
            this.txtIpg.Location = new System.Drawing.Point(129, 80);
            this.txtIpg.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtIpg.Name = "txtIpg";
            this.txtIpg.Size = new System.Drawing.Size(52, 22);
            this.txtIpg.TabIndex = 32;
            // 
            // txtDirCopyFlags
            // 
            this.txtDirCopyFlags.Location = new System.Drawing.Point(223, 48);
            this.txtDirCopyFlags.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDirCopyFlags.Name = "txtDirCopyFlags";
            this.txtDirCopyFlags.Size = new System.Drawing.Size(336, 22);
            this.txtDirCopyFlags.TabIndex = 31;
            // 
            // txtCopyFlags
            // 
            this.txtCopyFlags.Location = new System.Drawing.Point(223, 17);
            this.txtCopyFlags.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCopyFlags.Name = "txtCopyFlags";
            this.txtCopyFlags.Size = new System.Drawing.Size(336, 22);
            this.txtCopyFlags.TabIndex = 30;
            // 
            // chkCopySubdirs
            // 
            this.chkCopySubdirs.AutoSize = true;
            this.chkCopySubdirs.Checked = true;
            this.chkCopySubdirs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCopySubdirs.Location = new System.Drawing.Point(271, 219);
            this.chkCopySubdirs.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkCopySubdirs.Name = "chkCopySubdirs";
            this.chkCopySubdirs.Size = new System.Drawing.Size(186, 21);
            this.chkCopySubdirs.TabIndex = 17;
            this.chkCopySubdirs.Text = "Include all subdirectories";
            this.chkCopySubdirs.UseVisualStyleBackColor = true;
            // 
            // chkAdvanced
            // 
            this.chkAdvanced.AutoSize = true;
            this.chkAdvanced.Location = new System.Drawing.Point(467, 219);
            this.chkAdvanced.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkAdvanced.Name = "chkAdvanced";
            this.chkAdvanced.Size = new System.Drawing.Size(148, 21);
            this.chkAdvanced.TabIndex = 18;
            this.chkAdvanced.Text = "Advanced Settings";
            this.chkAdvanced.UseVisualStyleBackColor = true;
            this.chkAdvanced.CheckedChanged += new System.EventHandler(this.chkAdvanced_CheckedChanged);
            // 
            // chkMirror
            // 
            this.chkMirror.AutoSize = true;
            this.chkMirror.Checked = true;
            this.chkMirror.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMirror.Location = new System.Drawing.Point(21, 219);
            this.chkMirror.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkMirror.Name = "chkMirror";
            this.chkMirror.Size = new System.Drawing.Size(228, 21);
            this.chkMirror.TabIndex = 16;
            this.chkMirror.Text = "Mirror (revert changes on right)";
            this.chkMirror.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 170);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "Exclude Files:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 85);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Exclude Dirs:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 52);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Destination:";
            // 
            // txtExcludeFiles
            // 
            this.txtExcludeFiles.Location = new System.Drawing.Point(117, 166);
            this.txtExcludeFiles.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtExcludeFiles.Multiline = true;
            this.txtExcludeFiles.Name = "txtExcludeFiles";
            this.txtExcludeFiles.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExcludeFiles.Size = new System.Drawing.Size(457, 40);
            this.txtExcludeFiles.TabIndex = 15;
            // 
            // txtDestShowValid
            // 
            this.txtDestShowValid.AutoSize = true;
            this.txtDestShowValid.Location = new System.Drawing.Point(584, 52);
            this.txtDestShowValid.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtDestShowValid.Name = "txtDestShowValid";
            this.txtDestShowValid.Size = new System.Drawing.Size(17, 17);
            this.txtDestShowValid.TabIndex = 0;
            this.txtDestShowValid.Text = "✓";
            // 
            // txtSrcShowValid
            // 
            this.txtSrcShowValid.AutoSize = true;
            this.txtSrcShowValid.Location = new System.Drawing.Point(584, 20);
            this.txtSrcShowValid.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtSrcShowValid.Name = "txtSrcShowValid";
            this.txtSrcShowValid.Size = new System.Drawing.Size(17, 17);
            this.txtSrcShowValid.TabIndex = 0;
            this.txtSrcShowValid.Text = "✓";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source:";
            // 
            // txtExcludeDirs
            // 
            this.txtExcludeDirs.Location = new System.Drawing.Point(117, 81);
            this.txtExcludeDirs.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtExcludeDirs.Multiline = true;
            this.txtExcludeDirs.Name = "txtExcludeDirs";
            this.txtExcludeDirs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExcludeDirs.Size = new System.Drawing.Size(457, 77);
            this.txtExcludeDirs.TabIndex = 14;
            // 
            // txtDest
            // 
            this.txtDest.Location = new System.Drawing.Point(117, 48);
            this.txtDest.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDest.Name = "txtDest";
            this.txtDest.Size = new System.Drawing.Size(408, 22);
            this.txtDest.TabIndex = 11;
            this.txtDest.TextChanged += new System.EventHandler(this.txtDest_TextChanged);
            this.txtDest.Enter += new System.EventHandler(this.Txt_CallSelectOnEnter);
            // 
            // txtSrc
            // 
            this.txtSrc.Location = new System.Drawing.Point(117, 20);
            this.txtSrc.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtSrc.Name = "txtSrc";
            this.txtSrc.Size = new System.Drawing.Size(408, 22);
            this.txtSrc.TabIndex = 10;
            this.txtSrc.TextChanged += new System.EventHandler(this.txtSrc_TextChanged);
            this.txtSrc.Enter += new System.EventHandler(this.Txt_CallSelectOnEnter);
            // 
            // listBoxConfigs
            // 
            this.listBoxConfigs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxConfigs.FormattingEnabled = true;
            this.listBoxConfigs.ItemHeight = 16;
            this.listBoxConfigs.Location = new System.Drawing.Point(4, 4);
            this.listBoxConfigs.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listBoxConfigs.Name = "listBoxConfigs";
            this.listBoxConfigs.Size = new System.Drawing.Size(205, 456);
            this.listBoxConfigs.TabIndex = 0;
            this.listBoxConfigs.SelectedIndexChanged += new System.EventHandler(this.listBoxConfigs_SelectedIndexChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 2);
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.tableLayoutPanel2.Controls.Add(this.btnShowCmd, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnOpenWinmerge, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnPreviewRun, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 468);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(881, 54);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // btnShowCmd
            // 
            this.btnShowCmd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnShowCmd.Location = new System.Drawing.Point(616, 4);
            this.btnShowCmd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnShowCmd.Name = "btnShowCmd";
            this.btnShowCmd.Size = new System.Drawing.Size(125, 46);
            this.btnShowCmd.TabIndex = 52;
            this.btnShowCmd.Text = "Show Cmd";
            this.btnShowCmd.UseVisualStyleBackColor = true;
            this.btnShowCmd.Click += new System.EventHandler(this.btnShowCmd_Click);
            // 
            // btnOpenWinmerge
            // 
            this.btnOpenWinmerge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnOpenWinmerge.Location = new System.Drawing.Point(310, 4);
            this.btnOpenWinmerge.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOpenWinmerge.Name = "btnOpenWinmerge";
            this.btnOpenWinmerge.Size = new System.Drawing.Size(298, 46);
            this.btnOpenWinmerge.TabIndex = 51;
            this.btnOpenWinmerge.Text = "Open in WinMerge";
            this.btnOpenWinmerge.UseVisualStyleBackColor = true;
            this.btnOpenWinmerge.Click += new System.EventHandler(this.btnOpenWinmerge_Click);
            // 
            // btnPreviewRun
            // 
            this.btnPreviewRun.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPreviewRun.Location = new System.Drawing.Point(4, 4);
            this.btnPreviewRun.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPreviewRun.Name = "btnPreviewRun";
            this.btnPreviewRun.Size = new System.Drawing.Size(298, 46);
            this.btnPreviewRun.TabIndex = 50;
            this.btnPreviewRun.Text = "Preview / Run";
            this.btnPreviewRun.UseVisualStyleBackColor = true;
            this.btnPreviewRun.Click += new System.EventHandler(this.btnPreviewRun_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(889, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSave,
            this.mnuSaveAs,
            this.toolStripMenuItem1,
            this.mnuCopyCmd,
            this.mnuSwapSrcDest,
            this.mnuWinMergePath,
            this.mnuManuallyDeleted,
            this.toolStripMenuItem2,
            this.runTestsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // mnuSave
            // 
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnuSave.Size = new System.Drawing.Size(250, 26);
            this.mnuSave.Text = "Save";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // mnuSaveAs
            // 
            this.mnuSaveAs.Name = "mnuSaveAs";
            this.mnuSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.mnuSaveAs.Size = new System.Drawing.Size(250, 26);
            this.mnuSaveAs.Text = "Save As...";
            this.mnuSaveAs.Click += new System.EventHandler(this.mnuSaveAs_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(247, 6);
            // 
            // mnuCopyCmd
            // 
            this.mnuCopyCmd.Name = "mnuCopyCmd";
            this.mnuCopyCmd.Size = new System.Drawing.Size(250, 26);
            this.mnuCopyCmd.Text = "Copy Cmd";
            this.mnuCopyCmd.Click += new System.EventHandler(this.mnuCopyCmd_Click);
            // 
            // mnuSwapSrcDest
            // 
            this.mnuSwapSrcDest.Name = "mnuSwapSrcDest";
            this.mnuSwapSrcDest.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.mnuSwapSrcDest.Size = new System.Drawing.Size(250, 26);
            this.mnuSwapSrcDest.Text = "Swap Src/Dest";
            this.mnuSwapSrcDest.Click += new System.EventHandler(this.mnuSwapSrcDest_Click);
            // 
            // mnuWinMergePath
            // 
            this.mnuWinMergePath.Name = "mnuWinMergePath";
            this.mnuWinMergePath.Size = new System.Drawing.Size(250, 26);
            this.mnuWinMergePath.Text = "WinMerge Path...";
            this.mnuWinMergePath.Click += new System.EventHandler(this.mnuWinMergePath_Click);
            // 
            // mnuManuallyDeleted
            // 
            this.mnuManuallyDeleted.Name = "mnuManuallyDeleted";
            this.mnuManuallyDeleted.Size = new System.Drawing.Size(250, 26);
            this.mnuManuallyDeleted.Text = "Manually deleted path...";
            this.mnuManuallyDeleted.Click += new System.EventHandler(this.mnuManuallyDeleted_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(247, 6);
            // 
            // runTestsToolStripMenuItem
            // 
            this.runTestsToolStripMenuItem.Name = "runTestsToolStripMenuItem";
            this.runTestsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.T)));
            this.runTestsToolStripMenuItem.Size = new System.Drawing.Size(250, 26);
            this.runTestsToolStripMenuItem.Text = "Run Tests";
            this.runTestsToolStripMenuItem.Click += new System.EventHandler(this.runTestsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(247, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(250, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(133, 26);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // FormSyncMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 554);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormSyncMain";
            this.Text = "Rbcpy";
            this.Load += new System.EventHandler(this.CreateSyncMain_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FormSyncMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FormSyncMain_DragEnter);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelAdvancedSettings.ResumeLayout(false);
            this.panelAdvancedSettings.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnPreviewRun;
        private System.Windows.Forms.Button btnOpenWinmerge;
        private System.Windows.Forms.ListBox listBoxConfigs;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelAdvancedSettings;
        private System.Windows.Forms.CheckBox chkCompensateDST;
        private System.Windows.Forms.CheckBox chkFatTimes;
        private System.Windows.Forms.CheckBox chkSymlinkNotTarget;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtnWaitBetweenRetries;
        private System.Windows.Forms.TextBox txtCustom;
        private System.Windows.Forms.TextBox txtnRetries;
        private System.Windows.Forms.TextBox txtnThreads;
        private System.Windows.Forms.TextBox txtIpg;
        private System.Windows.Forms.TextBox txtDirCopyFlags;
        private System.Windows.Forms.TextBox txtCopyFlags;
        private System.Windows.Forms.CheckBox chkCopySubdirs;
        private System.Windows.Forms.CheckBox chkAdvanced;
        private System.Windows.Forms.CheckBox chkMirror;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtExcludeFiles;
        private System.Windows.Forms.Label txtDestShowValid;
        private System.Windows.Forms.Label txtSrcShowValid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtExcludeDirs;
        private System.Windows.Forms.TextBox txtDest;
        private System.Windows.Forms.TextBox txtSrc;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.ToolStripMenuItem mnuSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyCmd;
        private System.Windows.Forms.ToolStripMenuItem mnuSwapSrcDest;
        private System.Windows.Forms.ToolStripMenuItem mnuWinMergePath;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuManuallyDeleted;
        private System.Windows.Forms.Button btnPickSrc;
        private System.Windows.Forms.Button btnPickDest;
        private System.Windows.Forms.Button btnShowCmd;
        private System.Windows.Forms.ToolStripMenuItem runTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
    }
}
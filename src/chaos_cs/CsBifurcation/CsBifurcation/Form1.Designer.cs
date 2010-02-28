namespace CsBifurcation
{
    partial class Form1
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
            this.tbSettling = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSettling = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbShading = new System.Windows.Forms.TrackBar();
            this.lblShading = new System.Windows.Forms.Label();
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
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileRender = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileAnimate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewZoomUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewReset = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewRedraw = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdvShades = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdvPoints = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAdvBounds = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdvAddQuality = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdvAutoRedraw = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tbParam3 = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.lblParam3 = new System.Windows.Forms.Label();
            this.tbParam4 = new System.Windows.Forms.TrackBar();
            this.label10 = new System.Windows.Forms.Label();
            this.lblParam4 = new System.Windows.Forms.Label();
            this.txtP0 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtInit = new System.Windows.Forms.TextBox();
            this.pointPlotBifurcationUserControl1 = new CsBifurcation.PlotBitmapBifurcationControl();
            ((System.ComponentModel.ISupportInitialize)(this.tbSettling)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam2)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam4)).BeginInit();
            this.SuspendLayout();
            // 
            // txtExpression
            // 
            this.txtExpression.Location = new System.Drawing.Point(541, 136);
            this.txtExpression.Multiline = true;
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExpression.Size = new System.Drawing.Size(209, 61);
            this.txtExpression.TabIndex = 0;
            this.txtExpression.Text = "p = r*p*(1-p);";
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(541, 203);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(210, 50);
            this.btnGo.TabIndex = 1;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // tbSettling
            // 
            this.tbSettling.Location = new System.Drawing.Point(67, 466);
            this.tbSettling.Maximum = 1000;
            this.tbSettling.Name = "tbSettling";
            this.tbSettling.Size = new System.Drawing.Size(370, 45);
            this.tbSettling.TabIndex = 3;
            this.tbSettling.Scroll += new System.EventHandler(this.tbSettling_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 468);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Settling";
            // 
            // lblSettling
            // 
            this.lblSettling.AutoSize = true;
            this.lblSettling.Location = new System.Drawing.Point(10, 481);
            this.lblSettling.Name = "lblSettling";
            this.lblSettling.Size = new System.Drawing.Size(12, 13);
            this.lblSettling.TabIndex = 4;
            this.lblSettling.Text = "x";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 501);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Shading";
            // 
            // tbShading
            // 
            this.tbShading.Location = new System.Drawing.Point(67, 501);
            this.tbShading.Maximum = 1000;
            this.tbShading.Name = "tbShading";
            this.tbShading.Size = new System.Drawing.Size(370, 45);
            this.tbShading.TabIndex = 3;
            this.tbShading.Scroll += new System.EventHandler(this.tbShading_Scroll);
            // 
            // lblShading
            // 
            this.lblShading.AutoSize = true;
            this.lblShading.Location = new System.Drawing.Point(10, 514);
            this.lblShading.Name = "lblShading";
            this.lblShading.Size = new System.Drawing.Size(12, 13);
            this.lblShading.TabIndex = 4;
            this.lblShading.Text = "x";
            // 
            // tbParam1
            // 
            this.tbParam1.LargeChange = 10;
            this.tbParam1.Location = new System.Drawing.Point(67, 539);
            this.tbParam1.Maximum = 10000;
            this.tbParam1.Name = "tbParam1";
            this.tbParam1.Size = new System.Drawing.Size(370, 45);
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
            // 
            // tbParam2
            // 
            this.tbParam2.LargeChange = 10;
            this.tbParam2.Location = new System.Drawing.Point(67, 572);
            this.tbParam2.Maximum = 10000;
            this.tbParam2.Name = "tbParam2";
            this.tbParam2.Size = new System.Drawing.Size(370, 45);
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
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(776, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Ben Fisher, 2010";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.advancedToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(863, 24);
            this.menuStrip1.TabIndex = 14;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileNew,
            this.mnuFileOpen,
            this.mnuFileSave,
            this.toolStripSeparator1,
            this.mnuFileRender,
            this.mnuFileAnimate,
            this.toolStripSeparator5,
            this.mnuFileExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // mnuFileNew
            // 
            this.mnuFileNew.Name = "mnuFileNew";
            this.mnuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.mnuFileNew.Size = new System.Drawing.Size(195, 22);
            this.mnuFileNew.Text = "New";
            this.mnuFileNew.Click += new System.EventHandler(this.mnuFileNew_Click);
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Name = "mnuFileOpen";
            this.mnuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mnuFileOpen.Size = new System.Drawing.Size(195, 22);
            this.mnuFileOpen.Text = "Open...";
            this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
            // 
            // mnuFileSave
            // 
            this.mnuFileSave.Name = "mnuFileSave";
            this.mnuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnuFileSave.Size = new System.Drawing.Size(195, 22);
            this.mnuFileSave.Text = "Save As...";
            this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
            // 
            // mnuFileRender
            // 
            this.mnuFileRender.Name = "mnuFileRender";
            this.mnuFileRender.Size = new System.Drawing.Size(195, 22);
            this.mnuFileRender.Text = "Render Image to Disk...";
            this.mnuFileRender.Click += new System.EventHandler(this.mnuFileRender_Click);
            // 
            // mnuFileAnimate
            // 
            this.mnuFileAnimate.Name = "mnuFileAnimate";
            this.mnuFileAnimate.Size = new System.Drawing.Size(195, 22);
            this.mnuFileAnimate.Text = "Create Animation...";
            this.mnuFileAnimate.Click += new System.EventHandler(this.mnuFileAnimate_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(192, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Size = new System.Drawing.Size(195, 22);
            this.mnuFileExit.Text = "Exit";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewZoomIn,
            this.mnuViewZoomOut,
            this.mnuViewZoomUndo,
            this.toolStripSeparator3,
            this.mnuViewReset,
            this.toolStripSeparator4,
            this.mnuViewRedraw});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // mnuViewZoomIn
            // 
            this.mnuViewZoomIn.Name = "mnuViewZoomIn";
            this.mnuViewZoomIn.Size = new System.Drawing.Size(179, 22);
            this.mnuViewZoomIn.Text = "Zoom In";
            this.mnuViewZoomIn.Click += new System.EventHandler(this.mnuViewZoomIn_Click);
            // 
            // mnuViewZoomOut
            // 
            this.mnuViewZoomOut.Name = "mnuViewZoomOut";
            this.mnuViewZoomOut.Size = new System.Drawing.Size(179, 22);
            this.mnuViewZoomOut.Text = "Zoom Out";
            this.mnuViewZoomOut.Click += new System.EventHandler(this.mnuViewZoomOut_Click);
            // 
            // mnuViewZoomUndo
            // 
            this.mnuViewZoomUndo.Name = "mnuViewZoomUndo";
            this.mnuViewZoomUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.mnuViewZoomUndo.Size = new System.Drawing.Size(179, 22);
            this.mnuViewZoomUndo.Text = "Undo Zoom";
            this.mnuViewZoomUndo.Click += new System.EventHandler(this.mnuViewZoomUndo_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(176, 6);
            // 
            // mnuViewReset
            // 
            this.mnuViewReset.Name = "mnuViewReset";
            this.mnuViewReset.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Up)));
            this.mnuViewReset.Size = new System.Drawing.Size(179, 22);
            this.mnuViewReset.Text = "Reset";
            this.mnuViewReset.Click += new System.EventHandler(this.mnuViewReset_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(176, 6);
            // 
            // mnuViewRedraw
            // 
            this.mnuViewRedraw.Name = "mnuViewRedraw";
            this.mnuViewRedraw.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Space)));
            this.mnuViewRedraw.Size = new System.Drawing.Size(179, 22);
            this.mnuViewRedraw.Text = "Redraw";
            this.mnuViewRedraw.Click += new System.EventHandler(this.mnuViewRedraw_Click);
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAdvShades,
            this.mnuAdvPoints,
            this.toolStripSeparator2,
            this.mnuAdvBounds,
            this.mnuAdvAddQuality,
            this.toolStripSeparator6,
            this.mnuAdvAutoRedraw});
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.advancedToolStripMenuItem.Text = "Advanced";
            // 
            // mnuAdvShades
            // 
            this.mnuAdvShades.Checked = true;
            this.mnuAdvShades.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuAdvShades.Name = "mnuAdvShades";
            this.mnuAdvShades.Size = new System.Drawing.Size(207, 22);
            this.mnuAdvShades.Text = "Draw Shades";
            this.mnuAdvShades.Click += new System.EventHandler(this.mnuAdvShades_Click);
            // 
            // mnuAdvPoints
            // 
            this.mnuAdvPoints.Name = "mnuAdvPoints";
            this.mnuAdvPoints.Size = new System.Drawing.Size(207, 22);
            this.mnuAdvPoints.Text = "Draw Points";
            this.mnuAdvPoints.Click += new System.EventHandler(this.mnuAdvPoints_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(204, 6);
            // 
            // mnuAdvBounds
            // 
            this.mnuAdvBounds.Name = "mnuAdvBounds";
            this.mnuAdvBounds.Size = new System.Drawing.Size(207, 22);
            this.mnuAdvBounds.Text = "Set Bounds...";
            this.mnuAdvBounds.Click += new System.EventHandler(this.mnuAdvBounds_Click);
            // 
            // mnuAdvAddQuality
            // 
            this.mnuAdvAddQuality.Name = "mnuAdvAddQuality";
            this.mnuAdvAddQuality.Size = new System.Drawing.Size(207, 22);
            this.mnuAdvAddQuality.Text = "Additional Quality...";
            this.mnuAdvAddQuality.Click += new System.EventHandler(this.mnuAdvAddQuality_Click);
            // 
            // mnuAdvAutoRedraw
            // 
            this.mnuAdvAutoRedraw.Name = "mnuAdvAutoRedraw";
            this.mnuAdvAutoRedraw.Size = new System.Drawing.Size(207, 22);
            this.mnuAdvAutoRedraw.Text = "Redraw when c1 changes";
            this.mnuAdvAutoRedraw.Click += new System.EventHandler(this.mnuAdvAutoRedraw_Click);
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
            this.mnuHelpAbout.Size = new System.Drawing.Size(152, 22);
            this.mnuHelpAbout.Text = "About";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(204, 6);
            // 
            // tbParam3
            // 
            this.tbParam3.LargeChange = 10;
            this.tbParam3.Location = new System.Drawing.Point(501, 540);
            this.tbParam3.Maximum = 10000;
            this.tbParam3.Name = "tbParam3";
            this.tbParam3.Size = new System.Drawing.Size(350, 45);
            this.tbParam3.TabIndex = 3;
            this.tbParam3.Scroll += new System.EventHandler(this.tbParam3_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(444, 540);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "c3";
            // 
            // lblParam3
            // 
            this.lblParam3.AutoSize = true;
            this.lblParam3.Location = new System.Drawing.Point(443, 553);
            this.lblParam3.Name = "lblParam3";
            this.lblParam3.Size = new System.Drawing.Size(15, 13);
            this.lblParam3.TabIndex = 4;
            this.lblParam3.Text = " x";
            // 
            // tbParam4
            // 
            this.tbParam4.LargeChange = 10;
            this.tbParam4.Location = new System.Drawing.Point(501, 572);
            this.tbParam4.Maximum = 10000;
            this.tbParam4.Name = "tbParam4";
            this.tbParam4.Size = new System.Drawing.Size(350, 45);
            this.tbParam4.TabIndex = 3;
            this.tbParam4.Scroll += new System.EventHandler(this.tbParam4_Scroll);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(444, 572);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(19, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "c4";
            // 
            // lblParam4
            // 
            this.lblParam4.AutoSize = true;
            this.lblParam4.Location = new System.Drawing.Point(443, 585);
            this.lblParam4.Name = "lblParam4";
            this.lblParam4.Size = new System.Drawing.Size(15, 13);
            this.lblParam4.TabIndex = 4;
            this.lblParam4.Text = " x";
            // 
            // txtP0
            // 
            this.txtP0.Location = new System.Drawing.Point(693, 474);
            this.txtP0.Name = "txtP0";
            this.txtP0.Size = new System.Drawing.Size(127, 20);
            this.txtP0.TabIndex = 21;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(637, 499);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(50, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "init. code";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(652, 477);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "P0 =";
            // 
            // txtInit
            // 
            this.txtInit.Location = new System.Drawing.Point(693, 497);
            this.txtInit.Multiline = true;
            this.txtInit.Name = "txtInit";
            this.txtInit.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInit.Size = new System.Drawing.Size(147, 43);
            this.txtInit.TabIndex = 18;
            // 
            // pointPlotBifurcationUserControl1
            // 
            this.pointPlotBifurcationUserControl1.Location = new System.Drawing.Point(12, 27);
            this.pointPlotBifurcationUserControl1.Name = "pointPlotBifurcationUserControl1";
            this.pointPlotBifurcationUserControl1.Size = new System.Drawing.Size(415, 432);
            this.pointPlotBifurcationUserControl1.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(863, 623);
            this.Controls.Add(this.txtP0);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtInit);
            this.Controls.Add(this.pointPlotBifurcationUserControl1);
            this.Controls.Add(this.lblParam2);
            this.Controls.Add(this.lblParam4);
            this.Controls.Add(this.lblParam3);
            this.Controls.Add(this.lblParam1);
            this.Controls.Add(this.lblShading);
            this.Controls.Add(this.lblSettling);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbParam2);
            this.Controls.Add(this.tbParam4);
            this.Controls.Add(this.tbParam3);
            this.Controls.Add(this.tbParam1);
            this.Controls.Add(this.tbShading);
            this.Controls.Add(this.tbSettling);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtExpression);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "CsBifcuration";
            ((System.ComponentModel.ISupportInitialize)(this.tbSettling)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam2)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtExpression;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TrackBar tbSettling;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSettling;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar tbShading;
        private System.Windows.Forms.Label lblShading;
        private System.Windows.Forms.TrackBar tbParam1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblParam1;
        private System.Windows.Forms.TrackBar tbParam2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblParam2;
        private PlotBitmapBifurcationControl pointPlotBifurcationUserControl1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advancedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuAdvShades;
        private System.Windows.Forms.ToolStripMenuItem mnuAdvPoints;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuAdvBounds;
        private System.Windows.Forms.ToolStripMenuItem mnuAdvAddQuality;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem mnuAdvAutoRedraw;
        private System.Windows.Forms.ToolStripMenuItem mnuViewZoomIn;
        private System.Windows.Forms.ToolStripMenuItem mnuViewZoomOut;
        private System.Windows.Forms.ToolStripMenuItem mnuViewZoomUndo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mnuViewReset;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem mnuViewRedraw;
        private System.Windows.Forms.ToolStripMenuItem mnuFileRender;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem mnuFileAnimate;
        private System.Windows.Forms.ToolStripMenuItem mnuFileNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.TrackBar tbParam3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblParam3;
        private System.Windows.Forms.TrackBar tbParam4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblParam4;
        private System.Windows.Forms.TextBox txtP0;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtInit;
    }
}


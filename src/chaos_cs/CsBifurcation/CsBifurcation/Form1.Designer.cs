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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtP0 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtInit = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileRender = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewRedraw = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdvShades = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdvPoints = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.setBoundsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.additionalQualityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdvAutoRedraw = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.pointPlotBifurcationUserControl1 = new CsBifurcation.PlotBitmapBifurcationControl();
            ((System.ComponentModel.ISupportInitialize)(this.tbSettling)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtExpression
            // 
            this.txtExpression.Location = new System.Drawing.Point(519, 127);
            this.txtExpression.Multiline = true;
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExpression.Size = new System.Drawing.Size(209, 61);
            this.txtExpression.TabIndex = 0;
            this.txtExpression.Text = "p = r*p*(1-p);";
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(519, 194);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(210, 50);
            this.btnGo.TabIndex = 1;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // tbSettling
            // 
            this.tbSettling.Location = new System.Drawing.Point(68, 512);
            this.tbSettling.Maximum = 1000;
            this.tbSettling.Name = "tbSettling";
            this.tbSettling.Size = new System.Drawing.Size(370, 45);
            this.tbSettling.TabIndex = 3;
            this.tbSettling.Scroll += new System.EventHandler(this.tbSettling_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 514);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Settling";
            // 
            // lblSettling
            // 
            this.lblSettling.AutoSize = true;
            this.lblSettling.Location = new System.Drawing.Point(11, 527);
            this.lblSettling.Name = "lblSettling";
            this.lblSettling.Size = new System.Drawing.Size(12, 13);
            this.lblSettling.TabIndex = 4;
            this.lblSettling.Text = "x";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 547);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Shading";
            // 
            // tbShading
            // 
            this.tbShading.Location = new System.Drawing.Point(68, 547);
            this.tbShading.Maximum = 1000;
            this.tbShading.Name = "tbShading";
            this.tbShading.Size = new System.Drawing.Size(370, 45);
            this.tbShading.TabIndex = 3;
            this.tbShading.Scroll += new System.EventHandler(this.tbShading_Scroll);
            // 
            // lblShading
            // 
            this.lblShading.AutoSize = true;
            this.lblShading.Location = new System.Drawing.Point(11, 560);
            this.lblShading.Name = "lblShading";
            this.lblShading.Size = new System.Drawing.Size(12, 13);
            this.lblShading.TabIndex = 4;
            this.lblShading.Text = "x";
            // 
            // tbParam1
            // 
            this.tbParam1.Location = new System.Drawing.Point(68, 585);
            this.tbParam1.Maximum = 1000;
            this.tbParam1.Name = "tbParam1";
            this.tbParam1.Size = new System.Drawing.Size(370, 45);
            this.tbParam1.TabIndex = 3;
            this.tbParam1.Scroll += new System.EventHandler(this.tbParam1_Scroll);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 585);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "c1";
            // 
            // lblParam1
            // 
            this.lblParam1.AutoSize = true;
            this.lblParam1.Location = new System.Drawing.Point(11, 598);
            this.lblParam1.Name = "lblParam1";
            this.lblParam1.Size = new System.Drawing.Size(15, 13);
            this.lblParam1.TabIndex = 4;
            this.lblParam1.Text = " x";
            // 
            // tbParam2
            // 
            this.tbParam2.Location = new System.Drawing.Point(68, 618);
            this.tbParam2.Maximum = 1000;
            this.tbParam2.Name = "tbParam2";
            this.tbParam2.Size = new System.Drawing.Size(370, 45);
            this.tbParam2.TabIndex = 3;
            this.tbParam2.Scroll += new System.EventHandler(this.tbParam2_Scroll);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 618);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(19, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "c2";
            // 
            // lblParam2
            // 
            this.lblParam2.AutoSize = true;
            this.lblParam2.Location = new System.Drawing.Point(11, 631);
            this.lblParam2.Name = "lblParam2";
            this.lblParam2.Size = new System.Drawing.Size(15, 13);
            this.lblParam2.TabIndex = 4;
            this.lblParam2.Text = "x ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(749, 590);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Ben Fisher, 2010";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtP0);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtInit);
            this.groupBox1.Location = new System.Drawing.Point(471, 466);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(272, 134);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " ";
            // 
            // txtP0
            // 
            this.txtP0.Location = new System.Drawing.Point(74, 46);
            this.txtP0.Name = "txtP0";
            this.txtP0.Size = new System.Drawing.Size(84, 20);
            this.txtP0.TabIndex = 17;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 71);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(50, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "init. code";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(33, 49);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "P0 =";
            // 
            // txtInit
            // 
            this.txtInit.Location = new System.Drawing.Point(74, 69);
            this.txtInit.Multiline = true;
            this.txtInit.Name = "txtInit";
            this.txtInit.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInit.Size = new System.Drawing.Size(147, 43);
            this.txtInit.TabIndex = 13;
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
            this.menuStrip1.Size = new System.Drawing.Size(840, 24);
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
            this.toolStripMenuItem1,
            this.toolStripSeparator5,
            this.exitToolStripMenuItem});
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
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(195, 22);
            this.toolStripMenuItem1.Text = "Create Animation...";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(192, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem,
            this.undoZoomToolStripMenuItem,
            this.toolStripSeparator3,
            this.resetToolStripMenuItem,
            this.toolStripSeparator4,
            this.mnuViewRedraw});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.zoomInToolStripMenuItem.Text = "Zoom In";
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.zoomOutToolStripMenuItem.Text = "Zoom Out";
            // 
            // undoZoomToolStripMenuItem
            // 
            this.undoZoomToolStripMenuItem.Name = "undoZoomToolStripMenuItem";
            this.undoZoomToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoZoomToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.undoZoomToolStripMenuItem.Text = "Undo Zoom";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(176, 6);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.resetToolStripMenuItem.Text = "Reset";
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
            this.setBoundsToolStripMenuItem,
            this.additionalQualityToolStripMenuItem,
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
            // setBoundsToolStripMenuItem
            // 
            this.setBoundsToolStripMenuItem.Name = "setBoundsToolStripMenuItem";
            this.setBoundsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.setBoundsToolStripMenuItem.Text = "Set Bounds...";
            // 
            // additionalQualityToolStripMenuItem
            // 
            this.additionalQualityToolStripMenuItem.Name = "additionalQualityToolStripMenuItem";
            this.additionalQualityToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.additionalQualityToolStripMenuItem.Text = "Additional Quality...";
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
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(204, 6);
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
            this.ClientSize = new System.Drawing.Size(840, 670);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pointPlotBifurcationUserControl1);
            this.Controls.Add(this.lblParam2);
            this.Controls.Add(this.lblParam1);
            this.Controls.Add(this.lblShading);
            this.Controls.Add(this.lblSettling);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbParam2);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtP0;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtInit;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advancedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuAdvShades;
        private System.Windows.Forms.ToolStripMenuItem mnuAdvPoints;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem setBoundsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem additionalQualityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuAdvAutoRedraw;
        private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoZoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem mnuViewRedraw;
        private System.Windows.Forms.ToolStripMenuItem mnuFileRender;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
    }
}


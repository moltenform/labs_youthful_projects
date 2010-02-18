namespace chaosExplorerControl
{
    partial class PointPlotUserControl
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PointPlotUserControl));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mnuMode = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoomUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZmWiden = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZmHeighten = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuReset = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuRender = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRenderLarge = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopyParams = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCoords = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMode,
            this.toolStripStatusLabel1,
            this.lblCoords});
            this.statusStrip1.Location = new System.Drawing.Point(0, 271);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(345, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // mnuMode
            // 
            this.mnuMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuZoomIn,
            this.mnuZoomOut,
            this.mnuZoomUndo,
            this.mnuZmWiden,
            this.mnuZmHeighten,
            this.toolStripSeparator1,
            this.mnuReset,
            this.toolStripSeparator2,
            this.mnuRender,
            this.mnuRenderLarge,
            this.mnuCopyParams});
            this.mnuMode.Image = ((System.Drawing.Image)(resources.GetObject("mnuMode.Image")));
            this.mnuMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuMode.Name = "mnuMode";
            this.mnuMode.Size = new System.Drawing.Size(45, 20);
            this.mnuMode.Text = "View";
            // 
            // mnuZoomIn
            // 
            this.mnuZoomIn.Name = "mnuZoomIn";
            this.mnuZoomIn.Size = new System.Drawing.Size(152, 22);
            this.mnuZoomIn.Text = "Zoom In";
            this.mnuZoomIn.Click += new System.EventHandler(this.mnuZoomIn_Click);
            // 
            // mnuZoomOut
            // 
            this.mnuZoomOut.Name = "mnuZoomOut";
            this.mnuZoomOut.Size = new System.Drawing.Size(152, 22);
            this.mnuZoomOut.Text = "Zoom Out";
            this.mnuZoomOut.Click += new System.EventHandler(this.mnuZoomOut_Click);
            // 
            // mnuZoomUndo
            // 
            this.mnuZoomUndo.Name = "mnuZoomUndo";
            this.mnuZoomUndo.Size = new System.Drawing.Size(152, 22);
            this.mnuZoomUndo.Text = "Undo Zoom";
            this.mnuZoomUndo.Click += new System.EventHandler(this.mnuZoomUndo_Click);
            // 
            // mnuZmWiden
            // 
            this.mnuZmWiden.Name = "mnuZmWiden";
            this.mnuZmWiden.Size = new System.Drawing.Size(152, 22);
            this.mnuZmWiden.Text = "Widen View";
            this.mnuZmWiden.Click += new System.EventHandler(this.mnuZmWiden_Click);
            // 
            // mnuZmHeighten
            // 
            this.mnuZmHeighten.Name = "mnuZmHeighten";
            this.mnuZmHeighten.Size = new System.Drawing.Size(152, 22);
            this.mnuZmHeighten.Text = "Heighten View";
            this.mnuZmHeighten.Click += new System.EventHandler(this.mnuZmHeighten_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // mnuReset
            // 
            this.mnuReset.Name = "mnuReset";
            this.mnuReset.Size = new System.Drawing.Size(152, 22);
            this.mnuReset.Text = "Reset";
            this.mnuReset.Click += new System.EventHandler(this.mnuReset_Click_1);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // mnuRender
            // 
            this.mnuRender.Name = "mnuRender";
            this.mnuRender.Size = new System.Drawing.Size(152, 22);
            this.mnuRender.Text = "Render";
            this.mnuRender.Click += new System.EventHandler(this.mnuRender_Click);
            // 
            // mnuRenderLarge
            // 
            this.mnuRenderLarge.Name = "mnuRenderLarge";
            this.mnuRenderLarge.Size = new System.Drawing.Size(152, 22);
            this.mnuRenderLarge.Text = "Render Large";
            this.mnuRenderLarge.Click += new System.EventHandler(this.mnuRenderLarge_Click);
            // 
            // mnuCopyParams
            // 
            this.mnuCopyParams.Name = "mnuCopyParams";
            this.mnuCopyParams.Size = new System.Drawing.Size(152, 22);
            this.mnuCopyParams.Text = "Copy Params";
            this.mnuCopyParams.Click += new System.EventHandler(this.mnuCopyParams_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(46, 17);
            this.toolStripStatusLabel1.Text = "             ";
            // 
            // lblCoords
            // 
            this.lblCoords.Name = "lblCoords";
            this.lblCoords.Size = new System.Drawing.Size(22, 17);
            this.lblCoords.Text = "0,0";
            // 
            // PointPlotUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.statusStrip1);
            this.Name = "PointPlotUserControl";
            this.Size = new System.Drawing.Size(345, 293);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripDropDownButton mnuMode;
        private System.Windows.Forms.ToolStripMenuItem mnuReset;
        private System.Windows.Forms.ToolStripStatusLabel lblCoords;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuZoomOut;
        private System.Windows.Forms.ToolStripMenuItem mnuZoomIn;
        private System.Windows.Forms.ToolStripMenuItem mnuZoomUndo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuRender;
        private System.Windows.Forms.ToolStripMenuItem mnuRenderLarge;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyParams;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem mnuZmWiden;
        private System.Windows.Forms.ToolStripMenuItem mnuZmHeighten;
    }
}

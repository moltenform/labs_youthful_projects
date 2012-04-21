namespace BurgerManagerie
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.filieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.editPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewBreathing = new System.Windows.Forms.ToolStripMenuItem();
            this.viewIncSliding = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDecSliding = new System.Windows.Forms.ToolStripMenuItem();
            this.viewIncSettling = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDecSettling = new System.Windows.Forms.ToolStripMenuItem();
            this.viewIncShading = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDecShading = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.viewNudgePosition = new System.Windows.Forms.ToolStripMenuItem();
            this.viewSetPos = new System.Windows.Forms.ToolStripMenuItem();
            this.viewZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.viewZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.viewResest = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filieToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(765, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // filieToolStripMenuItem
            // 
            this.filieToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileNew,
            this.fileOpen,
            this.fileSave,
            this.toolStripSeparator1,
            this.fileExit});
            this.filieToolStripMenuItem.Name = "filieToolStripMenuItem";
            this.filieToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.filieToolStripMenuItem.Text = "File";
            // 
            // fileNew
            // 
            this.fileNew.Name = "fileNew";
            this.fileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.fileNew.Size = new System.Drawing.Size(200, 22);
            this.fileNew.Text = "New";
            // 
            // fileOpen
            // 
            this.fileOpen.Name = "fileOpen";
            this.fileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.fileOpen.Size = new System.Drawing.Size(200, 22);
            this.fileOpen.Text = "Open Favorite...";
            // 
            // fileSave
            // 
            this.fileSave.Name = "fileSave";
            this.fileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.fileSave.Size = new System.Drawing.Size(200, 22);
            this.fileSave.Text = "Save Favorite...";
            this.fileSave.Click += new System.EventHandler(this.fileSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(197, 6);
            // 
            // fileExit
            // 
            this.fileExit.Name = "fileExit";
            this.fileExit.Size = new System.Drawing.Size(200, 22);
            this.fileExit.Text = "Exit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editCopy,
            this.editPaste});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // editCopy
            // 
            this.editCopy.Name = "editCopy";
            this.editCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.editCopy.Size = new System.Drawing.Size(193, 22);
            this.editCopy.Text = "Copy Location";
            this.editCopy.Click += new System.EventHandler(this.editCopy_Click);
            // 
            // editPaste
            // 
            this.editPaste.Name = "editPaste";
            this.editPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.editPaste.Size = new System.Drawing.Size(193, 22);
            this.editPaste.Text = "Paste Location";
            this.editPaste.Click += new System.EventHandler(this.editPaste_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewBreathing,
            this.viewIncSliding,
            this.viewDecSliding,
            this.viewIncSettling,
            this.viewDecSettling,
            this.viewIncShading,
            this.viewDecShading,
            this.toolStripSeparator2,
            this.viewNudgePosition,
            this.viewSetPos,
            this.viewZoomIn,
            this.viewZoomOut,
            this.viewResest});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // viewBreathing
            // 
            this.viewBreathing.Checked = true;
            this.viewBreathing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewBreathing.Name = "viewBreathing";
            this.viewBreathing.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.viewBreathing.Size = new System.Drawing.Size(220, 22);
            this.viewBreathing.Text = "Breathing";
            // 
            // viewIncSliding
            // 
            this.viewIncSliding.Name = "viewIncSliding";
            this.viewIncSliding.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D0)));
            this.viewIncSliding.Size = new System.Drawing.Size(220, 22);
            this.viewIncSliding.Text = "Increase Sliding";
            // 
            // viewDecSliding
            // 
            this.viewDecSliding.Name = "viewDecSliding";
            this.viewDecSliding.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D9)));
            this.viewDecSliding.Size = new System.Drawing.Size(220, 22);
            this.viewDecSliding.Text = "Decrease Sliding";
            // 
            // viewIncSettling
            // 
            this.viewIncSettling.Name = "viewIncSettling";
            this.viewIncSettling.Size = new System.Drawing.Size(220, 22);
            this.viewIncSettling.Text = "Increase Settling";
            // 
            // viewDecSettling
            // 
            this.viewDecSettling.Name = "viewDecSettling";
            this.viewDecSettling.Size = new System.Drawing.Size(220, 22);
            this.viewDecSettling.Text = "Decrease Settling";
            // 
            // viewIncShading
            // 
            this.viewIncShading.Name = "viewIncShading";
            this.viewIncShading.Size = new System.Drawing.Size(220, 22);
            this.viewIncShading.Text = "Increase Shade";
            // 
            // viewDecShading
            // 
            this.viewDecShading.Name = "viewDecShading";
            this.viewDecShading.Size = new System.Drawing.Size(220, 22);
            this.viewDecShading.Text = "Decrease Shade";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(217, 6);
            // 
            // viewNudgePosition
            // 
            this.viewNudgePosition.Name = "viewNudgePosition";
            this.viewNudgePosition.Size = new System.Drawing.Size(220, 22);
            this.viewNudgePosition.Text = "Nudge Position... (not here)";
            // 
            // viewSetPos
            // 
            this.viewSetPos.Name = "viewSetPos";
            this.viewSetPos.Size = new System.Drawing.Size(220, 22);
            this.viewSetPos.Text = "Set Position...";
            this.viewSetPos.Click += new System.EventHandler(this.viewSetPos_Click);
            // 
            // viewZoomIn
            // 
            this.viewZoomIn.Name = "viewZoomIn";
            this.viewZoomIn.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.J)));
            this.viewZoomIn.Size = new System.Drawing.Size(220, 22);
            this.viewZoomIn.Text = "Zoom In";
            this.viewZoomIn.Click += new System.EventHandler(this.viewZoomIn_Click);
            // 
            // viewZoomOut
            // 
            this.viewZoomOut.Name = "viewZoomOut";
            this.viewZoomOut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.viewZoomOut.Size = new System.Drawing.Size(220, 22);
            this.viewZoomOut.Text = "Zoom Out";
            // 
            // viewResest
            // 
            this.viewResest.Name = "viewResest";
            this.viewResest.Size = new System.Drawing.Size(220, 22);
            this.viewResest.Text = "Reset";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(12, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(742, 467);
            this.panel1.TabIndex = 4;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(2048, 2048);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 507);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Menagerie";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem filieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileNew;
        private System.Windows.Forms.ToolStripMenuItem fileOpen;
        private System.Windows.Forms.ToolStripMenuItem fileSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem fileExit;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editCopy;
        private System.Windows.Forms.ToolStripMenuItem editPaste;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewBreathing;
        private System.Windows.Forms.ToolStripMenuItem viewIncSettling;
        private System.Windows.Forms.ToolStripMenuItem viewDecSettling;
        private System.Windows.Forms.ToolStripMenuItem viewIncShading;
        private System.Windows.Forms.ToolStripMenuItem viewDecShading;
        private System.Windows.Forms.ToolStripMenuItem viewNudgePosition;
        private System.Windows.Forms.ToolStripMenuItem viewSetPos;
        private System.Windows.Forms.ToolStripMenuItem viewIncSliding;
        private System.Windows.Forms.ToolStripMenuItem viewDecSliding;
        private System.Windows.Forms.ToolStripMenuItem viewZoomIn;
        private System.Windows.Forms.ToolStripMenuItem viewZoomOut;
        private System.Windows.Forms.ToolStripMenuItem viewResest;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}


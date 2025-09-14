namespace rbcpy
{
    partial class FormSyncItems
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSyncItems));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtSummary = new System.Windows.Forms.TextBox();
            this.lblNameOfAction = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnCompareWinmerge = new System.Windows.Forms.Button();
            this.btnIncludeBoth = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSelectedDetailsLeft = new System.Windows.Forms.TextBox();
            this.txtSelectedDetailsRight = new System.Windows.Forms.TextBox();
            this.btnLeftToRight = new System.Windows.Forms.Button();
            this.btnRightToLeft = new System.Windows.Forms.Button();
            this.btnShowLeft = new System.Windows.Forms.Button();
            this.btnShowRight = new System.Windows.Forms.Button();
            this.listView = new System.Windows.Forms.ListView();
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopyAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopySelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewSummary = new System.Windows.Forms.ToolStripMenuItem();
            this.viewIncludeTempEntries = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.txtSummary, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblNameOfAction, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.listView, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 33);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 394F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(976, 876);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // txtSummary
            // 
            this.txtSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSummary.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSummary.Location = new System.Drawing.Point(4, 676);
            this.txtSummary.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSummary.Multiline = true;
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.Size = new System.Drawing.Size(968, 118);
            this.txtSummary.TabIndex = 9;
            this.txtSummary.Text = "Summary";
            // 
            // lblNameOfAction
            // 
            this.lblNameOfAction.AutoSize = true;
            this.lblNameOfAction.Location = new System.Drawing.Point(4, 0);
            this.lblNameOfAction.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNameOfAction.Name = "lblNameOfAction";
            this.lblNameOfAction.Size = new System.Drawing.Size(67, 20);
            this.lblNameOfAction.TabIndex = 5;
            this.lblNameOfAction.Text = "Preview:";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 270F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 147F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 129F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.btnRun, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnCompareWinmerge, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnIncludeBoth, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 804);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(968, 67);
            this.tableLayoutPanel2.TabIndex = 10;
            // 
            // btnRun
            // 
            this.btnRun.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRun.Location = new System.Drawing.Point(4, 5);
            this.btnRun.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(262, 57);
            this.btnRun.TabIndex = 0;
            this.btnRun.Text = "Run Synchronization";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnCompareWinmerge
            // 
            this.btnCompareWinmerge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCompareWinmerge.Location = new System.Drawing.Point(274, 5);
            this.btnCompareWinmerge.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCompareWinmerge.Name = "btnCompareWinmerge";
            this.btnCompareWinmerge.Size = new System.Drawing.Size(139, 57);
            this.btnCompareWinmerge.TabIndex = 1;
            this.btnCompareWinmerge.Text = "Compare in WinMerge";
            this.btnCompareWinmerge.UseVisualStyleBackColor = true;
            this.btnCompareWinmerge.Click += new System.EventHandler(this.btnCompareWinmerge_Click);
            // 
            // btnIncludeBoth
            // 
            this.btnIncludeBoth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnIncludeBoth.Location = new System.Drawing.Point(421, 5);
            this.btnIncludeBoth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnIncludeBoth.Name = "btnIncludeBoth";
            this.btnIncludeBoth.Size = new System.Drawing.Size(121, 57);
            this.btnIncludeBoth.TabIndex = 2;
            this.btnIncludeBoth.Text = "Include both...";
            this.btnIncludeBoth.UseVisualStyleBackColor = true;
            this.btnIncludeBoth.Click += new System.EventHandler(this.btnIncludeBoth_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 5;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tableLayoutPanel3.Controls.Add(this.textBox1, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label3, 3, 1);
            this.tableLayoutPanel3.Controls.Add(this.txtSelectedDetailsLeft, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.txtSelectedDetailsRight, 3, 2);
            this.tableLayoutPanel3.Controls.Add(this.btnLeftToRight, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.btnRightToLeft, 3, 3);
            this.tableLayoutPanel3.Controls.Add(this.btnShowLeft, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.btnShowRight, 3, 4);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(4, 282);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(968, 384);
            this.tableLayoutPanel3.TabIndex = 11;
            // 
            // textBox1
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.textBox1, 3);
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(49, 5);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(868, 58);
            this.textBox1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(49, 68);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(400, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Left:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(517, 68);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(400, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Right:";
            // 
            // txtSelectedDetailsLeft
            // 
            this.txtSelectedDetailsLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSelectedDetailsLeft.Location = new System.Drawing.Point(49, 93);
            this.txtSelectedDetailsLeft.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSelectedDetailsLeft.Multiline = true;
            this.txtSelectedDetailsLeft.Name = "txtSelectedDetailsLeft";
            this.txtSelectedDetailsLeft.ReadOnly = true;
            this.txtSelectedDetailsLeft.Size = new System.Drawing.Size(400, 125);
            this.txtSelectedDetailsLeft.TabIndex = 5;
            // 
            // txtSelectedDetailsRight
            // 
            this.txtSelectedDetailsRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSelectedDetailsRight.Location = new System.Drawing.Point(517, 93);
            this.txtSelectedDetailsRight.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSelectedDetailsRight.Multiline = true;
            this.txtSelectedDetailsRight.Name = "txtSelectedDetailsRight";
            this.txtSelectedDetailsRight.ReadOnly = true;
            this.txtSelectedDetailsRight.Size = new System.Drawing.Size(400, 125);
            this.txtSelectedDetailsRight.TabIndex = 6;
            // 
            // btnLeftToRight
            // 
            this.btnLeftToRight.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnLeftToRight.Location = new System.Drawing.Point(49, 228);
            this.btnLeftToRight.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnLeftToRight.Name = "btnLeftToRight";
            this.btnLeftToRight.Size = new System.Drawing.Size(186, 35);
            this.btnLeftToRight.TabIndex = 7;
            this.btnLeftToRight.Text = "Copy Left --> Right";
            this.btnLeftToRight.UseVisualStyleBackColor = true;
            this.btnLeftToRight.Click += new System.EventHandler(this.btnLeftToRight_Click);
            // 
            // btnRightToLeft
            // 
            this.btnRightToLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnRightToLeft.Location = new System.Drawing.Point(517, 228);
            this.btnRightToLeft.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRightToLeft.Name = "btnRightToLeft";
            this.btnRightToLeft.Size = new System.Drawing.Size(195, 35);
            this.btnRightToLeft.TabIndex = 8;
            this.btnRightToLeft.Text = "Copy Left <-- Right";
            this.btnRightToLeft.UseVisualStyleBackColor = true;
            this.btnRightToLeft.Click += new System.EventHandler(this.btnRightToLeft_Click);
            // 
            // btnShowLeft
            // 
            this.btnShowLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnShowLeft.Location = new System.Drawing.Point(49, 273);
            this.btnShowLeft.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnShowLeft.Name = "btnShowLeft";
            this.btnShowLeft.Size = new System.Drawing.Size(186, 35);
            this.btnShowLeft.TabIndex = 9;
            this.btnShowLeft.Text = "Show on Disk";
            this.btnShowLeft.UseVisualStyleBackColor = true;
            this.btnShowLeft.Click += new System.EventHandler(this.btnShowLeft_Click);
            // 
            // btnShowRight
            // 
            this.btnShowRight.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnShowRight.Location = new System.Drawing.Point(517, 273);
            this.btnShowRight.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnShowRight.Name = "btnShowRight";
            this.btnShowRight.Size = new System.Drawing.Size(195, 35);
            this.btnShowRight.TabIndex = 10;
            this.btnShowRight.Text = "Show on Disk";
            this.btnShowRight.UseVisualStyleBackColor = true;
            this.btnShowRight.Click += new System.EventHandler(this.btnShowRight_Click);
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colType,
            this.colAction,
            this.colPath});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(4, 25);
            this.listView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(968, 247);
            this.listView.TabIndex = 12;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            this.listView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView_MouseDoubleClick);
            // 
            // colType
            // 
            this.colType.Text = "Type";
            this.colType.Width = 80;
            // 
            // colAction
            // 
            this.colAction.Text = "Action";
            this.colAction.Width = 80;
            // 
            // colPath
            // 
            this.colPath.Text = "Path";
            this.colPath.Width = 430;
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCopyAll,
            this.mnuCopySelected,
            this.toolStripMenuItem3,
            this.mnuViewSummary,
            this.viewIncludeTempEntries});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(58, 29);
            this.toolStripMenuItem1.Text = "Edit";
            // 
            // mnuCopyAll
            // 
            this.mnuCopyAll.Name = "mnuCopyAll";
            this.mnuCopyAll.Size = new System.Drawing.Size(311, 34);
            this.mnuCopyAll.Text = "Copy All";
            this.mnuCopyAll.Click += new System.EventHandler(this.mnuCopyAll_Click);
            // 
            // mnuCopySelected
            // 
            this.mnuCopySelected.Name = "mnuCopySelected";
            this.mnuCopySelected.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.mnuCopySelected.Size = new System.Drawing.Size(311, 34);
            this.mnuCopySelected.Text = "Copy Selected";
            this.mnuCopySelected.Click += new System.EventHandler(this.mnuCopySelected_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(308, 6);
            // 
            // mnuViewSummary
            // 
            this.mnuViewSummary.Name = "mnuViewSummary";
            this.mnuViewSummary.Size = new System.Drawing.Size(311, 34);
            this.mnuViewSummary.Text = "View Summary";
            this.mnuViewSummary.Click += new System.EventHandler(this.mnuViewSummary_Click);
            // 
            // viewIncludeTempEntries
            // 
            this.viewIncludeTempEntries.Name = "viewIncludeTempEntries";
            this.viewIncludeTempEntries.Size = new System.Drawing.Size(311, 34);
            this.viewIncludeTempEntries.Text = "Include Temp Log Entries";
            this.viewIncludeTempEntries.Click += new System.EventHandler(this.viewIncludeTempEntries_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(976, 33);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "empty.png");
            this.imageList1.Images.SetKeyName(1, "go-next.png");
            this.imageList1.Images.SetKeyName(2, "go-next-upd.png");
            this.imageList1.Images.SetKeyName(3, "go-next-updwarn.png");
            this.imageList1.Images.SetKeyName(4, "list-remove.png");
            // 
            // FormSyncItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 909);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FormSyncItems";
            this.Text = "Preview";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSyncItems_FormClosed);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblNameOfAction;
        private System.Windows.Forms.TextBox txtSummary;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCompareWinmerge;
        private System.Windows.Forms.TextBox txtSelectedDetailsLeft;
        private System.Windows.Forms.TextBox txtSelectedDetailsRight;
        private System.Windows.Forms.Button btnLeftToRight;
        private System.Windows.Forms.Button btnRightToLeft;
        private System.Windows.Forms.Button btnShowLeft;
        private System.Windows.Forms.Button btnShowRight;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyAll;
        private System.Windows.Forms.ToolStripMenuItem mnuCopySelected;
        private System.Windows.Forms.ToolStripMenuItem mnuViewSummary;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewIncludeTempEntries;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colAction;
        private System.Windows.Forms.ColumnHeader colPath;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.Button btnIncludeBoth;
    }
}
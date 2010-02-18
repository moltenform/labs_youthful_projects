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
            this.btnSave = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBounds = new System.Windows.Forms.Button();
            this.txtP0 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.rdoPoints = new System.Windows.Forms.RadioButton();
            this.rdoShade = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtInit = new System.Windows.Forms.TextBox();
            this.pointPlotBifurcationUserControl1 = new CsBifurcation.PointPlotBifurcationUserControl();
            ((System.ComponentModel.ISupportInitialize)(this.tbSettling)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtExpression
            // 
            this.txtExpression.Location = new System.Drawing.Point(545, 180);
            this.txtExpression.Multiline = true;
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.Size = new System.Drawing.Size(167, 43);
            this.txtExpression.TabIndex = 0;
            this.txtExpression.Text = "p = r*p*(1-p);";
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(545, 229);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(167, 40);
            this.btnGo.TabIndex = 1;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // tbSettling
            // 
            this.tbSettling.Location = new System.Drawing.Point(66, 467);
            this.tbSettling.Maximum = 1000;
            this.tbSettling.Name = "tbSettling";
            this.tbSettling.Size = new System.Drawing.Size(370, 45);
            this.tbSettling.TabIndex = 3;
            this.tbSettling.Scroll += new System.EventHandler(this.tbSettling_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 469);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Settling";
            // 
            // lblSettling
            // 
            this.lblSettling.AutoSize = true;
            this.lblSettling.Location = new System.Drawing.Point(9, 482);
            this.lblSettling.Name = "lblSettling";
            this.lblSettling.Size = new System.Drawing.Size(12, 13);
            this.lblSettling.TabIndex = 4;
            this.lblSettling.Text = "x";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 502);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Darkness";
            // 
            // tbShading
            // 
            this.tbShading.Location = new System.Drawing.Point(66, 502);
            this.tbShading.Maximum = 1000;
            this.tbShading.Name = "tbShading";
            this.tbShading.Size = new System.Drawing.Size(370, 45);
            this.tbShading.TabIndex = 3;
            this.tbShading.Scroll += new System.EventHandler(this.tbShading_Scroll);
            // 
            // lblShading
            // 
            this.lblShading.AutoSize = true;
            this.lblShading.Location = new System.Drawing.Point(9, 515);
            this.lblShading.Name = "lblShading";
            this.lblShading.Size = new System.Drawing.Size(12, 13);
            this.lblShading.TabIndex = 4;
            this.lblShading.Text = "x";
            // 
            // tbParam1
            // 
            this.tbParam1.Location = new System.Drawing.Point(66, 540);
            this.tbParam1.Maximum = 1000;
            this.tbParam1.Name = "tbParam1";
            this.tbParam1.Size = new System.Drawing.Size(370, 45);
            this.tbParam1.TabIndex = 3;
            this.tbParam1.Scroll += new System.EventHandler(this.tbParam1_Scroll);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 540);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "c1";
            // 
            // lblParam1
            // 
            this.lblParam1.AutoSize = true;
            this.lblParam1.Location = new System.Drawing.Point(9, 553);
            this.lblParam1.Name = "lblParam1";
            this.lblParam1.Size = new System.Drawing.Size(15, 13);
            this.lblParam1.TabIndex = 4;
            this.lblParam1.Text = " x";
            // 
            // tbParam2
            // 
            this.tbParam2.Location = new System.Drawing.Point(66, 573);
            this.tbParam2.Maximum = 1000;
            this.tbParam2.Name = "tbParam2";
            this.tbParam2.Size = new System.Drawing.Size(370, 45);
            this.tbParam2.TabIndex = 3;
            this.tbParam2.Scroll += new System.EventHandler(this.tbParam2_Scroll);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 573);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(19, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "c2";
            // 
            // lblParam2
            // 
            this.lblParam2.AutoSize = true;
            this.lblParam2.Location = new System.Drawing.Point(9, 586);
            this.lblParam2.Name = "lblParam2";
            this.lblParam2.Size = new System.Drawing.Size(15, 13);
            this.lblParam2.TabIndex = 4;
            this.lblParam2.Text = "x ";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(778, 18);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(58, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save...";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(615, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Presets:";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "a",
            "b",
            "c"});
            this.comboBox1.Location = new System.Drawing.Point(666, 20);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(106, 21);
            this.comboBox1.TabIndex = 10;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
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
            this.groupBox1.Controls.Add(this.btnBounds);
            this.groupBox1.Controls.Add(this.txtP0);
            this.groupBox1.Controls.Add(this.panel1);
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
            // btnBounds
            // 
            this.btnBounds.Location = new System.Drawing.Point(166, 46);
            this.btnBounds.Name = "btnBounds";
            this.btnBounds.Size = new System.Drawing.Size(64, 20);
            this.btnBounds.TabIndex = 14;
            this.btnBounds.Text = "Bounds...";
            this.btnBounds.UseVisualStyleBackColor = true;
            this.btnBounds.Click += new System.EventHandler(this.btnBounds_Click);
            // 
            // txtP0
            // 
            this.txtP0.Location = new System.Drawing.Point(74, 46);
            this.txtP0.Name = "txtP0";
            this.txtP0.Size = new System.Drawing.Size(84, 20);
            this.txtP0.TabIndex = 17;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.rdoPoints);
            this.panel1.Controls.Add(this.rdoShade);
            this.panel1.Location = new System.Drawing.Point(21, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(237, 25);
            this.panel1.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(2, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Drawing style:";
            // 
            // rdoPoints
            // 
            this.rdoPoints.AutoSize = true;
            this.rdoPoints.Location = new System.Drawing.Point(146, 3);
            this.rdoPoints.Name = "rdoPoints";
            this.rdoPoints.Size = new System.Drawing.Size(54, 17);
            this.rdoPoints.TabIndex = 9;
            this.rdoPoints.TabStop = true;
            this.rdoPoints.Text = "Points";
            this.rdoPoints.UseVisualStyleBackColor = true;
            // 
            // rdoShade
            // 
            this.rdoShade.AutoSize = true;
            this.rdoShade.Location = new System.Drawing.Point(81, 3);
            this.rdoShade.Name = "rdoShade";
            this.rdoShade.Size = new System.Drawing.Size(56, 17);
            this.rdoShade.TabIndex = 9;
            this.rdoShade.TabStop = true;
            this.rdoShade.Text = "Shade";
            this.rdoShade.UseVisualStyleBackColor = true;
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
            // pointPlotBifurcationUserControl1
            // 
            this.pointPlotBifurcationUserControl1.Location = new System.Drawing.Point(12, 8);
            this.pointPlotBifurcationUserControl1.Name = "pointPlotBifurcationUserControl1";
            this.pointPlotBifurcationUserControl1.Size = new System.Drawing.Size(415, 432);
            this.pointPlotBifurcationUserControl1.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 612);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label2);
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
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtExpression);
            this.Name = "Form1";
            this.Text = "CsBifcuration";
            ((System.ComponentModel.ISupportInitialize)(this.tbSettling)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbParam2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private System.Windows.Forms.Button btnSave;
        private PointPlotBifurcationUserControl pointPlotBifurcationUserControl1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtP0;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rdoPoints;
        private System.Windows.Forms.RadioButton rdoShade;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtInit;
        private System.Windows.Forms.Button btnBounds;
    }
}


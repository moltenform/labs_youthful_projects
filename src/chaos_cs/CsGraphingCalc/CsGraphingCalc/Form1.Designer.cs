namespace CsGraphingCalc
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
            this.txtEq1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEq2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtInit = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnPlot = new System.Windows.Forms.Button();
            this.btnSetView = new System.Windows.Forms.Button();
            this.btnEvalAt = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            this.pointPlotUserControl1 = new CsGraphingCalc.PlotGraphEvaluateControl();
            this.SuspendLayout();
            // 
            // txtEq1
            // 
            this.txtEq1.Location = new System.Drawing.Point(533, 29);
            this.txtEq1.Multiline = true;
            this.txtEq1.Name = "txtEq1";
            this.txtEq1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEq1.Size = new System.Drawing.Size(292, 56);
            this.txtEq1.TabIndex = 1;
            this.txtEq1.Text = "x * sin(x)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(531, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "y=";
            // 
            // txtEq2
            // 
            this.txtEq2.Location = new System.Drawing.Point(533, 111);
            this.txtEq2.Multiline = true;
            this.txtEq2.Name = "txtEq2";
            this.txtEq2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEq2.Size = new System.Drawing.Size(292, 56);
            this.txtEq2.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(531, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "y=";
            // 
            // txtInit
            // 
            this.txtInit.Location = new System.Drawing.Point(535, 457);
            this.txtInit.Multiline = true;
            this.txtInit.Name = "txtInit";
            this.txtInit.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInit.Size = new System.Drawing.Size(292, 56);
            this.txtInit.TabIndex = 99;
            this.txtInit.Text = "// scroll down to see how to define custom functions\r\n\r\n\r\n\r\nFN1 fn=delegate(doubl" +
    "e a) { return a*a; };\r\n//now fn(a) can be used above";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(533, 441);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "init";
            // 
            // btnPlot
            // 
            this.btnPlot.Location = new System.Drawing.Point(535, 186);
            this.btnPlot.Name = "btnPlot";
            this.btnPlot.Size = new System.Drawing.Size(75, 23);
            this.btnPlot.TabIndex = 5;
            this.btnPlot.Text = "Plot";
            this.btnPlot.UseVisualStyleBackColor = true;
            this.btnPlot.Click += new System.EventHandler(this.btnPlot_Click);
            // 
            // btnSetView
            // 
            this.btnSetView.Location = new System.Drawing.Point(631, 186);
            this.btnSetView.Name = "btnSetView";
            this.btnSetView.Size = new System.Drawing.Size(75, 23);
            this.btnSetView.TabIndex = 5;
            this.btnSetView.Text = "Set View";
            this.btnSetView.UseVisualStyleBackColor = true;
            this.btnSetView.Click += new System.EventHandler(this.btnSetView_Click);
            // 
            // btnEvalAt
            // 
            this.btnEvalAt.Location = new System.Drawing.Point(726, 186);
            this.btnEvalAt.Name = "btnEvalAt";
            this.btnEvalAt.Size = new System.Drawing.Size(75, 23);
            this.btnEvalAt.TabIndex = 5;
            this.btnEvalAt.Text = "Eval at...";
            this.btnEvalAt.UseVisualStyleBackColor = true;
            this.btnEvalAt.Click += new System.EventHandler(this.btnEvalAt_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(533, 544);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(226, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Ben Fisher, 2010, halfhourhacks.blogspot.com";
            // 
            // btnHelp
            // 
            this.btnHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHelp.Location = new System.Drawing.Point(801, 537);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(26, 27);
            this.btnHelp.TabIndex = 100;
            this.btnHelp.Text = "?";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // pointPlotUserControl1
            // 
            this.pointPlotUserControl1.Location = new System.Drawing.Point(12, 12);
            this.pointPlotUserControl1.Name = "pointPlotUserControl1";
            this.pointPlotUserControl1.Size = new System.Drawing.Size(503, 527);
            this.pointPlotUserControl1.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(837, 566);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnEvalAt);
            this.Controls.Add(this.btnSetView);
            this.Controls.Add(this.btnPlot);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pointPlotUserControl1);
            this.Controls.Add(this.txtInit);
            this.Controls.Add(this.txtEq2);
            this.Controls.Add(this.txtEq1);
            this.Name = "Form1";
            this.Text = "Compiling Graphing Calc";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtEq1;
        private PlotGraphEvaluateControl pointPlotUserControl1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtEq2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtInit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnPlot;
        private System.Windows.Forms.Button btnSetView;
        private System.Windows.Forms.Button btnEvalAt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnHelp;
    }
}


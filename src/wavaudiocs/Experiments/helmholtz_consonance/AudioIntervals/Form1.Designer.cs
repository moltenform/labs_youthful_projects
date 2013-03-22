namespace AudioIntervals
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.chkInterval = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.enterNharmonics = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.btnHelmholtz = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.imgHoltz = new System.Windows.Forms.PictureBox();
            this.imgVas = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.enterNharmonics)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgHoltz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgVas)).BeginInit();
            this.SuspendLayout();
            // 
            // chkInterval
            // 
            this.chkInterval.AutoSize = true;
            this.chkInterval.Checked = true;
            this.chkInterval.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkInterval.Location = new System.Drawing.Point(24, 513);
            this.chkInterval.Name = "chkInterval";
            this.chkInterval.Size = new System.Drawing.Size(84, 17);
            this.chkInterval.TabIndex = 1;
            this.chkInterval.Text = "Play Interval";
            this.chkInterval.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 549);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Harmonics:";
            // 
            // enterNharmonics
            // 
            this.enterNharmonics.Location = new System.Drawing.Point(90, 547);
            this.enterNharmonics.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.enterNharmonics.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.enterNharmonics.Name = "enterNharmonics";
            this.enterNharmonics.Size = new System.Drawing.Size(42, 20);
            this.enterNharmonics.TabIndex = 3;
            this.enterNharmonics.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.radioButton2);
            this.panel1.Controls.Add(this.btnHelmholtz);
            this.panel1.Location = new System.Drawing.Point(155, 473);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 108);
            this.panel1.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Figure:";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(32, 72);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(104, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "Vassilakis (2007)";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // btnHelmholtz
            // 
            this.btnHelmholtz.AutoSize = true;
            this.btnHelmholtz.Checked = true;
            this.btnHelmholtz.Location = new System.Drawing.Point(32, 50);
            this.btnHelmholtz.Name = "btnHelmholtz";
            this.btnHelmholtz.Size = new System.Drawing.Size(104, 17);
            this.btnHelmholtz.TabIndex = 0;
            this.btnHelmholtz.TabStop = true;
            this.btnHelmholtz.Text = "Helmholtz (1885)";
            this.btnHelmholtz.UseVisualStyleBackColor = true;
            this.btnHelmholtz.CheckedChanged += new System.EventHandler(this.btnHelmholtz_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(451, 568);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Citations in readme file.";
            // 
            // imgHoltz
            // 
            this.imgHoltz.Image = ((System.Drawing.Image)(resources.GetObject("imgHoltz.Image")));
            this.imgHoltz.Location = new System.Drawing.Point(-13, 0);
            this.imgHoltz.Name = "imgHoltz";
            this.imgHoltz.Size = new System.Drawing.Size(612, 426);
            this.imgHoltz.TabIndex = 7;
            this.imgHoltz.TabStop = false;
            this.imgHoltz.MouseUp += new System.Windows.Forms.MouseEventHandler(this.imgHoltz_MouseUp);
            // 
            // imgVas
            // 
            this.imgVas.Image = ((System.Drawing.Image)(resources.GetObject("imgVas.Image")));
            this.imgVas.Location = new System.Drawing.Point(-13, 0);
            this.imgVas.Name = "imgVas";
            this.imgVas.Size = new System.Drawing.Size(627, 439);
            this.imgVas.TabIndex = 8;
            this.imgVas.TabStop = false;
            this.imgVas.Visible = false;
            this.imgVas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.imgVas_MouseUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(418, 473);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Click on figure to play interval.";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 586);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.imgVas);
            this.Controls.Add(this.imgHoltz);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.enterNharmonics);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkInterval);
            this.Name = "Form1";
            this.Text = "AudioIntervals";
            ((System.ComponentModel.ISupportInitialize)(this.enterNharmonics)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgHoltz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgVas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkInterval;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown enterNharmonics;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton btnHelmholtz;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox imgHoltz;
        private System.Windows.Forms.PictureBox imgVas;
        private System.Windows.Forms.Label label4;
    }
}


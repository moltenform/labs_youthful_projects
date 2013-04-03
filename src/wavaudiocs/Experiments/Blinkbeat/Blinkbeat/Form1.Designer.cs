namespace Blinkbeat
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblFilename = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnGo = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.lblBpm = new System.Windows.Forms.Label();
            this.btnBPM = new System.Windows.Forms.Button();
            this.btnBar8 = new System.Windows.Forms.Button();
            this.btnBar7 = new System.Windows.Forms.Button();
            this.btnBar6 = new System.Windows.Forms.Button();
            this.btnBar5 = new System.Windows.Forms.Button();
            this.btnBar4 = new System.Windows.Forms.Button();
            this.btnBar3 = new System.Windows.Forms.Button();
            this.btnBar2 = new System.Windows.Forms.Button();
            this.btnBar1 = new System.Windows.Forms.Button();
            this.timerPulse = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(626, 388);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 375);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(235, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Powered by CsWaveAudio. By Ben Fisher, 2008";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblFilename);
            this.panel1.Controls.Add(this.btnStop);
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.btnGo);
            this.panel1.Controls.Add(this.btnOpen);
            this.panel1.Controls.Add(this.lblBpm);
            this.panel1.Controls.Add(this.btnBPM);
            this.panel1.Controls.Add(this.btnBar8);
            this.panel1.Controls.Add(this.btnBar7);
            this.panel1.Controls.Add(this.btnBar6);
            this.panel1.Controls.Add(this.btnBar5);
            this.panel1.Controls.Add(this.btnBar4);
            this.panel1.Controls.Add(this.btnBar3);
            this.panel1.Controls.Add(this.btnBar2);
            this.panel1.Controls.Add(this.btnBar1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(620, 369);
            this.panel1.TabIndex = 1;
            // 
            // lblFilename
            // 
            this.lblFilename.AutoSize = true;
            this.lblFilename.Location = new System.Drawing.Point(114, 17);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(10, 13);
            this.lblFilename.TabIndex = 7;
            this.lblFilename.Text = " ";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(311, 144);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(570, 144);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(13, 13);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "0";
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(23, 38);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(269, 130);
            this.btnGo.TabIndex = 4;
            this.btnGo.Text = "Play";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(23, 8);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 3;
            this.btnOpen.Text = "Open...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // lblBpm
            // 
            this.lblBpm.AutoSize = true;
            this.lblBpm.Location = new System.Drawing.Point(566, 14);
            this.lblBpm.Name = "lblBpm";
            this.lblBpm.Size = new System.Drawing.Size(13, 13);
            this.lblBpm.TabIndex = 2;
            this.lblBpm.Text = "0";
            // 
            // btnBPM
            // 
            this.btnBPM.Location = new System.Drawing.Point(468, 9);
            this.btnBPM.Name = "btnBPM";
            this.btnBPM.Size = new System.Drawing.Size(92, 23);
            this.btnBPM.TabIndex = 1;
            this.btnBPM.Text = "Estimate BPM";
            this.btnBPM.UseVisualStyleBackColor = true;
            this.btnBPM.Click += new System.EventHandler(this.btnBPM_Click);
            // 
            // btnBar8
            // 
            this.btnBar8.Location = new System.Drawing.Point(23, 328);
            this.btnBar8.Name = "btnBar8";
            this.btnBar8.Size = new System.Drawing.Size(560, 16);
            this.btnBar8.TabIndex = 0;
            this.btnBar8.Text = " ";
            this.btnBar8.UseVisualStyleBackColor = true;
            // 
            // btnBar7
            // 
            this.btnBar7.Location = new System.Drawing.Point(23, 306);
            this.btnBar7.Name = "btnBar7";
            this.btnBar7.Size = new System.Drawing.Size(560, 16);
            this.btnBar7.TabIndex = 0;
            this.btnBar7.Text = " ";
            this.btnBar7.UseVisualStyleBackColor = true;
            // 
            // btnBar6
            // 
            this.btnBar6.Location = new System.Drawing.Point(23, 284);
            this.btnBar6.Name = "btnBar6";
            this.btnBar6.Size = new System.Drawing.Size(560, 16);
            this.btnBar6.TabIndex = 0;
            this.btnBar6.Text = " ";
            this.btnBar6.UseVisualStyleBackColor = true;
            // 
            // btnBar5
            // 
            this.btnBar5.Location = new System.Drawing.Point(23, 262);
            this.btnBar5.Name = "btnBar5";
            this.btnBar5.Size = new System.Drawing.Size(560, 16);
            this.btnBar5.TabIndex = 0;
            this.btnBar5.Text = " ";
            this.btnBar5.UseVisualStyleBackColor = true;
            // 
            // btnBar4
            // 
            this.btnBar4.Location = new System.Drawing.Point(23, 240);
            this.btnBar4.Name = "btnBar4";
            this.btnBar4.Size = new System.Drawing.Size(560, 16);
            this.btnBar4.TabIndex = 0;
            this.btnBar4.Text = " ";
            this.btnBar4.UseVisualStyleBackColor = true;
            // 
            // btnBar3
            // 
            this.btnBar3.Location = new System.Drawing.Point(23, 218);
            this.btnBar3.Name = "btnBar3";
            this.btnBar3.Size = new System.Drawing.Size(560, 16);
            this.btnBar3.TabIndex = 0;
            this.btnBar3.Text = " ";
            this.btnBar3.UseVisualStyleBackColor = true;
            // 
            // btnBar2
            // 
            this.btnBar2.Location = new System.Drawing.Point(23, 196);
            this.btnBar2.Name = "btnBar2";
            this.btnBar2.Size = new System.Drawing.Size(560, 16);
            this.btnBar2.TabIndex = 0;
            this.btnBar2.Text = " ";
            this.btnBar2.UseVisualStyleBackColor = true;
            // 
            // btnBar1
            // 
            this.btnBar1.Location = new System.Drawing.Point(23, 174);
            this.btnBar1.Name = "btnBar1";
            this.btnBar1.Size = new System.Drawing.Size(560, 16);
            this.btnBar1.TabIndex = 0;
            this.btnBar1.Text = " ";
            this.btnBar1.UseVisualStyleBackColor = true;
            // 
            // timerPulse
            // 
            this.timerPulse.Tick += new System.EventHandler(this.timerPulse_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 388);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Blinkbeat";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnBar1;
        private System.Windows.Forms.Button btnBar2;
        private System.Windows.Forms.Button btnBar8;
        private System.Windows.Forms.Button btnBar7;
        private System.Windows.Forms.Button btnBar6;
        private System.Windows.Forms.Button btnBar5;
        private System.Windows.Forms.Button btnBar4;
        private System.Windows.Forms.Button btnBar3;
        private System.Windows.Forms.Button btnBPM;
        private System.Windows.Forms.Label lblBpm;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Timer timerPulse;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblFilename;


    }
}


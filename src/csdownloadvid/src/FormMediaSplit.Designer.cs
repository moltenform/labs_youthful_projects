namespace CsDownloadVid
{
    partial class FormMediaSplit
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
            this.btnGetInput = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxFadeout = new System.Windows.Forms.CheckBox();
            this.txtFadeLength = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSplitpoints = new System.Windows.Forms.TextBox();
            this.lblShortStatus = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.btnSplit = new System.Windows.Forms.Button();
            this.btnImportAudacityLabel = new System.Windows.Forms.Button();
            this.btnShowSum = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnToMp3DirectCut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGetInput
            // 
            this.btnGetInput.Location = new System.Drawing.Point(245, 12);
            this.btnGetInput.Name = "btnGetInput";
            this.btnGetInput.Size = new System.Drawing.Size(75, 23);
            this.btnGetInput.TabIndex = 31;
            this.btnGetInput.Text = "Choose...";
            this.btnGetInput.UseVisualStyleBackColor = true;
            this.btnGetInput.Click += new System.EventHandler(this.btnGetInput_Click);
            // 
            // txtInput
            // 
            this.txtInput.BackColor = System.Drawing.SystemColors.Control;
            this.txtInput.Location = new System.Drawing.Point(13, 41);
            this.txtInput.Name = "txtInput";
            this.txtInput.ReadOnly = true;
            this.txtInput.Size = new System.Drawing.Size(307, 20);
            this.txtInput.TabIndex = 33;
            this.txtInput.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(229, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "Choose a m4v video, mp4 video, or m4a audio:";
            // 
            // checkBoxFadeout
            // 
            this.checkBoxFadeout.AutoSize = true;
            this.checkBoxFadeout.Location = new System.Drawing.Point(47, 70);
            this.checkBoxFadeout.Name = "checkBoxFadeout";
            this.checkBoxFadeout.Size = new System.Drawing.Size(197, 17);
            this.checkBoxFadeout.TabIndex = 34;
            this.checkBoxFadeout.Text = "Add fadeout, fade length in seconds";
            this.checkBoxFadeout.UseVisualStyleBackColor = true;
            // 
            // txtFadeLength
            // 
            this.txtFadeLength.Location = new System.Drawing.Point(253, 67);
            this.txtFadeLength.Name = "txtFadeLength";
            this.txtFadeLength.Size = new System.Drawing.Size(67, 20);
            this.txtFadeLength.TabIndex = 35;
            this.txtFadeLength.Text = "4";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(141, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Enter split points in seconds:";
            // 
            // txtSplitpoints
            // 
            this.txtSplitpoints.BackColor = System.Drawing.SystemColors.Window;
            this.txtSplitpoints.Location = new System.Drawing.Point(15, 131);
            this.txtSplitpoints.Multiline = true;
            this.txtSplitpoints.Name = "txtSplitpoints";
            this.txtSplitpoints.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSplitpoints.Size = new System.Drawing.Size(252, 109);
            this.txtSplitpoints.TabIndex = 36;
            // 
            // lblShortStatus
            // 
            this.lblShortStatus.AutoSize = true;
            this.lblShortStatus.Location = new System.Drawing.Point(11, 328);
            this.lblShortStatus.Name = "lblShortStatus";
            this.lblShortStatus.Size = new System.Drawing.Size(45, 13);
            this.lblShortStatus.TabIndex = 40;
            this.lblShortStatus.Text = "Results:";
            // 
            // txtStatus
            // 
            this.txtStatus.BackColor = System.Drawing.SystemColors.Control;
            this.txtStatus.Location = new System.Drawing.Point(12, 346);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(447, 108);
            this.txtStatus.TabIndex = 39;
            // 
            // btnSplit
            // 
            this.btnSplit.Location = new System.Drawing.Point(12, 255);
            this.btnSplit.Name = "btnSplit";
            this.btnSplit.Size = new System.Drawing.Size(447, 67);
            this.btnSplit.TabIndex = 38;
            this.btnSplit.Text = "Split (Lossless)";
            this.btnSplit.UseVisualStyleBackColor = true;
            this.btnSplit.Click += new System.EventHandler(this.btnSplit_Click);
            // 
            // btnImportAudacityLabel
            // 
            this.btnImportAudacityLabel.Location = new System.Drawing.Point(273, 131);
            this.btnImportAudacityLabel.Name = "btnImportAudacityLabel";
            this.btnImportAudacityLabel.Size = new System.Drawing.Size(186, 23);
            this.btnImportAudacityLabel.TabIndex = 38;
            this.btnImportAudacityLabel.Text = "Import Audacity label track...";
            this.btnImportAudacityLabel.UseVisualStyleBackColor = true;
            this.btnImportAudacityLabel.Click += new System.EventHandler(this.btnImportAudacity_Click);
            // 
            // btnShowSum
            // 
            this.btnShowSum.Location = new System.Drawing.Point(273, 160);
            this.btnShowSum.Name = "btnShowSum";
            this.btnShowSum.Size = new System.Drawing.Size(186, 23);
            this.btnShowSum.TabIndex = 38;
            this.btnShowSum.Text = "Show sum of these times...";
            this.btnShowSum.UseVisualStyleBackColor = true;
            this.btnShowSum.Click += new System.EventHandler(this.btnShowSum_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(63, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "(requires qaac.exe)";
            // 
            // btnToMp3DirectCut
            // 
            this.btnToMp3DirectCut.Location = new System.Drawing.Point(273, 189);
            this.btnToMp3DirectCut.Name = "btnToMp3DirectCut";
            this.btnToMp3DirectCut.Size = new System.Drawing.Size(186, 23);
            this.btnToMp3DirectCut.TabIndex = 38;
            this.btnToMp3DirectCut.Text = "Export to mp3DirectCut...";
            this.btnToMp3DirectCut.UseVisualStyleBackColor = true;
            this.btnToMp3DirectCut.Click += new System.EventHandler(this.btnToMp3DirectCut_Click);
            // 
            // FormMediaSplit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 466);
            this.Controls.Add(this.lblShortStatus);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnToMp3DirectCut);
            this.Controls.Add(this.btnShowSum);
            this.Controls.Add(this.btnImportAudacityLabel);
            this.Controls.Add(this.btnSplit);
            this.Controls.Add(this.txtSplitpoints);
            this.Controls.Add(this.txtFadeLength);
            this.Controls.Add(this.checkBoxFadeout);
            this.Controls.Add(this.btnGetInput);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Name = "FormMediaSplit";
            this.Text = "Split a video or a song";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetInput;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxFadeout;
        private System.Windows.Forms.TextBox txtFadeLength;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSplitpoints;
        private System.Windows.Forms.Label lblShortStatus;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Button btnSplit;
        private System.Windows.Forms.Button btnImportAudacityLabel;
        private System.Windows.Forms.Button btnShowSum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnToMp3DirectCut;
    }
}
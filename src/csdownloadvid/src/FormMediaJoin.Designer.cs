namespace CsDownloadVid
{
    partial class FormMediaJoin
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnJoin = new System.Windows.Forms.Button();
            this.lblShortStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbOutputFormat = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnGetInput
            // 
            this.btnGetInput.Location = new System.Drawing.Point(289, 17);
            this.btnGetInput.Name = "btnGetInput";
            this.btnGetInput.Size = new System.Drawing.Size(75, 23);
            this.btnGetInput.TabIndex = 6;
            this.btnGetInput.Text = "Choose...";
            this.btnGetInput.UseVisualStyleBackColor = true;
            this.btnGetInput.Click += new System.EventHandler(this.btnGetInput_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(200, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Enter some m4a audio or mp4 video files:";
            // 
            // txtInput
            // 
            this.txtInput.BackColor = System.Drawing.SystemColors.Control;
            this.txtInput.Location = new System.Drawing.Point(12, 46);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInput.Size = new System.Drawing.Size(686, 112);
            this.txtInput.TabIndex = 7;
            this.txtInput.WordWrap = false;
            // 
            // txtStatus
            // 
            this.txtStatus.BackColor = System.Drawing.SystemColors.Control;
            this.txtStatus.Location = new System.Drawing.Point(12, 263);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(352, 108);
            this.txtStatus.TabIndex = 10;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(173, 186);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(128, 42);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnJoin
            // 
            this.btnJoin.Location = new System.Drawing.Point(15, 186);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(142, 42);
            this.btnJoin.TabIndex = 9;
            this.btnJoin.Text = "Join Together (lossless)";
            this.btnJoin.UseVisualStyleBackColor = true;
            this.btnJoin.Click += new System.EventHandler(this.btnJoin_Click);
            // 
            // lblShortStatus
            // 
            this.lblShortStatus.AutoSize = true;
            this.lblShortStatus.Location = new System.Drawing.Point(12, 247);
            this.lblShortStatus.Name = "lblShortStatus";
            this.lblShortStatus.Size = new System.Drawing.Size(45, 13);
            this.lblShortStatus.TabIndex = 11;
            this.lblShortStatus.Text = "Results:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(413, 182);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Output format:";
            // 
            // tbOutputFormat
            // 
            this.tbOutputFormat.Location = new System.Drawing.Point(416, 198);
            this.tbOutputFormat.Name = "tbOutputFormat";
            this.tbOutputFormat.Size = new System.Drawing.Size(128, 20);
            this.tbOutputFormat.TabIndex = 13;
            this.tbOutputFormat.Text = "auto";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(546, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 13);
            this.label3.TabIndex = 33;
            this.label3.Text = "CsDownloadVid, by Ben Fisher";
            // 
            // FormMediaJoin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 388);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbOutputFormat);
            this.Controls.Add(this.lblShortStatus);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnJoin);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.btnGetInput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Name = "FormMediaJoin";
            this.Text = "Join Together";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnJoin;
        private System.Windows.Forms.Label lblShortStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbOutputFormat;
        private System.Windows.Forms.Label label3;
    }
}
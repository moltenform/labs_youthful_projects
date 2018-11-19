namespace CsDownloadVid
{
    partial class FormAudioFromVideo
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
            this.txtInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGetInput = new System.Windows.Forms.Button();
            this.getAudio = new System.Windows.Forms.Button();
            this.getVideo = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.btnCombineAudioVideo = new System.Windows.Forms.Button();
            this.lblShortStatus = new System.Windows.Forms.Label();
            this.tbOutputFormat = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtInput
            // 
            this.txtInput.BackColor = System.Drawing.SystemColors.Control;
            this.txtInput.Location = new System.Drawing.Point(23, 60);
            this.txtInput.Name = "txtInput";
            this.txtInput.ReadOnly = true;
            this.txtInput.Size = new System.Drawing.Size(576, 20);
            this.txtInput.TabIndex = 30;
            this.txtInput.Click += new System.EventHandler(this.txtInput_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Choose a mp4 video file:";
            // 
            // btnGetInput
            // 
            this.btnGetInput.Location = new System.Drawing.Point(171, 31);
            this.btnGetInput.Name = "btnGetInput";
            this.btnGetInput.Size = new System.Drawing.Size(75, 23);
            this.btnGetInput.TabIndex = 1;
            this.btnGetInput.Text = "Choose...";
            this.btnGetInput.UseVisualStyleBackColor = true;
            this.btnGetInput.Click += new System.EventHandler(this.btnGetInput_Click);
            // 
            // getAudio
            // 
            this.getAudio.Location = new System.Drawing.Point(23, 108);
            this.getAudio.Name = "getAudio";
            this.getAudio.Size = new System.Drawing.Size(128, 42);
            this.getAudio.TabIndex = 4;
            this.getAudio.Text = "Extract Audio Channel (lossless)";
            this.getAudio.UseVisualStyleBackColor = true;
            this.getAudio.Click += new System.EventHandler(this.getAudio_Click);
            // 
            // getVideo
            // 
            this.getVideo.Location = new System.Drawing.Point(171, 108);
            this.getVideo.Name = "getVideo";
            this.getVideo.Size = new System.Drawing.Size(128, 42);
            this.getVideo.TabIndex = 4;
            this.getVideo.Text = "Extract Video Channel (lossless)";
            this.getVideo.UseVisualStyleBackColor = true;
            this.getVideo.Click += new System.EventHandler(this.getVideo_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.BackColor = System.Drawing.SystemColors.Control;
            this.txtStatus.Location = new System.Drawing.Point(23, 176);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(276, 108);
            this.txtStatus.TabIndex = 5;
            // 
            // btnCombineAudioVideo
            // 
            this.btnCombineAudioVideo.Location = new System.Drawing.Point(318, 108);
            this.btnCombineAudioVideo.Name = "btnCombineAudioVideo";
            this.btnCombineAudioVideo.Size = new System.Drawing.Size(128, 42);
            this.btnCombineAudioVideo.TabIndex = 4;
            this.btnCombineAudioVideo.Text = "Combine Audio+Video Channel (lossless)";
            this.btnCombineAudioVideo.UseVisualStyleBackColor = true;
            this.btnCombineAudioVideo.Click += new System.EventHandler(this.btnCombineAudioVideo_Click);
            // 
            // lblShortStatus
            // 
            this.lblShortStatus.AutoSize = true;
            this.lblShortStatus.Location = new System.Drawing.Point(29, 160);
            this.lblShortStatus.Name = "lblShortStatus";
            this.lblShortStatus.Size = new System.Drawing.Size(45, 13);
            this.lblShortStatus.TabIndex = 2;
            this.lblShortStatus.Text = "Results:";
            // 
            // tbOutputFormat
            // 
            this.tbOutputFormat.Location = new System.Drawing.Point(339, 215);
            this.tbOutputFormat.Name = "tbOutputFormat";
            this.tbOutputFormat.Size = new System.Drawing.Size(107, 20);
            this.tbOutputFormat.TabIndex = 32;
            this.tbOutputFormat.Text = "auto";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(335, 199);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Output format:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(471, 108);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(128, 42);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(458, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "CsDownloadVid, by Ben Fisher";
            // 
            // FormAudioFromVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 305);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbOutputFormat);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCombineAudioVideo);
            this.Controls.Add(this.getVideo);
            this.Controls.Add(this.getAudio);
            this.Controls.Add(this.btnGetInput);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.lblShortStatus);
            this.Controls.Add(this.label2);
            this.Name = "FormAudioFromVideo";
            this.Text = "Separate Audio And Video";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnGetInput;
        private System.Windows.Forms.Button getAudio;
        private System.Windows.Forms.Button getVideo;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Button btnCombineAudioVideo;
        private System.Windows.Forms.Label lblShortStatus;
        private System.Windows.Forms.TextBox tbOutputFormat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
    }
}
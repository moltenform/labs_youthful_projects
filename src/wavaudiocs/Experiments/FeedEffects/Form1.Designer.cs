namespace FeedEffects
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
            this.button1 = new System.Windows.Forms.Button();
            this.presets = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.controlFeed5 = new FeedEffects.ControlFeed();
            this.controlFeed4 = new FeedEffects.ControlFeed();
            this.controlFeed3 = new FeedEffects.ControlFeed();
            this.controlFeed2 = new FeedEffects.ControlFeed();
            this.controlFeed1 = new FeedEffects.ControlFeed();
            this.btnTry = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(52, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // presets
            // 
            this.presets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.presets.FormattingEnabled = true;
            this.presets.Items.AddRange(new object[] {
            "No effect",
            "Phaser",
            "Flange"});
            this.presets.Location = new System.Drawing.Point(236, 13);
            this.presets.Name = "presets";
            this.presets.Size = new System.Drawing.Size(121, 21);
            this.presets.TabIndex = 2;
            this.presets.SelectedIndexChanged += new System.EventHandler(this.presets_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(185, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Presets:";
            // 
            // controlFeed5
            // 
            this.controlFeed5.Location = new System.Drawing.Point(832, 42);
            this.controlFeed5.Name = "controlFeed5";
            this.controlFeed5.Size = new System.Drawing.Size(199, 176);
            this.controlFeed5.TabIndex = 1;
            // 
            // controlFeed4
            // 
            this.controlFeed4.Location = new System.Drawing.Point(627, 42);
            this.controlFeed4.Name = "controlFeed4";
            this.controlFeed4.Size = new System.Drawing.Size(199, 176);
            this.controlFeed4.TabIndex = 1;
            // 
            // controlFeed3
            // 
            this.controlFeed3.Location = new System.Drawing.Point(422, 42);
            this.controlFeed3.Name = "controlFeed3";
            this.controlFeed3.Size = new System.Drawing.Size(199, 176);
            this.controlFeed3.TabIndex = 1;
            // 
            // controlFeed2
            // 
            this.controlFeed2.Location = new System.Drawing.Point(217, 42);
            this.controlFeed2.Name = "controlFeed2";
            this.controlFeed2.Size = new System.Drawing.Size(199, 176);
            this.controlFeed2.TabIndex = 1;
            // 
            // controlFeed1
            // 
            this.controlFeed1.Location = new System.Drawing.Point(12, 42);
            this.controlFeed1.Name = "controlFeed1";
            this.controlFeed1.Size = new System.Drawing.Size(199, 176);
            this.controlFeed1.TabIndex = 1;
            // 
            // btnTry
            // 
            this.btnTry.Location = new System.Drawing.Point(445, 5);
            this.btnTry.Name = "btnTry";
            this.btnTry.Size = new System.Drawing.Size(75, 23);
            this.btnTry.TabIndex = 4;
            this.btnTry.Text = "Try It";
            this.btnTry.UseVisualStyleBackColor = true;
            this.btnTry.Click += new System.EventHandler(this.btnTry_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 333);
            this.Controls.Add(this.btnTry);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.presets);
            this.Controls.Add(this.controlFeed5);
            this.Controls.Add(this.controlFeed4);
            this.Controls.Add(this.controlFeed3);
            this.Controls.Add(this.controlFeed2);
            this.Controls.Add(this.controlFeed1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private ControlFeed controlFeed1;
        private ControlFeed controlFeed2;
        private ControlFeed controlFeed3;
        private ControlFeed controlFeed4;
        private ControlFeed controlFeed5;
        private System.Windows.Forms.ComboBox presets;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnTry;
    }
}


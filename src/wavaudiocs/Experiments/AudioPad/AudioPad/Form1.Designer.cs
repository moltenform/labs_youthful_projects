namespace AudioPad
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
            this.btnGo = new System.Windows.Forms.Button();
            this.slidePitch = new System.Windows.Forms.TrackBar();
            this.slideSmooth = new System.Windows.Forms.TrackBar();
            this.slideWidth = new System.Windows.Forms.TrackBar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.timerDisplay = new System.Windows.Forms.Timer(this.components);
            this.timerPoll = new System.Windows.Forms.Timer(this.components);
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblSmooth = new System.Windows.Forms.Label();
            this.lblPitch = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSine = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSquare = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTriangle = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSawtooth = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWhitenoise = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRednoise = new System.Windows.Forms.ToolStripMenuItem();
            this.waveformPlot1 = new AudioPad.WaveformPlot();
            ((System.ComponentModel.ISupportInitialize)(this.slidePitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slideSmooth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slideWidth)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(12, 341);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(75, 23);
            this.btnGo.TabIndex = 0;
            this.btnGo.Text = "Start";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // slidePitch
            // 
            this.slidePitch.Location = new System.Drawing.Point(53, 174);
            this.slidePitch.Maximum = 9;
            this.slidePitch.Name = "slidePitch";
            this.slidePitch.Size = new System.Drawing.Size(203, 45);
            this.slidePitch.TabIndex = 1;
            this.slidePitch.Value = 2;
            // 
            // slideSmooth
            // 
            this.slideSmooth.Location = new System.Drawing.Point(61, 89);
            this.slideSmooth.Maximum = 200;
            this.slideSmooth.Name = "slideSmooth";
            this.slideSmooth.Size = new System.Drawing.Size(203, 45);
            this.slideSmooth.TabIndex = 2;
            // 
            // slideWidth
            // 
            this.slideWidth.Location = new System.Drawing.Point(61, 40);
            this.slideWidth.Maximum = 97;
            this.slideWidth.Minimum = -97;
            this.slideWidth.Name = "slideWidth";
            this.slideWidth.Size = new System.Drawing.Size(203, 45);
            this.slideWidth.TabIndex = 3;
            this.slideWidth.Value = 10;
            // 
            // timer
            // 
            this.timer.Interval = 10;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // timerDisplay
            // 
            this.timerDisplay.Interval = 200;
            this.timerDisplay.Tick += new System.EventHandler(this.timerDisplay_Tick);
            // 
            // timerPoll
            // 
            this.timerPoll.Interval = 30;
            this.timerPoll.Tick += new System.EventHandler(this.timerPoll_Tick);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(50, 141);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 8;
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(12, 40);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(38, 13);
            this.lblWidth.TabIndex = 9;
            this.lblWidth.Text = "Width:";
            // 
            // lblSmooth
            // 
            this.lblSmooth.AutoSize = true;
            this.lblSmooth.Location = new System.Drawing.Point(12, 89);
            this.lblSmooth.Name = "lblSmooth";
            this.lblSmooth.Size = new System.Drawing.Size(46, 13);
            this.lblSmooth.TabIndex = 10;
            this.lblSmooth.Text = "Smooth:";
            // 
            // lblPitch
            // 
            this.lblPitch.AutoSize = true;
            this.lblPitch.Location = new System.Drawing.Point(13, 174);
            this.lblPitch.Name = "lblPitch";
            this.lblPitch.Size = new System.Drawing.Size(31, 13);
            this.lblPitch.TabIndex = 11;
            this.lblPitch.Text = "Pitch";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(692, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripSeparator1,
            this.menuSine,
            this.menuSquare,
            this.menuTriangle,
            this.menuSawtooth,
            this.menuWhitenoise,
            this.menuRednoise});
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            this.textToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.textToolStripMenuItem.Text = "Play";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(155, 22);
            this.toolStripMenuItem1.Text = "Current Sound";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.MenuPlayCurrentSound_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
            // 
            // menuSine
            // 
            this.menuSine.Name = "menuSine";
            this.menuSine.Size = new System.Drawing.Size(155, 22);
            this.menuSine.Text = "Sine";
            this.menuSine.Click += new System.EventHandler(this.menuSine_Click);
            // 
            // menuSquare
            // 
            this.menuSquare.Name = "menuSquare";
            this.menuSquare.Size = new System.Drawing.Size(155, 22);
            this.menuSquare.Text = "Square";
            this.menuSquare.Click += new System.EventHandler(this.menuSquare_Click);
            // 
            // menuTriangle
            // 
            this.menuTriangle.Name = "menuTriangle";
            this.menuTriangle.Size = new System.Drawing.Size(155, 22);
            this.menuTriangle.Text = "Triangle";
            this.menuTriangle.Click += new System.EventHandler(this.menuTriangle_Click);
            // 
            // menuSawtooth
            // 
            this.menuSawtooth.Name = "menuSawtooth";
            this.menuSawtooth.Size = new System.Drawing.Size(155, 22);
            this.menuSawtooth.Text = "Sawtooth";
            this.menuSawtooth.Click += new System.EventHandler(this.menuSawtooth_Click);
            // 
            // menuWhitenoise
            // 
            this.menuWhitenoise.Name = "menuWhitenoise";
            this.menuWhitenoise.Size = new System.Drawing.Size(155, 22);
            this.menuWhitenoise.Text = "White Noise";
            this.menuWhitenoise.Click += new System.EventHandler(this.menuWhitenoise_Click);
            // 
            // menuRednoise
            // 
            this.menuRednoise.Name = "menuRednoise";
            this.menuRednoise.Size = new System.Drawing.Size(155, 22);
            this.menuRednoise.Text = "Red Noise";
            this.menuRednoise.Click += new System.EventHandler(this.menuRednoise_Click);
            // 
            // waveformPlot1
            // 
            this.waveformPlot1.Location = new System.Drawing.Point(289, 40);
            this.waveformPlot1.Name = "waveformPlot1";
            this.waveformPlot1.Size = new System.Drawing.Size(391, 324);
            this.waveformPlot1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 377);
            this.Controls.Add(this.lblPitch);
            this.Controls.Add(this.lblSmooth);
            this.Controls.Add(this.lblWidth);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.waveformPlot1);
            this.Controls.Add(this.slideWidth);
            this.Controls.Add(this.slideSmooth);
            this.Controls.Add(this.slidePitch);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "AudioPad";
            ((System.ComponentModel.ISupportInitialize)(this.slidePitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slideSmooth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slideWidth)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TrackBar slidePitch;
        private System.Windows.Forms.TrackBar slideSmooth;
        private System.Windows.Forms.TrackBar slideWidth;
        private System.Windows.Forms.Timer timer;
        private WaveformPlot waveformPlot1;
        private System.Windows.Forms.Timer timerDisplay;
        private System.Windows.Forms.Timer timerPoll;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblSmooth;
        private System.Windows.Forms.Label lblPitch;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuSine;
        private System.Windows.Forms.ToolStripMenuItem menuSquare;
        private System.Windows.Forms.ToolStripMenuItem menuTriangle;
        private System.Windows.Forms.ToolStripMenuItem menuSawtooth;
        private System.Windows.Forms.ToolStripMenuItem menuWhitenoise;
        private System.Windows.Forms.ToolStripMenuItem menuRednoise;
    }
}


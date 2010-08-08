using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FeedEffects
{
    public partial class ControlFeed : UserControl
    {
        public ControlFeed()
        {
            InitializeComponent();
            this.comboBox1.SelectedIndex = 0;
            this.txtAmplitude.Text = "0";
            this.txtFreq.Text = "1";
            this.txtMean.Text = "0";
            this.txtMultiply.Text = "1.0";
        }
        public void Config(double amp, double freq, double mean, double mult, int index)
        {
            this.comboBox1.SelectedIndex = index;
            this.txtAmplitude.Text = amp.ToString();
            this.txtFreq.Text = freq.ToString() ;
            this.txtMean.Text = mean.ToString();
            this.txtMultiply.Text = mult.ToString();
        }
        public double GetMultiply()
        {
            return double.Parse(this.txtMultiply.Text);
        }
        public bool IsDisabled()
        {
            return Math.Abs(double.Parse(this.txtMultiply.Text)) < 0.001;
        }
        public int[] Go(int length)
        {
            //create the wave.
            int[] res = new int[length];

            double amplitude = double.Parse(this.txtAmplitude.Text) * 44100;
            double freq = double.Parse(this.txtFreq.Text);
            double mean = double.Parse(this.txtMean.Text) * 44100;
            double multiply = double.Parse(this.txtMultiply.Text);
            PeriodicAlternative palt = new PASin();
            if (this.comboBox1.SelectedIndex == 1) palt = new PASin();
            else if (this.comboBox1.SelectedIndex == 2) palt = new PATri();
            else if (this.comboBox1.SelectedIndex == 3) palt = new PASawtooth();
            else if (this.comboBox1.SelectedIndex == 4) palt = new PASquare();
            
            for (int i = 0; i < length; i++)
            {
                if (this.comboBox1.SelectedIndex == 0) // Constant
                {
                    res[i] = (int)mean;
                }
                else
                {
                    res[i] = (int)(mean + amplitude * palt.GetValue(i * 2.0 * Math.PI * freq / 44100.0));
                }
               // if (i % 1000 == 0) System.Diagnostics.Debug.WriteLine("hhhh");
            }

            return res;
        }
    }
}

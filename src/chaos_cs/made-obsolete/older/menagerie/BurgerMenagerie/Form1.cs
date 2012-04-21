using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace BurgerManagerie
{
    public partial class Form1 : Form
    {
        string sFolder = @"..\..\..\";
        public Form1()
        {
            InitializeComponent();
            //pictureBox1.Load(sFolder+"thefig.png");
            pictureBox1.Load(sFolder+"thefig.png");
            viewNudgePosition.Visible = false;
        }
        double lastA, lastB; //todo: update these when changed, maybe onActivate
        
        private void pictureBox1_MouseUp_1(object sender, MouseEventArgs e)
        {
            int ix = e.X, iy=e.Y;
            int iwidth = pictureBox1.Image.Width;
            int iheight = pictureBox1.Image.Height;
            iy = iheight-iy;
            double X0, X1, Y0, Y1;

            X0=-3.4; X1=1.1; Y0=1.3; Y1=2.05;
            //X0=-1; X1=0.25; Y0=1.5; Y1=1.75;
            /*if (ix>450)
            {
                X0=-1; X1=1; Y0=0.5; Y1=2.5;
                ix-=450;
            }
            else
            {
                X0=-3.5; X1=-1; Y0=0.5; Y1=2.5;
            }*/
            double fx = (ix/(double)iwidth) * (X1-X0) + X0;
            double fy = (iy/(double)iheight) * (Y1-Y0) + Y0;

            SetPosition(fx,fy);

            this.Activate();

        }

        private void fileSave_Click(object sender, EventArgs e)
        {
            bool b = FlipCSharp.CsAutomate.ActivatePaint("SDL_app");
            if (!b)
                return;
            Thread.Sleep(200);
           // FlipCSharp.CsAutomate.Send("%{PGDN}");
            FlipCSharp.CsAutomate.Send("%{PRTSC}");
            Thread.Sleep(10);
            Clipboard.GetImage(); //take a screenshot! sweet!

            Thread.Sleep(200);
            this.Activate();

        }

        private void editCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lastA+","+lastB);
        }

        private void editPaste_Click(object sender, EventArgs e)
        {
            double a,b;
            string s = Clipboard.GetText();
            string[] parts = s.Split(new char[] { ',' });
            if (parts.Length!=2) return;
            if (!double.TryParse(parts[0], out a))
                return;
            if (!double.TryParse(parts[1], out b))
                return;
            this.SetPosition(a, b);
        }

        private void SetPosition(double a, double b)
        {
            lastA = a; lastB=b;
            bool bb = FlipCSharp.CsAutomate.ActivatePaint("SDL_app");
            if (!bb)
                return;

            TextWriter tw= new StreamWriter(sFolder+"outTrans.txt");
            tw.WriteLine(a.ToString());
            tw.WriteLine(b.ToString());
            tw.Close();

            Thread.Sleep(20);

            FlipCSharp.CsAutomate.Send("%s");

            Thread.Sleep(20);
        }
        private bool getString(string prompt, string def, out double outd)
        {
            outd=0;
            string s = InputBoxForm.GetStrInput(prompt, def);
            if (s == null || s =="") { return false; }
            return double.TryParse(s, out outd);
        }
        private void viewSetPos_Click(object sender, EventArgs e)
        {
            double a, b;
            if (!getString("a:", lastA.ToString(), out a)) return;
            if (!getString("b:", lastA.ToString(), out b)) return;
            SetPosition(a, b);
        }

        private void viewZoomIn_Click(object sender, EventArgs e)
        {

        }

        

        
	    
    }
}
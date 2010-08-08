using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DrawAuto
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const int D=750;
        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void go(int pattern)
        {
            Bitmap b = new Bitmap(D, D/2);
            for (int x = 0; x < D; x++)
            {
                for (int y = 0; y < D; y++)
                {
                    // b.SetPixel(x,y,Color.FromArgb(x,y,0));
                }
            }
            int[] patternar;
            if (pattern==184)
                patternar=new int[] { 1, 0, 1, 1, 1, 0, 0, 0 };
            else if (pattern==30)
                patternar=new int[] { 0, 0, 0, 1, 1, 1, 1, 0 };
            else if (pattern==90)
                patternar=new int[] { 0, 1, 0, 1, 1, 0, 1, 0 };
            else if (pattern==110)
                patternar=new int[] { 0, 1, 1, 0, 1, 1, 1, 0 };
            else throw new ArgumentException("pattern not supported");

            int[] current = new int[D]; int[] temp = new int[D]; int[] newf = new int[D];
            current[D/2] = 1;
            for (int i = 0; i < D/2 /*50*/; i++)
            {


                // paint the picture
                for (int x = 0; x < D; x++)
                    if (current[x] == 0) b.SetPixel(x, i, Color.White);
                    else b.SetPixel(x, i, Color.Black);

                // get new
                newf = new int[D];
                for (int xmid = 1; xmid < (D-1); xmid++)
                {
                    int vva = current[xmid - 1];
                    int vvb = current[xmid];
                    int vvc = current[xmid + 1];

                    if (vva == 1 && vvb == 1 && vvc == 1)
                        newf[xmid] = patternar[0];
                    else if (vva == 1 && vvb == 1 && vvc == 0)
                        newf[xmid] = patternar[1];
                    else if (vva == 1 && vvb == 0 && vvc == 1)
                        newf[xmid] = patternar[2];
                    else if (vva == 1 && vvb == 0 && vvc == 0)
                        newf[xmid] = patternar[3];
                    else if (vva == 0 && vvb == 1 && vvc == 1)
                        newf[xmid] = patternar[4];
                    else if (vva == 0 && vvb == 1 && vvc == 0)
                        newf[xmid] = patternar[5];
                    else if (vva == 0 && vvb == 0 && vvc == 1)
                        newf[xmid] = patternar[6];
                    else if (vva == 0 && vvb == 0 && vvc == 0)
                        newf[xmid] = patternar[7];
                }

                //assign to new
                for (int x=0; x<D; x++)
                    current[x] = newf[x];
            }

            this.pictureBox1.Image = b;

        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            int p = Int32.Parse((sender as Button).Tag as string);
            go(p);
        }


    }

    

}
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




        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(500, 500);
            for (int x = 0; x < 500; x++)
            {
                for (int y = 0; y < 500; y++)
                {
                    // b.SetPixel(x,y,Color.FromArgb(x,y,0));
                }
            }


            int[] current = new int[500]; int[] temp = new int[500]; int[] newf = new int[500];
            current[500/2] = 1;
            for (int i = 0; i < 500/*50*/; i++)
            {


                // paint the picture
                for (int x = 0; x < 500; x++)
                    if (current[x] == 0) b.SetPixel(x, i, Color.White);
                    else b.SetPixel(x, i, Color.Black);

                // get new
                newf = new int[500];
                for (int xmid = 1; xmid < (500-1); xmid++)
                {
                    int vva = current[xmid - 1];
                    int vvb = current[xmid];
                    int vvc = current[xmid + 1];
                    if (true)
                    {
                        if (vva == 1 && vvb == 1 && vvc == 1)
                            newf[xmid] = 0;
                        else if (vva == 1 && vvb == 1 && vvc == 0)
                            newf[xmid] = 1;
                        else if (vva == 1 && vvb == 0 && vvc == 1)
                            newf[xmid] = 0;
                        else if (vva == 1 && vvb == 0 && vvc == 0)
                            newf[xmid] = 1;
                        else if (vva == 0 && vvb == 1 && vvc == 1)
                            newf[xmid] = 1;
                        else if (vva == 0 && vvb == 1 && vvc == 0)
                            newf[xmid] = 0;
                        else if (vva == 0 && vvb == 0 && vvc == 1)
                            newf[xmid] = 1;
                        else if (vva == 0 && vvb == 0 && vvc == 0)
                            newf[xmid] = 0;
                    }
                    else
                    {
                        if (vva == 1 && vvb == 1 && vvc == 1)
                            newf[xmid] = 0;
                        else if (vva == 1 && vvb == 1 && vvc == 0)
                            newf[xmid] = 0;
                        else if (vva == 1 && vvb == 0 && vvc == 1)
                            newf[xmid] = 0;
                        else if (vva == 1 && vvb == 0 && vvc == 0)
                            newf[xmid] = 1;
                        else if (vva == 0 && vvb == 1 && vvc == 1)
                            newf[xmid] = 1;
                        else if (vva == 0 && vvb == 1 && vvc == 0)
                            newf[xmid] = 1;
                        else if (vva == 0 && vvb == 0 && vvc == 1)
                            newf[xmid] = 1;
                        else if (vva == 0 && vvb == 0 && vvc == 0)
                            newf[xmid] = 0;
                    }

                }

                //assign to new
                for (int x=0; x<500; x++)
                    current[x] = newf[x];
            }




            this.pictureBox1.Image = b;

        }
    }

    

}
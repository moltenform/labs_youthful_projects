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
            Bitmap b = new Bitmap(D, D/2);
            for (int x = 0; x < D; x++)
            {
                for (int y = 0; y < D; y++)
                {
                    // b.SetPixel(x,y,Color.FromArgb(x,y,0));
                }
            }


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
                for (int x=0; x<D; x++)
                    current[x] = newf[x];
            }




            this.pictureBox1.Image = b;

        }
    }

    

}
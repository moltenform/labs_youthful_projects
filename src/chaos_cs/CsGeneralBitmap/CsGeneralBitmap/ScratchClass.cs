using System;
using System.Collections.Generic;
using System.Text;

namespace CsGeneralBitmap
{
    public class ScratchClass
    {
        double X0, X1, Y0, Y1;
        double paramSettle;
        bool bIsRendering;
        const double RED=0, BLUE=0;
        Random R;

        int width; int height; double[] arrAns;
        public void getData()
        {
            //$$SECTION$$
            
            double xn, yn, xnew, a, b;
            a=0.7; b=1.7;
            double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
            double fx = X0, fy = Y1; //y counts downwards
            for (int x = 0; x < width; x+=1)
            {
                fy = Y1;
                for (int y=0; y<height; y++)
                {
                    xn = fx; yn=fy;
                    xnew = a*xn-yn*yn;//1-a*xn*xn + yn;
                    double ynew = b*yn+xn*yn;//b*xn;
                    double xnew2 = a*xnew-ynew*ynew;
                    double ynew2 = b*ynew+xnew*ynew;//b*xn;
                    if (Math.Abs(xnew2-xn)<0.01 && Math.Abs(ynew2-yn)<0.01)
                        arrAns[y+x*height] *= 0.2;

                    fy -= dy;
                }
                fx += dx;
            }

            //$$SECTION$$
        }



    }
}

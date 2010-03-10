using System;
using System.Collections.Generic;
using System.Text;

namespace CsGeneralBitmap
{
    public class ScratchClass
    {
        double val,fx,fy;
        double c1,c2,c3,c4;
        double X0, X1, Y0, Y1;
        int paramSettle, paramIters;
        bool bIsRendering;
        const double RED=0, BLUE=0;
        Random R;

        int width; int height; double[] arrAns;
        public void getData()
        {
            //$$SECTION$$
            
            //$$standardloop
            val = fx*fy + Math.Abs(fx);
            //$$endstandardloop
            //$$SECTION$$
        }



    }
}


using System;

namespace alglib
{
    public class testsplineinterpolationunit
    {
        public static bool testsplineinterpolation(bool silent)
        {
            bool result = new bool();
            bool waserrors = new bool();
            bool cserrors = new bool();
            bool hserrors = new bool();
            bool aserrors = new bool();
            bool lserrors = new bool();
            bool dserrors = new bool();
            bool uperrors = new bool();
            bool cperrors = new bool();
            bool lterrors = new bool();
            bool ierrors = new bool();
            int n = 0;
            int i = 0;
            int k = 0;
            int pass = 0;
            int passcount = 0;
            int bltype = 0;
            int brtype = 0;
            double[] x = new double[0];
            double[] y = new double[0];
            double[] y2 = new double[0];
            double[] d = new double[0];
            double[] c = new double[0];
            double[] c2 = new double[0];
            double a = 0;
            double b = 0;
            double bl = 0;
            double br = 0;
            double t = 0;
            double sa = 0;
            double sb = 0;
            double v = 0;
            double lstep = 0;
            double h = 0;
            double l10 = 0;
            double l11 = 0;
            double l12 = 0;
            double l20 = 0;
            double l21 = 0;
            double l22 = 0;
            double s = 0;
            double ds = 0;
            double d2s = 0;
            double s2 = 0;
            double ds2 = 0;
            double d2s2 = 0;
            double vl = 0;
            double vm = 0;
            double vr = 0;
            double err = 0;
            int i_ = 0;

            waserrors = false;
            passcount = 20;
            lstep = 0.005;
            h = 0.00001;
            lserrors = false;
            cserrors = false;
            hserrors = false;
            aserrors = false;
            dserrors = false;
            cperrors = false;
            uperrors = false;
            lterrors = false;
            ierrors = false;
            
            //
            // General test: linear, cubic, Hermite, Akima
            //
            for(n=2; n<=8; n++)
            {
                x = new double[n-1+1];
                y = new double[n-1+1];
                d = new double[n-1+1];
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // Prepare task
                    //
                    a = -1-AP.Math.RandomReal();
                    b = +1+AP.Math.RandomReal();
                    bl = 2*AP.Math.RandomReal()-1;
                    br = 2*AP.Math.RandomReal()-1;
                    for(i=0; i<=n-1; i++)
                    {
                        x[i] = 0.5*(b+a)+0.5*(b-a)*Math.Cos(Math.PI*(2*i+1)/(2*n));
                        if( i==0 )
                        {
                            x[i] = a;
                        }
                        if( i==n-1 )
                        {
                            x[i] = b;
                        }
                        y[i] = Math.Cos(1.3*Math.PI*x[i]+0.4);
                        d[i] = -(1.3*Math.PI*Math.Sin(1.3*Math.PI*x[i]+0.4));
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        k = AP.Math.RandomInteger(n);
                        if( k!=i )
                        {
                            t = x[i];
                            x[i] = x[k];
                            x[k] = t;
                            t = y[i];
                            y[i] = y[k];
                            y[k] = t;
                            t = d[i];
                            d[i] = d[k];
                            d[k] = t;
                        }
                    }
                    
                    //
                    // Build linear spline
                    // Test for general interpolation scheme properties:
                    // * values at nodes
                    // * continuous function
                    // Test for specific properties is implemented below.
                    //
                    spline3.buildlinearspline(x, y, n, ref c);
                    err = 0;
                    for(i=0; i<=n-1; i++)
                    {
                        err = Math.Max(err, Math.Abs(y[i]-spline3.splineinterpolation(ref c, x[i])));
                    }
                    lserrors = lserrors | err>100*AP.Math.MachineEpsilon;
                    lconst(a, b, ref c, lstep, ref l10, ref l11, ref l12);
                    lconst(a, b, ref c, lstep/3, ref l20, ref l21, ref l22);
                    lserrors = lserrors | l20/l10>1.2;
                    
                    //
                    // Build cubic spline.
                    // Test for interpolation scheme properties:
                    // * values at nodes
                    // * boundary conditions
                    // * continuous function
                    // * continuous first derivative
                    // * continuous second derivative
                    //
                    for(bltype=0; bltype<=2; bltype++)
                    {
                        for(brtype=0; brtype<=2; brtype++)
                        {
                            spline3.buildcubicspline(x, y, n, bltype, bl, brtype, br, ref c);
                            err = 0;
                            for(i=0; i<=n-1; i++)
                            {
                                err = Math.Max(err, Math.Abs(y[i]-spline3.splineinterpolation(ref c, x[i])));
                            }
                            cserrors = cserrors | err>100*AP.Math.MachineEpsilon;
                            err = 0;
                            if( bltype==0 )
                            {
                                spline3.splinedifferentiation(ref c, a-h, ref s, ref ds, ref d2s);
                                spline3.splinedifferentiation(ref c, a+h, ref s2, ref ds2, ref d2s2);
                                t = (d2s2-d2s)/(2*h);
                                err = Math.Max(err, Math.Abs(t));
                            }
                            if( bltype==1 )
                            {
                                t = (spline3.splineinterpolation(ref c, a+h)-spline3.splineinterpolation(ref c, a-h))/(2*h);
                                err = Math.Max(err, Math.Abs(bl-t));
                            }
                            if( bltype==2 )
                            {
                                t = (spline3.splineinterpolation(ref c, a+h)-2*spline3.splineinterpolation(ref c, a)+spline3.splineinterpolation(ref c, a-h))/AP.Math.Sqr(h);
                                err = Math.Max(err, Math.Abs(bl-t));
                            }
                            if( brtype==0 )
                            {
                                spline3.splinedifferentiation(ref c, b-h, ref s, ref ds, ref d2s);
                                spline3.splinedifferentiation(ref c, b+h, ref s2, ref ds2, ref d2s2);
                                t = (d2s2-d2s)/(2*h);
                                err = Math.Max(err, Math.Abs(t));
                            }
                            if( brtype==1 )
                            {
                                t = (spline3.splineinterpolation(ref c, b+h)-spline3.splineinterpolation(ref c, b-h))/(2*h);
                                err = Math.Max(err, Math.Abs(br-t));
                            }
                            if( brtype==2 )
                            {
                                t = (spline3.splineinterpolation(ref c, b+h)-2*spline3.splineinterpolation(ref c, b)+spline3.splineinterpolation(ref c, b-h))/AP.Math.Sqr(h);
                                err = Math.Max(err, Math.Abs(br-t));
                            }
                            cserrors = cserrors | err>1.0E-3;
                            lconst(a, b, ref c, lstep, ref l10, ref l11, ref l12);
                            lconst(a, b, ref c, lstep/3, ref l20, ref l21, ref l22);
                            cserrors = cserrors | l20/l10>1.2 & l10>1.0E-6;
                            cserrors = cserrors | l21/l11>1.2 & l11>1.0E-6;
                            cserrors = cserrors | l22/l12>1.2 & l12>1.0E-6;
                        }
                    }
                    
                    //
                    // Build Hermite spline.
                    // Test for interpolation scheme properties:
                    // * values and derivatives at nodes
                    // * continuous function
                    // * continuous first derivative
                    //
                    spline3.buildhermitespline(x, y, d, n, ref c);
                    err = 0;
                    for(i=0; i<=n-1; i++)
                    {
                        err = Math.Max(err, Math.Abs(y[i]-spline3.splineinterpolation(ref c, x[i])));
                    }
                    hserrors = hserrors | err>100*AP.Math.MachineEpsilon;
                    err = 0;
                    for(i=0; i<=n-1; i++)
                    {
                        t = (spline3.splineinterpolation(ref c, x[i]+h)-spline3.splineinterpolation(ref c, x[i]-h))/(2*h);
                        err = Math.Max(err, Math.Abs(d[i]-t));
                    }
                    hserrors = hserrors | err>1.0E-3;
                    lconst(a, b, ref c, lstep, ref l10, ref l11, ref l12);
                    lconst(a, b, ref c, lstep/3, ref l20, ref l21, ref l22);
                    hserrors = hserrors | l20/l10>1.2;
                    hserrors = hserrors | l21/l11>1.2;
                    
                    //
                    // Build Akima spline
                    // Test for general interpolation scheme properties:
                    // * values at nodes
                    // * continuous function
                    // * continuous first derivative
                    // Test for specific properties is implemented below.
                    //
                    if( n>=5 )
                    {
                        spline3.buildakimaspline(x, y, n, ref c);
                        err = 0;
                        for(i=0; i<=n-1; i++)
                        {
                            err = Math.Max(err, Math.Abs(y[i]-spline3.splineinterpolation(ref c, x[i])));
                        }
                        aserrors = aserrors | err>100*AP.Math.MachineEpsilon;
                        lconst(a, b, ref c, lstep, ref l10, ref l11, ref l12);
                        lconst(a, b, ref c, lstep/3, ref l20, ref l21, ref l22);
                        hserrors = hserrors | l20/l10>1.2;
                        hserrors = hserrors | l21/l11>1.2;
                    }
                }
            }
            
            //
            // Special linear spline test:
            // test for linearity between x[i] and x[i+1]
            //
            for(n=2; n<=10; n++)
            {
                x = new double[n-1+1];
                y = new double[n-1+1];
                
                //
                // Prepare task
                //
                a = -1;
                b = +1;
                for(i=0; i<=n-1; i++)
                {
                    x[i] = a+(b-a)*i/(n-1);
                    y[i] = 2*AP.Math.RandomReal()-1;
                }
                spline3.buildlinearspline(x, y, n, ref c);
                
                //
                // Test
                //
                err = 0;
                for(k=0; k<=n-2; k++)
                {
                    a = x[k];
                    b = x[k+1];
                    for(pass=1; pass<=passcount; pass++)
                    {
                        t = a+(b-a)*AP.Math.RandomReal();
                        v = y[k]+(t-a)/(b-a)*(y[k+1]-y[k]);
                        err = Math.Max(err, Math.Abs(spline3.splineinterpolation(ref c, t)-v));
                    }
                }
                lserrors = lserrors | err>100*AP.Math.MachineEpsilon;
            }
            
            //
            // Special Akima test: test outlier sensitivity
            // Spline value at (x[i], x[i+1]) should depend from
            // f[i-2], f[i-1], f[i], f[i+1], f[i+2], f[i+3] only.
            //
            for(n=5; n<=10; n++)
            {
                x = new double[n-1+1];
                y = new double[n-1+1];
                y2 = new double[n-1+1];
                
                //
                // Prepare unperturbed Akima spline
                //
                a = -1;
                b = +1;
                for(i=0; i<=n-1; i++)
                {
                    x[i] = a+(b-a)*i/(n-1);
                    y[i] = Math.Cos(1.3*Math.PI*x[i]+0.4);
                }
                spline3.buildakimaspline(x, y, n, ref c);
                
                //
                // Process perturbed tasks
                //
                err = 0;
                for(k=0; k<=n-1; k++)
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        y2[i_] = y[i_];
                    }
                    y2[k] = 5;
                    spline3.buildakimaspline(x, y2, n, ref c2);
                    
                    //
                    // Test left part independence
                    //
                    if( k-3>=1 )
                    {
                        a = -1;
                        b = x[k-3];
                        for(pass=1; pass<=passcount; pass++)
                        {
                            t = a+(b-a)*AP.Math.RandomReal();
                            err = Math.Max(err, Math.Abs(spline3.splineinterpolation(ref c, t)-spline3.splineinterpolation(ref c2, t)));
                        }
                    }
                    
                    //
                    // Test right part independence
                    //
                    if( k+3<=n-2 )
                    {
                        a = x[k+3];
                        b = +1;
                        for(pass=1; pass<=passcount; pass++)
                        {
                            t = a+(b-a)*AP.Math.RandomReal();
                            err = Math.Max(err, Math.Abs(spline3.splineinterpolation(ref c, t)-spline3.splineinterpolation(ref c2, t)));
                        }
                    }
                }
                aserrors = aserrors | err>100*AP.Math.MachineEpsilon;
            }
            
            //
            // Differentiation, unpack test
            //
            for(n=2; n<=10; n++)
            {
                x = new double[n-1+1];
                y = new double[n-1+1];
                
                //
                // Prepare cubic spline
                //
                a = -1-AP.Math.RandomReal();
                b = +1+AP.Math.RandomReal();
                for(i=0; i<=n-1; i++)
                {
                    x[i] = a+(b-a)*i/(n-1);
                    y[i] = Math.Cos(1.3*Math.PI*x[i]+0.4);
                }
                spline3.buildcubicspline(x, y, n, 2, 0.0, 2, 0.0, ref c);
                
                //
                // Test diff
                //
                err = 0;
                for(pass=1; pass<=passcount; pass++)
                {
                    t = a+(b-a)*AP.Math.RandomReal();
                    spline3.splinedifferentiation(ref c, t, ref s, ref ds, ref d2s);
                    vl = spline3.splineinterpolation(ref c, t-h);
                    vm = spline3.splineinterpolation(ref c, t);
                    vr = spline3.splineinterpolation(ref c, t+h);
                    err = Math.Max(err, Math.Abs(s-vm));
                    err = Math.Max(err, Math.Abs(ds-(vr-vl)/(2*h)));
                    err = Math.Max(err, Math.Abs(d2s-(vr-2*vm+vl)/AP.Math.Sqr(h)));
                }
                dserrors = dserrors | err>0.001;
                
                //
                // Test copy
                //
                spline3.splinecopy(ref c, ref c2);
                err = 0;
                for(pass=1; pass<=passcount; pass++)
                {
                    t = a+(b-a)*AP.Math.RandomReal();
                    err = Math.Max(err, Math.Abs(spline3.splineinterpolation(ref c, t)-spline3.splineinterpolation(ref c2, t)));
                }
                cperrors = cperrors | err>100*AP.Math.MachineEpsilon;
                
                //
                // Test unpack
                //
                uperrors = uperrors | !testunpack(ref c, ref x);
                
                //
                // Test lin.trans.
                //
                err = 0;
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // LinTransX, general A
                    //
                    sa = 4*AP.Math.RandomReal()-2;
                    sb = 2*AP.Math.RandomReal()-1;
                    t = a+(b-a)*AP.Math.RandomReal();
                    spline3.splinecopy(ref c, ref c2);
                    spline3.splinelintransx(ref c2, sa, sb);
                    err = Math.Max(err, Math.Abs(spline3.splineinterpolation(ref c, t)-spline3.splineinterpolation(ref c2, (t-sb)/sa)));
                    
                    //
                    // LinTransX, special case: A=0
                    //
                    sb = 2*AP.Math.RandomReal()-1;
                    t = a+(b-a)*AP.Math.RandomReal();
                    spline3.splinecopy(ref c, ref c2);
                    spline3.splinelintransx(ref c2, 0, sb);
                    err = Math.Max(err, Math.Abs(spline3.splineinterpolation(ref c, sb)-spline3.splineinterpolation(ref c2, t)));
                    
                    //
                    // LinTransY
                    //
                    sa = 2*AP.Math.RandomReal()-1;
                    sb = 2*AP.Math.RandomReal()-1;
                    t = a+(b-a)*AP.Math.RandomReal();
                    spline3.splinecopy(ref c, ref c2);
                    spline3.splinelintransy(ref c2, sa, sb);
                    err = Math.Max(err, Math.Abs(sa*spline3.splineinterpolation(ref c, t)+sb-spline3.splineinterpolation(ref c2, t)));
                }
                lterrors = lterrors | err>100*AP.Math.MachineEpsilon;
            }
            
            //
            // Testing integration
            //
            err = 0;
            for(n=20; n<=35; n++)
            {
                x = new double[n-1+1];
                y = new double[n-1+1];
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // Prepare cubic spline
                    //
                    a = -1-0.2*AP.Math.RandomReal();
                    b = +1+0.2*AP.Math.RandomReal();
                    for(i=0; i<=n-1; i++)
                    {
                        x[i] = a+(b-a)*i/(n-1);
                        y[i] = Math.Sin(Math.PI*x[i]+0.4)+Math.Exp(x[i]);
                    }
                    bl = Math.PI*Math.Cos(Math.PI*a+0.4)+Math.Exp(a);
                    br = Math.PI*Math.Cos(Math.PI*b+0.4)+Math.Exp(b);
                    spline3.buildcubicspline(x, y, n, 1, bl, 1, br, ref c);
                    
                    //
                    // Test
                    //
                    t = a+(b-a)*AP.Math.RandomReal();
                    v = -(Math.Cos(Math.PI*a+0.4)/Math.PI)+Math.Exp(a);
                    v = -(Math.Cos(Math.PI*t+0.4)/Math.PI)+Math.Exp(t)-v;
                    v = v-spline3.splineintegration(ref c, t);
                    err = Math.Max(err, Math.Abs(v));
                }
            }
            ierrors = ierrors | err>0.001;
            
            //
            // report
            //
            waserrors = lserrors | cserrors | hserrors | aserrors | dserrors | cperrors | uperrors | lterrors | ierrors;
            if( !silent )
            {
                System.Console.Write("TESTING SPLINE INTERPOLATION");
                System.Console.WriteLine();
                
                //
                // Normal tests
                //
                System.Console.Write("LINEAR SPLINE TEST:                      ");
                if( lserrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("CUBIC SPLINE TEST:                       ");
                if( cserrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("HERMITE SPLINE TEST:                     ");
                if( hserrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("AKIMA SPLINE TEST:                       ");
                if( aserrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("DIFFERENTIATION TEST:                    ");
                if( dserrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("COPY TEST:                               ");
                if( cperrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("UNPACK TEST:                             ");
                if( uperrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("LIN.TRANS. TEST:                         ");
                if( lterrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("INTEGRATION TEST:                        ");
                if( ierrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                if( waserrors )
                {
                    System.Console.Write("TEST FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("TEST PASSED");
                    System.Console.WriteLine();
                }
                System.Console.WriteLine();
                System.Console.WriteLine();
            }
            
            //
            // end
            //
            result = !waserrors;
            return result;
        }


        /*************************************************************************
        Lipschitz constants for spline inself, first and second derivatives.
        *************************************************************************/
        private static void lconst(double a,
            double b,
            ref double[] c,
            double lstep,
            ref double l0,
            ref double l1,
            ref double l2)
        {
            double t = 0;
            double vl = 0;
            double vm = 0;
            double vr = 0;
            double prevf = 0;
            double prevd = 0;
            double prevd2 = 0;
            double f = 0;
            double d = 0;
            double d2 = 0;

            l0 = 0;
            l1 = 0;
            l2 = 0;
            t = a-0.1;
            vl = spline3.splineinterpolation(ref c, t-2*lstep);
            vm = spline3.splineinterpolation(ref c, t-lstep);
            vr = spline3.splineinterpolation(ref c, t);
            f = vm;
            d = (vr-vl)/(2*lstep);
            d2 = (vr-2*vm+vl)/AP.Math.Sqr(lstep);
            while( t<=b+0.1 )
            {
                prevf = f;
                prevd = d;
                prevd2 = d2;
                vl = vm;
                vm = vr;
                vr = spline3.splineinterpolation(ref c, t+lstep);
                f = vm;
                d = (vr-vl)/(2*lstep);
                d2 = (vr-2*vm+vl)/AP.Math.Sqr(lstep);
                l0 = Math.Max(l0, Math.Abs((f-prevf)/lstep));
                l1 = Math.Max(l1, Math.Abs((d-prevd)/lstep));
                l2 = Math.Max(l2, Math.Abs((d2-prevd2)/lstep));
                t = t+lstep;
            }
        }


        /*************************************************************************
        Lipschitz constants for spline inself, first and second derivatives.
        *************************************************************************/
        private static bool testunpack(ref double[] c,
            ref double[] x)
        {
            bool result = new bool();
            int i = 0;
            int n = 0;
            double err = 0;
            double t = 0;
            double v1 = 0;
            double v2 = 0;
            int pass = 0;
            int passcount = 0;
            double[,] tbl = new double[0,0];

            passcount = 20;
            err = 0;
            spline3.splineunpack(ref c, ref n, ref tbl);
            for(i=0; i<=n-2; i++)
            {
                for(pass=1; pass<=passcount; pass++)
                {
                    t = AP.Math.RandomReal()*(tbl[i,1]-tbl[i,0]);
                    v1 = tbl[i,2]+t*tbl[i,3]+AP.Math.Sqr(t)*tbl[i,4]+t*AP.Math.Sqr(t)*tbl[i,5];
                    v2 = spline3.splineinterpolation(ref c, tbl[i,0]+t);
                    err = Math.Max(err, Math.Abs(v1-v2));
                }
            }
            for(i=0; i<=n-2; i++)
            {
                err = Math.Max(err, Math.Abs(x[i]-tbl[i,0]));
            }
            for(i=0; i<=n-2; i++)
            {
                err = Math.Max(err, Math.Abs(x[i+1]-tbl[i,1]));
            }
            result = err<100*AP.Math.MachineEpsilon;
            return result;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testsplineinterpolationunit_test_silent()
        {
            bool result = new bool();

            result = testsplineinterpolation(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testsplineinterpolationunit_test()
        {
            bool result = new bool();

            result = testsplineinterpolation(false);
            return result;
        }
    }
}

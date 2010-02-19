//http://bytes.com/topic/c-sharp/answers/722688-codedomprovider-createcompiler-obsolete-fix
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace CodedomEvaluator
{
    public static class CodedomTest
    {
        public static bool doubleEqual(double a, double b)
        {
            return Math.Abs(a-b)<0.0001;
        }
        public static string Test()
        {
            CodedomEvaluator cd = new CodedomEvaluator();
            string strErr, strExp; double result; Dictionary<string, double> d;

            strExp = "1+1";
            result = cd.simpleMathEval(strExp, out strErr);
            Debug.Assert(strErr == "");
            Debug.Assert(doubleEqual(result, 2.0));
            strExp = "1+1;";
            result = cd.simpleMathEval(strExp, out strErr);
            Debug.Assert(strErr == "");
            Debug.Assert(doubleEqual(result, 2.0));

            strExp = "2.6*1.1 + (7-3*2.4)";
            result = cd.simpleMathEval(strExp, out strErr);
            Debug.Assert(strErr == "");
            Debug.Assert(doubleEqual(result, 2.66));

            strExp = "x*x+4.0";
            result = cd.simpleMathEval(strExp, "x", 2.0, out strErr);
            Debug.Assert(strErr == "");
            Debug.Assert(doubleEqual(result, 8.0));

            strExp = "x*y + 1.0";
            d = new Dictionary<string,double>();
            d["x"] = 2.0; d["y"] = 3.0;
            result = cd.simpleMathEval(strExp, d, out strErr);
            Debug.Assert(strErr == "");
            Debug.Assert(doubleEqual(result, 7.0));

            strExp = "double z; z=2.0; ans = x*y + z;";
            d = new Dictionary<string, double>();
            d["x"] = 2.0; d["y"] = 3.0;
            result = cd.mathEval(strExp, d, out strErr);
            Debug.Assert(strErr == "");
            Debug.Assert(doubleEqual(result, 8.0));

            strExp = "arrAns[0]=x;arrAns[1]=x*2;arrAns[2]=x*3;";
            d = new Dictionary<string, double>();
            d["x"] = 15;
            double[] resultArr = cd.mathEvalArray(strExp, d,3, out strErr);
            Debug.Assert(strErr == "");
            Debug.Assert(doubleEqual(resultArr[0], 15.0));
            Debug.Assert(doubleEqual(resultArr[1], 30.0));
            Debug.Assert(doubleEqual(resultArr[2], 45.0));

            strExp = "for(int i=0;i<40;i++) arrAns[i]=i*i;";
            d = new Dictionary<string, double>();
            resultArr = cd.mathEvalArray(strExp, d, 40, out strErr);
            Debug.Assert(strErr == "");
            for (int i=0;i<40;i++)
                Debug.Assert(doubleEqual(resultArr[i], i*i));

            strExp = "FN1 inf=delegate(double a) { return a*a; }; ans = inf(4.0);";
            d = new Dictionary<string, double>();
            result = cd.mathEval(strExp, d, out strErr);
            Debug.Assert(strErr == "");
            Debug.Assert(doubleEqual(result, 16.0));

            strExp = "FN2 inf=delegate(double a,double b) { return a/(b+1); }; for(int i=0;i<40;i++) arrAns[i]=inf(i,i);";
            d = new Dictionary<string, double>();
            resultArr = cd.mathEvalArray(strExp, d, 40, out strErr);
            Debug.Assert(strErr == "");
            for (int i=0; i<40; i++)
                Debug.Assert(doubleEqual(resultArr[i], i/((double)i+1)));

            string strLoopExp = "val = x*x*x;";
            resultArr=CodedomLoopExpression.codedomLoopExpression("", strLoopExp, 1.0, 0.1, 50, out strErr);
            Debug.Assert(strErr == "");
            double x=1.0;
            for (int i=0; i<50; i++)
            {
                Debug.Assert(doubleEqual(resultArr[i], x*x*x));
                x+=0.1;
            }

            //create an overflow. We expect exception to be caught earlier, and that strErr records result.
            try
            {
                strExp = "arrAns[40]=30.1;";
                d = new Dictionary<string, double>();
                resultArr = cd.mathEvalArray(strExp, d, 1, out strErr);
            }
            catch (Exception )
            {
                Debug.Assert(false); //an exception occurred.
            }
            Debug.Assert(strErr != "");



            Debug.Assert(cd.addMathMethods("4*sin(.4)+Sin(acos(.9))")=="4*Math.Sin(.4)+Math.Sin(Math.Acos(.9))");
            Debug.Assert(cd.addMathMethods("4*othersin(.4) + ocoso(.6)")=="4*othersin(.4) + ocoso(.6)");
            return "";
        }


    }
}

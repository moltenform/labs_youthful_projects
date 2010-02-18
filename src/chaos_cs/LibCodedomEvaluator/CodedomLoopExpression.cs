using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace CodedomEvaluator
{
    public class CodedomLoopExpression
    {
        //note to use CultureInfo.InvariantCulture when turning doubles to strings
        //assumes variable is "x"
        //place result into "val"
        public static double[] codedomLoopExpression(string strAdditionalExp, string strLoopExp, double xstart, double xinc, int nIters, out string strError)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(strAdditionalExp);
            sb.AppendLine("double val;");
            sb.AppendLine("double x = "+xstart.ToString(CultureInfo.InvariantCulture)+";");
            sb.AppendLine( "for (int iii=0; iii<"+nIters.ToString()+"; iii++) {");
            sb.AppendLine(strLoopExp);
            sb.AppendLine( "arrAns[iii]=val;");
            sb.AppendLine( "x+= "+xinc.ToString(CultureInfo.InvariantCulture)+"; }");

            CodedomEvaluator cd = new CodedomEvaluator();
            return cd.mathEvalArray(sb.ToString(), new Dictionary<string, double>(), nIters, out strError);

        }


    }
}

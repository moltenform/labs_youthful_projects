using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace CodedomEvaluator
{
    public class CodedomGeneral
    {
        protected string sReferencedDll="";
        public CodedomGeneral() : this(null) {}
        public CodedomGeneral(string strDll)
        {
            if (strDll != null) sReferencedDll = strDll;
        }

        public object evaluateGeneral(string strExp, string sNamespace, string sReturnType, out string strError)
        {
            object ret = buildAndRun(strExp, sNamespace, sReturnType, out strError);
            if (ret==null || strError != "") return null;
            return ret;
        }
        public double[] evaluateArray(string strExp, out string strError) { return evaluateArray(strExp, "", out strError); }
        public double[] evaluateArray(string strExp, string sNamespace, out string strError)
        {
            object ret = buildAndRun(strExp, sNamespace, "", out strError);
            if (ret==null || strError != "") return null;
            double[] d = (double[])ret;
            return d;
        }

        protected object buildAndRun(string expression, string sNamespace, string sReturnType, out string strOutError)
        {
            strOutError = ""; 
            // build the class using codedom
            string source = generateClass(expression, sNamespace, sReturnType, out strOutError);
            if (source==null) { return null; }


            // compile the class into an in-memory assembly.
            // if it doesn't compile, show errors in the window
            CompilerResults results = CompileAssembly(source);

            // if the code compiled okay,
            // run the code using the new assembly (which is inside the results)
            if (results.Errors.Count > 0 || results.CompiledAssembly == null)
            {
                strOutError = "Error";
                //Do we have any compiler errors?
                if (results.Errors.Count > 0)
                {
                    foreach (CompilerError error in results.Errors)
                        strOutError+="\r\n" + error.ErrorText;
                }
                return null;
            }
            else
            {
                // run the evaluation function
                return RunCode(results, out strOutError);
            }

        }

        ICodeCompiler CreateCompiler()
        {
            //Create an instance of the C# compiler   
            CodeDomProvider codeProvider = null;
            codeProvider = new CSharpCodeProvider();
            ICodeCompiler compiler = codeProvider.CreateCompiler();
            return compiler;
        }

        protected string generateClass(string sSource, string sNamespace, string returnType, out string sError)
        {
            sError="";
            string sMarkBegin = "//$$Main";
            string sMarkEnd = "//$$EndMain";
            if (sSource.Split(new string[] { sMarkBegin }, StringSplitOptions.None).Length!=2) { sError = "Needs one beginning:"+sMarkBegin; return null; }
            if (sSource.Split(new string[] { sMarkEnd }, StringSplitOptions.None).Length!=2) { sError = "Needs one ending:"+sMarkEnd; return null; }
            string s3 = strStructure3;
            if (returnType!=null && returnType!="")
                s3 = s3.Replace("double[]", returnType);
            sSource = sSource.Replace(sMarkBegin, s3);
            sSource = sSource.Replace(sMarkEnd, strStructure4);
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(strStructure1);
            if (sNamespace!=null && sNamespace!="") sb.AppendLine("using "+sNamespace+";");
            sb.AppendLine(strStructure2);
            sb.AppendLine(sSource);
            sb.AppendLine(strStructure5);
            return sb.ToString();
        }

        string strStructure1 =@"namespace ExpressionEvaluator { ";

        string strStructure2 = @"
    using System;
    
    public class Calculator {
";
        string strStructure3 = @" public virtual double[] Calculate() {
		double[] result;
";
        string strStructure4 = @" return result; } ";
        string strStructure5 = @" 
       public delegate double FN1(double p0);
        
        public delegate double FN2(double p0, double p1);
        
        public delegate double FN3(double p0, double p1, double p2);
    }
}";



        /// <summary>
        /// Runs the Calculate method in our on-the-fly assembly
        /// </summary>
        /// <param name="results"></param>
        private object RunCode(CompilerResults results, out string strError)
        {
            strError = "";
            object ret = null;
            Assembly executingAssembly = results.CompiledAssembly;
            //cant call the entry method if the assembly is null
            if (executingAssembly != null)
            {
                object assemblyInstance = executingAssembly.CreateInstance("ExpressionEvaluator.Calculator");
                //Use reflection to call the static Main function

                Module[] modules = executingAssembly.GetModules(false);
                Type[] types = modules[0].GetTypes();

                //loop through each class that was defined and look for the first occurrance of the entry point method
                foreach (Type type in types)
                {
                    MethodInfo[] mis = type.GetMethods();
                    foreach (MethodInfo mi in mis)
                    {
                        if (mi.Name == "Calculate")
                        {
                            //catch run-time exceptions
                            try
                            {
                                object result = mi.Invoke(assemblyInstance, null);
                                //provide the array as a parameter here?
                                ret = result;
                            }
                            catch (Exception e)
                            {
                                strError = "Runtime error:" + e.ToString();
                                ret = null;
                            }
                            break;
                        }
                    }
                }

            }
            return ret;
        }

        /// <summary>
        /// Compiles the c# into an assembly if there are no syntax errors
        /// </summary>
        /// <returns></returns>
        private CompilerResults CompileAssembly(string source)
        {
            // create a compiler
            ICodeCompiler compiler = CreateCompiler();
            // get all the compiler parameters
            CompilerParameters parms = CreateCompilerParameters();
            // compile the code into an assembly

            return CompileCode(compiler, parms, source);
        }
        //Returns "" if no errors, an error string if there are errors
        private CompilerResults CompileCode(ICodeCompiler compiler, CompilerParameters parms, string source)
        {
            //actually compile the code
            CompilerResults results = compiler.CompileAssemblyFromSource(
                                        parms, source);
            return results;
        }
        CompilerParameters CreateCompilerParameters()
        {
            //add compiler parameters and assembly references
            CompilerParameters compilerParams = new CompilerParameters();
            compilerParams.CompilerOptions = "/target:library /optimize";
            compilerParams.GenerateExecutable = false;
            compilerParams.GenerateInMemory = true;
            compilerParams.IncludeDebugInformation = false;
            compilerParams.ReferencedAssemblies.Add("mscorlib.dll");
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            if (sReferencedDll != null && sReferencedDll!="")
                compilerParams.ReferencedAssemblies.Add(sReferencedDll);
            //note: reference to System.Windows.Forms.dll allows Clipboard and MessageBox

            //add any aditional references needed
            //            foreach (string refAssembly in code.References)
            //              compilerParams.ReferencedAssemblies.Add(refAssembly);
            
            return compilerParams;
        }
    }
}



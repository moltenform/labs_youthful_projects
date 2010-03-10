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
    public class CodedomEvaluator
    {
        private Dictionary<string,string> dictTranslate;
        public CodedomEvaluator()
        {
            dictTranslate = new Dictionary<string, string>();
            dictTranslate["E"] = "Math.E";
            dictTranslate["PI"] = "Math.PI";
            dictTranslate["ABS"] = "Math.Abs";
            dictTranslate["ACOS"] = "Math.Acos";
            dictTranslate["ASIN"] = "Math.Asin";
            dictTranslate["ATAN"] = "Math.Atan";
            dictTranslate["ATAN2"] = "Math.Atan2";
            dictTranslate["CEILING"] = "Math.Ceiling";
            dictTranslate["COS"] = "Math.Cos";
            dictTranslate["COSH"] = "Math.Cosh";
            dictTranslate["EXP"] = "Math.Exp";
            dictTranslate["FLOOR"] = "Math.Floor";
            dictTranslate["LOG"] = "Math.Log";
            dictTranslate["LOG10"] = "Math.Log10";
            dictTranslate["MAX"] = "Math.Max";
            dictTranslate["MIN"] = "Math.Min";
            dictTranslate["POW"] = "Math.Pow";
            dictTranslate["ROUND"] = "Math.Round";
            dictTranslate["SIN"] = "Math.Sin";
            dictTranslate["SINH"] = "Math.Sinh";
            dictTranslate["SQRT"] = "Math.Sqrt";
            dictTranslate["TAN"] = "Math.Tan";
            dictTranslate["TANH"] = "Math.Tanh";
        }
        public string addMathMethods(string s)
        {
            // instead of doing some type of weird regex search, this is simpler and better
            foreach (KeyValuePair<string, string> pair in this.dictTranslate)
            {
                s = Regex.Replace(s, "\\b" + pair.Key + "\\b", pair.Value, RegexOptions.IgnoreCase);
            }
            return s;
        }

        //public double CompileAndRun(string expression, out string strError)
        private object buildAndRun(string expression, Dictionary<string, double> vars, bool bSimpleAssign, bool bRetArray, int arrayLen, out string strError)
        {
            _source = new StringBuilder();
            strError = "";
            // build the class using codedom
            BuildClass(expression, vars, bSimpleAssign, bRetArray, arrayLen);

            // compile the class into an in-memory assembly.
            // if it doesn't compile, show errors in the window
            CompilerResults results = CompileAssembly();


            strError = "";

            //System.Diagnostics.Debug.WriteLine(_source.ToString());
            // if the code compiled okay,
            // run the code using the new assembly (which is inside the results)
            if (results.Errors.Count > 0 || results.CompiledAssembly == null)
            {
                strError = "Error";
                //Do we have any compiler errors?
                if (results.Errors.Count > 0)
                {
                    foreach (CompilerError error in results.Errors)
                        strError+="\r\n" + error.ErrorText;
                }
                return null;
            }
            else
            {
                // run the evaluation function
                return RunCode(results, out strError);
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
        /// <summary>
        /// Create parameters for compiling
        /// </summary>
        /// <returns></returns>
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

            //add any aditional references needed
            //            foreach (string refAssembly in code.References)
            //              compilerParams.ReferencedAssemblies.Add(refAssembly);

            return compilerParams;
        }

        //Returns "" if no errors, an error string if there are errors
        private CompilerResults CompileCode(ICodeCompiler compiler, CompilerParameters parms, string source)
        {
            //actually compile the code
            CompilerResults results = compiler.CompileAssemblyFromSource(
                                        parms, source);


            return results;
        }

       
       
        /// <summary>
        /// Compiles the c# into an assembly if there are no syntax errors
        /// </summary>
        /// <returns></returns>
        private CompilerResults CompileAssembly()
        {
            // create a compiler
            ICodeCompiler compiler = CreateCompiler();
            // get all the compiler parameters
            CompilerParameters parms = CreateCompilerParameters();
            // compile the code into an assembly
            
            return CompileCode(compiler, parms, _source.ToString());

        }
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


        CodeMemberField FieldVariable(string fieldName, string typeName, MemberAttributes accessLevel)
        {
            CodeMemberField field = new CodeMemberField(typeName, fieldName);
            field.Attributes = accessLevel;
            return field;
        }
        CodeMemberField FieldVariable(string fieldName, Type type, MemberAttributes accessLevel)
        {
            CodeMemberField field = new CodeMemberField(type, fieldName);
            field.Attributes = accessLevel;
            return field;
        }

        /// <summary>
        /// Very simplistic getter/setter properties
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="internalName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        CodeMemberProperty MakeProperty(string propertyName, string internalName, Type type)
        {
            CodeMemberProperty myProperty = new CodeMemberProperty();
            myProperty.Name = propertyName;
            myProperty.Comments.Add(new CodeCommentStatement(String.Format("The {0} property is the returned result", propertyName)));
            myProperty.Attributes = MemberAttributes.Public;
            myProperty.Type = new CodeTypeReference(type);
            myProperty.HasGet = true;
            myProperty.GetStatements.Add(
                new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), internalName)));

            myProperty.HasSet = true;
            myProperty.SetStatements.Add(
                new CodeAssignStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), internalName),
                    new CodePropertySetValueReferenceExpression()));

            return myProperty;
        }



        StringBuilder _source;

        //NEITHER need a "return".
        //just an expression, not many statements. (i.e. just "4+6;" is accepted)
        public double simpleMathEval(string strExp, out string strError) { return simpleMathEval(strExp,new Dictionary<string, double>(), out strError); }
        public double simpleMathEval(string strExp, string strVarname, double varValue, out string strError)
        {
            Dictionary<string, double> d =new Dictionary<string, double>();
            d[strVarname] = varValue;
            return simpleMathEval(strExp,d, out strError);
        }
        public double simpleMathEval(string strExp, Dictionary<string, double> vars, out string strError)
        {
            object ret = buildAndRun(strExp, vars, /*bSimpleAssign*/ true, /*bRetArray*/ false, -1 /*arrayLen*/, out strError);
            if (strError != "") return 0;
            Double d = (Double)ret;
            return d;
        }

        //requires statements. (i.e. needs "ans=4+6;" and not "4+6;")

        public double mathEval(string strExp, Dictionary<string, double> vars, out string strError)
        {
            object ret = buildAndRun(strExp, vars, /*bSimpleAssign*/ false, /*bRetArray*/ false, -1 /*arrayLen*/, out strError);
            if (strError != "") return 0;
            Double d = (Double)ret;
            return d;
        }
        public double[] mathEvalArray(string strExp, Dictionary<string, double> vars, int arrayLen, out string strError)
        {
            object ret = buildAndRun(strExp, vars, /*bSimpleAssign*/ false, /*bRetArray*/ true, arrayLen, out strError);
            if (strError != "") return null;
            double[] d = (double[])ret;
            return d;
        }



        /// <summary>
        /// Main driving routine for building a class
        /// </summary>
        void BuildClass(string expression, Dictionary<string, double> vars, bool bSimpleAssign, bool bRetArray, int arrayLen)
        {
            // need a string to put the code into
            _source = new StringBuilder();
            StringWriter sw = new StringWriter(_source);

            //Declare provider and generator
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            ICodeGenerator generator = codeProvider.CreateGenerator(sw);
            CodeGeneratorOptions codeOpts = new CodeGeneratorOptions();

            CodeNamespace myNamespace = new CodeNamespace("ExpressionEvaluator");
            myNamespace.Imports.Add(new CodeNamespaceImport("System"));
            //myNamespace.Imports.Add(new CodeNamespaceImport("System.Windows.Forms")); 

            //Build the class declaration and member variables			
            CodeTypeDeclaration classDeclaration = new CodeTypeDeclaration();
            classDeclaration.IsClass = true;
            classDeclaration.Name = "Calculator";
            classDeclaration.Attributes = MemberAttributes.Public;
            classDeclaration.Members.Add(FieldVariable("ans", typeof(double), MemberAttributes.Private));
            classDeclaration.Members.Add(FieldVariable("arrAns", typeof(double[]), MemberAttributes.Private));
            foreach (KeyValuePair<string, double> k in vars)
                classDeclaration.Members.Add(FieldVariable(k.Key, typeof(double), MemberAttributes.Private));

            //default constructor
            CodeConstructor defaultConstructor = new CodeConstructor();
            defaultConstructor.Attributes = MemberAttributes.Public;
            defaultConstructor.Comments.Add(new CodeCommentStatement("Default Constructor for class", true));
            classDeclaration.Members.Add(defaultConstructor);

            CodeTypeDelegate dg = new CodeTypeDelegate("FN1");
            dg.ReturnType = new CodeTypeReference(typeof(double));
            dg.Parameters.Add(new CodeParameterDeclarationExpression(typeof(double), "p0"));
            classDeclaration.Members.Add(dg);
            dg = new CodeTypeDelegate("FN2");
            dg.ReturnType = new CodeTypeReference(typeof(double));
            dg.Parameters.Add(new CodeParameterDeclarationExpression(typeof(double), "p0"));
            dg.Parameters.Add(new CodeParameterDeclarationExpression(typeof(double), "p1"));
            classDeclaration.Members.Add(dg);
            dg = new CodeTypeDelegate("FN3");
            dg.ReturnType = new CodeTypeReference(typeof(double));
            dg.Parameters.Add(new CodeParameterDeclarationExpression(typeof(double), "p0"));
            dg.Parameters.Add(new CodeParameterDeclarationExpression(typeof(double), "p1"));
            dg.Parameters.Add(new CodeParameterDeclarationExpression(typeof(double), "p2"));
            classDeclaration.Members.Add(dg);

            //property
            classDeclaration.Members.Add(this.MakeProperty("Ans", "ans", typeof(double)));
            classDeclaration.Members.Add(this.MakeProperty("ArrAns", "arrAns", typeof(double[])));

            //Our Calculate Method
            CodeMemberMethod myMethod = new CodeMemberMethod();
            myMethod.Name = "Calculate";
            myMethod.Attributes = MemberAttributes.Public;
            if (bRetArray)
            {
                myMethod.Statements.Add(new CodeAssignStatement(new CodeSnippetExpression("arrAns"), new CodeSnippetExpression("new double["+arrayLen.ToString(CultureInfo.InvariantCulture)+"]")));
                myMethod.ReturnType = new CodeTypeReference(typeof(double[]));
            }
            else
            {
                myMethod.Statements.Add(new CodeAssignStatement(new CodeSnippetExpression("ans"), new CodeSnippetExpression("0.0")));
                myMethod.ReturnType = new CodeTypeReference(typeof(double));
            }
            

            foreach (KeyValuePair<string, double> k in vars)
                myMethod.Statements.Add(new CodeAssignStatement(new CodeSnippetExpression(k.Key), new CodeSnippetExpression(k.Value.ToString(CultureInfo.InvariantCulture))));

            if (bSimpleAssign) //require a full statement?
                myMethod.Statements.Add(new CodeAssignStatement(new CodeSnippetExpression(bRetArray?"arrAns":"ans"), new CodeSnippetExpression(expression)));
            else
                myMethod.Statements.Add(new CodeSnippetExpression(expression));


             if (bRetArray)
                 myMethod.Statements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "arrAns")));
             else
                 myMethod.Statements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Ans")));

            classDeclaration.Members.Add(myMethod);
            //write code
            myNamespace.Types.Add(classDeclaration);
            generator.GenerateCodeFromNamespace(myNamespace, sw, codeOpts);
            sw.Flush();
            sw.Close();
        }
    }
}

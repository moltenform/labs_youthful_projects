An Excessively Fast Graphing Calculator
Ben Fisher, 2010, halfhourhacks.blogspot.com
Released under GPLv3.

Lately I've been exploring the code-generation features of the .NET framework. It's cool to be able to compile and run new code at runtime. The CSharpCodeProvider and System.CodeDom.Compiler classes provide this ability.

Evaluating code at runtime is especially useful when accepting user scripts, or evaluating a new mathematical expression. In Python, one can use eval() or exec to run code, but in C# the process is more complicated. So, I wrote a wrapping class with a convenient interface, that can be reused in other projects.

For example, 
public double simpleMathEval(string strExp, out string strErr)
can easily evaluate an expression:
double ret = simpleMathEval("1+3+Math.sin(4.6)", out strErr);
The more advanced method mathEval accepts any amount of code and inner functions.

As a proof of concept, I wrote a graphing calculator. It's really fast - compiling your expression. When you click Plot, a new program is generated that is essentially a loop containing your expression. (Compilation or runtime errors are caught and relayed back to you). On the graph, zoom in easily by clicking and dragging to draw a rectangle.

(Note that I try to be intelligent when drawing asymptotes, making this plot of cosecant look cleaner than if it'd been plotted in Matlab).

I based the code on the efforts of Mike Gold's "CodeDom Calculator", but made a new interface and extended it to allow arrays. As the title of this blog suggests, though, I haven't polished or thoroughly tested it. 

A more advanced example - you can put all sorts of code in here.

To see how to use CodedomEvaluator, CodedomTest.cs contains some examples.
CodedomEvaluator Usage:
double simpleMathEval(string strExp, out string strErr)
-evaluates a string, not a full statement. example: "3.0*5.1"returns 15.3.
double simpleMathEval(string strExp, string strVarname, double varValue, out string strErr)
-same as above, but provide a variable. 
example: strVarname="x", varValue=3, "x*x" returns 9.0.
double mathEval(string strExp, Dictionary<string, double> vars, out string strErr)
-evaluates a string that is a full statement. must assign to "ans". 
the vars dictionary can be used to provide variables.
example: vars["x"] = 3.0, "ans=x+4;" returns 7.0.
double[] mathEvalArray(string strExp, Dictionary<string, double> vars, int arrayLen, out string strErr)
-like above, but will return an array of values. The array arrAns[] will be created of length arrayLen.
example: arrayLen=40, "for(int i=0;i<40;i++) arrAns[i]=i*i;"
Because the inner loop is compiled, 
this is a fast way to evaluate an expression many times.



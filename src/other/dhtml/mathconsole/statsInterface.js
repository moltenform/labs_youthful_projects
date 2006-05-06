// Basic Interface
function clr()
{
	g_InputHistory = [];
	g_OutputHistory = [];
	document.getElementById("txtConsole").innerText = '';
}
clr.doc = function() { return ShowDoc("0.0.0"); }
AvailFunctions["0.0.0"] = "clr";
Docs.Prototypes["0.0.0"] = 'clr()';
Docs.ProtoExplain["0.0.0"] = '';
Docs.Description["0.0.0"] = "Clear the console.";


function clrmem() { mem = new Object; }
clrmem.doc = function() { return ShowDoc("0.0.1"); }
AvailFunctions["0.0.1"] = "clrmem";
Docs.Prototypes["0.0.1"] = 'clrmem()';
Docs.ProtoExplain["0.0.1"] = '';
Docs.Description["0.0.1"] = "Clear the memory.";


function ans( nIndex )
{
	if (nIndex === null || nIndex === undefined)
		return g_OutputHistory[g_OutputHistory.length-1 ];
	else
		return g_OutputHistory[g_OutputHistory.length-1-nIndex ];
}
ans.doc = function() { return ShowDoc("0.0.2"); }
AvailFunctions["0.0.2"] = "ans";
Docs.Prototypes["0.0.2"] = 'ans( [x] )';
Docs.ProtoExplain["0.0.2"] = "Where x is integer (optional).";
Docs.Description["0.0.2"] = "Interface:Returns the result of a previous command.";


function echo( strIn )
{
	var o = document.getElementById("txtConsole");
	o.value += '\n' + FixAlignRight(strIn);
	o.scrollTop = 1000;
}
echo.doc = function() { return ShowDoc("0.0.3"); }
AvailFunctions["0.0.3"] = "ans";
Docs.Prototypes["0.0.3"] = 'ans( [x] )';
Docs.ProtoExplain["0.0.3"] = "Where x is integer (optional).";
Docs.Description["0.0.3"] = "Interface:Returns the result of a previous command.";












//http://www.crockford.com/javascript/remedial.html
//http://www.crockford.com/javascript/inheritance.html

/*
Distribution Object
--------------
Type
Mean
Variance / Degrees Freedom / Parameter
*/
/*
function D( )
{
	var argv = D.arguments;

	if (! argv)
	{
		this.type = null;
		this.mean = null;
		this.variance = null;
		this.stddev = null;
		return;
	}
	var strType = argv[0];
	
	switch (strType)
	{
		case "n":
		case "normal":
			
			this.type = "normal";
			break;
			
			
		case "t":
		
		case "chisqr":
		case "x2":
		
			break;
		case "bernoulli":
		case "b2":
		
			break;
			
		case "binomial":
		case "bn":
		
		
			break;
		case "poisson":
		case "psn":
	
			break;	
		
		case "exponential":
		case "exp":
		
			break;
		
	}
	

	this.u = this.mean;
	this.o2 = this.variance;
	this.o = this.stddev;
	this.t = this.type;
}

D.prototype.Pdf = function (atValue)
{
	if (!this.CheckValues(["type"])) return;

}

D.prototype.CheckValues = function (astrProps)
{


}


//Others

function Sum( oIn )
{
	if isArray( oIn ) return ASum(oIn);
	else if isString( oIn )
	{
	
	
	}
}


//Parsing Data
//Usage: Get("r1"), Get("c4")
function Get( strIn )
{

}

*/
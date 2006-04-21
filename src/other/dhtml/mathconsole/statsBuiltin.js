//Returns list of available basic functions.
function doc()
{
	return 'Methods:\r\n' +
	'abs,acos,asin,atan,atan2,ceil,cos,exp,floor,ln,max,min,pow,random,round,sin,sqrt,tan' +
	'\r\n\r\nConstants:\r\nE,PI,RT2,RT1_2,LN2,LN10,LOG2E,LOG10E' +
	'\r\n\r\nCustom:  deg2rad,rad2deg' +
	'\r\nInterface:  ans,clr';
}


function deg2rad( x ) { return (x*(180/Math.PI)); }
function rad2deg( x ) { return (x*(Math.PI/180)); }
AvailFunctions["0.2.0"] = "deg2rad";
deg2rad.doc = function() { return ShowDoc("0.2.0"); }
Docs.Description["0.2.0"] = "Converts angle in degrees to radians.";
AvailFunctions["0.2.1"] = "rad2deg";
deg2rad.doc = function() { return ShowDoc("0.2.1"); }
Docs.Description["0.2.1"] = "Converts angle in radians to degrees.";
function randInt( x,y ) { return 4; }
function round2place( x,y ) { return 4; }


//Built in functions
AvailFunctions["0.0.0"] = "abs";
Docs.Description["0.0.0"] = "Absolute value of x.";
AvailFunctions["0.0.1"] = "acos";
Docs.ProtoExplain["0.0.1"] = WXIR+', [-1.0,1.0]';
Docs.Description["0.0.1"] = "Arccosine of x (radians).";
AvailFunctions["0.0.2"] = "asin";
Docs.ProtoExplain["0.0.2"] = WXIR+', [-1.0,1.0]';
Docs.Description["0.0.2"] = "Arcsin of x (radians).";
AvailFunctions["0.0.3"] = "atan";
Docs.Description["0.0.3"] = "Arctangent of x (radians).";
AvailFunctions["0.0.4"] = "atan2";
Docs.ProtoExplain["0.0.4"] = "Where y, x are reals.";
Docs.Description["0.0.4"] = "The angle theta (in radians) of an (x,y) point.";
Docs.Prototypes["0.0.4"] = "atan2(y,x)";

AvailFunctions["0.0.5"] = "ceil";
Docs.Description["0.0.5"] = "Round upwards to nearest integer.";
AvailFunctions["0.0.6"] = "cos";
Docs.Description["0.0.6"] = "Cosine of x (radians).";
AvailFunctions["0.0.f"] = "sin";
Docs.Description["0.0.f"] = "Sine of x (radians).";
AvailFunctions["0.0.h"] = "tan";
Docs.Description["0.0.h"] = "Tangent of x (radians).";

AvailFunctions["0.0.x"] = "temp";
Docs.Prototypes["0.0.x"] = "temp(x)";
Docs.ProtoExplain["0.0.x"] = WXIR;
Docs.Description["0.0.x"] = "Temp value of x.";


AvailFunctions["0.0.7"] = "exp";
Docs.Description["0.0.7"] = "E to the x power.";
AvailFunctions["0.0.8"] = "floor";
Docs.Description["0.0.8"] = "Round downwards to nearest integer.";
AvailFunctions["0.0.9"] = "ln";
Docs.ProtoExplain["0.0.9"] = WXIR+', x>0';
Docs.Description["0.0.9"] = "Natural logarithm of x.";
AvailFunctions["0.0.a"] = "max";
Docs.Prototypes["0.0.a"] = "max(a,b,c...)";
Docs.Description["0.0.a"] = "Returns the greatest argument.";
Docs.ProtoExplain["0.0.a"] = 'Where all arguments are reals';
AvailFunctions["0.0.b"] = "min";
Docs.Prototypes["0.0.b"] = "min(a,b,c...)";
Docs.ProtoExplain["0.0.b"] = 'Where all arguments are reals';
Docs.Description["0.0.b"] = "Returns the smallest argument.";
AvailFunctions["0.0.c"] = "pow";
Docs.Prototypes["0.0.c"] = "pow(x,y)";
Docs.ProtoExplain["0.0.c"] = 'x and y are reals';
Docs.Description["0.0.c"] = "Find x raised to the y power.";
AvailFunctions["0.0.d"] = "random";
Docs.Prototypes["0.0.d"] = "random()";
Docs.ProtoExplain["0.0.d"] = '';
Docs.Description["0.0.d"] = "Random number between 0 and 1.";
AvailFunctions["0.0.e"] = "round";
Docs.Description["0.0.e"] = "Round to the nearest integer x.";

AvailFunctions["0.0.g"] = "sqrt";
Docs.ProtoExplain["0.0.g"] = WXIR+', x>0';
Docs.Description["0.0.g"] = "Square root of x.";





//Basic Functions
function gcd(a,b) //Euclid's algorithm
{
	if (b==0) return a;
	else return gcd(b, a%b);
}
function lcm (a,b)
{
	return gcd(a,b);
}



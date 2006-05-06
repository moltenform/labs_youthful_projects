//Returns list of available basic functions.
function doc()
{
	return 'Methods:\r\n' +
	'abs,acos,asin,atan,atan2,ceil,cos,exp,floor,ln,max,min,pow,random,round,sin,sqrt,tan' +
	'\r\n\r\nConstants:\r\nE,PI,RT2,RT1_2,LN2,LN10,LOG2E,LOG10E' +
	'\r\n\r\nCustom:  deg2rad,rad2deg' +
	'\r\nInterface:  ans,clr';
}
AvailFunctions["0.0.x"] = "temp";
Docs.Prototypes["0.0.x"] = "temp(x)";
Docs.ProtoExplain["0.0.x"] = WXIR;
Docs.Description["0.0.x"] = "Temp value of x.";

function deg2rad( x ) { return x*(Math.PI/180); }
deg2rad.doc = function() { return ShowDoc("0.2.7"); }
AvailFunctions["0.2.7"] = "deg2rad";
Docs.Description["0.2.7"] = "Converts angle in degrees to radians.";

function rad2deg( x ) { return x*(180/Math.PI); }
deg2rad.doc = function() { return ShowDoc("0.2.8");}
AvailFunctions["0.2.8"] = "rad2deg";
Docs.Description["0.2.8"] = "Converts angle in radians to degrees.";

function sinh(x) { return (Math.exp(x)-Math.exp(-x))/2;}
sinh.doc = function() { return ShowDoc("0.2.9"); }
AvailFunctions["0.2.9"] = "sinh";
Docs.Description["0.2.9"] = "Hyperbolic sine";

function cosh(x) { return (Math.exp(x)+Math.exp(-x))/2;}
cosh.doc = function() { return ShowDoc("0.2.a"); }
AvailFunctions["0.2.a"] = "cosh";
Docs.Description["0.2.a"] = "Hyperbolic cosine";

function tanh(x) { return (Math.exp(x)-Math.exp(-x))/(Math.exp(x)+Math.exp(-x));}
tanh.doc = function() { return ShowDoc("0.2.b"); }
AvailFunctions["0.2.b"] = "tanh";
Docs.Description["0.2.b"] = "Hyperbolic tangent";

function asinh(x) { return atanh(x/Math.sqrt(1+x*x));}
asinh.doc = function() { return ShowDoc("0.2.c"); }
AvailFunctions["0.2.c"] = "asinh";
Docs.Description["0.2.c"] = "Inverse hyperbolic sine";

function acosh(x) {return (x<1)? NaN : atanh(Math.sqrt(x*x-1)/x);}
acosh.doc = function() { return ShowDoc("0.2.d"); }
AvailFunctions["0.2.d"] = "acosh";
Docs.ProtoExplain["0.2.d"] = WXIR+', x>=1';
Docs.Description["0.2.d"] = "Inverse hyperbolic cosine";

function atanh(x) { return (Math.log((1+x)/(1-x)))/2; }
atanh.doc = function() { return ShowDoc("0.2.e"); }
AvailFunctions["0.2.e"] = "atanh";
Docs.ProtoExplain["0.2.e"] = WXIR+', abs(x)>=1';
Docs.Description["0.2.e"] = "Inverse hyperbolic cosine";

//solving a triangle
function _parsetri(strIn)
{
	var astr = strIn.split(',');
	if (!astr.length || astr.length!=3) return null;
	var data = [];
	var type = '', value = 0, nValue=0;
	for (var i=0; i<astr.length; i++)
	{
		var lc= astr[i].charAt(astr[i].length-1);
		if (lc == 'o')
		{
			nValue = parseFloat(astr[i].substr(0,astr[i].length-1)); if (isNaN(nValue)) return NaN;
			value = deg2rad(nValue);
			type = 'angle';
		}
		else if (lc=='*')
		{
			value = astr[i].substr(0,astr[i].length-1);
			type = 'angle';
		}
		else
		{
			value = astr[i];
			type = 'side';
		}
		nValue = parseFloat(value);
		if (isNaN(nValue)) return NaN;
		var o = new Object;
		o.type = type; o.value = nValue;
		data[data.length] = o;
	}
	return data;
}

//Trisolve implementation , see http://oakroadsystems.com/twt/solving.htm
//Input: Radians, Output: Degrees
function _trisolve(oTri)
{
	var a,b,c;
	var outTri = new Object;
	if ((oTri[0].type=='side') && (oTri[1].type=='side') && (oTri[2].type=='side'))
	{
		outTri.type = "SSS: Three Sides";
		outTri.A = a = oTri[0].value;
		outTri.B = b = oTri[1].value;
		outTri.C = c = oTri[2].value;
		outTri.a = rad2deg(Math.acos(       (Math.pow(c,2.)+Math.pow(b,2.)-Math.pow(a,2.))/(2.*b*c)                 ));
		outTri.b = rad2deg(Math.acos(       (Math.pow(a,2.)+Math.pow(c,2.)-Math.pow(b,2.))/(2.*a*c)                 ));
		outTri.c = rad2deg(Math.acos(       (Math.pow(a,2.)+Math.pow(b,2.)-Math.pow(c,2.))/(2.*a*b)                 ));
	}
	else if ((oTri[0].type=='side') && (oTri[1].type=='angle') && (oTri[2].type=='side'))
	{
		outTri.type = 'SAS: Two sides and included angle';
		outTri.A = a = oTri[0].value;
		outTri.B = b = oTri[2].value;
		outTri.C = c = Math.pow(      Math.pow(a,2.)+Math.pow(b,2.)-2.*a*b*Math.cos(oTri[1].value)                     ,0.5);
		outTri.a = rad2deg(Math.acos(       (Math.pow(c,2.)+Math.pow(b,2.)-Math.pow(a,2.))/(2.*b*c)                 ));
		outTri.b = rad2deg(Math.acos(       (Math.pow(a,2.)+Math.pow(c,2.)-Math.pow(b,2.))/(2.*a*c)                 ));
		outTri.c = rad2deg(oTri[1].value);
	}
	else if ((oTri[0].type=='angle') && (oTri[1].type=='angle') && (oTri[2].type=='side'))
	{
		outTri.type = "AAS: Two angles and non-included side";
		outTri.a = rad2deg(oTri[0].value);
		outTri.b = rad2deg(oTri[1].value);
		outTri.c = 180-outTri.a-outTri.b;
		outTri.A = oTri[2].value;
		outTri.B = oTri[2].value * Math.sin(oTri[1].value)/Math.sin(oTri[0].value);
		outTri.C = oTri[2].value * Math.sin(deg2rad(outTri.c))/Math.sin(oTri[0].value);
	}
	else if ((oTri[0].type=='angle') && (oTri[1].type=='side') && (oTri[2].type=='angle'))
	{
		outTri.type = "ASA: Two angles and included side";
		outTri.a = rad2deg(oTri[0].value);
		outTri.c = rad2deg(oTri[2].value);
		outTri.b = 180-outTri.a-outTri.c;
		outTri.A = oTri[1].value * Math.sin(oTri[0].value)/Math.sin(deg2rad(outTri.b));
		outTri.B = oTri[1].value;
		outTri.C = oTri[1].value * Math.sin(oTri[2].value)/Math.sin(deg2rad(outTri.b));
	}
	else if ((oTri[0].type=='side') && (oTri[1].type=='side') && (oTri[2].type=='angle'))
	{
		outTri.type = "SSA: Two sides and non-included angle";
		outTri.A = oTri[0].value;
		outTri.B = oTri[1].value;
		outTri.a = rad2deg(oTri[2].value);
		var h = oTri[1].value*Math.sin(oTri[2].value)
		if ((oTri[0].value<h)||(outTri.a>=180&&oTri[0].value<=oTri[1].value))
			return NaN;
		if (h<oTri[0].value && oTri[0].value<oTri[1].value)
		{
			var tb = rad2deg(Math.asin(oTri[1].value*Math.sin(oTri[2].value)/oTri[0].value));
			var outTri2 = cloneObject(outTri);
			outTri2.type = "SSA: Two sides and non-included angle (obtuse)";
			outTri2.b = tb;
			outTri2.c = 180.-outTri2.a-outTri2.b;
			outTri2.C = oTri[0].value*Math.sin(deg2rad(outTri2.c))/Math.sin(oTri[2].value);		
			
			outTri.type = "SSA: Two sides and non-included angle (acute)";
			outTri.b = 180.-tb;
			outTri.c = 180.-outTri.a-outTri.b;
			outTri.C = oTri[0].value*Math.sin(deg2rad(outTri.c))/Math.sin(oTri[2].value);
			outTri.area = 0.5*outTri.A*outTri.B*Math.sin(deg2rad(outTri.c));
			return [outTri, outTri2];
		}
		outTri.b = rad2deg(Math.asin(oTri[1].value*Math.sin(oTri[2].value)/oTri[0].value));
		outTri.c = 180.-outTri.a-outTri.b;
		outTri.C = oTri[0].value*Math.sin(deg2rad(outTri.c))/Math.sin(oTri[2].value);
	}
	outTri.area = 0.5*outTri.A*outTri.B*Math.sin(deg2rad(outTri.c));
	return [outTri];
}



function trisolve(strIn) 
{
	var oTri = _parsetri(strIn); if (! oTri) return null;
	var outTri = _trisolve(oTri);
	for (var i=0; i<outTri.length; i++)
	{
		echo (outTri[i].type);
		if (outTri[i].area!=NaN) echo('Area: '+outTri[i].area);
		echo ('A: ' + outTri[i].A.toFixed(5));
		echo ('B: ' + outTri[i].B.toFixed(5));
		echo ('C: ' + outTri[i].C.toFixed(5));
		echo ('a: ' + outTri[i].a.toFixed(5));
		echo ('b: ' + outTri[i].b.toFixed(5));
		echo ('c: ' + outTri[i].c.toFixed(5));
	}
	return '';
}


//Linear Algebra
function det(m)
{
	if (!m || !m[0] || !m[0].length) return null;
	if (m.length != m[0].length) return null; //must be sqr matrix
	var d1= m.length;
	
	if (d1 ==1)
		return m[0];
	else if (d1==2)
		return m[0][0]*m[1][1]-m[1][0]*m[0][1];
	else
	{
		var tms = m.length;
		var det = 1;
		var objOut = new Object;
		m = _UpperTriangle(m, objOut);

		for (var i = 0; i < tms; i++) {
			det = det * m[i][i];
		} // multiply down diagonal

		return det * objOut.iDF;// adjust w/ determinant factor
	}
}

function _UpperTriangle( m , objOut) //http://www.mkaz.com/math/
{
	var f1 = 0, temp = 0;
	var tms = m.length;
	var v = 1;
	var iDF = 1;

	for (var col = 0; col < tms - 1; col++) {
		for (var row = col + 1; row < tms; row++) {
			v = 1;
			outahere: while (m[col][col] == 0) // check if 0 in diagonal
			{ // if so switch until not
				if (col + v >= tms) // check if switched all rows
				{
					iDF = 0;
					break outahere;
				} else {
					for (var c = 0; c < tms; c++) {
						temp = m[col][c];
						m[col][c] = m[col + v][c]; // switch rows
						m[col + v][c] = temp;
					}
					v++; // count row switchs
					iDF = iDF * -1; // each switch changes determinant factor
				}
			}
			if (m[col][col] != 0) {
				try {
					f1 = (-1) * m[row][col] / m[col][col];
					for (var i = col; i < tms; i++) {
						m[row][i] = f1 * m[col][i] + m[row][i];
					}
				} catch (e) {
					alert(e);
				}
			}
		}
	}
	objOut.iDF = iDF;
	return m;
}






function matrix()
{
	var ar,i;
	if (arguments.length == 0) return[];
	else if (arguments.length == 1) for (i=0;i<arguments[0];i++) ar[i]=0;
	else if (arguments.length == 2)
	{
		for (i=0;i<arguments[0];i++) 
		{
			ar[i]=[];
			for (var j=0;j<arguments[1];j++) ar[i][j]=0;
		}
	}
	else if (arguments.length == 3)
	{
		for (i=0;i<arguments[0];i++) 
		{
			ar[i]=[];
			for (var j=0;j<arguments[1];j++) 
			{
				ar[i][j]=[];
				for (var k=0;k<arguments[2];k++) ar[i][j][k]=0;
			}
		}
	}
	else return null;
	return ar;
}


//Basic Functions
function fpart(x) { return x-Math.floor(x); }
fpart.doc = function() { return ShowDoc("0.1.b"); }
AvailFunctions["0.1.b"] = "fpart";
Docs.Description["0.1.b"] = "Fractional part";

var ipart = Math.floor;
ipart.doc = function() { return ShowDoc("0.1.c"); }
AvailFunctions["0.1.c"] = "ipart";
Docs.Description["0.1.c"] = "Integer part, identical to floor()";

function gcd(a,b) { return (b==0) ? a : gcd(b, a%b); }
gcd.doc = function() { return ShowDoc("0.1.d"); }
AvailFunctions["0.1.d"] = "gcd";
Docs.Prototypes["0.1.d"] = "gcd(a,b)";
Docs.ProtoExplain["0.1.d"] = 'a and b are integers.';
Docs.Description["0.1.d"] = "Greatest common divisor";

function lcm(a,b) { return (a*b)/gcd(a,b); }
lcm.doc = function() { return ShowDoc("0.1.e"); }
AvailFunctions["0.1.e"] = "lcm";
Docs.Prototypes["0.1.e"] = "lcm(a,b)";
Docs.ProtoExplain["0.1.e"] = 'a and b are integers.';
Docs.Description["0.1.e"] = "Least common multiple";

function log(a,b) { return (b==null||b==undefined)?Math.log(a):Math.log(a)/Math.log(b); }
log.doc = function() { return ShowDoc("0.1.f"); }
AvailFunctions["0.1.f"] = "log";
Docs.Prototypes["0.1.f"] = "log(a,[b])";
Docs.ProtoExplain["0.1.f"] = 'a is number, b is base.';
Docs.Description["0.1.f"] = "Logarithm with base b. If no base is specified, defaults to e.";




function factorize( n ) //http://en.wikipedia.org/wiki/Prime_factorization_algorithm
{
	if (n!=Math.floor(n) || n<1) return NaN;
	if (n==1 ) return [];
	else if (n==2) return [2];
	else if (n==3) return [3];
			
	var primes = [];
	var candidate = 2;
	while (!primes.length && candidate<=n)
	{
		if (n%candidate==0 && isprime(candidate))
		{
			primes.push(candidate); 
			primes = primes.concat( factorize(n/candidate) );
		}
		candidate++;
	}
	return primes;
}
AvailFunctions["0.0.x"] = "factorize";
Docs.Prototypes["0.0.x"] = "factorize(n)";
Docs.ProtoExplain["0.0.x"] = WXIN;
Docs.Description["0.0.x"] = "Temp value of x.";


function isprime( n ) 
{
	if (n!=Math.floor(n) || n<1) return NaN;
	if (n==1 ) return false;
	else if (n==2 || n==3) return true;
	for (var x=2; x<Math.floor(Math.sqrt(n))+1; x++) if (n%x == 0) return false;
	return true;
}
function randint(lo, hi)
{
	return Math.floor (Math.random()*((hi-lo)+1))+lo;
}


function chbase(x,b) {
	var x1=Math.round(x);
	return (x1.toString(b));
}



//Built in functions
AvailFunctions["0.1.2"] = "abs";
Docs.Description["0.1.2"] = "Absolute value of x.";
AvailFunctions["0.2.4"] = "acos";
Docs.ProtoExplain["0.2.4"] = WXIR+', [-1.0,1.0]';
Docs.Description["0.2.4"] = "Arccosine of x (radians).";
AvailFunctions["0.2.3"] = "asin";
Docs.ProtoExplain["0.2.3"] = WXIR+', [-1.0,1.0]';
Docs.Description["0.2.3"] = "Arcsin of x (radians).";
AvailFunctions["0.2.5"] = "atan";
Docs.Description["0.2.5"] = "Arctangent of x (radians).";
AvailFunctions["0.2.6"] = "atan2";
Docs.ProtoExplain["0.2.6"] = "Where y, x are reals.";
Docs.Description["0.2.6"] = "The angle theta (in radians) of an (x,y) point.";
Docs.Prototypes["0.2.6"] = "atan2(y,x)";

AvailFunctions["0.1.4"] = "ceil";
Docs.Description["0.1.4"] = "Round upwards to nearest integer.";
AvailFunctions["0.2.1"] = "cos";
Docs.Description["0.0.6"] = "Cosine of x (radians).";
AvailFunctions["0.2.0"] = "sin";
Docs.Description["0.2.0"] = "Sine of x (radians).";
AvailFunctions["0.2.2"] = "tan";
Docs.Description["0.0.h"] = "Tangent of x (radians).";

AvailFunctions["0.1.6"] = "exp";
Docs.Description["0.1.6"] = "E to the x power.";
AvailFunctions["0.1.3"] = "floor";
Docs.Description["0.1.3"] = "Round downwards to nearest integer.";
AvailFunctions["0.1.7"] = "ln";
Docs.ProtoExplain["0.1.7"] = WXIR+', x>0';
Docs.Description["0.1.3"] = "Natural logarithm of x.";
AvailFunctions["0.1.0"] = "max";
Docs.Prototypes["0.1.0"] = "max(a,b,c...)";
Docs.Description["0.1.0"] = "Returns the greatest argument.";
Docs.ProtoExplain["0.1.0"] = 'Where all arguments are reals';
AvailFunctions["0.1.1"] = "min";
Docs.Prototypes["0.1.1"] = "min(a,b,c...)";
Docs.ProtoExplain["0.1.1"] = 'Where all arguments are reals';
Docs.Description["0.1.1"] = "Returns the smallest argument.";
AvailFunctions["0.1.8"] = "pow";
Docs.Prototypes["0.1.8"] = "pow(x,y)";
Docs.ProtoExplain["0.1.8"] = 'x and y are reals';
Docs.Description["0.1.8"] = "Find x raised to the y power.";
AvailFunctions["0.1.a"] = "rand";
Docs.Prototypes["0.1.a"] = "rand()";
Docs.ProtoExplain["0.1.a"] = '';
Docs.Description["0.1.a"] = "Random number between 0 and 1.";
AvailFunctions["0.1.5"] = "round";
Docs.Description["0.1.5"] = "Round to the nearest integer x.";
AvailFunctions["0.1.9"] = "sqrt";
Docs.ProtoExplain["0.1.9"] = WXIR+', x>0';
Docs.Description["0.1.9"] = "Square root of x.";

//Operators
Operators[0] = '+';
OperatorsExplain[0] = 'Addition'
Operators[1] = '-';
OperatorsExplain[1] = 'Subtraction'
Operators[2] = '*';
OperatorsExplain[2] = 'Multiply'
Operators[3] = '/';
OperatorsExplain[3] = 'Divide'
Operators[4] = '%';
OperatorsExplain[4] = 'Modulus'
Operators[5] = '++';
OperatorsExplain[5] = 'Increment'
Operators[6] = '--';
OperatorsExplain[6] = 'Decrement'
Operators[7] = '==';
OperatorsExplain[7] = 'Equality of value'
Operators[8] = '===';
OperatorsExplain[8] = 'Equality of value and of type'
Operators[9] = '!=';
OperatorsExplain[9] = 'Inequality'
Operators[10] = '>';
OperatorsExplain[10] = 'Greater than'
Operators[11] = '<';
OperatorsExplain[11] = 'Less than'
Operators[12] = '>=';
OperatorsExplain[12] = 'Greater than or equal to'
Operators[13] = '<=';
OperatorsExplain[13] = 'Less than or equal to'
Operators[14] = '&&';
OperatorsExplain[14] = 'Logical and'
Operators[15] = '||';
OperatorsExplain[15] = 'Logical or'
Operators[16] = '!';
OperatorsExplain[16] = 'Logical not'


//Number variable Methods
AvailMethods[0]["0.6.0"] = "toFixed";
Docs.ProtoExplain["0.6.0"] = WXIN;
Docs.Description["0.6.0"] = "Pads number to x decimal digits. (JS1.5)";
AvailMethods[0]["0.6.1"] = "toPrecision";
Docs.ProtoExplain["0.6.1"] = WXIN;
Docs.Description["0.6.1"] = "Pads number to x significant figures. (JS1.5)";



//String methods





//Date methods



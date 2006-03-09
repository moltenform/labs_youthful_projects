///////////////////////////////
//Data
function findmean()
{
	var an = parseData( dataIn.value.split('\r\n') );
	if (an.length==0) { output.value = '(No data)'; return; }
	
	var total = 0;
	for (var i in an) total += an[i];
	outputData.value = total/an.length;
}

function findvar()
{
	var an = parseData( dataIn.value.split('\r\n') );
	if (an.length==0) { output.value = '(No data)'; return; }
	
	var total = 0;
	for (var i in an) total += an[i];
	var mean = total/an.length;
	total = 0;
	for (var i in an) total += Math.pow(an[i]-mean,2);
	
	outputData.value = (1/(an.length-1))*total;
}
function parseData( an )
{
	dataOut = [];
	for (var i=0;i<an.length;i++)
	{
		if (an[i]==='' || an[i]===null)
			continue;
		dataOut.push(parseInt(an[i],10));
	}
	return dataOut;
}



function invnormal(p)
{
	// Lower tail quantile for standard normal distribution function.
	//
	// This function returns an approximation of the inverse cumulative
	// standard normal distribution function.  I.e., given P, it returns
	// an approximation to the X satisfying P = Pr{Z <= X} where Z is a
	// random variable from the standard normal distribution.
	//
	// The algorithm uses a minimax approximation by rational functions
	// and the result has a relative error whose absolute value is less
	// than 1.15e-9.
	//
	// Author:      Peter J. Acklam
	// An algorithm with a relative error less than 1.15·10-9 in the entire region.
	
	// Coefficients in rational approximations
        var a = new Array(-3.969683028665376e+01,  2.209460984245205e+02,
                          -2.759285104469687e+02,  1.383577518672690e+02,
                          -3.066479806614716e+01,  2.506628277459239e+00);
        var b = new Array(-5.447609879822406e+01,  1.615858368580409e+02,
                          -1.556989798598866e+02,  6.680131188771972e+01,
                          -1.328068155288572e+01 );
        var c = new Array(-7.784894002430293e-03, -3.223964580411365e-01,
                          -2.400758277161838e+00, -2.549732539343734e+00,
                           4.374664141464968e+00,  2.938163982698783e+00);
        var d = new Array (7.784695709041462e-03,  3.224671290700398e-01,
                           2.445134137142996e+00,  3.754408661907416e+00);

        // Define break-points.
        var plow  = 0.02425;
        var phigh = 1 - plow;

        // Rational approximation for lower region:
        if ( p < plow ) {
                 var q  = Math.sqrt(-2*Math.log(p));
                 return (((((c[0]*q+c[1])*q+c[2])*q+c[3])*q+c[4])*q+c[5]) /
                                                 ((((d[0]*q+d[1])*q+d[2])*q+d[3])*q+1);
        }

        // Rational approximation for upper region:
        if ( phigh < p ) {
                 var q  = Math.sqrt(-2*Math.log(1-p));
                 return -(((((c[0]*q+c[1])*q+c[2])*q+c[3])*q+c[4])*q+c[5]) /
                                                        ((((d[0]*q+d[1])*q+d[2])*q+d[3])*q+1);
        }

        // Rational approximation for central region:
        var q = p - 0.5;
        var r = q*q;
        return (((((a[0]*r+a[1])*r+a[2])*r+a[3])*r+a[4])*r+a[5])*q /
                                 (((((b[0]*r+b[1])*r+b[2])*r+b[3])*r+b[4])*r+1);
}




/*  PROGRAMMED BY: T.Haavie  */
/*  DATE/VERSION: 88-06-23/1.0 */
/*  Uses an expansion in terms of Chebyshev polynomials. */
function ErrorFunction(x)
{
	var cof, value;
	var i, n;
	
	if (x >= -4.0 && x < 4.0)
		value = error1(x);
	else if (x >= 4.0)
		value = error2(x);
	else
		return null;

	return value;
}


function error1(x) //Calculation of the error function  for -4<= x <= 4
{
	var MAX = 28;  //The number of coefficients in a[].
	var a = [ 3.8873036552229044,
		-1.3816314200197992,
		0.6473164048545842,
		-0.3059310244220356,
		0.1386797472020301,
		-0.0592474565912590,
		0.0236917518249282,
		-0.0088473626352405,
		0.0030856617113609,
		-0.0010063863512380,
		0.0003075463288431,
		-0.0000882619837554,
		0.0000238450961661,
		-0.0000060791002851,
		0.0000014659721734,
		-0.0000003351599343,
		0.0000000728057954,
		-0.0000000150579118,
		0.0000000029709474,
		-0.0000000005602127,
		0.0000000001011316,
		-0.0000000000175065,
		0.0000000000029104,
		-0.0000000000004653,
		0.0000000000000716,
		-0.0000000000000106,
		0.0000000000000015,
		-0.0000000000000002 ];

	var k;
	var arg, t, value, b0, b1, b2;

	arg = .25 * x;    // Argument in Chebyshev expansion is x/4.
	t = 2.0 * (2.0 * arg * arg - 1.0);

	b2 = 0.0;
	b1 = 0.0;
	b0 = a[MAX-1];

	for (k = MAX-2; k >= 0; k--) 
	{
		b2 = b1;
		b1 = b0;
		b0 = t * b1 - b2 + a[k];
	}
	value = .5 *  (b0 - b2);
	return arg * value;
}


function error2(x)   //Calculation of the error function  for x >= 4
{
	var MAX = 14;  // The number of coefficients in a[].

	var a = [ 1.9707052722575449,
		-0.0143397402717750,
		0.0002973616922026,
		-0.0000098035160434,
		0.0000004331334203,
		-0.0000000236215003,
		0.0000000015154968,
		-0.0000000001108494,
		0.0000000000090426,
		-0.0000000000008095,
		0.0000000000000785,
		-0.0000000000000082,
		0.0000000000000009,
		-0.0000000000000001 ];

	var invsqr = .5641895835477563;

	var k;
	var arg, t, value, b0, b1, b2;
	
	arg = 4.0/x;    // Argument in the Chebyshev expansion.
	t = 2.0 * (2.0 * arg * arg - 1.0);
	
	b2 = 0.0;
	b1 = 0.0;
	b0 = a[MAX-1];
	
	for (k = MAX-2; k >= 0; k--) 
	{
		b2 = b1;
		b1 = b0;
		b0 = t * b1 - b2 + a[k];
	}
	value = .5 *  (b0 - b2);
	value = 1.0 - invsqr * Math.exp(-x * x) * value / x;

	return value;
}

//See wikipedia entry of Lanscoz approximation
function gamma( z )
{
	var g = 7;
	var p = [0.99999999999980993, 676.5203681218851, -1259.1392167224028,
	     771.32342877765313, -176.61502916214059, 12.507343278686905,
	     -0.13857109526572012, 9.9843695780195716e-6, 1.5056327351493116e-7];

	// Reflection formula
	if (z < 0.5)
		return Math.PI / (Math.sin(Math.PI*z)*gamma(1-z));
	else
	{
		z -= 1;
		var x = p[0];
		for (var i=1; i<g+2; i++) //i in range(1, g+2):
		    x += p[i]/(z+i);
		var t = z + g + 0.5;
		return Math.sqrt(2*Math.PI) * Math.pow(t,(z+0.5)) * Math.exp(-t) * x;
	}
}

function tdistribution( t, v )
{
	var x = v/(v+Math.pow(t,2));
	var a = v/2;
	var b = 0.5
	var result = incompleteBeta(a,b,x);
	return result; //If values are bad, will return null
}

function incompleteBeta(a, b, x) //Returns the incomplete beta function Ix(a, b).
{
	var bt;
	if (x < 0.0 || x > 1.0) return null;
	if (x == 0.0 || x == 1.0) 
		bt=0.0;
	else //Factors in front of the continued fraction. //bt=exp(gammln(a+b)-gammln(a)-gammln(b)+a*log(x)+b*log(1.0-x));
		bt = (gamma(a+b)*Math.exp(a*Math.log(x))*Math.exp(b*Math.log(1.0-x)))/(gamma(a)*gamma(b));
	
	if (x < (a+1.0)/(a+b+2.0)) //Use continued fraction directly.
		return bt*betacf(a,b,x)/a;
	else //Use continued fraction after making the symreturn
		return 1.0-bt*betacf(b,a,1.0-x)/b; //metry transformation.
}

function betacf( a,  b,  x) // Evaluates continued fraction for incomplete beta function by modified Lentz’s
{
	var MAXIT = 100;
	var EPS = 3.0e-7;
	var FPMIN = 1.0e-30;
		
	var m,m2;
	var aa,c,d,del,h,qab,qam,qap;
	qab=a+b; //These q’s will be used in factors that occur
	qap=a+1.0; //in the coefficients (6.4.6).
	qam=a-1.0;
	c=1.0; //First step of Lentz’s method.
	d=1.0-qab*x/qap;
	if (Math.abs(d) < FPMIN) d=FPMIN;
	d=1.0/d;
	h=d;
	for (m=1;m<=MAXIT;m++) {
		m2=2*m;
		aa=m*(b-m)*x/((qam+m2)*(a+m2));
		d=1.0+aa*d; //One step (the even one) of the recurrence.
		if (Math.abs(d) < FPMIN) d=FPMIN;
		c=1.0+aa/c;
		if (Math.abs(c) < FPMIN) c=FPMIN;
		d=1.0/d;
		h *= d*c;
		aa = -(a+m)*(qab+m)*x/((a+m2)*(qap+m2));
		d=1.0+aa*d; //Next step of the recurrence (the odd one).
		if (Math.abs(d) < FPMIN) d=FPMIN;
		c=1.0+aa/c;
		if (Math.abs(c) < FPMIN) c=FPMIN;
		d=1.0/d;
		del=d*c;
		h *= del;
		if (Math.abs(del-1.0) < EPS) break; //Are we done?
	}
	if (m > MAXIT) return null; //("a or b too big, or MAXIT too small in betacf");
	return h;
}

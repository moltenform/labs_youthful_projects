
function factorial( n )
{
	if (n==0) return 1;
	else if (n==Math.floor(n) && n>0) return Permutation( n,n);
	else if (gamma) return gamma(n+1);
	else return NaN;
}
AvailFunctions["1.0.0"] = "factorial";
factorial.doc = function() { return ShowDoc("1.0.0");}
Docs.Description["1.0.0"] = "Finds factorial of n. If n is not an integer, or n<0, uses gamma function.";

//Different Stats Functions
function permutation( n, r )
{
	if (n<0 || r<0) return NaN;
	if (r>n) return 0;
	var nTotal = 1;
	for (var i = 0; i<r; i++ )
	{
		nTotal *= (n-i);
	}
	return nTotal;
}

function combination( n, r )
{
	if (n<0 || r<0 || n==0 || r>n) return 0;
	var nTotal = 1;
	for (var i = 0; i<r; i++ )
	{
		nTotal *= (n-i);
	}
	return nTotal;

}

function sum( an )
{
	if (!an) return null;
	var nTotal = 0;
	for (var i in an) nTotal += an[i];
	return nTotal;
}

function mean( an )
{
	if (!an) return null;
	return sum(an)/an.length;
}

function median( an )
{



}

function mode( an ) //kind of inefficient
{
	if (!an) return null;
	var oCount = new Object();
	for (var i in an) 
	if (oCount[an[i]]) oCount[an[i]]++;
	else oCount[an[i]] = 1;
	
	oCount["begin"] = -Infinity;
	
	var nWinners = [];
	nWinners[0] = "begin";
	for (var i in oCount)
	{
		if (oCount[i] > oCount[nWinners[0]]) nWinners=[i];
		else if (oCount[i] == oCount[nWinners[0]]) nWinners.push(i);
	}
	return nWinners;
}


function vari(an)
{
	if (!an) return null;
	var nTotal = 0; 
	var nMean = mean(an);
	for (var i in an) nTotal += Math.pow(an[i]-nMean,2);
	
	return nTotal/(an.length-1);
}


function min( an )
{
	if (!an) return null;
	var nMin = Infinity;
	if (isArray(an))
	{
		for (var i in an) if (an[i] < nMin) nMin=an[i];
	}
	else
	{
		for (var i =0; i<arguments.length; i++) if (arguments[i] < nMin) nMin=arguments[i];	
	}
	return nMin;
}
function max( an )
{
	if (!an) return null;
	var nMax = -Infinity;
	if (isArray(an))
	{
		for (var i in an) if (an[i] > nMax) nMax=an[i];
	}
	else
	{
		for (var i =0; i<arguments.length; i++) if (arguments[i] > nMax) nMax=arguments[i];	
	}
	return nMax;
}



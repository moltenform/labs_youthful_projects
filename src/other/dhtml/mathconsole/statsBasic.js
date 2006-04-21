
function Factorial( n )
{
	if (n==0) return 1;
	else if (n==Math.floor(n) && n>0) return Permutation( n,n);
	else if (gamma) return gamma(n+1);
	else return NaN;
}
AvailFunctions["1.0.0"] = "Factorial";
Factorial.doc = function() { return ShowDoc("1.0.0");}
Docs.Description["1.0.0"] = "Finds factorial of n. If n is not an integer, or n<0, uses gamma function.";

//Different Stats Functions
function Permutation( n, r )
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

 
function Combination( n, r )
{
	if (n<0 || r<0 || n==0 || r>n) return 0;
	var nTotal = 1;
	for (var i = 0; i<r; i++ )
	{
		nTotal *= (n-i);
	}
	return nTotal;

}



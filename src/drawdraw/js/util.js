//drawdraw, Ben Fisher, 2010. released under the GPLv3

alerd = function(s)
{
	alert(s)
}

alerdbg = function(s)
{
	alert(debugprint(s))
}

// credit: stackoverflow
function debugprint(obj, maxDepth, prefix)
{
	var result = '';
	if (!prefix)
	{
		prefix='';
	}
	
	for (var key in obj)
	{
		if (typeof obj[key] == 'object')
		{
			if (maxDepth !== undefined && maxDepth <= 1)
			{
				result += (prefix + key + '=object [max depth reached]\n');
			}
			else
			{
				result += debugprint(obj[key], (maxDepth) ? maxDepth - 1: maxDepth, prefix + key + '.');
			}
		}
		else
		{
			result += (prefix + key + '=' + obj[key] + '\n');
		}
	}
	return result;
}

String.prototype.startsWith = function(s)
{
	return (this.indexOf(s)===0);
}

// use "new" to construct an instance.
Time = {};
Time.createTimer = function() 
{
	return new _Timer();
}

_Timer = function() 
{
	this.dt = new Date();
}

_Timer.prototype.check = function()
{
	var n = new Date();
	var nDiff = n.getTime() - this.dt.getTime();
	return nDiff;
}

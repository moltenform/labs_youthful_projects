//Ben Fisher, 2010
//You may not use this source code for other projects without permission from author.
function $(s){return document.getElementById(s); } 

//use a settimeoutloop, so that a plot button is not necessary.
var g_currentPlottedData = '';

function lookForChanges()
{
	var sdata = $('txtInput').value;
	if (sdata!=g_currentPlottedData)
	{
		update(); //replot the figure.
		g_currentPlottedData=sdata;
	}
	setTimeout('lookForChanges()',500);
}

function createDataFromString(sIn)
{
	var ar = sIn.replace(/\r\n/g,'\n').split('\n');
	var data = [];
	
	for (var i=0; i<ar.length; i++)
	{
		var s = ar[i];
		//~ var s = ar[i].replace(/ /g,'').replace(/\t/g,'');
		if (!s) continue;
		//split on comma or on tab
		if (s.indexOf(',')!=-1)
		{
			var spl = s.split(',');
			data.push( {x:parseFloat(spl[0]), y:parseFloat(spl[1])} );
		}
		else if (s.indexOf('\t')!=-1)
		{
			var spl = s.split('\t');
			data.push( {x:parseFloat(spl[0]), y:parseFloat(spl[1])} );
		}
		else
		{
			data.push( {x: i + 0.0, y:parseFloat(s) } );
		}
		
		if (isNaN(data[data.length-1].x)||isNaN(data[data.length-1].y))
			data.pop(); //don't want NaNs
	}
	
	function cmpdata(d1,d2) {return d1.x - d2.x;}
	data.sort(cmpdata);
	return data;
}


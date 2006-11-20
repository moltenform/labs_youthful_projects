
keys = {};

/*availablestyles = {
'control' : true,
'shift': true
};*/

//In the order of control|shift|alt
//Possible combinations:
//normal
//control
//shift
//alt
//controlshift
//controlalt
//altshift
//controlshiftalt


l = 'A'; keys[l] = new Object;
keys[l].normal = {imp:'TYPE'};
keys[l].control = 
{
	brev:"sel all",
	name:"Select all",
	imp:"(Builtin 2013)",
	doc:"Selects all text."
};

l = 'B'; keys[l] = new Object;
keys[l].normal = {imp:'TYPE'};
keys[l].control = 
{
	brev:"expand",
	name:"Expand Abbreviation",
	imp:"(Builtin )",
	doc:"Replaces text with an abbreviation. (Abbreviations are set in the 'abbreviations properties' file)."
};

l = 'C'; keys[l] = new Object;
keys[l].normal = {imp:'TYPE'};
keys[l].control = 
{
	brev:"copy",
	name:"Copy",
	imp:"(Builtin 2178)",
	doc:"Copy the selection to the clipboard."
};

l = 'D'; keys[l] = new Object;
keys[l].normal = {imp:'TYPE'};
keys[l].control = 
{
	brev:"duplicate",
	color:"red",
	name:"Duplicate",
	imp:"(Builtin 2469)",
	doc:"Duplicate the selection. If selection empty, duplicates the line containing the caret."
};

l = 'E'; keys[l] = new Object;
keys[l].normal = {imp:'TYPE'};
keys[l].control = 
{
	brev:"brace",
	name:"Match Brace",
	imp:"(Builtin )",
	doc:"Matches closing or opening brace. {, }, (, )"
};
keys[l].controlshift = 
{
	brev:"brace",
	name:"Select to Brace",
	imp:"(Builtin )",
	doc:"Select to next brace. {, }, (, )"
};

l = 'F'; keys[l] = new Object;
keys[l].normal = {imp:'TYPE'};
keys[l].control = 
{
	brev:"find",
	name:"Find",
	imp:"(Builtin )",
	doc:"Open find dialog."
};
keys[l].controlshift = 
{
	brev:"findfiles",
	name:"Find in Files",
	imp:"(Builtin )",
	doc:"Find a string within files."
};

l = 'G'; keys[l] = new Object;
keys[l].normal = {imp:'TYPE'};
keys[l].control = 
{
	brev:"go to",
	name:"Go to Line",
	imp:"(Builtin)",
	doc:"Go to line in code."
};
keys[l].shiftalt = 
{
	brev:"test",
	name:"Testing",
	imp:"(Builtin)",
	doc:"Go to line in code."
};






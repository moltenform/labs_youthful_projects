function GetPrototype(strDocId)
{
	if (Docs.Prototypes[strDocId]) return Docs.Prototypes[strDocId];
	else if (AvailFunctions[strDocId]) return AvailFunctions[strDocId] + '(x)';
	else return null;
}
function GetProtoExplain(strDocId)
{
	if (Docs.ProtoExplain[strDocId]) return Docs.ProtoExplain[strDocId];
	else return WXIR;
}


function ShowDoc(strDocId)
{
	var BASEIMGPATH = 'img/';
	var strP = unnull( GetPrototype( strDocId ) );
	var strPD = unnull( GetProtoExplain(strDocId) );
	var strD = unnull( Docs.Description[strDocId] );
	var strPic = unnull( Docs.FormulaPics[strDocId] );
	
	var str = '<div class="DocPrototypes">' + strP + '</div>'
		+ ((strPic=='') ? '' : '<img class="DocFormulaPics" src="' + BASEIMGPATH + strPic +'"><br>')
		+ '<div class="DocProtoExplain">' + strPD + '</div>'
		+ '<div class="DocDescription">' + strD + '</div>';
	
	//Change this probably
	divDoc.innerHTML = str;
	
	return strP;
}
//so that there are no ugly "NULL"
function unnull(strIn) { return (strIn) ? strIn : ''; }

//Tutorials
//TutorName[0]='How to define a piecewise function';
//Tutor[0]='Use the ternary operator. m.p=function(x){return (x>3) ? 4 : 6;}';






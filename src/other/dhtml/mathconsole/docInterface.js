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





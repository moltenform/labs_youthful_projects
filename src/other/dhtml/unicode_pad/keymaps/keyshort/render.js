function stripPercent(charin)
{
	if (charin.indexOf('%')!= -1)
	{
		var l = charin.lastIndexOf('%');
		return charin.substring(l+1, charin.length);
	}
	return charin;
}
function entity(charin)
{
	if (charin==' ') return '&nbsp;';
	else if (charin=='&') return "&amp;";
	else if (charin=='<') return "&lt;";
	else if (charin=='>') return "&gt;";
	
	charin = charin.replace(/</g,"&lt;").replace(/>/g,"&gt;");
	if (charin.indexOf('&') != -1 && charin.indexOf(';') == -1)
	{
		// & without ; is most likely a mistake, so fix it.
		return charin.replace('&','&amp;');
	}
	// otherwise, let the entity through.
	return charin;
}
function drawKeyboard(scale, height, type)
{
	//var strNewtable = '<table style="width:500px">'
	var strNewtable = '<table class="keyboard" cellpadding=0 cellspacing=2><tr>'
	var strHtml = strNewtable;
	var ao, nWidth;
	for (var i=0; i<Layout.cellLayout_length; i++)
	{
		ao = Layout.cellLayout[i];
		if (ao[0]===null && ao[1]==-9999)
		{
			strHtml+= '</tr></table>'+strNewtable;
			
		}
		else
		{
			nWidth = Math.round(scale * ao[1]);
			if (Layout.correctAlignment[i]) nWidth +=(Layout.correctAlignment[i][0]-Layout.correctAlignment[i][1])*4;
			strHtml += '<td id="cell' + i + '" '  ;
			strHtml += (ao[0]===null) ? 'class="spacer" ' : 'onclick="clickkey(' + i + ')" ';
			strHtml += '>' + ((ao[0]===null) ? '&nbsp;' : entity(stripPercent(ao[0])));
			//strHtml += '<div class="cmd" id="cmd' + i + '"  style="width:' + (nWidth-30) + 'px">&nbsp;</div></td>\r\n';
			strHtml += '<div class="cmd" id="cmd' + i + '" style="width:' + nWidth + 'px;" >&nbsp;</div></td>\r\n';
		}
	}
	strHtml+='</tr></table>';
	divout.innerHTML= strHtml;
	
	populate('normal');
}
function populate(style)
{
	//style should be "normal" or "control"
	var o;
	for (var i=0; i<Layout.cellLayout_length; i++)
	{
		ao = Layout.cellLayout[i];
		if (ao[0]===null)
			continue;
		
		//o = document.getElementById('cmd'+i);
		//	o.innerText = 'h';
		o = document.getElementById('cmd'+i);
		if (keys[ ao[0] ] && keys[ ao[0] ][style] && keys[ ao[0] ][style].brev)
		{
			var str = entity( keys[ ao[0] ][style].brev );
			var name= keys[ ao[0] ][style].name;
			if (keys[ ao[0] ][style].color)
			{
				str = '<span style="color:'+keys[ ao[0] ][style].color+'">'+str+'</span>';
			}
			o.innerHTML = str;
			o.title = name;
		}
		else
		{
			o.innerHTML = '';
			o.title = '';
		}
	}
}
function getdoc(style, nK)
{
	ao = Layout.cellLayout[nK];
	if (ao===undefined||ao[0]===undefined)
		return null;
	if (keys[ ao[0] ] && keys[ ao[0] ][style] && keys[ ao[0] ][style].brev)
	{
		return [ keys[ ao[0] ][style].name, keys[ ao[0] ][style].imp, keys[ ao[0] ][style].doc];
	}
	else
		return null;
}
function getModifierString(bCtrl, bShift, bAlt)
{
	var strstyle = '';
	if (bCtrl) strstyle += 'control';
	if (bShift) strstyle += 'shift';
	if (bAlt) strstyle += 'alt';
	if (strstyle == '') strstyle = 'normal';
	return strstyle;
}
function setModifiers(bCtrl, bShift, bAlt)
{
	var strstyle = getModifierString(bCtrl, bShift, bAlt);
	if (strstyle != modifierstyle)
	{
		populate(strstyle);
		modifierstyle = strstyle;
		//out.value = strstyle + "\r\n" + Math.random();
	}
}
/*function getModifierStringRendered(bCtrl, bShift, bAlt)
{
	var astyle = [];
	if (bCtrl) astyle[astyle.length] = ( 'Ctrl' );
	if (bShift) astyle[astyle.length]  = ( 'Shift' );
	if (bAlt) astyle[astyle.length] = ( 'Alt' );
	if (!astyle.length) return '';
	else return astyle.join('+') + '+';
}*/
function getModifierStringRendered(strModifiers)
{
	if (strModifiers == 'normal') return '';
	return  strModifiers.replace('control', 'Ctrl+').replace('alt', 'Alt+').replace('shift','Shift+');
}

//http://translate.google.com/translate?sourceid=navclient&hl=en&u=http%3a%2f%2fwww%2ebrain4%2ede%2fprogrammierecke%2fjs%2ftastatur%2ephp%23start

//var eventmapkeyup = { 44:14 }; Print screen is only called on keyup
var eventmapkeydown = {
27:0, 112: 1, 113:2, 114:3, 115:4,116:5, 117:6, 118:7, 119:8, 120:9, 121:10, 122:11, 123:12,  /*esc + fn keys*/
145:15, 19:16, /*scroll lock etc*/
192:18, 49:19, 50:20, 51:21, 52:22,53:23,54:24,55:25,56:26,57:27,48:28, /*top row numbers*/
189:29, 187:30, 8:31,45:32,36:33,33:34,144:35,111:36,106:37,109:38, /*Insert, etc*/
9:40,81:41,87:42,69:43,82:44,84:45,89:46,85:47,73:48,79:49,80:50, /*Letters Q-P*/
219:51,221:52,220:53,46:54,35:55,34:56,103:57,104:58,105:59,107:60, /*{ to +*/
20:62,65:63,83:64,68:65,70:66,71:67,72:68,74:69,75:70,76:71,186:72,222:73,13:74, /*Letters A-L*/
100:76, 101:77, 102:78, /*keypad 4-6*/
90:81,88:82,67:83,86:84,66:85,78:86,77:87,188:88,190:89,191:90, /*Letters z-m*/
38:93,97:95,98:96,99:97,13:98, /*keypad 1-3*/
32:103, 93:106, 37:108, 40:109, 39:110, 96:111, 110:112 /*Bottom row*/
};
//Modifiers done seperately!

var keyshifts = 
{
18:'~',19:'!',20:'@',21:'#',22:'$',23:'%',24:'^',25:'&', 26:'*', 27:'(', 28:')',29:'_', 30:'+'

};
var keymodifiers =
{
17:['control',100,107], 16:['shift',80,91], 18:['alt',102,104]
};

//Render name | shift name | keywidth | handle onkeydown?
keymap = {
0: ['Esc',100],
1:['F1',100],
2:['F2',100],
3:['F3',100],
4:['F4',100],
5:['F5',100],
6:['F6',100],
7:['F7',100],
8:['F8',100],
9:['F9',100],
10:['F10',100],
11:['F11',100],
12:['F12',100],
13:[null,500],
14:['PntScr',100],
15:['ScrLck',100],
16:['Break',100],
17:[null,-9999], /*New line*/

18:['`',100],
19:['1',100],
20:['2',100],
21:['3',100],
22:['4',100],
23:['5',100],
24:['6',100],
25:['7',100],
26:['8',100],
27:['9',100],
28:['0',100],
29:['-',100],
30:['=',100],
31:['BckSpc',200],/*1500*/
32:['Ins',100],
33:['Home',100],
34:['PgUp',100],
35:['NL',100],
36:['%Pad%/',100],
37:['%Pad%*',100],
38:['%Pad%-',100],
39:[null,-9999],

40:['Tab',160],
41:['Q',100],
42:['W',100],
43:['E',100],
44:['R',100],
45:['T',100],
46:['Y',100],
47:['U',100],
48:['I',100],
49:['O',100],
50:['P',100],
51:['[',100],
52:[']',100],
53:['\\',140],/*1500*/
54:['Del',100],
55:['End',100],
56:['PgDn',100],
57:['%Pad%7',100],
58:['%Pad%8',100],
59:['%Pad%9',100],
60:['%Pad%+',100],
61:[null,-9999],


62:['Caps',180],
63:['A',100],
64:['S',100],
65:['D',100],
66:['F',100],
67:['G',100],
68:['H',100],
69:['J',100],
70:['K',100],
71:['L',100],
72:[':',100],
73:["'",100],
74:['Enter',220 ],
75:[null,300],
76:['%Pad%4',100],
77:['%Pad%5',100],
78:['%Pad%6',200],
79:[null,-9999],

80:['Shift',250],
81:['Z',100],
82:['X',100],
83:['C',100],
84:['V',100],
85:['B',100],
86:['N',100],
87:['M',100],
88:[',',100],
89:['.',100],
90:['/',100],
91:['Shift',250],
92:[null,100],
93:['Up',100],
94:[null,100],
95:['%Pad%1',100],
96:['%Pad%2',100],
97:['%Pad%3',100],
98:['%Pad%Enter',100],
99:[null,-9999],

100:['Ctrl',140],
101:['Win',140],
102:['Alt',140],
103:[' ',520],
104:['Alt',140],
105:['Win',140],
106:['*',140],
107:['Ctrl',140],
108:['Left',100],
109:['Down',100],
110:['Right',100],
111:['%Pad%0',200],
112:['%Pad%.',100]



};
margshift ={}; margshift[31]=14; margshift[53]=14; margshift[74]=13; margshift[91]=12; margshift[107]=8;

var keymap_length =113;

function clickKey(nKey)
{
	var keyname = keymap[nKey][0];
	alert ('you clicked on a '+keyname);
}
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
else return charin.replace('&','&amp;').replace('<',"&lt;").replace('>',"&gt;");
}
function drawKeyboard(scale, height, type)
{
	//var strNewtable = '<table style="width:500px">'
	var strNewtable = '<table class="keyboard" cellpadding=0 cellspacing=2><tr>'
	var strHtml = strNewtable;
	var ao, nWidth;
	for (var i=0; i<keymap_length; i++)
	{
		ao = keymap[i];
		if (ao[0]===null && ao[1]==-9999)
		{
			strHtml+= '</tr></table>'+strNewtable;
			
		}
		else
		{
			nWidth = Math.round(scale * ao[1]);
			if (margshift[i]) nWidth +=(14-margshift[i])*4;
			strHtml += '<td id="cell' + i + '" '  ;
			strHtml += (ao[0]===null) ? 'class="spacer" ' : 'onclick="presskey(' + i + ')" ';
			strHtml += '>' + ((ao[0]===null) ? '&nbsp;' : entity(stripPercent(ao[0])));
			//strHtml += '<div class="cmd" id="cmd' + i + '"  style="width:' + (nWidth-30) + 'px">&nbsp;</div></td>\r\n';
			strHtml += '<div class="cmd" id="cmd' + i + '" style="width:' + nWidth + 'px;" >&nbsp;</div></td>\r\n';
		}
	}
	strHtml+='</tr></table>';
	divout.innerHTML= strHtml;
	
	populate('normal');
	//out.value= strHtml;
}
function populate(style)
{
	//style should be "normal" or "control"
	var o;
	for (var i=0; i<keymap_length; i++)
	{
		ao = keymap[i];
		if (ao[0]===null)
			continue;
		
		//o = document.getElementById('cmd'+i);
		//	o.innerText = 'h';
		o = document.getElementById('cmd'+i);
		if (keys[ ao[0] ] && keys[ ao[0] ][style] && keys[ ao[0] ][style].brev)
		{
			var str = entity( keys[ ao[0] ][style].brev );
			if (keys[ ao[0] ][style].color)
			{
				str = '<span style="color:'+keys[ ao[0] ][style].color+'">'+str+'</span>';
			}
			o.innerHTML = str;
				
		}
		else
		{
			o.innerHTML = '';
			
		}
	}
}
function getdoc(style, nK)
{
	ao = keymap[nK];
	if (ao===undefined||ao[0]===undefined)
		return null;
	if (keys[ ao[0] ] && keys[ ao[0] ][style] && keys[ ao[0] ][style].brev)
	{
		return [ keys[ ao[0] ][style].name, keys[ ao[0] ][style].imp, keys[ ao[0] ][style].doc];
	}
	else
		return null;
}
function getstyle(bCtrl, bShift, bAlt)
{
	var strstyle = '';
	if (bCtrl) strstyle += 'control';
	if (bShift) strstyle += 'shift';
	if (bAlt) strstyle += 'alt';
	if (strstyle == '') strstyle = 'normal';
	return strstyle;
}
function setstyle(bCtrl, bShift, bAlt)
{
	var strstyle = getstyle(bCtrl, bShift, bAlt);
	if (strstyle != modifierstyle)
	{
		populate(strstyle);
		modifierstyle = strstyle;
		//out.value = strstyle + "\r\n" + Math.random();
	}
}
function renderstyle(bCtrl, bShift, bAlt)
{
	var astyle = [];
	if (bCtrl) astyle .push( 'Ctrl' );
	if (bShift) astyle .push( 'Shift' );
	if (bAlt) astyle .push( 'Alt' );
	if (!astyle.length) return '';
	else return astyle.join('+') + '+';
}

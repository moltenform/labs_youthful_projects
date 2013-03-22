// htmlfreq beatcirle, Ben Fisher, 2011
// http://halfhourhacks.blogspot.com
// GPL v3 license, see http://www.gnu.org/licenses/gpl.txt for the terms of using this code
// This is a written-on-the-bus spare-time project, production code it ain't!


var Ra = null; //main raphael instance
var g_sampleRate = 44100;
var WIDTH=800
var HEIGHT=600
var g_beatObject = null;



function connect()
{
	
}

var fndrawbuttons = null;

function onbtnmousedown()
{
	//~ alert(this.ben_chn + ','+this.ben_i)
	var chn=this.ben_chn
	var division=this.ben_i
	if (g_beatObject.channels[chn].arrHits[division])
	{
		g_beatObject.channels[chn].arrHits[division] = 0
		this.attr({'fill':'#ddd'})
	}
	else
	{
		g_beatObject.channels[chn].arrHits[division] = 1
		this.attr({'fill':'#000'})
	}
	
	g_currentAudioMap=createAudioMap(g_beatObject) //refresh the map!
}

var g_ui_buttons = []
var g_ui_buttonconnect = []
function setup()
{
	var g_mediaNames = ["kick",get909bass ,
"snr1",get909snare , 
"snr2",get606snare , 
"snr3",get808snare , 
"clap",get909clap , 
"hihat",get909closed, 
"hihat2",get909pedal , 
"beep",getbip , 
"jungle",getjunglesnare]
	var arPatches =  ['off']
	
	
	getmedia()
	//~ setTimeout(getmedia, 20)
	
	for (var j=0; j<g_mediaNames.length; j+=2)
		arPatches.push(g_mediaNames[j])
	
	g_beatObject = new CBeatTop()
	
	g_beatObject.channels[0].rotation=0.2
	g_beatObject.channels[1].rotation=0.2
	g_beatObject.channels[2].rotation=0.2
	g_beatObject.channels[3].rotation=0.2
	
	g_beatObject.channels[0].samplename='kick'; g_beatObject.channels[0].samplename_f=1
	g_beatObject.channels[1].samplename='clap'; g_beatObject.channels[1].samplename_f=2
	g_beatObject.channels[2].samplename='snr1'; g_beatObject.channels[2].samplename_f=4
	g_beatObject.channels[3].samplename='hihat'; g_beatObject.channels[3].samplename_f=3
	
	for (var i=0; i<64; i+=4)
		g_beatObject.channels[0].arrHits[i] = 1
	
	for (var i=0; i<63; i+=8)
		g_beatObject.channels[1].arrHits[i+4] = 1
	
	// set up g_requestSoundChannelState
	for (var i=0; i<g_beatObject.channels.length;i++)
		g_requestSoundChannelState[i] = {pos:0, audio:getNone}
	
	
	g_currentAudioMap=createAudioMap(g_beatObject)
	
	//~ if (document.location.href.indexOf('?')!=-1)
	//~ {
		//~ var retState = onLoadFromJson(document.location.href.substring(document.location.href.indexOf('?')+1))
		//~ if (retState)  g_beatObject = retState
	//~ }
	
	Ra = Raphael("holder", WIDTH, HEIGHT);
	
	var cx=430, cy=230, wx=140, wy=140;
	var colors = ['#999', '#00f','#0f0','#f00', '#ff0']
	for (var i=colors.length-1; i>=0; i--)
	{
		Ra.ellipse(cx,cy,wx+16*i,wy+16*i).attr({"stroke-width": 1, 'fill':colors[i]});
		if (i!=0)
		{
			var nChannel = i
		}
	}
	
	//draw buttons later so above stripes
	var maxbuttons = 128
	for (var chn =0; chn<g_beatObject.channels.length; chn++)
	{
		g_ui_buttons[chn] = []
		for (var i=0; i<maxbuttons; i++)
		{
			g_ui_buttons[chn][i] = Ra.ellipse(-6,-4,4,4)
			g_ui_buttons[chn][i].hide()
			g_ui_buttons[chn][i].ben_chn = chn
			g_ui_buttons[chn][i].ben_i = i
			g_ui_buttons[chn][i].mousedown(onbtnmousedown)
		}
	}
	for (var chn =0; chn<g_beatObject.channels.length; chn++)
	{
		g_ui_buttonconnect[chn] = []
		for (var i=0; i<maxbuttons / 4; i++)
		{
			g_ui_buttonconnect[chn][i] = Ra.path('M1,1,L,5,5' )
			g_ui_buttonconnect[chn][i].hide()
		}
	}
	
	
	
	//~ $('ui_label_znchanwavetype'+sx).raphael.remove()
	
	//~ for (var x=0; x<g_beatObject.channels.length; x++)
	//~ {
		//~ var cdzone = makesliderzone('znmodtype'+sx+','+sy, g_audioState.channels[x].modifiers[y], 'modtype_f', 40+300*x, 155+60*y, 120/2, 0.0, 1.0 /*unused*/)
		//~ $('ui_label_znmodtype'+sx+','+sy).raphael.remove()
		//~ addEnumeratedType(cdzone, arModTypes, 'modtype', -20)
	//~ }
	
	
	
	fndrawbuttons = function()
	{
		if (g_beatObject.divisions > maxbuttons) {errmsg('too many divisions'); return;}
		for (var chn=0; chn<g_beatObject.channels.length; chn++)
		{
			for (var j=0; j<g_beatObject.divisions; j++)
			{
				var angl= Math.PI*2*(j/(g_beatObject.divisions+0.0))
				angl += g_beatObject.channels[chn].rotation
				var m = wx +16*(1+chn) - 4
				var ccx=Math.cos(angl)*m+cx
				var ccy=Math.sin(angl)*m+cy
				var thefill = g_beatObject.channels[chn].arrHits[j] ? '#000' : '#ddd'
				g_ui_buttons[chn][j].attr({cx:ccx, cy:ccy, fill:thefill})
				g_ui_buttons[chn][j].show()
				
				if (j%16==0)
				{
					thefill = (j%16==0) ? '#666' : '#999'
					var x1=Math.cos(angl)*(m-12)+cx,y1=Math.sin(angl)*(m-12)+cy,
						x2=Math.cos(angl)*(m-4)+cx,y2=Math.sin(angl)*(m-4)+cy;
					g_ui_buttonconnect[chn][j/4].attr({path:'M'+x1+','+y1+',L,'+x2+','+y2, fill:thefill})
					g_ui_buttonconnect[chn][j/4].show()
				}
			}
			for (var j=g_beatObject.divisions;j<maxbuttons; j++)
			{
				g_ui_buttons[chn][j].hide()
				if (j%4==0)
					g_ui_buttonconnect[chn][j/4].hide()
			}
		}
	}
	
	fndrawbuttons()
	
	

	for (var chn =0; chn<g_beatObject.channels.length; chn++)
	{
		var xbase = 70+chn * 200
		var ybase = 500
		var cdzone = null
		
		var px1=xbase,px2=xbase+80,py1=ybase,py2=ybase;
		Ra.path('M'+px1+','+py1+',L,'+px2+','+py2).attr({"stroke-width": 4, 'stroke':colors[chn+1]});
		
		ybase += 20
		cdzone = makesliderzone('znvol'+chn, g_beatObject.channels[chn], 'vol', xbase, ybase, 80, 0.0, 1.0)
		
		ybase += 20
		cdzone = makesliderzone('znsmpl'+chn, g_beatObject.channels[chn], 'samplename_f', xbase, ybase, 80, 0.0, 1.0 /*unused*/)
		$('ui_label_znsmpl'+chn).raphael.remove()
		addEnumeratedType(cdzone, arPatches, 'samplename', -50)
		cdzone.tmp_prevfn=cdzone.onValueChanged;
		cdzone.onValueChanged = function(fx,fy)
		{
			this.tmp_prevfn(fx,fy)
			setuprequestSoundChannelState(g_beatObject)
		}
		
		ybase += 20
		cdzone = makesliderzone('znrotate'+chn, g_beatObject.channels[chn], 'rotation', xbase, ybase, 80, 0.0, 2*Math.PI / 4)
		cdzone.tmp_whichchn=chn
		cdzone.onValueChanged = function(fx,fy)
		{
			g_currentAudioMap=createAudioMap(g_beatObject)
			fndrawbuttons()
			//might be more efficient to have one map per channel, so only have to recalc one map, but whatever
		}
		
		
		if (chn==0)
		{
			ybase +=20
			cdzone = makesliderzone('zntempo', g_beatObject, 'tempo', xbase,ybase, 80, 0, 30)
			cdzone.onValueChanged = function(fx,fy)
			{
				g_currentAudioMap=createAudioMap(g_beatObject)
			}
		}
	} 
	
	cd2_setselect(null) //no current selection
	
	
	// start audio !
	var audioDestination = new AudioDataDestination(g_sampleRate, requestSoundData);
	
	
}


function toggleVis(bVisible)
{
	
}


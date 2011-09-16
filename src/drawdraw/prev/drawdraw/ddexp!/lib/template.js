
// may be useful:
// g_slidestate.c1, g_slidestate.c2
// clearshapes, drawline, drawpoint

function onuserload()
{
	drawline(0.5, 0.5, 0.75, 0.75)
	print('generating')
}

function onuserkeycode(nkeycode)
{
	var paramB = 2;
	if (nkeycode==107) //+
		paramB += 1.0;
	else if (nkeycode==109) //-
		paramB -= 1.0;
	else
		return;
	alert('value of paramb is now '+paramB)
}

function onuserslidermove()
{
	print('moved')
}
function onuserslidermovelarge()
{
	print('movedlarge')
}



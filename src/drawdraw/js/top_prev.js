



function saveToJson()
{
    if (!JSON)
    {
        alerd('Could not find JSON object. Try using latest Firefox or Chrome.');
        return;
    }
    
    var arRes = []
    for (var objId in g_ui.domToCoordObject)
    {
        // first, strip out functions
        var newobj = {}
        for (var key in g_ui.domToCoordObject[objId])
        {
            if (typeof g_ui.domToCoordObject[objId][key] != 'function')
            {
                newobj[key] = g_ui.domToCoordObject[objId][key]
            }
        }
        
        arRes.push(newobj)
    }
    
    // in the future, it might be interesting to save the basis g_ui.mainContextShape.
    // for now we don't need it because it doesn't change. and we don't want to add extra characters
    // same with the zoomlevel... fewer characters the better
    var objTop = {vn:'0.1', nshapes:g_classesglobal.nShapesToDraw, pm:g_classesglobal.nJustPerimeter, shapes:arRes}
    
    var s= JSON.stringify(objTop)
    s = s.replace(/"/g,'')
    s = s.replace(/x1/g, 'X').replace(/x2/g, 'A').replace(/y1/g, 'Y').replace(/y2/g, 'B')
    s = s.replace(/:/g, '=')
    return s
}

function onSave()
{
    var s = saveToJson()
    var fsturl = document.location.href.split('?')[0];
    prompt('share this link!', fsturl + '?' + s)
}

function onloadJson()
{
    var sOldJson = saveToJson()
    var openD = prompt('Save diagram by copying string into text document. Load new diagram by pasting new string here (no newlines):', sOldJson)
    if (openD && openD!= sOldJson && confirm('delete existing and load new?'))
    {
        loadJson(openD)
    }
}

function loadJson(s)
{
    // we'll use a few search/replace tricks to "condense" the json.
    // for example:
    // ab{1{a {b .A. anApple banA
    
    s = s.replace(/=/g, ':')
    s = s.replace(/({)([a-zA-Z])/g, '$1"$2')
    s = s.replace(/(:)([a-zA-Z])/g, '$1"$2')
    s = s.replace(/(,)([a-zA-Z])/g, '$1"$2')
    s = s.replace(/([a-zA-Z])(,)/g, '$1"$2')
    s = s.replace(/([a-zA-Z])(:)/g, '$1"$2')
    s = s.replace(/([a-zA-Z])(})/g, '$1"$2')
    s = s.replace(/\bA\b/g, 'x2').replace(/\bB\b/g, 'y2').replace(/\bX\b/g, 'x1').replace(/\bY\b/g, 'y1')
    
    try
    {
        var obj = JSON.parse(s)
    }
    catch(err)
    {
        alerd('could not parse')
        return;
    }
    
    if (!obj['shapes'])
    {
        alerd('Could not find shapes.');
        return;
    }
    
    var version = obj['vn']
    if (obj['nshapes'])
    {
        g_classesglobal.nShapesToDraw = parseInt(obj['nshapes'])
    }
    
    if (obj['pm'])
    {
        g_classesglobal.nJustPerimeter = parseInt(obj['pm'])
    }
    
    var shapes = obj['shapes']
    for (var i = 0; i < shapes.length; i++)
    {
        var oCoord = new CRawShape(shapes[i])
        createNew(oCoord.type)
        g_ui.domToCoordObject[g_ui.domSelected.id] = oCoord
        refreshShape(g_ui.domSelected)
    }
    
    g_ui.domSelected = null;
    hideSelect();
    doTransformRender();
}






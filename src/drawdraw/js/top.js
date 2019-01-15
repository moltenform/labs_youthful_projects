
/**
 * Ben Fisher, 2010
 * @license GNU General Public License version 3
 * https://www.gnu.org/licenses/gpl-3.0.txt
 * https://github.com/moltenjs/labs_youthful_projects
 */

var Ra = null; //main raphael instance
var g_state = { inited: false }
var g_ui = { inited: false }

function doesHaveJson() {
    if (document.location.href.indexOf('#') != -1) {
        var sJson = document.location.href.substring(document.location.href.indexOf('#') + 1)
        if (sJson && sJson.length > 0 && !sJson.startsWith('blank')) {
            sJson = decompressLz(sJson)
            sJson = myDecompress(sJson)
            return JSON.parse(sJson)
        }
    }

    return undefined
}

g_state.init = function (self) {
    if (self.inited) {
        return
    }

    self.shouldDrawReferenceContext = false // we used to draw a "reference" marker. it wasn't a selectable object.
    self.debugMeasureTiming = false;
    
     // zoom, default to 1
    self.zoomLevel = 1
     // number of shapes to draw, can be adjusted by user
    self.nShapesToDraw = 300
     // this option can be enabled by the user, where we'll draw just the perimeter.
    self.nJustPerimeter = false

    self.inited = true
}

g_ui.init = function (self) {
    if (self.inited) {
        return
    }

    Ra = Raphael("holder", g_ui.master_w, g_ui.master_h);

    self.domSelected = undefined
    self.isRendering = false;

    // record what shapes have been created.
    self.domToCoordObject = {}

    // the arrow-head lines for lgen shapes
    self.mapObjIdToArrow = {};

    // compensate for devicePixelRatio
    self.resizeFactor = 1;

    // holding dom objects is not ideal... we're past the the days of ie memory leaks though.
    self.shapeSelectA = null;
    self.shapeSelectB = null;

    self.allLines = [];
    self.initPools(self)
    
    // adjust selection handle size on screens with high dpi
    self.resizeFactor = 1
    if (window.devicePixelRatio !== undefined) {
        self.resizeFactor *= window.devicePixelRatio
    }

    if (window.devicePixelRatio !== undefined && window.devicePixelRatio > 1) {
        // consider multiplying self.resizeFactor here
    }
    
    self.oneLineGraphic = Ra.path('M1,1,L,1,1');

    // draw selection handles
    var handlesize = 4 * self.resizeFactor
    self.shapeSelectA = Ra.ellipse(1, 1, handlesize, handlesize).attr({ "stroke-width": 1 });
    self.shapeSelectB = Ra.ellipse(1, 1, handlesize, handlesize).attr({ "stroke-width": 1 });
    self.shapeSelectA.attr({ fill: "#0f0", opacity: 1 });
    self.shapeSelectB.attr({ fill: "#0f0", opacity: 1 });
    self.shapeSelectA.drag(onDragResize_move, onDragResize_start, onDragResize_up);
    self.shapeSelectB.drag(onDragResize_move, onDragResize_start, onDragResize_up);

    if (self.resizeFactor > 1) {
        g_ui.mainContextShape = new CRawShape({ type: 'lgen', x1: 40, x2: 40, y1: 140, y2: 40 });
    } else {
        g_ui.mainContextShape = new CRawShape({ type: 'lgen', x1: 200, x2: 200, y1: 200, y2: 100 });
    }

    g_ui.mainContext = contextFromRawShape(g_ui.mainContextShape);
    var mainGenPath = Ra.path('M1,1,L,1,1').attr({ 'stroke-width': 6 * self.resizeFactor })
    updatePath(mainGenPath, g_ui.mainContextShape)
    mainGenPath.attr({ stroke: '#888' });

    if (g_state.shouldDrawReferenceContext) {
        // manually draw the main arrow reference. this is not adjustable by the user.
        var mainArrow = drawArrow(mainGenPath, g_ui.mainContextShape, true)
        mainArrow.attr({ stroke: '#aaf', fill: '#aaf' });
    }

    self.wasEverInited = true
    self.inited = true
}

g_ui.initPools = function (self) {
    var poolCirclesCreate = function() {
        return Ra.circle(1, 1, 1)
    }

    var poolCirclesReset = function(obj) {
        obj.hide()
    }

    self.poolCircles = new CResourcePool(poolCirclesCreate, poolCirclesReset)
}

g_ui.teardown = function (self) {
    self.poolCircles.clearAll()
    self.shapeSelectA.hide()
    self.shapeSelectB.hide()
}

function loadDefaultDoc() {
    // create a normal line, as an example
    createNew('l');
    var oCoord = g_ui.domToCoordObject[g_ui.domSelected.id];
    oCoord.x1 = g_ui.mainContextShape.x1;
    oCoord.y1 = g_ui.mainContextShape.y1;
    oCoord.x2 = g_ui.mainContextShape.x2;
    oCoord.y2 = g_ui.mainContextShape.y2;
    refreshShape(g_ui.domSelected);

    // create a normal lgen, as an example
    createNew('lgen');
    oCoord = g_ui.domToCoordObject[g_ui.domSelected.id];
    oCoord.x1 = g_ui.mainContextShape.x1;
    oCoord.y1 = g_ui.mainContextShape.y1;
    oCoord.x2 = g_ui.mainContextShape.x2 + 40;
    oCoord.y2 = g_ui.mainContextShape.y2 + 50;
    refreshShape(g_ui.domSelected);
}

function on_locationhashchange() {
    // reset the entire ui
    g_ui.teardown(g_ui)
    Ra = null; //main raphael instance
    $('holder').innerHTML = ''
    g_state.inited = false
    g_ui.inited = false
    initAll()
}

function initAll() {
    if (!g_ui.wasEverInited) {
        var whenBubbling = false
        window.addEventListener("hashchange", on_locationhashchange, false, whenBubbling);
        
        $("idbtnaddline").addEventListener("click", on_btnaddline, whenBubbling)
        $("idbtnaddcircle").addEventListener("click", on_btnaddcircle, whenBubbling)
        $("idbtnaddgen").addEventListener("click", on_btnaddgen, whenBubbling)
        $("idbtntheta").addEventListener("click", on_btntheta, whenBubbling)
        $("idbtndelete").addEventListener("click", on_btndelete, whenBubbling)
        $("idbtnonlyperim").addEventListener("click", on_btnonlyperim, whenBubbling)
        $("idbtndrawmore").addEventListener("click", on_btndrawmore, whenBubbling)
        $("idbtndrawless").addEventListener("click", on_btndrawless, whenBubbling)
        $("idbtnzoomin").addEventListener("click", on_btnzoomin, whenBubbling)
        $("idbtnzoomout").addEventListener("click", on_btnzoomout, whenBubbling)
        $("idbtnopenexample").addEventListener("click", on_btnopenexample, whenBubbling)
        $("idbtnsave").addEventListener("click", on_btnsave, whenBubbling)
        
        var w = Math.max(document.documentElement.clientWidth, window.innerWidth || 0)
        var h = Math.max(document.documentElement.clientHeight, window.innerHeight || 0)
        $("holder").style.width = (w - 80) + 'px'
        $("holder").style.height = (h - 50) + 'px'
        $("copy").style.top = (h - 30) + 'px'
        $("copy").style.left = (w - 350) + 'px'
        $("copy").style.width = (300) + 'px'
        $("copy").style.display = 'block'
        g_ui.master_w = w
        g_ui.master_h = h
    }

    g_state.init(g_state)
    g_ui.init(g_ui)
    var jsonRawObj = doesHaveJson()
    if (jsonRawObj) {
        onLoadFromJsonRawObj(jsonRawObj)
    } else {
        loadDefaultDoc()
    }

    showSelect();
}

function refreshShape(domObj) {
    var oCoord = g_ui.domToCoordObject[domObj.id];
    updatePath(domObj, oCoord);

    if (oCoord.type == 'lgen') {
        drawArrow(domObj, oCoord, false)
    }

    if (g_ui.domSelected) {
        showSelect()
    }
}

function createNew(stype) {
    if (stype == 'l' || stype == 'lgen') {
        var newEntity = Ra.path('M1,1,L,1,1');
        var newRawShape = new CRawShape({
            type: 'l', x1: g_ui.mainContextShape.x2,
            y1: g_ui.mainContextShape.y2, x2: g_ui.mainContextShape.x2 + 30, y2: g_ui.mainContextShape.y2
        });

        newRawShape.type = stype;
        if (stype == 'lgen') {
            newEntity.attr({ stroke: '#922', opacity: 1.0 })
        }
    } else if (stype == 'c') {
        var newEntity = Ra.circle(1, 1, 1)
        var newRawShape = new CRawShape({ type: 'c', x1: g_ui.mainContextShape.x2, y1: g_ui.mainContextShape.y2, rx: 20 });
    }

    g_ui.allLines.push(newEntity)
    g_ui.domToCoordObject[newEntity.id] = newRawShape;

    // draw the shape at its initial position
    refreshShape(newEntity);
    newEntity.mousedown(onMouseDownSelectIt);
    newEntity.attr({ "stroke-width": 4 * self.resizeFactor });
    g_ui.domSelected = newEntity;
    showSelect();

    // raphael.js workaround for rendering bug in Safari.
    Ra.safari();
    return newEntity;
}

function showSelect() {
    g_ui.shapeSelectA.show();
    g_ui.shapeSelectB.show();
    var oCoord = g_ui.domToCoordObject[g_ui.domSelected.id];
    if (!oCoord) {
        errmsg('in showSelect, nothing selected?');
        return;
    }

    if (oCoord.type.startsWith('l')) {
        g_ui.shapeSelectA.attr({ cx: oCoord.x1, cy: oCoord.y1 })
        g_ui.shapeSelectB.attr({ cx: oCoord.x2, cy: oCoord.y2 })
    } else if (oCoord.type == 'c') {
        g_ui.shapeSelectA.attr({ cx: oCoord.x1, cy: oCoord.y1 })
        g_ui.shapeSelectB.attr({ cx: oCoord.x1 + oCoord.rx, cy: oCoord.y1 })
    }

    // g_ui.shapeSelectB should be in front, so that circles aren't stuck small
    g_ui.shapeSelectA.toFront();
    g_ui.shapeSelectB.toFront();

    // raphael.js workaround for rendering bug in Safari.
    Ra.safari();
}

function hideSelect() {
    g_ui.shapeSelectA.hide();
    g_ui.shapeSelectB.hide();
}

function deleteSelected() {
    if (!g_ui.domSelected) {
        return;
    }

    if (g_ui.domToCoordObject[g_ui.domSelected.id].type == 'lgen') {
        g_ui.mapObjIdToArrow[g_ui.domSelected.id].remove() // remove arrow
        delete g_ui.mapObjIdToArrow[g_ui.domSelected.id];
    }

    delete g_ui.domToCoordObject[g_ui.domSelected.id];
    hideSelect();
    g_ui.domSelected.remove();
    g_ui.domSelected = null;
}

function invisSelected() {
    if (g_ui.domSelected) {
        g_ui.domToCoordObject[g_ui.domSelected.id].visible = !g_ui.domToCoordObject[g_ui.domSelected.id].visible;
        refreshShape(g_ui.domSelected)
    }
}

function onMouseDownSelectIt(event) {
    if (this) {
        g_ui.domSelected = this;
        showSelect();
    }
}

function doTransformRender() {
    // prevent reentrance
    if (g_ui.isRendering) {
        return;
    }

    g_ui.isRendering = true;

    var gens = [];
    var objs = [];
    for (var objId in g_ui.domToCoordObject) {
        if (g_ui.domToCoordObject[objId].type == 'lgen') {
            gens.push(g_ui.domToCoordObject[objId])
        } else if (g_ui.domToCoordObject[objId].x1 !== undefined) {
            objs.push(g_ui.domToCoordObject[objId])
        } else {
            errmsg('warning: something else in map')
        }
    }

    //make main reference into a context
    var initialContext = g_ui.mainContext;

    //convert shapes to be relative to initial context
    for (var i = 0; i < gens.length; i++) {
        gens[i] = rawShapeToRelativeShape(initialContext, gens[i])
    }

    for (var i = 0; i < objs.length; i++) {
        objs[i] = rawShapeToRelativeShape(initialContext, objs[i])
    }

    var contextQueue = [initialContext]
    var nAdjustX = (g_ui.resizeFactor == 1) ? 300 : 80;

    if (g_state.debugMeasureTiming) {
        oTimer = Time.createTimer()
    }

    // it's a bit unclean that we do both the computation and the ui here
    transform(contextQueue, objs, gens, -1/*nThresholdBeforeDraw*/, g_state.nShapesToDraw, nAdjustX);

    if (g_state.debugMeasureTiming) {
        console.log(oTimer.check())
    }

    g_ui.isRendering = false;
}


/**
 * Ben Fisher, 2010
 * @license GNU General Public License version 3
 * https://www.gnu.org/licenses/gpl-3.0.txt
 * https://github.com/moltenjs/labs_youthful_projects
 */

"use strict";
function on_btnaddline(e) {
    on_btngeneral();
    createNew("l");
    doTransformRender();
    g_state.hasUserChanges = true;
}

function on_btnaddcircle(e) {
    on_btngeneral();
    createNew("c");
    doTransformRender();
    g_state.hasUserChanges = true;
}

function on_btnaddgen(e) {
    on_btngeneral();
    createNew("lgen");
    doTransformRender();
    g_state.hasUserChanges = true;
}

function getFloat(txt, d) {
    var s = prompt(txt, d);
    return parseFloat(s);
}

function on_btntheta(e) {
    on_btngeneral();
    // manually sets coordinates that the user provides
    if (!g_ui.domSelected && !(e && e.shiftKey)) {
        errmsg("nothing is selected");
        return;
    }

    var msg = e && e.shiftKey ? "Move all shapes " : "Move shape ";
    var movex = getFloat(msg + "horizontally:", 0.0);
    if (!isFinite(movex)) {
        return;
    }

    var movey = getFloat(msg + "vertically:", 0.0);
    if (!isFinite(movey)) {
        return;
    }

    g_state.hasUserChanges = true;

    if (e && e.shiftKey) {
        // move all the shapes
        for (var key in g_ui.domToCoordObject) {
            var sh = g_ui.domToCoordObject[key];
            sh.x1 += movex;
            sh.y1 += movey;
            sh.x2 += movex;
            sh.y2 += movey;
        }
        // move main too...
        g_ui.mainContextShape.x1 += movex;
        g_ui.mainContextShape.y1 += movey;
        g_ui.mainContextShape.x2 += movex;
        g_ui.mainContextShape.y2 += movey;

        // save and reload
        var s = saveToJson();
        var fsturl = document.location.href.split("#")[0];
        var lnk = fsturl + "#" + s;
        document.location.href = lnk;
        return;
    }

    var shape = g_ui.domToCoordObject[g_ui.domSelected.id];
    var mainContext = contextFromRawShape(g_ui.mainContextShape);
    var newRelShape = rawShapeToRelativeShape(mainContext, shape);
    if (shape.type.startsWith("c")) {
        var setAngle = 0;
        var setLength = getFloat("Set radius:", newRelShape.length);
        if (!isFinite(setLength)) {
            return;
        }
    } else {
        var setLength = getFloat("Set length:", newRelShape.length);
        if (!isFinite(setLength)) {
            return;
        }

        var setAngle = getFloat("Set angle (degrees):", newRelShape.rotation);
        if (!isFinite(setAngle)) {
            return;
        }
    }

    shape.x1 += movex * mainContext.length;
    shape.x2 += movex * mainContext.length;
    shape.y1 += movey * mainContext.length;
    shape.y2 += movey * mainContext.length;
    newRelShape = rawShapeToRelativeShape(mainContext, shape);
    newRelShape.rotation = setAngle;
    newRelShape.length = setLength;
    var shpResult = new CRawShape();
    drawShapeRelativeToContext(mainContext, newRelShape, shpResult);
    shape.x2 = shpResult.x2;
    shape.y2 = shpResult.y2;
    shape.rx = shpResult.rx;
    refreshShape(g_ui.domSelected);
    doTransformRender();
}

function on_btndelete(e) {
    on_btngeneral();
    deleteSelected();
    doTransformRender();
    g_state.hasUserChanges = true;
}

function on_btnonlyperim(e) {
    on_btngeneral();
    if (g_state.nJustPerimeter > 0) {
        g_state.nJustPerimeter = 0;
        doTransformRender();
    } else {
        g_state.nJustPerimeter = 250;
        doTransformRender();
    }

    refreshBtnPerim();
    g_state.hasUserChanges = true;
}

function refreshBtnPerim() {
    $("idbtnonlyperim").style.backgroundColor =
        g_state.nJustPerimeter > 1 ? "#555" : "";
}

function on_btndrawmore(e) {
    on_btngeneral();
    g_state.nShapesToDraw += 25;
    doTransformRender();
    g_state.hasUserChanges = true;
}

function on_btndrawless(e) {
    on_btngeneral();
    g_state.nShapesToDraw -= 25;
    doTransformRender();
    g_state.hasUserChanges = true;
}

function on_btnzoomin(e) {
    on_btngeneral();
    on_btnzoom(1);
}

function on_btnzoomout(e) {
    on_btngeneral();
    on_btnzoom(-1);
}

function on_btnzoom(nDir) {
    if (nDir == 1) {
        g_state.zoomLevel *= 1.25;
    } else if (nDir == -1) {
        g_state.zoomLevel /= 1.25;
    }

    doTransformRender();
    g_state.hasUserChanges = true;
}

function on_btnopenexample(e) {
    on_btngeneral();
    if (g_state.hasUserChanges && !confirm("Open an example?")) {
        // ask this question since we'll wipe out user's state otherwise
        return;
    }

    onLoadExample();
    g_state.hasUserChanges = false;
}

function on_btnsave(e) {
    on_btngeneral();
    var skipCompress = e && e.shiftKey;
    onSave(skipCompress);
    g_state.hasUserChanges = false;
}

var onDragResize_start = function() {
    on_btngeneral();
    // storing original coordinates
    this.ox = this.attr("cx");
    this.oy = this.attr("cy");
    var oCoord = g_ui.domToCoordObject[g_ui.domSelected.id];
    if (oCoord.type == "c" && this === g_ui.shapeSelectB) {
        this.origRadius = oCoord.rx;
    }

    this.countRefresh = 0;
};

var onDragResize_move = function(dx, dy) {
    var oCoord = g_ui.domToCoordObject[g_ui.domSelected.id];
    if (!oCoord) {
        errmsg("no oCoord obj");
        return;
    }
    
    g_state.hasUserChanges = true;

    if (oCoord.type == "c" && this === g_ui.shapeSelectB) {
        dy = 0;
    }

    if (oCoord.type == "c" && dx + this.origRadius < 1) {
        // prevent making a circle with < 1 radius
        dx = -this.origRadius + 1;
    }

    this.attr({ cx: this.ox + dx, cy: this.oy + dy });

    if (this != g_ui.shapeSelectA && this != g_ui.shapeSelectB) {
        console.error("unknown resizer?");
        return;
    }

    if (oCoord.type.startsWith("l")) {
        // line
        if (this === g_ui.shapeSelectA) {
            oCoord.x1 = this.ox + dx;
            oCoord.y1 = this.oy + dy;
        } else if (this === g_ui.shapeSelectB) {
            oCoord.x2 = this.ox + dx;
            oCoord.y2 = this.oy + dy;
        }
    } else if (oCoord.type == "c") {
        // circle
        if (this === g_ui.shapeSelectA) {
            // moving the center
            oCoord.x1 = this.ox + dx;
            oCoord.y1 = this.oy + dy;

            // move other resizer accordingly.
            g_ui.shapeSelectB.attr({
                cx: oCoord.x1 + oCoord.rx,
                cy: oCoord.y1
            });
        } else if (this === g_ui.shapeSelectB) {
            // changing the radius!
            oCoord.rx = dx + this.origRadius;
        }
    }

    refreshShape(g_ui.domSelected);

    this.countRefresh++;
    if (this.countRefresh > 1) {
        this.countRefresh = 0;
    }

    if (this.countRefresh == 0) {
        doTransformRender();
    }
};

var onDragResize_up = function() {
    // redraw shapes.
    doTransformRender();
};

function on_btngeneral() {
    $("welcometext").style.display = "none";
}

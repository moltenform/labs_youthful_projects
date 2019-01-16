/**
 * Ben Fisher, 2010
 * @license GNU General Public License version 3
 * https://www.gnu.org/licenses/gpl-3.0.txt
 * https://github.com/moltenjs/labs_youthful_projects
 */

"use strict";
function on_btnaddline(e) {
    createNew("l");
    doTransformRender();
}

function on_btnaddcircle(e) {
    createNew("c");
    doTransformRender();
}

function on_btnaddgen(e) {
    createNew("lgen");
    doTransformRender();
}

function on_btntheta(e) {
    // manually sets coordinates that the user provides
    if (!g_ui.domSelected) {
        errmsg("nothing is selected");
        return;
    }

    var shape = g_ui.domToCoordObject[g_ui.domSelected.id];
    var sMvCoords = prompt("Move shape by (x,y)", "0,0");
    if (!sMvCoords || sMvCoords.split(",").length != 2) {
        return;
    }

    var x = parseFloat(sMvCoords.split(",")[0]) * mainContext.length;
    var y = parseFloat(sMvCoords.split(",")[1]) * mainContext.length;
    if (isNaN(x) || isNaN(y)) {
        return;
    }

    shape.x1 += x;
    shape.x2 += x;
    shape.y1 += y;
    shape.y2 += y;
    refreshShape(g_ui.domSelected);

    // reuse code, even if it's a bit unclear
    var newRelShape = rawShapeToRelativeShape(mainContext, shape);
    var newRel = prompt(
        "Write new (angle in degrees, length)",
        newRelShape.rotation + "," + newRelShape.length
    );

    if (!newRel || newRel.split(",").length != 2) {
        return;
    }

    var rot = parseFloat(newRel.split(",")[0]);
    var l = parseFloat(newRel.split(",")[1]);
    if (isNaN(rot) || isNaN(l)) {
        return;
    }

    newRelShape.rotation = rot;
    newRelShape.length = l;
    var shpResult = new CRawShape();
    drawShapeRelativeToContext(mainContext, newRelShape, shpResult);
    shape.x2 = shpResult.x2;
    shape.y2 = shpResult.y2;
    shape.rx = shpResult.rx;
    refreshShape(g_ui.domSelected);
    doTransformRender();
}

function on_btndelete(e) {
    deleteSelected();
    doTransformRender();
}

function on_btnonlyperim(e) {
    if (g_state.nJustPerimeter > 0) {
        g_state.nJustPerimeter = 0;
        doTransformRender();
    } else {
        g_state.nJustPerimeter = 250;
        g_state.nShapesToDraw = Math.min(g_state.nShapesToDraw, 500);
        doTransformRender();
    }
}

function on_btndrawmore(e) {
    g_state.nShapesToDraw += 25;
    doTransformRender();
}

function on_btndrawless(e) {
    g_state.nShapesToDraw -= 25;
    doTransformRender();
}

function on_btnzoomin(e) {
    on_btnzoom(1);
}

function on_btnzoomout(e) {
    on_btnzoom(-1);
}

function on_btnzoomout(nDir) {
    if (nDir == 1) {
        g_state.zoomLevel *= 1.25;
    } else if (nDir == -1) {
        g_state.zoomLevel /= 1.25;
    }

    doTransformRender();
}

function on_btnopenexample(e) {
    onLoadExample();
}

function on_btnsave(e) {}

var onDragResize_start = function() {
    // storing original coordinates
    this.ox = this.attr("cx");
    this.oy = this.attr("cy");
    var oCoord = g_ui.domToCoordObject[g_ui.domSelected.id];
    if (oCoord.type == "c" && this === self.shapeSelectB) {
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

    if (oCoord.type == "c" && this === self.shapeSelectB) {
        dy = 0;
    }

    if (oCoord.type == "c" && dx + this.origRadius < 1) {
        // prevent making a circle with < 1 radius
        dx = -this.origRadius + 1;
    }

    this.attr({ cx: this.ox + dx, cy: this.oy + dy });

    if (this != self.shapeSelectA && this != self.shapeSelectB) {
        errmsg("unknown resizer?");
        return;
    }

    if (oCoord.type.startsWith("l")) {
        // line
        if (this === self.shapeSelectA) {
            oCoord.x1 = this.ox + dx;
            oCoord.y1 = this.oy + dy;
        } else if (this === self.shapeSelectB) {
            oCoord.x2 = this.ox + dx;
            oCoord.y2 = this.oy + dy;
        }
    } else if (oCoord.type == "c") {
        // circle
        if (this === self.shapeSelectA) {
            // moving the center
            oCoord.x1 = this.ox + dx;
            oCoord.y1 = this.oy + dy;

            // move other resizer accordingly.
            self.shapeSelectB.attr({
                cx: oCoord.x1 + oCoord.rx,
                cy: oCoord.y1
            });
        } else if (this === self.shapeSelectB) {
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

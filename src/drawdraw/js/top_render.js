/**
 * Ben Fisher, 2010
 * @license GNU General Public License version 3
 * https://www.gnu.org/licenses/gpl-3.0.txt
 * https://github.com/moltenjs/labs_youthful_projects
 */

"use strict";
function drawArrow(domObj, rawShape, noAdd) {
    if (!g_ui.mapObjIdToArrow) {
        g_ui.mapObjIdToArrow = {};
    }

    // based on: http://taitems.tumblr.com/post/549973287/drawing-arrows-in-raphaeljs
    if (!g_ui.mapObjIdToArrow[domObj.id]) {
        g_ui.mapObjIdToArrow[domObj.id] = Ra.path("M1,1,L,1,1");
    }

    var arSize = 6 * g_ui.resizeFactor;
    var angleRad = Math.atan2(
        rawShape.x1 - rawShape.x2,
        rawShape.y2 - rawShape.y1
    );
    var angle = (angleRad / (2 * Math.PI)) * 360;
    var locx = rawShape.x2; // + 4 * g_ui.resizeFactor * Math.cos(anglerad)
    var locy = rawShape.y2; // + 4 * g_ui.resizeFactor * Math.sin(anglerad)
    g_ui.mapObjIdToArrow[domObj.id].attr({
        path:
            "M" + locx + " " + locy + " L" + (locx - arSize) + " " + (locy - arSize) +
            " L" + (locx - arSize) + " " + (locy + arSize) + " L" + locx + " " + locy
    });

    g_ui.mapObjIdToArrow[domObj.id].attr("stroke-width", g_ui.resizeFactor);
    g_ui.mapObjIdToArrow[domObj.id]
        .attr("fill", "#922")
        .attr("stroke", "#922")
        .rotate(90 + angle, locx, locy);

    // move arrow head behind the line
    g_ui.mapObjIdToArrow[domObj.id].toBack();
    if (noAdd) {
        // don't add to dict
        var ret = g_ui.mapObjIdToArrow[domObj.id];
        delete g_ui.mapObjIdToArrow[domObj.id];
        return ret;
    } else {
        return g_ui.mapObjIdToArrow[domObj.id];
    }
}

function removeArrows() {
    for (var key in g_ui.mapObjIdToArrow) {
        g_ui.mapObjIdToArrow[key].remove();
    }

    g_ui.mapObjIdToArrow = {};
}

function renderAllLines(arResults) {
    if (!arResults) {
        g_ui.oneLineGraphic.attr("path", "M1,1,L,1,1");
    } else {
        var allpath = arResults.join(" ");
        g_ui.oneLineGraphic.attr("path", allpath);
    }
}

function render_resetAllShapes() {
    g_ui.poolCircles.clearAll();
}

function renderCircle(rawShape) {
    var circle = g_ui.poolCircles.getNext();
    circle.show();
    updatePath(circle, rawShape);
}

// draws only the path.
// colors, arrows, and any ornamentation should be drawn elsewhere. (e.g. refreshShape())
function updatePath(domObj, rawShape) {
    if (rawShape.type == "l" || rawShape.type == "lgen") {
        var sNewPath =
            "M" +
            rawShape.x1.toString() +
            "," +
            rawShape.y1.toString() +
            ",L" +
            rawShape.x2.toString() +
            "," +
            rawShape.y2.toString();
        domObj.attr("path", sNewPath);
    } else if (rawShape.type == "c") {
        domObj.attr({ cx: rawShape.x1, cy: rawShape.y1, r: rawShape.rx });
    } else {
        errmsg("error: unknown shapetype");
    }
}

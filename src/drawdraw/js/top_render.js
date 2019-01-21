
/**
 * Ben Fisher, 2010
 * @license GNU General Public License version 3
 * https://www.gnu.org/licenses/gpl-3.0.txt
 * https://github.com/moltenjs/labs_youthful_projects
 */

"use strict";
// used to have a drawArrow() function here but we now use raphael to draw the arrow

// building a string is expensive, but
// the window.SVGPathSeg interface is deprecated+removed,
// and the new setPathData interface is not yet implemented
// https://svgwg.org/specs/paths/#DOMInterfaces

function renderAllLines(arResults) {
    if (!arResults) {
        g_ui.oneLineGraphic.attr("path", "M1,1,L,1,1");
    } else {
        var allpath = arResults.join(" ");
        if (g_state.checkVeryLargeNumbers && allpath.indexOf("e+") !== -1) {
            console.log("too large");
            return;
        }

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
// colors and any ornamentation should be drawn elsewhere. (e.g. refreshShape())
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


/**
 * Ben Fisher, 2010
 * @license GNU General Public License version 3
 * https://www.gnu.org/licenses/gpl-3.0.txt
 * https://github.com/moltenjs/labs_youthful_projects
 */

// a 'context' is a frame of reference. it specifies what is our current 'rotation'. what is the current 'length'
function CContext() {
    this.startx = 0.0
    this.starty = 0.0
    this.rotation = 0.0 // degrees. not used for circles.
    this.length = 0.0 // for circles, radius size
}

// a shape, relative to a context. requires a context in order to be drawn. 
function CRelativeShape() {
    this.type = 'l'
    this.angleToStart = 0.0 // angle from (start pt of context) to (start pt of shape)
    this.lengthToStart = 0.0 // distance from (start pt of context) to (start pt of shape)
    this.rotation = 0.0 // degrees. meaningless for circles.
    this.length = 0.0 // for circles, radius size
}

// raw shape object. used in ui frontend and returned by output.
// type is mandatory.
function CRawShape(obj) {
    this.type = 'l';
    this.x1 = 0;
    this.x2 = 0;
    this.y1 = 0;
    this.y2 = 0;
    this.rx = 0;

    // set attributes based on incoming dict.
    if (obj) {
        if (!obj.type) {
            errmsg('must pass type to rawShape constructor')
        }

        if (!obj.x1 || !obj.y1) {
            errmsg(debugprint(obj));
            errmsg('must pass coords to rawShape constructor')
        }

        for (var key in obj) {
            this[key] = obj[key]
        }
    }
}

function contextFromRawShape(rawShape) {
    if (!rawShape.type.startsWith('l')) {
        errmsg('can only be done for lines')
    }

    var context = new CContext()
    context.startx = rawShape.x1;
    context.starty = rawShape.y1;
    context.length = Math.sqrt((rawShape.x2 - rawShape.x1) * (rawShape.x2 - rawShape.x1) +
        (rawShape.y2 - rawShape.y1) * (rawShape.y2 - rawShape.y1))
    context.rotation = deg(Math.atan2(rawShape.y2 - rawShape.y1, rawShape.x2 - rawShape.x1));
    return context
}


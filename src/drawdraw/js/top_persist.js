
/**
 * Ben Fisher, 2010
 * @license GNU General Public License version 3
 * https://www.gnu.org/licenses/gpl-3.0.txt
 * https://github.com/moltenform/labs_youthful_projects
 */

"use strict";
function jsonToState2018(obj) {
    if (!obj["v"] && !obj["vn"]) {
        errmsg("Could not find version.");
        return;
    }

    if (obj["v"] > 2) {
        errmsg("File is from a future version... it might not load correctly.");
    }

    if (obj["nshapes"]) {
        g_state.nShapesToDraw = parseInt(obj["nshapes"]);
    }

    if (obj["pm"]) {
        g_state.nJustPerimeter = parseInt(obj["pm"]);
    }

    if (obj["perimeter"]) {
        g_state.nJustPerimeter = parseInt(obj["perimeter"]);
    }

    if (obj["main"]) {
        g_ui.mainContextShape = obj["main"];
    }

    if (!obj["shapes"]) {
        errmsg("Could not find shapes.");
        return;
    }

    var shapes = obj["shapes"];
    for (var i = 0; i < shapes.length; i++) {
        var oCoord = new CRawShape(shapes[i]);
        createNew(oCoord.type);
        g_ui.domToCoordObject[g_ui.domSelected.id] = oCoord;
        refreshShape(g_ui.domSelected);
    }

    refreshBtnPerim();
}

function onLoadFromJson_go(s) {
    // undo url-encoding
    s = s.replace(/%27/g, '"');
    s = s.replace(/%22/g, '"');
    if (!s.startsWith('{')) {
        s = LZString.decompressFromEncodedURIComponent(s);
    }
    
    s = g_state.compression.decompress(s);
    if (!s.startsWith('{"')) {
        errmsg("does not look like valid json " + s);
    }

    return s;
}

function onLoadFromJsonRawObj(s) {
    var s = onLoadFromJson_go(s);

    try {
        var obj = JSON.parse(s);
    } catch (err) {
        errmsg("could not parse");
        return;
    }

    jsonToState2018(obj);
    g_ui.domSelected = null;
    hideSelect();
    updatePath(g_ui.mainGenPath, g_ui.mainContextShape);
    doTransformRender();
}

function saveToJson() {
    if (!JSON) {
        errmsg(
            "Could not find JSON object. Try using latest Firefox or Chrome."
        );
        return;
    }

    var arRes = [];
    for (var objId in g_ui.domToCoordObject) {
        // first, strip out functions
        var newobj = {};
        for (var key in g_ui.domToCoordObject[objId]) {
            if (typeof g_ui.domToCoordObject[objId][key] != "function") {
                newobj[key] = g_ui.domToCoordObject[objId][key];
            }
        }

        arRes.push(newobj);
    }

    var objTop = {
        v: 2,
        nshapes: g_state.nShapesToDraw,
        perimeter: g_state.nJustPerimeter,
        zoomLevel: g_state.zoomLevel,
        main: g_ui.mainContextShape,
        shapes: arRes
    };

    var s = JSON.stringify(objTop);
    return s;
}

function onSave(skipCompress) {
    var s = saveToJson();
    if (!skipCompress) {
        s = g_state.compression.compress(s);
        s = LZString.compressToEncodedURIComponent(s);
    }

    var fsturl = document.location.href.split("#")[0];
    var lnk = fsturl + "#" + s;
    if (lnk.length >= 2000 && !isFirefox()) {
        alert(
            "warning... this document is too long for Chrome, which limits the length to 2000 characters. Please try Firefox instead."
        );
    }

    prompt("Copy this link, and send it to your friends!", lnk);
}

function CCompressCommonTermsDrawdraw() {
    this.startingChar = 126; // large enough that char (this.startingChar + x) will never be alphanumeric or a regexp metachar
    this.listTerms = [
        // important: do not remove items or change the order here -- this list is append-only
        '{"v":2,"nshapes":300,"perimeter":0,"zoomLevel":1,"main":{"type":"lgen","x1":200,"x2":200,"y1":200,"y2":100,"rx":0},',
        '"nshapes":300,"perimeter":0,"zoomLevel":1,"main":{"type":"lgen","x1":200,"x2":200,"y1":200,"y2":100,"rx":0},',
        '"main":{"type":"lgen","x1":200,"x2":200,"y1":200,"y2":100,"rx":0}',
        '{"v":',
        ',"nshapes":',
        ',"perimeter":',
        ',"zoomLevel":',
        ',"main":',
        '"type":',
        ',"x1":',
        ',"x2":',
        ',"y1":',
        ',"y2":',
        ',"rx":0},{',
        ',"rx":',
        ',"shapes":[{',
        '"type":"l"',
        '"type":"lgen"',
        '"type":"c"',
        "},{"
    ];
}

CCompressCommonTermsDrawdraw.prototype.compress = function(s) {
    for (var i = 0; i < this.listTerms.length; i++) {
        var re = new RegExp(this.escapeForRegex(this.listTerms[i]), "g");
        var replacement = String.fromCharCode(this.startingChar + i);
        s = s.replace(re, replacement);
    }

    return s;
};

CCompressCommonTermsDrawdraw.prototype.decompress = function(s) {
    for (var i = 0; i < this.listTerms.length; i++) {
        var replacement = String.fromCharCode(this.startingChar + i);
        var re = new RegExp(replacement, "g");
        s = s.replace(re, this.listTerms[i]);
    }

    return s;
};

CCompressCommonTermsDrawdraw.prototype.escapeForRegex = function(s) {
    return s.replace(/[-\/\\^$*+?.()|[\]{}]/g, "\\$&");
};

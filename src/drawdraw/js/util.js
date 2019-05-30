
/**
 * Ben Fisher, 2010
 * @license GNU General Public License version 3
 * https://www.gnu.org/licenses/gpl-3.0.txt
 * https://github.com/moltenform/labs_youthful_projects
 */

"use strict";
function rad(a) {
    return ((a % 360) * Math.PI) / 180;
}

function deg(a) {
    return ((a * 180) / Math.PI) % 360;
}

function degcos(a) {
    return Math.cos((a / 360.0) * 2 * Math.PI);
}

function degsin(a) {
    return Math.sin((a / 360.0) * 2 * Math.PI);
}

function errmsg(s) {
    alert(s);
}

function errmsgdbg(s) {
    alert(debugprint(s));
}

function $(sId) {
    return document.getElementById(sId);
}

// credit: stackoverflow
function debugprint(obj, maxDepth, prefix) {
    var result = "";
    if (!prefix) {
        prefix = "";
    }

    for (var key in obj) {
        if (typeof obj[key] == "object") {
            if (maxDepth !== undefined && maxDepth <= 1) {
                result += prefix + key + "=object [max depth reached]\n";
            } else {
                result += debugprint(
                    obj[key],
                    maxDepth ? maxDepth - 1 : maxDepth,
                    prefix + key + "."
                );
            }
        } else {
            result += prefix + key + "=" + obj[key] + "\n";
        }
    }
    return result;
}

String.prototype.startsWith = function(s) {
    return this.indexOf(s) === 0;
};

var Time = {};

Time.createTimer = function() {
    return new _Timer();
};

function _Timer() {
    this.dt = new Date();
}

_Timer.prototype.check = function() {
    var n = new Date();
    var nDiff = n.getTime() - this.dt.getTime();
    return nDiff;
};

// CResourcePool, a way to save the cost of allocations.
// we'll store a cache of objects to be re-used.

function CResourcePool(fnCreateNew, fnReset) {
    this.fnCreateNew = fnCreateNew;
    this.fnReset = fnReset;
    this.contents = [];
    this.marker = 0;
}

CResourcePool.prototype.getNext = function() {
    assertTrue(this.marker >= 0, this.marker.toString());
    if (this.marker < this.contents.length) {
        var ret = this.contents[this.marker];
        this.marker += 1;
        return ret;
    } else {
        var ret = this.fnCreateNew();
        this.contents[this.marker] = ret;
        this.marker += 1;
        return ret;
    }
};

CResourcePool.prototype.clearAll = function() {
    for (var i = 0; i < this.marker; i++) {
        this.fnReset(this.contents[i]);
    }

    this.marker = 0;
};

function assertTrue(b, s) {
    if (!b) {
        var msg = "assertTrue failed " + (s || "");
        throw msg;
    }
}

function assertEqual(expected, got, s) {
    if (expected != got) {
        var msg =
            "assertEqual failed, expected " +
            expected +
            " but got " +
            got +
            " " +
            (s || "");
        throw msg;
    }
}

function CResourcePool_tests() {
    var fnCreate = function() {
        return { a: -100, alive: true };
    };
    var fnReset = function(a) {
        a.alive = false;
    };
    var pool = new CResourcePool(fnCreate, fnReset);

    var o1 = pool.getNext();
    assertEqual(-100, o1.a);
    o1.a = 1;
    var o2 = pool.getNext();
    assertEqual(-100, o2.a);
    o2.a = 2;
    var o3 = pool.getNext();
    assertEqual(-100, o3.a);
    o3.a = 3;
    pool.clearAll();

    var o1 = pool.getNext();
    assertEqual(1, o1.a);
    assertEqual(false, o1.alive);
    o1.alive = true;
    var o2 = pool.getNext();
    assertEqual(2, o2.a);
    assertEqual(false, o2.alive);
    o2.alive = true;
    pool.clearAll();

    var o1 = pool.getNext();
    assertEqual(1, o1.a);
    assertEqual(false, o1.alive);
    o1.alive = true;
    var o2 = pool.getNext();
    assertEqual(2, o2.a);
    assertEqual(false, o2.alive);
    o2.alive = true;
    var o3 = pool.getNext();
    assertEqual(3, o3.a);
    assertEqual(false, o3.alive);
    o3.alive = true;
    var o4 = pool.getNext();
    assertEqual(-100, o4.a);
    assertEqual(true, o4.alive);
    o4.a = 4;
}

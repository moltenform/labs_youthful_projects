
/**
 * Ben Fisher, 2010
 * @license GNU General Public License version 3
 * https://www.gnu.org/licenses/gpl-3.0.txt
 * https://github.com/moltenjs/labs_youthful_projects
 */
 
 
var g_examples = [
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=lgen,X=218,A=290,Y=233,B=205,rx=0},{type=c,X=200,A=0,Y=150,B=0,rx=50}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=160.5,A=189.5,Y=193,B=98,rx=0},{type=lgen,X=160.5,A=189.5,Y=193,B=98,rx=0},{type=l,X=200,A=221,Y=200,B=131,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=190.92140921409214,A=159.3495934959349,Y=307.18157181571814,B=402.43902439024384,rx=0},{type=lgen,X=205.92140921409214,A=159.3495934959349,Y=318.18157181571814,B=402.43902439024384,rx=0},{type=l,X=200,A=200,Y=200,B=100,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=224.92140921409214,A=159.3495934959349,Y=419.18157181571814,B=396.43902439024384,rx=0},{type=lgen,X=205.92140921409214,A=159.3495934959349,Y=318.18157181571814,B=402.43902439024384,rx=0},{type=l,X=131,A=201,Y=119,B=112,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=190.92140921409214,A=159.3495934959349,Y=307.18157181571814,B=402.43902439024384,rx=0},{type=lgen,X=190.92140921409214,A=162.3495934959349,Y=307.18157181571814,B=401.43902439024384,rx=0},{type=l,X=200,A=200,Y=200,B=100,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=234.5469477142075,A=268.34565197554525,Y=199.78100191623324,B=234.70207135687562,rx=0},{type=l,X=187.82735651063052,A=282.35240441646135,Y=196.43215621863308,B=226.77251574048728,rx=0},{type=lgen,X=187.82735651063052,A=282.35240441646135,Y=196.43215621863308,B=226.77251574048728,rx=0},{type=l,X=200,A=200,Y=200,B=100,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=234.5469477142075,A=268.34565197554525,Y=199.78100191623324,B=234.70207135687562,rx=0},{type=l,X=187.82735651063052,A=282.35240441646135,Y=196.43215621863308,B=226.77251574048728,rx=0},{type=lgen,X=187.82735651063052,A=283.35240441646135,Y=196.43215621863308,B=227.77251574048728,rx=0},{type=l,X=200,A=200,Y=200,B=100,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=234.5469477142075,A=268.34565197554525,Y=199.78100191623324,B=234.70207135687562,rx=0},{type=l,X=187.82735651063052,A=282.35240441646135,Y=196.43215621863308,B=226.77251574048728,rx=0},{type=lgen,X=187.82735651063052,A=283.35240441646135,Y=196.43215621863308,B=227.77251574048728,rx=0},{type=l,X=200,A=200,Y=200,B=100,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=190.92140921409214,A=159.3495934959349,Y=307.18157181571814,B=402.43902439024384,rx=0},{type=lgen,X=189.92140921409214,A=162.3495934959349,Y=305.18157181571814,B=401.43902439024384,rx=0},{type=l,X=200,A=200,Y=200,B=100,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=158.5,A=191.5,Y=177,B=97,rx=0},{type=lgen,X=158.5,A=108.5,Y=177,B=90,rx=0},{type=l,X=200,A=200,Y=200,B=100,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=158.5,A=191.5,Y=177,B=97,rx=0},{type=lgen,X=158.5,A=60.5,Y=177,B=200,rx=0},{type=l,X=200,A=200,Y=200,B=100,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=158.5,A=191.5,Y=177,B=97,rx=0},{type=lgen,X=158.5,A=60.5,Y=177,B=156,rx=0},{type=l,X=200,A=200,Y=200,B=100,rx=0}]}',
    '{vn=0.1,nshapes=325,pm=0,shapes=[{type=l,X=158.5,A=199.5,Y=177,B=100,rx=0},{type=lgen,X=158.5,A=58.5,Y=177,B=185,rx=0},{type=l,X=200,A=200,Y=200,B=100,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=200,A=286.6025403784439,Y=200,B=150,rx=0},{type=lgen,X=200,A=297,Y=200,B=178,rx=0},{type=l,X=200,A=286.6025403784439,Y=100,B=150,rx=0},{type=l,X=200,A=200,Y=200,B=100,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=lgen,X=122.50000000000004,A=195.0000000000001,Y=138.99999999999997,B=199.99999999999991,rx=0},{type=c,X=179,A=0,Y=231,B=0,rx=17},{type=c,X=163,A=0,Y=215,B=0,rx=4.5}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=lgen,X=266.50000000000006,A=195.0000000000001,Y=141.99999999999997,B=199.99999999999991,rx=0},{type=c,X=179,A=0,Y=231,B=0,rx=17},{type=c,X=190,A=0,Y=198,B=0,rx=12.5},{type=c,X=190,A=0,Y=198,B=0,rx=1}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=144.70753871752208,A=195.31844361960836,Y=288.44233969025987,B=299.07922692947653,rx=0},{type=lgen,X=147.70753871752208,A=229.31844361960836,Y=310.44233969025987,B=360.07922692947653,rx=0},{type=c,X=33.96377831818768,A=0,Y=332.47228977345446,B=0,rx=4.5160552062968335},{type=l,X=135.00575963138363,A=5.74184052220653,Y=75.71995392294889,B=253.70984257007547,rx=0},{type=l,X=200,A=200,Y=200,B=100,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=200,A=200,Y=200,B=100,rx=0},{type=lgen,X=200,A=286,Y=200,B=150,rx=0},{type=c,X=200,A=0,Y=150,B=0,rx=50}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=200,A=234,Y=200,B=114,rx=0},{type=lgen,X=200,A=175,Y=200,B=118,rx=0},{type=c,X=200,A=0,Y=150,B=0,rx=50}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=lgen,X=218,A=290,Y=233,B=205,rx=0},{type=c,X=200,A=0,Y=150,B=0,rx=50},{type=c,X=201,A=0,Y=151,B=0,rx=40},{type=l,X=184,A=240,Y=115,B=161,rx=0},{type=c,X=243,A=0,Y=116,B=0,rx=5}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=161,A=162,Y=200,B=99,rx=0},{type=lgen,X=201,A=252,Y=75,B=37,rx=0},{type=l,X=165,A=253,Y=101,B=101,rx=0},{type=l,X=251,A=252,Y=103,B=197,rx=0},{type=l,X=161,A=250,Y=198,B=198,rx=0},{type=l,X=207,A=207,Y=101,B=195,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=lgen,X=231,A=230,Y=128,B=179,rx=0},{type=c,X=201,A=0,Y=151,B=0,rx=60}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=lgen,X=207.5,A=198.5,Y=197.5,B=127.5,rx=0},{type=lgen,X=126,A=182,Y=173,B=189.00000000000003,rx=0},{type=l,X=200,A=200,Y=200,B=100,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=lgen,X=207.5,A=198.5,Y=197.5,B=127.5,rx=0},{type=lgen,X=120,A=143,Y=204,B=186.00000000000003,rx=0},{type=l,X=200,A=200,Y=200,B=100,rx=0}]}',
    '{vn=0.1,nshapes=500,pm=0,shapes=[{type=l,X=200,A=200,Y=200,B=100,rx=0},{type=lgen,X=200,A=200,Y=100,B=50,rx=0},{type=lgen,X=150,A=150,Y=200,B=150,rx=0},{type=lgen,X=250,A=250,Y=200,B=150,rx=0}]}',
    '{vn=0.1,nshapes=1100,pm=0,shapes=[{type=lgen,X=200,A=200,Y=100,B=50,rx=0},{type=lgen,X=150,A=150,Y=200,B=150,rx=0},{type=lgen,X=250,A=250,Y=200,B=150,rx=0},{type=c,X=200,A=0,Y=150,B=0,rx=99}]}',
    '{vn=0.1,nshapes=450,pm=0,shapes=[{type=lgen,X=201,A=200,Y=51,B=92,rx=0},{type=lgen,X=137,A=137,Y=155,B=193,rx=0},{type=lgen,X=257,A=256,Y=156,B=195,rx=0},{type=c,X=200,A=0,Y=142,B=0,rx=118}]}',
    '{vn=0.1,nshapes=450,pm=0,shapes=[{type=lgen,X=149,A=149,Y=104,B=85,rx=0},{type=lgen,X=244,A=245,Y=107,B=86,rx=0},{type=c,X=200,A=0,Y=142,B=0,rx=118},{type=l,X=149,A=239,Y=184,B=184,rx=0},{type=l,X=272,A=238,Y=161,B=183,rx=0},{type=l,X=149,A=123,Y=183,B=158,rx=0}]}',
    '{vn=0.1,nshapes=725,pm=0,shapes=[{type=lgen,X=200,A=214,Y=200,B=125,rx=0},{type=lgen,X=200,A=158,Y=100,B=119,rx=0},{type=c,X=156,A=0,Y=135,B=0,rx=13}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=149,A=149,Y=199,B=101,rx=0},{type=lgen,X=215,A=243,Y=83,B=56,rx=0},{type=l,X=147,A=259,Y=101,B=101,rx=0},{type=l,X=258,A=261,Y=103,B=199,rx=0},{type=l,X=150,A=262,Y=197,B=197,rx=0},{type=lgen,X=183,A=155,Y=86,B=53,rx=0}]}',
    '{vn=0.1,nshapes=1000,pm=250,shapes=[{type=lgen,X=200,A=200,Y=200,B=166.66666700000002,rx=0},{type=lgen,X=200.000003,A=200.000003,Y=133.33333299999998,B=99.9999997,rx=0},{type=lgen,X=228.86751057272994,A=200,Y=150.000001,B=133.33333299999998,rx=0},{type=lgen,X=200,A=228.86751057272994,Y=166.666666,B=150.000001,rx=0},{type=l,X=200,A=122,Y=100,B=193,rx=0}]}',
    '{vn=0.1,nshapes=1300,pm=250,shapes=[{type=lgen,X=200,A=200,Y=200,B=165.66666700000002,rx=0},{type=lgen,X=200.000003,A=200.000003,Y=141.33333299999998,B=99.9999997,rx=0},{type=lgen,X=235.86751057272994,A=199,Y=151.000001,B=141.33333299999998,rx=0},{type=lgen,X=200,A=237.86751057272994,Y=160.666666,B=153.000001,rx=0},{type=l,X=200,A=122,Y=100,B=193,rx=0}]}',
    '{vn=0.1,nshapes=2350,pm=250,shapes=[{type=l,X=200,A=200,Y=200,B=100,rx=0},{type=lgen,X=200,A=200,Y=200,B=175,rx=0},{type=lgen,X=200,A=200,Y=125,B=100,rx=0},{type=lgen,X=200,A=230,Y=175,B=175,rx=0},{type=lgen,X=225,A=175.00000000000003,Y=150,B=150,rx=0},{type=lgen,X=225,A=225,Y=175,B=150,rx=0},{type=lgen,X=175,A=200,Y=125,B=125,rx=0},{type=lgen,X=175,A=175,Y=150,B=125,rx=0}]}',
    '{vn=0.1,nshapes=1325,pm=250,shapes=[{type=l,X=200,A=200,Y=200,B=100,rx=0},{type=lgen,X=200,A=239,Y=200,B=149,rx=0},{type=lgen,X=200,A=256,Y=100,B=150,rx=0}]}',
    '{vn=0.1,nshapes=1000,pm=250,shapes=[{type=l,X=200,A=200,Y=200,B=100,rx=0},{type=lgen,X=200,A=240,Y=200,B=150,rx=0},{type=lgen,X=239,A=199,Y=150,B=101,rx=0}]}',
    '{vn=0.1,nshapes=4300,pm=250,shapes=[{type=l,X=200,A=200,Y=150,B=100,rx=0},{type=lgen,X=200,A=256,Y=200,B=149,rx=0},{type=lgen,X=255,A=199,Y=150,B=101,rx=0}]}',
    '{vn=0.1,nshapes=2000,pm=250,shapes=[{type=l,X=200,A=200,Y=200,B=100,rx=0},{type=lgen,X=200,A=149,Y=200,B=223,rx=0},{type=lgen,X=200,A=241,Y=100,B=62,rx=0},{type=lgen,X=200,A=228,Y=100,B=149,rx=0}]}',
    '{vn=0.1,nshapes=1000,pm=250,shapes=[{type=lgen,X=200,A=200,Y=200,B=166.66666700000002,rx=0},{type=lgen,X=200.000003,A=200.000003,Y=133.33333299999998,B=99.9999997,rx=0},{type=l,X=200,A=122,Y=100,B=193,rx=0}]}',
    '{vn=0.1,nshapes=4300,pm=0,shapes=[{type=l,X=174,A=200,Y=100,B=100,rx=0},{type=lgen,X=200,A=256,Y=200,B=149,rx=0},{type=lgen,X=202,A=199,Y=170,B=101,rx=0}]}',
    '{vn=0.1,nshapes=4300,pm=250,shapes=[{type=l,X=219,A=200,Y=114,B=100,rx=0},{type=lgen,X=200,A=256,Y=200,B=149,rx=0},{type=lgen,X=202,A=199,Y=170,B=101,rx=0}]}',
    '{vn=0.1,nshapes=4300,pm=0,shapes=[{type=l,X=219,A=200,Y=114,B=100,rx=0},{type=lgen,X=245,A=256,Y=227,B=149,rx=0},{type=lgen,X=202,A=199,Y=170,B=101,rx=0}]}',
    '{vn=0.1,nshapes=4300,pm=0,shapes=[{type=l,X=219,A=200,Y=114,B=100,rx=0},{type=lgen,X=295,A=256,Y=241,B=149,rx=0},{type=lgen,X=202,A=199,Y=170,B=101,rx=0}]}',
    '{vn=0.1,nshapes=425,pm=0,shapes=[{type=l,X=199,A=200,Y=201,B=124,rx=0},{type=c,X=200,A=0,Y=100,B=0,rx=31},{type=l,X=268,A=230,Y=100,B=100,rx=0},{type=l,X=167,A=77,Y=99,B=118,rx=0},{type=l,X=201,A=201,Y=68,B=37,rx=0},{type=lgen,X=211,A=238,Y=87,B=60,rx=0},{type=lgen,X=187,A=165,Y=89,B=63,rx=0},{type=lgen,X=216,A=241,Y=112,B=137,rx=0},{type=lgen,X=189,A=164,Y=112,B=139,rx=0}]}',
    '{vn=0.1,nshapes=675,pm=0,shapes=[{type=l,X=200,A=200,Y=200,B=100,rx=0},{type=lgen,X=226,A=243,Y=101,B=74,rx=0},{type=l,X=169,A=230,Y=101,B=100,rx=0},{type=lgen,X=172,A=151,Y=103,B=71,rx=0},{type=lgen,X=228,A=243,Y=101,B=126,rx=0},{type=lgen,X=169,A=147,Y=102,B=124,rx=0}]}',
    '{vn=0.1,nshapes=300,pm=0,shapes=[{type=l,X=200,A=200,Y=200,B=100,rx=0},{type=l,X=200,A=293,Y=100,B=100,rx=0},{type=l,X=202,A=246,Y=164,B=163,rx=0},{type=l,X=244,A=244,Y=197,B=163,rx=0},{type=l,X=256,A=291,Y=137,B=137,rx=0},{type=l,X=254,A=255,Y=100,B=137,rx=0},{type=lgen,X=273,A=273,Y=100,B=120,rx=0},{type=lgen,X=228,A=228,Y=196,B=177,rx=0},{type=lgen,X=283,A=283,Y=137,B=123,rx=0},{type=lgen,X=215,A=215,Y=164,B=177,rx=0}]}',
]
    
// keep separate from g_audioglobal, since we reset g_audioglobal after loading a doc
g_curExample = 0
function onLoadExample() {
    g_curExample = onLoadExampleImpl(g_curExample)
}

function onLoadExampleImpl(curExample) {
    curExample+=1
    curExample = Math.abs(curExample)
    curExample = curExample % g_examples.length
    snewdata = g_examples[curExample]
    var ser = tremolo_serializeAndCompress(JSON.parse(snewdata), false /* skipCompressed */)
    var snewurl = document.location.href.split('#')[0] + '#' + ser
    document.location.href = snewurl	
    return curExample
}


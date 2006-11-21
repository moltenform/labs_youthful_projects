//~ en-us.js
//~ Ben Fisher, 2006
//~ Contains data for a standard english keyboard layout.

//~ There are two different id numbers:
//~ The "keyid" is the keyCode value of the key.
//~ The "cellid" is the id of the cell in the table, specific to this program.

//~ This object maps "keyid" to "cellid"
//~ This mapping does not contain modifiers (Ctrl, Shift, Alt )
Layout.keycodeMap = {
27:0, 112: 1, 113:2, 114:3, 115:4,116:5, 117:6, 118:7, 119:8, 120:9, 121:10, 122:11, 123:12,  /*esc + fn keys*/
145:15, 19:16, /*scroll lock etc*/
192:18, 49:19, 50:20, 51:21, 52:22,53:23,54:24,55:25,56:26,57:27,48:28, /*top row numbers*/
189:29, 187:30, 8:31,45:32,36:33,33:34,144:35,111:36,106:37,109:38, /*Insert, etc*/
9:40,81:41,87:42,69:43,82:44,84:45,89:46,85:47,73:48,79:49,80:50, /*Letters Q-P*/
219:51,221:52,220:53,46:54,35:55,34:56,103:57,104:58,105:59,107:60, /*{ to +*/
20:62,65:63,83:64,68:65,70:66,71:67,72:68,74:69,75:70,76:71,186:72,222:73,13:74, /*Letters A-L*/
100:76, 101:77, 102:78, /*keypad 4-6*/
90:81,88:82,67:83,86:84,66:85,78:86,77:87,188:88,190:89,191:90, /*Letters z-m*/
38:93,97:95,98:96,99:97,13:98, /*keypad 1-3*/
32:103, 93:106, 37:108, 40:109, 39:110, 96:111, 110:112 /*Bottom row*/
};

//~ Maps "keyid" of modifier keys to one or more "cellid"s.
Layout.keycodeModifiers =
{
17:['control',100,107], 16:['shift',80,91], 18:['alt',102,104]
};

//~ Visual layout data for the keyboard. One entry for every key.
//~ cellid : [keyName , keyWidth], where keyWidth is relative (not in pixels).
//~ A null keyName and positive width means a spacer element. A null keyName and negative width means a new line.
Layout.cellLayout = {
0: ['Esc',100],
1:['F1',100],
2:['F2',100],
3:['F3',100],
4:['F4',100],
5:['F5',100],
6:['F6',100],
7:['F7',100],
8:['F8',100],
9:['F9',100],
10:['F10',100],
11:['F11',100],
12:['F12',100],
13:[null,500],
14:['PntScr',100],
15:['ScrLck',100],
16:['Break',100],
17:[null,-9999], /*New line*/

18:['`',100],
19:['1',100],
20:['2',100],
21:['3',100],
22:['4',100],
23:['5',100],
24:['6',100],
25:['7',100],
26:['8',100],
27:['9',100],
28:['0',100],
29:['-',100],
30:['=',100],
31:['Bkspce',200],/*1500*/
32:['Ins',100],
33:['Home',100],
34:['PgUp',100],
35:['NmLck',100],
36:['%Num%/',100],
37:['%Num%*',100],
38:['%Num%-',100],
39:[null,-9999],

40:['Tab',160],
41:['Q',100],
42:['W',100],
43:['E',100],
44:['R',100],
45:['T',100],
46:['Y',100],
47:['U',100],
48:['I',100],
49:['O',100],
50:['P',100],
51:['[',100],
52:[']',100],
53:['\\',140],/*1500*/
54:['Del',100],
55:['End',100],
56:['PgDn',100],
57:['%Num%7',100],
58:['%Num%8',100],
59:['%Num%9',100],
60:['%Num%+',100],
61:[null,-9999],


62:['Caps',180],
63:['A',100],
64:['S',100],
65:['D',100],
66:['F',100],
67:['G',100],
68:['H',100],
69:['J',100],
70:['K',100],
71:['L',100],
72:[':',100],
73:["'",100],
74:['Enter',220 ],
75:[null,300],
76:['%Num%4',100],
77:['%Num%5',100],
78:['%Num%6',/*200*/ 100],
79:[null,-9999],

80:['Shift',250],
81:['Z',100],
82:['X',100],
83:['C',100],
84:['V',100],
85:['B',100],
86:['N',100],
87:['M',100],
88:[',',100],
89:['.',100],
90:['/',100],
91:['Shift',250],
92:[null,100],
93:['Up',100],
94:[null,100],
95:['%Num%1',100],
96:['%Num%2',100],
97:['%Num%3',100],
98:['%Num%Enter',100],
99:[null,-9999],

100:['Ctrl',140],
101:['Win',140],
102:['Alt',140],
103:[' ',520],
104:['Alt',140],
105:['Win',140],
106:['*',140],
107:['Ctrl',140],
108:['Left',100],
109:['Down',100],
110:['Right',100],
111:['%Num%0',200],
112:['%Num%.',100]



};
Layout.cellLayout_length =113;

//Because of the cell margins and padding, the cells begin to misalign from left to right.
//The following map is a correcting factor for a certain cellid. 
//The first element is how many cells there should have been, and the second is how many there were.
//The difference between these numbers can be multiplied by the marginwidth in pixels to get a correcting factor 
Layout.correctAlignment ={  31:[14,14], 53:[14,14], 74:[14,13], 91:[14,12], 107:[14,8],
13 : [4,1], 75 : [3,1] };


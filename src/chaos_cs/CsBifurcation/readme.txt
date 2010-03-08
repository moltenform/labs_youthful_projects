CsBifurcation
Ben Fisher, 2010, halfhourhacks.blogspot.com
Released under GPLv3.

I wrote a program to draw bifurcation diagrams of any 1-dimensional map.

Recently, I studied 1-dimensional maps in my nonlinear dynamics course. 
I recommend reading about this topic: it is fascinating that an expression
as simple as p_next = r*p*(1-p) can lead to what is known as "chaos." r is a constant,
 p is a number between 0 and 1, and the expression is repeatedly evaluated.
The resulting sequence of p can either settle to a 
fixed number, periodically cycle between values, or continue aperiodically and seemingly in a random fashion.
A <a href="http://en.wikipedia.org/wiki/Bifurcation_diagram">Bifurcation diagram</a> concisely depicts
the behavior as the parameter r changes.
Read more about the logistic map on <a href="http://en.wikipedia.org/wiki/Logistic_map">Wikipedia</a>
or <a href="http://hypertextbook.com/chaos/11.shtml">elsewhere</a>.

I used Matlab for most numerical work pertaining to the course. In my spare time, though, I wrote this program to experiment with 
different maps and parameters. Also, because the program runs so much more quickly, I could create better images without waiting.
(This program builds upon my earlier project that dynamically compiles code, and so it runs efficiently.)

<a href="http://students.olin.edu/2010/bfisher/blog/09other/csbifurcation_bin.zip">Download</a> (Unzip and run CsBifurcation.exe). 
<a href="http://students.olin.edu/2010/bfisher/blog/09other/csbifurcation_src.zip">Source</a>. (GPLv3 license)
To use the program, type in an expression, and click Go. By default, the famous logistic map is shown.
Use the mouse to zoom in. By holding the Alt key and dragging, you can zoom in on a non-square rectangle of the plot.
Refer to the readme.txt file for more information. I encourage you to run this program and explore the intricate details of the logistic map,
and to invent your own 1D maps.
The program can also create animations by slowly changing one of the parameters over time. I will post some
of the animations I've made at a later date.

As I wrote new expressions and drew bifurcation diagrams, I began to search for interesting shapes and details. 
I began to compose images based on appearance rather than mathematical properties.

Details
-----------------
"Settling" is the amount of iterations run before any points are drawn. To be most accurate, this value should be set high to eliminate
transient effects, but low values can speed drawing time or produce an interesting appearence.
"Shading" is the amount of darkness added each time a pixel is landed on.
To increase the number of iterations, add a line like "additionalShading=2.0;" to your init. code.

You can use "c1" and "c2" in your expression, and they will have the values that you have set. 
Open the examples in "cfgs" to see what else can be done.

Mouse shortcuts
-------------
* Drag			zoom in window		(must be from left to right)
* Alt key+Drag		non-square zoom

* Shift key+click		zoom out
* Right click		undo zoom
* Shift key+right click	reset view
* Middle click		reset view


Keyboard shortcuts
-------------
* Press ctrl+space to redraw
* Press Left/Right or Pgup/Pgdn to make small changes to C1, C2
 
Tips and Tricks
-----------
* rand() returns a random number 0.0 to 1.0. randneg() is from -1.0 to 1.0.
* click the value of c1 (to the left of the trackbar) to manually set its value
* you can enter an arbitrary expression for P0, such as c1, rand(), or randneg().
* Change additionalShading in init. code for higher quality. Useful when rendering.


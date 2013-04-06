CsPhasePortrait
Ben Fisher, 2010, 
Released under GPLv3.




Details
-----------------
"Settling" is the amount of iterations run before any points are drawn. To be most accurate,
this value should be set high to eliminate transient effects, but low values can speed 
drawing time or produce an interesting appearence.
"Shading" is the amount of darkness added each time a pixel is landed on.
To increase the number of iterations, add a line like "additionalShading=2.0;" to your init. code.

You can use "c1" and "c2" in your expression, and they will have the values that you have set. 
Open the examples in "cfgs" to see what else can be done.

Keyboard shortcuts
-------------
* Press ctrl+space to redraw
* Press Left/Right or Pgup/Pgdn to make small changes to C1, C2 if they have focus

Mouse shortcuts
-------------
* Drag			zoom in window		(must be from left to right)
* Alt key+Drag		non-square zoom

* Shift key+click		zoom out
* Right click		undo zoom
* Shift key+right click	reset view
* Middle click		reset view

Alt-shift-drag to (or maybe I shouldn't talk about this.)


Tips and Tricks
-----------
* rand() returns a random number 0.0 to 1.0. randneg() is from -1.0 to 1.0.
* alert("hello") and clipboardset("45")
* click the value of c1 (to the left of the trackbar) to manually set its value
* you can enter an arbitrary expression for P0, such as c1, rand(), or randneg().
* Change additionalShading in init. code for higher quality. Useful when rendering.


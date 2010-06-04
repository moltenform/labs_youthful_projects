SDL Fastmapmap
Ben Fisher, 2010, GPL license.
http://halfhourhacks.blogspot.com/2010/06/chaotic-maps.html
Post feedback as comments on this blog.

fastmap.exe is the Burgers map, a chaotic 2D map introduced by J. M. Burgers.
fastmaphenon.exe is the Henon map, a standard chaotic 2D map.
fastmapginger.exe is the Gingerbread map, a chaotic 2D map.

Usage
------------------
Use the arrow keys to move around.

Control-click	zoom in
Shift-click		zoom out
Right-click 		undo zoom
Alt-drag		zoom to region
Alt-shift-drag	zoom to any rectangle

Press Tab to switch between these drawing modes:
1	Basins of Attraction
	(Alt-1 to add color, Shift-1 to color based on last x coordinate)
	
2	Quadrants
	(Alt-2 for contrast, Shift-2 to color based on difference between coords)
	
3	Phase Portrait

4	Color lines
	(Shift-4 for shaded disk, Alt-4 and Alt-shift 4 change disk size) 

A short tutorial
------------------
	- Open fastmap.exe
	- Tap the right arrow a bit to change the value of a.
	- Press Ctrl-s, type 'test', and press Enter to save as test.cfg
	- Ctrl-click on the shape to zoom in, Shift-click to zoom out
	- Press Ctrl-n to reset the view.
	- Press Ctrl-o, type 'test' and press Enter to open our test.cfg
	- Press Alt-b and watch the shapes move
	- Press Alt-b again to stop movement
	- Click the cross-hairs icon to show the diagram. Click in the diagram.

More hotkeys
------------------
Alt-b		starts 'breathing' mode
b, shift-b	adjust amplitude of breathing

Ctrl-S	Save
Ctrl-O	Open
Ctrl-N	Reset
Ctrl-R	Render image (higher quality)
Ctrl-B	Render animation of 'breathing' mode (sequence of .bmp files)
Ctrl-' Ctrl-; Ctrl-: 	Set other parameters

Middle-click to reset view.
Shift-arrow keys for finer movement.

Ctrl-F1	save into first keyframe (Ctrl-F2 for second keyframe and so on...)
F1		open first keyframe
Ctrl-Shift-F1	delete first keyframe
Shift-3		set number of interpolated frames per keyframe
Return/Enter	preview animation
Ctrl-Enter		render animation, creates sequence of .bmp files

= or -		increase or decrease iterations
Shift = or -		adjust coloring, shading
Alt-D		change diagram coloring

Longer tutorial; how to make animations
----------------
	- Open fastmap.exe
	- look in saves_b and try opening some examples.
		(Press Ctrl-o to open, don't type the .cfg extension)
	- Press 1 to show 'basins' coloring mode.
	- Press Alt-1 to add blue coloring.
	- Press 3 to return to portrait mode.
	Let's create an animation. Look for an interesting shape, using arrow keys.
	- Press Ctrl-F1 to save this as the first keyframe
	Move to another interesting area
	- Press Ctrl-F2 to save as second keyframe
	Now, you can press Enter to preview the animation.
	Press Shift-3 to set number of frames
	Press F1 to load the first frame again.
	- Press Ctrl-F3 to save a third keyframe
	But, if you change your mind and only want to animate between the first two,
		- Press Ctrl-shift-F3 to delete the 3rd keyframe
		Now the animation is just between the first two.
	Press Ctrl-Enter to save the animation as many .bmp files.
	Programs like Virtualdub or Windows Movie Maker can make a movie from these.
	

Compilation
------------------
Edit whichmap.h and recompile to change map expression.
In Linux, use the supplied Makefile. 
Depends on packages libsdl and libsdl-dev.
Move the binary into the 'out' directory, currently the 'data' and 'saves_b' directories
must be in the same directory as the binary.

In Windows, 
Tested compiling under visual studio 2005 or newer.
You may need to edit common.h to specify the path to sdl.h.
Move the binary into the 'out' directory, currently the 'data' and 'saves_b' directories
must be in the same directory as the binary.

Some explanation
-------------
This is the first release of "fastmapmap", a program I wrote that plots 2D chaotic maps. 
A discrete chaotic map will take the point (x,y) to a new (x_, y_) based on a function. 
For example, the Henon map takes the point (x,y) to (y+1-a*x*x, b*x), where a and b are constants.
One can draw a "portrait" of a map by choosing an (x0, y0) and repeatedly evaluating the function,
shading in each point that is landed on.
Depending on the values of a and b, sometimes the point moves off to become very large, or sometimes
the point alternates jumping between two or more values. Other times, the points shade in a region in 
a complicated way, never repeating, which is a chaotic orbit.

The program fastmapmap can draw these portraits in real time. (I put some effort into optimization). 
The parameters a and b can be quickly changed with the arrow keys. The plot can be easily navigated; 
hold alt and drag the mouse to zoom in on a region.

The program is called fastmapmap because an additional plot can be drawn that is a 
"map" of the behavior of the map. The x axis represents the constant a and the y axis is b.
In this plot, black areas are periodic, red areas escape to infinity, and colored areas are chaotic.
So, the plot can depict which parameter values may be interesting to observe.

Licensing
----------------
Drawing text from
http://cone3d.gamedev.net/cgi-bin/index.pl?page=tutorials/gfxsdl/tut4
HSL2RGB from
http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
Quick cast-to-int from
http://www.mega-nerd.com/FPcast/#Macro

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
    
    
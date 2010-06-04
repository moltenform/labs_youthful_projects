SDL Fastmapmap
Ben Fisher, 2010, GPL license.
http://halfhourhacks.blogspot.com
Post feedback as comments on http://halfhourhacks.blogspot.com/2010/06/chaotic-maps.html

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

Compilation
------------------
Edit whichmap.h and recompile to change map expression.
In Linux, use the supplied Makefile. 
Move the binary into the 'out' directory, currently the 'data' and 'saves_burger' directories
must be in the same directory as the binary.

In Windows, 
You may need to edit common.h to specify the path to sdl.h.
Move the binary into the 'out' directory, currently the 'data' and 'saves_burger' directories
must be in the same directory as the binary.


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
    
    
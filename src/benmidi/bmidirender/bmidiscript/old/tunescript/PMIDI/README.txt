PMIDI: A Python wrapper around the Windows MIDI stream functions
See the LICENSE.txt file for restrictions on use.

See the scale() function in the Composer.py file installed with this package for a good example of
what PMIDI can do. Here is a very simple example for starters.

from PMIDI import *
from time import sleep

s = Sequencer()
o = s.NewSong()
t = o.NewVoice()
m = t.NewMeasure()
m.NewNote(0, 8, 'C', 4)
m.NewNote(8, 8, 'D', 4)
m.NewNote(16, 8, 'E', 4)
m.NewNote(24, 8, 'F', 4)
m.NewNote(32, 8, 'G', 4)
m.NewNote(40, 8, 'A', 4)
m.NewNote(48, 8, 'B', 4)
m.NewNote(56, 8, 'C', 5)

s.Play()
sleep(5)

s.Close()
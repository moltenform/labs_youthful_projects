
import sys; sys.path.append('..')

from mingus.containers import Composition,Track, Instrument, Note, Bar
from mingus.extra import MusicXML, LilyPond

class TrebleInstrument(Instrument):
	name = ''
	range = (Note('C', 0), Note('C', 10))
	clef = 'Treble'
	def __init__(self, name):
		self.name = name
		Instrument.__init__(self)

class BassInstrument(Instrument):
	name = ''
	range = (Note('C', 0), Note('C', 10))
	clef = 'Bass'
	def __init__(self, name):
		self.name = name
		Instrument.__init__(self)

comp = Composition()
comp.set_title('The Mingus')
comp.set_author('Ben')

ins = TrebleInstrument('kazoo')

track = Track(ins)
track.name = 'WWW' #instead of 'untitled'
comp.add_track(track)

firstbar = Bar(meter=(3,4))
track.add_bar(firstbar)
print track.add_notes(['C-5'], 4.0)
print track.add_notes(['E-5'], 2.0)

print track.add_notes(['C-4','D-4'], 8.0)
print track.add_notes(['C-4','D-4','F-4'], 8.0)
print track.add_notes([], 8.0) #treated as rest?
print track.add_notes(['C-4','D-4'], 8.0)
print track.add_notes(['C-4','D-4'], 8.0)
print track.add_notes(['C-4','D-4'], 8.0)
print track.add_notes(['C-4','D-4'], 24.0)
print track.add_notes(['C-4','D-4'], 24.0)
print track.add_notes(['C-4','D-4'], 24.0)
print track.add_notes(['C-4','D-4'], 8.0)

s = MusicXML.from_Composition(comp)
f=open('out.xml','w')
f.write(s)
f.close()

s=LilyPond.from_Composition(comp)
f=open('out.ly','w')
f.write(s)
f.close()

#4.0 qtr
#2.0 half
#1.0 whole




'''
bbuilder.py, Ben Fisher 2008
Simplified interface for "building" a midi.
Read the examples below to see how to use it.

making a simple melody
	b = BMidiBuilder()
	b.note('c', 1) #pitch 60, duration 1 qtr note
	b.note('d', 1)
	b.note('e', 1)
	b.note('f', 1)
	b.note('g', 4)
	b.save('out.mid')

making a chord
	b = BMidiBuilder()
	b.note('c', 1)
	b.rewind(1)
	b.note('e', 1)
	b.rewind(1)
	b.note('g', 1)
	b.save('out.mid') #doesn't "sort" the events until the end.


more options
	b = BMidiBuilder()
	b.tempo = 60
	b.setInstrument('flute') #looks up in list, until first match. b.setInstrument(73) also works
	b.note('c', 1, velocity=127)
	
	#ways to type same note:
	b.note(61, 1)
	b.note('c#', 1) #use sharps, not flats.
	b.note('c#4', 1)
	
	#octaves
	b.note('c#5', 1)
	b.note('c#6', 1)
	b.save('out.mid')

advanced (2 tracks)
	tr1 = BMidiBuilder()
	tr1.setInstrument('fretless bass')
	tr1.note('c3', 2)
	tr1.note('d3', 2)
	tr1.note('e3', 2)
	
	tr2 = BMidiBuilder()
	tr2.setInstrument('ocarina')
	tr2.note('e4', 2)
	tr2.note('f4', 2)
	tr2.note('g4', 2)
	
	BMidiBuilder.joinTracks( [tr1, tr2], 'out.mid') #tempo of first track used.
'''

import cPMIDI
import Constants

class CommandFactory(object):
  '''Factory class that generates MIDI commands from simple values.'''
  
  def MIDINote(self, attack, pitch, channel):
    '''Classmethod. Create a MIDI note on/off command.
    
    @param attack: Note attack value (0-127)
    @type attack: integer
    @param pitch: Note pitch value (0-127)
    @type pitch: integer
    @param channel: Channel to sound note (0-15)
    @type channel: integer
    '''
    cmd = 0x90 + channel
    return (Constants.MEVT_SHORTMSG << 24) | (attack << 16) | (pitch << 8) | cmd
  
  def MIDITempo(self, val):
    '''Classmethod. Create a MIDI tempo command.
    
    @param val: The tempo in beats per minute (bpm)
    @type val: integer
    '''  
    return (Constants.MEVT_TEMPO << 24) | val
  
  def MIDIVoice(self, instrument, channel):
    '''Classmethod. Create a MIDI change program command.
    
    @param instrument: The program number of the instrument (0-127)
    @type instrument: integer
    @param channel: Channel to associate instrument with
    @type channel: integer
    '''    
    cmd = 0xC0 + channel
    return (instrument << 8) | cmd
  
  MIDINote = classmethod(MIDINote)
  MIDITempo = classmethod(MIDITempo)
  MIDIVoice = classmethod(MIDIVoice)

class Note(object):
  '''Class that represents a single note in a measure.
  
  @ivar pitch: Note name
  @type pitch: string
  @ivar beat: Beat to start sounding note in current measure (0-64)
  @type beat: integer
  @ivar channel: Channel number which will play the note (0-16)
  @type channel: integer
  @ivar measure: Measure holding the note
  @type measure: L{Measure}
  @ivar duration: Duration of the note in beats
  @type duration: integer
  @ivar start: Indicator of whether note is starting or stopping
  @type start: boolean
  '''
  def __init__(self, channel, measure, beat, duration, pitch, start=True):
    '''Initialize an instance. 
    
    @param channel: Channel number which will play the note (0-16)
    @type channel: integer
    @param measure: Measure holding the note
    @type measure: L{Measure}
    @param beat: Beat to start sounding note in current measure (0-64)
    @type beat: integer
    @param duration: Duration of the note in beats
    @type duration: integer
    @param pitch: Note name
    @type pitch: string
    @param start: Indicator of whether note is starting or stopping
    @type start: boolean
    '''
    # check some important params
    if pitch > 127:
      raise ValueError('invalid pitch')
    self.pitch = pitch
    if beat >= Constants.CPM:
      raise ValueError('invalid beat')
    self.beat = beat    

    # just store the rest
    self.channel = channel
    self.measure = measure
    self.duration = duration
    self.start = start
   
  def __repr__(self):
    '''Human readable note.
    
    @return: Printable representation of a note and its params
    @rtype: string
    '''
    s = '<ch: %(channel)d, meas: %(measure)d, beat: %(beat)d, dur: %(duration)d, name: %(name)s, oct: %(octave)d, flag: %(start)s>\n' % \
        self.__dict__
    return s
    
  def __cmp__(self, other):
    '''Compare notes first by measure, then by beat, then by start or stop flag. This op is needed for proper sequencing.
    
    @param other: Another note
    @type other: L{Note}
    @return: Indicator whether this note should be sequenced before or after the other note
    @rtype: boolean
    '''
    if self.measure < other.measure:
      return -1
    elif self.measure > other.measure:
      return 0
    else:
      if self.beat < other.beat:
        return -1
      elif self.beat > other.beat:
        return 1
      else:
        if self.start and not other.start:
          return 1
        elif not self.start and other.start:
          return -1
        else:
          return 0
      
  def DelayBetween(self, other):
    '''Compute the delay between this note and another.
    
    @param other: Another note
    @type other: L{Note}
    @return: Duration in ticks between this note and the other
    @rtype: integer
    '''
    if other is None:
      return Constants.TPC * (self.measure * Constants.CPM  + self.beat)
    else:
      return Constants.TPC * ((self.measure - other.measure) * Constants.CPM + (self.beat - other.beat))
      
  def MakeStopNote(self):
    '''Generate a partner note that represents when this note should stop.
    
    @return: Note silencing a previously played note of the same pitch on the same channel
    @rtype: L{Note}
    '''
    pos = self.measure * Constants.CPM + self.beat + self.duration
    m = pos / Constants.CPM
    b = pos % Constants.CPM
    return Note(self.channel, m, b, 0, self.pitch, False)
  
  def ToMIDI(self):
    '''Convert this note to a MIDI command and return it.
    
    @return: A MIDI command represting the note
    @rtype: integer
    '''
    attack = int(self.start) * 0x7F
    return CommandFactory.MIDINote(attack, self.pitch, self.channel)

class Measure(object):
  '''Class that represents a single measure for a voice.
  
  @ivar notes: All of the notes starting in this measure
  @type notes: list of L{Note}
  @ivar measure_number: The position of this measure with respect to all other measures
  @type measure_number: integer
  @ivar channel: Channel the notes in this measure will sound play on
  @type channel: integer
  '''
  def __init__(self, channel, measure_number):
    '''Initialize an instance.
    
    @param channel: Channel the notes in this measure will sound play on
    @type channel: integer
    @param measure_number: The position of this measure with respect to all other measures
    @type measure_number: integer
    '''
    self.notes = []
    self.measure_number = measure_number
    self.channel = channel
    
  def NewNote(self, beat, duration, name, octave):
    '''Create a new note in this measure and store it.
    
    @param beat: Beat to start sounding note in current measure (0-64)
    @type beat: integer
    @param duration: Duration of the note in beats
    @type duration: integer
    @param name: Note name
    @type name: string
    @param octave: Octave of the note (5 is middle)
    @type octave: integer
    '''
    pitch = octave * 12 + Constants.note_name_map[name]
    self.notes.append(Note(self.channel, self.measure_number, beat, duration, pitch))
    
  def NewHit(self, beat, duration, name):
    '''Generate a new note object for a drumset and store it.
    
    @param beat: Beat to sound the hit in the current measure (0-64)
    @type beat: integer
    @param duration: Duration of the note in beats
    @type duration: integer
    @param name: Name of the instrument in the drum kit to hit
    @type name: string
    '''
    pitch = Constants.GM_drums.index(name) + 35
    self.notes.append(Note(self.channel, self.measure_number, beat, duration, pitch))
    
  def ClearNotes(self):
    '''Get rid of all the held notes.'''
    self.notes = []
    
  def Sequence(self):
    '''Generate a list of all the notes and their stop partners.
    
    @return: Sequenced notes in this measure
    @rtype: list of integer
    '''
    seq = []
    for n in self.notes:
      seq.append(n)
      seq.append(n.MakeStopNote())
    return seq
      
class Voice(object):
  '''Class that represents a single voice in a song.
  
  @ivar channel: Channel associated with this voice
  @type channel: integer
  @ivar instrument: Program number of the instrument represented by this voice
  @type instrument: integer
  @ivar measures: All of the measures played by this voice
  @type measures: list of L{Measure}
  '''
  def __init__(self, channel):
    '''Initialize an instance.
    
    @param channel: Channel associated with this voice
    @type channel: integer
    '''
    self.channel = channel
    self.instrument = 0
    self.measures = []
    
  def NewMeasure(self, number=None):
    '''Create a new measure object. Store it and return a reference so notes can be added.
    
    @param number: The position of the new measure in the sequence of measures. If None, will create measure n+1
    @type number: integer
    @return: The created measure
    @rtype: L{Measure}
    '''
    # see if the user has specified a specific measure number
    if number is not None:
      m = Measure(self.channel, number)
    else:
      m = Measure(self.channel, len(self.measures))
    self.measures.append(m)
    return m    
    
  def Sequence(self):
    '''Generate a sorted list of all notes in all measures.
    
    @return: Sequenced notes in all measures
    @rtype: list of integer
    '''
    # grab all info from the measures
    seq = []
    for m in self.measures:
      seq += m.Sequence()
      
    # sort the sequence
    seq.sort()
      
    return seq
  
  def ToMIDI(self):
    '''Convert this voice to a MIDI change program command and return it.
    
    @return: A MIDI command represting a program change
    @rtype: integer
    '''
    return CommandFactory.MIDIVoice(self.instrument, self.channel)
        
class Instrument(Voice):
  '''Class that represents a single, melodic instrument.'''
  
  def ListInstruments(self):
    '''Return a list of standard instrument names.
    
    @return: Listing of all General MIDI instrument names (some spelled wrong, got it off the Web)
    @rtype: list of string
    '''
    return Constants.GM_instruments
    
  def SetInstrumentByName(self, name):
    '''Set the current instrument by name.
    
    @param name: The name of the instrument to use for this voice
    @type name: string
    '''
    try:
      i = Constants.GM_instruments.index(name)
      self.instrument = i
    except:
      raise ValueError('invalid instrument')
      
  def SetInstrumentByNumber(self, i):
    '''Set the current instrument by number.
    
    @param i: The number of the instrument to use for this voice
    @type i: integer
    '''
    if i < 0 or i >= len(Constants.GM_instruments):
      raise ValueError('invalid instrument')
    else:
      self.instrument = i
    
class Drums(Voice):
  '''Class that acts as a set of drums.'''
  
  def ListInstruments(self):
    '''Return a list of standard drum names.
    
    @return: Listing of all drums in the General MIDI drum kit (some spelled wrong, got it off the Web too)
    @rtype: list of string
    '''
    return Constants.GM_drums

class Song(object):
  '''Class that wraps the dirty details of creating and sequencing MIDI instructions in order to play a song.
  
  @ivar voices: All of the voices used in this song
  @type voices: list of L{Voice}
  @ivar channel_count: Total number of channels used in the song
  @type channel_count: integer
  @ivar has_drums: Indicator of whether or not the drum channel is in use
  @type has_drums: boolean
  @ivar tempo: Tempo in beats per minute
  @type tempo: integer
  '''
  
  def __init__(self, tempo=120):
    '''Initialize an instance.
    
    @param tempo: Tempo in beats per minute (default = 120)
    @type tempo: integer
    '''
    self.voices = []
    self.channel_count = 0
    self.has_drums = False
    self.tempo = tempo
    
  def SetTempo(self, val):
    '''Set the tempo to a value in beats per minute.
    
    @param val: New tempo value
    @type val: integer
    '''
    self.tempo = val
    
  def GetTempo(self):
    '''Get the tempp in beats per minute.
    
    @return: Current tempo value
    @rtype: integer
    '''
    return self.tempo
  
  def GetVoices(self):
    '''Return all of the current voices.
    
    @return: List of all voices used in the song
    @rtype: list of L{Voice}
    '''
    return self.voices
    
  def NewVoice(self, is_drum=False):
    '''Create a new voice object and add it to the song. Return a reference.
    
    @param is_drum: Indicator of whether the new voice is the drum kit or not (default = False)
    @type is_drum: boolean
    @return: The new voice
    @rtype: L{Voice}
    '''
    # see if we're trying to assign a drum voice
    if is_drum:
      if self.has_drums:
        raise ValueError('drum channel in use')
      else:
        v = Drums(9)
        self.channel_count += 1
        self.voices.append(v)
        
    # otherwise, treat it as an instrument
    else:
      if self.channel_count == 15:
        raise ValueError('no free channels')
      
      v = Instrument(self.channel_count)
      self.channel_count += 1
      self.voices.append(v)
      
      # skip the drum channel
      if self.channel_count == 9: 
        self.channel_count += 1
        
    return v        

  def Sequence(self):
    '''Create an ordered list of all of the notes in a song.
    
    @return: All of the MIDI note commands in the song ordered by time of occurence
    @rtype: list of integer
    '''
    # grab all info from the tracks
    seq = []
    for v in self.voices:
      seq += v.Sequence()
      
    # do a global sort
    seq.sort()
    
    return seq
  
  def ToMIDI(self):
    '''Return the tempo and instrument program change commands.
    
    @return: All commands changing the tempo or program for a channel
    @rtype: list of integer
    '''
    t = int(60.0 / self.tempo * 1e6)
    cmd = [0, 0, CommandFactory.MIDITempo(t)]
    
    for v in self.voices:
      cmd += [0, 0, v.ToMIDI()]
      
    return cmd
    
class Sequencer(object):
  '''Class the encapsulates the functions necessary to generate and play short MIDI sequences.
  
  @ivar stream: MIDI stream handle (opaque)
  @type stream: integer
  @ivar song: Current song
  @type song: L{Song}
  @ivar seq: Last sequenced song
  @type seq: list of integer
  '''
  def __init__(self, device=0):
    self.stream  = cPMIDI.OpenMIDIStream(device)
    self.song = None
    self.seq = None

  def Close(self):
    '''Close the open MIDI stream. Call this before destroying the object.'''
    cPMIDI.CloseMIDIStream(self.stream)
    
  def NewSong(self):
    '''Create a new song, store it, and return a reference.
    
    @return: The new song
    @rtype: L{Song}
    '''
    self.song = Song()
    self.seq = None
    return self.song
  
  def Sequence(self):
    '''Convert the currently held song into a sequence of MIDI commands.'''
    self.seq = []
    last = None
    
    # get the tempo and instrument commands from the song
    self.seq += self.song.ToMIDI()
    
    # convert all the notes now, keeping track of the delay between each command
    notes = self.song.Sequence()
    for n in notes:
      self.seq += [n.DelayBetween(last), 0, n.ToMIDI()]
      last = n
  
  def Play(self, song=None):
    '''Play a song.
    
    @param song: Song to play. Play the currently held song if None.
    @type song: L{Song}
    '''
    # see if the user wants to play the provided song or the held song
    if song is not None:
      self.song = song
      self.seq = None
      self.Sequence()
      
    # now see if the held song is already sequenced
    elif self.seq is None:
      self.Sequence()
      
    # now pass the sequence to the underlying MIDI library for playback
    cPMIDI.FillMIDIStream(self.stream, tuple(self.seq))
    
    # start the stream playing
    cPMIDI.RestartMIDIStream(self.stream)
    
  def Stop(self):
    '''Stop a currently playing song.'''
    cPMIDI.StopMIDIStream(self.stream)
    
  def Pause(self):
    '''Pause a currently playing song.'''
    cPMIDI.PauseMIDIStream(self.stream)
    
  def Unpause(self):
    '''Pause a currently playing song.'''
    cPMIDI.RestartMIDIStream(self.stream)    
 
def scale():
  '''Example code that plays an embellished scale.'''
  import time
  
  s = Sequencer()
  song = s.NewSong()
  song.SetTempo(160)
  
  t = song.NewVoice()
  t.SetInstrumentByName('French Horn')
  m = t.NewMeasure()
  m.NewNote(0, 16, 'C', 5)
  m.NewNote(16, 16, 'D', 5)
  m.NewNote(32, 16, 'E', 5)
  m.NewNote(48, 16, 'F', 5)
  
  m = t.NewMeasure()
  m.NewNote(0, 16, 'G', 5)  
  m.NewNote(16, 16, 'A', 5)
  m.NewNote(32, 16, 'B', 5)
  m.NewNote(48, 16, 'C', 6)
  m.NewNote(48, 16, 'E', 5)
  m.NewNote(48, 16, 'G', 5)

  t = song.NewVoice()
  t.SetInstrumentByName('Accoustic Bass')
  m = t.NewMeasure()
  m.NewNote(0, 8, 'C', 4)
  m.NewNote(8, 8, 'C', 4)
  m.NewNote(16, 8, 'C', 4)
  m.NewNote(24, 8, 'C', 4)
  m.NewNote(32, 8, 'C', 4)
  m.NewNote(40, 8, 'C', 4)
  m.NewNote(48, 8, 'C', 4)
  m.NewNote(56, 8, 'C', 4)

  m = t.NewMeasure()
  m.NewNote(0, 8, 'C', 3)
  m.NewNote(8, 8, 'C', 3)
  m.NewNote(16, 8, 'C', 3)
  m.NewNote(24, 8, 'C', 3)
  m.NewNote(32, 8, 'C', 3)
  m.NewNote(40, 8, 'C', 3)
  m.NewNote(48, 8, 'C', 3)
  m.NewNote(56, 8, 'C', 3)
  
  t = song.NewVoice()
  t.SetInstrumentByName('Tinkle bell')
  m = t.NewMeasure(1)
  for i in range(8):
    run = ['C', 'B', 'Bb', 'A', 'Ab', 'G', 'Gb', 'F']
    m.NewNote(i+56, 1, run[i], 8)

  t = song.NewVoice(is_drum = True)
  m = t.NewMeasure()
  m.NewHit(0, 8, 'Bass Drum 1')
  m.NewHit(8, 8, 'Pedal Hi-Hat')
  m.NewHit(16, 8, 'Bass Drum 1')
  m.NewHit(24, 8, 'Pedal Hi-Hat')
  m.NewHit(32, 8, 'Bass Drum 1')
  m.NewHit(40, 8, 'Pedal Hi-Hat')
  m.NewHit(48, 8, 'Bass Drum 1')
  m.NewHit(56, 8, 'Pedal Hi-Hat')

  m = t.NewMeasure()
  m.NewHit(0, 8, 'Bass Drum 1')
  m.NewHit(8, 8, 'Pedal Hi-Hat')
  m.NewHit(16, 8, 'Bass Drum 1')
  m.NewHit(24, 8, 'Pedal Hi-Hat')
  m.NewHit(32, 8, 'Bass Drum 1')
  m.NewHit(40, 8, 'Pedal Hi-Hat')
  m.NewHit(48, 8, 'Bass Drum 1')
  m.NewHit(56, 8, 'Pedal Hi-Hat')

  s.Play()
  time.sleep(4)
  s.Close()
  
if __name__ == '__main__':
  scale()
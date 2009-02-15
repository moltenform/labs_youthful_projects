
import sys, string, types, exceptions

def midiToFrequency(n):
	return 8.1758 * (2** (n/12.0))
def frequencyToMidi(f):
	import math
	return int(round(69 + 12*math.log(f/440.0)/math.log(2.0)))


def pitchToName(nPitch):
	nOctave = int(nPitch / 12)-1
	nNote = nPitch % 12
	map = {0:'C',1:'C#',2:'D',3:'D#',4:'E',5:'F',6:'F#',7:'G',8:'G#',9:'A',10:'A#',11:'B'}
	return (map[nNote], nOctave)

def nameToPitch(s):
	# a very simple way to do this. I've written better ways elsewhere.
	#requires format c4 or c#4. doesn't accept flats.
	s = s.upper()
	map = {'C':0,'C#':1,'DB':1,'D':2,'D#':3,'EB':3,'E':4,'F':5,'F#':6,'GB':6,'G':7,'G#':8,'AB':8,'A':9,'A#':10,'BB':10,'B':11}
	c1 = s[0]
	c2 = s[0:2]
	if c2 in map:
		note = map[c2]
		octave = int(s[2:]) + 1
		return octave*12 + note
	elif c1 in map:
		note = map[c1]
		octave = int(s[1:]) + 1
		return octave*12 + note
	else:
		return None
		
		
#############

def showstr(str, n=16):
    for x in str[:n]:
        print ('%02x' % ord(x)),
    print

def getNumber(str, length):
    # MIDI uses big-endian for everything
    sum = 0
    for i in range(length):
        sum = (sum << 8) + ord(str[i])
    return sum, str[length:]

def getVariableLengthNumber(str):
    sum = 0
    i = 0
    while 1:
        x = ord(str[i])
        i = i + 1
        sum = (sum << 7) + (x & 0x7F)
        if not (x & 0x80):
            return sum, str[i:]

def putNumber(num, length):
    # MIDI uses big-endian for everything
    lst = [ ]
    for i in range(length):
        n = 8 * (length - 1 - i)
        lst.append(chr((num >> n) & 0xFF))
    return string.join(lst, "")

def putVariableLengthNumber(x):
    lst = [ ]
    while 1:
        y, x = x & 0x7F, x >> 7
        lst.append(chr(y + 0x80))
        if x == 0:
            break
    lst.reverse()
    lst[-1] = chr(ord(lst[-1]) & 0x7f)
    return string.join(lst, "")

class EnumException(exceptions.Exception):
    pass

class Enumeration:
    def __init__(self, enumList):
        lookup = { }
        reverseLookup = { }
        displayValues = { }
        i = 0
        displayValue = ''
        uniqueNames = [ ]
        uniqueValues = [ ]
        for x in enumList:
            if type(x) == types.TupleType:
                if len(x)==3:
                    x, i, displayValue = x
                else:
                    x, i = x; displayValue = ''
            if type(x) != types.StringType:
                raise EnumException, "enum name is not a string: " + x
            if type(i) != types.IntType:
                raise EnumException, "enum value is not an integer: " + i
            if x in uniqueNames:
                raise EnumException, "enum name is not unique: " + x
            if i in uniqueValues:
                raise EnumException, "enum value is not unique for " + x
            uniqueNames.append(x)
            uniqueValues.append(i)
            lookup[x] = i
            reverseLookup[i] = x
            displayValues[x] = displayValue
            i = i + 1
        self.lookup = lookup
        self.reverseLookup = reverseLookup
        self.displayValues = displayValues
    def __add__(self, other):
        lst = [ ]
        for k in self.lookup.keys():
            lst.append((k, self.lookup[k]))
        for k in other.lookup.keys():
            lst.append((k, other.lookup[k]))
        return Enumeration(lst)
    def hasattr(self, attr):
        return self.lookup.has_key(attr)
    def has_value(self, attr):
        return self.reverseLookup.has_key(attr)
    def __getattr__(self, attr):
        if not self.lookup.has_key(attr):
            raise AttributeError
        return self.lookup[attr]
    def whatis(self, value):
        return self.reverseLookup[value]



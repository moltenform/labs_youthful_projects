from __future__ import print_function
import sys, string, types


def midiToFrequency(n):
    return 8.1758 * (2**(n / 12.0))


def frequencyToMidi(f):
    import math
    return int(round(69 + 12 * math.log(f / 440.0) / math.log(2.0)))


def pitchToName(nPitch):
    nOctave = int(float(nPitch) / 12) - 1
    nNote = nPitch % 12
    map = {
        0: 'C',
        1: 'C#',
        2: 'D',
        3: 'D#',
        4: 'E',
        5: 'F',
        6: 'F#',
        7: 'G',
        8: 'G#',
        9: 'A',
        10: 'A#',
        11: 'B'
    }
    return (map[nNote], nOctave)


def nameToPitch(s):
    # a very simple way to do this. I've written better ways elsewhere.
    #requires format c4 or c#4. doesn't accept flats.
    s = s.upper()
    map = {
        'C': 0,
        'C#': 1,
        'DB': 1,
        'D': 2,
        'D#': 3,
        'EB': 3,
        'E': 4,
        'F': 5,
        'F#': 6,
        'GB': 6,
        'G': 7,
        'G#': 8,
        'AB': 8,
        'A': 9,
        'A#': 10,
        'BB': 10,
        'B': 11
    }
    c1 = s[0]
    c2 = s[0:2]
    if c2 in map:
        note = map[c2]
        octave = int(s[2:]) + 1
        return octave * 12 + note
    elif c1 in map:
        note = map[c1]
        octave = int(s[1:]) + 1
        return octave * 12 + note
    else:
        return None


def dataToPitchBend(pitch, velocity):
    #took a lot of trial and error to get this, seems right now, except for sign.
    yy = (velocity) & 0x7F
    xx = (pitch) & 0x7F
    return -1 * (8192 - ((yy << 7) + xx))


def pitchBendToData(n):
    #turn 00cccccccfffffff into 0ccccccc0fffffff
    n = -1 * (8192 - n)
    assert n < 2**16
    mask1 = int('0000000001111111', 2)
    mask2 = int('0011111110000000', 2)
    datalo = n & mask1
    datahi = (n & mask2) >> 7
    return datalo, datahi


def dataToTempo(data): #return value is in microseconds per quarter note
    assert len(data) == 3 #midi spec, data is 3 bytes
    assert isByteList(data)
    return data[0] * (16**4) + data[1] * (16**2) + data[2]


def tempoToData(msPerQtrNote):
    msPerQtrNote = int(msPerQtrNote)
    data = [0, 0, 0]
    data[0] = (msPerQtrNote & (0xFF << 16)) >> 16
    data[1] = (msPerQtrNote & (0xFF << 8)) >> 8
    data[2] = (msPerQtrNote & (0xFF))
    return bytearray(data)


#############


def showbytes(b, n=16):
    for x in b[:n]:
        print('%02x' % x, end=' ')
    print()


def getNumber(b, length):
    # MIDI uses big-endian for everything
    assert isByteList(b)
    sum = 0
    for i in range(length):
        sum = (sum << 8) + b[i]
    return sum, b[length:]


def getVariableLengthNumber(b):
    assert isByteList(b)
    sum = 0
    i = 0
    while 1:
        x = b[i]
        i = i + 1
        sum = (sum << 7) + (x & 0x7F)
        if not (x & 0x80):
            return sum, b[i:]


def putNumber(num, length):
    # MIDI uses big-endian for everything
    lst = []
    for i in range(length):
        n = 8 * (length - 1 - i)
        lst.append((num >> n) & 0xFF)
    return bytearray(lst)


def putVariableLengthNumber(x):
    assert x >= 0
    lst = []
    while 1:
        y, x = x & 0x7F, x >> 7
        lst.append(y + 0x80)
        if x == 0:
            break
    lst.reverse()
    lst[-1] = lst[-1] & 0x7f
    return bytearray(lst)


class EnumException(Exception):
    pass


class Enumeration(object):
    def __init__(self, enumList):
        lookup = {}
        reverseLookup = {}
        displayValues = {}
        i = 0
        displayValue = ''
        uniqueNames = []
        uniqueValues = []
        for x in enumList:
            if isinstance(x, tuple):
                if len(x) == 3:
                    x, i, displayValue = x
                else:
                    x, i = x
                    displayValue = ''
            if not isinstance(x, str):
                raise EnumException("enum name is not a string: " + x)
            if not isinstance(i, int):
                raise EnumException("enum value is not an integer: " + i)
            if x in uniqueNames:
                raise EnumException("enum name is not unique: " + x)
            if i in uniqueValues:
                raise EnumException("enum value is not unique for " + x)
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
        lst = []
        for k in self.lookup.keys():
            lst.append((k, self.lookup[k]))
        for k in other.lookup.keys():
            lst.append((k, other.lookup[k]))
        return Enumeration(lst)

    def hasattr(self, attr):
        return attr in self.lookup

    def has_value(self, attr):
        return attr in self.reverseLookup

    def __getattr__(self, attr):
        if attr not in self.lookup:
            raise AttributeError
        return self.lookup[attr]

    def whatis(self, value):
        return self.reverseLookup[value]


def strToByteList(s):
    ret = []
    for c in s:
        ret.append(ord(c))
    return bytearray(ret)


def byteListToStr(lst):
    assert isByteList(lst)
    ret = ''
    for c in lst:
        ret += chr(c)
    return ret


def isByteList(obj):
    return isinstance(obj, bytearray)

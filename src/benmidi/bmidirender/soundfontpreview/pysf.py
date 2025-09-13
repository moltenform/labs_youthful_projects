#!/usr/bin/python

# pysf, by Ben Collver

# The following changes have been made from pysf-2:
# Removed XML beautification because it freezes on large output.
# Added more loop types.
# Added preset id number, used by bmidi2wav.
#
# For some sf2 files, after converting to xml, pysf crashes when
# converting back to .sf2.  Not fixed yet.

import aifc, array, chunk, datetime, logging, math, os, os.path
import struct, sys, tempfile, wave


class PysfException(Exception):
    pass


class SfChunkReader(chunk.Chunk):
    Item = 0
    Form = b'NONE'

    def __init__(self, Handle):
        chunk.Chunk.__init__(self, Handle, True, False, False)
        if self.getsize() > 3:
            self.seek(0)
            self.Form = self.read(4)
            self.seek(0)

    def HeaderTell(self):
        return self.offset - 8

    def DataRead(self):
        self.seek(0)
        return self.read()

    def FormSkip(self):
        self.seek(4)

    def IsEnd(self):
        Retval = False
        if self.tell() >= self.getsize():
            Retval = True
        return Retval

    def SubChunk(self):
        Pos = self.tell()
        Retval = SfChunkReader(self.file)
        Size = Retval.getsize()
        self.seek(Pos + 8 + Size)
        return Retval


def old_div(a, b):
    "Equivalent to ``a / b`` on Python 2"
    import numbers
    if isinstance(a, numbers.Integral) and isinstance(b, numbers.Integral):
        return a // b
    else:
        return a / b


class SfTreeItem(object):
    Level = None
    CkId = None
    Form = None
    Chunk = None

    def __init__(self, Level, CkId, Form, Chunk):
        (self.Level, self.CkId, self.Form, self.Chunk) = (Level, CkId,
            Form, Chunk)

    def ChunkAssign(self, Chunk):
        self.Chunk = Chunk


class SfTree(object):
    Prefix = None
    Items = None
    Containers = None
    Parent = None
    Out = None
    WtPrefix = None

    def __init__(self, Items, Containers, Parent, Out, WtPrefix):
        self.Prefix = None
        self.Items = Items
        self.Containers = Containers
        self.Parent = Parent
        self.Out = Out
        self.WtPrefix = WtPrefix

    def ChunkFind(self, Chunk, Level):
        Retval = b'Not Found'
        for Item in self.Items:
            if Level != Item.Level:
                continue
            if Chunk.getname() != Item.CkId:
                continue
            if Item.Form != None and Chunk.Form != Item.Form:
                continue
            if Item.Chunk != None:
                Retval = b'Duplicate'
                break
            Chunk.Item = Item
            Item.ChunkAssign(Chunk)
            Retval = b'Found'
            break
        return Retval

    def ChunkIsContainer(self, Chunk):
        Retval = ListHas(self.Containers, Chunk.getname())
        if Retval:
            Chunk.FormSkip()
        return Retval

    def CkId(self, CkId, Form, Level):
        Retval = None
        for Item in self.Items:
            if Item.Chunk == None:
                continue
            if Level != -1 and Level != Item.Level:
                continue
            if CkId != None and CkId != Item.CkId:
                continue
            if Form != None and Form != Item.Form:
                continue
            Retval = Item
            break
        return Retval

    def CkIdStr(self, CkId, Form, Level):
        Retval = None
        Item = self.CkId(CkId, Form, Level)
        if Item != None:
            Retval = Item.Chunk.DataRead().split(b'\0', 1)[0]
        return Retval

    def Read(self, Chunk, Level):
        Found = self.ChunkFind(Chunk, Level)
        Fpos = Chunk.HeaderTell()
        CkId = Chunk.getname()
        Form = Chunk.Form
        PFmtStr = "Ck %s, pos %ld, id %c%c%c%c, frm %c%c%c%c, lvl %d, len %d"
        logging.info(PFmtStr % (Found, Fpos,
            CkId[0], CkId[1], CkId[2], CkId[3],
            Form[0], Form[1], Form[2], Form[3], Level, Chunk.getsize()))
        if Found != b'Found':
            logging.warn("Chunk %s" % (Found))
            Chunk.close()
        else:
            if self.ChunkIsContainer(Chunk) == True:
                while Chunk.IsEnd() == False:
                    SubChunk = Chunk.SubChunk()
                    self.Read(SubChunk, Level + 1)


class SfZoneType(object):
    KeyN = None
    ItemN = None
    ItemMax = None
    Oper = None
    Bag = None
    Gen = None
    Mod = None
    Hdr = None

    def __init__(self, ZoneTypeStr):
        if ZoneTypeStr == b'instrument':
            self.KeyN = b'instrument'
            self.ItemN = b'wavetable'
            self.Oper = 53
            self.Bag = b'ibag'
            self.Gen = b'igen'
            self.Mod = b'imod'
            self.Hdr = b'inst'
        elif ZoneTypeStr == b'preset':
            self.KeyN = b'preset'
            self.ItemN = b'instrument'
            self.Oper = 41
            self.Bag = b'pbag'
            self.Gen = b'pgen'
            self.Mod = b'pmod'
            self.Hdr = b'phdr'
        else:
            raise PysfException('Invalid value.')


def ustr(Arg):
    if sys.version_info[0] > 2:
        if isinstance(Arg, int):
            return str(Arg)
        elif isinstance(Arg, str):
            return Arg
        else:
            return Arg.decode('utf-8')
    else:
        return unicode(str(Arg), 'utf-8')


def Def(Variable, Default):
    if Variable == None:
        Variable = Default
    return Variable


def Val(Dict, Key):
    if Dict == None:
        Retval = None
    elif ListHas(list(Dict.keys()), Key):
        Retval = Dict[Key]
    else:
        Retval = None
    return Retval


def ListHas(List, Item):
    return len([x for x in List if x == Item]) > 0


def SfZoneList(Tree, Zt):
    AchName = b'ZORKMID'
    WBagNdx = -999
    WBank = -999
    WPreset = -999
    Order = 0
    Bag = Tree.CkId(Zt.Bag, None, -1)
    if Bag == None:
        raise PysfException("no %s section" % (Zt.Bag))
    BagD = Bag.Chunk.DataRead()
    BagFmtStr = '<2H'
    BagRecLen = struct.calcsize(BagFmtStr)
    Gen = Tree.CkId(Zt.Gen, None, -1)
    if Gen == None:
        raise PysfException("no %s section" % (Zt.Gen))
    GenD = Gen.Chunk.DataRead()
    GenFmtStr = '<2H'
    GenRecLen = struct.calcsize(GenFmtStr)
    Hdr = Tree.CkId(Zt.Hdr, None, -1)
    if Hdr == None:
        raise PysfException("no %s section" % (Zt.Gen))
    HdrD = Hdr.Chunk.DataRead()
    HdrFmtStr = '<20sH'
    if Zt.KeyN == b'preset':
        HdrFmtStr = HdrFmtStr + '2H3I'
    HdrFmtLen = struct.calcsize(HdrFmtStr)
    List = []
    while len(HdrD) > 0:
        logging.info("reading %s %d" % (Zt.KeyN, Order + 1))
        (ZoneIndex, LastAchName, LastWBagNdx, LastWBank, LastWPreset) = (1,
            AchName, WBagNdx, WBank, WPreset)
        if Zt.KeyN == b'instrument':
            (AchName, WBagNdx) = struct.unpack(HdrFmtStr, HdrD[0:HdrFmtLen])
        elif Zt.KeyN == b'preset':
            (AchName, WPreset, WBank, WBagNdx, DwLibrary, DwGenre,
             DwMorphology) = struct.unpack(HdrFmtStr, HdrD[0:HdrFmtLen])
        AchName = AchName.split(b'\0', 1)[0]
        HdrD = HdrD[HdrFmtLen:]
        if Order > 0:
            IPDict = {'id': Order, u'name': LastAchName, u'zones': {}}
            if Zt.KeyN == b'preset':
                IPDict[u'bank'] = LastWBank
                IPDict[u'presetId'] = LastWPreset
            ZList = []
            for I in range(LastWBagNdx, WBagNdx):
                Base = I * BagRecLen
                J = WGenNdx = struct.unpack('<H', BagD[Base:Base + 2])[0]
                ZDict = {}
                Generators = []
                while True:
                    OBase = J * GenRecLen
                    ABase = OBase + 2
                    SfGenOper = struct.unpack('<H', GenD[OBase:OBase + 2])[0]
                    (RangeBegin, RangeEnd) = struct.unpack('2B',
                        GenD[ABase:ABase + 2])
                    ShAmount = struct.unpack('<h', GenD[ABase:ABase + 2])[0]
                    WAmount = struct.unpack('<H', GenD[ABase:ABase + 2])[0]
                    try:
                        Name = SfGenNames[SfGenOper].split(b'_', 1)[1]
                    except KeyError:
                        Name = b'unknown'
                    if ListHas([43, 44], SfGenOper) == True:
                        ZDict[ustr(Name)] = {u'begin': RangeBegin,
                            u'end': RangeEnd}
                    elif ListHas([33, 34, 35, 36, 38], SfGenOper) == True:
                        Dt = 0
                        if ShAmount != SHMIN:
                            Dt = pow(2.0, ShAmount / 1200.0)
                        ZDict[ustr(Name)] = u'%0.04f' % (Dt)
                    elif ListHas([37, 39, 40], SfGenOper) == True:
                        ZDict[ustr(Name)] = ShAmount
                    elif ListHas([53, 41], SfGenOper) == True:
                        if SfGenOper != Zt.Oper:
                            raise PysfException("%s operator found in %s context" % (
                                Name, Zt.KeyN))
                        ZDict[ustr(Zt.ItemN) + u'Id'] = WAmount + 1
                    elif SfGenOper == 58:
                        if Zt.KeyN == b'preset':
                            logging.warn('ignoring overridingRootKey in preset')
                        else:
                            ZDict[ustr(Name)] = WAmount
                    elif SfGenOper == 57:
                        if Zt.KeyN == b'preset':
                            logging.warn('ignoring exclusiveClass in preset')
                        else:
                            ZDict[ustr(Name)] = WAmount
                    elif SfGenOper == 54:
                        if Zt.KeyN == b'preset':
                            logging.warn('ignoring sampleModes in preset')
                        else:
                            ZDict[ustr(Name)] = Def(Val(SfSampleModes,
                              WAmount), "0_LoopNone")
                    else:
                        Generators.append({u'comment': Name,
                            u'hexAmount': "0x%x" % WAmount, u'oper': SfGenOper})
                    if SfGenOper == Zt.Oper:
                        break
                    J = J + 1
                if len(Generators) > 0:
                    ZDict[u'gens'] = {u'gen': Generators}
                ZList.append(ZDict)
                ZoneIndex = ZoneIndex + 1
            IPDict[u'zones'][u'zone'] = ZList
            List.append(IPDict)
        Order = Order + 1
    return List


def SfItems():
    return [SfTreeItem(0, b'RIFF', b'sfbk', None),
        SfTreeItem(1, b'LIST', b'INFO', None),
        SfTreeItem(1, b'LIST', b'sdta', None),
        SfTreeItem(1, b'LIST', b'pdta', None),
        SfTreeItem(2, b'ifil', None, None),
        SfTreeItem(2, b'isng', None, None),
        SfTreeItem(2, b'INAM', None, None),
        SfTreeItem(2, b'irom', None, None),
        SfTreeItem(2, b'iver', None, None),
        SfTreeItem(2, b'ICRD', None, None),
        SfTreeItem(2, b'IENG', None, None),
        SfTreeItem(2, b'IPRD', None, None),
        SfTreeItem(2, b'ICOP', None, None),
        SfTreeItem(2, b'ICMT', None, None),
        SfTreeItem(2, b'ISFT', None, None),
        SfTreeItem(2, b'smpl', None, None),
        SfTreeItem(2, b'sm24', None, None),
        SfTreeItem(2, b'phdr', None, None),
        SfTreeItem(2, b'pbag', None, None),
        SfTreeItem(2, b'pmod', None, None),
        SfTreeItem(2, b'pgen', None, None),
        SfTreeItem(2, b'inst', None, None),
        SfTreeItem(2, b'ibag', None, None),
        SfTreeItem(2, b'imod', None, None),
        SfTreeItem(2, b'igen', None, None),
        SfTreeItem(2, b'shdr', None, None)]


logging.getLogger().setLevel(logging.WARN)
PysfVersion = 2
SfContainers = (b'RIFF', b'LIST')
SfGenNames = [b'0_startAddrsOffset',
    b'1_endAddrsOffset',
    b'2_startloopAddrsOffset',
    b'3_endloopAddrsOffset',
    b'4_startAddrsCoarseOffset',
    b'5_modLfoToPitch',
    b'6_vibLfoToPitchi',
    b'7_modEnvToPitch',
    b'8_initialFilterFc',
    b'9_initialFilterQ',
    b'10_modLfoToFilterFc',
    b'11_modEnvToFilterFc',
    b'12_endAddrsCoarseOffset',
    b'13_modLfoToVolume',
    b'14_unused',
    b'15_chorusEffectsSend',
    b'16_reverbEffectsSend',
    b'17_pan',
    b'18_unused',
    b'19_unused',
    b'20_unused',
    b'21_delayModLFO',
    b'22_freqModLFO',
    b'23_delayVibLFO',
    b'24_freqVibLFO',
    b'25_delayModEnv',
    b'26_attackModEnv',
    b'27_holdModEnv',
    b'28_decayModEnv',
    b'29_sustainModEnv',
    b'30_releaseModEnv',
    b'31_keynumToModEnvHold',
    b'32_keynumToModEnvDecay',
    b'33_delayVolEnv',
    b'34_attackVolEnv',
    b'35_holdVolEnv',
    b'36_decayVolEnv',
    b'37_sustainVolEnv',
    b'38_releaseVolEnv',
    b'39_keynumToVolEnvHold',
    b'40_keynumToVolEnvDecay',
    b'41_instrument',
    b'42_reserved',
    b'43_keyRange',
    b'44_velRange',
    b'45_startloopAddrsCoarseOffset',
    b'46_keynum',
    b'47_velocity',
    b'48_initialAttenuation',
    b'49_reserved',
    b'50_endloopAddrsCoarseOffset',
    b'51_coarseTune',
    b'52_fineTune',
    b'53_sampleId',
    b'54_sampleModes',
    b'55_reserved',
    b'56_scaleTuning',
    b'57_exclusiveClass',
    b'58_overridingRootKey',
    b'59_unused',
    b'60_endOper']
SfStNames = {1: b'monoSample',
    2: b'rightSample',
    4: b'leftSample',
    8: b'linkedSample',
    32769: b'RomMonoSample',
    32770: b'RomRightSample',
    32772: b'RomLeftSample',
    32776: b'RomLinkedSample'}
SfModIndexDescs = {0: b'No Controller',
    2: b'Note-On Velocity',
    3: b'Note-On Key Number',
    10: b'Poly Pressure',
    13: b'Channel Pressure',
    14: b'Pitch Wheel',
    16: b'Pitch Wheel Sensitivity'}
SfSampleModes = {0: b'0_LoopNone',
    1: b'1_LoopContinuous',
    2: b'2_LoopReserved',
    3: b'3_LoopRelease'}
SfModTypeDescs = (b'Linear', b'Concave', b'Convex', b'Switch')
SHMIN = -32768
SHOOBVAL = -32769

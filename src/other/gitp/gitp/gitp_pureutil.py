# Ben Fisher
# Copyright (c) 2021, MIT License

import zipfile
import re
try:
    from shinerainsoftsevenutil.standard import *
    from shinerainsoftsevenutil.core import *
except ImportError:
    print('Please install shinerainsevenlib; python -m pip install shinerainsevenlib')

class GitPacketException(RuntimeError):
    pass

def assertGitPacket(condition, *msgs):
    if not condition:
        raise GitPacketException(' '.join((str(msg) for msg in msgs)))

def shasMatch(sh1, sh2):
    sh1 = sh1.strip()
    sh2 = sh2.strip()
    shortlen = 7
    assertTrue(len(sh1) >= shortlen)
    assertTrue(len(sh2) >= shortlen)
    assertTrue(re.match(r'^[a-zA-Z0-9]+$', sh1))
    assertTrue(re.match(r'^[a-zA-Z0-9]+$', sh2))
    if len(sh1) > len(sh2):
        return sh1[0:len(sh2)] == sh2
    elif len(sh2) > len(sh1):
        return sh2[0:len(sh1)] == sh1
    else:
        return sh1 == sh2

def isOkForFilename(s):
    return re.match(r"^[a-zA-Z0-9!@#$%^&()`~+=',; _-]+$", s)

def getLatestProjCountImpl(proj, shortdesc, filelist):
    # want the highest one: if there is a 003 but no 002, still return 004
    # projname_004_desc.gitp.zip
    highestSeen = 0
    for short in filelist:
        if short.startswith(proj + '_') and short.endswith('.gitp.zip'):
            sNum = short.split('_')[1].split('.')[0]
            num = int(sNum, 10)
            highestSeen = max(highestSeen, num)
    
    sNum = '%03d' % (highestSeen + 1)
    sDesc = '_' + shortdesc if shortdesc else ''
    return f'{proj}_{sNum}{sDesc}.gitp.zip'

def withoutNumber(s):
    lst = s.split('.')
    assertTrue(len(lst) >= 3, s)
    assertTrue(len(lst[-2]) == 3, 'expected .001. in string', s)
    lst.pop(-2)
    return '.'.join(lst)

def transformPath(s, suffix):
    withsuffix = files.splitExt(s)[0] + '.' + suffix + files.splitExt(s)[1]
    return withsuffix.replace('\\', ';').replace('/', ';')

def compareTwoFiles(f1, f2):
    # in the future, might add logic where whitespace-only changes are ok
    txt1 = files.readAll(f1, encoding='latin-1').replace('\r\n', '\n').strip()
    txt2 = files.readAll(f2, encoding='latin-1').replace('\r\n', '\n').strip()
    return txt1 == txt2

def compareWithFileInZip(zip, path, suffix):
    import io
    txt1 = files.readAll(path, encoding='latin-1').replace('\r\n', '\n').strip()
    innerName = transformPath(path, suffix)
    if not innerName in zip.namelist():
        trace(f'zip file doesn\'t contain {innerName}')
        return False
    else:
        with zip.open(innerName) as f:
            fWrapped = io.TextIOWrapper(f, 'latin-1')
            txt2 = fWrapped.read().replace('\r\n', '\n').strip()
        return txt1 == txt2

def splitBytesBySize(b, size):
    ints = [int(c) for c in b]
    ret = takeBatch(ints, size)
    return jslike.map(ret, lambda elem: bytes(elem))

def tests():
    # shasMatch
    assertTrue(shasMatch('7ffaef0000f4f1938528c6732886b23c72c9f9a2', '7ffaef0000f4f1938528c6732886b23c72c9f9a2'))
    assertTrue(not shasMatch('7ffaef0000f4f1938528c6732886b23c72c9f9a2', '7ffaef0000f4f1938528c6732886b23c72c9f9a3'))
    assertTrue(shasMatch('7ffaef0000f4f1938528c6732886b23c72c9f9a2', '7ffaef0000f4f1938528c6732886b23c72c9f9a'))
    assertTrue(not shasMatch('7ffaef0000f4f1938528c6732886b23c72c9f9a2', '7ffaef0001'))
    assertTrue(shasMatch('7ffaef0000f4f1938528c6732886b23c72c9f9a2', '7ffaef0000'))
    assertTrue(shasMatch('7ffaef0000f4f1938528c6732886b23c72c9f9a2', '7ffaef0000f4f1938528c6732886b23c72c9f9a2'))
    assertTrue(not shasMatch('7ffaef0000f4f1938528c6732886b23c72c9f9a3', '7ffaef0000f4f1938528c6732886b23c72c9f9a2'))
    assertTrue(shasMatch('7ffaef0000f4f1938528c6732886b23c72c9f9a', '7ffaef0000f4f1938528c6732886b23c72c9f9a2'))
    assertTrue(not shasMatch('7ffaef0001', '7ffaef0000f4f1938528c6732886b23c72c9f9a2'))
    assertTrue(shasMatch('7ffaef0000', '7ffaef0000f4f1938528c6732886b23c72c9f9a2'))
    
    # isOkForFilename
    assertTrue(not isOkForFilename(''))
    assertTrue(isOkForFilename(' '))
    assertTrue(isOkForFilename('abc'))
    assertTrue(isOkForFilename('a_b_c'))
    assertTrue(not isOkForFilename('a*c'))
    assertTrue(not isOkForFilename('*'))

    # without a description
    filelist = []
    assertEq('myproj_001.gitp.zip', getLatestProjCountImpl('myproj', '', filelist))
    filelist = 'myproj_001.gitp.zip'.split(',')
    assertEq('myproj_002.gitp.zip', getLatestProjCountImpl('myproj', '', filelist))
    filelist = 'myproj_001.gitp.zip,myproj_002.gitp.zip'.split(',')
    assertEq('myproj_003.gitp.zip', getLatestProjCountImpl('myproj', '', filelist))
    filelist = 'myproj_002.gitp.zip,myproj_003.gitp.zip'.split(',')
    assertEq('myproj_004.gitp.zip', getLatestProjCountImpl('myproj', '', filelist))
    filelist = 'myproj_002.gitp.zip,myproj_003.gitp.zip,other_004.gitp.zip'.split(',')
    assertEq('myproj_004.gitp.zip', getLatestProjCountImpl('myproj', '', filelist))

    # with a description
    filelist = []
    assertEq('myproj_001_desc.gitp.zip', getLatestProjCountImpl('myproj', 'desc', filelist))
    filelist = 'myproj_001_d1.gitp.zip'.split(',')
    assertEq('myproj_002_desc.gitp.zip', getLatestProjCountImpl('myproj', 'desc', filelist))
    filelist = 'myproj_001_d.2.gitp.zip,myproj_002_d.2.gitp.zip'.split(',')
    assertEq('myproj_003_desc.gitp.zip', getLatestProjCountImpl('myproj', 'desc', filelist))
    filelist = 'myproj_002.gitp.zip,myproj_003_d3.gitp.zip'.split(',')
    assertEq('myproj_004_desc.gitp.zip', getLatestProjCountImpl('myproj', 'desc', filelist))
    filelist = 'myproj_002.gitp.zip,myproj_003.gitp.zip,other_004.gitp.zip'.split(',')
    assertEq('myproj_004_desc.1.gitp.zip', getLatestProjCountImpl('myproj', 'desc.1', filelist))

    # withoutNumber
    assertEq('abc.ts', withoutNumber('abc.001.ts'))
    assertEq('abc.def.ts', withoutNumber('abc.def.001.ts'))
    assertException(lambda: withoutNumber('abcdef.01.ts'), AssertionError)

    # transformPath
    assertEq('ef.test.001.txt', transformPath('ef.test.txt', '001'))
    assertEq('ab;cd;ef.001.txt', transformPath('ab/cd/ef.txt', '001'))
    assertEq('ab;cd;ef.test.001.txt', transformPath('ab/cd/ef.test.txt', '001'))

    # compareTwoFiles
    compareTwoFilesTests()

    # splitBytesBySize
    assertEq([b'123',b'456',b'789'], splitBytesBySize(b'123456789', 3))
    assertEq([b'123456789'], splitBytesBySize(b'123456789', 11))
    assertEq([b'123456789'], splitBytesBySize(b'123456789', 10))
    assertEq([b'123456789'], splitBytesBySize(b'123456789', 9))
    assertEq([b'12345678', b'9'], splitBytesBySize(b'123456789', 8))
    assertEq([b'12345', b'6789'], splitBytesBySize(b'123456789', 5))
    
    trace('tests complete')

def compareTwoFilesTests():
    testSets = [
        [b'', b'', True],
        [b'', b'a', False],
        [b'', b' ', True],
        [b'abc def', b'abc def', True],
        [b'abc def', b'abc def ', True],
        [b'abc def', b'abcdef', False],
        [b'abc def', b'abc Def', False],
        [b'abc def', b'abc  def', False],
        [b'abc\ndef\nghi', b'abc\ndef\nghi', True],
        [b'abc\ndef\nghi', b'abc\r\ndef\r\nghi', True],
        [b'abc\ndef\nghi', b'abc\r\ndef\r\nGhi', False],
        [b'abc\0def\nghi', b'abc\0def\nghi', True],
        [b'abc\0def\nghi', b'abc\0def\r\nghi', True],
        [b'abc\0def\nghi', b'abc\0def\r\nGhi', False],
        [b'abc$defghi', b'abc$defghi', True],
        [b'abc$defghi', b'abc$defGhi', False],
        [b'abc$defghi', b'abc^defghi', False],
    ]
    
    def replaceSymbols(b):
        return b.replace(b'$', bytes([240, 241, 242])).replace(b'^', bytes([240, 241, 243]))

    for b1, b2, expected in testSets:
        files.writeAll('tmp1.test', replaceSymbols(b1), 'wb')
        files.writeAll('tmp2.test', replaceSymbols(b2), 'wb')
        assertEq(expected, compareTwoFiles('tmp1.test', 'tmp2.test'))
        assertEq(expected, compareTwoFiles('tmp2.test', 'tmp1.test'))
        with zipfile.ZipFile('tmp.zip', 'w') as zip:
            zip.write('tmp2.test', arcname='tmp1.001.test')
        with zipfile.ZipFile('tmp.zip') as zip:
            assertEq(expected, compareWithFileInZip(zip, 'tmp1.test', '001'))
        with zipfile.ZipFile('tmp.zip', 'w') as zip:
            zip.write('tmp1.test', arcname='tmp2.001.test')
        with zipfile.ZipFile('tmp.zip') as zip:
            assertEq(expected, compareWithFileInZip(zip, 'tmp2.test', '001'))
        files.delete('tmp.zip')
        files.delete('tmp1.test')
        files.delete('tmp2.test')

if __name__ == '__main__':
    tests()


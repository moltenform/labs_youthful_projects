

import os
import pytest
from ..files import join
from ..common_ui import *
from .test_files import fixture_dir as fixture_dir_
fixture_dir = fixture_dir_

class TestCommonHigher(object):
    # getRandomString
    def test_getRandomString(self):
        s1 = getRandomString()
        assert all((c in '0123456789' for c in s1))
        s2 = getRandomString()
        assert all((c in '0123456789' for c in s2))
        assert s1 != s2

    # genGuid
    def test_genGuid(self):
        s1 = genGuid()
        assert 36 == len(s1)
        s2 = genGuid()
        assert 36 == len(s2)
        assert s1 != s2

    # genGuid, as base 64
    def test_genGuidBase64(self):
        s1 = genGuid(asBase64=True)
        assert 24 == len(s1)
        s2 = genGuid(asBase64=True)
        assert 24 == len(s2)
        assert s1 != s2

    # getClipboardText
    def test_getClipboardTextWithNoUnicode(self):
        # let's check that pyperclip is installed
        import pyperclip  # NOQA
        prev = getClipboardText()
        try:
            setClipboardText('normal ascii')
            assert 'normal ascii' == getClipboardText()
        finally:
            setClipboardText(prev)

    def test_getClipboardTextWithUnicode(self):
        # let's check that pyperclip is installed
        import pyperclip  # NOQA
        prev = getClipboardText()
        try:
            setClipboardText(u'\u1E31\u1E77\u1E53\u006E')
            assert u'\u1E31\u1E77\u1E53\u006E' == getClipboardText()
        finally:
            setClipboardText(prev)

@pytest.mark.skipif('not isPy3OrNewer')
class TestPersistedDict(object):
    def test_noPersistYet(self, fixture_dir):
        # won't persist since 5 things haven't been written
        path = join(fixture_dir, 'test.json')
        obj = PersistedDict(path, warnIfCreatingNew=False, persistEveryNWrites=5)
        obj.set('key1', 'val1')
        assert 1 == len(obj.data)
        objRead = PersistedDict(path)
        assert 0 == len(objRead.data)

    def test_willPersist(self, fixture_dir):
        path = join(fixture_dir, 'test.json')
        obj = PersistedDict(path, warnIfCreatingNew=False, persistEveryNWrites=5)
        for i in range(6):
            obj.set('key%d' % i, 'val%d' % i)
        assertEq(6, len(obj.data))
        objRead = PersistedDict(path)
        assertEq(5, len(objRead.data))
        for i in range(5):
            assert 'val%d' % i == objRead.data['key%d' % i]

    def test_canLoadEmpty(self, fixture_dir):
        path = join(fixture_dir, 'test.json')
        obj = PersistedDict(path, warnIfCreatingNew=False, persistEveryNWrites=5)
        obj.persist()
        assertTrue(files.isfile(path))
        objRead = PersistedDict(path, warnIfCreatingNew=True)
        assert 0 == len(objRead.data)

    def test_canWriteUnicode(self, fixture_dir):
        path = join(fixture_dir, u'test\u1101.json')
        obj = PersistedDict(path, warnIfCreatingNew=False, persistEveryNWrites=1)
        obj.set('key\u1101', '\u1101val')
        objRead = PersistedDict(path, warnIfCreatingNew=True)
        assert 1 == len(objRead.data)
        assert '\u1101val' == objRead.data['key\u1101']

    def test_canWriteDataTypes(self, fixture_dir):
        path = join(fixture_dir, u'test.json')
        obj = PersistedDict(path, warnIfCreatingNew=False, persistEveryNWrites=1)
        obj.set('testInt', 123)
        obj.set('testString', 'abc')
        obj.set('testBool', True)
        obj.set('testList', [12, 34, 56])
        obj.set('testFloat', 1.2345)
        objRead = PersistedDict(path, warnIfCreatingNew=True)
        assert 123 == objRead.data['testInt']
        assert 'abc' == objRead.data['testString']
        assert True is objRead.data['testBool']
        assert [12, 34, 56] == objRead.data['testList']
        assertFloatEq(1.2345, objRead.data['testFloat'])

    def test_canWriteDataTypesAsKeys(self, fixture_dir):
        path = join(fixture_dir, u'test.json')
        obj = PersistedDict(path, warnIfCreatingNew=False, persistEveryNWrites=1)
        obj.set(123, 'testInt')
        obj.set('abc', 'testString')
        obj.set(True, 'testBool')
        objRead = PersistedDict(path, warnIfCreatingNew=True)
        assert 'testInt' == objRead.data['123']
        assert 'testString' == objRead.data['abc']
        assert 'testBool' == objRead.data['true']

    def test_twoDeep(self, fixture_dir):
        path = join(fixture_dir, u'test.json')
        obj = PersistedDict(path, warnIfCreatingNew=False, persistEveryNWrites=1)
        obj.set('rkey1', {})
        obj.set('rkey2', {})
        obj.setSubdict('rkey1', 'k1', 'v11')
        obj.setSubdict('rkey1', 'k2', 'v12')
        obj.setSubdict('rkey2', 'k1', 'v21')
        obj.setSubdict('rkey2', 'k2', 'v22')
        objRead = PersistedDict(path, warnIfCreatingNew=True)
        assert 2 == len(objRead.data)
        assert 'v11' == objRead.data['rkey1']['k1']
        assert 'v12' == objRead.data['rkey1']['k2']
        assert 'v21' == objRead.data['rkey2']['k1']
        assert 'v22' == objRead.data['rkey2']['k2']

    def test_threeDeep(self, fixture_dir):
        path = join(fixture_dir, u'test.json')
        obj = PersistedDict(path, warnIfCreatingNew=False, persistEveryNWrites=1)
        obj.set('rrkey1', {})
        obj.set('rrkey2', {})
        obj.setSubdict('rrkey1', 'rkey1', {})
        obj.setSubdict('rrkey1', 'rkey2', {})
        obj.setSubdict('rrkey2', 'rkey1', {})
        obj.setSubdict('rrkey2', 'rkey2', {})
        obj.setSubsubdict('rrkey1', 'rkey2', 'rkeya', 'va')
        obj.setSubsubdict('rrkey2', 'rkey2', 'rkeyb', 'vb')
        objRead = PersistedDict(path, warnIfCreatingNew=True)
        assert 2 == len(objRead.data)
        assert {} == objRead.data['rrkey1']['rkey1']
        assert 'va' == objRead.data['rrkey1']['rkey2']['rkeya']
        assert {} == objRead.data['rrkey2']['rkey1']
        assert 'vb' == objRead.data['rrkey2']['rkey2']['rkeyb']

    def test_keepHandle(self, fixture_dir):
        path = join(fixture_dir, 'test.json')
        obj = PersistedDict(path, warnIfCreatingNew=False, persistEveryNWrites=1, keepHandle=True)
        for i in range(6):
            obj.set('key%d' % i, 'val%d' % i)
        assertEq(6, len(obj.data))
        obj.close()
        del obj
        objRead = PersistedDict(path)
        assertEq(6, len(objRead.data))
        for i in range(6):
            assert 'val%d' % i == objRead.data['key%d' % i]

class TestParsePlus(object):
    def runBasicParse(self, s, pattern):
        parser = ParsePlus(pattern)
        return parser.match(s)

    def test_basic(self):
        found = self.runBasicParse(r'<p>abc</p>', r'<p>{content}</p>')
        assert found.content == 'abc'

    def test_mustMatchEntire1(self):
        found = self.runBasicParse(r'a<p>abc</p>', r'<p>{content}</p>')
        assert found is None

    def test_mustMatchEntire2(self):
        found = self.runBasicParse(r'<p>abc</p>a', r'<p>{content}</p>')
        assert found is None

    def test_mustMatchEntire3(self):
        found = self.runBasicParse(r'a<p>abc</p>a', r'<p>{content}</p>')
        assert found is None

    def test_mustMatchEntire4(self):
        found = self.runBasicParse(r'a\n<p>abc</p>', r'<p>{content}</p>')
        assert found is None

    def test_mustMatchEntire5(self):
        found = self.runBasicParse(r'<p>abc</p>\na', r'<p>{content}</p>')
        assert found is None

    def test_mustMatchEntire6(self):
        found = self.runBasicParse(r'a\n<p>abc</p>\na', r'<p>{content}</p>')
        assert found is None

    def test_shouldEscapeBackslash(self):
        found = self.runBasicParse(r'<p>abc</p>a\b', r'<p>{content}</p>a\b')
        assert found.content == 'abc'

    def test_shouldEscapeSymbols(self):
        found = self.runBasicParse(r'<p>abc</p>a??**)b', r'<p>{content}</p>a??**)b')
        assert found.content == 'abc'

    def test_shouldEscapeDotStar(self):
        found = self.runBasicParse(r'<p>abc</p>a.*?', r'<p>{content}</p>a.*?')
        assert found.content == 'abc'

    def test_ignoreDoubleBracketsOpen1(self):
        found = self.runBasicParse(r'{<p>abc</p>', r'{{<p>{content}</p>')
        assert found.content == 'abc'

    def test_ignoreDoubleBracketsOpen2(self):
        found = self.runBasicParse(r'<p>a{bc</p>', r'<p>{content}</p>')
        assert found.content == 'a{bc'

    def test_ignoreDoubleBracketsOpen3(self):
        found = self.runBasicParse(r'<p>abc</p>{', r'<p>{content}</p>{{')
        assert found.content == 'abc'

    def test_ignoreDoubleBracketsClose1(self):
        found = self.runBasicParse(r'}<p>abc</p>', r'}}<p>{content}</p>')
        assert found.content == 'abc'

    def test_ignoreDoubleBracketsClose2(self):
        found = self.runBasicParse(r'<p>a}bc</p>', r'<p>{content}</p>')
        assert found.content == 'a}bc'

    def test_ignoreDoubleBracketsClose3(self):
        found = self.runBasicParse(r'<p>abc</p>}', r'<p>{content}</p>}}')
        assert found.content == 'abc'

    def test_ignoreDoubleBracketsBoth1(self):
        found = self.runBasicParse(r'{}<p>abc</p>', r'{{}}<p>{content}</p>')
        assert found.content == 'abc'

    def test_ignoreDoubleBracketsBoth2(self):
        found = self.runBasicParse(r'<p>a{}bc</p>', r'<p>{content}</p>')
        assert found.content == 'a{}bc'

    def test_ignoreDoubleBracketsBoth3(self):
        found = self.runBasicParse(r'<p>abc</p>{}', r'<p>{content}</p>{{}}')
        assert found.content == 'abc'

    def test_ignoreDoubleBracketsBothInside1(self):
        found = self.runBasicParse(r'1{<p>abc</p>}1', r'1{{<p>{content}</p>}}1')
        assert found.content == 'abc'

    def test_ignoreDoubleBracketsBothInside2(self):
        found = self.runBasicParse(r'{<p>abc</p>}', r'{{<p>{content}</p>}}')
        assert found.content == 'abc'

    def test_emptyNameIsOk1(self):
        found = self.runBasicParse(r'456|ABC|123', r'{}|{main}|123')
        assert found.main == 'ABC'

    def test_emptyNamesIsOk2(self):
        found = self.runBasicParse(r'456|ABC|123', r'{}|{main}|{}')
        assert found.main == 'ABC'

    def test_nameMustBeAlphanum1(self):
        found = self.runBasicParse(r'456|ABC|123', r'{}|{bad name}|{}')
        assert found is None

    def test_nameMustBeAlphanum2(self):
        found = self.runBasicParse(r'456|ABC|123', r'{}|{bad)name}|{}')
        assert found is None

    def test_nameMustBeAlphanum3(self):
        found = self.runBasicParse(r'456|ABC|123', r'{}|{bad>name}|{}')
        assert found is None

    def test_nameCanHaveUnderscore(self):
        found = self.runBasicParse(r'456|ABC|123', r'{}|{good_name}|{}')
        assert found.good_name == 'ABC'

    def test_canNoteReturnEmpty(self):
        found = self.runBasicParse(r'456||123', r'{}|{fld}|{}')
        assert found is None

    def test_unnamedCanNotBeEmpty(self):
        found = self.runBasicParse(r'|a|', r'{}|{fld}|{}')
        assert found is None

    def test_hasNewline(self):
        found = self.runBasicParse('456|a\nb|123', r'{}|{fld}|{}')
        assert found.fld == 'a\nb'

    def test_hasWindowsNewline(self):
        found = self.runBasicParse('456|a\r\nb|123', r'{}|{fld}|{}')
        assert found.fld == 'a\r\nb'

    def test_hasNewlineRestricted(self):
        found = self.runBasicParse('456|a\nb|123', r'{}|{ss:NoNewlines}|{}')
        assert found is None

    def test_hasWindowsNewlineRestricted(self):
        found = self.runBasicParse('456|a\nb|123', r'{}|{ss:NoNewlines}|{}')
        assert found is None

    def test_hasSpaces(self):
        found = self.runBasicParse('456|a  b|123', r'{}|{ss}|{}')
        assert found.ss == 'a  b'

    def test_hasSpacesRestricted(self):
        found = self.runBasicParse('456|a  b|123', r'{}|{ss:NoSpaces}|{}')
        assert found is None

    def test_multipleFields2(self):
        found = self.runBasicParse(r'a|b', r'{c1}|{c2}')
        assert found.c1 == 'a'
        assert found.c2 == 'b'

    def test_multipleFields3(self):
        found = self.runBasicParse(r'a|b|c', r'{c1}|{c2}|{c3}')
        assert found.c1 == 'a'
        assert found.c2 == 'b'
        assert found.c3 == 'c'

    def test_multipleFields4(self):
        found = self.runBasicParse(r'a|b|c|d', r'{c1}|{c2}|{c3}|{c4}')
        assert found.c1 == 'a'
        assert found.c2 == 'b'
        assert found.c3 == 'c'
        assert found.c4 == 'd'

    def test_multipleFields5(self):
        found = self.runBasicParse(r'aa|bb|cc|dd', r'{c1}|{c2}|{c3}|{c4}')
        assert found.c1 == 'aa'
        assert found.c2 == 'bb'
        assert found.c3 == 'cc'
        assert found.c4 == 'dd'

    def test_multipleFieldsNotEnough(self):
        found = self.runBasicParse(r'a|b|c', r'{c1}|{c2}|{c3}|{c4}')
        assert found is None

    def test_multipleFieldsDemo(self):
        found = self.runBasicParse(r'<first>ff</first><second>ss</second>',
            r'<first>{c1}</first><second>{c2}</second>')
        assert found.c1 == 'ff'
        assert found.c2 == 'ss'

    def test_replaceTextFailsIfNotExists(self, fixture_dir):
        path = files.join(fixture_dir, 'testreplace.txt')
        contentsFail = '<tag> Target</b> </tag>'
        files.writeall(path, contentsFail)
        with pytest.raises(RuntimeError) as exc:
            ParsePlus('<b>{s}</b>').replaceFieldWithTextIntoFile(path,
                's', 'o')
        exc.match('pattern not found')

    def test_replaceTextFailsIfMultiple(self, fixture_dir):
        path = files.join(fixture_dir, 'testreplace.txt')
        contentsFail = '<tag> <b>Target</b> <b>Other</b></tag>'
        files.writeall(path, contentsFail)
        with pytest.raises(RuntimeError) as exc:
            ParsePlus('<b>{s}</b>').replaceFieldWithTextIntoFile(path,
                's', 'o', allowOnlyOnce=True)
        exc.match('only once')

    def test_replaceTextFailsIfMultipleOpen(self, fixture_dir):
        path = files.join(fixture_dir, 'testreplace.txt')
        contentsFail = '<tag> <b>Target</b> <b>Other</tag></b>'
        files.writeall(path, contentsFail)
        with pytest.raises(RuntimeError) as exc:
            ParsePlus('<b>{s}</b>').replaceFieldWithTextIntoFile(path,
                's', 'o', allowOnlyOnce=True)
        exc.match('only once')

    def test_replaceTextFailsIfMultipleClose(self, fixture_dir):
        path = files.join(fixture_dir, 'testreplace.txt')
        contentsFail = '<tag> <b>Target</b> Othe<b>r</b></tag>'
        files.writeall(path, contentsFail)
        with pytest.raises(RuntimeError) as exc:
            ParsePlus('<b>{s}</b>').replaceFieldWithTextIntoFile(path,
                's', 'o', allowOnlyOnce=True)
        exc.match('only once')

    def test_replaceTextAppendsIfNotExists(self, fixture_dir):
        path = files.join(fixture_dir, 'testreplace.txt')
        contents = '<tag>Target</b> </tag>'
        files.writeall(path, contents)
        ParsePlus('<b>{s}</b>').replaceFieldWithTextIntoFile(path,
            's', 'o', appendIfNotFound=':append:')

        newContents = files.readall(path)
        assert newContents == '<tag>Target</b> </tag>:append:'

    def test_replaceTextSucceeds(self, fixture_dir):
        path = files.join(fixture_dir, 'testreplace.txt')
        contents = '<tag> <b>Target</b> </tag>'
        files.writeall(path, contents)
        ParsePlus('<b>{s}</b>').replaceFieldWithTextIntoFile(path,
            's', 'out', appendIfNotFound=':append:')
        newContents = files.readall(path)
        assert newContents == '<tag> <b>out</b> </tag>'

    def test_replaceTextSucceedsManyClosers(self, fixture_dir):
        path = files.join(fixture_dir, 'testreplace.txt')
        contents = '<tag> <b>Target</b> and</b></tag>'
        files.writeall(path, contents)
        ParsePlus('<b>{s}</b>').replaceFieldWithTextIntoFile(path, 's', 'o')
        newContents = files.readall(path)
        assert newContents == '<tag> <b>o</b> and</b></tag>'

    def test_replaceTextSucceedsManyBoth(self, fixture_dir):
        path = files.join(fixture_dir, 'testreplace.txt')
        contents = '<tag> <b>Target</b> <b>other</b></tag>'
        files.writeall(path, contents)
        ParsePlus('<b>{s}</b>').replaceFieldWithTextIntoFile(path, 's', 'o')
        newContents = files.readall(path)
        assert newContents == '<tag> <b>o</b> <b>other</b></tag>'

    def test_replaceTextSucceedsLonger(self, fixture_dir):
        path = files.join(fixture_dir, 'testreplace.txt')
        contents = '<tag> <look>LongerTextIsHere</look> </tag>'
        files.writeall(path, contents)
        ParsePlus('<look>{s}</look>').replaceFieldWithTextIntoFile(path,
            's', 'o')
        newContents = files.readall(path)
        assert newContents == '<tag> <look>o</look> </tag>'

    def test_isCaseSensitive(self):
        found = ParsePlus('aa {s} bb').match('aA 123 bB')
        assert found is None

    def test_isNotCaseSensitive(self):
        found = ParsePlus('aa {s} bb', case_sensitive=False).match('aA 123 bB')
        assert found.s == '123'

    def parseCsvWithThreeFields(self, s):
        if not s.endswith('\n'):
            s += '\n'
        p = ParsePlus('1{f1:NoNewlines},{f2:NoNewlines},{f3:NoNewlines}\n',
            escapeSequences=['\\,', '\\\n'])
        return list(p.findall(s))

    def test_findAllNotFound(self):
        found = self.parseCsvWithThreeFields('1a,b\n1c,d')
        assert len(found) == 0

    def test_findFalsePositive(self):
        # currently it detects it as one long string
        # I might consider finding a more elegant way around this sometime
        found = self.parseCsvWithThreeFields('1a,b,c|d,e,f|g,h,i')
        assert len(found) == 1

    def test_findFindFull(self):
        sInput = '1aa,bb,cc\n'
        found = self.parseCsvWithThreeFields(sInput)
        assert len(found) == 1
        assert found[0].f1 == 'aa'
        assert found[0].f2 == 'bb'
        assert found[0].f3 == 'cc'
        assert found[0].spans['f1'] == (1, 3)
        assert found[0].spans['f2'] == (4, 6)
        assert found[0].spans['f3'] == (7, 9)
        assert found[0].getTotalSpan() == (0, 10)
        assert sInput[1:3] == 'aa'
        assert sInput[4:6] == 'bb'
        assert sInput[7:9] == 'cc'
        assert sInput[0:10] == sInput

    def test_findFindOne(self):
        sInput = 'AAA1aa,bb,cc\nZZ'
        found = self.parseCsvWithThreeFields(sInput)
        assert len(found) == 1
        assert found[0].f1 == 'aa'
        assert found[0].f2 == 'bb'
        assert found[0].f3 == 'cc'
        assert found[0].spans['f1'] == (3 + 1, 3 + 3)
        assert found[0].spans['f2'] == (3 + 4, 3 + 6)
        assert found[0].spans['f3'] == (3 + 7, 3 + 9)
        assert found[0].getTotalSpan() == (3 + 0, 3 + 10)
        assert sInput[3 + 0: 3 + 10] == '1aa,bb,cc\n'

    def test_findFindThree(self):
        sInput = 'AAA1aa,bb,cc\n1ddd,eee,ff\n1gg,hh,iii\nZ'
        found = self.parseCsvWithThreeFields(sInput)
        assert len(found) == 3
        assert found[0].f1 == 'aa'
        assert found[0].f2 == 'bb'
        assert found[0].f3 == 'cc'
        assert found[0].spans['f1'] == (3 + 1, 3 + 3)
        assert found[0].spans['f2'] == (3 + 4, 3 + 6)
        assert found[0].spans['f3'] == (3 + 7, 3 + 9)
        assert found[0].getTotalSpan() == (3 + 0, 3 + 10)
        assert sInput[3 + 0: 3 + 10] == '1aa,bb,cc\n'
        assert found[1].f1 == 'ddd'
        assert found[1].f2 == 'eee'
        assert found[1].f3 == 'ff'
        assert found[1].spans['f1'] == (13 + 1, 13 + 4)
        assert found[1].spans['f2'] == (13 + 5, 13 + 8)
        assert found[1].spans['f3'] == (13 + 9, 13 + 11)
        assert found[1].getTotalSpan() == (13 + 0, 13 + 12)
        assert sInput[13 + 0: 13 + 12] == '1ddd,eee,ff\n'
        assert found[2].f1 == 'gg'
        assert found[2].f2 == 'hh'
        assert found[2].f3 == 'iii'
        assert found[2].spans['f1'] == (25 + 1, 25 + 3)
        assert found[2].spans['f2'] == (25 + 4, 25 + 6)
        assert found[2].spans['f3'] == (25 + 7, 25 + 10)
        assert found[2].getTotalSpan() == (25 + 0, 25 + 11)
        assert sInput[25 + 0: 25 + 11] == '1gg,hh,iii\n'

    def test_oneEscapeSequence(self):
        sInput = 'AAA1aa,bb,cc\n1dd\\\nd\\\n,eee,ff\nZ'
        found = self.parseCsvWithThreeFields(sInput)
        assert len(found) == 2
        assert found[0].f1 == 'aa'
        assert found[0].f2 == 'bb'
        assert found[0].f3 == 'cc'
        assert found[0].getTotalSpan() == (3, 13)
        assert found[1].f1 == 'dd\\\nd\\\n'
        assert found[1].f2 == 'eee'
        assert found[1].f3 == 'ff'
        assert found[1].getTotalSpan() == (13, 29)

    def test_twoEscapeSequences(self):
        sInput = 'AAA1aa,\\,,cc\n1\\,dd\\\nd\\\n,eee,ff\nZ'
        found = self.parseCsvWithThreeFields(sInput)
        assert len(found) == 2
        assert found[0].f1 == 'aa'
        assert found[0].f2 == '\\,'
        assert found[0].f3 == 'cc'
        assert found[0].getTotalSpan() == (3, 13)
        assert found[1].f1 == '\\,dd\\\nd\\\n'
        assert found[1].f2 == 'eee'
        assert found[1].f3 == 'ff'
        assert found[1].getTotalSpan() == (13, 31)

    def test_inputStringContainsRareChar1(self):
        with pytest.raises(RuntimeError) as exc:
            ParsePlus('{}', escapeSequences=['11', '22']).match('a\x01')
        exc.match('input string contains')

    def test_inputStringContainsRareChar2(self):
        with pytest.raises(RuntimeError) as exc:
            ParsePlus('{}', escapeSequences=['11', '22']).match('a\x02')
        exc.match('input string contains')

    def test_cannotYetSupportLotsOfSequences(self):
        with pytest.raises(ValueError) as exc:
            ParsePlus('{}', escapeSequences=['11', '22', '33', '44', '55', '66']).match('a')
        exc.match('a max of')

    def test_cannotYetSupportGetTotalSpanIfOpenBraces(self):
        with pytest.raises(RuntimeError) as exc:
            ParsePlus('{s}is {{').search('bis {').getTotalSpan()
        exc.match("don't yet support")

    def test_cannotYetSupportGetTotalSpanIfCloseBraces(self):
        with pytest.raises(RuntimeError) as exc:
            ParsePlus('{s}is }}').search('bis }').getTotalSpan()
        exc.match("don't yet support")

class TestCommonUI(object):
    def test_checkIsDigit(self):
        # make sure isdigit behaves as expected
        assert not ''.isdigit()
        assert '0'.isdigit()
        assert '123'.isdigit()
        assert not '123 '.isdigit()
        assert not '123a'.isdigit()
        assert not 'a123'.isdigit()

    def test_findUnusedLetterMaintainsUsedLetterState(self):
        d = dict()
        assert 0 == findUnusedLetter(d, 'abc')
        assert 1 == findUnusedLetter(d, 'abc')
        assert 2 == findUnusedLetter(d, 'abc')
        assert None is findUnusedLetter(d, 'abc')
        assert None is findUnusedLetter(d, 'ABC')
        assert None is findUnusedLetter(d, 'a b c!@#')

    def test_softDeleteFileShouldMakeFileNotExist(self, fixture_dir):
        path = join(fixture_dir, 'testdelfile1.txt')
        files.writeall(path, 'contents')
        assert os.path.exists(path)
        newlocation = softDeleteFile(path)
        assert not os.path.exists(path)
        assert os.path.exists(newlocation)

    def test_softDeleteFileShouldRenameFirstCharOfFile(self, fixture_dir):
        path = join(fixture_dir, 'zzzz', 'testdelfile2.txt')
        files.makedirs(files.getparent(path))
        files.writeall(path, 'contents')
        newlocation = softDeleteFile(path)
        assert os.path.exists(newlocation)
        assert files.getname(newlocation).startswith('z')

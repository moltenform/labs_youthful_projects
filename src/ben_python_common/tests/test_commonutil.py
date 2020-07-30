# BenPythonCommon,
# 2015 Ben Fisher, released under the GPLv3 license.

import pytest
from os.path import join
from collections import OrderedDict
from .. import files
from ..common_ui import *
from .test_files import fixture_dir as fixture_dir_
fixture_dir = fixture_dir_

class TestBucket(object):
    def test_bucket(self):
        a = Bucket()
        a.f1 = 'abc'
        assert a.f1 == 'abc'
        a.f1 = 'def'
        assert a.f1 == 'def'
        a.f2 = 'abc'
        assert a.f1 == 'def'
        assert a.f2 == 'abc'

    def test_bucketCtor(self):
        a = Bucket(f1='start1', f2='start2')
        a.f3 = 'start3'
        assert a.f1 == 'start1'
        assert a.f2 == 'start2'
        assert a.f3 == 'start3'
        
    def test_bucketRepr(self):
        a = Bucket()
        a.f1 = 'abc'
        a.f2 = 'def'
        assert 'f1=abc\n\n\nf2=def' == repr(a)
    
class TestSimpleEnum(object):
    def test_simpleEnum(self):
        a = SimpleEnum(['item1', 'item2'])
        assert a.item1 == 'item1'
        assert a.item2 == 'item2'
        with pytest.raises(AttributeError):
            a.item3
        with pytest.raises(RuntimeError):
            a.item3 = 1
        with pytest.raises(RuntimeError):
            del a.item1

    def test_simpleEnumUnderscore(self):
        a = SimpleEnum(['item1', 'item2'])
        a._other = 1
        assert 1 == a._other

    def test_simpleEnumShouldNotAcceptString(self):
        with pytest.raises(AssertionError):
            SimpleEnum('item1|item2')

class TestStringHelpersSimple(object):
    # getPrintable
    def test_getPrintableEmpty(self):
        assert '' == getPrintable('')

    def test_getPrintableNormalAscii(self):
        assert 'normal ascii' == getPrintable('normal ascii')

    def test_getPrintableNormalUnicode(self):
        assert 'normal unicode' == getPrintable(u'normal unicode')

    def test_getPrintableWithUniChars(self):
        assert 'k?u?o??n' == getPrintable(u'\u1E31\u1E77\u1E53\u006E')

    def test_getPrintableWithUniCompositeSequence(self):
        assert 'k?u?o??n' == getPrintable(u'\u006B\u0301\u0075\u032D\u006F\u0304\u0301\u006E')

    # toValidFilename
    def test_toValidFilenameEmpty(self):
        assert '' == toValidFilename('')

    def test_toValidFilenameAFew(self):
        assert 'a-b c' == toValidFilename('a:b\nc')

    def test_toValidFilenameWindowsNewline(self):
        assert 'a c' == toValidFilename('a\r\nc')
    
    # splice
    def test_splice(self):
        assert 'abef' == splice('abcdef', 2, 2, '')
        assert 'ab1ef' == splice('abcdef', 2, 2, '1')
        assert 'ab123ef' == splice('abcdef', 2, 2, '123')
        assert 'ab123def' == splice('abcdef', 2, 1, '123')
        assert 'ab123cdef' == splice('abcdef', 2, 0, '123')

    # stripHtmlTags
    def test_stripHtmlTagsBasic(self):
        assert 'a b c' == stripHtmlTags('a b c')
        assert '' == stripHtmlTags('')
        assert '' == stripHtmlTags('<a b c>')
        assert '1 2' == stripHtmlTags('1<a b c>2')

    def test_stripHtmlTagsNested(self):
        assert '1 c?2' == stripHtmlTags('1<a <b> c>2')
        assert '1 2' == stripHtmlTags('1<a <b c>2')
        assert '1 c?2' == stripHtmlTags('1<a b> c>2')
        assert '1 c?2' == stripHtmlTags('1<a <b> c>2')

    def test_stripHtmlTagsUnclosed(self):
        assert 'open?' == stripHtmlTags('open>')
        assert 'open? abc' == stripHtmlTags('open> abc')
        assert '?open abc' == stripHtmlTags('>open abc')
        assert '' == stripHtmlTags('<close')
        assert 'abc' == stripHtmlTags('abc<close')
        assert 'abc close?' == stripHtmlTags('abc close<')

    def test_stripHtmlTagsManyTagsRepeatedSpace(self):
        assert 'a b c d e', stripHtmlTags('a b c<abc> d </abc>e')
        assert 'a b c d e', stripHtmlTags('a b c<abc>d</abc>e')
        assert 'a b c d e', stripHtmlTags('a b c<abc><b>d</abc>e')

    # replaceMustExist
    def test_replaceMustExist(self):
        assert 'abc DEF ghi' == replaceMustExist('abc def ghi', 'def', 'DEF')
        assert 'ABC def ABC' == replaceMustExist('abc def abc', 'abc', 'ABC')
        with pytest.raises(AssertionError):
            replaceMustExist('abc def abc', 'abcd', 'ABC')

    # replaceWholeWord
    def test_replaceWholeWord(self):
        assert 'w,n,w other,wantother,w.other' == reReplaceWholeWord('want,n,want other,wantother,want.other', 'want', 'w')

    def test_replaceWholeWordWithPunctation(self):
        assert 'w,n,w other,w??|tother,w.other' == reReplaceWholeWord('w??|t,n,w??|t other,w??|tother,w??|t.other', 'w??|t', 'w')

    def test_replaceWholeWordWithCasing(self):
        assert 'and A fad pineapple A da' == reReplaceWholeWord('and a fad pineapple a da', 'a', 'A')

    # truncateWithEllipsis
    def test_truncateWithEllipsisEmptyString(self):
        assert '' == truncateWithEllipsis('', 2)

    def test_truncateWithEllipsisStringLength1(self):
        assert 'a' == truncateWithEllipsis('a', 2)

    def test_truncateWithEllipsisStringLength2(self):
        assert 'ab' == truncateWithEllipsis('ab', 2)

    def test_truncateWithEllipsisStringLength3(self):
        assert 'ab' == truncateWithEllipsis('abc', 2)

    def test_truncateWithEllipsisStringLength4(self):
        assert 'ab' == truncateWithEllipsis('abcd', 2)

    def test_truncateWithEllipsisEmptyStringTo4(self):
        assert '' == truncateWithEllipsis('', 4)

    def test_truncateWithEllipsisStringTo4Length1(self):
        assert 'a' == truncateWithEllipsis('a', 4)

    def test_truncateWithEllipsisStringTo4Length2(self):
        assert 'ab' == truncateWithEllipsis('ab', 4)

    def test_truncateWithEllipsisStringTo4Length4(self):
        assert 'abcd' == truncateWithEllipsis('abcd', 4)

    def test_truncateWithEllipsisStringLength5TruncatedTo4(self):
        assert 'a...' == truncateWithEllipsis('abcde', 4)

    def test_truncateWithEllipsisStringLength6TruncatedTo4(self):
        assert 'a...' == truncateWithEllipsis('abcdef', 4)

    # formatSize
    def test_formatSizeGb(self):
        assert '3.00GB' == formatSize(3 * 1024 * 1024 * 1024)
        
    def test_formatSizeGbAndFewBytes(self):
        assert '3.00GB' == formatSize(3 * 1024 * 1024 * 1024 + 123)
    
    def test_formatSizeGbDecimal(self):
        assert '3.12GB' == formatSize(3 * 1024 * 1024 * 1024 + 123 * 1024 * 1024)
    
    def test_formatSizeGbDecimalRound(self):
        assert '3.17GB' == formatSize(3 * 1024 * 1024 * 1024 + 169 * 1024 * 1024)
    
    def test_formatSizeMb(self):
        assert '2.31MB' == formatSize(2 * 1024 * 1024 + 315 * 1024)
    
    def test_formatSizeKb(self):
        assert '1.77KB' == formatSize(1 * 1024 + 789)
    
    def test_formatSizeB(self):
        assert '1000b' == formatSize(1000)
    
    def test_formatSize1000B(self):
        assert '678b' == formatSize(678)
    
    def test_formatSizeZeroB(self):
        assert '0b' == formatSize(0)


class TestDataStructures(object):
    # takeBatch
    def test_takeBatchNonLazy(self):
        assert [[1, 2, 3], [4, 5, 6], [7]] == takeBatch([1, 2, 3, 4, 5, 6, 7], 3)
        assert [[1, 2, 3], [4, 5, 6]] == takeBatch([1, 2, 3, 4, 5, 6], 3)
        assert [[1, 2, 3], [4, 5]] == takeBatch([1, 2, 3, 4, 5], 3)
        assert [[1, 2], [3, 4], [5]] == takeBatch([1, 2, 3, 4, 5], 2)
    
    def test_takeBatchWithCallbackOddNumber(self):
        log = []
        def callback(batch, log=log):
            log.append(list(batch))
        with TakeBatch(2, callback) as obj:
            obj.append(1)
            obj.append(2)
            obj.append(3)
        assert [[1, 2], [3]] == log
    
    def test_takeBatchWithCallbackEvenNumber(self):
        log = []
        def callback(batch, log=log):
            log.append(list(batch))
        with TakeBatch(2, callback) as obj:
            obj.append(1)
            obj.append(2)
            obj.append(3)
            obj.append(4)
        assert [[1, 2], [3, 4]] == log
    
    def test_takeBatchWithCallbackSmallNumber(self):
        log = []
        def callback(batch, log=log):
            log.append(list(batch))
        with TakeBatch(3, callback) as obj:
            obj.append(1)
        assert [[1]] == log
    
    def test_takeBatchWithCallbackDoNotCallOnException(self):
        log = []
        def callback(batch, log=log):
            log.append(list(batch))
        
        # normally, leaving scope of TakeBatch makes final call, but don't if leaving because of exception
        with pytest.raises(IOError):
            with TakeBatch(2, callback) as obj:
                obj.append(1)
                obj.append(2)
                obj.append(3)
                raise IOError()
        assert [[1, 2]] == log
    
    def test_recentlyUsedList_MaxNotExceeded(self):
        mruTest = RecentlyUsedList(maxSize=5)
        mruTest.add('abc')
        mruTest.add('def')
        mruTest.add('ghi')
        assert ['ghi', 'def', 'abc'] == mruTest.getList()

    def test_recentlyUsedList_RedundantEntryMovedToTop(self):
        mruTest = RecentlyUsedList(maxSize=5)
        mruTest.add('aaa')
        mruTest.add('bbb')
        mruTest.add('ccc')
        mruTest.add('bbb')
        assert ['bbb', 'ccc', 'aaa'] == mruTest.getList()
    
    def test_recentlyUsedList_MaxSizePreventsGrowth(self):
        mruTest = RecentlyUsedList(maxSize=2)
        mruTest.add('aaa')
        mruTest.add('bbb')
        mruTest.add('ccc')
        assert ['ccc', 'bbb'] == mruTest.getList()
    
    def test_recentlyUsedList_MaxSizePreventsMoreGrowth(self):
        mruTest = RecentlyUsedList(maxSize=2)
        mruTest.add('aaa')
        mruTest.add('bbb')
        mruTest.add('ccc')
        mruTest.add('ddd')
        assert ['ddd', 'ccc'] == mruTest.getList()

    def test_memoizeCountNumberOfCalls_RepeatedCall(self):
        countCalls = Bucket(count=0)

        @BoundedMemoize
        def addTwoNumbers(a, b, countCalls=countCalls):
            countCalls.count += 1
            return a + b
        assert 20 == addTwoNumbers(10, 10)
        assert 1 == countCalls.count
        assert 20 == addTwoNumbers(10, 10)
        assert 40 == addTwoNumbers(20, 20)
        assert 2 == countCalls.count
    
    def test_memoizeCountNumberOfCalls_InterleavedCall(self):
        countCalls = Bucket(count=0)

        @BoundedMemoize
        def addTwoNumbers(a, b, countCalls=countCalls):
            countCalls.count += 1
            return a + b
        assert 20 == addTwoNumbers(10, 10)
        assert 1 == countCalls.count
        assert 40 == addTwoNumbers(20, 20)
        assert 20 == addTwoNumbers(10, 10)
        assert 2 == countCalls.count

    @pytest.mark.skipif('not isPy3OrNewer')
    def test_modtimeRendered(self, fixture_dir):
        files.writeall(join(fixture_dir, 'a.txt'), 'contents')
        curtimeWritten = files.getModTimeNs(join(fixture_dir, 'a.txt'), asMillisTime=True)
        curtimeNow = getNowAsMillisTime()

        # we expect it to be at least within 1 day
        dayMilliseconds = 24 * 60 * 60 * 1000
        assert abs(curtimeWritten - curtimeNow) < dayMilliseconds

        # so we expect at least the date to match
        nCharsInDate = 10
        scurtimeWritten = renderMillisTime(curtimeWritten)
        scurtimeNow = renderMillisTime(curtimeNow)
        assert scurtimeWritten[0:nCharsInDate] == scurtimeNow[0:nCharsInDate]

    def test_renderTime(self):
        t = getNowAsMillisTime()
        s = renderMillisTime(t)
        assert len(s) > 16

    def test_checkOrderedDictEqualitySame(self):
        d1 = OrderedDict()
        d1['a'] = 1
        d1['b'] = 2
        d1same = OrderedDict()
        d1same['a'] = 1
        d1same['b'] = 2
        assert d1 == d1
        assert d1 == d1same
        assert d1same == d1
    
    def test_checkOrderedDictEqualityDifferentOrder(self):
        d1 = OrderedDict()
        d1['a'] = 1
        d1['b'] = 2
        d2 = OrderedDict()
        d2['b'] = 2
        d2['a'] = 1
        assert d1 != d2
    
    def test_checkOrderedDictEqualityDifferentValues(self):
        d1 = OrderedDict()
        d1['a'] = 1
        d1['b'] = 2
        d2 = OrderedDict()
        d2['a'] = 1
        d2['b'] = 3
        assert d1 != d2


try:
    import dateparser
    hasDateparser = True
except ImportError:
    hasDateparser = False

if hasDateparser:
    class TestDateParsing(object):
        def test_cannotYetSupportGetTotalSpanIfOpenBraces(self):
            def test_spanish_dates_should_not_parsed():
                uu = EnglishDateParserWrapper()
                assert uu.parse(u'Martes 21 de Octubre de 2014') is None
            
            def test_spanish_dates_will_parse_if_we_hack_it_and_give_it_a_different_parser():
                uu = EnglishDateParserWrapper()
                uu.p = dateparser.date.DateDataParser()
                parsed = uu.parse(u'Martes 21 de Octubre de 2014')
                assert 2014 == parsed.year
            
            def test_incomplete_dates_should_not_parsed():
                uu = EnglishDateParserWrapper()
                assert uu.parse(u'December 2015') is None
            
            def test_incomplete_dates_will_parse_if_we_hack_it_and_give_it_a_different_parser():
                uuu = EnglishDateParserWrapper()
                uuu.p = dateparser.date.DateDataParser()
                parsed = uuu.parse(u'December 2015')
                assert 2015 == parsed.year
            
            def test_dates_can_get_this():
                uu = EnglishDateParserWrapper()
                got = uu.parse('30 Jan 2018')
                assert 30 == got.day
                assert 1 == got.month
                assert 2018 == got.year
            
            def test_and_confirm_MDY():
                uu = EnglishDateParserWrapper()
                got = uu.parse('4/5/2016')
                assert 5 == got.day
                assert 4 == got.month
                assert 2016 == got.year
                
                got = uu.parse('18 feb 12')
                assert 18 == got.day
                assert 2 == got.month
                assert 2012 == got.year
                
                got = uu.parse('August 24 2018')
                assert 24 == got.day
                assert 8 == got.month
                assert 2018 == got.year
                
                got = uu.parse('2016-04-11 21:07:47.763957')
                assert 11 == got.day
                assert 4 == got.month
                assert 2016 == got.year
                
                got = uu.parse('Mar 31, 2011 17:41:41 GMT')
                assert 31 == got.day
                assert 3 == got.month
                assert 2011 == got.year
            
            def test_twitter_api_format_we_needed_to_tweak_it_a_bit():
                uu = EnglishDateParserWrapper()
                assert "Wed Nov 07 04:01:10 2018 +0000" == uu.fromFullWithTimezone("Wed Nov 07 04:01:10 +0000 2018")
                got = uu.parse(uu.fromFullWithTimezone("Wed Nov 07 04:01:10 +0000 2018"))
                assert 7 == got.day
                assert 11 == got.month
                assert 2018 == got.year
                
                assert "Wed Nov 07 04:01:10 2018" == uu.fromFullWithTimezone("Wed Nov 07 04:01:10 2018")
                got = uu.parse(uu.fromFullWithTimezone("Wed Nov 07 04:01:10 2018"))
                assert 7 == got.day
                assert 11 == got.month
                assert 2018 == got.year
            
            def test_ensure_month_day_year():
                # 1362456244 is 3(month)/5(day)/2013
                fhhfghfghfgh
                uu = EnglishDateParserWrapper()
                test1 = uu.getDaysBeforeInMilliseconds('3/5/2013 4:04:04 GMT', 0)
                assert 1362456244000 == test1
                test2 = uu.getDaysBeforeInMilliseconds('3/5/2013 4:04:04 GMT', 1)
                assert 1362456244000 - 86400000 == test2
                test3 = uu.getDaysBeforeInMilliseconds('3/5/2013 4:04:04 GMT', 100)
                assert 1362456244000 - 100 * 86400000 == test3
    
class TestCustomAsserts(object):
    def raisevalueerr(self):
        raise ValueError('msg')
    
    # assertException
    def test_assertExceptionExpectsAnyException(self):
        assertException(self.raisevalueerr, None)
    
    def test_assertExceptionExpectsSpecificException(self):
        assertException(self.raisevalueerr, ValueError)
    
    def test_assertExceptionExpectsSpecificExceptionAndMessage(self):
        assertException(self.raisevalueerr, ValueError, 'msg')
    
    def test_assertExceptionFailsIfNoExceptionThrown(self):
        with pytest.raises(AssertionError) as exc:
            assertException(lambda: 1, None)
        exc.match('did not throw')
    
    def test_assertExceptionFailsIfWrongExceptionThrown(self):
        with pytest.raises(AssertionError) as exc:
            assertException(self.raisevalueerr, ValueError, 'notmsg')
        exc.match('exception string check failed')
    
    # assertTrue
    def test_assertTrueExpectsTrue(self):
        assertTrue(True)
        assertTrue(1)
        assertTrue('string')
    
    def test_assertTrueFailsIfFalse(self):
        with pytest.raises(AssertionError):
            assertTrue(False)
    
    def test_assertTrueFailsIfFalseWithMessage(self):
        with pytest.raises(AssertionError) as exc:
            assertTrue(False, 'custom msg')
        exc.match('custom msg')
    
    # assertEq
    def test_assertEq(self):
        assertEq(True, True)
        assertEq(1, 1)
        assertEq((1, 2, 3), (1, 2, 3))
    
    def test_assertEqFailsIfNotEqual(self):
        with pytest.raises(AssertionError):
            assertEq(1, 2, 'msg here')
    
    # test assertFloatEq
    def test_assertFloatEqEqual(self):
        assertFloatEq(0.0, 0)
        assertFloatEq(0.1234, 0.1234)
        assertFloatEq(-0.1234, -0.1234)
    
    def test_assertFloatEqEqualWithinPrecision(self):
        assertFloatEq(0.123456788, 0.123456789)
    
    def test_assertFloatNotEqualGreater(self):
        with pytest.raises(AssertionError):
            assertFloatEq(0.4, 0.1234)
    
    def test_assertFloatNotEqualSmaller(self):
        with pytest.raises(AssertionError):
            assertFloatEq(0.1234, 0.4)
    
    def test_assertFloatNotEqualBitGreater(self):
        with pytest.raises(AssertionError):
            assertFloatEq(-0.123457, -0.123456)
    
    def test_assertFloatNotEqualBitSmaller(self):
        with pytest.raises(AssertionError):
            assertFloatEq(-0.123457, -0.123458)
            
    def test_assertFloatNotEqualNegative(self):
        with pytest.raises(AssertionError):
            assertFloatEq(0.1234, -0.1234)

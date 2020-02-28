# BenPythonCommon,
# 2015 Ben Fisher, released under the GPLv3 license.

import pytest
import tempfile
import os
from os.path import join
from ..common_util import isPy3OrNewer, renderMillisTime, getNowAsMillisTime
from ..files import (readall, writeall, copy, move, sep, run, isemptydir, listchildren,
    getname, getparent, listfiles, recursedirs, recursefiles, listfileinfo, recursefileinfo,
    computeHash, runWithoutWaitUnicode, ensure_empty_directory, ustr, makedirs,
    isfile, isdir, rmdir, extensionPossiblyExecutable, writeallunlessalreadythere,
    getModTimeNs, setModTimeNs, addAllToZip)

class TestDirectoryList(object):
    def test_listChildren(self, fixture_fulldir):
        expected = ['P1.PNG', 'a1.txt', 'a2png', 's1', 's2']
        expectedTuples = [(join(fixture_fulldir, s), s) for s in expected]
        assert expectedTuples == sorted(list(listchildren(fixture_fulldir)))
    
    def test_listChildrenFilenamesOnly(self, fixture_fulldir):
        expected = ['P1.PNG', 'a1.txt', 'a2png', 's1', 's2']
        assert expected == sorted(list(listchildren(fixture_fulldir, filenamesOnly=True)))
    
    def test_listChildrenCertainExtensions(self, fixture_fulldir):
        expected = ['P1.PNG', 'a1.txt']
        assert expected == sorted(list(listchildren(fixture_fulldir, filenamesOnly=True, allowedexts=['png', 'txt'])))

    def test_listFiles(self, fixture_fulldir):
        expected = ['P1.PNG', 'a1.txt', 'a2png']
        expectedTuples = [(join(fixture_fulldir, s), s) for s in expected]
        assert expectedTuples == sorted(list(listfiles(fixture_fulldir)))
    
    def test_listFilesFilenamesOnly(self, fixture_fulldir):
        expected = ['P1.PNG', 'a1.txt', 'a2png']
        assert expected == sorted(list(listfiles(fixture_fulldir, filenamesOnly=True)))

    def test_listFilesCertainExtensions(self, fixture_fulldir):
        expected = ['P1.PNG', 'a1.txt']
        assert expected == sorted(list(listfiles(fixture_fulldir, filenamesOnly=True, allowedexts=['png', 'txt'])))

    def test_recurseFiles(self, fixture_fulldir):
        expected = ['/P1.PNG', '/a1.txt', '/a2png', '/s1/ss1/file.txt', '/s2/other.txt']
        expectedTuples = [(fixture_fulldir + s.replace('/', sep), getname(s)) for s in expected]
        assert expectedTuples == sorted(list(recursefiles(fixture_fulldir)))
    
    def test_recurseFilesFilenamesOnly(self, fixture_fulldir):
        expected = ['P1.PNG', 'a1.txt', 'a2png', 'file.txt', 'other.txt']
        assert expected == sorted(list(recursefiles(fixture_fulldir, filenamesOnly=True)))
    
    def test_recurseFilesCertainExtensions(self, fixture_fulldir):
        expected = ['a1.txt', 'file.txt', 'other.txt']
        assert expected == sorted(list(recursefiles(fixture_fulldir, filenamesOnly=True, allowedexts=['txt'])))
        
    def test_recurseFilesAcceptAllSubDirs(self, fixture_fulldir):
        expected = ['a1.txt', 'file.txt', 'other.txt']
        assert expected == sorted(list(
            recursefiles(fixture_fulldir, filenamesOnly=True, allowedexts=['txt'], fnFilterDirs=lambda d: True)))
    
    def test_recurseFilesAcceptNoSubDirs(self, fixture_fulldir):
        expected = ['a1.txt']
        assert expected == sorted(list(
            recursefiles(fixture_fulldir, filenamesOnly=True, allowedexts=['txt'], fnFilterDirs=lambda d: False)))
    
    def test_recurseFilesExcludeOneSubdir(self, fixture_fulldir):
        expected = ['a1.txt', 'other.txt']
        def filter(d):
            return getname(d) != 's1'
        assert expected == sorted(list(recursefiles(fixture_fulldir, filenamesOnly=True, allowedexts=['txt'], fnFilterDirs=filter)))
    
    def test_recurseDirs(self, fixture_fulldir):
        expected = ['/full', '/full/s1', '/full/s1/ss1', '/full/s1/ss2', '/full/s2']
        expectedTuples = [(getparent(fixture_fulldir) + s.replace('/', sep), getname(s)) for s in expected]
        assert expectedTuples == sorted(list(recursedirs(fixture_fulldir)))
    
    def test_recurseDirsNamesOnly(self, fixture_fulldir):
        expected = ['full', 's1', 's2', 'ss1', 'ss2']
        assert expected == sorted(list(recursedirs(fixture_fulldir, filenamesOnly=True)))
    
    def test_recurseDirsExcludeOneSubdir(self, fixture_fulldir):
        expected = ['full', 's2']
        def filter(d):
            return getname(d) != 's1'
        assert expected == sorted(list(recursedirs(fixture_fulldir, filenamesOnly=True, fnFilterDirs=filter)))

    def test_listFileInfo(self, fixture_fulldir):
        if isPy3OrNewer:
            expected = [('full', 'P1.PNG', 8), ('full', 'a1.txt', 8), ('full', 'a2png', 8)]
            got = [(getname(getparent(o.path)), o.short(), o.size()) for o in listfileinfo(fixture_fulldir)]
            assert expected == sorted(got)
    
    def test_listFileInfoIncludeDirs(self, fixture_fulldir):
        if isPy3OrNewer:
            expected = [('full', 'P1.PNG', 8), ('full', 'a1.txt', 8), ('full', 'a2png', 8),
                ('full', 's1', 0), ('full', 's2', 0)]
            got = [(getname(getparent(o.path)), o.short(), o.size())
                for o in listfileinfo(fixture_fulldir, filesOnly=False)]
            assert expected == sorted(got)
            
    def test_recurseFileInfo(self, fixture_fulldir):
        if isPy3OrNewer:
            expected = [('full', 'P1.PNG', 8), ('full', 'a1.txt', 8), ('full', 'a2png', 8),
                ('s2', 'other.txt', 8), ('ss1', 'file.txt', 8)]
            got = [(getname(getparent(o.path)), o.short(), o.size())
                for o in recursefileinfo(fixture_fulldir)]
            assert expected == sorted(got)
    
    def test_recurseFileInfoIncludeDirs(self, fixture_fulldir):
        if isPy3OrNewer:
            expected = [('full', 'P1.PNG', 8), ('full', 'a1.txt', 8), ('full', 'a2png', 8),
                ('full', 's1', 0), ('full', 's2', 0), ('s1', 'ss1', 0), ('s1', 'ss2', 0),
                ('s2', 'other.txt', 8), ('ss1', 'file.txt', 8)]
            got = [(getname(getparent(o.path)), o.short(), o.size())
                for o in recursefileinfo(fixture_fulldir, filesOnly=False)]
            assert expected == sorted(got)

    def test_checkNamedParameters(self, fixture_dir):
        with pytest.raises(ValueError) as exc:
            list(listchildren(fixture_dir, True))
        exc.match('please name parameters')

class TestWriteUnlessThere(object):
    def test_writeallunlessthereNewFile(self, fixture_dir):
        path = join(fixture_dir, 'a.dat')
        ret = writeallunlessalreadythere(path, b'abc', mode='wb')
        assert ret == True
        assert b'abc' == readall(path, 'rb')

    def test_writeallunlessthereChangedFile(self, fixture_dir):
        path = join(fixture_dir, 'a.dat')
        writeall(path, b'abcd', 'wb')
        ret = writeallunlessalreadythere(path, b'abc', mode='wb')
        assert ret == True
        assert b'abc' == readall(path, 'rb')

    def test_writeallunlessthereSameFile(self, fixture_dir):
        path = join(fixture_dir, 'a.dat')
        writeall(path, b'abc', 'wb')
        ret = writeallunlessalreadythere(path, b'abc', mode='wb')
        assert ret == False
        assert b'abc' == readall(path, 'rb')

    def test_writeallunlessthereNewTxtFile(self, fixture_dir):
        if isPy3OrNewer:
            path = join(fixture_dir, 'a.txt')
            ret = writeallunlessalreadythere(path, 'abc', mode='w')
            assert ret == True
            assert 'abc' == readall(path)

    def test_writeallunlessthereChangedTxtFile(self, fixture_dir):
        if isPy3OrNewer:
            path = join(fixture_dir, 'a.txt')
            writeall(path, 'abcd')
            ret = writeallunlessalreadythere(path, 'abc', mode='w')
            assert ret == True
            assert 'abc' == readall(path)

    def test_writeallunlessthereSameTxtFile(self, fixture_dir):
        if isPy3OrNewer:
            path = join(fixture_dir, 'a.txt')
            writeall(path, 'abc')
            ret = writeallunlessalreadythere(path, 'abc', mode='w')
            assert ret == False
            assert 'abc' == readall(path)

class TestOtherUtils(object):
    def test_computeHashDefaultHash(self, fixture_dir):
        writeall(join(fixture_dir, 'a.txt'), 'contents')
        assert '4a756ca07e9487f482465a99e8286abc86ba4dc7' == computeHash(join(fixture_dir, 'a.txt'))
    
    def test_computeHashMd5Specified(self, fixture_dir):
        writeall(join(fixture_dir, 'a.txt'), 'contents')
        assert '4a756ca07e9487f482465a99e8286abc86ba4dc7' == computeHash(join(fixture_dir, 'a.txt'), 'sha1')
    
    def test_computeHashMd5(self, fixture_dir):
        import hashlib
        hasher = hashlib.md5()
        writeall(join(fixture_dir, 'a.txt'), 'contents')
        assert '98bf7d8c15784f0a3d63204441e1e2aa' == computeHash(join(fixture_dir, 'a.txt'), hasher)
    
    def test_computeHashCrc(self, fixture_dir):
        writeall(join(fixture_dir, 'a.txt'), 'contents')
        assert 'b4fa1177' == computeHash(join(fixture_dir, 'a.txt'), 'crc32')
    
    def test_computeHashNotExist(self, fixture_dir):
        writeall(join(fixture_dir, 'a.txt'), 'contents')
        with pytest.raises(ValueError):
            computeHash(join(fixture_dir, 'a.txt'), 'no_such_hash')

    def test_extensionPossiblyExecutableNoExt(self, fixture_dir):
        assert extensionPossiblyExecutable('noext') is False
        assert extensionPossiblyExecutable('/path/noext') is False

    def test_extensionPossiblyExecutableExt(self, fixture_dir):
        assert extensionPossiblyExecutable('ext.jpg') is False
        assert extensionPossiblyExecutable('/path/ext.jpg') is False

    def test_extensionPossiblyExecutableDirsep(self, fixture_dir):
        assert extensionPossiblyExecutable('dirsep/') is False
        assert extensionPossiblyExecutable('/path/dirsep/') is False

    def test_extensionPossiblyExecutablePeriod(self, fixture_dir):
        assert 'exe' == extensionPossiblyExecutable('test.jpg.exe')
        assert 'exe' == extensionPossiblyExecutable('/path/test.jpg.exe')
    
    def test_extensionPossiblyExecutablePeriodOk(self, fixture_dir):
        assert extensionPossiblyExecutable('test.exe.jpg') is False
        assert extensionPossiblyExecutable('/path/test.exe.jpg') is False
        
    def test_extensionPossiblyExecutableOk(self, fixture_dir):
        assert extensionPossiblyExecutable('ext.c') is False
        assert extensionPossiblyExecutable('/path/ext.c') is False
        assert extensionPossiblyExecutable('ext.longer') is False
        assert extensionPossiblyExecutable('/path/ext.longer') is False
    
    def test_extensionPossiblyExecutableExe(self, fixture_dir):
        assert 'exe' == extensionPossiblyExecutable('ext.exe')
        assert 'exe' == extensionPossiblyExecutable('/path/ext.exe')
        assert 'exe' == extensionPossiblyExecutable('ext.com')
        assert 'exe' == extensionPossiblyExecutable('/path/ext.com')
        assert 'exe' == extensionPossiblyExecutable('ext.vbScript')
        assert 'exe' == extensionPossiblyExecutable('/path/ext.vbScript')

    def test_extensionPossiblyExecutableWarn(self, fixture_dir):
        assert 'warn' == extensionPossiblyExecutable('ext.Url')
        assert 'warn' == extensionPossiblyExecutable('/path/ext.Url')
        assert 'warn' == extensionPossiblyExecutable('ext.doCM')
        assert 'warn' == extensionPossiblyExecutable('/path/ext.doCM')
        assert 'warn' == extensionPossiblyExecutable('ext.EXOPC')
        assert 'warn' == extensionPossiblyExecutable('/path/ext.EXOPC')

    def test_addAllToZip(self, fixture_dir):
        import zipfile
        def makeZip(**kwargs):
            ensure_empty_directory(fixture_dir)
            makedirs(join(fixture_dir, 'a/b'))
            writeall(join(fixture_dir, 'a/b.bmp'), 'contents111')
            writeall(join(fixture_dir, 'a/noext'), 'contents2')
            writeall(join(fixture_dir, 'a/b/im.png'), 'contents3')
            writeall(join(fixture_dir, 'a/b/te.txt'), 'contents4')
            outname = join(fixture_dir, 'a.zip')
            addAllToZip(join(fixture_dir, 'a'), outname, **kwargs)
            with zipfile.ZipFile(outname) as z:
                lst = z.infolist()
                lst.sort(key=lambda o: o.filename)
            return outname, lst

        # defaults to deflate method
        outname, lst = makeZip()
        assert 4 == len(lst)
        assert ('b.bmp', zipfile.ZIP_DEFLATED, 11) == (lst[0].filename, lst[0].compress_type, lst[0].file_size)
        assert ('b/im.png', zipfile.ZIP_DEFLATED, 9) == (lst[1].filename, lst[1].compress_type, lst[1].file_size)
        assert ('b/te.txt', zipfile.ZIP_DEFLATED, 9) == (lst[2].filename, lst[2].compress_type, lst[2].file_size)
        assert ('noext', zipfile.ZIP_DEFLATED, 9) == (lst[3].filename, lst[3].compress_type, lst[3].file_size)

        # use deflate+store
        outname, lst = makeZip(method='deflate', alreadyCompressedAsStore=True)
        assert 4 == len(lst)
        assert ('b.bmp', zipfile.ZIP_DEFLATED, 11) == (lst[0].filename, lst[0].compress_type, lst[0].file_size)
        assert ('b/im.png', zipfile.ZIP_STORED, 9) == (lst[1].filename, lst[1].compress_type, lst[1].file_size)
        assert ('b/te.txt', zipfile.ZIP_DEFLATED, 9) == (lst[2].filename, lst[2].compress_type, lst[2].file_size)
        assert ('noext', zipfile.ZIP_DEFLATED, 9) == (lst[3].filename, lst[3].compress_type, lst[3].file_size)

        # use lzma
        if isPy3OrNewer:
            outname, lst = makeZip(method='lzma')
            assert 4 == len(lst)
            assert ('b.bmp', zipfile.ZIP_LZMA, 11) == (lst[0].filename, lst[0].compress_type, lst[0].file_size)
            assert ('b/im.png', zipfile.ZIP_LZMA, 9) == (lst[1].filename, lst[1].compress_type, lst[1].file_size)
            assert ('b/te.txt', zipfile.ZIP_LZMA, 9) == (lst[2].filename, lst[2].compress_type, lst[2].file_size)
            assert ('noext', zipfile.ZIP_LZMA, 9) == (lst[3].filename, lst[3].compress_type, lst[3].file_size)

class TestCopyingFiles(object):
    def test_copyOverwrite_srcNotExist(self, fixture_dir):
        with pytest.raises(IOError):
            copy(join(fixture_dir, u'1\u1101.txt'), join(fixture_dir, u'2\u1101.txt'), True)
    
    def test_copyOverwrite_srcExists(self, fixture_dir):
        writeall(join(fixture_dir, u'1\u1101.txt'), 'contents')
        copy(join(fixture_dir, u'1\u1101.txt'), join(fixture_dir, u'2\u1101.txt'), True)
        assert 'contents' == readall(join(fixture_dir, u'1\u1101.txt'))
        assert 'contents' == readall(join(fixture_dir, u'2\u1101.txt'))
    
    def test_copyOverwrite_srcOverwrites(self, fixture_dir):
        writeall(join(fixture_dir, u'1\u1101.txt'), 'new')
        writeall(join(fixture_dir, u'2\u1101.txt'), 'old')
        copy(join(fixture_dir, u'1\u1101.txt'), join(fixture_dir, u'2\u1101.txt'), True)
        assert 'new' == readall(join(fixture_dir, u'1\u1101.txt'))
        assert 'new' == readall(join(fixture_dir, u'2\u1101.txt'))
    
    def test_copyNoOverwrite_srcNotExist(self, fixture_dir):
        with pytest.raises(IOError):
            copy(join(fixture_dir, u'1\u1101.txt'), join(fixture_dir, u'2\u1101.txt'), False)
    
    def test_copyNoOverwrite_srcExists(self, fixture_dir):
        writeall(join(fixture_dir, u'1\u1101.txt'), 'contents')
        copy(join(fixture_dir, u'1\u1101.txt'), join(fixture_dir, u'2\u1101.txt'), False)
        assert 'contents' == readall(join(fixture_dir, u'1\u1101.txt'))
        assert 'contents' == readall(join(fixture_dir, u'2\u1101.txt'))
    
    def test_copyNoOverwrite_shouldNotOverwrite(self, fixture_dir):
        writeall(join(fixture_dir, u'1\u1101.txt'), 'new')
        writeall(join(fixture_dir, u'2\u1101.txt'), 'old')
        with pytest.raises((IOError, OSError)):
            copy(join(fixture_dir, u'1\u1101.txt'), join(fixture_dir, u'2\u1101.txt'), False)
        assert 'new' == readall(join(fixture_dir, u'1\u1101.txt'))
        assert 'old' == readall(join(fixture_dir, u'2\u1101.txt'))
    
    def test_shouldNotCopyDir(self, fixture_dir):
        # by default, copy is for copying files, not dirs
        makedirs(join(fixture_dir, 'tmpdir1'))
        assert isdir(join(fixture_dir, 'tmpdir1'))
        try:
            with pytest.raises(IOError):
                copy(join(fixture_dir, 'tmpdir1'), join(fixture_dir, 'tmpdir2'), False)
        finally:
            rmdir(join(fixture_dir, 'tmpdir1'))

class TestMovingFiles(object):
    def test_moveOverwrite_srcNotExist(self, fixture_dir):
        with pytest.raises(IOError):
            move(join(fixture_dir, u'1\u1101.txt'), join(fixture_dir, u'2\u1101.txt'), True)
    
    def test_moveOverwrite_srcExists(self, fixture_dir):
        writeall(join(fixture_dir, u'1\u1101.txt'), 'contents')
        move(join(fixture_dir, u'1\u1101.txt'), join(fixture_dir, u'2\u1101.txt'), True)
        assert not isfile(join(fixture_dir, u'1\u1101.txt'))
        assert 'contents' == readall(join(fixture_dir, u'2\u1101.txt'))
    
    def test_moveOverwrite_srcOverwrites(self, fixture_dir):
        writeall(join(fixture_dir, u'1\u1101.txt'), 'new')
        writeall(join(fixture_dir, u'2\u1101.txt'), 'old')
        move(join(fixture_dir, u'1\u1101.txt'), join(fixture_dir, u'2\u1101.txt'), True)
        assert not isfile(join(fixture_dir, u'1\u1101.txt'))
        assert 'new' == readall(join(fixture_dir, u'2\u1101.txt'))
    
    def test_moveNoOverwrite_srcNotExist(self, fixture_dir):
        with pytest.raises(IOError):
            move(join(fixture_dir, u'1\u1101.txt'), join(fixture_dir, u'2\u1101.txt'), False)
    
    def test_moveNoOverwrite_srcExists(self, fixture_dir):
        writeall(join(fixture_dir, u'1\u1101.txt'), 'contents')
        move(join(fixture_dir, u'1\u1101.txt'), join(fixture_dir, u'2\u1101.txt'), False)
        assert not isfile(join(fixture_dir, u'1\u1101.txt'))
        assert 'contents' == readall(join(fixture_dir, u'2\u1101.txt'))
    
    def test_moveNoOverwrite_shouldNotOverwrite(self, fixture_dir):
        writeall(join(fixture_dir, u'1\u1101.txt'), 'new')
        writeall(join(fixture_dir, u'2\u1101.txt'), 'old')
        with pytest.raises((IOError, OSError)):
            move(join(fixture_dir, u'1\u1101.txt'), join(fixture_dir, u'2\u1101.txt'), False)
        assert 'new' == readall(join(fixture_dir, u'1\u1101.txt'))
        assert 'old' == readall(join(fixture_dir, u'2\u1101.txt'))

    def test_shouldNotMoveDir(self, fixture_dir):
        # by default, move is for moving files, not dirs
        makedirs(join(fixture_dir, 'tmpdir1'))
        assert isdir(join(fixture_dir, 'tmpdir1'))
        try:
            with pytest.raises(IOError):
                move(join(fixture_dir, 'tmpdir1'), join(fixture_dir, 'tmpdir2'), False)
        finally:
            rmdir(join(fixture_dir, 'tmpdir1'))

class TestMakeDirectories(object):
    def test_makeDirectoriesAlreadyExists(self, fixture_dir):
        makedirs(fixture_dir)
        assert isemptydir(fixture_dir)
    
    def test_makeDirectoriesOneLevel(self, fixture_dir):
        makedirs(fixture_dir + sep + 'a')
        assert isemptydir(fixture_dir + sep + 'a')
    
    def test_makeDirectoriesTwoLevels(self, fixture_dir):
        makedirs(fixture_dir + sep + 'a' + sep + 'a')
        assert isemptydir(fixture_dir + sep + 'a' + sep + 'a')

class TestFiletimes(object):
    @pytest.mark.skipif('not isPy3OrNewer')
    def test_modtimeIsUpdated(self, fixture_dir):
        writeall(join(fixture_dir, 'a.txt'), 'contents')
        curtime1 = getModTimeNs(join(fixture_dir, 'a.txt'))
        curtime2 = getModTimeNs(join(fixture_dir, 'a.txt'))
        assert curtime1 == curtime2

        # update the time by changing the file
        import time
        time.sleep(2)
        with open(join(fixture_dir, 'a.txt'), 'a') as f:
            f.write('changed')
        curtime3 = getModTimeNs(join(fixture_dir, 'a.txt'))
        curtime4 = getModTimeNs(join(fixture_dir, 'a.txt'))
        assert curtime3 == curtime4
        assert curtime3 > curtime2

        # update the time manually
        setModTimeNs(join(fixture_dir, 'a.txt'), curtime3 // 100)
        curtime5 = getModTimeNs(join(fixture_dir, 'a.txt'))
        assert curtime5 < curtime4

    @pytest.mark.skipif('not isPy3OrNewer')
    def test_modtimeRendered(self, fixture_dir):
        writeall(join(fixture_dir, 'a.txt'), 'contents')
        curtimeWritten = getModTimeNs(join(fixture_dir, 'a.txt'), asMillisTime=True)
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

@pytest.mark.skipif('not sys.platform.startswith("win")')
class TestRunProcess(object):
    def test_runShellScript(self, fixture_dir):
        writeall(join(fixture_dir, 'src.txt'), 'contents')
        writeall(join(fixture_dir, 's.bat'), 'copy src.txt dest.txt')
        returncode, stdout, stderr = run([join(fixture_dir, 's.bat')])
        assert returncode == 0 and isfile(join(fixture_dir, 'dest.txt'))
    
    def test_runShellScriptWithoutCapture(self, fixture_dir):
        writeall(join(fixture_dir, 'src.txt'), 'contents')
        writeall(join(fixture_dir, 's.bat'), 'copy src.txt dest.txt')
        returncode, stdout, stderr = run([join(fixture_dir, 's.bat')], captureOutput=False)
        assert returncode == 0 and isfile(join(fixture_dir, 'dest.txt'))

    def test_runShellScriptWithUnicodeChars(self, fixture_dir):
        import time
        writeall(join(fixture_dir, 'src.txt'), 'contents')
        writeall(join(fixture_dir, u's\u1101.bat'), 'copy src.txt dest.txt')
        runWithoutWaitUnicode([join(fixture_dir, u's\u1101.bat')])
        time.sleep(0.5)
        assert isfile(join(fixture_dir, 'dest.txt'))
    
    def test_runGetExitCode(self, fixture_dir):
        writeall(join(fixture_dir, 's.bat'), '\nexit /b 123')
        returncode, stdout, stderr = run([join(fixture_dir, 's.bat')], throwOnFailure=False)
        assert 123 == returncode
    
    def test_runGetExitCodeWithoutCapture(self, fixture_dir):
        writeall(join(fixture_dir, 's.bat'), '\nexit /b 123')
        returncode, stdout, stderr = run([join(fixture_dir, 's.bat')], throwOnFailure=False, captureOutput=False)
        assert 123 == returncode
    
    def test_runNonZeroExitShouldThrow(self, fixture_dir):
        writeall(join(fixture_dir, 's.bat'), '\nexit /b 123')
        with pytest.raises(RuntimeError):
            run([join(fixture_dir, 's.bat')])
    
    def test_runNonZeroExitShouldThrowWithoutCapture(self, fixture_dir):
        writeall(join(fixture_dir, 's.bat'), '\nexit /b 123')
        with pytest.raises(RuntimeError):
            run([join(fixture_dir, 's.bat')], captureOutput=False)
    
    def test_runSendArgument(self, fixture_dir):
        writeall(join(fixture_dir, 's.bat'), '\n@echo off\necho %1')
        returncode, stdout, stderr = run([join(fixture_dir, 's.bat'), 'testarg'])
        assert returncode == 0 and stdout == b'testarg'
    
    def test_runSendArgumentContainingSpaces(self, fixture_dir):
        writeall(join(fixture_dir, 's.bat'), '\n@echo off\necho %1')
        returncode, stdout, stderr = run([join(fixture_dir, 's.bat'), 'test arg'])
        assert returncode == 0 and stdout == b'"test arg"'
    
    def test_runGetOutput(self, fixture_dir):
        # the subprocess module uses threads to capture both stderr and stdout without deadlock
        writeall(join(fixture_dir, 's.bat'), '@echo off\necho testecho\necho testechoerr 1>&2')
        returncode, stdout, stderr = run([join(fixture_dir, 's.bat')])
        assert returncode == 0
        assert stdout == b'testecho'
        assert stderr == b'testechoerr'
    
@pytest.fixture()
def fixture_dir():
    basedir = join(tempfile.gettempdir(), 'ben_python_common_test', 'empty')
    basedir = ustr(basedir)
    ensure_empty_directory(basedir)
    os.chdir(basedir)
    yield basedir
    ensure_empty_directory(basedir)

@pytest.fixture(scope='module')
def fixture_fulldir():
    basedir = join(tempfile.gettempdir(), 'ben_python_common_test', 'full')
    basedir = ustr(basedir)
    ensure_empty_directory(basedir)
    
    # create every combination:
    # full						contains files and dirs
    # full/s1					contains dirs but no files
    # full/s1/ss1 			contains files but no dirs
    # full/s1/ss2 			contains no files or dirs
    dirsToCreate = ['s1', 's2', 's1/ss1', 's1/ss2']
    for dir in dirsToCreate:
        os.makedirs(join(basedir, dir).replace('/', sep))
    
    filesToCreate = ['P1.PNG', 'a1.txt', 'a2png', 's1/ss1/file.txt', 's2/other.txt']
    for file in filesToCreate:
        writeall(join(basedir, file).replace('/', sep), 'contents')
    
    yield basedir
    ensure_empty_directory(basedir)

# BenPythonCommon,
# 2015 Ben Fisher, released under the GPLv3 license.

import pytest
import tempfile
import os
import sys
from os.path import join
from ..common_util import isPy3OrNewer
from ..common_higher import getNowAsMillisTime
from ..files import (readall, writeall, copy, move, sep, run, isemptydir, listchildren,
    getname, getparent, listfiles, recursedirs, recursefiles, listfileinfo, recursefileinfo,
    computeHash, runWithoutWaitUnicode, ensureEmptyDirectory, ustr, makedirs,
    isfile, isdir, rmdir, extensionPossiblyExecutable, getext, exists, deletesure,
    windowsUrlFileGet, windowsUrlFileWrite, runRsync, runRsyncErrMap,
    setFileLastModifiedTime, getsize, getModTimeNs, setModTimeNs,
    addAllToZip, findBinaryOnPath)

class TestWrappers(object):
    def test_getparent(self):
        assert '/path/to' == getparent('/path/to/file')

    def test_getname(self):
        assert 'file' == getname('/path/to/file')

    def test_getext(self):
        # corner cases
        assert '' == getext('/path/to/')
        assert '' == getext('/path/to/..')
        assert '' == getext('/path/to/file')
        assert '' == getext('/path/to/file.')
        assert 'txt' == getext('/path/to/file.txt')
        assert 'txt' == getext('/path/to/file.other.txt')
        assert '' == getext('/path/to/.txt')

        # remove dot on different extension lengths
        assert 'a' == getext('/path/to/file.a')
        assert 'ab' == getext('/path/to/file.ab')
        assert 'abcde' == getext('/path/to/file.abcde')

        # make lowercase
        assert 'txt' == getext('/path/to/file.TXT')
        assert 'txt' == getext('/path/to/file.TxT')
        assert 'txt' == getext('/path/to/file.tXt')

    def test_listDeletesure(self, fixture_dir):
        # typical use
        writeall(join(fixture_dir, 'file'), b'a', 'wb')
        assert exists(join(fixture_dir, 'file'))
        deletesure(join(fixture_dir, 'file'))
        assert not exists(join(fixture_dir, 'file'))

        # ok to try to delete non existing
        deletesure(join(fixture_dir, 'non-existing-file'))

        # attempt delete while held should fail
        if sys.platform.startswith("win"):
            hold = open(join(fixture_dir, 'file'), 'w')
            with pytest.raises(Exception):
                deletesure(join(fixture_dir, 'file'))
            hold.close()

    def test_makedirs(self, fixture_dir):
        # one-deep
        assert not isdir(join(fixture_dir, 'd'))
        makedirs(join(fixture_dir, 'd'))
        assert isdir(join(fixture_dir, 'd'))

        # two-deep
        assert not isdir(join(fixture_dir, 'd1', 'd2'))
        makedirs(join(fixture_dir, 'd1', 'd2'))
        assert isdir(join(fixture_dir, 'd1', 'd2'))

        # ok if already exists
        makedirs(join(fixture_dir, 'd1', 'd2'))
        assert isdir(join(fixture_dir, 'd1', 'd2'))

    def test_ensureEmptyDirectory(self, fixture_fulldir):
        # recursively delete
        assert 5 == len(list(listchildren(fixture_fulldir)))
        assert not isemptydir(fixture_fulldir)
        ensureEmptyDirectory(fixture_fulldir)
        assert 0 == len(list(listchildren(fixture_fulldir)))
        assert isemptydir(fixture_fulldir)

        # can't delete a file
        writeall(join(fixture_fulldir, 'file'), b'a', 'wb')
        with pytest.raises(Exception):
            ensureEmptyDirectory(join(fixture_dir, 'file'))

        # will create directory if not exists
        assert not isdir(join(fixture_fulldir, 'd1', 'd2'))
        ensureEmptyDirectory(join(fixture_fulldir, 'd1', 'd2'))
        assert isdir(join(fixture_fulldir, 'd1', 'd2'))

        # necessary, since fixture_fulldir is built once per module
        restoreDirectoryContents(fixture_fulldir)

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

class TestWriteFiles(object):
    def test_readAndWriteSimple(self, fixture_dir):
        ret = writeall(join(fixture_dir, 'a.txt'), 'abc', mode='w')
        assert ret is True
        assert u'abc' == readall(join(fixture_dir, 'a.txt'), 'r')
        ret = writeall(join(fixture_dir, 'a.txt'), 'def', mode='w')
        assert ret is True
        assert u'def' == readall(join(fixture_dir, 'a.txt'), 'r')

    def test_readAndWriteUtf8(self, fixture_dir):
        path = join(fixture_dir, u'a\u1E31.txt')
        kwargs = dict(encoding='utf-8') if isPy3OrNewer else dict(unicodetype='utf-8')
        ret = writeall(path, u'\u1E31\u1E77\u1E53\u006E', **kwargs)
        assert ret is True
        assert u'\u1E31\u1E77\u1E53\u006E' == readall(path, **kwargs)

    def test_readAndWriteUtf16(self, fixture_dir):
        path = join(fixture_dir, u'a\u1E31.txt')
        kwargs = dict(encoding='utf-16le') if isPy3OrNewer else dict(unicodetype='utf-16le')
        ret = writeall(path, u'\u1E31\u1E77\u1E53\u006E', **kwargs)
        assert ret is True
        assert u'\u1E31\u1E77\u1E53\u006E' == readall(path, **kwargs)

    def test_writeAllUnlessThereNewFile(self, fixture_dir):
        path = join(fixture_dir, 'a.dat')
        ret = writeall(path, b'abc', mode='wb', skipIfSameContent=True)
        assert ret is True
        assert b'abc' == readall(path, 'rb')

    def test_writeAllUnlessThereChangedFile(self, fixture_dir):
        path = join(fixture_dir, 'a.dat')
        writeall(path, b'abcd', 'wb')
        ret = writeall(path, b'abc', mode='wb', skipIfSameContent=True)
        assert ret is True
        assert b'abc' == readall(path, 'rb')

    def test_writeAllUnlessThereSameFile(self, fixture_dir):
        path = join(fixture_dir, 'a.dat')
        writeall(path, b'abc', 'wb')
        ret = writeall(path, b'abc', mode='wb', skipIfSameContent=True)
        assert ret is False
        assert b'abc' == readall(path, 'rb')

    def test_writeAllUnlessThereNewTxtFile(self, fixture_dir):
        path = join(fixture_dir, 'a.txt')
        ret = writeall(path, 'abc', mode='w', skipIfSameContent=True)
        assert ret is True
        assert 'abc' == readall(path)

    def test_writeAllUnlessThereChangedTxtFile(self, fixture_dir):
        path = join(fixture_dir, 'a.txt')
        writeall(path, 'abcd')
        ret = writeall(path, 'abc', mode='w', skipIfSameContent=True)
        assert ret is True
        assert 'abc' == readall(path)

    def test_writeAllUnlessThereSameTxtFile(self, fixture_dir):
        path = join(fixture_dir, 'a.txt')
        writeall(path, 'abc')
        ret = writeall(path, 'abc', mode='w', skipIfSameContent=True)
        assert ret is False
        assert 'abc' == readall(path)

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

    def tupleFromObj(self, o):
        # x-platform differences in what is the size of a directory
        size = 0 if isdir(o.path) else o.size()
        return (getname(getparent(o.path)), o.short(), size)

    @pytest.mark.skipif('not isPy3OrNewer')
    def test_listFileInfo(self, fixture_fulldir):
        expected = [('full', 'P1.PNG', 15), ('full', 'a1.txt', 15), ('full', 'a2png', 14)]
        got = [self.tupleFromObj(o) for o in listfileinfo(fixture_fulldir)]
        assert expected == sorted(got)

    @pytest.mark.skipif('not isPy3OrNewer')
    def test_listFileInfoIncludeDirs(self, fixture_fulldir):
        expected = [('full', 'P1.PNG', 15), ('full', 'a1.txt', 15), ('full', 'a2png', 14),
            ('full', 's1', 0), ('full', 's2', 0)]
        got = [self.tupleFromObj(o)
            for o in listfileinfo(fixture_fulldir, filesOnly=False)]
        assert expected == sorted(got)

    @pytest.mark.skipif('not isPy3OrNewer')
    def test_recurseFileInfo(self, fixture_fulldir):
        expected = [('full', 'P1.PNG', 15), ('full', 'a1.txt', 15), ('full', 'a2png', 14),
            ('s2', 'other.txt', 18), ('ss1', 'file.txt', 17)]
        got = [self.tupleFromObj(o)
            for o in recursefileinfo(fixture_fulldir)]
        assert expected == sorted(got)

    @pytest.mark.skipif('not isPy3OrNewer')
    def test_recurseFileInfoIncludeDirs(self, fixture_fulldir):
        expected = [('full', 'P1.PNG', 15), ('full', 'a1.txt', 15), ('full', 'a2png', 14),
            ('full', 's1', 0), ('full', 's2', 0), ('s1', 'ss1', 0), ('s1', 'ss2', 0),
            ('s2', 'other.txt', 18), ('ss1', 'file.txt', 17)]
        got = [self.tupleFromObj(o)
            for o in recursefileinfo(fixture_fulldir, filesOnly=False)]
        assert expected == sorted(got)

    def test_checkNamedParameters(self, fixture_dir):
        with pytest.raises(ValueError) as exc:
            list(listchildren(fixture_dir, True))
        exc.match('please name parameters')

class TestFilesUtils(object):
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

    def test_findBinaryOnPath(self, fixture_dir):
        # found, abs path given
        path = join(fixture_dir, '_lookwpath_')
        writeall(path, 'abc')
        assert path == findBinaryOnPath(os.path.abspath(path))

        # not found
        os.environ["PATH"] += os.pathsep + fixture_dir
        assert None is findBinaryOnPath('_lookforthis_')

        # found
        writeall(join(fixture_dir, '_lookforthis_'), 'abc')
        assert join(fixture_dir, '_lookforthis_') == findBinaryOnPath('_lookforthis_')

        # windows can look for a exe
        if sys.platform.startswith('win'):
            writeall(join(fixture_dir, '_lookforthis2_.exe'), 'abc')
            assert join(fixture_dir, '_lookforthis2_.exe') == findBinaryOnPath('_lookforthis2_')

        # windows can look for a bat
        if sys.platform.startswith('win'):
            writeall(join(fixture_dir, '_lookforthis3_.bat'), 'abc')
            assert join(fixture_dir, '_lookforthis3_.bat') == findBinaryOnPath('_lookforthis3_')

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

    def test_addAllToZip(self, fixture_dir):
        import zipfile

        def makeZip(**kwargs):
            ensureEmptyDirectory(fixture_dir)
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

    def test_windowsUrlFileGet(self, fixture_dir):
        # typical file
        example = '''[InternetShortcut]
URL=https://example.net/
        '''
        writeall(join(fixture_dir, 'a.url'), example)
        assert 'https://example.net/' == windowsUrlFileGet(join(fixture_dir, 'a.url'))

        # has different keys
        example = '''[InternetShortcut]
Icon=12345
URL=https://exampletwo.net/'''
        writeall(join(fixture_dir, 'a.url'), example)
        assert 'https://exampletwo.net/' == windowsUrlFileGet(join(fixture_dir, 'a.url'))

        # has no url
        example = '''[InternetShortcut]
Icon=12345'''
        writeall(join(fixture_dir, 'a.url'), example)
        with pytest.raises(RuntimeError):
            windowsUrlFileGet(join(fixture_dir, 'a.url'))

    def test_windowsUrlFileWrite(self, fixture_dir):
        expected = '''[InternetShortcut]
URL=https://example.net/
'''
        deletesure(join(fixture_dir, 'a.url'))
        windowsUrlFileWrite(join(fixture_dir, 'a.url'), 'https://example.net/')
        assert expected.replace('\r\n', '\n') == readall(join(fixture_dir, 'a.url'))
        assert 'https://example.net/' == windowsUrlFileGet(join(fixture_dir, 'a.url'))

class TestRunRSync(object):
    def test_normal(self, fixture_fulldir, fixture_dir):
        # typical usage
        expect = 'P1.PNG,15|a1.txt,15|a2png,14|s1,0|s1/ss1,0|s1/ss1/file.txt,17|s1/ss2,0|s2,0|s2/other.txt,18'
        assert expect == listDirectoryToString(fixture_fulldir)
        dest = join(fixture_dir, 'dest')
        makedirs(dest)
        runRsync(fixture_fulldir, dest, deleteExisting=True)
        assert expect == listDirectoryToString(dest)

        # copy it again, nothing to change
        runRsync(fixture_fulldir, dest, deleteExisting=True)
        assert expect == listDirectoryToString(dest)

    def test_empty(self, fixture_dir):
        # copying an empty folder should succeed
        src = join(fixture_dir, 'src')
        makedirs(src)
        dest = join(fixture_dir, 'dest')
        makedirs(dest)
        runRsync(src, dest, deleteExisting=True)
        assert '' == listDirectoryToString(dest)

    def test_shouldOverwrite(self, fixture_dir):
        # create a modified dir
        src = join(fixture_dir, 'src')
        restoreDirectoryContents(src)
        dest = join(fixture_dir, 'dest')
        restoreDirectoryContents(dest)
        modifyDirectoryContents(src)
        expect = 'P1.PNG,15|a1.txt,15|a2png,14|s1,0|s1/ss1,0|s1/ss1/file.txt,17|s1/ss2,0|s2,0|s2/other.txt,18'
        assert expect == listDirectoryToString(dest)
        expect = 'P1.PNG,15|a2.txt,15|newfile,11|s1,0|s1/ss1,0|s1/ss1/file.txt,35|s1/ss2,0|s2,0|s2/other.txt,34'
        assert expect == listDirectoryToString(src)

        # run rsync and delete existing
        runRsync(src, dest, deleteExisting=True)
        assert expect == listDirectoryToString(dest)

    def test_shouldNotOverwrite(self, fixture_dir):
        # create a modified dir
        src = join(fixture_dir, 'src')
        restoreDirectoryContents(src)
        dest = join(fixture_dir, 'dest')
        restoreDirectoryContents(dest)
        modifyDirectoryContents(src)
        expect = 'P1.PNG,15|a1.txt,15|a2png,14|s1,0|s1/ss1,0|s1/ss1/file.txt,17|s1/ss2,0|s2,0|s2/other.txt,18'
        assert expect == listDirectoryToString(dest)
        expect = 'P1.PNG,15|a2.txt,15|newfile,11|s1,0|s1/ss1,0|s1/ss1/file.txt,35|s1/ss2,0|s2,0|s2/other.txt,34'
        assert expect == listDirectoryToString(src)

        # run rsync and don't delete existing
        runRsync(src, dest, deleteExisting=False)
        expect = 'P1.PNG,15|a1.txt,15|a2.txt,15|a2png,14|newfile,11|s1,0|s1/ss1,0|s1/ss1/file.txt,35|s1/ss2,0|s2,0|s2/other.txt,34'
        assert expect == listDirectoryToString(dest)

    def test_excludes(self, fixture_dir):
        # create a modified dir
        src = join(fixture_dir, 'src')
        restoreDirectoryContents(src)
        dest = join(fixture_dir, 'dest')
        restoreDirectoryContents(dest)
        modifyDirectoryContents(src)
        expect = 'P1.PNG,15|a1.txt,15|a2png,14|s1,0|s1/ss1,0|s1/ss1/file.txt,17|s1/ss2,0|s2,0|s2/other.txt,18'
        assert expect == listDirectoryToString(dest)
        expect = 'P1.PNG,15|a2.txt,15|newfile,11|s1,0|s1/ss1,0|s1/ss1/file.txt,35|s1/ss2,0|s2,0|s2/other.txt,34'
        assert expect == listDirectoryToString(src)

        # run rsync with exclusions
        runRsync(src, dest, deleteExisting=True, excludeFiles=['a1.txt', 'newfile', 'other.txt'], excludeDirs=['ss1'])
        expect = 'P1.PNG,15|a1.txt,15|a2.txt,15|s1,0|s1/ss1,0|s1/ss1/file.txt,17|s1/ss2,0|s2,0|s2/other.txt,18'
        assert expect == listDirectoryToString(dest)

    def test_expectFailure(self, fixture_dir):
        # try to copy non-existing directory
        src = join(fixture_dir, 'src')
        dest = join(fixture_dir, 'dest')
        with pytest.raises(Exception):
            runRsync(src, dest, deleteExisting=True, checkExist=False)

    def test_nonWindowsError(self, fixture_dir):
        assert (True, '') == runRsyncErrMap(0, 'linux')
        assert (False, 'Syntax or usage error') == runRsyncErrMap(1, 'linux')
        assert (False, 'Timeout waiting for daemon connection') == runRsyncErrMap(35, 'linux')
        assert (False, 'Unknown') == runRsyncErrMap(-1, 'linux')
        assert (False, 'Unknown') == runRsyncErrMap(100, 'linux')

    def test_windowsError(self, fixture_dir):
        res = runRsyncErrMap(0x0, 'windows')
        assert res[0] is True and res[1] == ''
        res = runRsyncErrMap(0x1, 'windows')
        assert res[0] is True and 'One or more files' in res[1]
        res = runRsyncErrMap(0x1 | 0x4, 'windows')
        assert res[0] is True and 'One or more files' in res[1] and 'Mismatched files' in res[1]
        res = runRsyncErrMap(0x1 | 0x10, 'windows')
        assert res[0] is False and 'One or more files' in res[1] and 'Serious error' in res[1]
        res = runRsyncErrMap(0x1 | 0x40, 'windows')
        assert res[0] is False and 'One or more files' in res[1]
        res = runRsyncErrMap(0x20, 'windows')
        assert res[0] is False and res[1] == ''

@pytest.mark.skipif('sys.platform.startswith("win")')
class TestRunProcess(object):
    def test_runShellScript(self, fixture_dir):
        writeall(join(fixture_dir, 'src.txt'), 'contents')
        writeall(join(fixture_dir, 's.sh'), 'cp src.txt dest.txt')
        retcode, stdout, stderr = run(['/bin/bash', join(fixture_dir, 's.sh')])
        assert retcode == 0 and isfile(join(fixture_dir, 'dest.txt'))

    def test_runShellScriptWithoutCapture(self, fixture_dir):
        writeall(join(fixture_dir, 'src.txt'), 'contents')
        writeall(join(fixture_dir, 's.sh'), 'cp src.txt dest.txt')
        retcode, stdout, stderr = run(['/bin/bash', join(fixture_dir, 's.sh')], captureOutput=False)
        assert retcode == 0 and isfile(join(fixture_dir, 'dest.txt'))

    def test_runShellScriptWithUnicodeChars(self, fixture_dir):
        import time
        writeall(join(fixture_dir, 'src.txt'), 'contents')
        writeall(join(fixture_dir, u's\u1101.sh'), 'cp src.txt dest.txt')
        runWithoutWaitUnicode(['/bin/bash', join(fixture_dir, u's\u1101.sh')])
        time.sleep(0.5)
        assert isfile(join(fixture_dir, 'dest.txt'))

    def test_runGetExitCode(self, fixture_dir):
        writeall(join(fixture_dir, 's.sh'), '\nexit 123')
        retcode, stdout, stderr = run(['/bin/bash', join(fixture_dir, 's.sh')], throwOnFailure=False)
        assert 123 == retcode

    def test_runGetExitCodeWithoutCapture(self, fixture_dir):
        writeall(join(fixture_dir, 's.sh'), '\nexit 123')
        retcode, stdout, stderr = run(['/bin/bash', join(fixture_dir, 's.sh')], throwOnFailure=False, captureOutput=False)
        assert 123 == retcode

    def test_runNonZeroExitShouldThrow(self, fixture_dir):
        writeall(join(fixture_dir, 's.sh'), '\nexit 123')
        with pytest.raises(RuntimeError):
            run(['/bin/bash', join(fixture_dir, 's.sh')])

    def test_runNonZeroExitShouldThrowWithoutCapture(self, fixture_dir):
        writeall(join(fixture_dir, 's.sh'), '\nexit 123')
        with pytest.raises(RuntimeError):
            run(['/bin/bash', join(fixture_dir, 's.sh')], captureOutput=False)

    def test_runSendArgument(self, fixture_dir):
        writeall(join(fixture_dir, 's.sh'), '\n@echo off\necho $1')
        retcode, stdout, stderr = run(['/bin/bash', join(fixture_dir, 's.sh'), 'testarg'])
        assert retcode == 0 and stdout == b'testarg'

    def test_runSendArgumentContainingSpaces(self, fixture_dir):
        writeall(join(fixture_dir, 's.sh'), '\n@echo off\necho $1')
        retcode, stdout, stderr = run(['/bin/bash', join(fixture_dir, 's.sh'), 'test arg'])
        assert retcode == 0 and stdout == b'test arg'

    def test_runGetOutput(self, fixture_dir):
        # the subprocess module uses threads to capture both stderr and stdout without deadlock
        writeall(join(fixture_dir, 's.sh'), 'echo testecho\necho testechoerr 1>&2')
        retcode, stdout, stderr = run(['/bin/bash', join(fixture_dir, 's.sh')])
        assert retcode == 0
        assert stdout == b'testecho'
        assert stderr == b'testechoerr'

@pytest.mark.skipif('not sys.platform.startswith("win")')
class TestRunProcessWin(object):
    def test_runShellScript(self, fixture_dir):
        writeall(join(fixture_dir, 'src.txt'), 'contents')
        writeall(join(fixture_dir, 's.bat'), 'copy src.txt dest.txt')
        retcode, stdout, stderr = run([join(fixture_dir, 's.bat')])
        assert retcode == 0 and isfile(join(fixture_dir, 'dest.txt'))

    def test_runShellScriptWithoutCapture(self, fixture_dir):
        writeall(join(fixture_dir, 'src.txt'), 'contents')
        writeall(join(fixture_dir, 's.bat'), 'copy src.txt dest.txt')
        retcode, stdout, stderr = run([join(fixture_dir, 's.bat')], captureOutput=False)
        assert retcode == 0 and isfile(join(fixture_dir, 'dest.txt'))

    def test_runShellScriptWithUnicodeChars(self, fixture_dir):
        import time
        writeall(join(fixture_dir, 'src.txt'), 'contents')
        writeall(join(fixture_dir, u's\u1101.bat'), 'copy src.txt dest.txt')
        runWithoutWaitUnicode([join(fixture_dir, u's\u1101.bat')])
        time.sleep(0.5)
        assert isfile(join(fixture_dir, 'dest.txt'))

    def test_runGetExitCode(self, fixture_dir):
        writeall(join(fixture_dir, 's.bat'), '\nexit /b 123')
        retcode, stdout, stderr = run([join(fixture_dir, 's.bat')], throwOnFailure=False)
        assert 123 == retcode

    def test_runGetExitCodeWithoutCapture(self, fixture_dir):
        writeall(join(fixture_dir, 's.bat'), '\nexit /b 123')
        retcode, stdout, stderr = run([join(fixture_dir, 's.bat')], throwOnFailure=False, captureOutput=False)
        assert 123 == retcode

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
        retcode, stdout, stderr = run([join(fixture_dir, 's.bat'), 'testarg'])
        assert retcode == 0 and stdout == b'testarg'

    def test_runSendArgumentContainingSpaces(self, fixture_dir):
        writeall(join(fixture_dir, 's.bat'), '\n@echo off\necho %1')
        retcode, stdout, stderr = run([join(fixture_dir, 's.bat'), 'test arg'])
        assert retcode == 0 and stdout == b'"test arg"'

    def test_runGetOutput(self, fixture_dir):
        # the subprocess module uses threads to capture both stderr and stdout without deadlock
        writeall(join(fixture_dir, 's.bat'), '@echo off\necho testecho\necho testechoerr 1>&2')
        retcode, stdout, stderr = run([join(fixture_dir, 's.bat')])
        assert retcode == 0
        assert stdout == b'testecho'
        assert stderr == b'testechoerr'

def restoreDirectoryContents(basedir):
    ensureEmptyDirectory(basedir)

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
        writeall(join(basedir, file).replace('/', sep), 'contents_' + getname(file))

def modifyDirectoryContents(basedir):
    # deleted file
    os.unlink(join(basedir, 'a2png'))

    # new file
    writeall(join(basedir, 'newfile'), 'newcontents')

    # modified (newer)
    writeall(join(basedir, 's1/ss1/file.txt'), 'changedcontents' + '-' * 20)

    # modified (older)
    writeall(join(basedir, 's2/other.txt'), 'changedcontent' + '-' * 20)
    oneday = 60 * 60 * 24
    setFileLastModifiedTime(join(basedir, 's2/other.txt'),
        getNowAsMillisTime() / 1000.0 - oneday)

    # renamed file
    move(join(basedir, 'a1.txt'), join(basedir, 'a2.txt'), True)

def listDirectoryToString(basedir):
    out = []
    for f, short in recursefiles(basedir, includeDirs=True):
        s = f.replace(basedir, '').replace(os.sep, '/').lstrip('/')
        if s:
            # don't include the root
            size = 0 if isdir(f) else getsize(f)
            out.append(s + ',' + str(size))
    return '|'.join(sorted(out))

@pytest.fixture()
def fixture_dir():
    basedir = join(tempfile.gettempdir(), 'ben_python_common_test', 'empty')
    basedir = ustr(basedir)
    ensureEmptyDirectory(basedir)
    os.chdir(basedir)
    yield basedir
    ensureEmptyDirectory(basedir)

@pytest.fixture(scope='module')
def fixture_fulldir():
    basedir = join(tempfile.gettempdir(), 'ben_python_common_test', 'full')
    basedir = ustr(basedir)
    restoreDirectoryContents(basedir)
    yield basedir
    ensureEmptyDirectory(basedir)

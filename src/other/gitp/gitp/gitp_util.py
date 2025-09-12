# Ben Fisher
# Copyright (c) 2021, MIT License

import os
import sys
import zipfile
import json
from .gitp_pureutil import *

# where patches will be stored
outDir = '/Users/ben/patches'

# repos where you will make your changes.
workingRepos = [
    r'/Users/ben/dev/examplerepo6/examplerepo',
    r'/Users/ben/dev/examplerepo7/examplerepo',
    r'/Users/ben/dev/examplerepo8/examplerepo',
]

# temp repos used for showing diffs.
# solely used because you might have code running against
# your working repo and not want to change the files there.
tempRepos = {
    r'/Users/ben/dev/examplerepo6/examplerepo': r'/Users/ben/dev/mainbranches/examplerepo6b/examplerepo',
    r'/Users/ben/dev/examplerepo7/examplerepo': r'/Users/ben/dev/mainbranches/examplerepo7b/examplerepo',
    r'/Users/ben/dev/examplerepo8/examplerepo': r'/Users/ben/dev/mainbranches/examplerepo8b/examplerepo',
}

# a commit id on dev that is the basis for your work. use gitp updatebasis to update.  
basisCommits = {
    r'/Users/ben/dev/examplerepo8/examplerepo': r'7d5d04f033efd892dda336194c95c6c8bf2479f5',
}

# workaround for a problem showing gitk on recent macosx
runFixForGitk = True

# branch name for main
def mainBranch(d):
    if 'examplerepo-example' in d:
        return 'main'
    else:
        return 'dev'

# add tests
def addTests():
    root = '/Users/ben/gitptests'
    workingRepos.append(root)
    tempRepos[root] = root + 'b'
    basisCommits[root] = 'c628618320b25dd'

addTests()

# other comparison tools besides gitk:
# windows
#     winmerge
# linux
#     meld, xxdiff, KDiff3, Kompare, beediff
# macos
#     1) install xcode
#     2) sudo xcode-select -s /Applications/Xcode.app/Contents/Developer
#     3) opendiff

def getOutDirs():
    assertGitPacket(files.isDir(outDir), 'not found, please edit outDir in gitp_util.py')
    tmpDir = outDir + '/tmp'
    files.ensureEmptyDirectory(tmpDir)
    return outDir, tmpDir

def filesSortedInverseLmt(dir):
    out = []
    for f, short in files.listFiles(dir):
        out.append(((f, short), files.getLastModTime(f)))
    out.sort(key=lambda entry: entry[1], reverse=True)
    return [entry[0] for entry in out]

def getProjs(dir):
    projs = []
    for f, short in filesSortedInverseLmt(dir):
        if f.endswith('.gitp.zip') and not 'testgitptest' in short:
            proj = short.split('_')[0]
            if proj not in projs:
                projs.append(proj)
    return projs

def chooseProjName():
    outDir, tmpDir = getOutDirs()
    print('Choose a project name from the list, or type a new project name!')
    print('0) cancel')
    projs = getProjs(outDir)
    for i, proj in enumerate(projs):
        print(f'{i+1}) {proj}')
    while True:
        print()
        inp = rinput('')
        try:
            nInput = int(inp)
        except ValueError:
            nInput = -1
        if nInput == 0:
            raise GitPacketException("Canceled.")
        elif nInput != -1 and nInput >= 0 and nInput <= len(projs):
            return projs[nInput - 1]
        elif len(inp) > 1 and isOkForFilename(inp) and not '_' in inp:
            return inp
        print('Not a valid project name')

def chooseExistingProjName(outDir):
    projs = getProjs(outDir)
    i, chosen = getInputFromChoices('Choose a project', projs)
    assertGitPacket(i >= 0, "User canceled.")
    return chosen

def choosePacket(projname, outDir):
    opts = []
    for f, short in filesSortedInverseLmt(outDir):
        if short.startswith(f'{projname}_') and short.endswith('.gitp.zip'):
            opts.append(short)
    i, chosen = getInputFromChoices('Choose a packet', opts)
    assertGitPacket(i >= 0, "User canceled.")
    fullpath = files.join(outDir, chosen)
    assertTrue(files.isFile(fullpath))
    return fullpath

def promptPacketName():
    while True:
        inp = rinput('Provide a description of this packet (optional) in the form "Short description: more details"\n').strip()
        if not inp:
            return '', ''
        if ':' in inp:
            short, long = inp.split(':', 1)
        else:
            short, long = inp, ''
        if isOkForFilename(short):
            return short.strip(), long.strip()
        else:
            trace("The short description must contain only simple filename safe characters")

def getLatestProjCount(proj, shortdesc):
    outDir, tmpDir = getOutDirs()
    filelist = [short for (f, short) in files.listFiles(outDir)]
    return getLatestProjCountImpl(proj, shortdesc, filelist)

def checkOtherInstancesWhenStarting():
    import atexit
    if files.exists(getTrueTmp() + '/gitp_is_prob_running'):
        warn('It looks like gitp is still running, please wait for it to stop if it is still running.')
        return
    if files.exists(getTrueTmp() + '/gitp_is_not_clean_exit'):
        warn('It looks like gitp did not exit cleanly, please fix up the repos first.')
        return
    
    atexit.register(lambda: files.deleteSure(getTrueTmp() + '/gitp_is_prob_running'))
    files.writeAll(getTrueTmp() + '/gitp_is_prob_running', '')
    files.writeAll(getTrueTmp() + '/gitp_is_not_clean_exit', '')

def markCleanExitWhenEnding():
    files.deleteSure(getTrueTmp() + '/gitp_is_not_clean_exit')

def getTrueTmp():
    return '/Users/bf/Documents/temp/'

def showInSeparateThreadAndContinue(args, continueAfterSec, shell=False):
    # Desired behavior:
    # if you exit out of external program before 5 seconds, continue with the program and exit cleanly
    # if you are still in the external program after 5 seconds, continue but leave the program running
    import time
    import threading
    try:
        import queue
    except ImportError: # py 2.x
        import Queue as queue

    def fnTimer(q):
        # don't leave any handles open here, since this is a daemon thread,
        # they might not be cleaned up when the thread is terminated.
        time.sleep(continueAfterSec)
        q.put(True)

    def fnRunProcess(q):
        files.run(args, shell=shell, createNoWindow=True, captureOutput=False,
            silenceOutput=False, wait=True)
        q.put(True)

    q = queue.Queue()
    thTimer = threading.Thread(target=fnTimer, args=[q])
    thTimer.setDaemon(True) # if it's the last thread running, program exits early
    thTimer.start()
    thRunProcess = threading.Thread(target=fnRunProcess, args=[q])
    thRunProcess.start()
    q.get() # wait for either one, but not both, of the threads

# https://code.activestate.com/recipes/576620-changedirectory-context-manager/
# Christophe Simonis, MIT license
class ChangeCurrentDirectory(object):
    def __init__(self, directory):
        assertTrue(directory and os.path.isabs(directory), "not an absolute path", directory)
        self._dir = directory
        self._cwd = os.path.abspath(os.getcwd())
        self._pwd = self._cwd

    @property
    def current(self):
        return self._cwd
    
    @property
    def previous(self):
        return self._pwd
        
    def __enter__(self):
        self._pwd = self._cwd
        os.chdir(self._dir)
        self._cwd = os.getcwd()
        return self

    def __exit__(self, *args):
        os.chdir(self._pwd)
        self._cwd = self._pwd

def addFileToZip(zip, destname, srcfile):
    zip.write(srcfile, arcname=destname, compress_type=zipfile.ZIP_LZMA)

def addTextToZip(zip, destname, srctxt):
    outdir, tmpdir = getOutDirs()
    tmpname = tmpdir + '/tmp.txt'
    files.writeAll(tmpname, srctxt, encoding='utf-8')
    addFileToZip(zip, destname, tmpname)
    files.delete(tmpname)

def getManifestJsonFromZip(zip, innerName):
    assertGitPacket(innerName in zip.namelist(), f'zip file doesn\'t contain {innerName}') 
    with zip.open(innerName) as f:
        manifest = f.read()
    
    return json.loads(manifest)

# Ben Fisher
# Copyright (c) 2021, MIT License

from requests.api import patch
from .gitp_util import *
import json
import zipfile

def addAndCheckNoDupe(d, s):
    assertTrue(not s in d, 'encountered same path twice?', s, d)
    assertGitPacket('.' in files.getname(s), "we don't yet support files with no extensions", s)
    d[s] = True

def areThereUnstagedFiles():
    results = getGitResults('git_status_--porcelain_--untracked-files=all', strip=False)
    results = results.split('\n') if results else []
    for line in results:
        if line.startswith('??') or line.startswith(' '):
            return True
    return False

def areThereStagedFiles():
    # when files are staged, git tries to find 'moves', 'copies', etc,
    # which is many cases to consider. I'll use unstaged files instead,
    # only using `git diff --cached` to ask if any files are staged or not
    ret = getGitResults('git_diff_--name-only_--cached')
    return len(ret.split('\n') if ret else []) > 0

def getGitResults(cmdSeperatedByUnder, addArgs=None, okIfErrTxt=None, strip=True):
    cmd = cmdSeperatedByUnder.split('_')
    if addArgs:
        assertTrue(not isinstance(addArgs, anystringtype))
        cmd.extend(map(str, addArgs))
    retcode, stdout, stderr = files.run(cmd, throwOnFailure=False)
    stdout = stdout.decode('utf-8')
    stderr = stderr.decode('utf-8')
    if retcode != 0 and (not okIfErrTxt or okIfErrTxt not in stderr):
        exceptionText = 'retcode is not 0 for process ' + \
            str(cmd) + '\nretcode was ' + str(retcode) + \
            '\nstdout was ' + str(stdout) + \
            '\nstderr was ' + str(stderr)
        raise RuntimeError(getPrintable(exceptionText))
    return stdout.strip() if strip else stdout

def getLast10Commits(nCommits=10):
    ret = getGitResults(f'git_log_--oneline_-n', [nCommits])
    lines = ret.strip().split('\n')
    assertWarn(len(lines) >= 3, 'expected at least 3 lines, is this a very new repo?')
    retIds = []
    retText = []
    for line in lines:
        id, text = line.split(' ', 1)
        retIds.append(id)
        retText.append(text)
    return retIds, retText

def findCommitInTheLast10OrThrow(searchFor, nCommits=10):
    # returns list of commit ids until a match found
    lastCommits, lastCommitTexts = getLast10Commits()
    foundIndex = jslike.findIndex(lastCommits, lambda val: shasMatch(val, searchFor))
    if foundIndex == -1:
        assertGitPacket(False, f"basis not found in the last {nCommits} of {os.getcwd()}, wrong basis? wrong branch? {searchFor}")
    return lastCommits[0: foundIndex+1], lastCommitTexts[0: foundIndex+1]

def allFilesModifiedInCommit(commitId):
    # includes modifications, new files, deleted files
    assertTrue(commitId and len(commitId) > 5)
    hasNoFiles = False
    ret = getGitResults(f'git_show_--name-only', [commitId])
    pts = ret.strip().split('\n\n')
    if len(pts) < 2:
        assertGitPacket(False, 'expected 3 sections separated by \\n\\n when looking for files in commit', commitId, ret)
    if len(pts) < 3:
        hasNoFiles = True
        pts.append('')
    for i, part in enumerate(pts):
        hasIndent = i != 0 and i != len(pts)-1
        sublines = part.split('\n')
        assertGitPacket(jslike.every(sublines, lambda line: line.startswith('    ') == hasIndent),
            "the results of git show were different than we thought", commitId, ret)

    filesList = pts[-1].strip()
    return [] if hasNoFiles else filesList.split('\n')

def allFilesModifiedInCommits(arrCommits):
    filesMod = {}
    for commit in arrCommits:
        for file in allFilesModifiedInCommit(commit):
            filesMod[file] = True
    return filesMod

def allFilesPotentiallyModifiedSinceCommit(commitId):
    retIds, retText = findCommitInTheLast10OrThrow(commitId)
    return allFilesModifiedInCommits(retIds)

def getUnstagedFiles():
    # lists all of the newly added files, and not the directories
    # also includes all deletions
    # git ls-files --other --modified --exclude-standard works as well,
    # but gives us less information.
    assertGitPacket(not areThereStagedFiles(), "did not expect staged files, please git reset them", os.getcwd())
    results = getGitResults('git_status_--porcelain_--untracked-files=all', strip=False)
    results = results.split('\n') if results else []
    ret = Bucket(allchanged={}, modified={}, deleted={}, added={})
    for line in results:
        assertTrue(line[2:3] == ' ')
        status = line[0:2]
        path = line[3:]
        addAndCheckNoDupe(ret.allchanged, path)
        assertGitPacket(not files.isdir(path), "path should not be a directory.", path)
        if status == ' M':
            addAndCheckNoDupe(ret.modified, path)
            assertGitPacket(files.isfile(path), "path not found", path)
        elif status == ' D':
            addAndCheckNoDupe(ret.deleted, path)
            assertGitPacket(not files.isfile(path), "path found but thought to be deleted", path)
        elif status == '??':
            addAndCheckNoDupe(ret.added, path)
            assertGitPacket(files.isfile(path), "path not found", path)
        else:
            assertGitPacket('unexpected status. do you have staged files/are you in the middle of merging?', '\n'.join(results))
    
    return ret

def makeDestLookExactlyLikeSrc(src, dest, pathsPossiblyModified):
    # we'd use rsync-and-skip-.git-directories for this,
    # but that is slow for very large repos since filetimes are different.
    # we want the same result as rsync, but faster.
    assertTrue(files.isdir(src+'/.git'), 'not a git repo')
    assertTrue(files.isdir(dest+'/.git'), 'not a git repo')
    for path in pathsPossiblyModified:
        assertTrue(not path.startswith('/') and not path.startswith('\\'))
        p1 = files.join(src, path)
        p2 = files.join(dest, path)
        assertTrue(not files.isdir(p1), 'dir?', p1)
        assertTrue(not files.isdir(p2), 'dir?', p2)
        if files.isfile(p1) and files.isfile(p2):
            files.makedirs(files.getparent(p2))
            files.copy(p1, p2, True)
        elif files.isfile(p1) and not files.isfile(p2):
            files.makedirs(files.getparent(p2))
            files.copy(p1, p2, True)
        elif not files.isfile(p1) and files.isfile(p2):
            files.delete(p2)
        elif not files.isfile(p1) and not files.isfile(p2):
            pass
        else:
            assertTrue(False, 'not reached')

def isPendingMerge():
    return (files.exists('.git/MERGE_HEAD') or # a merge is in progress
        files.exists('.git/rebase-apply') or
        files.exists('.git/rebase-merge'))

def currentCommitId():
    return getGitResults('git_rev-parse_--verify_HEAD')

def getCommitMsgFromCommitId(commitId):
    return getGitResults('git_show_-s_--format=%B', [commitId]).strip()

def getRestoreBranchAndPrepMoveBranch(d):
    assertGitPacket(not areThereStagedFiles(), f"expect no staged files in {d}")
    assertGitPacket(not areThereUnstagedFiles(), f'expect no unstaged files in {d}')
    curBranch = getGitResults('git_branch_--show-current')
    if curBranch.startswith('gitptemp'):
        getGitResults('git_co', [mainBranch(d)])
        curBranch = mainBranch(d)
    return curBranch

def determineRootPaths(checkTempRepo=True):
    dir = os.path.abspath(os.getcwd())
    removeAllDsStore()
    assertGitPacket(files.isdir(dir + '/.git'), 'please cd to the root of a git repo')
    assertGitPacket(dir in workingRepos, f'dir {dir} not found in workingRepos, please add to gitp_util.py')
    assertGitPacket(not isPendingMerge(), f'pending merge or rebase in {dir}?')
    curBranch = getGitResults('git_branch_--show-current')
    assertGitPacket(curBranch == 'gpworking', f'please create a branch called "gpworking" off of basis-commit {basisCommits.get(dir, "")} from {mainBranch(dir)}')
    curCommit = currentCommitId()
    assertGitPacket(dir in basisCommits, f'no corresponding entry in basisCommits for {dir}, please add to gitp_util.py')
    assertGitPacket(shasMatch(basisCommits[dir], curCommit), f'expected to be at commit {basisCommits[dir]} but is {curCommit}, please update gitp_util.py or go to that commit')
    assertGitPacket(not areThereStagedFiles(), f'should be no staged files in {dir}, please reset them')
    if checkTempRepo:
        assertGitPacket(dir in tempRepos, f'no corresponding entry in tempRepos for {dir}, please add to gitp_util.py')
        assertGitPacket(files.isdir(tempRepos[dir] + '/.git'), f'tempRepos entry {tempRepos[dir]} is not the root of a git repo')
        assertGitPacket(os.path.isabs(tempRepos[dir]), f'tempRepos entry {tempRepos[dir]} is not an absolute path')
        with ChangeCurrentDirectory(tempRepos[dir]) as cd:
            removeAllDsStore()
            curBranch = getGitResults('git_branch_--show-current')
            if curBranch.startswith('gitptemp-'):
                getGitResults('git_co', [mainBranch(tempRepos[dir])])
            assertGitPacket(not isPendingMerge(), f'pending merge or rebase in {tempRepos[dir]}?')
            assertGitPacket(not areThereStagedFiles(), f'should not have staged files in {tempRepos[dir]}')
            assertGitPacket(not areThereUnstagedFiles(), f'should not have unstaged files in {tempRepos[dir]}')
            findCommitInTheLast10OrThrow(basisCommits[dir])

    root, tmproot = dir, tempRepos.get(dir, '')
    endsWithB = lambda s: s.endswith('b') or files.getname(files.getparent(s)).endswith('b')
    assertTrue(endsWithB(tmproot), 'expect tmpRepos entry to end with b')
    assertTrue(not endsWithB(root), 'expect repos entry to not end with b')
    return root, tmproot

def removeAllDsStore():
    for f, short in list(files.recursefiles('.')):
        if short == '.DS_Store':
            files.delete(f)

def makeAFormatPatchInTmpDir(tmpDir, baseBranch):
    assertTrue(not areThereStagedFiles(), "expect no staged files after commit")
    assertTrue(not areThereUnstagedFiles(), "expect no unstaged files after commit")
    # warning: don't use git diff, which wasn't designed for binary data
    files.ensureEmptyDirectory(tmpDir)
    getGitResults('git_format-patch', [
        #~ f'--unified=5', # 5 lines of context instead of default 3
        f'--output-directory={tmpDir}', # specify output dir (there's no way to specify patch path)
        f'--full-index', # Instead of the first handful of characters, show the full pre- and post-image blob object names on the "index" line 
        f'--binary', # support binary
        baseBranch
    ])

    fls = [f for f, short in files.listfiles(tmpDir) if f.endswith('.patch')]
    assertEq(1, len(fls), "expected exactly one patch", fls)
    return fls[0]

def applyPatch(patchfile, dest):
    with ChangeCurrentDirectory(dest) as cd:
        # don't use the --binary or --inaccurate-eof flags
        assertTrue(files.isfile(patchfile), "not found", patchfile)
        retcode, stdout, stderr = files.run(['git', 'apply', '--check', patchfile], throwOnFailure=None)
        assertGitPacket(retcode == 0, "We weren't certain the patch will apply, due to:", 
            stdout.decode('utf-8'), stderr.decode('utf-8'))

        getGitResults(f'git_apply', [patchfile])

def gitpTop_Pack_ConfirmMergeApplies(root, tmproot, patch, changes):
    basis = basisCommits[root]
    with ChangeCurrentDirectory(tmproot) as cd:
        restoreBranch = getRestoreBranchAndPrepMoveBranch(tmproot)
        getGitResults(f'git_branch_-D_gitptemp-confirmmerge', okIfErrTxt='not found.')
        getGitResults(f'git_co', [basis])
        getGitResults(f'git_co_-b_gitptemp-confirmmerge')

        # apply it
        applyPatch(patch, tmproot)

        # confirm changes
        for path in changes.allchanged.keys():
            assertTrue(not path.startswith('/') and not path.startswith('\\') and not os.path.isabs(path))
            f1 = files.join(root, path)
            f2 = files.join(tmproot, path)
            if path in changes.modified or path in changes.added:
                assertTrue(compareTwoFiles(f1, f2), "bad patch? files not the same", f1, f2)
            elif path in changes.deleted:
                assertTrue(not files.exists(f1) and not files.exists(f1), "bad patch? still exists after applying a delete", f1, f2)
            else:
                assertTrue(False, 'path not in any category?', f1, f2)

        # clean up changes
        getGitResults(f'git_reset_--hard')
        getGitResults(f'git_clean_-fdx')
        getGitResults(f'git_co', [restoreBranch])
        getGitResults(f'git_branch_-D_gitptemp-confirmmerge', okIfErrTxt='not found.')

def grabWorkingVersionsOfFiles(root, tmproot, changes, callback):
    for path in changes.allchanged.keys():
        assertTrue(not path.startswith('/') and not path.startswith('\\') and not os.path.isabs(path))
        f1 = files.join(root, path)
        # double-check that the change looks like we expect
        if path in changes.modified:
            assertWarn(files.exists(f1), "expected to exist", f1)
            callback(f1, path)
        elif path in changes.added:
            assertWarn(files.exists(f1), "expected to exist", f1)
            callback(f1, path)
        elif path in changes.deleted:
            assertWarn(not files.exists(f1), "expected to not exist", f1)
        else:
            assertTrue(False, 'path not in any category?', f1)

def grabBaseVersionsOfFiles(root, tmproot, changes, callback):
    basis = basisCommits[root]
    with ChangeCurrentDirectory(tmproot) as cd:
        restoreBranch = getRestoreBranchAndPrepMoveBranch(tmproot)
        getGitResults(f'git_branch_-D_gitptemp-getbaseversions', okIfErrTxt='not found.')
        getGitResults(f'git_co', [basis])
        getGitResults(f'git_co_-b_gitptemp-getbaseversions')

        for path in changes.allchanged.keys():
            assertTrue(not path.startswith('/') and not path.startswith('\\') and not os.path.isabs(path))
            f1 = files.join(root, path)
            f2 = files.join(tmproot, path)
            # double-check that the change looks like we expect
            if path in changes.modified:
                assertWarn(not compareTwoFiles(f1, f2), "expected modified, but these look the same", f1, f2)
                callback(f2, path)
            elif path in changes.added:
                assertWarn(files.exists(f1) and not files.exists(f2), "expected to exist on left but not right", f1, f2)
            elif path in changes.deleted:
                assertWarn(not files.exists(f1) and files.exists(f2), "expected to exist on right but not left", f1, f2)
            else:
                assertTrue(False, 'path not in any category?', f1, f2)

        # clean up changes
        getGitResults(f'git_reset_--hard')
        getGitResults(f'git_clean_-fdx')
        getGitResults(f'git_co', [restoreBranch])
        getGitResults(f'git_branch_-D_gitptemp-getbaseversions', okIfErrTxt='not found.')


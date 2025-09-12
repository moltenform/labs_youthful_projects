# Ben Fisher
# Copyright (c) 2021, MIT License

from .gitp_gitutil import *
from .gitp_withorigin import *
import zipfile
import time

def showGitkAndReset():
    from . import fixgitk
    if runFixForGitk:
        fixgitk.go()
    files.run('git_add_-A'.split('_')) # stage them so new files show up
    files.run('git_commit_--no-verify_-m_(Temporary commit for diffing)'.split('_')) # commit them so gitk shows it first
    showInSeparateThreadAndContinue(['gitk'], 10)
    files.deleteSure(getTrueTmp() + '/gitp_is_prob_running')
    files.run('git_reset_HEAD~'.split('_'))

def gitpTop_DiffPrev():
    root, tmproot = determineRootPaths(checkTempRepo=True)
    basis = basisCommits[root]
    moveAllChangesLeftToRight(root, tmproot, basis)
    
    # run gitk
    with ChangeCurrentDirectory(tmproot) as cd:
        showGitkAndReset()
        # clean up
        files.run('git_reset_--hard'.split('_'))
        # kill newly added files
        files.run('git_clean_-fdx'.split('_'))

def gitpTop_DiffMain():
    determineRootPaths(checkTempRepo=False)
    showGitkAndReset()

def gitpTop_RunCommitHook():
    determineRootPaths(checkTempRepo=False)
    files.run('git_add_-A'.split('_'))
    files.run('git_commit_-m_tmp'.split('_'))
    files.run('git_reset_HEAD~'.split('_'))

def gitpTop_Pack(projname=None, desc=None):
    root, tmproot = determineRootPaths(checkTempRepo=True)
    outDir, tmpDir = getOutDirs()
    changes = getUnstagedFiles()
    if len(changes.allchanged) == 0:
        trace('Nothing to pack, no changes seen')
        return
    
    proj = projname if projname else chooseProjName()
    shortdesc, longdesc = desc.split(':', 1) if desc else promptPacketName()
    filename = getLatestProjCount(proj, shortdesc)
    fullfilename = outDir + '/' + filename
    assertTrue(not files.isFile(fullfilename), "file already exists")
    with zipfile.ZipFile(fullfilename, 'w') as zip:
        patchPath = gitpTop_Pack_AddPatch(root, tmproot, zip)
        gitpTop_Pack_ConfirmMergeApplies(root, tmproot, patchPath, changes)
        gitpTop_Pack_GrabFiles(root, tmproot, zip, changes)
        gitpTop_Pack_AddManifest(zip, changes, root, tmproot, proj, shortdesc, longdesc, fullfilename)
    trace(f"complete. wrote to {fullfilename}")


def gitpTop_Pack_AddManifest(zip, changes, root, tmproot, proj, shortdesc, longdesc, fullfilename):
    manifest = {}
    manifest['gitp'] = 1
    manifest['gitpversion'] = '0,1'
    manifest['description'] = f"{proj}:{shortdesc}:{longdesc}"
    manifest['basis'] = basisCommits[root]
    manifest['packname'] = fullfilename
    manifest['branch1'] = getGitResults('git_branch_--show-current')
    manifest['fullpath1'] = root
    with ChangeCurrentDirectory(tmproot) as cd:
        manifest['branch2'] = getGitResults('git_branch_--show-current')
        manifest['fullpath2'] = tmproot

    manifest['time'] = getNowAsMillisTime()
    manifest['humantime'] = renderMillisTime(manifest['time'])
    manifest['modified'] = sorted(list(changes.modified.keys()))
    manifest['added'] = sorted(list(changes.added.keys()))
    manifest['deleted'] = sorted(list(changes.deleted.keys()))
    fnLastLine = lambda s: getGitResults(s).strip().split('\n')[-1].strip()
    try:
        for i in range(1, 10):
            # add recent commit messages to add the context for where we're at
            manifest[f'adjacentCommits{i}'] = fnLastLine('git_log_-' + str(i) + '_--pretty=%H')
            manifest[f'adjacentCommitstTime{i}'] = fnLastLine('git_log_-' + str(i) + '_--pretty=%cD')
            manifest[f'adjacentCommitstUnix{i}'] = fnLastLine('git_log_-' + str(i) + '_--pretty=%ct')
            manifest[f'adjacentCommitsMsg{i}'] = getCommitMsgFromCommitId(manifest[f'adjacentCommits{i}'])
    except Exception as e:
        assertTrue('retcode is not 0' in str(e), e)
        warn("Could not get commit context. Maybe this is a very young repo.")
    manifestTxt = json.dumps(manifest, indent=2)
    addTextToZip(zip, 'manifest.txt', manifestTxt)

def gitpTop_ShowDescription(verbose=False, fullpathtozip=None):
    fullpathtozip, zip = askUserForPathToGitp(fullpathtozip)
    with zip:
        manifest = getManifestJsonFromZip(zip, 'manifest.txt')
        if verbose:
            keys = list(manifest.keys())
        else:
            keys = 'description,basis,fullpath1,fullpath2,branch2,humantime,modified,added,deleted'.split(',')
        trace('filpath=', fullpathtozip)
        for key in keys:
            val = manifest[key]
            if isinstance(val, list):
               trace(f'{key}=', '\n' + '\n\t'.join(manifest[key]))
            else:
               trace(f'{key}={manifest[key]}')

def askUserForPathToGitp(fullpathtozip=None):
    if not fullpathtozip:
        outDir, tmpDir = getOutDirs()
        proj = chooseExistingProjName(outDir)
        fullpathtozip = choosePacket(proj, outDir)
    return fullpathtozip, zipfile.ZipFile(fullpathtozip)

def gitpTop_Pack_GrabFiles(root, tmproot, zip, changes):
    def addToZipWithSuffix(zip, realPath, shortenedPath, suffix):
        shortenedPath = transformPath(shortenedPath, suffix)
        addFileToZip(zip, shortenedPath, realPath)
    grabBaseVersionsOfFiles(root, tmproot, changes, lambda longpath, shortpath:addToZipWithSuffix(zip, longpath, shortpath, '000'))
    grabWorkingVersionsOfFiles(root, tmproot, changes, lambda longpath, shortpath:addToZipWithSuffix(zip, longpath, shortpath, '001'))

def gitpTop_Pack_AddPatch(root, tmproot, zip):
    # create the patch on in the tmproot. we could do it in the root,
    # but nice not to cause a re-compile in case you're currently running code in root.
    # note: this has to be kinda verbose because format-patch wants things to be in a branch 
    basis = basisCommits[root]
    outdir, tmpdir = getOutDirs()
    with ChangeCurrentDirectory(tmproot) as cd:
        restoreBranch = getRestoreBranchAndPrepMoveBranch(tmproot)
        getGitResults('git_branch_-D_gitptemp-workingforpatchgen1', okIfErrTxt='not found.')
        getGitResults('git_branch_-D_gitptemp-workingforpatchgen2', okIfErrTxt='not found.')
        getGitResults('git_co', [basis])
        getGitResults('git_co_-b_gitptemp-workingforpatchgen1')
        getGitResults('git_co_-b_gitptemp-workingforpatchgen2')
        moveAllChangesLeftToRight(root, tmproot, basis)
        assertGitPacket(not areThereStagedFiles(), f"expect no staged files in {tmproot}")
        assertGitPacket(areThereUnstagedFiles(), f'expect some unstaged files in {tmproot}')
        getGitResults('git_add_-A')
        assertGitPacket(areThereStagedFiles(), f"expect staged files in {tmproot}")
        assertGitPacket(not areThereUnstagedFiles(), f'expect no unstaged files in {tmproot}')
        getGitResults('git_commit_-m_gitptemp-workingforpatchgentmp_--no-verify')
        assertGitPacket(not areThereStagedFiles(), f"expect no staged files in {tmproot}")
        assertGitPacket(not areThereUnstagedFiles(), f'expect no unstaged files in {tmproot}')
        patchPath = makeAFormatPatchInTmpDir(tmpdir, 'gitptemp-workingforpatchgen1')
        getGitResults('git_co', [restoreBranch])
        assertTrue(files.exists(patchPath) and files.getSize(patchPath) > 0, 'not found or empty', patchPath)
        addFileToZip(zip, 'patch.patch', patchPath)
        getGitResults('git_branch_-D_gitptemp-workingforpatchgen1', okIfErrTxt='not found.')
        getGitResults('git_branch_-D_gitptemp-workingforpatchgen2', okIfErrTxt='not found.')
    
    return patchPath

def moveAllChangesLeftToRight(root, tmproot, basis):
    with ChangeCurrentDirectory(tmproot) as cd:
        assertGitPacket(not areThereStagedFiles(), f"please have no staged files in {tmproot}")
        assertGitPacket(not areThereUnstagedFiles(), f'please have no unstaged files in {tmproot}')

    # all potential mods on left side
    filesMod = {}
    with ChangeCurrentDirectory(root) as cd:
        for path in getUnstagedFiles().allchanged:
            filesMod[path] = True

    # all potential mods on right side
    with ChangeCurrentDirectory(tmproot) as cd:
        filesMod.update(allFilesPotentiallyModifiedSinceCommit(basis))
    
    # copy all from left to right (same results but faster than rsync)
    makeDestLookExactlyLikeSrc(root, tmproot, list(filesMod.keys()))

def gitpTop_SlowCopyToAltRepo():
    defaultSrc, defaultDest = getDirPairWithoutValidating()
    src = rinput(f'Enter full path to a source dir:\ndefault={defaultSrc}\n')
    src = src if src else defaultSrc
    dest = rinput(f'Enter full path to a dest dir:\ndefault={defaultDest}\n')
    dest = dest if dest else defaultDest
    if not files.exists(dest) and getInputBool('create dest directory?'):
        files.makeDirs(dest)
    files.runRsync(src, dest, deleteExisting=True, linExcludeRelative=[
        '.git', 'node_modules', 'dist'
    ])

def getDirPairWithoutValidating():
    dir = os.path.abspath(os.getcwd())

    defaultSrc = dir
    defaultDest = ''
    isADestWithMainRepo = ''
    for k in tempRepos:
        if dir == k:
            defaultDest = tempRepos[k]
        elif dir == tempRepos[k]:
            # we're in an alt repo sending to main
            defaultDest = k
    return defaultSrc, defaultDest

def gitpTop_CdAltRepo():
    defaultSrc, defaultDest = getDirPairWithoutValidating()
    assertGitPacket(defaultSrc and defaultDest, "directory not found")
    files.writeAll(getTrueTmp() + '/changedir.txt', defaultDest)

def gitpTop_CopyToAltRepo():
    root, tmproot = determineRootPaths(checkTempRepo=True)
    basis = basisCommits[root]
    if getInputBool(f'Move files from\n{root}\nto\n{tmproot}\n?'):
        moveAllChangesLeftToRight(root, tmproot, basis)

def gitpTop_Apply_ApplyAndCheck(fullpathtozip=None, alwaysToRoot=False):
    outDir, tmpDir = getOutDirs()
    assertWarn(not areThereStagedFiles(), "There are staged files, are you sure?")
    assertWarn(not areThereUnstagedFiles(), "There are unstaged files, are you sure?")
    fullpathtozip, zip = askUserForPathToGitp(fullpathtozip)
    with zip:
        assertTrue(not files.exists(tmpDir + '/patch.patch'))
        zip.extract('patch.patch', tmpDir)
        assertTrue(files.exists(tmpDir + '/patch.patch'))

        manifest = getManifestJsonFromZip(zip, 'manifest.txt')
        if shasMatch(manifest["basis"], currentCommitId()):
            expectPerfectApply = True
        else:
            trace(f'This patch was developed against commmit id {manifest["basis"]}. We recommend that you go ' + 
                'to that commit now and then use "merge" as needed.')
            expectPerfectApply = getInputBool("Did you go to this commit? Optional. y/n")
        
        if alwaysToRoot:
            useMainRoot = True
        else:
            choices = ['main repo', 'alt (b) repo']
            i, chosen = getInputFromChoices('Choose a repo', choices)
            assertGitPacket(i >= 0, "User canceled.")
            useMainRoot = i == 0

        root, tmproot = determineRootPaths(checkTempRepo=not useMainRoot)
        dirToUse = root if useMainRoot else tmproot
        with ChangeCurrentDirectory(dirToUse) as cd:
            applyPatch(tmpDir + '/patch.patch', dirToUse)
            # check results
            warnings = []
            for path in manifest['deleted']:
                if files.exists(path):
                    warnings.append(f'patch intended to delete path {path} but it still exists')
            for path in manifest['added']:
                if not files.exists(path):
                    warnings.append(f'patch intended to add path {path} but not found')
                else:
                    if not compareWithFileInZip(zip, path, '001'):
                        warnings.append(f'patch intended to add path {path} and it is not the same')
            for path in manifest['modified']:
                if not files.exists(path):
                    warnings.append(f'patch intended to modify path {path} but not found')
                else:
                    if expectPerfectApply and not compareWithFileInZip(zip, path, '001'):
                        warnings.append(f'patch intended to modify path {path} and it is not the same')
    if warnings:
        trace('warnings:' + '\n'.join(warnings))

def gitpTop_ShowPacketDiff(fullpathtozip=None):
    fullpathtozip, zip = askUserForPathToGitp(fullpathtozip)
    with zip:
        outDir, tmpDir = getOutDirs()
        fakeRepo = tmpDir + '/fakeRepo'
        manifest = getManifestJsonFromZip(zip, 'manifest.txt')
        trace('Setting up a fake repo in: ' + fakeRepo)
        files.ensureEmptyDirectory(fakeRepo)
        partsV0 = jslike.filter(zip.namelist(), lambda s: s.split('.')[-2:-1] == ['000'])
        partsV1 = jslike.filter(zip.namelist(), lambda s: s.split('.')[-2:-1] == ['001'])
        with ChangeCurrentDirectory(fakeRepo) as cd:
            getGitResults('git_init')
            for part in partsV0:
                wroteTo = zip.extract(part, fakeRepo)
                files.move(wroteTo, withoutNumber(wroteTo), True)
            for path in manifest['deleted']:
                onDisk = withoutNumber(transformPath(path, '000'))
                files.writeAll(onDisk, '(File was deleted)')
            getGitResults('git_add_-A')
            getGitResults('git_commit_-m_beforepatch')
            for part in partsV1:
                wroteTo = zip.extract(part, fakeRepo)
                files.move(wroteTo, withoutNumber(wroteTo), True)
            for path in manifest['deleted']:
                onDisk = withoutNumber(transformPath(path, '000'))
                files.deleteSure(onDisk)
            showGitkAndReset()
        files.ensureEmptyDirectory(fakeRepo)

def gitpTop_ViewRecent():
    outDir, tmpDir = getOutDirs()
    fls = jslike.filter(filesSortedInverseLmt(outDir),
        lambda s: s[1].endswith('.gitp.zip') and not 'testgitptest' in s[1])
    
    fls = jslike.map(fls, lambda s: s[1])[0:10]
    trace("We've recently made patches:\n\t" + '\n\t'.join(fls))
    trace("We've recently made patches for these projects:\n\t" + '\n\t'.join(getProjs(outDir)))

def gitpTop_ResetHardAll():
    assertGitPacket(not areThereStagedFiles(), "did not expect staged files, please git reset them", os.getcwd())
    if getInputBool('Are you sure you want to delete unsaved changes?'):
        getGitResults('git_reset_--hard')
        fls = getUnstagedFiles()
        for k in fls.added:
            files.delete(k, doTrace=True)

def gitpTop_UpdateBasis():
    root, tmproot = determineRootPaths(checkTempRepo=True)
    trace(f'To update basis, do the following:')
    steps = []
    steps.append(f'=== First, get working changes ===')
    steps.append(f'cd {tmproot}')
    steps.append(f'git co <publicworkingbranch>')
    steps.append(f'git branch -D gitptemp-devforupdate')
    steps.append(f'git branch -D gitptemp-publicwithdevupdates')
    steps.append(f'git branch -D gitptemp-devforworking')
    steps.append(f'git co -b gitptemp-devforworking')
    steps.append(f'rsync -az --delete-after {root}/src {tmproot}/src')
    steps.append(f'git commit -m gotworkingchanges --no-verify')
    steps.append(f'=== Then merge in changes from {mainBranch(root)} ===')
    steps.append(f'git co {mainBranch(root)}')
    steps.append(f'git pull origin {mainBranch(root)}')
    steps.append(f'git co <desired-basis-commit-id>')
    steps.append(f'git co -b gitptemp-devforupdate')
    steps.append(f'git co <publicworkingbranch>')
    steps.append(f'git merge gitptemp-devforupdate (and resolve conflicts)')
    steps.append(f'git co -b gitptemp-publicwithdevupdates')
    steps.append(f'git merge gitptemp-devforworking (and resolve conflicts)')
    steps.append(f'cd {root}')
    steps.append(f'git co {mainBranch(root)}')
    steps.append(f'git pull origin {mainBranch(root)}')
    steps.append(f'git co <desired-basis-commit-id>')
    steps.append(f'git branch -d gpworking')
    steps.append(f'git co -b gpworking')
    steps.append(f'rsync -az --delete-after {tmproot}/src {root}/src')
    steps.append(f'=== Then clean up repo ===')
    steps.append(f'cd {tmproot}')
    steps.append(f'git co <publicworkingbranch>')
    steps.append(f'rsync -az --delete-after {root}/src {tmproot}/src')
    steps.append(f'git branch -D gitptemp-devforupdate')
    steps.append(f'git branch -D gitptemp-publicwithdevupdates')
    steps.append(f'git branch -D gitptemp-devforworking')
    steps.append(f'EDIT gitp_util.py and type in the new <desired-basis-commit-id>')
    steps.append(f'Now both root and tmproot are ready. you can optionally push changes ' + 
        'to origin, or you can git reset HEAD~ as many times as you want and re-update.')
    for i, step in enumerate(steps):
        trace(f'\t{i+1}) {step}')

import os
import sys

marker = '_!_rcells_!_'
join = os.path.join
exists = os.path.exists
openlogfile = None


# returns True on success, err string on failure
def renameFiles(sDirectory, aFrom, aTo):
    if len(aFrom) == 0 and len(aTo) == 0:
        return True
    if len(aFrom) != len(aTo):
        return u'Lengths do not match.'

    if isMarkerInFilename(aFrom) or isMarkerInFilename(aTo):
        return u"Cellrename can't rename a file with the string '%s' in its name." % marker

    ret = verifyThatFilenamesValid(aTo)
    if ret != True:
        return ret

    ret = verifyNoDuplicateFilenames(aTo)
    if ret != True:
        return ret

    # don't overwrite a file if it already exists
    ret = verifyNoConflictsWithExistingFiles(sDirectory, aFrom, aTo)
    if ret != True:
        return ret

    ret = verifyFilesCanBeRenamed(sDirectory, aFrom)
    if ret != True:
        return ret

    # rename files to 0,1,2,3
    writeToLog(u'Begin')
    for i in range(len(aFrom)):
        logRename(join(sDirectory, aFrom[i]), join(sDirectory, marker + str(i)))

    # rename files 0,1,2,3 to the output names
    for i in range(len(aTo)):
        sOutputName = join(sDirectory, aTo[i])
        if exists(sOutputName):
            return u'Error: did not expect to see a file "%s".' % sOutputName
        logRename(join(sDirectory, marker + str(i)), sOutputName)
    return True


# returns True on success, err string on failure
def verifyThatFilenamesValid(aTo):
    dictBadChars = {
        '\\': 1,
        '/': 1,
        ':': 1,
        '*': 1,
        '?': 1,
        '"': 1,
        '<': 1,
        '>': 1,
        '|': 1,
        '\0': 1
    }
    for filename in aTo:
        if not filename or any((char in dictBadChars for char in filename)):
            return u'Invalid filename: "%s"' % filename
    return True


# returns True on success, err string on failure
def verifyNoDuplicateFilenames(a):
    if os.name == 'nt':
        setOfFilenames = set(filename.lower() for filename in a)
    else:
        setOfFilenames = set(a)
    if len(setOfFilenames) != len(a):
        return u'Duplicate filename seen'
    else:
        return True


# returns True on success, err string on failure
def verifyNoConflictsWithExistingFiles(sDirectory, aFrom, aTo):
    if os.name == 'nt':
        setOfPrevNames = set(filename.lower() for filename in aFrom)
    else:
        setOfPrevNames = set(aFrom)
    for sOutputName in aTo:
        if os.name == 'nt':
            sOutputName = sOutputName.lower()
        if exists(join(sDirectory, sOutputName)) and sOutputName not in setOfPrevNames:
            return u"File '%s' already exists." % sOutputName
    return True


# returns True on success, err string on failure
# also catches the case where the input file no longer exists.
def verifyFilesCanBeRenamed(sDirectory, aFrom):
    # rename files to "temp" and back to safely see if they can be renamed.
    for i in range(len(aFrom)):
        sInputName = aFrom[i]
        try:
            os.rename(join(sDirectory, sInputName), join(sDirectory, marker + 'temp' + str(i)))
        except Exception:
            if not exists(join(sDirectory, sInputName)):
                return u"File '%s' no longer exists." % sInputName
            else:
                return u"File '%s' could not be renamed." % sInputName
        os.rename(join(sDirectory, marker + 'temp' + str(i)), join(sDirectory, sInputName))
    return True


def getLogFilename():
    import tempfile
    return join(tempfile.gettempdir(), 'cellrename.1.12_log.txt')


def writeToLog(s):
    global openlogfile
    if not openlogfile:
        import io
        f = getLogFilename()
        try:
            openlogfile = io.open(f, 'a', encoding='utf-8')
        except:
            return

    if sys.version_info[0] <= 2:
        s = unicode(s)

    openlogfile.write(u'\n')
    openlogfile.write(s)


def logRename(src, dest):
    s = u'Renaming\t%s\tto\t%s\t' % (src, dest)
    writeToLog(s)
    os.rename(src, dest)


def isMarkerInFilename(a):
    return any(marker in filename for filename in a)


if __name__ == '__main__':
    import unittests
    unittests.engineunittest()

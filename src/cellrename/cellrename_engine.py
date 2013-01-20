
import os
marker = '_!_rcells_!_'
join = os.path.join
exists = os.path.exists

# returns True on success, err string on failure
def renameFiles(strDirectory, afrom, ato):
    if len(afrom)==0 and len(ato)==0:
        return True
    if len(afrom)!=len(ato):
        return 'Lengths do not match.'
    
    if isMarkerInFilename(afrom) or isMarkerInFilename(ato):
        return "Cellrename can't rename a file with the string '%s' in its name."%marker
    
    ret = verifyThatFilenamesValid(ato)
    if ret != True: return ret
    
    ret = verifyNoDuplicateFilenames(ato)
    if ret != True: return ret
    
    #Don't overwrite a file if it already exists
    ret = verifyNoConflictsWithExistingFiles(strDirectory, afrom, ato)
    if ret != True: return ret
    
    ret = verifyFilesCanBeRenamed(strDirectory, afrom)
    if ret != True: return ret
    
    #Rename files to 0,1,2,3
    for i in range(len(afrom)):
        os.rename( join(strDirectory,afrom[i]), join(strDirectory,marker+str(i)))
    
    #Rename files 0,1,2,3 to the output names
    for i in range(len(ato)):
        outputfilename = join(strDirectory,ato[i])
        if exists(outputfilename):
            return 'Error: did not expect to see a file "%s".'
        os.rename( join(strDirectory,marker+str(i)), outputfilename)
    return True


# returns True on success, err string on failure
def verifyThatFilenamesValid(ato):
    dictBadChars = {'\\':1, '/':1, ':':1, '*':1, '?':1, '"':1, '<':1, '>':1, '|':1, '\0':1 }
    for filename in ato:
        if not filename or any( (char in dictBadChars for char in filename)):
            return 'Invalid filename: "%s"'%filename
    return True

# returns True on success, err string on failure
def verifyNoDuplicateFilenames(ato):
    if os.name=='nt': setobjto = set(filename.lower() for filename in ato)
    else: setobjto = set(ato)
    if len(setobjto)!=len(ato):
        return 'Duplicate filename seen'
    else:
        return True

# returns True on success, err string on failure
def verifyNoConflictsWithExistingFiles(strDirectory, afrom, ato):
    if os.name=='nt': setobjfrom = set(filename.lower() for filename in afrom)
    else: setobjfrom = set(afrom)
    for outputfilename in ato:
        if exists(join(strDirectory, outputfilename)) and outputfilename.lower() not in setobjfrom:
            return "File '%s' already exists."%outputfilename
    return True

# returns True on success, err string on failure
# also catches the case where the input file no longer exists.
def verifyFilesCanBeRenamed(strDirectory, afrom):
    # rename files to "temp" and back to safely see if they can be renamed.
    for i in range(len(afrom)):
        inputfilename = afrom[i]
        try:
            os.rename(join(strDirectory,inputfilename), join(strDirectory,marker+'temp'+str(i)))
        except:
            if not exists(join(strDirectory,inputfilename)):
                return "File '%s' no longer exists."%inputfilename
            else:
                return "File '%s' could not be renamed."%inputfilename
        os.rename(join(strDirectory,marker+'temp'+str(i)), join(strDirectory,inputfilename))
    return True

def isMarkerInFilename(ar):
    return any(marker in filename for filename in ar)
    

if __name__=='__main__':
    import unittests
    unittests.engineunittest()
    

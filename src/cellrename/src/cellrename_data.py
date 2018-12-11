
# cellrename, by Ben Fisher. GPLv3.
# https://github.com/moltenform/labs_youthful_projects/tree/master/src/cellrename

import os
import fnmatch

class CellRenameData(object):
    ''' CellRenameData holds a list of cellrenameitems.
    The grid UI will load an instance of this class and render it. '''
    data = None
    directory = None
    filter = None
    include_dirs = False
    def __init__(self, sDirectory, sFilter, bIncludeDirs=False):
        self.directory = sDirectory
        self.filter = sFilter
        self.include_dirs = bIncludeDirs
        self.refresh()
    
    def refresh(self):
        # load the filenames!
        self.data = []
        aFrom = os.listdir(self.directory)
        if self.filter:
            aFrom = self.fnmatch_case_insensitive(aFrom, self.filter)
        
        for sFilename in aFrom:
            # skip hidden files
            if sFilename.startswith('.'):
                continue
            
            st =  os.stat(os.path.join(self.directory, sFilename))
            isDirectory = os.path.isdir(os.path.join(self.directory, sFilename))
            if isDirectory and not self.include_dirs: 
                continue
            
            elem = CellRenameItem()
            elem.filename = elem.newname = sFilename
            elem.modifiedTime = st.st_mtime
            elem.creationTime = st.st_ctime
            
            # don't show the "file size" for directories
            elem.sizeRendered = renderSize(st.st_size) if not isDirectory else ' '
            
            # the "filesize" for directories is -1 so that they will sort above files
            elem.size = st.st_size if not isDirectory else -1
            
            if os.name == 'nt':
                # on Windows, this is a float, let's make it an int
                elem.modifiedTime = int(10000 * elem.modifiedTime)
                elem.creationTime = int(10000 * elem.creationTime)
            
            self.data.append(elem)
    
    def getLength(self): 
        return len(self.data)
    
    def fnmatch_case_insensitive(self, filenames, pattern):
        # by default, fnmatch treats Windows and posix differently, which I'd rather not have.
        # let's use case-insensitive matching everywhere.
        pattern = pattern.lower()
        return [name for name in filenames if fnmatch.fnmatch(name.lower(), pattern)]
    
    def sort(self, sField, bReverse=False):
        map = dict(filename=lambda elem: elem.filename,
            newname=lambda elem: elem.newname,
            size=lambda elem: elem.size,
            modifiedTime=lambda elem: elem.modifiedTime,
            creationTime=lambda elem: elem.creationTime)
    
        if sField not in map:
            raise 'Invalid field name.'

        self.data.sort(key=map[sField], reverse=bReverse)

    # transformations act on the new name, so that transformations can be chained
    # add a suffix or prefix. returns True on success.
    def transformSuffixOrPrefix(self, bPrefix, sAdded):
        for i in range(len(self.data)):
            if bPrefix:
                self.data[i].newname = sAdded + self.data[i].newname
            else:
                # probably want to add this before the file extension.
                name, ext = os.path.splitext(self.data[i].newname)
                self.data[i].newname = name + sAdded + ext

        return True
                
    # append a number. returns True on success or errstring on failure.
    def transformAppendNumber(self, sNumberExample):
        if not all((c in '0123456789' for c in sNumberExample)):
            return 'Must consist of numerical digits.'

        nNumber = int(sNumberExample, 10)
        for i in range(len(self.data)):
            # if numbers grow to large, we handle it gracefully.
            sAdded = padn(i + nNumber, len(sNumberExample))
            
            # probably want to add this before the file extension.
            name, ext = os.path.splitext(self.data[i].newname)
            self.data[i].newname = name + ' ' + sAdded + ext

        return True
        
    # set based on a pattern
    def transformWithPattern(self, sPattern):
        padlength = len(str(len(self.data))) # e.g. if there are 100s of files we should use 3 digits
        def titlecase(s):
            result = ''
            for i, c in enumerate(s):
                if i == 0 or not s[i - 1].isalpha():
                    result += c.upper()
                else:
                    result += c.lower()

            return result
        
        def subpattern(s, elem, i):
            name, ext = os.path.splitext(self.data[i].newname)
            s = s.replace('%N', str(i + 1))                       # raw number
            s = s.replace('%n', padn(i + 1, padlength))            # padded number    
            s = s.replace('%0', padn(i, padlength))              # start with 0.
            s = s.replace('%CT', str(self.data[i].creationTime)) # creation time
            s = s.replace('%MT', str(self.data[i].modifiedTime)) # modified time
            s = s.replace('%f', name)                           # name part
            s = s.replace('%F', self.data[i].newname)           # full name
            s = s.replace('%u', name.lower())                   # to uppercase
            s = s.replace('%U', name.upper())                   # to lowercase
            s = s.replace('%t', titlecase(name))                # to titlecase
            return s + ext

        for i in range(len(self.data)):
            self.data[i].newname = subpattern(sPattern, self.data[i], i)

        return True
    
    # replace in filename (case-sensitive!)
    def transformReplace(self, sSearch, sReplace):
        for elem in self.data:
            elem.newname = elem.newname.replace(sSearch, sReplace)
        return True
    
    # replace with regular expression
    def transformRegexReplace(self, sRe, sReplace, bUseRegexSymbols, bCaseSensitive):
        import re
        if not bUseRegexSymbols:
            sRe = re.escape(sRe)

        try:
            objre = re.compile(sRe) if bCaseSensitive else re.compile(sRe, re.IGNORECASE)
        except re.error:
            return 'Could not create regular expression.'

        for elem in self.data:
            try:
                elem.newname = objre.sub(sReplace, elem.newname)
            except re.error:
                return 'Could not apply regular expression.'
        return True
            
    def prepareLists(self):
        aFrom = [elem.filename for elem in self.data]
        aTo = [elem.newname for elem in self.data]
        return aFrom, aTo
        
    def __str__(self):
        return '\n'.join((str(elem) for elem in self.data))

class CellRenameItem(object):
    filename = None
    newname = None
    size = None
    sizeRendered = None
    modifiedTime = None
    creationTime = None
    def __str__(self):
        return '\t'.join([self.filename, self.newname, self.sizeRendered])

def renderSize(n):
    if n == 0:
        return '0 Kb'
    if n < 2 ** 10:
        return '1 Kb'
    elif n < 2 ** 20:
        return str(n // (2 ** 10)) + ' Kb'
    else:
        return str(n // (2 ** 20)) + ' Mb'

def padn(n, digits):
    s = str(n)
    while len(s) < digits:
        s = '0' + s
    return s

if __name__ == '__main__':
    import unittests
    unittests.dataunittest_transforms()
    unittests.dataunittest_files()

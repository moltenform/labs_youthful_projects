
COLS = {'filename':0, 'newname':1, 'size':2}

import os
import fnmatch


class SheetData():
	data = None #List of SheetDataEntry objects.
	directory = None
	filter = None
	includeDirs = False
	def __init__(self, strDirectory, strFilter, bIncludeDirs=False):
		self.directory = strDirectory
		self.filter = strFilter
		self.includeDirs = bIncludeDirs
		self.refresh()
	
	def refresh(self):
		# Load all of the filenames!
		self.data = []
		afrom = os.listdir(self.directory)
		if self.filter: afrom = fnmatch.filter(afrom, self.filter)
		for fname in afrom:
			if fname.startswith('.'): continue #don't use hidden files
			st =  os.stat(os.path.join(self.directory, fname))
			isDirectory = os.path.isdir(os.path.join(self.directory, fname))
			if isDirectory and not self.includeDirs: 
				continue
			
			elem = SheetDataEntry()
			elem.filename = elem.newname = fname
			elem.size = st.st_size if not isDirectory else -1 #for sorting purposes, sort dirs above
			elem.sizeRendered = renderSize(st.st_size) if not isDirectory else ' ' #if a directory, show space as " "
			elem.modifiedTime = st.st_mtime
			elem.creationTime = st.st_ctime
			
			self.data.append(elem)
	
	def getLength(self): return len(self.data)
	
	def reloadFromGrid(self, gridobj): #should be called often
		for i in range(len(self.data)):
			elem = self.data[i]
			elem.newname = gridobj.GetCellValue(i, COLS['newname'])
		
	def renderToGrid(self, gridobj):
		for i in range(len(self.data)):
			elem = self.data[i]
			gridobj.SetCellValue(i, COLS['filename'], elem.filename)
			gridobj.SetCellValue(i, COLS['newname'], elem.newname)
			gridobj.SetCellValue(i, COLS['size'], elem.sizeRendered)
		
	def sort(self, strField, bReverse = False):
		map = { 'filename':lambda elem: elem.filename,
			'newname':lambda elem: elem.newname,
			'size':lambda elem: elem.size,
			'modifiedTime':lambda elem: elem.modifiedTime,
			'creationTime':lambda elem: elem.creationTime,
			}
		
		if strField not in map: raise 'Invalid field name.'
		
		self.data.sort(key=map[strField], reverse=bReverse)
			
	#Transformations act on the new name, so that transformations can be chained
	
	# add a suffix or prefix
	def transform_suffixorprefix(self, bPrefix, strAdded):
		for i in range(len(self.data)):
			if bPrefix:
				self.data[i].newname = strAdded+self.data[i].newname
			else:
				# probably want to add this before the file extension.
				name, ext = os.path.splitext(self.data[i].newname)
				self.data[i].newname = name+strAdded+ext
				
	# add a number
	def transform_addnumber(self, strNumber):
		if not all((c in '0123456789' for c in strNumber)):
			return 'Must consist of numerical digits.'
		nNumber = int(strNumber, 10)
		for i in range(len(self.data)):
			# if numbers grow to large, we handle it gracefully.
			strAdded = padn(i+nNumber, len(strNumber))
			# probably want to add this before the file extension.
			name, ext = os.path.splitext(self.data[i].newname)
			self.data[i].newname = name+' '+strAdded+ext
		return True
		
	# set based on a pattern
	def transform_pattern(self, strPattern):
		padlength = len(str(len(self.data))) #number of digits, e.g. 200 files = 3 digits
		# %n=padded number (i.e. 001, 002)   %N=number   %f=file name   %U=uppercase name   %u=lowercase name
		def subpattern(strPattern, elem, i):
			namepart, extpart = os.path.splitext(self.data[i].newname)
			strPattern = strPattern.replace('%N', str(i))
			strPattern = strPattern.replace('%n', padn(i,padlength))
			strPattern = strPattern.replace('%f', name)
			strPattern = strPattern.replace('%u', namepart.lower())
			strPattern = strPattern.replace('%U', namepart.upper())
			return strPattern + extpart
		
		for i in range(len(self.data)):
			self.data[i].newname = subpattern(strPattern, self.data[i], i)
	
	# replace in filename (case-sensitive!)
	def transform_replace(self, strSearch, strReplace):
		for elem in self.data:
			elem.newname = elem.newname.replace(strSearch, strReplace)
	
	# replace with regular expression
	def transform_regexreplace(self, strRe, strReplace, bUseRegexSymbols, bCaseSensitive):
		import re
		if not bUseRegexSymbols: strRe = re.escape(strRe)
		try:
			objre = re.compile(strRe) if bCaseSensitive else re.compile(strRe, re.IGNORECASE)
		except:
			print 'Could not create regular expression.'; return
		for elem in self.data:
			elem.newname = objre.sub( strReplace, elem.newname)
	
			
	def prepareLists(self):
		afrom = [elem.filename for elem in self.data]
		ato = [elem.newname for elem in self.data]
		return afrom, ato
		
	def __str__(self):
		return '\n'.join( [str(elem) for elem in self.data])

class SheetDataEntry():
	filename = None
	newname = None
	size = None
	sizeRendered = None
	def __str__(self):
		return ' '.join((self.filename, self.newname, self.sizeRendered))

def renderSize(n):
	if n==0:
		return '0 Kb'
	if n<2**10:
		return '1 Kb'
	elif n<2**20:
		return str(n//(2**10)) + ' Kb'
	else:
		return str(n//(2**20)) + ' Mb'

def padn(n,digits):
	s = str(n)
	while len(s)<digits:
		s = '0'+s
	return s


#unit tests
import exceptions
def expectThrow(fn, sExpectedError, TypeException=exceptions.RuntimeError):
	try:
		fn()
	except TypeException,e:
		sError = str(e).split('\n')[-1]
		if sExpectedError.lower() in sError.lower():
			print 'Pass:',sExpectedError, ' == ', sError
		elif TypeException==True:
			print 'Pass:',' ', sError
		else:
			print 'Fail: expected msg',sExpectedError,'got',sError
	else:
		print 'Fail: expected to throw! '+sExpectedError

def expectEqual(v, vExpected):
	if v != vExpected:
		print 'Fail: Expected '+str(vExpected) + ' but got '+str(v)
		raise exceptions.RuntimeError, 'stop'
	else:
		print 'Pass: '+str(vExpected) + ' == '+str(v)

def expectNotEqual(v, vExpected):
	if v == vExpected:
		print 'Fail: Expected '+str(vExpected) + ' not to equal '+str(v)
		raise exceptions.RuntimeError, 'stop'
	else:
		print 'Pass: '+str(vExpected) + ' != '+str(v)

def test1():
	# testing things. let's use real files!
	names = '''.gitconfig,1,2.gif,3__3_,_4__4.gif,a picture.JPG,first,second.gif,noext another with no ext,noext1,picture 2.jpg,picture and picture.jpG,test.doc,the PiCture with caps.jpg,'''
	
	


if __name__=='__main__':
	print 'abcdef'[2:]
	#~ print os.path.splitext('a/b/ctxt')
	#~ a = SheetData('.','*.*')
	#~ a.transform_regexp('([^_]+)_([^_]+)','\\2!!\\1')
	#~ print a
	
	
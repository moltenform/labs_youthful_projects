
import os
import fnmatch

# the grid ui will load a cellrenamedata object and render it.
class CellRenameData():
	data = None # list of cellrenameitem
	directory = None
	filter = None
	includeDirs = False
	def __init__(self, strDirectory, strFilter, bIncludeDirs=False):
		self.directory = strDirectory
		self.filter = strFilter
		self.includeDirs = bIncludeDirs
		self.refresh()
	
	def refresh(self):
		# load all of the filenames!
		self.data = []
		afrom = os.listdir(self.directory)
		if self.filter: afrom = fnmatch.filter(afrom, self.filter)
		for fname in afrom:
			if fname.startswith('.'): continue #don't use hidden files
			st =  os.stat(os.path.join(self.directory, fname))
			isDirectory = os.path.isdir(os.path.join(self.directory, fname))
			if isDirectory and not self.includeDirs: 
				continue
			
			elem = CellRenameItem()
			elem.filename = elem.newname = fname
			elem.size = st.st_size if not isDirectory else -1 #for sorting purposes, sort dirs above
			elem.sizeRendered = renderSize(st.st_size) if not isDirectory else ' ' #if a directory, show space as " "
			elem.modifiedTime = st.st_mtime
			elem.creationTime = st.st_ctime
			if os.name=='nt':
				#on Windows, this is a float, let's make it an int
				elem.modifiedTime = int(10000*elem.modifiedTime)
				elem.creationTime = int(10000*elem.creationTime)
			
			self.data.append(elem)
	
	def getLength(self): 
		return len(self.data)
	
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
	# add a suffix or prefix. returns True on success.
	def transform_suffixorprefix(self, bPrefix, strAdded):
		for i in range(len(self.data)):
			if bPrefix:
				self.data[i].newname = strAdded+self.data[i].newname
			else:
				# probably want to add this before the file extension.
				name, ext = os.path.splitext(self.data[i].newname)
				self.data[i].newname = name+strAdded+ext
		return True
				
	# add a number. returns True on success or errstring on failure.
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
		def subpattern(s, elem, i):
			namepart, extpart = os.path.splitext(self.data[i].newname)
			s = s.replace('%N', str(i+1))
			s = s.replace('%n', padn(i+1,padlength))
			s = s.replace('%0', padn(i,padlength))
			s = s.replace('%CT', str(self.data[i].creationTime))
			s = s.replace('%MT', str(self.data[i].modifiedTime))
			s = s.replace('%f', namepart)
			s = s.replace('%F', self.data[i].newname)
			s = s.replace('%u', namepart.lower())
			s = s.replace('%U', namepart.upper())
			return s + extpart
		
		for i in range(len(self.data)):
			self.data[i].newname = subpattern(strPattern, self.data[i], i)
		return True
	
	# replace in filename (case-sensitive!)
	def transform_replace(self, strSearch, strReplace):
		for elem in self.data:
			elem.newname = elem.newname.replace(strSearch, strReplace)
		return True
	
	# replace with regular expression
	def transform_regexreplace(self, strRe, strReplace, bUseRegexSymbols, bCaseSensitive):
		import re
		if not bUseRegexSymbols: strRe = re.escape(strRe)
		try:
			objre = re.compile(strRe) if bCaseSensitive else re.compile(strRe, re.IGNORECASE)
		except:
			return 'Could not create regular expression.'
		for elem in self.data:
			elem.newname = objre.sub( strReplace, elem.newname)
		return True
	
			
	def prepareLists(self):
		afrom = [elem.filename for elem in self.data]
		ato = [elem.newname for elem in self.data]
		return afrom, ato
		
	def __str__(self):
		return '\n'.join((str(elem) for elem in self.data))

class CellRenameItem():
	filename = None
	newname = None
	size = None
	sizeRendered = None
	modifiedTime = None
	creationTime = None
	def __str__(self):
		return '\t'.join([self.filename, self.newname, self.sizeRendered])

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


if __name__=='__main__':
	import unittests
	unittests.dataunittest_transforms()
	unittests.dataunittest_files()

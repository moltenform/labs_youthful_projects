
COLS = {'filename':0, 'newname':1, 'size':2,'creationTime':3,'modifiedTime':4}

import os
import fnmatch
import time

#At all times the ordering visible should be the ordering in the SheetData.data

class SheetData():
	nrows = 0
	data = None #List of SheetDataEntry objects.
	directory = None
	filter = None
	length=0
	def __init__(self, strDirectory, strFilter, bIncludeDirs=False):
		self.directory = strDirectory
		self.filter = strFilter
		self.includeDirs = bIncludeDirs
		self.refresh()
	
	def refresh(self):
		#Get all of the data!
		self.data = []
		afrom = os.listdir(self.directory)
		afrom = [fname for fname in afrom if fnmatch.fnmatch(fname, self.filter)]
		for fname in afrom:
			if fname.startswith('.'): continue #don't use hidden files
			st =  os.stat(os.path.join(self.directory, fname))
			isDirectory = st[0]&0x4000
			if isDirectory and not self.includeDirs: 
				continue
			
			elem = SheetDataEntry()
			elem.filename = elem.newname = fname
			elem.size = st.st_size if not isDirectory else -1 #for sorting purposes, sort dirs above
			elem.sizeRendered = renderSize(st.st_size) if not isDirectory else ' ' #if a directory, show space as " "
			elem.modifiedTime = st.st_mtime
			elem.modifiedTimeRendered = renderTime(st.st_mtime)
			elem.creationTime = st.st_ctime
			elem.creationTimeRendered = renderTime(st.st_ctime)
			
			self.data.append(elem)
		self.length = len(self.data)
	
	def reloadFromGrid(self, gridobj): #should be called often
		for i in range(len(self.data)):
			elem = self.data[i]
			elem.newname = gridobj.GetCellValue(i, COLS['newname'])
		
	def renderToGrid(self, gridobj):
		for i in range(len(self.data)):
			elem = self.data[i]
			gridobj.SetCellValue(i, COLS['filename'], elem.filename)
			gridobj.SetCellValue(i, COLS['newname'], elem.newname)
			gridobj.SetCellValue(i, COLS['modifiedTime'], elem.modifiedTimeRendered)
			gridobj.SetCellValue(i, COLS['creationTime'], elem.creationTimeRendered)
			gridobj.SetCellValue(i, COLS['size'], elem.sizeRendered)
		
	def sort(self, strField, bReverse = False):
		map = { 
			'filename':lambda elem: elem.filename,
			'newname':lambda elem: elem.newname,
			'modifiedTime':lambda elem: elem.modifiedTime,
			'creationTime':lambda elem: elem.creationTime,
			'size':lambda elem: elem.size
			}
		
		if strField not in map:
			raise 'Invalid field name.'
		
		self.data.sort(key=map[strField], reverse=bReverse)
			
	#Transformations act on the newname, so that transformations can be chained
	def transform_replace(self, strSearch, strReplace):
		for elem in self.data:
			elem.newname = elem.newname.replace(strSearch, strReplace)
		
	def transform_pattern(self, strPattern):
		padlength = len(str(len(self.data))) #number of digits, example: 200 files = 3 digits
		# Pattern syntax: %f=filename, %F=name, %E=extension, %U=uppercase, %u=lowercase, %n=number, %N=raw_number
		def subpattern(strPattern, elem, i):
			if '%f' in strPattern: strPattern = strPattern.replace('%f', elem.newname)
			if '%u' in strPattern: strPattern = strPattern.replace('%u', elem.newname.lower())
			if '%U' in strPattern: strPattern = strPattern.replace('%U', elem.newname.upper())
			if '%N' in strPattern: strPattern = strPattern.replace('%N', str(i))
			if '%n' in strPattern: strPattern = strPattern.replace('%n', padn(i,padlength))
			if '%F' in strPattern:
				if '.' in elem.newname: strPattern = strPattern.replace('%F',elem.newname.rsplit('.',1)[0])
				else: strPattern = strPattern.replace('%F', elem.newname)
			if '%E' in strPattern:
				if '.' in elem.newname: strPattern = strPattern.replace('%E',elem.newname.rsplit('.',1)[1])
				else: strPattern = strPattern.replace('%E', '')
			return strPattern
		
		for i in range(len(self.data)):
			self.data[i].newname = subpattern(strPattern, self.data[i], i)
		
	def transform_regexp(self, strRe, strReplace):
		import re
		try:
			objre = re.compile(strRe)
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
	# The rendered versions are stored as well, since they are unlikely to change
	filename = None
	newname = None
	
	modifiedTime = None
	modifiedTimeRendered = None
	creationTime = None
	creationTimeRendered = None
	
	size = None
	sizeRendered = None
	
	def __str__(self):
		return ' '.join((self.filename, self.newname, self.sizeRendered, self.modifiedTimeRendered, self.creationTimeRendered))

def renderSize(n):
	if n<2**10:
		return str(n) + 'B'
	elif n<2**20:
		return str(n/(2**10)) + 'K'
	else: #if n<2**30:
		return str(n/(2**20)) + 'Mb'

def padn(n,digits):
	s = str(n)
	while len(s)<digits:
		s = '0'+s
	return s

def renderTime(n):
	t = time.localtime(n)
	return time.strftime("%m/%d/%y %H:%M:%S", t)

if __name__=='__main__':
	#~ print os.listdir('.')
	#~ st =  os.stat('main.py')
	#~ modtime = time.localtime(st.st_mtime)
	#~ createtime = time.localtime(st.st_ctime)
	#~ print time.strftime("%m/%d/%y %H:%M:%S", modtime)
	#~ print time.strftime("%m/%d/%y %H:%M:%S", createtime)
	
	a = SheetData('.','*.*')
	a.sort('modifiedTime', True)
	a.transform_regexp('([^_]+)_([^_]+)','\\2!!\\1')
	print a
	
	
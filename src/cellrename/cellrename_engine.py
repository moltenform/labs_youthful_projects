import os
marker = '_!_rcells_!_'

def renameFiles(strDirectory, afrom, ato):
	if len(afrom)!=len(ato):
		return 'Lengths do not match.'
	if '' in ato:
		return 'Filename cannot be empty.'
	if checkDuplicates(afrom, ato):
		return 'Duplicate names in output file names.'
	if checkMarker(afrom, ato):
		return 'The string "'+marker+'" cannot be in a filename. Try to repair.'
	ret = checkFilesMissing(strDirectory, afrom, ato)
	if ret: return 'Not all input files '+ret+' can be found.'
	
	ret = checkOutputConflicts(strDirectory, afrom, ato) #If the file already exists and it's not an input
	if ret: return  'The file '+ret+' already exists.'
	
	ret = checkFilesCanBeRenamed(strDirectory, afrom)
	if ret: return  'The file '+ret+' cannot be renamed.'
		
	#Rename files to 0,1,2,3
	for i in range(len(afrom)):
		entry = afrom[i]
		os.rename( os.path.join(strDirectory,entry), os.path.join(strDirectory,marker+str(i)))
	
	import time
	time.sleep(0.5)
	
	#Rename files 0,1,2,3 to the output names
	for i in range(len(ato)):
		entry = ato[i]
		os.rename( os.path.join(strDirectory,marker+str(i)), os.path.join(strDirectory,entry))
	return True

def repair(strDirectory, anames):
	n=0
	for i in range(len(anames)):
		tempname = os.path.join(strDirectory, marker+str(i))
		if os.path.exists(tempname):
			os.rename(tempname, os.path.join(strDirectory, anames[i]))
			n+=1
	return n

def checkOutputConflicts(strDirectory, afrom, ato):
	import sys
	if sys.platform.startswith('win'): afromlower = [entry.lower() for entry in afrom] #Windows is case insensitive when checking if a file exists...
	else: afromlower = afrom
	for entry in ato:
		if os.path.exists(os.path.join(strDirectory, entry)) and entry.lower() not in afromlower:
			return entry
	return False

def checkFilesCanBeRenamed(strDirectory, afrom):
	# rename files to "temp" and back to safely see if they can be renamed.
	for i in range(len(afrom)):
		entry = afrom[i]
		try:
			os.rename( os.path.join(strDirectory,entry), os.path.join(strDirectory,marker+'temp'))
		except:
			return entry
		os.rename( os.path.join(strDirectory,marker+'temp'), os.path.join(strDirectory,entry))
	return False

def checkDuplicates(afrom, ato):
	dseen = {}
	for entry in ato:
		if entry in dseen:
			return True
		else:
			dseen[entry] = 1
	return False
	
def checkMarker(afrom, ato):
	for entry in afrom:
		if marker in entry: return True
	for entry in ato:
		if marker in entry: return True
	return False
	
def checkFilesMissing(dir, afrom, ato):
	for entry in afrom:
		if not os.path.exists(os.path.join(dir, entry)): return entry
		#~ if not os.access(os.path.join(dir, entry), os.W_OK): return True
		# Evidently write permissions aren't needed to rename something.
	return False

if __name__=='__main__':
	print checkDuplicates([],['a','b','c','d'])
	print checkDuplicates([],['a','b','a','c','d'])
	#~ print renameFiles('.',['jklkjl'],['kjkkjkj'])
	print renameFiles('test',['l.txt'],['l2.txt'])
	#~ print renameFiles('test',['test1.txt','test2.txt'],['t1.txt','t2.txt'])
	#~ print os.path.exists('test1.txt')

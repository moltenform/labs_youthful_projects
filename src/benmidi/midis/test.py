


import os,sys

sys.path.append('..\\bmidilib')
import bmidilib

#~ dir = '..\\midis'
#~ os.chdir(dir)

phase = 0 #run this file in two phases, first phase=0, then phase=1

files = os.listdir('.')
for file in files:
	if phase==1:
		if file.endswith('.mid'):
			print 'mf2t %s %s'%(file, file.replace('.mid', '.txt'))
	elif phase==0:
		if file.endswith('.mid') and not file.startswith('out_'):
			print file
			
			m = bmidilib.BMidiFile()
			m.open(file)
			m.read()
			m.close()
			
			m.open('out_'+file,'wb')
			m.write()
			m.close()
		
		
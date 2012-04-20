import sys, os
import re

import generate_writeparams
import generate_templates

#run devenv to build
#~ http://www.c-sharpcorner.com/UploadFile/tharakram/BuildDotNetSolution11162005052301AM/BuildDotNetSolution.aspx
PATHTOSLN = r'C:\pydev\mainsvn\chaos_cs\sdl_explore\sdl_2dmaps\fastpixelpic'
CfgFolder = r'C:\pydev\mainsvn\chaos_cs\sdl_explore\sdl_2dmaps\fastpixelpic\out\saves_gen'
if not os.path.exists(PATHTOSLN):
	#(running on the other machine)
	PATHTOSLN = r'C:\pydev\dogeneralbitmap\fastpixelpic'
	CfgFolder = r'C:\pydev\dogeneralbitmap\fastpixelpic\out\saves_gen'


if len(sys.argv)==1:
	sys.argv=['','samples\\sample-general2dst.ccc']


def main(filename):
	txt = ''
	try:
		txt = open(filename, 'r').read()
	except:
		print 'Error opening file.'
		sys.exit(1)
		return False
	txt = txt.replace('\r\n','\n')
	if '\n[code]' not in txt:
		print 'Error. [code] section not found in file.'
	if '\n[params]' not in txt:
		print 'Error. [params] section not found in file.'

	if '\n[code_diagram]' in txt:
		partBeforeCode, partCodem = txt.split('\n[code]')
		partCode, partDg1 = partCodem.split('\n[code_diagram]')
		partRanges, partParams = partDg1.split('\n[params]')
	else:
		partBeforeCode, partCodem = txt.split('\n[code]')
		partCode, partParams = partCodem.split('\n[params]')
		partCodeDiagram = ''
	#~ print 'c',partCode
	#~ print 'pdg', partCodeDiagram
	#~ print 'p', partParams
	
	
	#write parameters to there.
	generate_writeparams.writeParams(partParams, CfgFolder)
	
	#write code to the file.
	generate_templates.writeTemplates(partCode, PATHTOSLN)
	
	#get path BEFORE os.chdir!
	filename = os.path.abspath(filename)
	
	#compile
	os.chdir(PATHTOSLN) #necessary
	nStatus = os.system(os.path.join(PATHTOSLN, 'compile_msvc.bat'))
	
	if nStatus!=0:
		print 'Compilation error.'
		sys.exit(1)
		return False
	
	#run with the filepath.
	
	
	outdir = os.path.join(PATHTOSLN, 'out')
	exename = os.path.join(outdir, 'fastpixelpic.exe')
	os.chdir(outdir) #necessary
	#we pass the filename as a parameter, so changes can be written back.
	if False:
		#~ os.system(exename + ' "'+filename+'" -full')
		os.system(exename + ' "'+filename+'" -full')
	else:
		os.system(exename + ' "'+filename+'"') 
	

if __name__=='__main__':
	if len(sys.argv)!=2:
		print 'Arg 1 must be filename.'
		sys.exit(1)
	main(sys.argv[1])
	
	
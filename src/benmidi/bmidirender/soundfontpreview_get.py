"""
SoundFontPreview
Ben Fisher, 2009, GPL
halfhourhacks.blogspot.com
"""

import exceptions
import os
verbose = False

#tested with sfubar 0.9.
# Because this program uses its debugging information, other sfubar versions may not work.

class SoundFontInfo():
	type=None
	name = None
	date = None
	author = None
	product = None
	copyright = None
	comment = None
	
	presets = None
	def __init__(self):
		type = 'soundfont'
		self.presets = []
		#each is a SoundFontInfoPreset
	def __repr__(self):
		s = 'name: %s\n date: %s\n author: %s\n product: %s\n copyright: %s\n comment: %s'%(self.name,self.date,self.author,self.product,self.copyright, self.comment)
		for preset in self.presets: s+= '\n\tPreset '+str(preset)
		return s
		
class SoundFontInfoPreset():
	name=None
	bank=None
	presetNumber=None #a string, for now
	def __repr__(self):
		return '"%s"   bank: %s, instrument: %s'%(self.name, self.bank, self.presetNumber)


class SFInfoException(exceptions.Exception): pass

#specify mydirectory if not running from the same folder as sfubar.exe.
def getpresets(file, fbdirectory=None):
	sfubar = 'sfubar.exe' if fbdirectory==None else os.path.join(fbdirectory, 'sfubar.exe')
	if not os.path.exists(sfubar):
		raise SFInfoException('Could not find required file sfubar.exe')
	if not os.path.exists(file):
		raise SFInfoException('Could not find the soundfont...')
	if os.path.exists('out.txt'): os.unlink('out.txt')
	if os.path.exists('out.txt'): raise SFInfoException("Couldn't delete out.txt file.")
		
	os.system(sfubar+' --sfdebug "'+file.replace('"','\\"') +'" out.txt')
	
	try:
		f = open('out.txt','r')
	except IOError:
		raise SFInfoException("That soundfont might not exist. Couldn't open output file.")
	currentFont = SoundFontInfo()
	for line in f:
		if '=' in line:
			part1, part2 = line.strip().split('=',1)
			if part1=='RIFF.LIST.INAM.zstr':
				currentFont.name = part2
				if verbose: print 'Name: '+part2
			elif part1=='RIFF.LIST.ICRD.zstr':
				currentFont.date = part2
			elif part1=='RIFF.LIST.IENG.zstr':
				currentFont.author = part2
				if verbose: print 'By: '+part2
			elif part1=='RIFF.LIST.IPRD.zstr':
				currentFont.product = part2
			elif part1=='RIFF.LIST.ICOP.zstr':
				currentFont.copyright = part2
			elif part1=='RIFF.LIST.ICMT.zstr':
				currentFont.comment = part2
			else:
				if 'RIFF.LIST.phdr' in part1 and 'achPresetName' in part1:
					if part2=='EOP': 
						break
					if verbose: print '\n'+part2
						
					currentFont.presets.append( SoundFontInfoPreset() )
					currentFont.presets[-1].name=part2
				elif 'RIFF.LIST.phdr' in part1 and 'wPreset.' in part1:
					if verbose: print '\tProgram: '+part2
					currentFont.presets[-1].presetNumber= part2
				elif 'RIFF.LIST.phdr' in part1 and 'wBank.' in part1:
					if verbose:  print '\tBank: '+part2
					currentFont.presets[-1].bank= part2

	f.close()
	return currentFont

if __name__=='__main__':
	#~ verbose = True
	
	dir = 'soundfontpreview'
	o = getpresets(r'C:\Projects\midi\!midi_to_wave\soundfonts_trying\sfs\RolandChurchBells.sf2', dir)
	print o
	
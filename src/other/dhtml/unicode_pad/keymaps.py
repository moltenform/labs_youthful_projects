# Convert .py.js file into keymap in memory
import os

def get_available():
	dir = os.listdir('./keymaps')
	keymaps = [filename for filename in dir if filename.endswith('.py.js')]
	return keymaps

def parse(mapfilename = 'default.py.js'):
	f = open(os.path.join('keymaps', mapfilename))
	alltext = f.read()
	f.close()
	
	alltext = alltext.replace('\r\n','\n').replace('\n//', '\n#') #Convert // to #
	exec alltext
	# Now, the local variables modes, keys, and mapname will be created.
	
	dict_hotkeys = {}
	dict_modes = {}
	for mode in modes:
		mode = mode.replace('\t\t','\t').replace('\t\t','\t').replace('\t\t','\t').split('\t')
		modeChar = mode[0].split('=')[0]
		modeName = mode[0].split('=')[1]
		modeHotkey = mode[1]
		
		dict_modes[modeChar] = modeName
		dict_hotkeys[modeHotkey] = ('setmode', modeChar)
		
	
	for key in keys:
		key = key.replace('\t\t','\t').replace('\t\t','\t').replace('\t\t','\t').split('\t')
		hotkey = key[0]
		if key[1].startswith('0x'):
			hotvalue = int(key[1],16)
		else:
			hotvalue = int(key[1])
		if len(key)>2:
			hotdescription = key[2]  
		else:
			hotdescription = ''
		if hotkey in dict_hotkeys:
			print 'Warning: duplicate entry for hotkey '+str(hotkey)
		
		dict_hotkeys[hotkey] = ('char',hotvalue, hotdescription)
	
	return dict_modes, dict_hotkeys, mapname

if __name__ == '__main__':
	dict_modes, dict_hotkeys, mapname = parse()
	print dict_hotkeys


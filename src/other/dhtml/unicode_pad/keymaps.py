# Convert .py.js file into keymap in memory
import os

def parse():
	f = open(os.path.join('keymaps', 'current.py.js'))
	alltext = f.read()
	f.close()
	
	alltext = alltext.replace('\n//', '\n#') #Convert // to #
	exec alltext
	# Now, the local variables modes and keys will be created.
	
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
		hotvalue = int(key[1])
		hotdescription = key[2] if len(key)>2 else ''
		dict_hotkeys[hotkey] = ('char',hotvalue, hotdescription)
	
	return dict_modes, dict_hotkeys

if __name__ == '__main__':
	dict_modes, dict_hotkeys = parse()
	print dict_hotkeys


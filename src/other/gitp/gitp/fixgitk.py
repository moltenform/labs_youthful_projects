# Ben Fisher
# Copyright (c) 2021, MIT License

import sys
import os
from shinerainsoftsevenutil.standard import * # auto-transitioned
from shinerainsoftsevenutil.core import *

def go():
    if not sys.platform == 'darwin':
        return

    path = os.path.expandvars('$HOME/.config/git/gitk')
    if files.isFile(path):
        before = files.readAll(path, encoding='utf-8')
        after = before.replace('set geometry(state) zoomed', 
            'set geometry(state) normal')
        files.writeAll(path, after, encoding='utf-8')
    else:
        trace('Cannot fixgitk, settings not found.')

if __name__ == '__main__':
    go()

# Ben Fisher
# Copyright (c) 2021, MIT License

import sys
import os
from ben_python_common import *

def go():
    if not sys.platform == 'darwin':
        return

    path = os.path.expandvars('$HOME/.config/git/gitk')
    if files.isfile(path):
        before = files.readall(path, encoding='utf-8')
        after = before.replace('set geometry(state) zoomed', 
            'set geometry(state) normal')
        files.writeall(path, after, encoding='utf-8')
    else:
        trace('Cannot fixgitk, settings not found.')

if __name__ == '__main__':
    go()

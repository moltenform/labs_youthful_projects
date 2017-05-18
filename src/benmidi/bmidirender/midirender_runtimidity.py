
import os
import midirender_util

import sys
from bmidilib import bmidiplay

import tempfile
import traceback
from os import sep as os_sep
tempcfgfilename = tempfile.gettempdir() + os_sep + 'tmpcg.cfg'


class RenderTimidityMidiPlayer(bmidiplay.TimidityMidiPlayer):
    win_timiditypath = midirender_util.bmidirenderdirectory + os_sep + 'timidity'+ os_sep + 'timidity.exe'
    extraParameters = None
    cfgFile = None
        
    def setParameters(self, arParameters):
        self.extraParameters = arParameters
    def setConfiguration(self, strCfg, directoryForOldTimidity=None):
        if directoryForOldTimidity:
            self.win_timiditypath = midirender_util.bmidirenderdirectory + os_sep + 'timidity'+ os_sep + 'timidity95.exe'
            os.chdir(directoryForOldTimidity)
            self.cfgFile = '.tempmidirendercfg.cfg'
            filename = directoryForOldTimidity + os_sep + self.cfgFile
        else:
            filename = self.cfgFile = tempcfgfilename
        
        f=open(filename, 'w')
        f.write(strCfg)
        f.close()
    
    def _startPlayback(self, strMidiPath):
        import subprocess
        kwargs = {}
        if sys.platform.startswith('win'):
            tim = self.win_timiditypath
            kwargs['creationflags'] = 0x08000000
            
            # without these, get WindowsError Error 6 The handle is invalid
            # if running in bmidi_to_wav.exe or pythonw.exe context.
            kwargs['stderr'] = subprocess.PIPE
            kwargs['stdin'] = subprocess.PIPE
        else:
            tim = 'timidity'
        
        args = [tim]
        args.extend(self._additionalTimidityArgs())
        args.append(strMidiPath)
        
        try:
            self.process = subprocess.Popen(args, stdout=subprocess.PIPE, **kwargs)
        except EnvironmentError as e:
            traceback.print_exc()
            self.isPlaying = False
            raise bmidiplay.PlayMidiException('Could not play midi.\n Do you have Timidity installed?\n\n'+str(e))
            return
        
        self.process.wait()
        self.strLastStdOutput = self.process.stdout.read()
        self.process = None
        self.isPlaying = False
        # note that these lines should be run, even if signalStop() is called.
    
    def _additionalTimidityArgs(self):
        ret = []
        if self.cfgFile != None:
            ret.append('-c')
            ret.append(self.cfgFile)
        
        if self.extraParameters != None:
            ret.extend(self.extraParameters)
        
        return ret
    

def isTimidityInstalled():
    import subprocess
    try:
        process = subprocess.Popen(['timidity'], stdout=subprocess.PIPE) #will just display version info, etc. if suceeds
    except EnvironmentError as e:
        return False
    return True
    

if __name__=='__main__':
    try:
        from Tkinter import *
    except ImportError:
        from tkinter import *

    global mmplayer
    mmplayer = bmidiplay.TimidityMidiPlayer()
    
    def start(top):
        def start():
            global mmplayer
            mmplayer.playSynchronous(midirender_util.bmidirenderdirectory+'\\..\\midis\\tempotest.mid')
        
        def startsync():
            global mmplayer
            mmplayer.playAsync(midirender_util.bmidirenderdirectory+'\\..\\midis\\16keys.mid')
        
        def stop():
            global mmplayer
            mmplayer.signalStop()
        
        Label(text='Default, Without cfg file').pack()
        Button(text='startasync', command=startsync).pack()
        Button(text='startsync', command=start).pack()
        Button(text='stop', command=stop).pack()
        Label(text='With cfg file').pack()

    root = Tk()
    start(root)
    root.mainloop()

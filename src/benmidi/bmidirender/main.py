
"""
bmidi to wav
Ben Fisher, 2009, GPL
https://github.com/moltenform/labs_youthful_projects/tree/master/src/benmidi/README.md
"""

import threading
import os
import sys
import tempfile
try:
    from Tkinter import *
except ImportError:
    from tkinter import *

import midirender_util
import midirender_mixer
import midirender_audiooptions
import midirender_tempo
import midirender_consoleout
import midirender_choose_midi_voice
import midirender_playback
import midirender_runtimidity
import midirender_soundfont
import midirender_soundfont_info
from midirender_util import bmidilib, bmiditools, bmidirenderdirectory
from scoreview import scoreview, listview

clefspath = bmidirenderdirectory + os.sep + 'scoreview' + os.sep + 'clefs'
if not os.path.exists(clefspath):
    clefspath = bmidirenderdirectory + os.sep + '..' + os.sep + clefspath

if getattr(sys, 'frozen', False):
    sys.stdout = open(tempfile.gettempdir() + os.sep + 'bmidi_to_wav_tmpstdout.txt', 'w')
    sys.stderr = open(tempfile.gettempdir() + os.sep + 'bmidi_to_wav_tmpstderr.txt', 'w')

tempcfgfilename = tempfile.gettempdir() + os.sep + 'bmidi_to_wav_tmpcfg.cfg'

class App(object):
    def __init__(self, root):
        root.title('Bmidi to wav')
        root.protocol("WM_DELETE_WINDOW", self.onClose)
        self.top = root
        
        frameMain = pack(Frame(root), side=TOP, fill=BOTH, expand=True)
        self.lblFilename = pack(Label(frameMain, text='No file opened.'), side=TOP, anchor='w')
        self.icon0 = PhotoImage(data=icon0)
        self.icon1 = PhotoImage(data=icon1)
        self.icon2 = PhotoImage(data=icon2)
        
        frameDecogroup = Frame(frameMain, borderwidth=1, relief=GROOVE, padx=3, pady=3)
        self.sliderTime = Scale(frameDecogroup, orient=HORIZONTAL,resolution=0.1, width=5, from_=0, to_=0)
        self.sliderTime.pack(side=TOP, fill=X)
        
        frameBtns = Frame(frameDecogroup)
        self.btnPlay = pack(Button(frameBtns, image=self.icon0, text='Play', command=self.onBtnPlay), side=LEFT, padx=4)
        self.btnPause = pack(Button(frameBtns, image=self.icon1, text='Pause', command=self.onBtnPause), side=LEFT, padx=4)
        self.btnStop = pack(Button(frameBtns, image=self.icon2, text='Stop', command=self.onBtnStop, relief=SUNKEN), side=LEFT, padx=4)
        pack(Button(frameBtns, text='Choose SoundFont...', command=self.openSoundfontWindow), side=LEFT, padx=(70, 4))
        pack(Button(frameBtns, text='Save Wav...', command=self.onBtnSaveWave), side=LEFT, padx=4)
        
        frameBtns.pack(side=TOP, fill=X, pady=2)
        frameDecogroup.pack(side=TOP, fill=X)
        
        frameGrid = Frame(frameMain, borderwidth=1)
        frameGrid.pack(side=TOP,fill=BOTH, expand=True, anchor='w', pady=15)
        frameGrid.grid_columnconfigure(1, weight=1, minsize=20)
        
        self.haveDrawnHeaders = False
        self.isMidiLoaded = False
        
        self.objMidi = None
        self.frameGrid = frameGrid
        
        # save and reuse grid widgets created.
        self.gridwidgets = {} # index is tuple (row, column)
        self.gridbuttons = {} # index is tuple (row, column)
        self.listviews = {} # index is track number. 
        self.scoreviews = {}
        self.mixerWindow=None
        self.audioOptsWindow=None
        self.soundfontWindow=None
        self.consoleOutWindow=None
        self.currentSoundfont = [midirender_soundfont.getDefaultSoundfont()] # an array so we can pass it as a reference.
        self.create_menubar(root)
        self.clearModifications()
        self.player = midirender_playback.BMidiRenderPlayback(self.playCallbackToggleButton, self.playCallbackGetSlider, self.playCallbackSetSlider)
    
    def drawColumnHeaders(self):
        opts = {}
        Label(self.frameGrid, text='#', **opts).grid(row=0, column=0)
        Label(self.frameGrid, text='Track', **opts).grid(row=0, column=1)
        Label(self.frameGrid, text='Channel', **opts).grid(row=0, column=2)
        Label(self.frameGrid, text='Instrument', **opts).grid(row=0, column=3)
        Label(self.frameGrid, text='Begins', **opts).grid(row=0, column=4)
        Label(self.frameGrid, text='Notes', **opts).grid(row=0, column=5)
        Label(self.frameGrid, text=' ', **opts).grid(row=0, column=6)
        Label(self.frameGrid, text=' ', **opts).grid(row=0, column=7)
        Label(self.frameGrid, text=' ', **opts).grid(row=0, column=8)
        self.haveDrawnHeaders = True
    
    def create_menubar(self,root):
        root.bind('<Control-space>', self.onBtnPlay)
        root.bind('<Control-r>', self.onBtnSaveWave)
        root.bind('<Control-f>', self.openSoundfontWindow)
        root.bind('<Alt-F4>', lambda:root.quit)
        root.bind('<Control-o>', self.menu_openMidi)
        root.bind('<Control-S>', self.saveModifiedMidi)
        root.bind('<Control-m>', self.openMixerView)
        root.bind('<Control-p>', self.openAudioOptsWindow)
        menubar = Menu(root)
        
        menuFile = Menu(menubar, tearoff=0)
        menubar.add_cascade(label="File", menu=menuFile, underline=0)
        menuFile.add_command(label="Open MIDI", command=self.menu_openMidi, underline=0, accelerator='Ctrl+O')
        menuFile.add_separator()
        menuFile.add_command(label="Modify MIDI Events...", command=self.menuModifyRawMidi, underline=0)
        menuFile.add_separator()
        menuFile.add_command(label="Save Changes From Mixer...", command=self.saveModifiedMidi, underline=0, accelerator='Ctrl+Shift+S')
        menuFile.add_separator()
        menuFile.add_command(label="Exit", command=root.quit, underline=1)
        
        menuAudio = Menu(menubar, tearoff=0)
        menubar.add_cascade(label="Audio", menu=menuAudio, underline=0)
        menuAudio.add_command(label="Play", command=self.onBtnPlay, underline=0)
        menuAudio.add_command(label="Pause", command=self.onBtnPause, underline=2)
        menuAudio.add_command(label="Stop", command=self.onBtnStop, underline=0)
        menuAudio.add_separator()
        menuAudio.add_command(label="Audio Options...", command=self.openAudioOptsWindow, underline=6, accelerator='Ctrl+P')
        menuAudio.add_command(label="Change Tempo...", command=self.menu_changeTempo, underline=0)
        menuAudio.add_command(label="Transpose Pitch...", command=self.menu_changeTranspose, underline=0)
        menuAudio.add_command(label="Copy Options String", command=self.menuCopyAudioOptsString, underline=1)
        menuAudio.add_separator()
        menuAudio.add_command(label="Save Wav", command=self.onBtnSaveWave, underline=5, accelerator='Ctrl+R')
        menuAudio.add_separator()
        menuAudio.add_command(label="Choose SoundFont...", command=self.openSoundfontWindow, underline=13, accelerator='Ctrl+F')
        
        self.objOptionsDuration = IntVar()
        self.objOptionsDuration.set(0)
        self.objOptionsBarlines = IntVar()
        self.objOptionsBarlines.set(1)
        menuView = Menu(menubar, tearoff=0)
        menubar.add_cascade(label="View", menu=menuView, underline=0)
        menuView.add_command(label="Mixer", command=self.openMixerView, underline=0, accelerator='Ctrl+M')
        menuView.add_command(label="Console Output", command=self.menu_openConsoleWindow, underline=0)
        menuView.add_separator()
        menuView.add_command(label="SoundFont Information Tool", command=self.menu_soundFontInfoTool, underline=0)
        menuView.add_separator()
        menuView.add_checkbutton(label="Show Durations in score", variable=self.objOptionsDuration, underline=5, onvalue=1, offvalue=0)
        menuView.add_checkbutton(label="Show Barlines in score", variable=self.objOptionsBarlines, underline=5, onvalue=1, offvalue=0)
        
        menuHelp = Menu(menubar, tearoff=0)
        menuHelp.add_command(label='About', underline=0, command=(lambda: midirender_util.alert('Bmidi to wav, by Ben Fisher 2009\nA graphical frontend for Timidity and sfubar.\n\nSee the documentation at https://github.com/moltenform/labs_youthful_projects/tree/master/src/benmidi/README.md\n\nSource code at https://github.com/moltenform/labs_youthful_projects/tree/master/src/benmidi/bmidirender','Bmidi to wav')))
        menuHelp.add_command(label='Documentation', underline=0, command=(lambda: openOnlineDocs()))
        menubar.add_cascade(label="Help", menu=menuHelp, underline=0)
        
        root.config(menu=menubar)
        
    def menu_openMidi(self, evt=None):
        filename = midirender_util.ask_openfile(title='Open Midi File', types=['.mid|Mid file'])
        if not filename:
            return

        # first, see if it loads successfully.
        try:
            newmidi = bmidilib.BMidiFile()
            newmidi.open(filename, 'rb')
            newmidi.read()
            newmidi.close()
        except:
            e = sys.exc_info()[1]
            midirender_util.alert('Could not load midi: exception %s'%str(e), title='Could not load midi',icon='error')
            return
        
        self.lblFilename['text'] = filename
        self.loadMidiObj(newmidi)
            
    def loadMidiObj(self, newmidi):
        self.objMidi = newmidi
        if self.player.isPlaying(): return False
        
        if not self.haveDrawnHeaders: self.drawColumnHeaders()
        if not self.isMidiLoaded: 
            # check if Timidity is installed
            if sys.platform != 'win32':
                if not midirender_runtimidity.isTimidityInstalled():
                    midirender_util.alert('It appears that the program Timidity is not installed. This program is required for playing and rendering music.\n\nYou could try running something corresponding to "sudo apt-get install timidity" or "sudo yum install timidity++" in a terminal.')
            self.isMidiLoaded = True
        
        # close any open views
        for key in self.listviews:
            self.listviews[key].destroy()
        for key in self.scoreviews:
            self.scoreviews[key].destroy()
        self.listviews = {}
        self.scoreviews = {}
        self.clearModifications()
        
        # hide all of the old widgets
        for key in self.gridwidgets:
            w = self.gridwidgets[key]
            if w.master.is_smallframe==1:
                w.master.grid_forget()
        for key in self.gridbuttons:
            self.gridbuttons[key].grid_forget()
        
        def addLabel(text, y, x, isButton=False):
            # only create a new widget when necessary. This way, don't need to allocate every time a file is opened.
            if (x, y+1) not in self.gridwidgets:
                smallFrame = Frame(self.frameGrid, borderwidth=1, relief=RIDGE)
                smallFrame.is_smallframe=1
                if isButton: 
                    btn = Button(smallFrame, text=text, relief=GROOVE,anchor='w')
                    btn.config(command=midirender_util.Callable(self.onBtnChangeInstrument, y,btn))
                    btn.pack(anchor='w', fill=BOTH)
                    btn['disabledforeground'] = 'black' # means that when it is disabled, looks just like a label. sweet.
                    thewidget = btn
                    
                else: 
                    lbl = Label(smallFrame, text=text)
                    lbl.pack(anchor='w')
                    thewidget = lbl
                
                self.gridwidgets[(x,y+1)] = thewidget
            
            self.gridwidgets[(x,y+1)]['text'] = text
            self.gridwidgets[(x,y+1)].master.grid(row=y+1, column=x, sticky='nsew')
            return self.gridwidgets[(x,y+1)]
        
        lengthTimer = bmiditools.BMidiSecondsLength(self.objMidi)
        overallLengthSeconds = lengthTimer.getOverallLength(self.objMidi)
        self.sliderTime['to'] = max(1.0, overallLengthSeconds+1.0)
        self.player.load(max(1.0, overallLengthSeconds+1.0)) # "loading" will also set position to 0.0
        
        warnMultipleChannels = False
        for rownum in range(len(self.objMidi.tracks)):
            trackobj = self.objMidi.tracks[rownum]
            
            # Track Number
            addLabel( str(rownum), rownum, 0)
            
            # Track Name
            defaultname = 'Condtrack' if rownum==0 else ('Track %d'%rownum)
            searchFor = {'TRACKNAME':defaultname, 'INSTRUMENTS':1 }
            res = bmiditools.getTrackInformation(trackobj, searchFor)
            addLabel(res['TRACKNAME'], rownum, 1)
            
            # Track Channel(s)
            chanarray = self.findNoteChannels(trackobj)
            if len(chanarray)==0: channame='None'
            elif len(chanarray)>1: channame='(Many)'; warnMultipleChannels=True
            else: channame = str(chanarray[0])
            addLabel(channame, rownum, 2)
            countednoteevts = len(trackobj.notelist) # this assumes notelist is valid, and notelist is only valid if we've just read from a file
            
            # Track Instrument(s)
            instarray = res['INSTRUMENTS']
            if len(instarray)==0:
                instname='None'
            elif len(instarray)>1:
                instname='(Many)'
            else:
                instname = str(instarray[0]) + ' (' + bmidilib.getInstrumentName(instarray[0]) + ')'
            if channame == '10':
                instname = '(Percussion channel)'
            
            btn = addLabel( instname, rownum, 3, isButton=True) # add a button (not a label)
            isEnabled = channame!='10' and instname!='None' and instname!='(Many)' # countednoteevts>0
            if isEnabled:
                btn['state'] = NORMAL
                btn['relief'] = GROOVE
            else:
                btn['state'] = DISABLED
                btn['relief'] = FLAT
            # if there are multiple inst. changes in a track, we don't let you change instruments because there isn't a convenient way to do that.
            
            # Track Time
            if len(trackobj.notelist)==0: strTime = lengthTimer.secondsToString(0)
            else: strTime = lengthTimer.secondsToString(lengthTimer.ticksToSeconds(trackobj.notelist[0].time))
            addLabel( strTime, rownum, 4)
            
            # Track Notes
            addLabel( str(countednoteevts), rownum, 5)
            
            # Buttons
            if (rownum, 0) not in self.gridbuttons:
                btn = Button(self.frameGrid, text='Mixer', command=self.openMixerView)
                self.gridbuttons[(rownum, 0)] = btn
            self.gridbuttons[(rownum, 0)].grid(row=rownum+1, column=6)
            
            if (rownum, 1) not in self.gridbuttons:
                btn = Button(self.frameGrid, text='Score', command=midirender_util.Callable(self.openScoreView, rownum))
                self.gridbuttons[(rownum, 1)] = btn
            self.gridbuttons[(rownum, 1)].grid(row=rownum+1, column=7)
            
            if (rownum, 2) not in self.gridbuttons:
                btn = Button(self.frameGrid, text='List', command=midirender_util.Callable(self.openListView, rownum))
                self.gridbuttons[(rownum, 2)] = btn
            self.gridbuttons[(rownum, 2)].grid(row=rownum+1, column=8)
        
        if warnMultipleChannels:
            resp = midirender_util.ask_yesno('This midi file has notes from different channels in the same track (format 0). Click "yes" (recommended) to import it as a format 1 file, or "no" to leave it. ')
            if resp:
                newmidi = bmiditools.restructureMidi(self.objMidi)
                newmidi.format = 1
                self.loadMidiObj(newmidi)
                return
    
    def onBtnChangeInstrument(self, y, btn):
        # in this version, we'll actually modify the midi object. unclear if this is the best design.
        track = self.objMidi.tracks[y]
        theEvt = None
        for evt in track.events:
            if evt.type=='PROGRAM_CHANGE':
                theEvt = evt
                break
        if theEvt is None: return
        
        dlg = midirender_choose_midi_voice.ChooseMidiInstrumentDialog(self.frameGrid, 'Choose Instrument', min(theEvt.data,127))
        midiNumber = dlg.result
        if midiNumber is None: return
        
        theEvt.data = midiNumber
        btn['text'] = str(midiNumber) + ' (' + bmidilib.getInstrumentName(midiNumber) + ')'
        
    def onBtnSaveWave(self,e=None):
        if not self.isMidiLoaded:
            midirender_util.alert('Please open a MIDI file first.')
            return
        
        filename = midirender_util.ask_savefile(title="Create Wav File", types=['.wav|Wav file'])
        if not filename:
            return
            
        midicopy = self.buildModifiedMidi()
        arParams, directoryForOldTimidity = self.getParamsForTimidity(True)
        if arParams is None:
            return
            
        arParams.append('-o')
        arParams.append(filename)
        
        # play it synchronously, meaning that the whole program stalls while this happens...
        midirender_util.alert('Beginning export to wave...')
        objplayer = midirender_runtimidity.RenderTimidityMidiPlayer()
        objplayer.setConfiguration(self.buildCfg(), directoryForOldTimidity)
        objplayer.setParameters(arParams)
        objplayer.playMidiObject(midicopy, bSynchronous=True)
        if self.consoleOutWindow is not None:
            self.consoleOutWindow.clear()
            self.consoleOutWindow.writeToWindow(objplayer.strLastStdOutput)
        midirender_util.alert('Completed.')
    
    def clearModifications(self):
        # close Mixer window
        if self.mixerWindow:
            self.mixerWindow.destroy()
            self.mixerWindow = None
        if self.audioOptsWindow:
            self.audioOptsWindow.destroy()
            self.audioOptsWindow = None
        if self.soundfontWindow:
            self.soundfontWindow.destroy()
            self.soundfontWindow = None
            # note, however, that self.currentSoundfont remains as it should.
            
        # get rid of Tempo modifications.
        self.tempoScaleFactor = None
        self.transposePitches = None
        
    def buildCfg(self):
        # begin by adding the global soundfont or cfg file.
        filename =self.currentSoundfont[0]
        strCfg = ''
        if filename.endswith('.cfg'):
            path, justname = os.path.split(filename)
            if self.audioOptsWindow and self.audioOptsWindow.getUseOldTimidity():
                if ' ' in justname:
                    midirender_util.alert("Warning: old version of Timidity probably doesn't support spaces in filenames.")
                if justname.lower()!='timidity.cfg' and os.path.exists(path+os.sep+'timidity.cfg'):
                    midirender_util.alert("Warning: there is a filename timidity.cfg in this directory and so the old version of Timidity will use it.")
                strCfg += '\nsource %s' % (justname)
            else:
                strCfg += '\ndir "%s"\nsource "%s"' % (path, filename)
        else:

            strCfg += '\nsoundfont "%s"' % (filename)
            if self.audioOptsWindow is not None and self.audioOptsWindow.getPatchesTakePrecedence():
                strCfg +=' order=1'
        
        # now add customization to override specific voices, if set
        if self.soundfontWindow is not None:
            strCfg += '\n' + self.soundfontWindow.getCfgResults(self.audioOptsWindow and self.audioOptsWindow.getUseOldTimidity())
            
        if self.audioOptsWindow is not None:
            addedLines = self.audioOptsWindow.getAdditionalCfg()
            if addedLines:
                strCfg += '\n'+'\n'.join(addedLines)
            
        return strCfg
        
    def buildModifiedMidi(self):
        # at first I thought that I could avoid the deepcopy if we haven't changed any settings.
        # that is not right, though, because we cut the midi for playback that isn't at the beginning.
        
        midiCopy = bmiditools.getMidiCopy(self.objMidi, deep=True)
        if self.mixerWindow: 
            self.mixerWindow.createMixedMidi( midiCopy )
        if self.tempoScaleFactor is not None:
            midirender_tempo.doChangeTempo(midiCopy, self.tempoScaleFactor)
        return midiCopy
        
    def menuModifyRawMidi(self, evt=None):
        if sys.platform != 'win32':
            midirender_util.alert('Only supported on windows')
            return
        
        import tempfile, os, subprocess
        m2t = midirender_util.bmidirenderdirectory+'\\timidity\\m2t.exe'
        t2m = midirender_util.bmidirenderdirectory+'\\timidity\\t2m.exe'
        notepadexe = 'C:\\Windows\\System32\\notepad.exe'
        if not os.path.exists(m2t) or not os.path.exists(t2m) or not os.path.exists(notepadexe):
            midirender_util.alert('Could not find %s or %s or %s.'%(m2t, t2m, notepadexe))
            return
        
        filenameMidInput = midirender_util.ask_openfile(title='Choose Midi File to modify', types=['.mid|Mid file'])
        if not filenameMidInput:
            return
        
        filenameText = tempfile.gettempdir() + os.sep + 'tmpout.txt'
        try:
            if os.path.exists(filenameText):
                os.unlink(filenameText)
        except:
            pass
        if os.path.exists(filenameText):
            midirender_util.alert('Could not clear temporary file')
            return
        
        args = [m2t, filenameMidInput, filenameText]
        retcode = subprocess.call(args, creationflags=0x08000000)
        if retcode:
            midirender_util.alert('Midi to text returned failure')
            return
        if not os.path.exists(filenameText):
            midirender_util.alert('Midi to text did not write text file')
            return
        
        midirender_util.alert("You'll see the text file in notepad. Save and close when done editing.")
        modtimebefore = os.path.getmtime(filenameText)
        args = [notepadexe, filenameText]
        retcode = subprocess.call(args)
        if modtimebefore == os.path.getmtime(filenameText):
            # looks like the user cancelled before making any edits
            return
            
        filenameMidOutput = midirender_util.ask_savefile(title="Choose destination for Midi File", types=['.mid|Mid file'])
        if not filenameMidOutput: return
        
        args = [t2m, filenameText, filenameMidOutput]
        retcode = subprocess.call(args, creationflags=0x08000000)
        if retcode:
            midirender_util.alert('text to midi returned failure')
            return
        if not os.path.exists(filenameMidOutput):
            midirender_util.alert('text to midi did not write midi file')
            return
        midirender_util.alert('Complete.')
            
    def saveModifiedMidi(self, evt=None):
        if not self.isMidiLoaded:
            midirender_util.alert('Please open a MIDI file first.')
            return
            
        filename = midirender_util.ask_savefile(title="Save Midi File", types=['.mid|Mid file'])
        if not filename:
            return
        
        mfile = self.buildModifiedMidi()
        if mfile:
            mfile.open(filename,'wb')
            mfile.write()
            mfile.close()
    
    ######### Windows for settings ###############
    
    def menu_changeTempo(self, e=None):
        if not self.isMidiLoaded:
            midirender_util.alert('Please open a MIDI file first.')
            return
        
        res = midirender_tempo.queryChangeTempo(self.objMidi, self.tempoScaleFactor)
        if res is None:
            return # canceled.
        
        if abs(res-1.0) < 0.001:  # we don't need to change the tempo if it is staying the same.
            self.tempoScaleFactor = None
        else:
            self.tempoScaleFactor = res
    
    def menu_changeTranspose(self, e=None):
        if not self.isMidiLoaded:
            midirender_util.alert('Please open a MIDI file first.')
            return
            
        strPrompt = 'Adjust key (i.e., transpose the song) by n half tones. Valid range is from -24 to 24.'
        default = self.transposePitches if self.transposePitches else 0.0
        res = midirender_util.ask_float(strPrompt, default=default, min=-24.1, max=24.1, title='Transpose')
        if res is not None and res is not False:
            self.transposePitches = int(res)
            
    def openSoundfontWindow(self, e=None):
        if not self.isMidiLoaded:
            midirender_util.alert('Please open a MIDI file first.')
            return
        
        # this is different than the list and score view - there can only be one of them open at once
        if self.soundfontWindow:
            return # only allow one instance open at a time
            
        top = Toplevel()
        def callbackOnClose(): 
            self.soundfontWindow = None
            
        self.soundfontWindow = midirender_soundfont.BSoundfontWindow(top, self.currentSoundfont, self.objMidi, callbackOnClose)
        top.focus_set()
    
    def openMixerView(self, e=None):
        if not self.isMidiLoaded:
            midirender_util.alert('Please open a MIDI file first.')
            return
            
        if self.mixerWindow: 
            return # only allow one instance open at a time
            
        top = Toplevel()
        def callbackOnClose():
            self.mixerWindow = None
            
        self.mixerWindow = midirender_mixer.BMixerWindow(top, self.objMidi, {}, callbackOnClose)
        top.focus_set()
    
    def openAudioOptsWindow(self, evt=None):
        if not self.isMidiLoaded:
            midirender_util.alert('Please open a MIDI file first.')
            return
        
        if self.audioOptsWindow:
            return # only allow one instance open at a time
            
        top = Toplevel()
        def callbackOnClose():
            self.audioOptsWindow = None
            
        self.audioOptsWindow = midirender_audiooptions.WindowAudioOptions(top, callbackOnClose)
        top.focus_set()
        
    def menu_openConsoleWindow(self):
        if not self.isMidiLoaded:
            midirender_util.alert('Please open a MIDI file first.')
            return
        
        if self.consoleOutWindow:
            return # only allow one instance open at a time
        
        top = Toplevel()
        def callbackOnClose():
            self.consoleOutWindow = None
        
        self.consoleOutWindow = midirender_consoleout.BConsoleOutWindow(top, self.consoleOutCallback, callbackOnClose=callbackOnClose)
        top.focus_set()
    
    def consoleOutCallback(self):
        # the window has requested that we show the stdout.
        if not self.isMidiLoaded: 
            return
        
        if self.consoleOutWindow is not None: 
            self.consoleOutWindow.clear()
            self.consoleOutWindow.writeToWindow(self.player.getLastStdout())
    
    def menuCopyAudioOptsString(self, evt=None):
        params, dir = self.getParamsForTimidity(bRenderWav=False)
        if params is None:
            return # evidently an error occurred over there
            
        ret = 'timidity song.mid '+' '.join(('"' + p + '"' for p in params))
        ret += os.linesep+os.linesep + 'timidity.cfg:'+os.linesep
        ret += self.buildCfg()
        
        self.top.clipboard_clear()
        self.top.clipboard_append(ret)
        
    def openScoreView(self, n):
        if not self.isMidiLoaded:
            midirender_util.alert('Please open a MIDI file first.')
            return
            
        if len(self.objMidi.tracks[n].notelist)==0:
            midirender_util.alert('No notes to show in this track.')
            return
        
        opts = {}
        opts['show_durations'] = self.objOptionsDuration.get()
        opts['show_barlines'] = self.objOptionsBarlines.get()
        opts['show_stems'] = 1
        opts['prefer_flats'] = 0
        opts['clefspath'] = clefspath
        
        top = Toplevel()
        window = scoreview.ScoreViewWindow(top, n, self.objMidi.tracks[n],self.objMidi.ticksPerQuarterNote, opts)
        self.scoreviews[n] = top
        top.focus_set()
            
    def openListView(self, n):
        opts = {}
        top = Toplevel()
        window = listview.ListViewWindow(top, n, self.objMidi.tracks[n], opts)
        self.listviews[n] = top
        top.focus_set()
    
    def menu_soundFontInfoTool(self):
        filename = midirender_util.ask_openfile(initialfolder=midirender_soundfont.gm_dir, title='Choose SoundFont', types=['.sf2 .sbk .pat|SoundFont or Patch'])
        if not filename:
            return
        
        dlg = midirender_soundfont_info.BSoundFontInformation(self.top, filename, bSelectMode=False)
        # note that it is modal. that is fine for now, though.

    ######### Event handlers ################

    def playCallbackToggleButton(self, strBtn): 
        self.btnPlay.config(relief=RAISED)
        self.btnPause.config(relief=RAISED)
        self.btnStop.config(relief=RAISED)
        if strBtn=='play':
            self.btnPlay.config(relief=SUNKEN)
        elif strBtn=='pause':
            self.btnPause.config(relief=SUNKEN)
        elif strBtn=='stop':
            self.btnStop.config(relief=SUNKEN)
        else:
            raise Exception('Unknown button')
            
    def getParamsForTimidity(self, bRenderWav):
        directoryForOldTimidity = None
        
        if self.audioOptsWindow is not None:
            params = self.audioOptsWindow.getOptionsList(includeRenderOptions=bRenderWav) 
            if params is None:
                midirender_util.alert('Could not get parameters.')
                return None, None
            
            if self.audioOptsWindow.getUseOldTimidity():
                if not self.currentSoundfont[0].lower().endswith('.cfg'):
                    midirender_util.alert('Provide a cfg with no soundfonts for old timidity.')
                    return None, None
                    
                directoryForOldTimidity = os.path.split(self.currentSoundfont[0])[0]
        else:
            if bRenderWav:
                # in Linux render 24bit by default (workaround for timidity++ bug 710927)
                # this bug is now fixed in the distributions I've tested
                if not sys.platform.startswith('win'):
                    params = ['-Ow2']
                else:
                    params = ['-Ow']
            else:
                params = []
        
        if self.transposePitches is not None and (not self.audioOptsWindow or not self.audioOptsWindow.getUseOldTimidity()):
            params.append('--adjust-key=%d' % int(self.transposePitches))
        
        return params, directoryForOldTimidity
    
    def onBtnPlay(self, e=None):
        if not self.isMidiLoaded:
            midirender_util.alert('Please open a MIDI file first.')
            return
        
        params, directoryForOldTimidity = self.getParamsForTimidity(False)
        bypassTimidity = False
        if self.audioOptsWindow is not None:
            bypassTimidity = self.audioOptsWindow.getBypassTimidity()
        
        if params is not None:
            self.player.actionPlay(self.buildModifiedMidi(), params, self.buildCfg(), directoryForOldTimidity, not bypassTimidity)
        
    def playCallbackGetSlider(self):
        return self.sliderTime.get()

    def playCallbackSetSlider(self,v):
        self.sliderTime.set(v)

    def onBtnStop(self, e=None):
        if self.isMidiLoaded:
            self.player.actionStop()

    def onBtnPause(self, e=None):
        if self.isMidiLoaded:
            self.player.actionPause()

    def onClose(self):
        try:
            self.onBtnStop()
            # stop playback slider thread
            self.player.playingState = 'stopped'
        finally:
            self.top.destroy()
            
    def findNoteChannels(self, trackObject):
        channelsSeen = {}
        for note in trackObject.notelist:
            channelsSeen[note.channel] = 1

        return list(channelsSeen.keys())

def pack(o, **kwargs): 
    o.pack(**kwargs)
    return o

def openOnlineDocs():
    import webbrowser
    url = 'https://github.com/moltenform/labs_youthful_projects/tree/master/src/benmidi/README.md'
    webbrowser.open(url, new=1)

icon0 = '''R0lGODlhFAAUAPcAAAAAAIAAAACAAICAAAAAgIAAgACAgICAgMDAwP8AAAD/AP//AAAA//8A/wD//////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMwAAZgAAmQAAzAAA/wAzAAAzMwAzZgAzmQAzzAAz/wBmAABmMwBmZgBmmQBmzABm/wCZAACZMwCZZgCZmQCZzACZ/wDMAADMMwDMZgDMmQDMzADM/wD/AAD/MwD/ZgD/mQD/zAD//zMAADMAMzMAZjMAmTMAzDMA/zMzADMzMzMzZjMzmTMzzDMz/zNmADNmMzNmZjNmmTNmzDNm/zOZADOZMzOZZjOZmTOZzDOZ/zPMADPMMzPMZjPMmTPMzDPM/zP/ADP/MzP/ZjP/mTP/zDP//2YAAGYAM2YAZmYAmWYAzGYA/2YzAGYzM2YzZmYzmWYzzGYz/2ZmAGZmM2ZmZmZmmWZmzGZm/2aZAGaZM2aZZmaZmWaZzGaZ/2bMAGbMM2bMZmbMmWbMzGbM/2b/AGb/M2b/Zmb/mWb/zGb//5kAAJkAM5kAZpkAmZkAzJkA/5kzAJkzM5kzZpkzmZkzzJkz/5lmAJlmM5lmZplmmZlmzJlm/5mZAJmZM5mZZpmZmZmZzJmZ/5nMAJnMM5nMZpnMmZnMzJnM/5n/AJn/M5n/Zpn/mZn/zJn//8wAAMwAM8wAZswAmcwAzMwA/8wzAMwzM8wzZswzmcwzzMwz/8xmAMxmM8xmZsxmmcxmzMxm/8yZAMyZM8yZZsyZmcyZzMyZ/8zMAMzMM8zMZszMmczMzMzM/8z/AMz/M8z/Zsz/mcz/zMz///8AAP8AM/8AZv8Amf8AzP8A//8zAP8zM/8zZv8zmf8zzP8z//9mAP9mM/9mZv9mmf9mzP9m//+ZAP+ZM/+ZZv+Zmf+ZzP+Z///MAP/MM//MZv/Mmf/MzP/M////AP//M///Zv//mf///////yH5BAEAAP4ALAAAAAAUABQAQAg7AP0JHEiwoEGCABIqXHiwoUODCR82XBhRosWHFAFc9JeRocOKG0OKHLnRY8mOFjuahIhS4kqSMGM+DAgAOw=='''
icon1 = '''R0lGODlhFAAUAPcAAAAAAIAAAACAAICAAAAAgIAAgACAgICAgMDAwP8AAAD/AP//AAAA//8A/wD//////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMwAAZgAAmQAAzAAA/wAzAAAzMwAzZgAzmQAzzAAz/wBmAABmMwBmZgBmmQBmzABm/wCZAACZMwCZZgCZmQCZzACZ/wDMAADMMwDMZgDMmQDMzADM/wD/AAD/MwD/ZgD/mQD/zAD//zMAADMAMzMAZjMAmTMAzDMA/zMzADMzMzMzZjMzmTMzzDMz/zNmADNmMzNmZjNmmTNmzDNm/zOZADOZMzOZZjOZmTOZzDOZ/zPMADPMMzPMZjPMmTPMzDPM/zP/ADP/MzP/ZjP/mTP/zDP//2YAAGYAM2YAZmYAmWYAzGYA/2YzAGYzM2YzZmYzmWYzzGYz/2ZmAGZmM2ZmZmZmmWZmzGZm/2aZAGaZM2aZZmaZmWaZzGaZ/2bMAGbMM2bMZmbMmWbMzGbM/2b/AGb/M2b/Zmb/mWb/zGb//5kAAJkAM5kAZpkAmZkAzJkA/5kzAJkzM5kzZpkzmZkzzJkz/5lmAJlmM5lmZplmmZlmzJlm/5mZAJmZM5mZZpmZmZmZzJmZ/5nMAJnMM5nMZpnMmZnMzJnM/5n/AJn/M5n/Zpn/mZn/zJn//8wAAMwAM8wAZswAmcwAzMwA/8wzAMwzM8wzZswzmcwzzMwz/8xmAMxmM8xmZsxmmcxmzMxm/8yZAMyZM8yZZsyZmcyZzMyZ/8zMAMzMM8zMZszMmczMzMzM/8z/AMz/M8z/Zsz/mcz/zMz///8AAP8AM/8AZv8Amf8AzP8A//8zAP8zM/8zZv8zmf8zzP8z//9mAP9mM/9mZv9mmf9mzP9m//+ZAP+ZM/+ZZv+Zmf+ZzP+Z///MAP/MM//MZv/Mmf/MzP/M////AP//M///Zv//mf///////yH5BAEAAP4ALAAAAAAUABQAQAg9AP0JHEiwoMGCABIKTAjgoMOHECMeZLhQocSLBin60whRI8eHHi1iHEmypMmOFj86DNkwIkuJL0/KnFkwIAA7'''
icon2 = '''R0lGODlhFAAUAPcAAAAAAIAAAACAAICAAAAAgIAAgACAgICAgMDAwP8AAAD/AP//AAAA//8A/wD//////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMwAAZgAAmQAAzAAA/wAzAAAzMwAzZgAzmQAzzAAz/wBmAABmMwBmZgBmmQBmzABm/wCZAACZMwCZZgCZmQCZzACZ/wDMAADMMwDMZgDMmQDMzADM/wD/AAD/MwD/ZgD/mQD/zAD//zMAADMAMzMAZjMAmTMAzDMA/zMzADMzMzMzZjMzmTMzzDMz/zNmADNmMzNmZjNmmTNmzDNm/zOZADOZMzOZZjOZmTOZzDOZ/zPMADPMMzPMZjPMmTPMzDPM/zP/ADP/MzP/ZjP/mTP/zDP//2YAAGYAM2YAZmYAmWYAzGYA/2YzAGYzM2YzZmYzmWYzzGYz/2ZmAGZmM2ZmZmZmmWZmzGZm/2aZAGaZM2aZZmaZmWaZzGaZ/2bMAGbMM2bMZmbMmWbMzGbM/2b/AGb/M2b/Zmb/mWb/zGb//5kAAJkAM5kAZpkAmZkAzJkA/5kzAJkzM5kzZpkzmZkzzJkz/5lmAJlmM5lmZplmmZlmzJlm/5mZAJmZM5mZZpmZmZmZzJmZ/5nMAJnMM5nMZpnMmZnMzJnM/5n/AJn/M5n/Zpn/mZn/zJn//8wAAMwAM8wAZswAmcwAzMwA/8wzAMwzM8wzZswzmcwzzMwz/8xmAMxmM8xmZsxmmcxmzMxm/8yZAMyZM8yZZsyZmcyZzMyZ/8zMAMzMM8zMZszMmczMzMzM/8z/AMz/M8z/Zsz/mcz/zMz///8AAP8AM/8AZv8Amf8AzP8A//8zAP8zM/8zZv8zmf8zzP8z//9mAP9mM/9mZv9mmf9mzP9m//+ZAP+ZM/+ZZv+Zmf+ZzP+Z///MAP/MM//MZv/Mmf/MzP/M////AP//M///Zv//mf///////yH5BAEAAP4ALAAAAAAUABQAQAg4AP0JHEiwoMGCABIqTHiwocOHEA8uXBixosSJDCFizPhwIwCLIEOKHBnRY8mNJzGmnEiypUuDAQEAOw=='''

root = Tk()
app = App(root)
root.mainloop()

# todo: preview soundfont information could take a cfg and show all patches.
# todo: in choose soundfont window, change caption 'customize' to 'set each instrument individually'
# todo: instead of modify raw midi, have two menu items "MIDI to text" and "text to MIDI"

'''
midis are modified by:
-----------
1) if format 0 (everything in one track), convert to format 1 (tracks by channel). This occurs before anything else.

2) changes made directly.
    Instrument changes, made by clicking the instrument in the grid, modify the midi file directly.

3) changes based on other windows:
    
    Tempo changes - stored in the variable self.tempoScaleFactor
    midirender_mixer,mixer changes only while the mixer window is open
            (relies on tracknumber, so none of the other modifications before this should reorder tracks)

4) final modification
    cutting the midi to start playback halfway through the song

timidity playback is modified by
---------------
1) changes made directly:
    the variable self.currentSoundfont, which is modified by the soundfont window

2) changes made only when window is open
    "customize" options 
    midirender_audiooptions - when dialog is open
'''


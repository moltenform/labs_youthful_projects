import sys

if sys.platform=='win32':
    import winsound
    # See, http://docs.python.org/lib/module-winsound.html
    
    def audio_play(strFilename, bAsync=False):
        if bAsync:
            winsound.PlaySound(strFilename, winsound.SND_ASYNC)
        else:
            winsound.PlaySound(strFilename)
    
    def audio_play_memory(acWaveData):
        """The sound parameter to PlaySound() is a memory image of a WAV file, as a string."""
        winsound.PlaySound(acWaveData, winsound.SND_MEMORY)
    
    def audio_record(strFilename, nDuration, nSampleWidth=16, nSampleRate=22050):
        r = Recorder(nSampleWidth*8, int(nSampleRate), 1)   # Record
        r.start(strFilename)         # Record audio into file
        r.wait(nDuration)          # duration (seconds)
        # File is closed in wait().
        return 1
        
    #   Windows Audio Recorder
    #   Version : 2.0.0
    #   Author  : John Popplewell
    #   Email   : john@johnnypops.demon.co.uk
    #   Website : http://www.johnnypops.demon.co.uk/
    #   Changes :
    #       V2.0.0 21 Feb 2005 - Changed to 'ctypes'.
    #       V1.0.2 25 Mar 2002 - Renamed from "record-win32.py" to "recordwin32.py" (Oops!).
    #       V1.0.1 11 Mar 2002 - Added Licence + some cosmetic changes.
    #       V1.0.0 08 Mar 2002 - First release.
    #   Requires:
    #       'ctypes'. see http://sourceforge.net/projects/ctypes/ for details.
    #
    #   Note:
    #       The code for the callback-window is heavily inspired by similar code 
    #       in Sam Rushings 'DynWin'.
    #       See http://www.nightmare.com/~rushing/dynwin/ for details.
    #
    #   Licence:
    #       The authors hereby grant permission to use, copy, modify, distribute,
    #       and license this software and its documentation for any purpose, 
    #       provided that existing copyright notices are retained in all copies 
    #       and that this notice is included verbatim in any distributions. No 
    #       written agreement, license, or royalty fee is required for any of the
    #       authorized uses.
    #       
    #       IN NO EVENT SHALL THE AUTHORS OR DISTRIBUTORS BE LIABLE TO ANY PARTY
    #       FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES
    #       ARISING OUT OF THE USE OF THIS SOFTWARE, ITS DOCUMENTATION, OR ANY
    #       DERIVATIVES THEREOF, EVEN IF THE AUTHOR(S) HAVE BEEN ADVISED OF THE
    #       POSSIBILITY OF SUCH DAMAGE.
    #
    #       THE AUTHOR(S) AND DISTRIBUTORS SPECIFICALLY DISCLAIM ANY WARRANTIES,
    #       INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
    #       MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, AND
    #       NON-INFRINGEMENT.  THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, AND
    #       THE AUTHOR(S) AND DISTRIBUTORS HAVE NO OBLIGATION TO PROVIDE
    #       MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
    #
    #############################################################################

    import os, time, wave, getopt, types #, sys

    try:
        from ctypes import *
    except:
        print
        print "Requires ctypes."
        print "See http://sourceforge.net/projects/ctypes/ for details."
        sys.exit(0)

    user32  = windll.user32
    kernel32= windll.kernel32
    gdi32   = windll.gdi32
    winmm   = windll.winmm

    ########################################################################
    WS_OVERLAPPEDWINDOW = 0xCF0000
    CW_USEDEFAULT       = 0x80000000L

    IDI_APPLICATION     = 32512
    IDC_ARROW           = 32512
    COLOR_BACKGROUND    = 1

    class MSG(Structure):
        _fields_ = [
            ('hwnd', c_ulong),
            ('message', c_ulong),
            ('wParam', c_ulong),
            ('lParam', c_ulong),
            ('time', c_ulong),
            ('x', c_ulong),
            ('y', c_ulong),
        ]

    WNDPROC = WINFUNCTYPE(c_int, c_ulong, c_ulong, c_ulong, c_ulong)

    class WNDCLASSEX(Structure):
        _fields_ = [
            ('size', c_uint),
            ('style', c_uint),
            ('wndproc', WNDPROC),
            ('cls_extra', c_int),
            ('wnd_extra', c_int),
            ('instance', c_ulong),
            ('icon', c_ulong),
            ('cursor', c_ulong),
            ('background', c_ulong),
            ('menu_name', c_char_p),
            ('class_name', c_char_p),
            ('icon_sm', c_ulong),
        ]

    _window_classes = {}
    _window_map = {}

    module_handle = kernel32.GetModuleHandleA(0)

    class WindowClass:
        atom = 0
        def __init__(self, class_name, wndproc):
            wc = WNDCLASSEX()
            self.class_name = class_name
            self.wndproc    = WNDPROC(wndproc)
            wc.class_name   = c_char_p(self.class_name)
            wc.wndproc      = self.wndproc
            wc.instance     = module_handle
            wc.icon         = user32.LoadIconA(0, IDI_APPLICATION)
            wc.icon_sm      = user32.LoadIconA(0, IDI_APPLICATION)
            wc.cursor       = user32.LoadCursorA(0, IDC_ARROW)
            wc.background   = gdi32.GetStockObject(COLOR_BACKGROUND)
            wc.size         = sizeof(wc)
            self.wc = wc

        def register(self):
            global _window_classes
            if not _window_classes.has_key(self.class_name):
                self.atom = user32.RegisterClassExA(byref(self.wc))
                if self.atom:
                    _window_classes[self.class_name] = self
                return self
            raise ValueError("WindowClass() '%s' already registered"%self.class_name)

        def unregister(self):
            global _window_classes
            if self.atom:
                if user32.UnregisterClassA(self.wc.class_name, module_handle):
                    self.atom = 0
                    if _window_classes and _window_classes.has_key(self.class_name):
                        del _window_classes[self.class_name]
                return
            raise ValueError("Attempt to unregister a WindowClass() that has not been registered.")

        def __int__(self):
            if self.atom:
                return self.atom
            raise ValueError("WindowClass() has not been registered.")

        def __del__(self):
            if self.atom:
                self.unregister()

    def class_wndproc(hwnd, message, wparam, lparam):
        try:
            if _window_map.has_key(hwnd):
                retval = _window_map[hwnd].wndproc(hwnd, message, wparam, lparam)
                if retval is None:
                    return 0
                return retval
            return user32.DefWindowProcA(hwnd, message, wparam, lparam)
        except:
            return user32.DefWindowProcA(hwnd, message, wparam, lparam)

    _default_python_window_class = None

    def get_default_python_window_class():
        global _default_python_window_class
        if _default_python_window_class is None:
            c = WindowClass('default python window class', class_wndproc)
            c.register()
            _default_python_window_class = c
            return c
        return _default_python_window_class

    _messages = {
          2:['WM_DESTROY'],
        955:['MM_WOM_OPEN'],
        956:['MM_WOM_CLOSE'],
        957:['MM_WOM_DONE'],
        958:['MM_WIM_OPEN'],
        959:['MM_WIM_CLOSE'],
        960:['MM_WIM_DATA'],
    }
    class Window:
        hwnd        = 0
        style       = WS_OVERLAPPEDWINDOW
        parent      = 0
        menu        = 0
        instance    = 0
        param       = 0
        ext_style   = 0
        x = y = w = h = CW_USEDEFAULT

        def __init__(self, wname, wclass=None):
            self.window_name  = wname
            self.window_class = wclass
            self.instance     = module_handle

        def post_quit_message(self):
            return user32.PostQuitMessage(0)

        def destroy_window(self):
            return user32.DestroyWindow(self.hwnd)

        def create(self):
            if self.hwnd:
                raise ValueError("Window() already has a window handle.")

            if self.window_class is None:
                self.window_class = get_default_python_window_class()

            if isinstance(self.window_class, WindowClass):
                _class = c_char_p(self.window_class.class_name)
            else:
                _class = c_char_p(self.window_class)

            if isinstance(self.window_name, types.StringType):
                _name = c_char_p(self.window_name)
            else:
                _name = c_char_p(None)

            self.hwnd = user32.CreateWindowExA(
                self.ext_style, _class, _name,
                self.style, self.x, self.y, self.w, self.h,
                self.parent, self.menu, self.instance, self.param
            )
            global _window_map
            if self.hwnd:
                _window_map[self.hwnd] = self
                return self
            raise SystemError("Window() create failed.")

        def wndproc(self, hwnd, message, wparam, lparam):
            handled = 0
            if _messages.has_key(message):
                names = _messages[message]
                for name in names:
                    if hasattr(self, name):
                        handled = getattr(self, name)(wparam, lparam)
            if not handled:
                return user32.DefWindowProcA(hwnd, message, wparam, lparam)
            return handled

        def WM_DESTROY(self, wparam, lparam):
            global _window_map
            del _window_map[self.hwnd]
            self.hwnd = 0
            if not len(_window_map):
                self.post_quit_message()

    ########################################################################
    class ErrRecord(Exception):
        pass

    WAVE_MAPPER         = -1

    CALLBACK_NULL       = 0x00000000        # no callback
    CALLBACK_WINDOW     = 0x00010000        # dwCallback is a HWND
    CALLBACK_TASK       = 0x00020000        # dwCallback is a HTASK
    CALLBACK_FUNCTION   = 0x00030000        # dwCallback is a FARPROC
    CALLBACK_THREAD     = CALLBACK_TASK     # thread ID replaces 16 bit task
    CALLBACK_EVENT      = 0x00050000        # dwCallback is an EVENT Handle

    WAVE_FORMAT_QUERY   = 0x0001
    WAVE_ALLOWSYNC      = 0x0002
    WAVE_MAPPED         = 0x0004
    WAVE_FORMAT_DIRECT  = 0x0008

    WAVE_INVALIDFORMAT  = 0x00000000        # invalid format
    WAVE_FORMAT_1M08    = 0x00000001        # 11.025 kHz, Mono,   8-bit
    WAVE_FORMAT_1S08    = 0x00000002        # 11.025 kHz, Stereo, 8-bit
    WAVE_FORMAT_1M16    = 0x00000004        # 11.025 kHz, Mono,   16-bit
    WAVE_FORMAT_1S16    = 0x00000008        # 11.025 kHz, Stereo, 16-bit
    WAVE_FORMAT_2M08    = 0x00000010        # 22.05  kHz, Mono,   8-bit
    WAVE_FORMAT_2S08    = 0x00000020        # 22.05  kHz, Stereo, 8-bit
    WAVE_FORMAT_2M16    = 0x00000040        # 22.05  kHz, Mono,   16-bit
    WAVE_FORMAT_2S16    = 0x00000080        # 22.05  kHz, Stereo, 16-bit
    WAVE_FORMAT_4M08    = 0x00000100        # 44.1   kHz, Mono,   8-bit
    WAVE_FORMAT_4S08    = 0x00000200        # 44.1   kHz, Stereo, 8-bit
    WAVE_FORMAT_4M16    = 0x00000400        # 44.1   kHz, Mono,   16-bit
    WAVE_FORMAT_4S16    = 0x00000800        # 44.1   kHz, Stereo, 16-bit

    WAVE_FORMAT_PCM     = 1

    MAXERRORLENGTH      = 256

    PM_REMOVE           = 0x0001
    WM_QUIT             = 0x0012

    class WAVEINCAPS(Structure):
        _fields_ = [
            ('Mid', c_ushort),
            ('Pid', c_ushort),
            ('DriverVersion', c_long),
            ('Pname', c_char * 32),
            ('Formats', c_ulong),
            ('Channels', c_ushort),
            ('Reserved', c_ushort),
        ]

    class WAVEFORMATEX(Structure):
        _fields_ = [
            ('FormatTag', c_ushort), 
            ('Channels', c_ushort),
            ('SamplesPerSec', c_ulong),
            ('AvgBytesPerSec', c_ulong),
            ('BlockAlign', c_ushort),
            ('BitsPerSample', c_ushort),
            ('cbSize', c_ushort),
        ]

        def set_format(self, samplesize, samplerate, channels):
            self.FormatTag = WAVE_FORMAT_PCM
            self.Channels = channels
            self.BitsPerSample = samplesize
            self.BlockAlign = channels*samplesize/8
            self.SamplesPerSec = samplerate
            self.AvgBytesPerSec = samplerate*self.BlockAlign
            self.cbSize = 0

    class AudioBuffer(Structure): 
        """ Wraps up a WAVEHDR and audio data."""
        _fields_ = [
            ('lpData', c_void_p),
            ('BufferLength', c_ulong),
            ('BytesRecorded', c_ulong),
            ('User', c_ulong),
            ('Flags', c_ulong),
            ('Loops', c_ulong),
            ('lpNext', c_ulong),
            ('reserved', c_ulong),
        ]

        def __init__(self, hWave, block_len):
            Structure.__init__(self)
            self.handle = hWave
            self.membuf = create_string_buffer(block_len)
            self.lpData = cast(self.membuf, c_void_p)
            self.BufferLength = block_len
            self.Flags = 0
            self.BytesRecorded = 0
            self.User = 0
            self.Loops = 0
            self.lpNext = 0
            self.reserved = 0

        def inPrepare(self):
            return winmm.waveInPrepareHeader(self.handle, byref(self), sizeof(self))

        def inAdd(self):
            return winmm.waveInAddBuffer(self.handle, byref(self), sizeof(self))

        def inUnprepare(self):
            return winmm.waveInUnprepareHeader(self.handle, byref(self), sizeof(self))

        def __len__(self):
            return self.BytesRecorded

        def read(self):
            return self.membuf[:self.BytesRecorded]

        #def outPrepare(self):
        #    return winmm.waveOutPrepareHeader(self.handle, byref(self), sizeof(self))
        #def outWrite(self):
        #    return winmm.waveOutWrite(self.handle, byref(self), sizeof(self))
        #def outUnprepare(self):
        #    return winmm.waveOutUnprepareHeader(self.handle, byref(self), sizeof(self))
        #def write(self, s):
        #    self.membuf.write(s)

    def get_error_text(errcode):
        """ Utility function for error messages """
        msg = create_string_buffer(MAXERRORLENGTH)
        res = winmm.waveInGetErrorTextA( errcode, byref(msg), MAXERRORLENGTH )
        if res != 0:
            raise ErrRecord("ERROR: unable to get error text.")
        return msg.value

    def get_waveInCAPS(dev=WAVE_MAPPER):
        """ Utility function for waveInCAPS """
        wicaps = WAVEINCAPS()
        res = winmm.waveInGetDevCapsA( dev, byref(wicaps), sizeof(wicaps) )
        if res != 0:
            raise ErrRecord("ERROR: %s whilst opening device."%(get_error_text(res)))
        return wicaps

    class cbWindow(Window):
        """ Callback Window for buffer events """
        def __init__(self, window_name, cbTarget):
            Window.__init__(self, window_name)
            self.cb = cbTarget

        def WM_DESTROY(self, wparam, lparam):
            self.post_quit_message()

        def MM_WIM_OPEN(self, wparam, lparam):
            self.cb._cb_open(wparam)

        def MM_WIM_DATA(self, wparam, lparam):
            self.cb._cb_data(wparam, lparam)

        def MM_WIM_CLOSE(self, wparam, lparam):
            self.cb._cb_close(wparam)

    class Recorder:
        """ Uses 4 small buffers. Data loss can occur when the system is heavily loaded.
        
        A 'queue' in _cb_data() and a seperate thread to write the buffer 
        out and return it to waveIn might work, but only if the thread received 
        sufficient CPU.
        
        Lots of buffers would make more sense then, currently it depends on how 
        waveIn works. """

        NUM_BUFFERS = 4
        BUFFER_SIZE = 32   # in Kb

        INITIALIZING, STOPPED, STARTING, RECORDING, STOPPING = range(5)

      #  def __init__(self, samplesize=16, samplerate=44100, channels=2):
        def __init__(self, samplesize=16, samplerate=22050, channels=1):
            self.status = Recorder.INITIALIZING
            self.maxDevices = winmm.waveInGetNumDevs()
            if self.maxDevices == 0:
                raise ErrRecord("ERROR: no audio recording devices available.")
            self.status = Recorder.STOPPED
            self.wfx = WAVEFORMATEX()
            self.set_format(samplesize, samplerate, channels)

        def set_format(self, samplesize, samplerate, channels):
            """ Sets the recording format """
            if self.status != Recorder.STOPPED:
                return
            self.wfx.set_format(samplesize, samplerate, channels)

        def start(self, filename):
            """ Starts recording - triggers STARTING, RECORDING sequence """
            if self.status != Recorder.STOPPED:
                return
            self.status = Recorder.STARTING
            self.inside = 0

            self.w = cbWindow('Recording-Callback-Window', self ).create()

            self.hWaveIn = c_long()
            res = winmm.waveInOpen(byref(self.hWaveIn), WAVE_MAPPER, byref(self.wfx), self.w.hwnd, 0, CALLBACK_WINDOW)
            if res != 0:
                raise ErrRecord("ERROR: %s whilst opening device."%get_error_text(res))
            self.whdr = {}

            for i in range(Recorder.NUM_BUFFERS):
                buff = AudioBuffer(self.hWaveIn, Recorder.BUFFER_SIZE*1024)
                buff.inPrepare()
                buff.inAdd()
                self.whdr[addressof(buff)] = buff

            self.ofp = wave.open(filename, 'wb')
            self.ofp.setparams((self.wfx.Channels, self.wfx.BitsPerSample/8, self.wfx.SamplesPerSec, 0, 'NONE', ''))

            res = winmm.waveInStart(self.hWaveIn)
            if res != 0:
                raise ErrRecord("ERROR: %s whilst starting recording."%get_error_text(res))
            self.status = Recorder.RECORDING

        def stop(self):
            """ Stops recording - triggers STOPPING, STOPPED sequence """
            if self.status != Recorder.RECORDING:
                return
            self.status = Recorder.STOPPING
            res = winmm.waveInReset(self.hWaveIn)
            if res != 0:
                raise ErrRecord("ERROR: %s whilst reseting device."%get_error_text(res))

        def poll(self):
            """ Must be repeatedly called whilst recording. A windows message loop """
            msg = MSG()
            while user32.PeekMessageA(byref(msg), 0, 0, 0, PM_REMOVE):
                if msg.message == WM_QUIT:
                    return 0
                user32.TranslateMessage(byref(msg))
                user32.DispatchMessageA(byref(msg))
            return 1;

        def wait(self, delay):
            """ Utility for recording a timed chunk of audio """
            end_time = time.time()+delay
            while 1:
                try:
                    if not self.poll(): break
                    curr_time = time.time()
                    if curr_time > end_time:
                        self.stop()
                    time.sleep(0.01)            # be polite
                except KeyboardInterrupt:
                    self.stop()

        def _stop(self):
            """ Close wave device after the buffers have been flushed and freed """
            self.ofp.close()
            res = winmm.waveInClose(self.hWaveIn)
            self.status = Recorder.STOPPED
            return res

        def _cb_open(self, devID):
            """ Callback from the message window as device is opened """
            pass

        def _cb_data(self, devID, whdr_address):
            """ Callback from the message window for each data block """
            try:
                buff = self.whdr[whdr_address]
                if len(buff) != 0:
                    self.ofp.writeframesraw(buff.read())
                if self.status == Recorder.STOPPING:
                    buff.inUnprepare()
                    del self.whdr[whdr_address]
                    if len(self.whdr) == 0: self._stop()
                elif self.status == Recorder.RECORDING:
                    buff.inAdd()
                else:
                    print "_cb_data() called unexpectedly!"
            except:
                pass

        def _cb_close(self, devID):
            """ Callback from the message window as device closed.
                Destroys the message window """
            try:
                self.w.destroy_window()
            except:
                pass


    def _main():
        opts, args = getopt.getopt(sys.argv[1:], '')
        if len(args) != 2:
            print "Usage: python.exe recordwin32.py filename duration"
            print "     filename - .wav file"
            print "     duration - seconds"
            print "e.g. python.exe recordwin32.py c:\\moloko.wav 30.0"
            sys.exit(0)
        filename = args[0]
        duration = float(args[1])

        r = Recorder()            # Defaults to 16-bit, 44100hz, Stereo
        r.start(filename)         # Record audio into file
        print "Press Ctrl-C to stop ..."
        r.wait(duration)          # duration (seconds)



    

else:
    print "Platform not supported yet"
    
    
    
    
    
    
if __name__=='__main__':
    # Play some effects
    winsound.Beep(440,10)
    winsound.Beep(600,10)
    #~ for i in range(40,400,30):
        #~ winsound.Beep(i,90)
    #~ for i in range(40,400,30):
        #~ winsound.Beep(i,90)
    #~ for i in range(40,400,30):
        #~ winsound.Beep(i,90)
    #~ for i in range(40,400,30):
        #~ winsound.Beep(i,90)
#region License

/* Copyright (c) 2005 Leslie Sanford
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

using System;
using System.Diagnostics;
using System.Threading;
using Multimedia;

namespace Multimedia.Midi
{
    public sealed partial class InputDevice
    {
        public override void Close()
        {
            #region Guard

            if(disposed)
            {
                return;
            }

            #endregion

            lock(lockObject)
            {
                disposing = true;

                Reset();

                while(sysExBufferCount > 0)
                {
                    Monitor.Wait(lockObject);
                }

                int result = midiInClose(handle);

                Debug.Assert(result == DeviceException.MMSYSERR_NOERROR);

                delegateQueue.Dispose();
                disposed = true;
            }
        }

        public void StartRecording()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            lock(lockObject)
            {
                int result = midiInStart(Handle);

                if(result == DeviceException.MMSYSERR_NOERROR)
                {
                    recording = true;
                }
                else
                {
                    throw new InputDeviceException(result);
                }
            }
        }

        public void StopRecording()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            #region Guard

            if(!recording)
            {
                return;
            }

            #endregion

            lock(lockObject)
            {
                int result = midiInStop(Handle);

                if(result == DeviceException.MMSYSERR_NOERROR)
                {
                    recording = false;
                }
                else
                {
                    throw new InputDeviceException(result);
                }
            }
        }

        public override void Reset()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            lock(lockObject)
            {
                int result = midiInReset(Handle);

                if(result == DeviceException.MMSYSERR_NOERROR)
                {
                    recording = false;
                }
                else
                {
                    throw new InputDeviceException(result);
                }
            }
        }

        public void AddSysExBuffer(int size)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }
            else if(size < 0)
            {
                throw new ArgumentOutOfRangeException("size", size,
                    "System exclusive buffer size out of range.");
            }

            #endregion

            #region Guard

            if(disposing)
            {
                return;
            }

            #endregion

            lock(lockObject)
            {
                // Initialize the MidiHeader builder.
                headerBuilder.BufferLength = size;
                headerBuilder.Build();

                // Get the pointer to the built MidiHeader.
                IntPtr headerPtr = headerBuilder.Result;

                // Prepare the header to be used.
                int result = midiInPrepareHeader(Handle, headerPtr, SizeOfMidiHeader);

                // If the header was perpared successfully.
                if(result == DeviceException.MMSYSERR_NOERROR)
                {
                    sysExBufferCount++;

                    // Add the buffer to the InputDevice.
                    result = midiInAddBuffer(Handle, headerPtr, SizeOfMidiHeader);

                    // If the buffer could not be added.
                    if(result != DeviceException.MMSYSERR_NOERROR)
                    {
                        // Unprepare header - there's a chance that this will fail 
                        // for whatever reason, but there's not a lot that can be
                        // done about it at this point.
                        midiInUnprepareHeader(Handle, headerPtr, SizeOfMidiHeader);

                        sysExBufferCount--;

                        // Destroy header.
                        headerBuilder.Destroy();

                        // Throw an exception.
                        throw new InputDeviceException(result);
                    }
                }
                // Else the header could not be prepared.
                else
                {
                    // Destroy header.
                    headerBuilder.Destroy();

                    // Throw an exception.
                    throw new InputDeviceException(result);
                }
            }
        }

        public static MidiInCaps GetDeviceCapabilities(int deviceID)
        {
            int result;
            MidiInCaps caps = new MidiInCaps();

            result = midiInGetDevCaps(deviceID, ref caps, SizeOfMidiHeader);

            if(result != DeviceException.MMSYSERR_NOERROR)
            {
                throw new InputDeviceException(result);
            }

            return caps;
        }

        public override void Dispose()
        {
            #region Guard

            if(disposed)
            {
                return;
            }

            #endregion

            Close();

            GC.SuppressFinalize(this);
        }

        public void Connect(Sink<ChannelMessage> sink)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            channelNotifier.Connect(sink);
        }

        public void Disconnect(Sink<ChannelMessage> sink)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            channelNotifier.Disconnect(sink);
        }

        public void Connect(Sink<SysExMessage> sink)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            sysExNotifier.Connect(sink);
        }

        public void Disconnect(Sink<SysExMessage> sink)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            sysExNotifier.Disconnect(sink);
        }

        public void Connect(Sink<SysCommonMessage> sink)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            sysCommonNotifier.Connect(sink);
        }

        public void Disconnect(Sink<SysCommonMessage> sink)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            sysCommonNotifier.Disconnect(sink);
        }

        public void Connect(Sink<SysRealtimeMessage> sink)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            sysRealtimeNotifier.Connect(sink);
        }

        public void Disconnect(Sink<SysRealtimeMessage> sink)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            sysRealtimeNotifier.Disconnect(sink);
        }

        public void Connect(Sink<int> sink)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            invalidShortMessageNotifier.Connect(sink);
        }

        public void Disconnect(Sink<int> sink)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            invalidShortMessageNotifier.Disconnect(sink);
        }

        public void Connect(Sink<byte[]> sink)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            invalidSysExMessageNotifier.Connect(sink);
        }

        public void Disconnect(Sink<byte[]> sink)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }

            #endregion

            invalidSysExMessageNotifier.Disconnect(sink);
        }
    }
}

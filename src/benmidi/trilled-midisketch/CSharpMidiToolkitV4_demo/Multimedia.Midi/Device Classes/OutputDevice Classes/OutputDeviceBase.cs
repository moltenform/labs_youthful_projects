#region License

/* Copyright (c) 2006 Leslie Sanford
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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using StateMachineToolkit;

namespace Multimedia.Midi
{
    public abstract class OutputDeviceBase : Device
    {
        [DllImport("winmm.dll")]
        protected static extern int midiOutReset(int handle);

        [DllImport("winmm.dll")]
        protected static extern int midiOutShortMsg(int handle, int message);

        [DllImport("winmm.dll")]
        protected static extern int midiOutPrepareHeader(int handle,
            IntPtr headerPtr, int sizeOfMidiHeader);

        [DllImport("winmm.dll")]
        protected static extern int midiOutUnprepareHeader(int handle,
            IntPtr headerPtr, int sizeOfMidiHeader);

        [DllImport("winmm.dll")]
        protected static extern int midiOutLongMsg(int handle,
            IntPtr headerPtr, int sizeOfMidiHeader);

        [DllImport("winmm.dll")]
        protected static extern int midiOutGetDevCaps(int deviceID,
            ref MidiOutCaps caps, int sizeOfMidiOutCaps);

        [DllImport("winmm.dll")]
        protected static extern int midiOutGetNumDevs();

        protected const int MOM_OPEN = 0x3C7;
        protected const int MOM_CLOSE = 0x3C8;
        protected const int MOM_DONE = 0x3C9;

        protected delegate void GenericDelegate<T>(T args);

        // Represents the method that handles messages from Windows.
        protected delegate void MidiOutProc(int handle, int msg, int instance, int param1, int param2);

        // For releasing buffers.
        private DelegateQueue bufferQueue = new DelegateQueue();
        
        protected readonly object lockObject = new object();

        // The number of bufferb still in the queue.
        private volatile int bufferCount = 0;

        // Builds MidiHeader structures for sending system exclusive messages.
        private MidiHeaderBuilder headerBuilder = new MidiHeaderBuilder();

        // The device handle.
        protected int hndle = 0;

        // Indicates whether the device has been disposed.
        private bool disposed = false;

        public OutputDeviceBase()
        {
        }

        ~OutputDeviceBase()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                bufferQueue.Dispose();
            }
        }

        public override void Reset()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            #endregion

            lock(lockObject)
            {
                // Reset the OutputDevice.
                int result = midiOutReset(Handle);                

                // If the OutputDevice could not be reset.
                if(result != DeviceException.MMSYSERR_NOERROR)
                {
                    // Throw an exception.
                    throw new OutputDeviceException(result);
                }
            }
        }
        
        public virtual void Send(ChannelMessage message)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            #endregion

            Send(message.Message);
        }
        
        public virtual void Send(SysExMessage message)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            #endregion

            lock(lockObject)
            {
                headerBuilder.InitializeBuffer(message);
                headerBuilder.Build();

                // Prepare system exclusive buffer.
                int result = midiOutPrepareHeader(Handle, headerBuilder.Result, SizeOfMidiHeader);

                // If the system exclusive buffer was prepared successfully.
                if(result == DeviceException.MMSYSERR_NOERROR)
                {
                    bufferCount++;

                    // Send system exclusive message.
                    result = midiOutLongMsg(Handle, headerBuilder.Result, SizeOfMidiHeader);

                    // If the system exclusive message could not be sent.
                    if(result != DeviceException.MMSYSERR_NOERROR)
                    {
                        midiOutUnprepareHeader(Handle, headerBuilder.Result, SizeOfMidiHeader);
                        bufferCount--;
                        headerBuilder.Destroy();

                        // Throw an exception.
                        throw new OutputDeviceException(result);
                    }
                }
                // Else the system exclusive buffer could not be prepared.
                else
                {
                    // Destroy system exclusive buffer.
                    headerBuilder.Destroy();

                    // Throw an exception.
                    throw new OutputDeviceException(result);
                }
            }
        }
        
        public virtual void Send(SysCommonMessage message)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
            #endregion

            Send(message.Message);
        }
        
        public virtual void Send(SysRealtimeMessage message)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            #endregion

            Send(message.Message);
        }

        protected void Send(int message)
        {
            lock(lockObject)
            {
                int result = midiOutShortMsg(Handle, message);

                if(result != DeviceException.MMSYSERR_NOERROR)
                {
                    throw new OutputDeviceException(result);
                }
            }
        }

        public static MidiOutCaps GetDeviceCapabilities(int deviceID)
        {
            MidiOutCaps caps = new MidiOutCaps();

            // Get the device's capabilities.
            int result = midiOutGetDevCaps(deviceID, ref caps, Marshal.SizeOf(caps));

            // If the capabilities could not be retrieved.
            if(result != DeviceException.MMSYSERR_NOERROR)
            {
                // Throw an exception.
                throw new OutputDeviceException(result);
            }

            return caps;
        }

        // Handles Windows messages.
        protected virtual void HandleMessage(int handle, int msg, int instance, int param1, int param2)
        {
            if(msg == MOM_OPEN)
            {
            }
            else if(msg == MOM_CLOSE)
            {
            }
            else if(msg == MOM_DONE)
            {
                // Invoke the operation for releasing system exclusive buffers.
                bufferQueue.BeginInvoke(new GenericDelegate<IntPtr>(ReleaseBuffer), 
                    new object[] { new IntPtr(param1) });
            }
        }

        // Releases buffers.
        private void ReleaseBuffer(IntPtr headerPtr)
        {
            lock(lockObject)
            {
                MidiHeader header = (MidiHeader)Marshal.PtrToStructure(headerPtr, typeof(MidiHeader));

                if(header.bytesRecorded > 0)
                {
                    byte[] buffer = new byte[header.bytesRecorded];

                    Marshal.Copy(header.data, buffer, 0, buffer.Length);

                    if(SynchronizingObject != null)
                    {                        
                        SynchronizingObject.BeginInvoke(
                            new GenericDelegate<BufferFinishedEventArgs>(OnBufferFinished), 
                            new object[] { new BufferFinishedEventArgs(buffer) });
                    }
                    else
                    {
                        OnBufferFinished(new BufferFinishedEventArgs(buffer));
                    }
                }

                // Unprepare the buffer.
                int result = midiOutUnprepareHeader(Handle, headerPtr, SizeOfMidiHeader);

                // The call to midiInUnpreapreHeader should not fail. 
                Debug.Assert(result == DeviceException.MMSYSERR_NOERROR);

                // Release the buffer resources.
                headerBuilder.Destroy(headerPtr);

                bufferCount--;

                Monitor.Pulse(lockObject);

                Debug.Assert(bufferCount >= 0);                
            }
        }

        public override void Dispose()
        {
            #region Guard

            if(disposed)
            {
                return;
            }

            #endregion

            lock(lockObject)
            {
                Dispose(true);

                disposed = true;

                GC.SuppressFinalize(this);
            }
        }
        
        public override int Handle
        {
            get
            {
                return hndle;
            }
        }

        protected bool IsDisposed
        {
            get
            {
                return disposed;
            }
        }

        public static int DeviceCount
        {
            get
            {
                return midiOutGetNumDevs();
            }
        }
    }
}

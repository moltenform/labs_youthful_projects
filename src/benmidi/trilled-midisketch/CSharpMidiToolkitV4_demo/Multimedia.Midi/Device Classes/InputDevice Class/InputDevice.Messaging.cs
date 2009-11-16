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
using System.Runtime.InteropServices;
using System.Threading;
using Multimedia;

namespace Multimedia.Midi
{
    public sealed partial class InputDevice : Device
    {
        private void HandleMessage(int handle, int msg, int instance, int param1, int param2)
        {
            if(msg == MIM_OPEN)
            {
            }
            else if(msg == MIM_CLOSE)
            {
            }
            else if(msg == MIM_DATA)
            {
                delegateQueue.BeginInvoke(shortMessageHandler, new object[] { param1 });
            }
            else if(msg == MIM_MOREDATA)
            {
                delegateQueue.BeginInvoke(shortMessageHandler, new object[] { param1 });
            }
            else if(msg == MIM_LONGDATA)
            {
                delegateQueue.BeginInvoke(sysExMessageHandler, new object[] { param1 });
            }
            else if(msg == MIM_ERROR)
            {
                delegateQueue.BeginInvoke(invalidShortMessageHandler, new object[] { param1 });
            }
            else if(msg == MIM_LONGERROR)
            {
                delegateQueue.BeginInvoke(invalidSysExMessageHandler, new object[] { param1 });
            }
        }

        private void HandleShortMessage(int message)
        {
            int status = ShortMessage.UnpackStatus(message);

            if(status >= (int)ChannelCommand.NoteOff &&
                status <= (int)ChannelCommand.PitchWheel +
                ChannelMessage.MidiChannelMaxValue)
            {
                cmBuilder.Message = message;
                cmBuilder.Build();

                if(SynchronizingObject != null)
                {
                    SynchronizingObject.BeginInvoke(
                        new GenericDelegate<ChannelMessage>(channelNotifier.Notify),
                        new object[] { cmBuilder.Result });
                }
                else
                {
                    channelNotifier.Notify(cmBuilder.Result);
                }
            }
            else if(status == (int)SysCommonType.MidiTimeCode ||
                status == (int)SysCommonType.SongPositionPointer ||
                status == (int)SysCommonType.SongSelect ||
                status == (int)SysCommonType.TuneRequest)
            {
                scBuilder.Message = message;
                scBuilder.Build();

                if(SynchronizingObject != null)
                {
                    SynchronizingObject.BeginInvoke(
                        new GenericDelegate<SysCommonMessage>(sysCommonNotifier.Notify), 
                        new object[] { scBuilder.Result });
                }
                else
                {
                    sysCommonNotifier.Notify(scBuilder.Result);
                }
            }
            else
            {
                SysRealtimeMessage sysRealtimeMessage = null;

                switch((SysRealtimeType)status)
                {
                    case SysRealtimeType.ActiveSense:
                        sysRealtimeMessage = SysRealtimeMessage.ActiveSenseMessage;
                        break;

                    case SysRealtimeType.Clock:
                        sysRealtimeMessage = SysRealtimeMessage.ClockMessage;
                        break;

                    case SysRealtimeType.Continue:
                        sysRealtimeMessage = SysRealtimeMessage.ContinueMessage;
                        break;

                    case SysRealtimeType.Reset:
                        sysRealtimeMessage = SysRealtimeMessage.ResetMessage;
                        break;

                    case SysRealtimeType.Start:
                        sysRealtimeMessage = SysRealtimeMessage.StartMessage;
                        break;

                    case SysRealtimeType.Stop:
                        sysRealtimeMessage = SysRealtimeMessage.StopMessage;
                        break;

                    case SysRealtimeType.Tick:
                        sysRealtimeMessage = SysRealtimeMessage.TickMessage;
                        break;
                }

                if(SynchronizingObject != null)
                {
                    SynchronizingObject.BeginInvoke(
                        new GenericDelegate<SysRealtimeMessage>(sysRealtimeNotifier.Notify), 
                        new object[] { sysRealtimeMessage });
                }
                else
                {
                    sysRealtimeNotifier.Notify(sysRealtimeMessage);
                }
            }
        }

        private void HandleSysExMessage(int param1)
        {
            lock(lockObject)
            {
                IntPtr headerPtr = new IntPtr(param1);
                MidiHeader header = (MidiHeader)Marshal.PtrToStructure(headerPtr, typeof(MidiHeader));

                if(header.bytesRecorded > 0 && !disposing)
                {
                    byte[] data = new byte[header.bytesRecorded];

                    Marshal.Copy(header.data, data, 0, data.Length);

                    SysExMessage message = new SysExMessage(data);

                    if(SynchronizingObject != null)
                    {
                        SynchronizingObject.BeginInvoke(onBufferFinished,
                            new object[] { new BufferFinishedEventArgs(data) });
                        SynchronizingObject.BeginInvoke(
                            new GenericDelegate<SysExMessage>(sysExNotifier.Notify), 
                            new object[] { message });
                    }
                    else
                    {
                        OnBufferFinished(new BufferFinishedEventArgs(data));
                        sysExNotifier.Notify(message);
                    }
                }

                int result = midiInUnprepareHeader(Handle, headerPtr, SizeOfMidiHeader);

                // The call to midiInUnpreapreHeader should not fail. 
                Debug.Assert(result == DeviceException.MMSYSERR_NOERROR);

                headerBuilder.Destroy(headerPtr);

                sysExBufferCount--;

                Debug.Assert(sysExBufferCount >= 0);

                Monitor.Pulse(lockObject);
            }
        }

        private void HandleInvalidShortMessage(int message)
        {
            if(SynchronizingObject != null)
            {
                SynchronizingObject.BeginInvoke(
                    new GenericDelegate<int>(invalidShortMessageNotifier.Notify), 
                    new object[] { message });
            }
            else
            {
                invalidShortMessageNotifier.Notify(message);
            }
        }

        private void HandleInvalidSysExMessage(int param1)
        {
            lock(lockObject)
            {
                IntPtr headerPtr = new IntPtr(param1);
                MidiHeader header = (MidiHeader)Marshal.PtrToStructure(headerPtr, typeof(MidiHeader));

                byte[] data = new byte[header.bytesRecorded];

                Marshal.Copy(header.data, data, 0, data.Length);

                if(!disposing)
                {
                    if(SynchronizingObject != null)
                    {
                        SynchronizingObject.BeginInvoke(onBufferFinished,
                            new object[] { new BufferFinishedEventArgs(data) });
                        SynchronizingObject.BeginInvoke(
                            new GenericDelegate<byte[]>(invalidSysExMessageNotifier.Notify),
                            new object[] { data });
                    }
                    else
                    {
                        OnBufferFinished(new BufferFinishedEventArgs(data));
                        invalidSysExMessageNotifier.Notify(data);
                    }
                }

                int result = midiInUnprepareHeader(Handle, headerPtr, SizeOfMidiHeader);

                // The call to midiInUnpreapreHeader should not fail. 
                Debug.Assert(result == DeviceException.MMSYSERR_NOERROR);

                headerBuilder.Destroy(headerPtr);

                sysExBufferCount--;

                Debug.Assert(sysExBufferCount >= 0);

                Monitor.Pulse(lockObject);
            }
        }
    }
}
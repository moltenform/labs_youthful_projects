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
using System.Collections.Generic;
using System.ComponentModel;

namespace Multimedia.Midi
{
    public sealed partial class MidiFilePlayer : MidiFilePlayerBase
    {
        private MidiInternalClock clock = new MidiInternalClock();

        private OutputDevice outDevice;

        private MessageDispatcher dispatcher = new MessageDispatcher();

        private ChannelChaser chaser = new ChannelChaser();

        private ChannelStopper stopper =  new ChannelStopper();

        public Sequence sequence = new Sequence();

        private int length = 0;

        private List<IEnumerator<int>> enumerators = new List<IEnumerator<int>>();

        public /*volatile */ int position = 0;

        private int ticks = 0;

        private int trackFinishedCount = 0;

        private volatile bool disposed = false;

        public event EventHandler<BeatElapsedEventArgs> BeatElapsed;

        public event EventHandler PlayingFinished;

        public MidiFilePlayer(OutputDevice deviceID)
        {
            outDevice = deviceID;// new OutputDevice(deviceID);

            outDevice.RunningStatusEnabled = true;

            clock.Tick += HandleTick;

            chaser.Connect(outDevice.Send);
            stopper.Connect(outDevice.Send);

            dispatcher.Connect(new Sink<MetaMessage>(delegate(MetaMessage message)
            {
                if(message.MetaType == MetaType.EndOfTrack)
                {
                    trackFinishedCount++;

                    if(trackFinishedCount == sequence.Count)
                    {
                        Send(MidiFilePlayer.PlayingFinishedID);
                    }
                }
                else
                {
                    clock.ProcessMessage(message);
                }
            }));

            dispatcher.Connect(stopper.ProcessMessage);
            dispatcher.Connect((Sink<SysExMessage>)outDevice.Send);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                clock.Dispose();
                outDevice.Dispose();
                disposed = true;
            }

            base.Dispose(disposing);
        }            

        #region Facade Methods

        public void Start()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            #endregion

            Send(StartID);    
        }

        public void Continue()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            #endregion

            Send(ContinueID);
        }

        public void Stop()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            #endregion

            Send(StopID);
        }

        public void Open(string fileName)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            #endregion

            IAsyncResult result = Send(OpenID, fileName);

            WaitForCompletion(result);
        }

        public void SetPosition(int position)
        {
            #region Require

            if(position < 0)
            {
                throw new ArgumentOutOfRangeException("position", position,
                    "Sequence position out of range.");
            }

            #endregion

            Send(SetPositionID, position);
        }

        #endregion

        public int GetPosition()
        {
            return position;
        }

        private void OnBeatElapsed(BeatElapsedEventArgs e)
        {
            EventHandler<BeatElapsedEventArgs> handler = BeatElapsed;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        private void OnPlayingFinished(EventArgs e)
        {
            EventHandler handler = PlayingFinished;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        private void HandleTick(object sender, EventArgs e)
        {
            foreach(IEnumerator<int> en in enumerators)
            {
                en.MoveNext();
            }

            position++;

            ticks++;

            if(ticks == sequence.Division && position <= length)
            {
                Send(BeatElapsedID, position);

                ticks = 0;
            }
        }

        #region Action Methods

        protected override void Start(object[] args)
        {
            position = ticks = 0;

            Continue(args);            
        }

        protected override void Continue(object[] args)
        {
            #region Guard

            if(position >= Length)
            {
                return;
            }

            #endregion

            trackFinishedCount = 0;

            enumerators.Clear();

            foreach(Track t in sequence)
            {
                enumerators.Add(t.TickIterator(position, chaser, dispatcher).GetEnumerator());
            }

            clock.Start();
        }

        protected override void Stop(object[] args)
        {
            clock.Stop();
            stopper.AllSoundOff();
        }

        protected override void SetPosition(object[] args)
        {
            position = (int)args[0];
            ticks = position % sequence.Division;
        }

        protected override void Open(object[] args)
        {
            sequence.Load((string)args[0]);

            if(sequence.SequenceType == SequenceType.Smpte)
            {
                throw new NotSupportedException(
                    "SMPTE sequences not supported.");
            }

            length = sequence.GetLength();
            clock.Ppqn = sequence.Division;
            position = ticks = 0;
        }

        protected override void RaiseBeatElapsed(object[] args)
        {
            OnBeatElapsed(new BeatElapsedEventArgs((int)args[0]));
        }

        protected override void RaisePlayingFinished(object[] args)
        {
            OnPlayingFinished(EventArgs.Empty);
        }

        #endregion

        public int Length
        {
            get
            {
                return length;
            }
        }
    }

    public class BeatElapsedEventArgs : EventArgs
    {
        private int position;

        public BeatElapsedEventArgs(int position)
        {
            this.position = position;
        }

        public int Position
        {
            get
            {
                return position;
            }
        }
    }
}

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
using Multimedia;

namespace Multimedia.Midi
{
	/// <summary>
	/// Generates clock events internally.
	/// </summary>
	public class MidiInternalClock : PpqnClock, IComponent, ISource<SysRealtimeMessage>
    {
        #region MidiInternalClock Members

        #region Fields

        private delegate void GenericDelegate<T>(T args);

        // Used for generating tick events.
        private Multimedia.Timer timer = new Timer();

        // Parses meta message tempo change messages.
        private TempoChangeBuilder builder = new TempoChangeBuilder();

        // Indicates whether the clock is in master mode.
        private bool masterEnabled = false;

        // Tick accumulator.
        private int ticks = 0;

        private Notifier<SysRealtimeMessage> sysRealtimeNotifier = new Notifier<SysRealtimeMessage>();

        // Indicates whether the clock has been disposed.
        private bool disposed = false;

        private ISite site = null;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the MidiInternalClock class.
        /// </summary>
		public MidiInternalClock() : base(Timer.Capabilities.periodMin)
		{ 
            timer.Period = Timer.Capabilities.periodMin;
            timer.Tick += new EventHandler(HandleTick); 
        }

        /// <summary>
        /// Initializes a new instance of the MidiInternalClock class with the 
        /// specified IContainer.
        /// </summary>
        /// <param name="container">
        /// The IContainer to which the MidiInternalClock will add itself.
        /// </param>
        public MidiInternalClock(IContainer container) : 
            base(Timer.Capabilities.periodMin)
        {
            ///
            /// Required for Windows.Forms Class Composition Designer support
            ///
            container.Add(this);

            timer.Period = Timer.Capabilities.periodMin;
            timer.Tick += new EventHandler(HandleTick);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the MidiInternalClock.
        /// </summary>
        public void Start()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("MidiInternalClock");
            }

            #endregion

            #region Guard

            if(running)
            {
                return;
            }

            #endregion

            Reset();

            // If master mode is enabled.
            if(MasterEnabled)
            {
                // Send system realtime start message.

                if(SynchronizingObject != null &&
                    SynchronizingObject.InvokeRequired)
                {
                    SynchronizingObject.BeginInvoke(
                        new GenericDelegate<SysRealtimeMessage>(sysRealtimeNotifier.Notify),
                        new object[] { SysRealtimeMessage.StartMessage });
                }
                else
                {
                    sysRealtimeNotifier.Notify(SysRealtimeMessage.StartMessage);
                }
            }          

            // Start the multimedia timer in order to start generating ticks.
            timer.Start();

            // Indicate that the clock is now running.
            running = true;

            // Raise Started event.
            if(SynchronizingObject != null &&
                SynchronizingObject.InvokeRequired)
            {
                SynchronizingObject.BeginInvoke(
                    new GenericDelegate<EventArgs>(OnStarted),
                    new object[] { EventArgs.Empty });
            }
            else
            {
                OnStarted(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Resumes tick generation from the current position.
        /// </summary>
        public void Continue()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("MidiInternalClock");
            }

            #endregion

            #region Guard

            if(running)
            {
                return;
            }

            #endregion

            // If master mode is enabled.
            if(MasterEnabled)
            {
                // Send system realtime continue message.

                if(SynchronizingObject != null &&
                    SynchronizingObject.InvokeRequired)
                {
                    SynchronizingObject.BeginInvoke(
                        new GenericDelegate<SysRealtimeMessage>(sysRealtimeNotifier.Notify),
                        new object[] { SysRealtimeMessage.ContinueMessage });
                }
                else
                {
                    sysRealtimeNotifier.Notify(SysRealtimeMessage.ContinueMessage);
                }
            }

            // Start multimedia timer in order to start generating ticks.
            timer.Start();

            // Indicate that the clock is now running.
            running = true;

            // Raise Continued event.
            if(SynchronizingObject != null &&
                SynchronizingObject.InvokeRequired)
            {
                SynchronizingObject.BeginInvoke(
                    new GenericDelegate<EventArgs>(OnContinued),
                    new object[] { EventArgs.Empty });
            }
            else
            {
                OnContinued(EventArgs.Empty);
            }          
        }

        /// <summary>
        /// Stops the MidiInternalClock.
        /// </summary>
        public void Stop()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("MidiInternalClock");
            }

            #endregion

            #region Guard

            if(!running)
            {
                return;
            }

            #endregion

            // Stop the multimedia timer.
            timer.Stop();

            // If master mode is enabled.
            if(MasterEnabled)
            {
                // Send system realtime stop message.

                if(SynchronizingObject != null &&
                    SynchronizingObject.InvokeRequired)
                {
                    SynchronizingObject.BeginInvoke(
                        new GenericDelegate<SysRealtimeMessage>(sysRealtimeNotifier.Notify),
                        new object[] { SysRealtimeMessage.StopMessage });
                }
                else
                {
                    sysRealtimeNotifier.Notify(SysRealtimeMessage.StopMessage);
                }
            }

            // Indicate that the clock is not running.
            running = false;

            // Raise Stopped event.
            if(SynchronizingObject != null &&
                SynchronizingObject.InvokeRequired)
            {
                SynchronizingObject.BeginInvoke(
                    new GenericDelegate<EventArgs>(OnStopped),
                    new object[] { EventArgs.Empty });
            }
            else
            {
                OnStopped(EventArgs.Empty);
            }       
        }

        /// <summary>
        /// Processes tempo MetaMessages. 
        /// </summary>
        /// <param name="message">
        /// The MetaMessage to process.
        /// </param>
        public void ProcessMessage(MetaMessage message)
        {
            #region Require

            if(message == null)
            {
                throw new ArgumentNullException("message");
            }

            #endregion

            // If this is a tempo change message.
            if(message.MetaType == MetaType.Tempo)
            {
                TempoChangeBuilder builder = new TempoChangeBuilder(message);

                // Set the new tempo.
                Tempo = builder.Tempo;
            }
        }

        #region Event Raiser Methods

        protected virtual void OnDisposed(EventArgs e)
        {
            EventHandler handler = Disposed;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Event Handler Methods

        // Handles Tick events generated by the multimedia timer.
        private void HandleTick(object sender, EventArgs e)
        {
            int t = GenerateTicks();

            OnTick(t);

            ticks += t;

            if(MasterEnabled)
            {
                while(ticks >= TicksPerClock)
                {
                    ticks -= TicksPerClock;

                    sysRealtimeNotifier.Notify(SysRealtimeMessage.ClockMessage);
                }
            }             
        }        

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the master mode has been
        /// enabled.
        /// </summary>
        public bool MasterEnabled
        {
            get
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("MidiInternalClock");
                }

                #endregion

                return masterEnabled;
            }
            set
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("MidiInternalClock");
                }

                #endregion

                masterEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the tempo in microseconds per beat.
        /// </summary>
        public int Tempo
        {
            get
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("MidiInternalClock");
                }

                #endregion

                return GetTempo();
            }
            set
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("MidiInternalClock");
                }

                #endregion

                SetTempo(value);
            }
        }

        /// <summary>
        /// Gets or sets the ISynchronizeInvoke object to use for marshaling
        /// events.
        /// </summary>
        public ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("MidiInternalClock");
                }

                #endregion

                return timer.SynchronizingObject;
            }
            set
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("MidiInternalClock");
                }

                #endregion

                timer.SynchronizingObject = value;
            }
        }        

        #endregion

        #endregion

        #region IComponent Members

        public event EventHandler Disposed;

        public ISite Site
        {
            get
            {
                return site;
            }
            set
            {
                site = value;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            #region Guard

            if(disposed)
            {
                return;
            }

            #endregion            

            if(running)
            {
                // Stop the multimedia timer.
                timer.Stop();
            }            

            disposed = true;             

            timer.Dispose();

            GC.SuppressFinalize(this);

            OnDisposed(EventArgs.Empty);
        }

        #endregion

        #region ISource<SysRealtimeMessage> Members

        public void Connect(Sink<SysRealtimeMessage> sink)
        {
            sysRealtimeNotifier.Connect(sink);
        }

        public void Disconnect(Sink<SysRealtimeMessage> sink)
        {
            sysRealtimeNotifier.Disconnect(sink);
        }

        #endregion
    }
}

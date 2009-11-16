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

namespace Multimedia.Midi
{
    /// <summary>
    /// Dispatches IMidiMessages to their corresponding sink.
    /// </summary>
    public class MessageDispatcher : ISource<ChannelMessage>, ISource<SysExMessage>, ISource<MetaMessage>, 
        ISource<SysCommonMessage>, ISource<SysRealtimeMessage>
    {
        #region MessageDispatcher Members

        #region Fields

        private Notifier<ChannelMessage> channelNotifier = new Notifier<ChannelMessage>();

        private Notifier<SysExMessage> sysExNotifier = new Notifier<SysExMessage>();

        private Notifier<MetaMessage> metaNotifier = new Notifier<MetaMessage>();

        private Notifier<SysCommonMessage> sysCommonNotifier = new Notifier<SysCommonMessage>();

        private Notifier<SysRealtimeMessage> sysRealtimeNotifier = new Notifier<SysRealtimeMessage>();

        #endregion

        #region Methods

        /// <summary>
        /// Dispatches IMidiMessages to their corresponding sink.
        /// </summary>
        /// <param name="message">
        /// The IMidiMessage to dispatch.
        /// </param>
        public void Dispatch(IMidiMessage message)
        {
            #region Require

            if(message == null)
            {
                throw new ArgumentNullException("message");
            }

            #endregion

            switch(message.MessageType)
            {
                case MessageType.Channel:
                    channelNotifier.Notify((ChannelMessage)message);
                    break;

                case MessageType.SystemExclusive:
                    sysExNotifier.Notify((SysExMessage)message);
                    break;

                case MessageType.Meta:
                    metaNotifier.Notify((MetaMessage)message);
                    break;

                case MessageType.SystemCommon:
                    sysCommonNotifier.Notify((SysCommonMessage)message);
                    break;

                case MessageType.SystemRealtime:
                    sysRealtimeNotifier.Notify((SysRealtimeMessage)message);
                    break;
            }
        }

        #endregion

        #endregion

        #region ISource<ChannelMessage> Members

        public void Connect(Sink<ChannelMessage> sink)
        {
            channelNotifier.Connect(sink);
        }

        public void Disconnect(Sink<ChannelMessage> sink)
        {
            channelNotifier.Disconnect(sink);
        }

        #endregion

        #region ISource<SysExMessage> Members

        public void Connect(Sink<SysExMessage> sink)
        {
            sysExNotifier.Connect(sink);
        }

        public void Disconnect(Sink<SysExMessage> sink)
        {
            sysExNotifier.Disconnect(sink);
        }

        #endregion

        #region ISource<MetaMessage> Members

        public void Connect(Sink<MetaMessage> sink)
        {
            metaNotifier.Connect(sink);
        }

        public void Disconnect(Sink<MetaMessage> sink)
        {
            metaNotifier.Disconnect(sink);
        }

        #endregion

        #region ISource<SysCommonMessage> Members

        public void Connect(Sink<SysCommonMessage> sink)
        {
            sysCommonNotifier.Connect(sink);
        }

        public void Disconnect(Sink<SysCommonMessage> sink)
        {
            sysCommonNotifier.Disconnect(sink);
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

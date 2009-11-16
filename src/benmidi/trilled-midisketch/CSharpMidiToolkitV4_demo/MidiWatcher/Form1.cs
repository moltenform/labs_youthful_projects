using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Multimedia;
using Multimedia.Midi;

namespace MidiWatcher
{
    public partial class Form1 : Form
    {
        private const int SysExBufferSize = 1024;

        private InputDevice inDevice = null;

        public Form1()
        {
            InitializeComponent();

            if(InputDevice.DeviceCount == 0)
            {
                MessageBox.Show("No MIDI input devices available.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Close();
            }
            else
            {
                try
                {
                    inDevice = new InputDevice(0);
                    inDevice.SynchronizingObject = this;
                    inDevice.Connect((Sink<ChannelMessage>)ProcessMessage);
                    inDevice.Connect((Sink<SysExMessage>)ProcessMessage);
                    inDevice.Connect((Sink<SysCommonMessage>)ProcessMessage);
                    inDevice.Connect((Sink<SysRealtimeMessage>)ProcessMessage);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Close();
                }
            }
        }        

        protected override void OnClosed(EventArgs e)
        {
            if(inDevice != null)
            {
                inDevice.Close();
            }

            base.OnClosed(e);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            channelListBox.Items.Clear();

            try
            {
                inDevice.AddSysExBuffer(SysExBufferSize);
                inDevice.AddSysExBuffer(SysExBufferSize);
                inDevice.StartRecording();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            try
            {
                inDevice.StopRecording();
                inDevice.Reset();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void ProcessMessage(ChannelMessage message)
        {
            channelListBox.Items.Add(
                message.Command.ToString() + '\t' + '\t' +
                message.MidiChannel.ToString() + '\t' +
                message.Data1.ToString() + '\t' +
                message.Data2.ToString());

            channelListBox.SelectedIndex = channelListBox.Items.Count - 1;
        }

        private void ProcessMessage(SysExMessage message)
        {
            string result = "\n\n"; ;

            foreach(byte b in message.GetBytes())
            {
                result += string.Format("{0:X2} ", b);
            }

            sysExRichTextBox.Text += result;

            try
            {
                inDevice.AddSysExBuffer(SysExBufferSize);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void ProcessMessage(SysCommonMessage message)
        {
            sysCommonListBox.Items.Add(
                message.SysCommonType.ToString() + '\t' + '\t' +
                message.Data1.ToString() + '\t' +
                message.Data2.ToString());

            sysCommonListBox.SelectedIndex = sysCommonListBox.Items.Count - 1;
        }

        private void ProcessMessage(SysRealtimeMessage message)
        {
            sysRealtimeListBox.Items.Add(
                message.SysRealtimeType.ToString());

            sysRealtimeListBox.SelectedIndex = sysRealtimeListBox.Items.Count - 1;            
        }
    }
}
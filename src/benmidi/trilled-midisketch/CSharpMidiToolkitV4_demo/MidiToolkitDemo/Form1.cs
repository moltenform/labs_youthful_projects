using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Multimedia.Midi;

namespace MidiFilePlayerDemo
{
    public partial class Form1 : Form
    {
        private bool scrolling = false;

        private MidiFilePlayer midiFilePlayer1 = null;

        public Form1()
        {
            InitializeComponent();

            if(OutputDevice.DeviceCount == 0)
            {
                MessageBox.Show("No MIDI output devices available.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Close();
            }
            else
            {
                try
                {
                    midiFilePlayer1 = new MidiFilePlayer(0);
                    midiFilePlayer1.BeatElapsed += midiFilePlayer1_BeatElapsed;
                    midiFilePlayer1.PlayingFinished += midiFilePlayer1_PlayingFinished;
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
            if(midiFilePlayer1 != null)
            {
                midiFilePlayer1.Dispose();;
            }

            base.OnClosed(e);
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openMidiFileDialog.FileName;

                try
                {
                    midiFilePlayer1.Open(fileName);
                    positionHScrollBar.Value = 0;
                    positionHScrollBar.Maximum = midiFilePlayer1.Length;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.InnerException.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }                
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutDialog dlg = new AboutDialog();

            dlg.ShowDialog();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            midiFilePlayer1.Stop();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            midiFilePlayer1.Start();
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            midiFilePlayer1.Continue();
        }

        private void positionHScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if(e.Type == ScrollEventType.EndScroll)
            {
                midiFilePlayer1.SetPosition(positionHScrollBar.Value);
                scrolling = false;
            }
            else
            {
                scrolling = true;
            }
        }

        private void midiFilePlayer1_PlayingFinished(object sender, EventArgs e)
        {
        }

        private void midiFilePlayer1_BeatElapsed(object sender, BeatElapsedEventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new EventHandler<BeatElapsedEventArgs>(midiFilePlayer1_BeatElapsed), sender, e);
            }
            else
            {
                if(!scrolling)
                {
                    positionHScrollBar.Value = e.Position;
                }
            }
        }
    }
}
//Ben Fisher, 2008
//halfhourhacks.blogspot.com
//GPLv3 Licence

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using Microsoft.DirectX.DirectInput;

namespace AudioPad
{
    public partial class Form1 : Form
    {
        private Microsoft.DirectX.DirectSound.Device deviceSound;
        private Microsoft.DirectX.DirectInput.Device deviceInput;
        private JoystickState deviceInputState;
        private SoundWaves soundWaves;

        public Form1()
        {
            InitializeComponent();

            deviceSound = new Microsoft.DirectX.DirectSound.Device();
            deviceSound.SetCooperativeLevel(Handle, CooperativeLevel.Priority);
            Init();
            GetGamePads();

            //the soundwaves class contains methods for synthesizing audio, and playing static audio
            soundWaves = new SoundWaves(deviceSound);
        }
        

        private void Init()
        {
            addbuffer = new byte[ADDBUFFERSIZE];
            
            // prepare for streaming audio
            WaveFormat format = new WaveFormat();
            short bytesPersample = 2;
            format.BitsPerSample = 16;
            format.Channels = 1;
            format.BlockAlign = bytesPersample; 
            format.FormatTag = WaveFormatTag.Pcm;
            format.SamplesPerSecond = 44100;
            format.AverageBytesPerSecond = format.SamplesPerSecond * bytesPersample;
            
            BufferDescription desc = new BufferDescription(format);
            desc.DeferLocation = true;
            desc.BufferBytes = BUFFERSIZE;
            bufferstream = new SecondaryBuffer(desc, deviceSound);
        }

        SecondaryBuffer bufferstream;
        double BASEFREQ = 98.0; // one waveform = 450 samples long
        int BUFFERSIZE = 450 * 2 * 30; //enough room for 30 waveforms, 0.306 seconds
        int ADDBUFFERSIZE = 450 * 2 * 30; //30; //enough room for 30 waveforms, 0.306 seconds

        byte[] addbuffer; // contains 30 waveforms

       
        private void btnGo_Click(object sender, EventArgs e)
        {
            // Button is either "stop" or "go" 
            if (!timer.Enabled)
            {
                timer_Tick(null, null); //fill buffer initially
                bufferstream.Play(0, BufferPlayFlags.Looping);
                timer.Enabled = true;
                timerDisplay.Enabled = true;
                this.MainMenuStrip.Enabled = false;

                if (this.deviceInput != null)
                    timerPoll.Enabled = true; // if game pad connected, poll the input device !

                btnGo.Text = "Stop";
            }
            else
            {
                bufferstream.Stop();
                timer.Enabled = false;
                timerDisplay.Enabled = false;
                timerPoll.Enabled = false;
                this.MainMenuStrip.Enabled = true;
                btnGo.Text = "Start";
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            int nOffset = bufferstream.WritePosition;

            // if you only write at the offset, there will be a jump, and a click for every time.
            // rewriting the entire buffer solves this
            //(is it bad to empty the entire buffer when playing? seems to work. LockFlag.FromWriteCursor would have been complicated.)

            if (this.deviceInput == null)
                create16bit_quad(0.3, getpitchscalefromslider(), getwidthscalefromslider(), getsmoothnessfromslider());
            else
                create16bit_quad(0.3, getpitchscalefromslider(), currentJoystickWidth, currentJoystickSmoothness);

            bufferstream.Write(0, addbuffer, LockFlag.EntireBuffer);
        }



        //the reason pitch is restricted is that we want the samples to fit perfectly in the buffer. Otherwise you hear the break.
        //this could be solved by changing phase to make transition smooth.
        double[] pitchscales = new double[] { 1, 1.5, 2, 2.5, 3, 4.5, 5, 6, 7.5, 9 };
        private double getpitchscalefromslider()
        {
           return pitchscales[slidePitch.Value];
        }
        private double getwidthscalefromslider()
        {
            return slideWidth.Value * 2.0 / 200.0; // from -1 to 1
        }
        private double getsmoothnessfromslider()
        {
            return (slideSmooth.Value * 10.0 / 200.0) + 1; // from 1 to 10
        }
       
        // playing a standard sine wave,  left as a reference
        private void create16bit_sine(double amp, double pitchscale)
        {
            double frequency = BASEFREQ * pitchscale;

            double timeScale = frequency * 2 * Math.PI / (double)44100;
            int waveformPeriod = (int)(2 * Math.PI / timeScale);
            for (int i = 0; i < addbuffer.Length / 2; i++)
            {
                if (i <= waveformPeriod)
                {
                    double dbl = Math.Sin(i * timeScale) * amp;
                    short sh = (short)(dbl * short.MaxValue);

                    addbuffer[i * 2] = (byte)(sh & 0x00FF); // low byte
                    addbuffer[i * 2 + 1] = (byte)(sh >> 8); // high byte
                }
                else  // we have already computed the wave, periodic, so just use that
                {
                    int prevspot = i % waveformPeriod;
                    addbuffer[i * 2] = addbuffer[prevspot * 2];
                    addbuffer[i * 2 + 1] = addbuffer[prevspot * 2 + 1];
                }
            }
        }

        //Utility function for create16bit_quad. Checks bounds of value, adds it to the buffer, increments counter
        private void addpoint(double point,short peak, ref int j)
        {
            short sh;
            if (point < -peak) sh = (short)-peak;
            else if (point > peak) sh = (short)peak;
            else sh = (short)point;
            addbuffer[j * 2] = (byte)(sh & 0x00FF);
            addbuffer[j * 2 + 1] = (byte)(sh >> 8);
            j++;
        }
        //quadratic approximation. Convinient because it can approximate sine wave, square wave, and in-between
        private void create16bit_quad(double amp, double pitchscale, double pwidth /*-1 to 1*/, double smoothness /*1 to 10ish*/)
        {
            double frequency = BASEFREQ * pitchscale;

            double timeScale = frequency * 2 * Math.PI / (double)44100;
            int waveformPeriod = (int)(2 * Math.PI / timeScale);
            int peak =(int) (amp * short.MaxValue);
            int aimpeak = (int) (peak * smoothness); // largr if smoothness less

            int quartersection = (int)((waveformPeriod - waveformPeriod * Math.Abs(pwidth)) / 4.0);
            int flatsection = waveformPeriod - (quartersection*4);

            int w = quartersection*2;
            double factor = 4.0 * aimpeak / ((double) w*w);
            double val = 0; int x;
            short shpeak = (short)peak; short nshpeak = (short)-peak;

            // create the waveform
            int j=0;
            if (pwidth > 0)
            {
                for (int i = 0; i < quartersection; i++)
                {
                    x = i - quartersection;
                    val = aimpeak - (x * x * factor);
                    addpoint(val, shpeak, ref j);
                }
                for (int i = 0; i < flatsection; i++)
                {
                    val = peak;
                    addpoint(val, shpeak, ref j);
                }
                for (int i = 0; i < quartersection; i++)
                {
                    x = i;
                    val = aimpeak - (x * x * factor);
                    addpoint(val, shpeak, ref j);
                }
                for (int i = 0; i < quartersection * 2; i++)
                {
                    x = i - quartersection;
                    val = -aimpeak + (x * x * factor);
                    addpoint(val, shpeak, ref j);
                }
            }
            else
            {
                for (int i = 0; i < quartersection * 2; i++)
                {
                    x = i - quartersection;
                    val = aimpeak - (x * x * factor);
                    addpoint(val, shpeak, ref j);
                }
                
                for (int i = 0; i < quartersection; i++)
                {
                    x = i - quartersection;
                    val = -aimpeak + (x * x * factor);
                    addpoint(val, shpeak, ref j);
                }
                for (int i = 0; i < flatsection; i++)
                {
                    val = -peak;
                    addpoint(val, shpeak, ref j);
                }
                for (int i = 0; i < quartersection * 2; i++)
                {
                    x = i ;
                    val = -aimpeak + (x * x * factor);
                    addpoint(val, shpeak, ref j);
                }
            }
            for (int i = waveformPeriod; i < addbuffer.Length / 2; i++)
            {
                // we have already computed the wave, periodic, so just use that
                {
                    int prevspot = i % waveformPeriod;
                    addbuffer[i * 2] = addbuffer[prevspot * 2];
                    addbuffer[i * 2 + 1] = addbuffer[prevspot * 2 + 1];
                }
            }
            // We have filled the temp buffer. Cool.
        }

      
        

        private void GetGamePads()
        {
            // Reference code from http://blogs.msdn.com/coding4fun/archive/2006/11/03/940908.aspx
            this.deviceInput = null;
            this.lblStatus.Text = "Game controller not found. Use sliders.";
            DeviceList gameControllerList = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
            if (gameControllerList.Count > 0)
            {
                foreach (DeviceInstance deviceInstance in gameControllerList)
                {
                    // not a loop, get the first one available, and then break
                    this.deviceInput = new Microsoft.DirectX.DirectInput.Device(deviceInstance.InstanceGuid);
                    this.deviceInput.SetCooperativeLevel(this, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
                    this.deviceInput.SetDataFormat(DeviceDataFormat.Joystick);
                    this.lblStatus.Text = "Game controller connected. Use joystick.";
                    this.slideSmooth.Visible = false; this.slideWidth.Visible = false; this.lblWidth.Visible = false; this.lblSmooth.Visible = false;
                    break;
                }
            }
        }

        // refresh display of wave
        private void timerDisplay_Tick(object sender, EventArgs e)
        {
            double freqScale = getpitchscalefromslider();
            this.waveformPlot1.SetCurve(this.addbuffer, (int)(450 * 2 / freqScale), 16);
        }

        double currentJoystickWidth = 0.0, currentJoystickSmoothness = 1.0;
        private void timerPoll_Tick(object sender, EventArgs e)
        {
            try
            {
                deviceInput.Poll();
                deviceInputState = deviceInput.CurrentJoystickState;
            }
            catch (NotAcquiredException)
            {
                // try to reqcquire the device
                try { deviceInput.Acquire();  }
                catch (InputException iex) { Console.WriteLine(iex.Message);  }
            }
            catch (InputException ex2) { Console.WriteLine(ex2.Message); }

            this.currentJoystickSmoothness = ((deviceInputState.Y / ((double)65536))) * 20 + 1;
            this.currentJoystickWidth = (deviceInputState.X * 2 / ((double) 65536) - 1.0) * 0.7;
        }

        private void MenuPlayCurrentSound_Click(object sender, EventArgs e)
        {
            double freqScale = getpitchscalefromslider();

            create16bit_quad(0.3, freqScale, getwidthscalefromslider(), getsmoothnessfromslider());
            soundWaves.playbytes(addbuffer, 16, 44100);
            this.waveformPlot1.SetCurve(this.addbuffer, (int)(450 * 2 / freqScale), 16);
            
        }

        // yay for delegates
        delegate byte[] WaveformSynthesisFunction(double frequency, double amp, double duration);
        private void menuSine_Click(object sender, EventArgs e) { menuPlay(soundWaves.sine); }
        private void menuTriangle_Click(object sender, EventArgs e) { menuPlay(soundWaves.triangle); }
        private void menuSquare_Click(object sender, EventArgs e) { menuPlay(soundWaves.square); }
        private void menuSawtooth_Click(object sender, EventArgs e) { menuPlay(soundWaves.sawtooth); }
        private void menuWhitenoise_Click(object sender, EventArgs e) { menuPlay(soundWaves.whitenoise); }
        private void menuRednoise_Click(object sender, EventArgs e) { menuPlay(soundWaves.rednoise); }

        private void menuPlay(WaveformSynthesisFunction fn)
        {
            double freq = BASEFREQ * getpitchscalefromslider();
            byte[] sounddata = fn(freq, 0.3, 1.0);
            soundWaves.playbytes(sounddata, 16, 44100);
            //update plot
            int nbytes = (int) (2 * 44100 / freq);
            this.waveformPlot1.SetCurve(sounddata, nbytes, 16);
        }
        

    }
}
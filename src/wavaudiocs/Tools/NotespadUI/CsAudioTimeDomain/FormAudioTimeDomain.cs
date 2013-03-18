using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using CsWaveAudio;
using System.Diagnostics;

namespace CsAudioTimeDomain
{
    public partial class FormAudioTimeDomain : Form
    {
        public readonly string Version = "0.0.2";
        public readonly string PATH_BACKEND_PARAMS = @"C:\pydev\b_space_svn\audionew\Tools\NotespadBackend\vstudionotespad\ParamsSet.cs";
        public readonly string PATH_BACKEND_SRC = @"C:\pydev\b_space_svn\audionew\Tools\NotespadBackend\vstudionotespad\CNotespad.cs";
        public readonly string PATH_BACKEND_WAV = @"..\..\..\..\NotespadBackend\output.wav";

        private double paramRange = 1.0;
        private double[] paramValues = new double[4]; //inits to 0.0
        private Label[] lblParamLabels;
        private TrackBar[] tbParamTrackBars;

        private string strInitialDir="";
        private string strMediaDirectory="";
        private AudioPlayer aplayer;
        public FormAudioTimeDomain()
        {
            strInitialDir=Path.GetFullPath(".");
            if (!strInitialDir.EndsWith("\\")) strInitialDir += "\\";
            strMediaDirectory = Path.GetFullPath(@"..\..\..\..\..\Media\");
            if (!strMediaDirectory.EndsWith("\\")) strMediaDirectory += "\\";
            InitializeComponent();
            this.lblParamLabels = new Label[4];
            this.lblParamLabels[0]= lblParam1;
            this.lblParamLabels[1]= lblParam2;
            this.lblParamLabels[2]= lblParam3;
            this.lblParamLabels[3]= lblParam4;
            this.tbParamTrackBars = new TrackBar[] { tbParam1, tbParam2, tbParam3, tbParam4 };

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);

            aplayer = new AudioPlayer();
            mnuFileNew_Click(null, null);
        }
      
        private void btnSendParams_Click(object sender, EventArgs e) {
            //send params over
            string sSource = "public static class ParamsSet {";
            sSource += "\r\n public static double c1=" + this.paramValues[0].ToString(CultureInfo.InvariantCulture) + ";";
            sSource += "\r\n public static double c2=" + this.paramValues[1].ToString(CultureInfo.InvariantCulture) + ";";
            sSource += "\r\n public static double c3=" + this.paramValues[2].ToString(CultureInfo.InvariantCulture) + ";";
            sSource += "\r\n public static double c4=" + this.paramValues[3].ToString(CultureInfo.InvariantCulture) + ";";
            sSource += "};";
            using (TextWriter tw = new StreamWriter(PATH_BACKEND_PARAMS))
                tw.Write(sSource);
        }
        private string getSrcText()
        {
            string s;
            using (TextReader tr = new StreamReader(PATH_BACKEND_SRC))
                s = tr.ReadToEnd();
            return s;
        }
        private void setSrcText(string s)
        {
            if (s.Contains("//$$Main"))
            {
                //an old style one...
                s = "using System;\r\nusing CsWaveAudio;\r\n\r\n"+s;
                s=s.Replace("//$$Main", "public class CNotespad {\r\n public static WaveAudio Run() { WaveAudio result; ");
                s=s.Replace("//$$EndMain", "return result; \r\n}\r\n}");
                s=s.Replace("loadWav(", "Program.loadSample(");
            }
            using (TextWriter tw = new StreamWriter(PATH_BACKEND_SRC))
            {
                tw.Write(s);
            }
        }




        private void tbParam1_Scroll(object sender, EventArgs e) { onScroll(0); }
        private void tbParam2_Scroll(object sender, EventArgs e) { onScroll(1); }
        private void tbParam3_Scroll(object sender, EventArgs e) { onScroll(2); }
        private void tbParam4_Scroll(object sender, EventArgs e) { onScroll(3); }
        private void onScroll(int i)
        {
            TrackBar tb=this.tbParamTrackBars[i]; Label lbl=this.lblParamLabels[i];
            double v;
            if (i==0 || i==1)
                v = (tb.Value / ((double)tb.Maximum))*this.paramRange; 
            else
                v = (tb.Value / ((double)tb.Maximum))*2.0 - 1.0; 
            lbl.Text = v.ToString("0.####"); //4 decimals or fewer
            this.paramValues[i]=v;
        }

        //new format with newlines. facilitates merging code, and viewing in other editor.
        private string MARK = "\n_!@@!_\n";
        //1) 'baud'
        //2) version 0.2
        //3) src code
        //4) param ranges
        //5-8) parameters
        private void saveToBaud(string sFilename)
        {
            if (getSrcText().Contains(MARK)) {MessageBox.Show("Cannot save the string "+MARK+" in source."); return;}
            using (TextWriter tw = new StreamWriter(sFilename))
            {
                tw.Write("baud"); tw.Write(MARK);
                tw.Write("0.1"); tw.Write(MARK);
                tw.Write(getSrcText()); tw.Write(MARK);
                tw.Write(paramRange.ToString(CultureInfo.InvariantCulture)); tw.Write(MARK);
                tw.Write(paramValues[0].ToString(CultureInfo.InvariantCulture)); tw.Write(MARK);
                tw.Write(paramValues[1].ToString(CultureInfo.InvariantCulture)); tw.Write(MARK);
                tw.Write(paramValues[2].ToString(CultureInfo.InvariantCulture)); tw.Write(MARK);
                tw.Write(paramValues[3].ToString(CultureInfo.InvariantCulture)); tw.Write(MARK);
            }
        }
        private void loadFromBaud(string sFilename)
        {
            string s;
            using (TextReader tr = new StreamReader(sFilename))
                s = tr.ReadToEnd();
            s=s.Replace("\r\n", "\n");
            string[] sParts = s.Split(new string[] { MARK }, StringSplitOptions.None);
            if (sParts[0]!="baud") { MessageBox.Show("Not a audiotimedomain file."); return; }
            if (sParts[1]!="0.1") { MessageBox.Show("Opening future file, may not work perfectly."); return; }
            if (sParts.Length<8) {MessageBox.Show("Could not open file. Not enough sections."); return;}
            try
            {
                this.paramRange = double.Parse(sParts[3], CultureInfo.InvariantCulture);
                this.paramValues[0] = double.Parse(sParts[4], CultureInfo.InvariantCulture);
                this.paramValues[1] = double.Parse(sParts[5], CultureInfo.InvariantCulture);
                this.paramValues[2] = double.Parse(sParts[6], CultureInfo.InvariantCulture);
                this.paramValues[3] = double.Parse(sParts[7], CultureInfo.InvariantCulture);
                this.setSrcText(sParts[2]);
            }
            catch (InvalidCastException err)
            {
                MessageBox.Show("Loading Error:"+err.ToString());  return;
            }
            setSliderToValue(0); setSliderToValue(1); setSliderToValue(2); setSliderToValue(3);
            btnSendParams_Click(null, null);
        }
        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg1 = new OpenFileDialog();
            dlg1.RestoreDirectory = true;
            dlg1.Filter = "CsGeneralAudio files (*.baud)|*.baud";
            if (!(dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK && dlg1.FileName.Length > 0))
                return;
            loadFromBaud(dlg1.FileName);
        }
        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CsGeneralAudio files (*.baud)|*.baud";
            saveFileDialog1.RestoreDirectory = true;
            if (!(saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog1.FileName.Length > 0))
                return;
            saveToBaud(saveFileDialog1.FileName);
        }
        private void mnuFileNew_Click(object sender, EventArgs e)
        {
            string sFilename=null;
            if (File.Exists(strInitialDir + "default.baud"))
                sFilename = Path.GetFullPath(strInitialDir+ "default.baud");
            else if (File.Exists(strInitialDir+"..\\..\\..\\default.baud"))
                sFilename = Path.GetFullPath(strInitialDir+"..\\..\\..\\default.baud");

            if (sFilename!=null && File.Exists(sFilename))
                loadFromBaud(sFilename);
        }
        
        
        
        private void mnuFileExit_Click(object sender, EventArgs e) { Close(); }
        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("AudioNotespad\r\nBy Ben Fisher, 2010.\r\n\r\n\r\n\r.");
        }


        private void setSliderToValue(int i)
        {
            double v=this.paramValues[i]; TrackBar tb=this.tbParamTrackBars[i]; Label lbl=this.lblParamLabels[i];
            lbl.Text = v.ToString("0.####");

            int nVal;
            if (i==0 || i==1) 
                nVal = (int)(tb.Maximum*(v/paramRange));
            else
                nVal = (int)(tb.Maximum*(0.5*v+0.5));
            nVal = Math.Min(tb.Maximum, Math.Max(tb.Minimum, nVal)); //if beyond bounds, push to edge.
            tb.Value = nVal;
        }

        private bool manSetValue(int i)
        {
            Label lbl=this.lblParamLabels[i]; TrackBar tb=this.tbParamTrackBars[i];
            double current; if (!double.TryParse(lbl.Text, out current)) current=0.0;
            double v=0.0;
            if (!InputBoxForm.GetDouble("Value:", current, out v))
                return false;
            paramValues[i] = v;
            setSliderToValue(i);
            return true;
        }
        private void lblParam1_Click(object sender, EventArgs e)
        {
            manSetValue(0);
        }
        private void lblParam2_Click(object sender, EventArgs e)
        {
            manSetValue(1);
        }
        private void lblParam3_Click(object sender, EventArgs e)
        {
            manSetValue(2);
        }
        private void lblParam4_Click(object sender, EventArgs e)
        {
            manSetValue(3);
        }

        private void mnuAdvSetParamRange_Click(object sender, EventArgs e)
        {
            double v;
            double defaultRange=this.paramRange;
            if (!InputBoxForm.GetDouble("The trackbars allow to be set to a value between 0 and a. Choose value of a:", defaultRange, out v))
                return;

            this.paramRange = v; //so that 2.0 becomes range of 4
            setSliderToValue(0);
            setSliderToValue(1);
            setSliderToValue(2);
            setSliderToValue(3);
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length>0)
                loadFromBaud(files[0]);
        }
        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

      

        private void btnHearResults_Click(object sender, EventArgs e)
        {
            this.aplayer.Play(PATH_BACKEND_WAV, true);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.aplayer.Stop();
        }

    

        private void mnuRunPlay_Click(object sender, EventArgs e) {btnHearResults_Click(null,null); }
        private void mnuRunStop_Click(object sender, EventArgs e) {btnStop_Click(null, null); }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\Program Files\Audacity 1.3 Beta (Unicode)\audacity.exe", "\"" + Path.GetFullPath(PATH_BACKEND_WAV ) + "\"");
        }
        WaveAudio curAudio=null;
        private void button3_Click(object sender, EventArgs e)
        {
            double d = 0.0;
            if (!InputBoxForm.GetDouble("Enter speed:", 1.0, out d)) return;
            WaveAudio ww = new WaveAudio(PATH_BACKEND_WAV);
            this.curAudio = Effects.ScalePitchAndDuration(ww, d);
            this.aplayer.Play(this.curAudio, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string s = this.textBox1.Text;
            if (!s.Contains(":")) s = @"..\..\..\..\..\Sourceaudioclips\" + s;
            this.aplayer.Play(s,true);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string s = this.textBox2.Text;
            if (!s.Contains(":")) s = @"..\..\..\..\..\Sourceaudioclips\" + s;
            this.aplayer.Play(s, true);
        }

        

    }
}


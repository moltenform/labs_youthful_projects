// F5 to go?

using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using CsWaveAudio;

namespace CsAudioTimeDomain
{
    public partial class FormAudioTimeDomain : Form
    {
        public readonly string Version = "0.0.1";

        private double paramRange = 1.0;
        private double[] paramValues = new double[4]; //inits to 0.0
        //string strExpresision is held in the form.


        private Label[] lblParamLabels;
        private TrackBar[] tbParamTrackBars;

        private string strMediaDirectory=@"..\..\..\..\..\Media\";
        private AudioPlayer aplayer; private WaveAudio currentWave=null;
        public FormAudioTimeDomain()
        {

            InitializeComponent();
            this.lblParamLabels = new Label[4];
            this.lblParamLabels[0]= lblParam1;
            this.lblParamLabels[1]= lblParam2;
            this.lblParamLabels[2]= lblParam3;
            this.lblParamLabels[3]= lblParam4;
            this.tbParamTrackBars = new TrackBar[] { tbParam1, tbParam2, tbParam3, tbParam4 };

            
            lblParam1.Text = lblParam2.Text =lblParam3.Text =lblParam4.Text = "0.0";
            btnHelpPlay1.Text = btnHelpPlay2.Text = btnHelpPlay3.Text = btnHelpPlay4.Text = " ";

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);

            this.scintilla1.ConfigurationManager.Language = "cs";
            this.txtExpression.Visible = false;

            aplayer = new AudioPlayer();
            mnuFileNew_Click(null, null);
        }
        public string getSrcText()
        {
            //return this.txtExpression.Text;
            return this.scintilla1.Text;
        }
        public void setSrcText(string s)
        {
            //this.txtExpression.Text = s;
            this.scintilla1.Text = s;
        }
        

       
        public void ShowHelpers()
        {
            Regex r = new Regex("\"(.+?\\.wav)\"");
            string s = getSrcText();
            //while (true)
            //{
               Match m = r.Match(s);
               if (m==null) return;//break;
                //MessageBox.Show(m.Groups[1].ToString());
            string sFilename =     m.Groups[1].ToString();
            sFilename = sFilename.Replace("/", "\\");
            btnHelpPlay1.Tag = strMediaDirectory + sFilename;
            btnHelpPlay1.Text = sFilename;//.Replace(".wav","");
            btnHelpPlay1.Text = btnHelpPlay1.Text.Split('\\')[btnHelpPlay1.Text.Split('\\').Length-1];
           // }
        }

        private void btnGo_Click(object sender, EventArgs e) {
            CodedomEvaluator.CodedomGeneral gen =  new CodedomEvaluator.CodedomGeneral("WaveAudio.dll");
            string sErr = "";

            this.currentWave = null;
            string sSource = "private WaveAudio loadWav(string sName) { return new WaveAudio(@\"" +strMediaDirectory+"\""+@"+sName.Replace('/','\\')); }";
            sSource += "\r\n double c1=" + this.paramValues[0].ToString(CultureInfo.InvariantCulture) + ";";
            sSource += "\r\n double c2=" + this.paramValues[1].ToString(CultureInfo.InvariantCulture) + ";";
            sSource += "\r\n double c3=" + this.paramValues[2].ToString(CultureInfo.InvariantCulture) + ";";
            sSource += "\r\n double c4=" + this.paramValues[3].ToString(CultureInfo.InvariantCulture) + ";";
            //MessageBox.Show(sSource);
            sSource += "\r\n"+getSrcText();
            object res = gen.evaluateGeneral(sSource, "CsWaveAudio", "WaveAudio", out sErr);
            if (sErr!="")
            {
                MessageBox.Show(sErr); return;
            }
            WaveAudio w = res as WaveAudio;
            if (w==null)
            {
                MessageBox.Show("could not convert to waveaudio"); return;
            }
            this.currentWave = w;
            ShowHelpers();
            this.btnHearResults.Focus();
        }





        private void tbParam1_Scroll(object sender, EventArgs e) { onScroll(0); }
        private void tbParam2_Scroll(object sender, EventArgs e) { onScroll(1); }
        private void tbParam3_Scroll(object sender, EventArgs e) { onScroll(2); }
        private void tbParam4_Scroll(object sender, EventArgs e) { onScroll(3); }
        private void onScroll(int i)
        {
            TrackBar tb=this.tbParamTrackBars[i]; Label lbl=this.lblParamLabels[i];
            double v = (tb.Value / ((double)tb.Maximum))*this.paramRange; 
            lbl.Text = v.ToString("0.####"); //4 decimals or fewer
            this.paramValues[i]=v;
        }


        

        private void loadIni(string sFilename)
        {
            //note: requires absolute path to file.
            if (!File.Exists(sFilename)) return;
            IniFileParsing ifParsing = new IniFileParsing(sFilename, true);
            try
            {
                CsIniLoadHelper loader = new CsIniLoadHelper(ifParsing, "main_audiotime");
                this.paramValues[0] = loader.getDouble("p1");
                this.paramValues[1] = loader.getDouble("p2");
                this.paramValues[2] = loader.getDouble("p3");
                this.paramValues[3] = loader.getDouble("p4");
                this.paramRange = loader.getDouble("paramRange");

                // Expression is split into 5 parts to allow roughly 20k of code.
                string allsrc = loader.getString("paramExpression0") + 
                    loader.getString("paramExpression1") + loader.getString("paramExpression2") +
                    loader.getString("paramExpression3") + loader.getString("paramExpression4");
                this.setSrcText(allsrc);
            }
            catch (IniFileParsingException err)
            {
                MessageBox.Show("Prefs Error:"+err.ToString());
                return;
            }

            this.currentWave = null;
            setSliderToValue(0); setSliderToValue(1); setSliderToValue(2); setSliderToValue(3);            
        }
        private void saveIni(string sFilename)
        {
            IniFileParsing ifParsing = new IniFileParsing(sFilename, false); //creates ini if doesn't exist
            try
            {
                CsIniSaveHelper saver = new CsIniSaveHelper(ifParsing, "main_audiotime"); //one section called "main_portrait"

                saver.saveDouble("p1", this.paramValues[0]);
                saver.saveDouble("p2", this.paramValues[1]);
                saver.saveDouble("p3", this.paramValues[2]);
                saver.saveDouble("p4", this.paramValues[3]);
                saver.saveDouble("paramRange", this.paramRange);
                saver.saveString("programVersion", Version);

                // Expression is split into 5 parts to allow roughly 20k of code.
                List<string> parts = new List<string>(splitTextBySize(this.getSrcText(), IniFileParsing.MAXLINELENGTH - 2));
                saver.saveString("paramExpression0", (parts.Count>0) ? parts[0]:"");
                saver.saveString("paramExpression1", (parts.Count>1) ? parts[1]:"");
                saver.saveString("paramExpression2", (parts.Count>2) ? parts[2]:"");
                saver.saveString("paramExpression3", (parts.Count>3) ? parts[3]:"");
                saver.saveString("paramExpression4", (parts.Count>4) ? parts[4]:"");
            }
            catch (IniFileParsingException err)
            {
                MessageBox.Show("Prefs Error:"+err.ToString());
                return;
            }
        }
        private IEnumerable<string> splitTextBySize(string str, int chunkSize)
        {
            for (int i = 0; i < str.Length; i += chunkSize)
                yield return str.Substring(i, Math.Min(chunkSize, str.Length-i));
        }


        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg1 = new OpenFileDialog();
            dlg1.RestoreDirectory = true;
            dlg1.Filter = "CsGeneralAudio files (*.cfg)|*.cfg";
            if (!(dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK && dlg1.FileName.Length > 0))
                return;
            loadIni(dlg1.FileName);
            
        }
        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CsGeneralAudio files (*.cfg)|*.cfg";
            saveFileDialog1.RestoreDirectory = true;
            if (!(saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog1.FileName.Length > 0))
                return;
            saveIni(saveFileDialog1.FileName);
        }

        
        private void mnuFileNew_Click(object sender, EventArgs e)
        {
            string sFilename=null;
            if (File.Exists("default.cfg"))
                sFilename = Path.GetFullPath("default.cfg");
            else if (File.Exists("..\\..\\..\\default.cfg"))
                sFilename = Path.GetFullPath("..\\..\\..\\default.cfg");

            if (sFilename!=null && File.Exists(sFilename))
                loadIni(sFilename); // requires absolute path.
        }
        
        

        
        private void mnuFileExit_Click(object sender, EventArgs e) { Close(); }
        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CsGeneralAudio\r\nBy Ben Fisher, 2010.\r\n\r\nhttp://halfhourhacks.blogspot.com\r\n\r.");
        }


        private void setSliderToValue(int i)
        {
            double v=this.paramValues[i]; TrackBar tb=this.tbParamTrackBars[i]; Label lbl=this.lblParamLabels[i];
            lbl.Text = v.ToString("0.####");

            int nVal;
            nVal = (int)(tb.Maximum*(v/paramRange));
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
            double defaultRange=1.0;
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
                loadIni(files[0]);
        }
        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void mnuFileSaveWav_Click(object sender, EventArgs e)
        {
            if (this.currentWave==null) return;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Wav files|*.wav";
            saveFileDialog1.RestoreDirectory = true;
            if (!(saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog1.FileName.Length > 0))
                return;
            this.currentWave.SaveWaveFile(saveFileDialog1.FileName, 16);
        }

        private void btnHearResults_Click(object sender, EventArgs e)
        {
            
            if (this.currentWave==null) return;
            this.aplayer.Play(this.currentWave, true);//play async
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.aplayer.Stop();
        }

        private void btnHelpPlay1_Click(object sender, EventArgs e)
        {
            try
            {
                string sFilename = (sender as Button).Tag as string;
                this.aplayer.Play(new WaveAudio(sFilename), true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't find/play that file.");
            }
        }

        private void mnuRunRun_Click(object sender, EventArgs e) { btnGo_Click(null, null); }

        private void mnuRunListen_Click(object sender, EventArgs e) {btnHearResults_Click(null,null); }
    }
}
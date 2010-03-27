//compare against rev 233
/*have a standardlooptrace for "traces"
 * note alert() and clipboardset()
 * 
 * */

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using CsBifurcation;
using System.Collections.Generic;

namespace CsGeneralBitmap
{
    public partial class FormBifurcation : Form
    {
        public readonly string Version = "0.0.1";
        public FormBifurcation()
        {
            
            if (Directory.Exists(@"..\..\cfgs")) 
                Directory.SetCurrentDirectory(@"..\..\cfgs"); //Test environment
            else if (Directory.Exists(@"cfgs"))
                Directory.SetCurrentDirectory(@"cfgs"); //Release environment
            // otherwise we can't find the cfgs folder, but we don't need to crash.

            InitializeComponent();
            lblParam1.Text = lblParam2.Text =lblParam3.Text =lblParam4.Text = lblSettling.Text = lblShading.Text = "";

            this.plotCntrl.OnAltShiftDrag += new AltShiftDragDelegate(plotCntrl_OnAltShiftDrag);

            //modify layout from previous
            this.SuspendLayout();
            this.Text = "CsGeneralBitmap";
            this.label8.Visible = this.txtP0.Visible = false; //hide "P0 =" label
            this.label9.Visible = this.txtInit.Visible = false;  //hide "init. code"
            this.mnuAdvShades.Visible = this.mnuAdvPoints.Visible = false;
            this.mnuAdvAddQuality.Visible = false;
            this.label3.Text = "Iters";
            ToolStripMenuItem mnuAdvancedRenderSize = new System.Windows.Forms.ToolStripMenuItem();
            mnuAdvancedRenderSize.Text = "Render Size...";
            mnuAdvancedRenderSize.Click += new EventHandler(mnuAdvancedRenderSize_Click);
            this.advancedToolStripMenuItem.DropDownItems.Insert(4, mnuAdvancedRenderSize);
            ToolStripMenuItem mnuLoadScratch = new System.Windows.Forms.ToolStripMenuItem();
            mnuLoadScratch.Text = "Load scratch";
            mnuLoadScratch.Click += new EventHandler(mnuLoadScratch_Click);
            mnuLoadScratch.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.advancedToolStripMenuItem.DropDownItems.Insert(0,mnuLoadScratch);
            ToolStripMenuItem mnuSaveScratch = new System.Windows.Forms.ToolStripMenuItem();
            mnuSaveScratch.Text = "Save scratch";
            mnuSaveScratch.Click += new EventHandler(mnuSaveScratch_Click);
            mnuSaveScratch.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.advancedToolStripMenuItem.DropDownItems.Insert(1,mnuSaveScratch);

            this.txtExpression.Location = new System.Drawing.Point(568-75, 151-70);
            this.txtExpression.Size = new System.Drawing.Size(209+150, 61+70);
            this.FormClosed += new FormClosedEventHandler(onFormClose);
            
            this.ResumeLayout(false);

            this.tbParamIters = tbShading; lblParamIters=lblShading; //give it another, more accurate, name
            //use normal trackbar functionality for shading and points
            this.tbParamIters.Minimum = 0; this.tbParamIters.Maximum = 10000;
            this.tbSettling.Minimum = 0; this.tbSettling.Maximum = 2000;

            mnuFileNew_Click(null, null);
        }

        private bool bAltMode = false;
        void plotCntrl_OnAltShiftDrag(double nx0, double nx1, double ny0, double ny1)
        {
            //switch modes!
            plotCntrl.param3=nx0; plotCntrl.param4=ny1;
            setSliderToValue(plotCntrl.param3, tbParam3, lblParam3);
            setSliderToValue(plotCntrl.param4, tbParam4, lblParam4);

            bAltMode = !bAltMode;
            string s = this.txtExpression.Text;
            s=s.Trim();
            //comment //$$SW indicates that we wrote this, not a human
            if (s.StartsWith("bSwitch = true;//$$SW\r\n"))
                s = s.Replace("bSwitch = true;//$$SW\r\n", "");
            else if (s.StartsWith("bSwitch = false;//$$SW\r\n"))
                s = s.Replace("bSwitch = false;//$$SW\r\n", "");

            s = "bSwitch = "+(bAltMode?"true":"false") + ";//$$SW\r\n" + s;
            this.txtExpression.Text = s;
            Redraw();
        }

        

        public void Redraw()
        {
            //System.Diagnostics.Stopwatch st=new System.Diagnostics.Stopwatch();
            //st.Start();
            this.plotCntrl.paramExpression = getUserExpression(this.txtExpression.Text);
           
            this.plotCntrl.redraw();
            //st.Stop();
            //MessageBox.Show((st.ElapsedMilliseconds / 1000.0).ToString());
        }

        //transform sin into Math.sin, rand()=R.NextDouble()
        protected string getUserExpression(string s)
        {
            CodedomEvaluator.CodedomEvaluator cb = new CodedomEvaluator.CodedomEvaluator();
            //don't add math methods.
            //s = cb.addMathMethods(s);
            //s = Regex.Replace(s, "\\brand\\(\\)\\b", "R.NextDouble()");
            //s = Regex.Replace(s, "\\brand\\b", "R.NextDouble");
            //s = Regex.Replace(s, "\\brandneg\\(\\)", "((R.NextDouble()-0.5)*2)");
            s = Regex.Replace(s, "\\balert\\b", "System.Windows.Forms.MessageBox.Show");
            s = Regex.Replace(s, "\\bclipboardset\\b", "System.Windows.Forms.Clipboard.SetText");
            return s;
        }
       


        private void btnGo_Click(object sender, EventArgs e) { Redraw(); }

        private void tbParam1_Scroll(object sender, EventArgs e)
        {
            plotCntrl.param1 = onScroll(tbParam1, lblParam1);
            if (mnuAdvAutoRedraw.Checked)
                plotCntrl.redraw();
        }
        private void tbParam2_Scroll(object sender, EventArgs e)
        {
            plotCntrl.param2 = onScroll(tbParam2, lblParam2);
            if (mnuAdvAutoRedraw.Checked)
                plotCntrl.redraw();
        }
        

        private double paramRange = 4.0; //by default from -2.0 to 2.0
        private void tbSettling_Scroll(object sender, EventArgs e) { plotCntrl.paramSettle = (int)onScroll(tbSettling, lblSettling); }
        private void tbParamIters_Scroll(object sender, EventArgs e) { plotCntrl.paramIters = (int)onScroll(tbParamIters, lblParamIters); }
        private void tbParam3_Scroll(object sender, EventArgs e) { plotCntrl.param3 = onScroll(tbParam3, lblParam3); }
        private void tbParam4_Scroll(object sender, EventArgs e) { plotCntrl.param4 = onScroll(tbParam4, lblParam4); }
        private double onScroll(TrackBar tb, Label lbl)
        {
            double v = (tb.Value / ((double)tb.Maximum)); // from 0.0 to 1.0
            if (tb==tbParam1 || tb==tbParam2)
                v = (v-0.5)*paramRange; //by default from -1.0 to 1.0
            else if (tb==tbSettling || tb==tbParamIters) 
                v = tb.Value; //use normal trackbar, no scaling

            lbl.Text = v.ToString("0.####"); //4 decimals or fewer
            return v;
        }


        private void getBoundsManually()
        {
            double X0, X1, Y0, Y1, nX0, nX1, nY0, nY1;
            plotCntrl.getBounds(out X0, out X1, out Y0, out Y1);

            if (!InputBoxForm.GetDouble("Leftmost x", X0, out nX0)) return;
            if (!InputBoxForm.GetDouble("Rightmost x", X1, out nX1)) return;
            if (!InputBoxForm.GetDouble("Lowest y", Y0, out nY0)) return;
            if (!InputBoxForm.GetDouble("Greatest y", Y1, out nY1)) return;

            if (!(nY1 > nY0 && nX1 > nX0)) { MessageBox.Show("Invalid bounds."); return; }

            this.plotCntrl.setBounds(nX0, nX1, nY0, nY1);
            this.plotCntrl.redraw();
        }





        private void loadIni(string sFilename)
        {
            //note: requires absolute path to file.
            if (!File.Exists(sFilename)) return;
            IniFileParsing ifParsing = new IniFileParsing(sFilename, true);
            try
            {
                CsIniLoadHelper loader = new CsIniLoadHelper(ifParsing, "main_general");
                double nX0 = loader.getDouble("X0");
                double nX1 = loader.getDouble("X1");
                double nY0 = loader.getDouble("Y0");
                double nY1 = loader.getDouble("Y1");
                plotCntrl.setBounds(nX0, nX1, nY0, nY1);
                this.paramRange = loader.getDouble("paramRange");
                plotCntrl.param1 = loader.getDouble("param1");
                plotCntrl.param2 = loader.getDouble("param2");
                plotCntrl.param3 = loader.getDouble("param3", true);
                plotCntrl.param4 = loader.getDouble("param4", true);
                plotCntrl.paramSettle = loader.getInt("paramSettle");
                plotCntrl.paramIters = loader.getInt("paramIters");

                // Expression is split into 5 parts to allow roughly 20k of code.
                txtExpression.Text = loader.getString("paramExpression0") + 
                    loader.getString("paramExpression1") + loader.getString("paramExpression2") +
                    loader.getString("paramExpression3") + loader.getString("paramExpression4");
            }
            catch (IniFileParsingException err)
            {
                MessageBox.Show("Prefs Error:"+err.ToString());
                return;
            }

            //set ui
            double p1 = plotCntrl.param1; double p2 = plotCntrl.param2;
            double p3 = plotCntrl.param3; double p4 = plotCntrl.param4;
            int npoints = plotCntrl.paramIters; int nsettle = plotCntrl.paramSettle;

            setSliderToValue(npoints, tbParamIters, lblParamIters);
            setSliderToValue(nsettle, tbSettling, lblSettling);
            setSliderToValue(p1, tbParam1, lblParam1);
            setSliderToValue(p2, tbParam2, lblParam2);
            setSliderToValue(p3, tbParam3, lblParam3);
            setSliderToValue(p4, tbParam4, lblParam4);

            mnuSaveScratch_Click(null, null); //save into scratch file
            bAltMode = false;
            Redraw();
        }
        private void saveIni(string sFilename)
        {
            IniFileParsing ifParsing = new IniFileParsing(sFilename, false); //creates ini if doesn't exist
            try
            {
                CsIniSaveHelper saver = new CsIniSaveHelper(ifParsing, "main_general"); //one section called "main_portrait"

                double nX0, nX1, nY0, nY1;
                plotCntrl.getBounds(out nX0, out nX1, out nY0, out nY1);
                saver.saveDouble("X0", nX0);
                saver.saveDouble("X1", nX1);
                saver.saveDouble("Y0", nY0);
                saver.saveDouble("Y1", nY1);
                saver.saveDouble("paramRange", this.paramRange);
                saver.saveDouble("param1", plotCntrl.param1);
                saver.saveDouble("param2", plotCntrl.param2);
                saver.saveDouble("param3", plotCntrl.param3);
                saver.saveDouble("param4", plotCntrl.param4);
                saver.saveInt("paramSettle", plotCntrl.paramSettle);
                saver.saveInt("paramIters", plotCntrl.paramIters);
                
                saver.saveString("programVersion", Version);

                // Expression is split into 5 parts to allow roughly 20k of code.
                List<string> parts = new List<string>(splitTextBySize(txtExpression.Text, CsBifurcation.IniFileParsing.MAXLINELENGTH - 2));
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
            dlg1.Filter = "CsBifurcation files (*.cfg)|*.cfg";
            if (!(dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK && dlg1.FileName.Length > 0))
                return;
            loadIni(dlg1.FileName);
        }
        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CsBifurcation files (*.cfg)|*.cfg";
            saveFileDialog1.RestoreDirectory = true;
            if (!(saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog1.FileName.Length > 0))
                return;
            saveIni(saveFileDialog1.FileName);
        }

        private void mnuFileAnimate_Click(object sender, EventArgs e)
        {
            double c0_0, c0_1, c1_0, c1_1; int nframes;
            double param1=plotCntrl.param1, param2=plotCntrl.param2;

            if (!InputBoxForm.GetDouble("Initial c1:", param1, out c0_0)) return;
            if (!InputBoxForm.GetDouble("Final c1:", param1, out c0_1)) return;
            if (!InputBoxForm.GetDouble("Initial c2:", param2, out c1_0)) return;
            if (!InputBoxForm.GetDouble("Final c2:", param2, out c1_1)) return;
            if (!InputBoxForm.GetInt("Number of frames:", 50, out nframes)) return;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "png files (*.png)|*.png";
            saveFileDialog1.RestoreDirectory = true;
            if (!(saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog1.FileName.Length > 0))
                return;
            string sfilename= saveFileDialog1.FileName;
            double c0_inc = (c0_1-c0_0)/nframes;
            double c1_inc = (c1_1-c1_0)/nframes;

            for (int i=0; i<nframes; i++)
            {
                plotCntrl.param1 = c0_0;
                plotCntrl.param2 = c1_0;
                plotCntrl.renderToDiskSave(400, 400, sfilename.Replace(".png", "_"+i.ToString("000")+".png"));
                c0_0 += c0_inc;
                c1_0 += c1_inc;
            }
        }
        private void mnuFileNew_Click(object sender, EventArgs e)
        {
            string sFilename = Path.GetFullPath(Directory.GetCurrentDirectory()) + "\\default.cfg";
            if (File.Exists(sFilename))
                loadIni(sFilename); // requires absolute path.
            else
                Redraw();
            this.plotCntrl.clearUndo();
        }
        private int nRenderWidth = 3200, nRenderHeight=3200;
        private void mnuAdvancedRenderSize_Click(object sender, EventArgs e)
        {
            int width, height;
            if (!InputBoxForm.GetInt("Render Width:", nRenderWidth, out width)) return;
            if (!InputBoxForm.GetInt("Render Height:", nRenderHeight, out height)) return;
            if (width>0 && height>0) { nRenderHeight=height; nRenderWidth=width; }
        }
        private void mnuFileRender_Click(object sender, EventArgs e)
        {
            try
            {
                plotCntrl.bIsRendering = true;
                plotCntrl.renderToDisk(nRenderWidth, nRenderHeight);
            }
            finally { plotCntrl.bIsRendering = false; }
        }

        private void mnuAdvShades_Click(object sender, EventArgs e) { mnuAdvShades.Checked = true; mnuAdvPoints.Checked=false; Redraw(); }
        private void mnuAdvPoints_Click(object sender, EventArgs e) { mnuAdvShades.Checked = false; mnuAdvPoints.Checked=true; Redraw(); }
        private void mnuViewRedraw_Click(object sender, EventArgs e) { Redraw(); }
        private void mnuAdvAutoRedraw_Click(object sender, EventArgs e) { mnuAdvAutoRedraw.Checked=!mnuAdvAutoRedraw.Checked; }
        private void mnuFileExit_Click(object sender, EventArgs e) { Close(); }
        private void mnuViewZoomIn_Click(object sender, EventArgs e) { plotCntrl.zoomIn(); }
        private void mnuViewZoomOut_Click(object sender, EventArgs e) { plotCntrl.zoomOut(); }
        private void mnuViewZoomUndo_Click(object sender, EventArgs e) { plotCntrl.undoZoom(); }
        private void mnuViewReset_Click(object sender, EventArgs e) { plotCntrl.resetZoom(); }

        private void mnuAdvBounds_Click(object sender, EventArgs e) { getBoundsManually(); }
        private void mnuAdvAddQuality_Click(object sender, EventArgs e) { MessageBox.Show("To make image darker, add a line to \"init. code\" such as: \r\n\r\nadditionalDarkening=0.5;"); }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CsGeneralBitmap\r\nBy Ben Fisher, 2010.\r\n\r\nhttp://halfhourhacks.blogspot.com\r\n\r\nRefer to readme.txt to discover additional features.");
        }


        private void setSliderToValue(double v, TrackBar tb, Label lbl)
        {
            lbl.Text = v.ToString("0.####");

            int nVal;
            if (tb==tbSettling || tb==tbParamIters)
                nVal = (int) v; 
            else if (tb==tbParam1 || tb==tbParam2)
                nVal = (int)(tb.Maximum*(v/paramRange + 0.5));
            else
                nVal = (int)(tb.Maximum*v);
            nVal = Math.Min(tb.Maximum, Math.Max(tb.Minimum, nVal)); //if beyond bounds, push to edge.
            tb.Value = nVal;
        }

        private bool manSetValue(Label lbl, TrackBar tb, out double v)
        {
            double current; if (!double.TryParse(lbl.Text, out current)) current=0.0;
            v=0.0;
            if (!InputBoxForm.GetDouble("Value:", current, out v))
                return false;
            setSliderToValue(v, tb, lbl);
            return true;
        }
        private void lblSettling_Click(object sender, EventArgs e)
        {
            double v; if (manSetValue(lblSettling, tbSettling, out v)) { plotCntrl.paramSettle = (int)v; Redraw(); }
        }
        private void lblTotalPoints_Click(object sender, EventArgs e)
        {
            double v; if (manSetValue(lblParamIters, tbParamIters, out v)) { plotCntrl.paramIters = (int)v; Redraw(); }
        }
        private void lblParam1_Click(object sender, EventArgs e)
        {
            double v; if (manSetValue(lblParam1, tbParam1, out v)) { plotCntrl.param1 = v; Redraw(); }
        }
        private void lblParam2_Click(object sender, EventArgs e)
        {
            double v; if (manSetValue(lblParam2, tbParam2, out v)) { plotCntrl.param2 = v; Redraw(); }
        }
        private void lblParam3_Click(object sender, EventArgs e)
        {
            double v; if (manSetValue(lblParam3, tbParam3, out v)) { plotCntrl.param3 = v; Redraw(); }
        }
        private void lblParam4_Click(object sender, EventArgs e)
        {
            double v; if (manSetValue(lblParam4, tbParam4, out v)) { plotCntrl.param4 = v; Redraw(); }
        }

        private void mnuAdvSetParamRange_Click(object sender, EventArgs e)
        {
            double v;
            double defaultRange=2.0;
            if (!InputBoxForm.GetDouble("The trackbars allow c1 to be set to a value between -a to a. Choose value of a:", defaultRange, out v))
                return;

            this.paramRange = v*2.0; //so that 2.0 becomes range of 4
            setSliderToValue(plotCntrl.param1, tbParam1, lblParam1);
            setSliderToValue(plotCntrl.param2, tbParam2, lblParam2);
        }

        string sScratchFileTemplate = @"..\ScratchClass_template.cs"; //note .. since we're in "cfgs" dir
        string sScratchFile = @"..\ScratchClass.cs";
        string sScratchFileTemplateData = null;
        void mnuSaveScratch_Click(object sender, EventArgs e)
        {
            if (sScratchFileTemplateData==null)
            {
                using (TextReader tr = new StreamReader(sScratchFileTemplate))
                    sScratchFileTemplateData = tr.ReadToEnd();
            }

            using (TextWriter tw = new StreamWriter(sScratchFile))
                tw.Write(sScratchFileTemplateData.Replace("$$CODE$$", this.txtExpression.Text));
        }
        void mnuLoadScratch_Click(object sender, EventArgs e)
        {
            string s;
            using (TextReader tr = new StreamReader(sScratchFile))
                s = tr.ReadToEnd();
            string[] sections = s.Split(new string[] { "//$$SECTION$$" }, StringSplitOptions.None);
            System.Diagnostics.Debug.Assert(sections.Length == 3);
            //TODO: do something more intelligent than Trim()?
            this.txtExpression.Text = sections[1].Trim();
        }
        private void onFormClose(object sender, FormClosedEventArgs e)
        {
            //revert scratch file. Basically so that it's not dirty in svn.
            if (sScratchFileTemplateData!=null)
                using (TextWriter tw = new StreamWriter(sScratchFile))
                    tw.Write(sScratchFileTemplateData.Replace("$$CODE$$", ""));
        }


        //in this program, the "shading" trackbar is actually the "iters" trackbar.
        private void tbShading_Scroll(object sender, EventArgs e) { tbParamIters_Scroll(sender, e); }
        private void lblShading_Click(object sender, EventArgs e) { lblTotalPoints_Click(sender, e); }
        private TrackBar tbParamIters;
        private Label lblParamIters;

    }
}
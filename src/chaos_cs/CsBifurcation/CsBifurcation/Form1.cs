/*
 * only concern: if in program files, won't have write access, need to set up in docs
 * See mouse shortcuts in plotusercontrol.cs
 * Press ctrl+space to redraw
 * Enter arbitrary code in P0, such as C1, rand(), or randneg(). In init. code: alert() and clipboardset()
 * Change additionalShading in init code for higher quality.
 * Press left/right and pgup/pgdn to make small changes to C1, C2
 * click label to manually set value
 * 
 * 
 * version 217 was a large change.
 * todo: threading, previews, zoom animations, nudge view left/right?
 * todo: clicking on plot to zoom in should incorporate textbox/slider changes? Eliminate view menu, plotcontrol to have only public methods no ui?
 * in the future one could have a seperate "render mode", with different AdditionalShading, but for now can type in comment in init. code
 * 
 * note that you can't drag/drop files from a non-elevated program to an elevated program.
 * */

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace CsBifurcation
{
    public partial class Form1 : Form
    {
        public readonly string Version = "1.0.0";
        public Form1()
        {
            if (Directory.Exists(@"..\..\cfgs")) 
                Directory.SetCurrentDirectory(@"..\..\cfgs"); //Test environment
            else if (Directory.Exists(@"cfgs"))
                Directory.SetCurrentDirectory(@"cfgs"); //Release environment
            // otherwise we can't find the cfgs folder, but we don't need to crash.

            InitializeComponent();
            lblParam1.Text = lblParam2.Text =lblParam3.Text =lblParam4.Text = lblSettling.Text = lblShading.Text = "";
            mnuAdvShades.Checked = true; mnuAdvPoints.Checked = false;

            ToolStripMenuItem mnuAdvancedRenderSize = new System.Windows.Forms.ToolStripMenuItem();
            mnuAdvancedRenderSize.Text = "Render Size...";
            mnuAdvancedRenderSize.Click += new EventHandler(mnuAdvancedRenderSize_Click);
            this.advancedToolStripMenuItem.DropDownItems.Insert(4, mnuAdvancedRenderSize);

            ToolStripMenuItem mnuFileCopy = new System.Windows.Forms.ToolStripMenuItem();
            mnuFileCopy.Text = "Copy Image";
            mnuFileCopy.Click += new EventHandler(mnuFileCopyImage);
            mnuFileCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control|System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.C)));
            this.fileToolStripMenuItem.DropDownItems.Insert(4, mnuFileCopy);
            this.mnuViewZoomUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control|System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.Z)));

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);

            mnuFileNew_Click(null, null);
        }

        

        public void Redraw()
        {
            this.plotCntrl.paramExpression = getUserExpression(this.txtExpression.Text);
            this.plotCntrl.paramP0 = getUserExpression(this.txtP0.Text);
            this.plotCntrl.paramInit = getUserExpression(this.txtInit.Text);
            this.plotCntrl.bShading = this.mnuAdvShades.Checked;

            this.plotCntrl.redraw();
        }

        //transform sin into Math.sin, rand()=R.NextDouble()
        protected string getUserExpression(string s)
        {
            CodedomEvaluator.CodedomEvaluator cb = new CodedomEvaluator.CodedomEvaluator();
            s = cb.addMathMethods(s);
            //s = Regex.Replace(s, "\\brand\\(\\)\\b", "R.NextDouble()");
            s = Regex.Replace(s, "\\brand\\b", "R.NextDouble");
            s = Regex.Replace(s, "\\brandneg\\(\\)", "((R.NextDouble()-0.5)*2)");
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
        private double paramRange = 2.0; //by default from -1.0 to 1.0
        private void tbSettling_Scroll(object sender, EventArgs e) { plotCntrl.paramSettle = (int)onScroll(tbSettling, lblSettling); }
        private void tbShading_Scroll(object sender, EventArgs e) { plotCntrl.paramShading = onScroll(tbShading, lblShading); }
        private void tbParam2_Scroll(object sender, EventArgs e) { plotCntrl.param2 = onScroll(tbParam2, lblParam2); }
        private void tbParam3_Scroll(object sender, EventArgs e) { plotCntrl.param3 = onScroll(tbParam3, lblParam3); }
        private void tbParam4_Scroll(object sender, EventArgs e) { plotCntrl.param4 = onScroll(tbParam4, lblParam4); }
        private double onScroll(TrackBar tb, Label lbl)
        {
            double v = (tb.Value / ((double)tb.Maximum)); // from 0.0 to 1.0
            if (tb==tbParam1 || tb==tbParam2)
                v = (v-0.5)*paramRange; //by default from -1.0 to 1.0
            else if (tb==tbSettling)
                v = (int) (Math.Pow(8.0, v * 6));

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
                CsIniLoadHelper loader = new CsIniLoadHelper(ifParsing, "main");
                double nX0 = loader.getDouble("X0");
                double nX1 = loader.getDouble("X1");
                double nY0 = loader.getDouble("Y0");
                double nY1 = loader.getDouble("Y1");
                plotCntrl.setBounds(nX0, nX1, nY0, nY1);
                plotCntrl.param1 = loader.getDouble("param1");
                plotCntrl.param2 = loader.getDouble("param2");
                plotCntrl.param3 = loader.getDouble("param3", true);
                plotCntrl.param4 = loader.getDouble("param4", true);
                plotCntrl.paramSettle = loader.getInt("paramSettle");
                plotCntrl.paramShading = loader.getDouble("paramShading");

                //these are transformed, so set the ui instead of the prop. Call to Redraw will retrieve this.
                mnuAdvShades.Checked = loader.getInt("bShading")!=0;
                mnuAdvPoints.Checked = loader.getInt("bShading")==0;
                txtExpression.Text = loader.getString("paramExpression");
                txtP0.Text = loader.getString("paramP0");
                txtInit.Text = loader.getString("paramInit");

            }
            catch (IniFileParsingException err)
            {
                MessageBox.Show("Prefs Error:"+err.ToString());
                return;
            }

            //set ui
            double p1 = plotCntrl.param1; double p2 = plotCntrl.param2;
            double p3 = plotCntrl.param3; double p4 = plotCntrl.param4;
            double ps = plotCntrl.paramShading; double pst = plotCntrl.paramSettle;
            lblParam1.Text = p1.ToString();
            lblParam2.Text = p2.ToString();
            lblSettling.Text = pst.ToString();
            lblShading.Text = ps.ToString();
            
            setSliderToValue(ps, tbShading);
            setSliderToValue(p1, tbParam1);
            setSliderToValue(p2, tbParam2);
            setSliderToValue(p3, tbParam3);
            setSliderToValue(p4, tbParam4);

            Redraw();
        }
        private void saveIni(string sFilename)
        {
            IniFileParsing ifParsing = new IniFileParsing(sFilename, false); //creates ini if doesn't exist
            try
            {
                CsIniSaveHelper saver = new CsIniSaveHelper(ifParsing, "main"); //one section called "main"

                double nX0, nX1, nY0, nY1;
                plotCntrl.getBounds(out nX0, out nX1, out nY0, out nY1);
                saver.saveDouble("X0", nX0);
                saver.saveDouble("X1", nX1);
                saver.saveDouble("Y0", nY0);
                saver.saveDouble("Y1", nY1);
                saver.saveDouble("param1", plotCntrl.param1);
                saver.saveDouble("param2", plotCntrl.param2);
                saver.saveDouble("param3", plotCntrl.param3);
                saver.saveDouble("param4", plotCntrl.param4);
                saver.saveInt("paramSettle", plotCntrl.paramSettle);
                saver.saveDouble("paramShading", plotCntrl.paramShading);
                saver.saveString("paramExpression", txtExpression.Text);
                saver.saveString("paramInit", txtInit.Text);
                saver.saveString("paramP0", txtP0.Text);
                saver.saveInt("bShading", mnuAdvShades.Checked?1:0);
                saver.saveString("programVersion", Version);
            }
            catch (IniFileParsingException err)
            {
                MessageBox.Show("Prefs Error:"+err.ToString());
                return;
            }
        }

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg1 = new OpenFileDialog();
            dlg1.RestoreDirectory = true;
            dlg1.Filter = "CsBifurcation files (*.cfg)|*.cfg";
            if (!(dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK && dlg1.FileName.Length > 0))
                return;
            loadIni(dlg1.FileName);
            this.plotCntrl.clearUndo();
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
            int width,height;
            if (!InputBoxForm.GetInt("Render Width:", nRenderWidth, out width)) return;
            if (!InputBoxForm.GetInt("Render Height:", nRenderHeight, out height)) return;
            if (width>0 && height>0) { nRenderHeight=height; nRenderWidth=width; }
        }
        private void mnuFileCopyImage(object sender, EventArgs e) { this.plotCntrl.copyImage(); }
        private void mnuFileRender_Click(object sender, EventArgs e)
        {
            //assume rendering at (3200x3200).
            //time taken should increase by factor of 16.
            //width increases by factor of 4 already.
            //because 4 times taller, draw 4 times as many points to attempt maintain the way it looks
            try
            {
                plotCntrl.paramAdditionalIters = nRenderHeight/((double)400); //by default, 4.0;
                plotCntrl.renderToDisk(nRenderWidth, nRenderHeight);
            }
            finally { plotCntrl.paramAdditionalIters = 1.0; }
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
        private void mnuAdvAddQuality_Click(object sender, EventArgs e) { MessageBox.Show("To run more iterations, producing better images, add a line to \"init. code\" such as: \r\n\r\nadditionalShading=2.0;"); }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CsBifurcation\r\nBy Ben Fisher, 2010.\r\n\r\nhttp://halfhourhacks.blogspot.com\r\n\r\nRefer to readme.txt to discover additional features.");
        }


        private void setSliderToValue(double v, TrackBar tb)
        {
            int nVal;
            if (tb==tbSettling)
                nVal = tb.Value; //don't feel like setting this, would have to log it. TODO: have it set this.
            if (tb==tbParam1 || tb==tbParam2)
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
            setSliderToValue(v, tb);
            lbl.Text = v.ToString();
            return true;
        }
        private void lblSettling_Click(object sender, EventArgs e)
        {
            double v; if (manSetValue(lblSettling, tbSettling, out v)) { plotCntrl.paramSettle = (int)v; Redraw(); }
        }
        private void lblShading_Click(object sender, EventArgs e)
        {
            double v; if (manSetValue(lblShading, tbShading, out v)) { plotCntrl.paramShading = v; Redraw(); }
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
            if (!InputBoxForm.GetDouble("The trackbars allow c1 to be set to a value between -a to a. Choose value of a:", 1.0, out v))
                 return;
               
            this.paramRange = v*2.0; //so that 1.0 becomes range of 2
            setSliderToValue(plotCntrl.param1, tbParam1);
            setSliderToValue(plotCntrl.param2, tbParam2);
        }

        // for a while I considered allowing "loop preface" code that would be
        // inserted inside of the for loop, for each point (but not iterated). This would allow initial conditions common
        // to each point, for example to draw for 2d maps like the henon map.
        // This could be in the Advanced menu, say, with a menu item that would be checked if the value was set.
        // However, this isn't needed- one can take advantage of the "P0" box to insert additional code. See henon.cfg.

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
    }
}
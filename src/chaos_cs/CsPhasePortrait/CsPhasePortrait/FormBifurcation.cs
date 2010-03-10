//compare against rev 233
/*
 * problem: rendered pictures are too faint.
 * this is because the output image is much sparser, shades accumulate on normal but not on render.
 * one solution- increase nXpoints, not nIters!
 * todo: use reflection to assign inside the class. not allocate 400x400 every time!!
reflection to set this!
 * 
 * todo: fix rendering! increase nXpoints
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

namespace CsPhasePortrait
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

            //modify layout from previous
            this.SuspendLayout();
            this.Text = "CsPhasePortrait";
            this.label8.Visible = false; //hide "P0 =" label
            this.txtP0.Visible = false;
            this.label9.Location = new System.Drawing.Point(693 - 100, 483 - 50); //move "init. code" label to left
            this.txtInit.Location = new System.Drawing.Point(749 - 100, 481 - 50); // move "txtInit" to left
            this.txtInit.Size = new System.Drawing.Size(147 + 100, 43 + 50);
            this.mnuAdvShades.Visible = this.mnuAdvPoints.Visible = false;
            this.mnuAdvAddQuality.Text = "Darkness...";
            this.label3.Text = "Points (k)";
            this.ResumeLayout(false);

            this.tbTotalPoints = tbShading; lblTotalPoints=lblShading; //give it another, more accurate, name
            //use normal trackbar functionality for shading and points
            this.tbTotalPoints.Minimum = 0; this.tbTotalPoints.Maximum = 10000;
            this.tbSettling.Minimum = 0; this.tbSettling.Maximum = 2000;
            this.plotCntrl.OnAltShiftDrag += new AltShiftDragDelegate(plotCntrl_OnAltShiftDrag);

            mnuFileNew_Click(null, null);
        }

        void plotCntrl_OnAltShiftDrag(double nx0, double nx1, double ny0, double ny1)
        {
            string s = "" +
                "sx0=" + nx0.ToString(CultureInfo.InvariantCulture) + ";\r\n"+
                "sx1=" + nx1.ToString(CultureInfo.InvariantCulture) + ";\r\n"+
                "sy0=" + ny0.ToString(CultureInfo.InvariantCulture) + ";\r\n"+
                "sy1=" + ny1.ToString(CultureInfo.InvariantCulture) + ";\r\n";
            this.txtInit.Text = s;
            Redraw();
        }

        public void Redraw()
        {
            this.plotCntrl.paramExpression = getUserExpression(this.txtExpression.Text);
            this.plotCntrl.paramInit = getUserExpression(this.txtInit.Text);

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
        private void tbTotalPoints_Scroll(object sender, EventArgs e) { plotCntrl.paramTotalIters = (int)onScroll(tbTotalPoints, lblTotalPoints); }
        private void tbParam3_Scroll(object sender, EventArgs e) { plotCntrl.param3 = onScroll(tbParam3, lblParam3); }
        private void tbParam4_Scroll(object sender, EventArgs e) { plotCntrl.param4 = onScroll(tbParam4, lblParam4); }
        private double onScroll(TrackBar tb, Label lbl)
        {
            double v = (tb.Value / ((double)tb.Maximum)); // from 0.0 to 1.0
            if (tb==tbParam1 || tb==tbParam2)
                v = (v-0.5)*paramRange; //by default from -1.0 to 1.0
            else if (tb==tbSettling || tb==tbTotalPoints) 
                v = tb.Value; //use normal trackbar, no scaling

            lbl.Text = v.ToString("0.####"); //4 decimals or fewer
            return v;
        }

        
        private void getBoundsManually()
        {
            double X0, X1, Y0, Y1, nX0, nX1, nY0, nY1;
            plotCntrl.getBounds(out X0, out X1, out Y0, out Y1);

            if (!_getBounds("Leftmost x", X0, out nX0)) return;
            if (!_getBounds("Rightmost x", X1, out nX1)) return;
            if (!_getBounds("Lowest y", Y0, out nY0)) return;
            if (!_getBounds("Greatest y", Y1, out nY1)) return;

            if (!(nY1 > nY0 && nX1 > nX0)) { MessageBox.Show("Invalid bounds."); return; }

            this.plotCntrl.setBounds(nX0, nX1, nY0, nY1);
            this.plotCntrl.redraw();
        }
        private bool _getBounds(string sName, double fCurrent, out double dOut)
        {
            dOut = 0.0;
            string s = InputBoxForm.GetStrInput(sName, fCurrent.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="") return false;
            return double.TryParse(s, out dOut);
        }





        private void loadIni(string sFilename)
        {
            //note: requires absolute path to file.
            if (!File.Exists(sFilename)) return;
            IniFileParsing ifParsing = new IniFileParsing(sFilename, true);
            try
            {
                CsIniLoadHelper loader = new CsIniLoadHelper(ifParsing, "main_portrait");
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
                plotCntrl.paramTotalIters = loader.getInt("paramTotalIters");

                //these are transformed, so set the ui instead of the prop. Call to Redraw will retrieve this.
                txtExpression.Text = loader.getString("paramExpression");
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
            int npoints = plotCntrl.paramTotalIters; int nsettle = plotCntrl.paramSettle;

            setSliderToValue(npoints, tbTotalPoints, lblTotalPoints);
            setSliderToValue(nsettle, tbSettling, lblSettling);
            setSliderToValue(p1, tbParam1, lblParam1);
            setSliderToValue(p2, tbParam2, lblParam2);
            setSliderToValue(p3, tbParam3, lblParam3);
            setSliderToValue(p4, tbParam4, lblParam4);

            Redraw();
        }
        private void saveIni(string sFilename)
        {
            IniFileParsing ifParsing = new IniFileParsing(sFilename, false); //creates ini if doesn't exist
            try
            {
                CsIniSaveHelper saver = new CsIniSaveHelper(ifParsing, "main_portrait"); //one section called "main_portrait"

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
                saver.saveInt("paramTotalIters", plotCntrl.paramTotalIters);
                saver.saveString("paramExpression", txtExpression.Text);
                saver.saveString("paramInit", txtInit.Text);
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
            double d, c0_0, c0_1, c1_0, c1_1; string s; int nframes;
            double param1=plotCntrl.param1, param2=plotCntrl.param2;
            s = InputBoxForm.GetStrInput("Initial c1:", param1.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="" || !double.TryParse(s, out d)) return;
            c0_0=d;
            s = InputBoxForm.GetStrInput("Final c1:", param1.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="" || !double.TryParse(s, out d)) return;
            c0_1=d;
            s = InputBoxForm.GetStrInput("Initial c2:", param2.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="" || !double.TryParse(s, out d)) return;
            c1_0=d;
            s = InputBoxForm.GetStrInput("Final c2:", param2.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="" || !double.TryParse(s, out d)) return;
            c1_1=d;
            s = InputBoxForm.GetStrInput("Number of frames:", "50");
            if (s==null || s=="" || !int.TryParse(s, out nframes)) return;
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
        }
        private void mnuFileRender_Click(object sender, EventArgs e)
        {
            //create a plot 2 times larger. (1600x1600)
            //time taken should increase by factor of 4.
            // so draw 4 times more points to maintain approximately the same density.
            try
            {
                plotCntrl.paramAdditionalIters = 16;
                plotCntrl.renderToDisk(1600, 1600);
            }
            finally { plotCntrl.paramAdditionalIters = 1; }
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
            MessageBox.Show("CsPhasePortrait\r\nBy Ben Fisher, 2010.\r\n\r\nhttp://halfhourhacks.blogspot.com\r\n\r\nRefer to readme.txt to discover additional features.");
        }


        private void setSliderToValue(double v, TrackBar tb, Label lbl)
        {
            lbl.Text = v.ToString("0.####");

            int nVal;
            if (tb==tbSettling || tb==tbTotalPoints)
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
            v=0.0;
            string s = InputBoxForm.GetStrInput("Value:", lbl.Text);
            if (s==null||s==""||!double.TryParse(s, out v)) 
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
            double v; if (manSetValue(lblTotalPoints, tbTotalPoints, out v)) { plotCntrl.paramTotalIters = (int)v; Redraw(); }
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
            string s = InputBoxForm.GetStrInput("The trackbars allow c1 to be set to a value between -a to a. Choose value of a:", "2.0");
            if (s==null||s==""||!double.TryParse(s, out v))
                return;
            this.paramRange = v*2.0; //so that 1.0 becomes range of 2
            setSliderToValue(plotCntrl.param1, tbParam1, lblParam1);
            setSliderToValue(plotCntrl.param2, tbParam2, lblParam2);
        }

        // for a while I considered allowing "loop preface" code that would be
        // inserted inside of the for loop, for each point (but not iterated). This would allow initial conditions common
        // to each point, for example to draw for 2d maps like the henon map.
        // This could be in the Advanced menu, say, with a menu item that would be checked if the value was set.
        // However, this isn't needed- one can take advantage of the "P0" box to insert additional code. See henon.cfg.
        

        //in this program, the "shading" trackbar is actually the "total points" trackbar.
        private void tbShading_Scroll(object sender, EventArgs e) { tbTotalPoints_Scroll(sender, e); }
        private void lblShading_Click(object sender, EventArgs e) { lblTotalPoints_Click(sender, e); }
        private TrackBar tbTotalPoints;
        private Label lblTotalPoints;

    }
}
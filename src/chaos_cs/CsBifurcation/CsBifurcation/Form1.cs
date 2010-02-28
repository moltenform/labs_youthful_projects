/*
 * only concern: if in program files, won't have write access, need to set up in docs
 * See mouse shortcuts in plotusercontrol.cs
 * Press ctrl+space to redraw
 * Enter arbitrary code in P0, such as C1, rand(), or randneg()
 * Change additionalShading in init code for higher quality.
 * Press left/right and pgup/pgdn to make small changes to C1, C2
 * 
 * version 217 was a large change.
 * todo: threading, previews, zoom animations, undo stack, click lbl to fine-tune value. nudge view left/right?
 * todo: clicking on plot to zoom in should incorporate textbox changes? Eliminate view menu, plotcontrol to have only public methods no ui?
 * 
 * */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            else
                Directory.SetCurrentDirectory(@"cfgs"); //Release environment

            InitializeComponent();
            lblParam1.Text = lblParam2.Text =lblParam3.Text =lblParam4.Text = lblSettling.Text = lblShading.Text = "";
            mnuAdvShades.Checked = true; mnuAdvPoints.Checked = false;
            
            mnuFileNew_Click(null, null);
        }

        

        
       

        private void btnGo_Click(object sender, EventArgs e)
        {
            Redraw();
        }
        public void Redraw()
        {
            this.pointPlotBifurcationUserControl1.paramExpression = getUserExpression(this.txtExpression.Text);
            this.pointPlotBifurcationUserControl1.paramP0 = getUserExpression(this.txtP0.Text);
            this.pointPlotBifurcationUserControl1.paramInit = getUserExpression(this.txtInit.Text);
            this.pointPlotBifurcationUserControl1.bShading = this.mnuAdvShades.Checked;

            this.pointPlotBifurcationUserControl1.redraw();
        }



        //transform sin into Math.sin
        protected string getUserExpression(string s)
        {
            CodedomEvaluator.CodedomEvaluator cb = new CodedomEvaluator.CodedomEvaluator();
            s = cb.addMathMethods(s);
            //s = Regex.Replace(s, "\\brand\\(\\)\\b", "R.NextDouble()");
            s = Regex.Replace(s, "\\brand\\b", "R.NextDouble");
            s = Regex.Replace(s, "\\brandneg\\(\\)", "((R.NextDouble()-0.5)*2)");
            
            return s;
        }

        private void tbSettling_Scroll(object sender, EventArgs e)
        {
            double d = (tbSettling.Value / 1000.0);
            int v = (int)Math.Pow(10.0, d * 6);
            lblSettling.Text = v.ToString();
            this.pointPlotBifurcationUserControl1.paramSettle = v;
        }

        private void tbShading_Scroll(object sender, EventArgs e)
        {
            double v = (tbShading.Value / 1000.0);
            pointPlotBifurcationUserControl1.paramShading = v;
            lblShading.Text = v.ToString();
        }

        private void tbParam1_Scroll(object sender, EventArgs e)
        {
            double v = ((tbParam1.Value / ((double)tbParam1.Maximum)) - 0.5)*2;
            pointPlotBifurcationUserControl1.param1 = v;
            lblParam1.Text = v.ToString();
            if (mnuAdvAutoRedraw.Checked) pointPlotBifurcationUserControl1.redraw();
        }
        private void tbParam2_Scroll(object sender, EventArgs e)
        {
            double v = ((tbParam2.Value / ((double)tbParam1.Maximum)) - 0.5)*2;
            pointPlotBifurcationUserControl1.param2 = v;
            lblParam2.Text = v.ToString();
        }
        private void tbParam3_Scroll(object sender, EventArgs e)
        {
            double v = (tbParam3.Value / ((double)tbParam3.Maximum));
            pointPlotBifurcationUserControl1.param3 = v;
            lblParam3.Text = v.ToString();
        }
        private void tbParam4_Scroll(object sender, EventArgs e)
        {
            double v = (tbParam4.Value / ((double)tbParam4.Maximum));
            pointPlotBifurcationUserControl1.param4 = v;
            lblParam4.Text = v.ToString();
        }

        private bool _getBounds(string sName, double fCurrent, out double dOut)
        {
            dOut = 0.0;
            string s = InputBoxForm.GetStrInput(sName, fCurrent.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="") return false;
            return double.TryParse(s, out dOut);
        }
        private void setBounds()
        {
            double X0, X1, Y0, Y1, nX0, nX1, nY0, nY1;
            pointPlotBifurcationUserControl1.getBounds(out X0, out X1, out Y0, out Y1);

            if (!_getBounds("Leftmost x", X0, out nX0)) return;
            if (!_getBounds("Rightmost x", X1, out nX1)) return;
            if (!_getBounds("Lowest y", Y0, out nY0)) return;
            if (!_getBounds("Greatest y", Y1, out nY1)) return;

            if (!(nY1 > nY0 && nX1 > nX0)) { MessageBox.Show("Invalid bounds."); return; }

            this.pointPlotBifurcationUserControl1.setBounds(nX0, nX1, nY0, nY1);
            this.pointPlotBifurcationUserControl1.redraw();
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
        private void loadIni(string sFilename)
        {
            //note: requires absolute path to file.
            if (!File.Exists(sFilename)) return;
            IniFileParsing ifParsing = new IniFileParsing(sFilename, true);
            try
            {
                CsIniLoadHelper loader = new CsIniLoadHelper(ifParsing, "main");

                //set props
                double nX0 = loader.getDouble("X0");
                double nX1 = loader.getDouble("X1");
                double nY0 = loader.getDouble("Y0");
                double nY1 = loader.getDouble("Y1");
                pointPlotBifurcationUserControl1.setBounds(nX0, nX1, nY0, nY1);
                pointPlotBifurcationUserControl1.param1 = loader.getDouble("param1");
                pointPlotBifurcationUserControl1.param2 = loader.getDouble("param2");
                pointPlotBifurcationUserControl1.param3 = loader.getDouble("param3", true);
                pointPlotBifurcationUserControl1.param4 = loader.getDouble("param4", true);
                pointPlotBifurcationUserControl1.paramSettle = loader.getInt("paramSettle");
                pointPlotBifurcationUserControl1.paramShading = loader.getDouble("paramShading");

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
            double p1 = pointPlotBifurcationUserControl1.param1; double p2 = pointPlotBifurcationUserControl1.param2;
            double p3 = pointPlotBifurcationUserControl1.param3; double p4 = pointPlotBifurcationUserControl1.param4;
            double ps = pointPlotBifurcationUserControl1.paramShading; double pst = pointPlotBifurcationUserControl1.paramSettle;
            lblParam1.Text = p1.ToString();
            lblParam2.Text = p2.ToString();
            lblSettling.Text = pst.ToString();
            lblShading.Text = ps.ToString();
            // set sliders, TODO: also set for sliderSettling (known issue) 
            tbShading.Value = (int)(tbShading.Maximum*ps);
            tbParam1.Value = (int)(tbParam1.Maximum*(p1 / 2+.5));
            tbParam2.Value = (int)(tbParam2.Maximum*(p2 / 2+.5));
            tbParam3.Value = (int)(tbParam3.Maximum*p3);
            tbParam4.Value = (int)(tbParam4.Maximum*p4);

            Redraw();
        }
        private void saveIni(string sFilename)
        {
            IniFileParsing ifParsing = new IniFileParsing(sFilename, false); //creates ini if doesn't exist
            try
            {
                CsIniSaveHelper saver = new CsIniSaveHelper(ifParsing, "main"); //one section called "main"

                double nX0, nX1, nY0, nY1;
                pointPlotBifurcationUserControl1.getBounds(out nX0, out nX1, out nY0, out nY1);
                saver.saveDouble("X0", nX0);
                saver.saveDouble("X1", nX1);
                saver.saveDouble("Y0", nY0);
                saver.saveDouble("Y1", nY1);

                saver.saveDouble("param1", pointPlotBifurcationUserControl1.param1);
                saver.saveDouble("param2", pointPlotBifurcationUserControl1.param2);
                saver.saveInt("paramSettle", pointPlotBifurcationUserControl1.paramSettle);
                saver.saveDouble("paramShading", pointPlotBifurcationUserControl1.paramShading);

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
        private void mnuFileAnimate_Click(object sender, EventArgs e)
        {
            double d, c0_0, c0_1, c1_0, c1_1; string s; int nframes;
            double param1=pointPlotBifurcationUserControl1.param1, param2=pointPlotBifurcationUserControl1.param2;
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
                pointPlotBifurcationUserControl1.param1 = c0_0;
                pointPlotBifurcationUserControl1.param2 = c1_0;
                pointPlotBifurcationUserControl1.renderToDiskSave(400, 400, sfilename.Replace(".png", "_"+i.ToString()+".png"));
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
            //create 3200x3200
            pointPlotBifurcationUserControl1.renderToDisk(3200,3200);
        }

        private void mnuAdvShades_Click(object sender, EventArgs e) { mnuAdvShades.Checked = true; mnuAdvPoints.Checked=false; Redraw(); }
        private void mnuAdvPoints_Click(object sender, EventArgs e) { mnuAdvShades.Checked = false; mnuAdvPoints.Checked=true; Redraw(); }
        private void mnuViewRedraw_Click(object sender, EventArgs e) { Redraw(); }
        private void mnuAdvAutoRedraw_Click(object sender, EventArgs e) { mnuAdvAutoRedraw.Checked=!mnuAdvAutoRedraw.Checked; }
        private void mnuFileExit_Click(object sender, EventArgs e) { Close(); }
        private void mnuViewZoomIn_Click(object sender, EventArgs e) { pointPlotBifurcationUserControl1.zoomIn(); }
        private void mnuViewZoomOut_Click(object sender, EventArgs e) { pointPlotBifurcationUserControl1.zoomOut(); }
        private void mnuViewZoomUndo_Click(object sender, EventArgs e) { pointPlotBifurcationUserControl1.undoZoom(); }
        private void mnuViewReset_Click(object sender, EventArgs e) { pointPlotBifurcationUserControl1.resetZoom(); }

        private void mnuAdvBounds_Click(object sender, EventArgs e) { setBounds(); }
        private void mnuAdvAddQuality_Click(object sender, EventArgs e) { MessageBox.Show("To run more iterations, producing better images, add a line to \"init. code\" such as: \r\n\r\nadditionalShading=2.0;"); }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CsBifurcation\r\nBy Ben Fisher, 2010.\r\n\r\nhttp://halfhourhacks.blogspot.com");
        }



        



        

        



        
    }
}
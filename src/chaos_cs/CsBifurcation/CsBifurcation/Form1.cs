/*
 * See keyboard shortcuts in plotusercontrol.cs
 * Press ctrl+enter to redraw
 * Enter arbitrary code in P0, such as C1, rand(), or randneg()
 * Change additionalShading in init code for higher quality.
 * 
 * version 217 was a large change.
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
            lblParam1.Text = lblParam2.Text = lblSettling.Text = lblShading.Text = "";
            rdoShade.Checked = true; rdoPoints.Checked = false;
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            
            mnuFileNew_Click(null, null);
        }

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && ((e.Modifiers & Keys.Control)!=0))
                pointPlotBifurcationUserControl1.redraw();
            else
                e.Handled = false;
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
            this.pointPlotBifurcationUserControl1.bShading = this.rdoShade.Checked;

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
            //pointPlotBifurcationUserControl1.redraw();
        }

        private void tbShading_Scroll(object sender, EventArgs e)
        {
            double v = (tbShading.Value / 1000.0);
            pointPlotBifurcationUserControl1.paramShading = v;
            lblShading.Text = v.ToString();
        }

        private void tbParam1_Scroll(object sender, EventArgs e)
        {
            double v = (tbParam1.Value / 1000.0);
            pointPlotBifurcationUserControl1.param1 = v;
            lblParam1.Text = v.ToString();
        }

        private void tbParam2_Scroll(object sender, EventArgs e)
        {
            double v = (tbParam2.Value / 1000.0);
            pointPlotBifurcationUserControl1.param2 = v;
            lblParam2.Text = v.ToString();
        }

        private bool getBounds(string sName, double fCurrent, out double dOut)
        {
            dOut = 0.0;
            string s = InputBoxForm.GetStrInput(sName, fCurrent.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="") return false;
            return double.TryParse(s, out dOut);
        }
        private void btnBounds_Click(object sender, EventArgs e)
        {
            double X0, X1, Y0, Y1, nX0, nX1, nY0, nY1;
            pointPlotBifurcationUserControl1.getBounds(out X0, out X1, out Y0, out Y1);

            if (!getBounds("Leftmost x", X0, out nX0)) return;
            if (!getBounds("Rightmost x", X1, out nX1)) return;
            if (!getBounds("Lowest y", Y0, out nY0)) return;
            if (!getBounds("Greatest y", Y1, out nY1)) return;

            if (!(nY1 > nY0 && nX1 > nX0)) { MessageBox.Show("Invalid bounds."); return; }

            this.pointPlotBifurcationUserControl1.setBounds(nX0, nX1, nY0, nY1);
            this.pointPlotBifurcationUserControl1.redraw();
        }

        

       

        private void btnMovie_Click(object sender, EventArgs e)
        {
            double d, c0_0, c0_1, c1_0, c1_1; string s; int nframes;
            double param1=pointPlotBifurcationUserControl1.param1, param2=pointPlotBifurcationUserControl1.param2;
            s = InputBoxForm.GetStrInput("Initial c0:", param1.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="" || !double.TryParse(s, out d)) return;
            c0_0=d;
            s = InputBoxForm.GetStrInput("Final c0:", param1.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="" || !double.TryParse(s, out d)) return;
            c0_1=d;
            s = InputBoxForm.GetStrInput("Initial c1:", param2.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="" || !double.TryParse(s, out d)) return;
            c1_0=d;
            s = InputBoxForm.GetStrInput("Final c1:", param2.ToString(CultureInfo.InvariantCulture));
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
                pointPlotBifurcationUserControl1.renderToDiskSave(400,400,sfilename.Replace(".png","_"+i.ToString()+".png"));
                c0_0 += c0_inc;
                c1_0 += c1_inc;
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
        private void loadIni(string sFilename)
        {
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
                pointPlotBifurcationUserControl1.paramSettle = loader.getInt("paramSettle");
                pointPlotBifurcationUserControl1.paramShading = loader.getDouble("paramShading");

                //these are transformed, so set the ui instead of the prop. Call to Redraw will retrieve this.
                rdoShade.Checked = loader.getInt("bShading")!=0;
                rdoPoints.Checked = loader.getInt("bShading")==0;
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
            double ps = pointPlotBifurcationUserControl1.paramShading; double pst = pointPlotBifurcationUserControl1.paramSettle;
            lblParam1.Text = p1.ToString();
            lblParam2.Text = p2.ToString();
            lblSettling.Text = pst.ToString();
            lblShading.Text = ps.ToString();
            // set sliders, TODO: also set for sliderSettling (known issue) 
            tbShading.Value = (int)(tbShading.Maximum*ps);
            tbParam1.Value = (int)(tbShading.Maximum*p1);
            tbParam2.Value = (int)(tbShading.Maximum*p2);

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
                saver.saveInt("bShading", rdoShade.Checked?1:0);

                saver.saveString("programVersion", Version);
            }
            catch (IniFileParsingException err)
            {
                MessageBox.Show("Prefs Error:"+err.ToString());
                return;
            }
        }

        private void mnuFileNew_Click(object sender, EventArgs e)
        {
            string sFilename = Path.GetFullPath(Directory.GetCurrentDirectory()) + "\\default.cfg";
            if (File.Exists(sFilename))
                loadIni(sFilename);
            else
                Redraw();
        }

    }
}
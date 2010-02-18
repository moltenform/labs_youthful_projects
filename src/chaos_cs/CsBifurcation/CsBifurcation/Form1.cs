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
        public string strIniPath = @"C:\Ben's Goodies\SpringChaos\csbifurcation.ini"; //Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\csbifurcation.ini";
        public Form1()
        {
            InitializeComponent();
            lblParam1.Text = lblParam2.Text = lblSettling.Text = lblShading.Text = "";
            rdoShade.Checked = true; rdoPoints.Checked = false;
            txtP0.Text = "0.35";
            this.loadSavedConfigs();
        }

        
       

        private void btnGo_Click(object sender, EventArgs e)
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

        private void btnBounds_Click(object sender, EventArgs e)
        {
            double nX0, nX1, nY0, nY1;
            pointPlotBifurcationUserControl1.getBounds(out nX0, out nX1, out nY0, out nY1);
            string sCurrent=nX0.ToString(CultureInfo.InvariantCulture)+","+nX1.ToString(CultureInfo.InvariantCulture)+","+nY0.ToString(CultureInfo.InvariantCulture)+","+nY1.ToString(CultureInfo.InvariantCulture)+","+nX0.ToString(CultureInfo.InvariantCulture);

            double x0, x1, y0, y1;
            string s = InputBoxForm.GetStrInput("Enter boundaries, separated by commas:", sCurrent); //"-10,10,-10,10"
            if (s==null || s=="") return;
            string[] ss = s.Split(new char[] { ',' });
            if (ss.Length != 4) { MessageBox.Show("Couldn't parse boundaries."); return; }

            if (!double.TryParse(ss[0].Replace(" ", ""), out x0))
            { MessageBox.Show("Couldn't parse boundaries."); return; }
            if (!double.TryParse(ss[1].Replace(" ", ""), out x1))
            { MessageBox.Show("Couldn't parse boundaries."); return; }
            if (!double.TryParse(ss[2].Replace(" ", ""), out y0))
            { MessageBox.Show("Couldn't parse boundaries."); return; }
            if (!double.TryParse(ss[3].Replace(" ", ""), out y1))
            { MessageBox.Show("Couldn't parse boundaries."); return; }

            this.pointPlotBifurcationUserControl1.setBounds(x0, x1, y0, y1);
            this.pointPlotBifurcationUserControl1.redraw();

        }

        private void loadSavedConfigs()
        {
            comboBox1.Items.Clear();
            if (!File.Exists(strIniPath)) return;
            IniFileParsing ifParsing = new IniFileParsing(strIniPath, true);
            List<string> l = ifParsing.GetCategories();
            foreach (string s in l) comboBox1.Items.Add(s);
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!File.Exists(strIniPath)) return;
            IniFileParsing ifParsing = new IniFileParsing(strIniPath, true);
            string sChosen = this.comboBox1.Items[comboBox1.SelectedIndex] as string;
            try
            {
                CsIniLoadHelper loader = new CsIniLoadHelper(ifParsing, sChosen);
                
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

                //these are transformed, so set the ui instead of the prop
                //pointPlotBifurcationUserControl1.paramExpression = loader.getString("paramExpression");
                //pointPlotBifurcationUserControl1.paramInit = loader.getString("paramInit");
                //pointPlotBifurcationUserControl1.paramP0 = loader.getString("paramP0");
                //pointPlotBifurcationUserControl1.bShading = loader.getInt("bShading")!=0;
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

            //retrieves from txtExpression and redraws()
            btnGo_Click(null, null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string sName = InputBoxForm.GetStrInput("Enter name:", "");
            if (sName==null || sName=="")
                return;

            //make the name alphanumeric, but also allowing _,.
            Regex pattern=new Regex("[^a-zA-Z0-9_,. ]");
            if (pattern.IsMatch(sName)) { MessageBox.Show("Invalid character in name."); return; }

            /* It's ok to overwrite existing...
            foreach (object o in this.comboBox1.Items)
            {
                if (o as string).Equals(sName)) { MessageBox.Show("Name '"+sName+"' already exists.");  return;  }
            }*/

            IniFileParsing ifParsing = new IniFileParsing(strIniPath, false); //creates ini if doesn't exist
            try
            {
                CsIniSaveHelper saver = new CsIniSaveHelper(ifParsing, sName);

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

            }
            catch (IniFileParsingException err)
            {
                MessageBox.Show("Prefs Error:"+err.ToString());
                loadSavedConfigs();
                return;
            }
            loadSavedConfigs();

            //will create inifile if doesn't exist
           // IniFileParsing ifParsing = new IniFileParsing(strIniPath, false);
           // CsIniSaveHelper prefs = new CsIniSaveHelper(ifParsing, sName);

        }


    }
}
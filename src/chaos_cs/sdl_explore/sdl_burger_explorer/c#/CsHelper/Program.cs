using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CsBifurcation;
using System.IO;

namespace CsHelper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            TheProgram.TheMain(Environment.GetCommandLineArgs());
        }

        class TheProgram
        {
            //convieniently, c's system() is blocking and waits for us to quit.

            static string INPUTFILE = "data\\helper_in.txt";
            static string OUTPUTFILE = "data\\helper_out.txt";
            static string TEMPLATEFILE = "data\\save_template.txt";
            public static void TheMain(string[] args)
            {
                if (args.Length != 2)
                {
                    System.Console.WriteLine("Give argument of s,o,:,or;");
                    return;
                }
                string thearg = args[1];
                //writeToFile(OUTPUTFILE, "-1"); //means to wait
                //InputBoxForm tmpform = new InputBoxForm();
                //tmpform.Show();
                string sOutput = "0";
                try
                {
                    if (thearg.Equals("s"))
                    {
                        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                        saveFileDialog1.Filter = "Input files (*.cfg)|*.cfg";
                        saveFileDialog1.RestoreDirectory = true;
                        if ((saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog1.FileName.Length > 0))
                        {
                            string sInputData = readFromFile(INPUTFILE);
                            string sTemplate = readFromFile(TEMPLATEFILE);
                            sTemplate = sTemplate.Replace("%data%", sInputData);
                            string[] sParts = sInputData.Split(new char[] { ',' });
                            if (sParts.Length > 29)
                            {
                                sTemplate = sTemplate.Replace("%x0%", sParts[7]);
                                sTemplate = sTemplate.Replace("%x1%", sParts[9]);
                                sTemplate = sTemplate.Replace("%y0%", sParts[11]);
                                sTemplate = sTemplate.Replace("%y1%", sParts[13]);
                                sTemplate = sTemplate.Replace("%a%", sParts[15]);
                                sTemplate = sTemplate.Replace("%b%", sParts[17]);
                                sTemplate = sTemplate.Replace("%settle%", sParts[29]);
                            }
                            writeToFile(saveFileDialog1.FileName, sTemplate);
                            sOutput = "1";
                        }
                        else
                            sOutput = "0";
                    }
                    else if (thearg.Equals("o"))
                    {
                        OpenFileDialog dlg1 = new OpenFileDialog();
                        dlg1.RestoreDirectory = true;
                        dlg1.Filter = "Files (*.cfg)|*.cfg";
                        if ((dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK && dlg1.FileName.Length > 0))
                        {
                            //sOutput = dlg1.FileName; //simply write the filename to the output
                            sOutput = readFromFile(dlg1.FileName);
                        }
                        else
                            sOutput = "0";
                    }
                    
                    else if (thearg.Equals(":"))
                    {
                        string a="0", b="0"; double tmp;
                        string sInputData = readFromFile(INPUTFILE);
                        string[] sParts = sInputData.Split(new char[] { ',' });
                        if (sParts.Length > 29)
                        {
                            a = sParts[15]; b = sParts[17];
                        }
                        //if something invalid written, just revert to previous values.
                        string out1 = CsBifurcation.InputBoxForm.GetStrInput("Value of a:", a);
                        if (out1==null || out1==""|| !double.TryParse(out1, out tmp))
                            out1 = a;
                        string out2 = CsBifurcation.InputBoxForm.GetStrInput("Value of b:", b);
                        if (out2==null || out2==""|| !double.TryParse(out2, out tmp))
                            out2 = b;
                        sOutput = ("out:" + out1 + ","+out2);
                    }
                    else if (thearg.Equals(";"))
                    {
                        string seeds="20", settle="20", drawn="20"; int tmp;
                        string sInputData = readFromFile(INPUTFILE);
                        string[] sParts = sInputData.Split(new char[] { ',' });
                        if (sParts.Length > 29)
                        {
                            seeds = sParts[27];
                            settle = sParts[29];
                            drawn = sParts[31];
                        }
                        //if something invalid written, just revert to previous values.
                        string outSet = CsBifurcation.InputBoxForm.GetStrInput("Settling time:", settle);
                        if (outSet==null || outSet==""|| !int.TryParse(outSet, out tmp))
                            outSet = settle;
                        string outSeed = CsBifurcation.InputBoxForm.GetStrInput("Seeds per axis:", seeds);
                        if (outSeed==null || outSeed==""|| !int.TryParse(outSeed, out tmp))
                            outSeed = seeds;
                        string outDraw = CsBifurcation.InputBoxForm.GetStrInput("Points per seed:", drawn);
                        if (outDraw==null || outDraw==""|| !int.TryParse(outDraw, out tmp))
                            outDraw = drawn;

                        outSet = int.Parse(outSet).ToString();
                        outSeed = int.Parse(outSeed).ToString();
                        outDraw = int.Parse(outDraw).ToString();
                        sOutput = ("out:" + outSeed + ","+outSet+","+outDraw);
                    }

                }
                finally
                {
                    writeToFile(OUTPUTFILE, sOutput); //so that it stops waiting!
                }
            }


            static string readFromFile(string fname)
            {
                string s;
                using (TextReader tr = new StreamReader(fname))
                    s = tr.ReadToEnd();
                return s;
            }
            static void writeToFile(string fname, string text)
            {
                using (TextWriter tw = new StreamWriter(fname))
                    tw.Write(text);
            }
            static void writeToFileASCII(string fname, string text)
            {
                /*  Encoding enc = Encoding.ASCII;
                  using (StreamWriter fst = new StreamWriter(fname, enc))
                  using (TextWriter tw = new StreamWriter(fname, enc))
                      tw.Write(text);*/
            }
        }
    }
}
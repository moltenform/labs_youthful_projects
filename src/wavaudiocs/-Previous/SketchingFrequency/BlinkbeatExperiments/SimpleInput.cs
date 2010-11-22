//Ben Fisher, 2008
//halfhourhacks.blogspot.com
//GPL
//Simple input dialog I made very quickly. Why wasn't this built in?

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BlinkbeatExperiments
{
    public partial class SimpleInput : Form
    {
        public SimpleInput()
        {
            InitializeComponent();
        }

        public static string AskSimpleInput(string strQuestion, string strTitle, string strDefault)
        {
            SimpleInput form = new SimpleInput();
            form.lbl.Text = strQuestion;
            form.Text = strTitle;
            form.textBox1.Text = strDefault;
            if (form.ShowDialog() == DialogResult.OK)
            {
                string str = form.textBox1.Text;
                form.Close();
                return str;
            }
            else
            {
                return null;
            }
        }
    }
}
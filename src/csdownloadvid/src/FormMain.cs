// Copyright (c) Ben Fisher, 2016.
// Licensed under GPLv3, refer to LICENSE for details.

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace CsDownloadVid
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void mnuOpenFormGetVideo_Click(object sender, EventArgs e)
        {
            using (Form form = new FormGetVideo())
                form.ShowDialog(this);
        }

        private void mnuOpenFormAudioFromVideo_Click(object sender, EventArgs e)
        {
            using (Form form = new FormAudioFromVideo())
                form.ShowDialog(this);
        }

        private void mnuOpenFormMediaSplit_Click(object sender, EventArgs e)
        {
            using (Form form = new FormMediaSplit())
                form.ShowDialog(this);
        }

        private void mnuOpenFormMediaJoin_Click(object sender, EventArgs e)
        {
            using (Form form = new FormMediaJoin())
                form.ShowDialog(this);
        }

        private void mnuEncodeCustom_Click(object sender, EventArgs e)
        {
            using (Form form = new FormMediaJoin(true))
                form.ShowDialog(this);
        }

        private void mnuOpenHelpWebsite_Click(object sender = null, EventArgs e = null)
        {
            Process.Start("https://github.com/moltenform/labs_youthful_projects/" +
                "tree/master/src/csdownloadvid/README.md");
        }
    }
}

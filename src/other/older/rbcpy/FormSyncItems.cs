using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rbcpy
{
    public partial class FormSyncItems : Form
    {
        CCreateSyncResultsSet m_results;
        RbcpyGlobalSettings m_globalSettings;
        RunSyncOnBackgroundThread m_runner;
        bool m_includeProgress = false;
        int m_nSortCol = 3;
        const int m_rowDetail = 2;
        const int m_rowSummary = 3;
        bool m_bPreview = false;
        public FormSyncItems(CCreateSyncResultsSet results, RbcpyGlobalSettings globalSettings, bool bPreview)
        {
            InitializeComponent();
            m_results = results;
            m_globalSettings = globalSettings;
            m_bPreview = bPreview;

            if (bPreview)
            {
                this.Text = "Preview";
                this.lblNameOfAction.Text = "Preview:";
            }
            else
            {
                this.Text = "Results";
                this.lblNameOfAction.Text = "Results:";
                btnRun.Text = "Sync Complete";
                btnRun.Enabled = false;
                btnIncludeBoth.Enabled = false;
                btnCompareWinmerge.Enabled = false;
                btnLeftToRight.Enabled = btnRightToLeft.Enabled = false;
                btnShowLeft.Enabled = btnShowRight.Enabled = false;
            }

            listView.UseCompatibleStateImageBehavior = false;
            listView.SmallImageList = imageList1;
            txtSummary.ReadOnly = true;
            txtSummary.Text = results.sSummary;
            ReAddItems();
            ShowSummary();
            txtSummary.Select(0, 0);
            listView.Focus();
        }

        void ShowSummary()
        {
            this.tableLayoutPanel1.RowStyles[m_rowDetail].Height = 1;
            this.tableLayoutPanel1.RowStyles[m_rowSummary].Height = 120;
        }
        void HideSummary()
        {
            this.tableLayoutPanel1.RowStyles[m_rowDetail].Height = 260;
            this.tableLayoutPanel1.RowStyles[m_rowSummary].Height = 1;
        }

        private void mnuViewSummary_Click(object sender, EventArgs e)
        {
            ShowSummary();
        }

        private void ReAddItems()
        {
            CCreateSyncItem.SortFromColumnNumber(m_results.items, m_nSortCol);

            listView.SuspendLayout();
            listView.Items.Clear();
            foreach (var item in m_results.items)
            {
                if (item.status == CCreateSyncItemStatus.Unknown && !m_includeProgress)
                    continue;
                if (item.status == CCreateSyncItemStatus.AddedInDest && !m_results.config.m_mirror && !m_includeProgress)
                    continue;

                listView.Items.Add(item.GetListItem(m_bPreview));
            }

            listView.ResumeLayout();
        }

        private void mnuCopyAll_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in IterateListViewItems(false))
            {
                sb.AppendLine(item.ToString());
            }

            sb.AppendLine();
            Clipboard.SetText(sb.ToString());
        }

        private void mnuCopySelected_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in IterateListViewItems(true))
            {
                sb.AppendLine(item.ToString());
            }

            sb.AppendLine();
            Clipboard.SetText(sb.ToString());
        }

        private void btnCompareWinmerge_Click(object sender, EventArgs e)
        {
            // "View in winmerge"
            foreach (var item in IterateListViewItems(true))
            {
                var itemObj = item as CCreateSyncItem;
                if (itemObj != null && (itemObj.status == CCreateSyncItemStatus.ChangedAndDestNewer || itemObj.status == CCreateSyncItemStatus.ChangedAndSrcNewer))
                {
                    RunImplementation.OpenWinmerge(m_globalSettings.m_winMergeDir,
                        itemObj.GetLeftPath(m_results.config), itemObj.GetRightPath(m_results.config), false);
                }
            }
        }

        private IEnumerable<CCreateSyncItem> IterateListViewItems(bool onlySelected)
        {
            ICollection lists = listView.Items;
            if (onlySelected)
                lists = listView.SelectedItems;

            foreach (var item in lists)
            {
                ListViewItem lvitem = item as ListViewItem;
                if (lvitem != null && lvitem.Tag != null)
                {
                    CCreateSyncItem syncitem = lvitem.Tag as CCreateSyncItem;
                    if (syncitem != null)
                    {
                        yield return syncitem;
                    }
                }
            }
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            CCreateSyncItem first = IterateListViewItems(true /*only selected*/).FirstOrDefault();
            if (first == null || !m_bPreview)
            {
                ShowSummary();
            }
            else
            {
                HideSummary();
                StringBuilder sbLeft = new StringBuilder(), sbRight = new StringBuilder();
                GetSelectedDetailsText(sbLeft, sbRight);
                txtSelectedDetailsLeft.Text = sbLeft.ToString();
                txtSelectedDetailsRight.Text = sbRight.ToString();
            }
        }

        private void GetSelectedDetailsText(StringBuilder sbLeft, StringBuilder sbRight)
        {
            if (!m_bPreview)
                return;

            FileInfo recentLeft = null, recentRight = null;
            long nCountLeft = 0, nCountRight = 0, nTotalLeft = 0, nTotalRight = 0;
            foreach (var item in IterateListViewItems(true /*only selected*/))
            {
                if (item.IsInLeft())
                {
                    if (File.Exists(item.GetLeftPath(this.m_results.config)))
                    {
                        nCountLeft++;
                        recentLeft = new FileInfo(item.GetLeftPath(this.m_results.config));
                        nTotalLeft += recentLeft.Length;
                    }
                }
                if (item.IsInRight())
                {
                    if (File.Exists(item.GetRightPath(this.m_results.config)))
                    {
                        nCountRight++;
                        recentRight = new FileInfo(item.GetRightPath(this.m_results.config));
                        nTotalRight += recentRight.Length;
                    }
                }
            }

            btnLeftToRight.Visible = nCountLeft > 0;
            btnShowLeft.Visible = nCountLeft > 0;
            sbLeft.AppendLine("\r\n" + nCountLeft + " file(s).");
            sbLeft.AppendLine(String.Format("{0:n0} bytes.", nTotalLeft));
            if (nCountLeft == 1)
            {
                sbLeft.AppendLine("Created: " + recentLeft.CreationTime.ToString());
                sbLeft.AppendLine("Modified: " + recentLeft.LastWriteTime.ToString());
            }

            btnRightToLeft.Visible = nCountRight > 0;
            btnShowRight.Visible = nCountRight > 0;
            sbRight.AppendLine("\r\n" + nCountRight + " file(s).");
            sbRight.AppendLine(String.Format("{0:n0} bytes.", nTotalRight));
            if (nCountRight == 1)
            {
                sbRight.AppendLine("Created: " + recentRight.CreationTime.ToString());
                sbRight.AppendLine("Modified: " + recentRight.LastWriteTime.ToString());
            }
        }

        private void btnShowLeft_Click(object sender, EventArgs e)
        {
            foreach (var item in IterateListViewItems(true))
            {
                RunImplementation.ShowInExplorer(item.GetLeftPath(m_results.config));
                return;
            }
        }

        private void btnShowRight_Click(object sender, EventArgs e)
        {
            foreach (var item in IterateListViewItems(true))
            {
                RunImplementation.ShowInExplorer(item.GetRightPath(m_results.config));
                return;
            }
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            int nCol = e.Column + 1;
            if (m_nSortCol == nCol)
            {
                m_nSortCol *= -1;
            }
            else
            {
                m_nSortCol = nCol;
            }

            ReAddItems();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            m_runner = new RunSyncOnBackgroundThread
            {
                btnToTemporarilyDisable = btnRun,
                sPreviousButtonName = "Run Synchronization",
                globalSettings = m_globalSettings,
                config = m_results.config,
                preview = false,
            };

            m_runner.Run();
        }

        private void viewIncludeTempEntries_Click(object sender, EventArgs e)
        {
            m_includeProgress = !m_includeProgress;
            ReAddItems();
        }

        private void FormSyncItems_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (File.Exists(m_results.sLogFilename))
            {
                try
                {
                    File.Delete(m_results.sLogFilename);
                }
                catch(Exception)
                {
                    // swallow exception
                }
            }
        }

        private void ManualCopyFileImpl(string s1, string s2)
        {
            try
            {
                if (File.Exists(s2) && !String.IsNullOrEmpty(this.m_globalSettings.m_directoryForDeletedFiles))
                {
                    var newName = m_globalSettings.m_directoryForDeletedFiles + "\\" + Path.GetFileName(s2) + RunImplementation.GetRandomNumbersInString();
                    File.Copy(s2, newName, true /*okOverwrite*/);
                }

                File.Copy(s1, s2, true /*okOverwrite*/);
            }
            catch (Exception e)
            {
                MessageBox.Show("Copying "+s1+" to "+s2+". Exception "+e);
            }
        }
        private void ManualCopyFile(bool leftToRight)
        {
            // "copy left to right"
            if (MessageBox.Show("Confirm copy?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (var item in IterateListViewItems(true /*only selected*/))
                {
                    if (leftToRight && item.IsInLeft())
                    {
                        ManualCopyFileImpl(item.GetLeftPath(m_results.config), item.GetRightPath(m_results.config));
                    }
                    else if (!leftToRight && item.IsInRight())
                    {
                        ManualCopyFileImpl(item.GetRightPath(m_results.config), item.GetLeftPath(m_results.config));
                    }
                }
            }
        }

        private void btnLeftToRight_Click(object sender, EventArgs e)
        {
            ManualCopyFile(true);
        }

        private void btnRightToLeft_Click(object sender, EventArgs e)
        {
            ManualCopyFile(false);
        }

        private void btnIncludeBoth_Click(object sender, EventArgs e)
        {
            // save the existing one by copying it to the left side
            foreach (var item in IterateListViewItems(true /*only selected*/))
            {
                if (item.IsInRight())
                {
                    var nameOnLeft = Path.GetDirectoryName(item.GetLeftPath(m_results.config)) + "\\" + Path.GetFileNameWithoutExtension(item.GetLeftPath(m_results.config)) + RunImplementation.GetRandomNumbersInString() +
                        Path.GetExtension(item.GetLeftPath(m_results.config));
                    if (MessageBox.Show("Confirm copy from " + item.GetRightPath(m_results.config) + " to " + nameOnLeft + "?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        File.Copy(item.GetRightPath(m_results.config), nameOnLeft, false /*okToOverwrite*/);
                    }
                }
            }
        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            btnCompareWinmerge_Click(null, null);
        }
    }
}

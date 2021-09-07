using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace rbcpy
{
    public partial class FormSyncMain : Form
    {
        RbcpyGlobalSettings m_globalSettings = new RbcpyGlobalSettings();
        Dictionary<string, TextBox> m_mapTextItems;
        Dictionary<string, CheckBox> m_mapCheckItems;
        RunSyncOnBackgroundThread m_runner;
        Dictionary<string, string> m_variables = new Dictionary<string, string>();

        public FormSyncMain()
        {
            InitializeComponent();
            m_mapTextItems = new Dictionary<string, TextBox> { 
                {"m_src", this.txtSrc},
                {"m_destination", this.txtDest},
                {"m_excludeDirs", this.txtExcludeDirs},
                {"m_excludeFiles", this.txtExcludeFiles},
                {"m_copyFlags", this.txtCopyFlags},
                {"m_directoryCopyFlags", this.txtDirCopyFlags},
                {"m_ipg", this.txtIpg},
                {"m_nRetries", this.txtnRetries},
                {"m_waitBetweenRetries", this.txtnWaitBetweenRetries},
                {"m_nThreads", this.txtnThreads},
                {"m_custom", this.txtCustom},
            };

            m_mapCheckItems = new Dictionary<string, CheckBox> { 
                {"m_mirror", this.chkMirror},
                {"m_copySubDirsAndEmptySubdirs", this.chkCopySubdirs},
                {"m_symlinkNotTarget", this.chkSymlinkNotTarget},
                {"m_fatTimes", this.chkFatTimes},
                {"m_compensateDst", this.chkCompensateDST},
            };

            try
            {
                var directory = AppDomain.CurrentDomain.BaseDirectory.ToLowerInvariant();
                Directory.SetCurrentDirectory(directory);
                if (!Directory.Exists("configs"))
                {
                    Directory.CreateDirectory("configs");
                }

                if (File.Exists("configs/globalconfig.xml"))
                {
                    m_globalSettings = RbcpyGlobalSettings.Deserialize("configs/globalconfig.xml");
                }
            }
            catch
            {
                MessageBox.Show("Error loading. Please place in a writable directory.");
                m_globalSettings = new RbcpyGlobalSettings();
            }
        }

        private void chkAdvanced_CheckedChanged(object sender, EventArgs e)
        {
            panelAdvancedSettings.Visible = (sender as CheckBox).Checked;
        }

        private void CreateSyncMain_Load(object sender, EventArgs e)
        {
            // populate listbox
            this.listBoxConfigs.Items.Clear();
            foreach (var file in Directory.EnumerateFiles("configs", "*.xml"))
            {
                if (file.IndexOf("globalconfig") != -1)
                {
                    continue;
                }

                SavedConfigForListbox saved = new SavedConfigForListbox();
                saved.m_filename = file.Replace("configs\\", "").Replace("configs/", "").Replace(".xml", "");
                this.listBoxConfigs.Items.Add(saved);
            }

            this.listBoxConfigs.Items.Add(new SavedConfigForListbox());
            listBoxConfigs_SelectedIndexChanged(null, null);
        }

        private void mnuWinMergePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".exe";
            dialog.Filter = "Exe files (*.exe)|*.exe";
            dialog.Title = "Please point to WinMerge";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.m_globalSettings.m_winMergeDir = dialog.FileName;
                RbcpyGlobalSettings.Serialize(this.m_globalSettings, "configs/globalconfig.xml");
            }
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            var currentName = GetCurrentConfigName();
            if (currentName == null)
            {
                mnuSaveAs_Click(null, null);
                return;
            }

            var newFilename = "configs/" + currentName + ".xml";
            SyncConfiguration.Serialize(this.GetCurrentConfigFromUI(), newFilename);
            MessageBox.Show("Saved.");
        }

        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            var currentName = GetCurrentConfigName();
            currentName = currentName != null ? "" : currentName;
            var sNewName = InputBoxForm.GetStrInput("Choose a name:", currentName);
            if (sNewName == null) return;
            if (sNewName.Contains("\\") || sNewName.Contains("/") || sNewName.Contains("."))
            {
                MessageBox.Show("Invalid character in name.");
                return;
            }

            var newFilename = "configs/" + sNewName + ".xml";
            if (File.Exists(newFilename))
            {
                MessageBox.Show("A configuration already exists with this name.");
                return;
            }
            
            SyncConfiguration.Serialize(this.GetCurrentConfigFromUI(), newFilename);
            this.CreateSyncMain_Load(null, null);

            var justName = Path.GetFileNameWithoutExtension(newFilename);
            foreach (var item in listBoxConfigs.Items)
            {
                if ((item as SavedConfigForListbox).m_filename == justName)
                {
                    listBoxConfigs.SelectedItem = item;
                    break;
                }
            }
        }

        private void listBoxConfigs_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.panelAdvancedSettings.Visible = this.chkAdvanced.Checked;
            var name = GetCurrentConfigName();
            SetCurrentConfigToUI(this.getConfigFromFilename(name, false) ?? new SyncConfiguration());
        }

        private SyncConfiguration getConfigFromFilename(string name, bool applyVars)
        {
            if (name != null)
            {
                var newFilename = "configs/" + name + ".xml";
                if (!File.Exists(newFilename))
                {
                    MessageBox.Show("File does not exist.");
                }
                else
                {
                    return SyncConfiguration.Deserialize(newFilename, applyVars ? m_variables : null);
                }
            }

            return null;
        }

        private string GetCurrentConfigName()
        {
            if (this.listBoxConfigs.SelectedItem == null)
            {
                return null;
            }

            var selectedItem = this.listBoxConfigs.SelectedItem as SavedConfigForListbox;
            if (selectedItem == null || selectedItem.m_filename == null)
            {
                return null;
            }

            return selectedItem.m_filename;
        }

        private void mnuSwapSrcDest_Click(object sender, EventArgs e)
        {
            var temp = txtDest.Text;
            txtDest.Text = txtSrc.Text;
            txtSrc.Text = temp;
        }

        private void txtSrc_TextChanged(object sender, EventArgs e)
        {
            OnTextFieldChange(sender as TextBox, this.txtSrcShowValid);
        }

        private void txtDest_TextChanged(object sender, EventArgs e)
        {
            OnTextFieldChange(sender as TextBox, this.txtDestShowValid);
        }

        private void OnTextFieldChange(TextBox textBox, Label label)
        {
            if (Directory.Exists(RunImplementation.applyVariables(textBox.Text, m_variables)))
            {
                label.Text = "✓";
            }
            else
            {
                label.Text = "X";
            }
        }

        private void btnPreviewRun_Click(object sender, EventArgs e)
        {
            bool isPreview = true;
            var configs = new List<SyncConfiguration>();
            if (listBoxConfigs.SelectedItems.Count == 1)
            {
                configs.Add(GetCurrentConfigFromUI(true));
            }
            else if (listBoxConfigs.SelectedItems.Count > 1)
            {
                MessageBox.Show("Note: because many tasks are selected, we'll use the Saved versions.");
                isPreview = Utils.AskToConfirm("Preview before running?");
                foreach (var item in listBoxConfigs.SelectedItems) {
                    var selectedItem = item as SavedConfigForListbox;
                    if (selectedItem != null && selectedItem.m_filename != null)
                    {
                        var cfg = getConfigFromFilename(selectedItem.m_filename, true);
                        if (cfg == null)
                        {
                            return;
                        }

                        configs.Add(cfg);
                    }
                }
            }

            if (configs.Count == 0)
            {
                MessageBox.Show("No tasks selected.");
                return;
            }

            m_runner = new RunSyncOnBackgroundThread
            {
                btnToTemporarilyDisable = btnPreviewRun,
                sPreviousButtonName = "Preview / Run",
                globalSettings = m_globalSettings,
                configs = configs.ToArray(),
                preview = isPreview
            };

            m_runner.Run();
        }

        private void btnShowCmd_Click(object sender, EventArgs e)
        {
            var args = RunImplementation.Go(GetCurrentConfigFromUI(true), RunImplementation.GetLogFilename(), true, true);
            MessageBox.Show(args);
            if (Utils.AskToConfirm("Copy to clipboard?"))
            {
                Clipboard.SetText(args);
            }
        }

        private void Txt_CallSelectOnEnter(object sender, EventArgs e)
        {
            // Kick off SelectAll asyncronously so that it occurs after Click
            this.BeginInvoke((Action)delegate
            {
                (sender as TextBox).SelectAll();
            });
        }

        private void btnOpenWinmerge_Click(object sender, EventArgs e)
        {
            var config = GetCurrentConfigFromUI(true);
            if (!SyncConfiguration.Validate(config))
                return;

            RunImplementation.OpenWinmerge(m_globalSettings.m_winMergeDir, config.m_src, config.m_destination, true);
        }

        private SyncConfiguration GetCurrentConfigFromUI(bool applyVars = false)
        {
            SyncConfiguration config = new SyncConfiguration();
            Type type = config.GetType();
            FieldInfo[] properties = type.GetFields();

            foreach (FieldInfo property in properties)
            {
                if (property.Name.StartsWith("m_"))
                {
                    if (m_mapTextItems.ContainsKey(property.Name))
                    {
                        var v = m_mapTextItems[property.Name].Text;
                        v = applyVars ? RunImplementation.applyVariables(v, m_variables) : v;
                        property.SetValue(config, v);
                    }
                    else if (m_mapCheckItems.ContainsKey(property.Name))
                    {
                        property.SetValue(config, m_mapCheckItems[property.Name].Checked);
                    }
                    else
                    {
                        MessageBox.Show("unknown property:" + property.Name);
                    }
                }
            }

            return config;
        }

        private void SetCurrentConfigToUI(SyncConfiguration config)
        {
            Type type = config.GetType();
            FieldInfo[] properties = type.GetFields();
            foreach (FieldInfo property in properties)
            {
                if (property.Name.StartsWith("m_"))
                {
                    if (m_mapTextItems.ContainsKey(property.Name))
                    {
                        m_mapTextItems[property.Name].Text = (string)property.GetValue(config);
                    }
                    else if (m_mapCheckItems.ContainsKey(property.Name))
                    {
                        m_mapCheckItems[property.Name].Checked = (bool)property.GetValue(config);
                    }
                    else
                    {
                        MessageBox.Show("unknown property:" + property.Name);
                    }
                }
            }
        }

        static void Test_CheckUIElements(SyncConfiguration config, FormSyncMain form)
        {
            Utils.AssertEq(config.m_src, form.txtSrc.Text);
            Utils.AssertEq(config.m_destination, form.txtDest.Text);
            Utils.AssertEq(config.m_excludeDirs, form.txtExcludeDirs.Text);
            Utils.AssertEq(config.m_excludeFiles, form.txtExcludeFiles.Text);
            Utils.AssertEq(config.m_copyFlags, form.txtCopyFlags.Text);
            Utils.AssertEq(config.m_directoryCopyFlags, form.txtDirCopyFlags.Text);
            Utils.AssertEq(config.m_ipg, form.txtIpg.Text);
            Utils.AssertEq(config.m_nRetries, form.txtnRetries.Text);
            Utils.AssertEq(config.m_waitBetweenRetries, form.txtnWaitBetweenRetries.Text);
            Utils.AssertEq(config.m_nThreads, form.txtnThreads.Text);
            Utils.AssertEq(config.m_custom, form.txtCustom.Text);

            Utils.AssertEq(config.m_mirror, form.chkMirror.Checked);
            Utils.AssertEq(config.m_copySubDirsAndEmptySubdirs, form.chkCopySubdirs.Checked);
            Utils.AssertEq(config.m_symlinkNotTarget, form.chkSymlinkNotTarget.Checked);
            Utils.AssertEq(config.m_fatTimes, form.chkFatTimes.Checked);
            Utils.AssertEq(config.m_compensateDst, form.chkCompensateDST.Checked);
        }

        static void Test_SetUIElements(SyncConfiguration config, FormSyncMain form)
        {
            form.txtSrc.Text = config.m_src;
            form.txtDest.Text = config.m_destination;
            form.txtExcludeDirs.Text = config.m_excludeDirs;
            form.txtExcludeFiles.Text = config.m_excludeFiles;
            form.txtCopyFlags.Text = config.m_copyFlags;
            form.txtDirCopyFlags.Text = config.m_directoryCopyFlags;
            form.txtIpg.Text = config.m_ipg;
            form.txtnRetries.Text = config.m_nRetries;
            form.txtnWaitBetweenRetries.Text = config.m_waitBetweenRetries;
            form.txtnThreads.Text = config.m_nThreads;
            form.txtCustom.Text = config.m_custom;

            form.chkMirror.Checked = config.m_mirror;
            form.chkCopySubdirs.Checked = config.m_copySubDirsAndEmptySubdirs;
            form.chkSymlinkNotTarget.Checked = config.m_symlinkNotTarget;
            form.chkFatTimes.Checked = config.m_fatTimes;
            form.chkCompensateDST.Checked = config.m_compensateDst;
        }

        static void Test_CheckUIElementsAfterLoad(FormSyncMain form)
        {
            var prevConfig = form.GetCurrentConfigFromUI();
            var config1 = SyncConfiguration.Deserialize(Testing.GetTestFile("test_cfg_01.xml"));
            form.SetCurrentConfigToUI(config1);
            Test_CheckUIElements(config1, form);
            var config2 = SyncConfiguration.Deserialize(Testing.GetTestFile("test_cfg_02.xml"));
            form.SetCurrentConfigToUI(config2);
            Test_CheckUIElements(config2, form);
            form.SetCurrentConfigToUI(prevConfig);
        }

        static void Test_SaveFromUIElements(FormSyncMain form)
        {
            var prevConfig = form.GetCurrentConfigFromUI();
            var config1 = SyncConfiguration.Deserialize(Testing.GetTestFile("test_cfg_01.xml"));
            form.SetCurrentConfigToUI(config1);
            var config2 = SyncConfiguration.Deserialize(Testing.GetTestFile("test_cfg_02.xml"));
            Test_SetUIElements(config2, form);
            var config2FromForm = form.GetCurrentConfigFromUI();

            File.Delete(Testing.GetTestTempFile("test_cfg_02_got.xml"));
            SyncConfiguration.Serialize(config2FromForm, Testing.GetTestTempFile("test_cfg_02_got.xml"));
            string sExpected = File.ReadAllText(Testing.GetTestFile("test_cfg_02.xml"));
            string sGot = File.ReadAllText(Testing.GetTestTempFile("test_cfg_02_got.xml"));
            form.SetCurrentConfigToUI(prevConfig);
        }

        private void mnuSetDeletedPath_Click(object sender, EventArgs e)
        {
            var prevDir = this.m_globalSettings.m_directoryForDeletedFiles ?? "c:\\";
            var sNewName = InputBoxForm.GetStrInput("Please enter a directory where manually deleted files will be moved, or enter no text to disable this feature:", prevDir);
            if (string.IsNullOrEmpty(sNewName))
            {
                return;
            }
            else if (!Directory.Exists(sNewName))
            {
                MessageBox.Show("Directory does not exist");
                return;
            }
            else
            {
                this.m_globalSettings.m_directoryForDeletedFiles = sNewName;
                RbcpyGlobalSettings.Serialize(this.m_globalSettings, "configs/globalconfig.xml");
            }
        }

        private void btnPickSrc_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtSrc.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnPickDest_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtDest.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void FormSyncMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void FormSyncMain_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                if (filePaths.Length == 1)
                {
                    if (txtSrc.Text.StartsWith("Enter ") || txtSrc.Text.Trim().Length == 0)
                    {
                        txtSrc.Text = filePaths[0];
                    }
                    else
                    {
                        txtDest.Text = filePaths[0];
                    }
                }
            }
        }

        private void runTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RbcpyTests.RunTests();
            Test_SaveFromUIElements(this);
            Test_CheckUIElementsAfterLoad(this);
            MessageBox.Show("Tests complete.");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Rbcpy, by Ben Fisher, 2017");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuSetVariable_Click(object sender, EventArgs e)
        {
            var varName = InputBoxForm.GetStrInput("Name of variable:", "$base");
            if (!string.IsNullOrEmpty(varName) && varName.StartsWith("$"))
            {
                var val = InputBoxForm.GetStrInput("Value of variable:", "");
                if (!string.IsNullOrEmpty(val))
                {
                    this.m_variables[varName] = val;
                }
            }

            // Refresh UI
            this.listBoxConfigs_SelectedIndexChanged(null, null);
        }

        private void openConfigDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dir = Path.GetFullPath(".") + Utils.Sep + "configs";
            Utils.OpenDirInExplorer(dir);
        }
    }

    public class SavedConfigForListbox
    {
        public string m_filename;
        public override string ToString()
        {
            return m_filename != null ? m_filename : "<New>";
        }
    }

    public class RunSyncOnBackgroundThread
    {
        public Button btnToTemporarilyDisable;
        public string sPreviousButtonName;
        public RbcpyGlobalSettings globalSettings;
        public SyncConfiguration[] configs;
        public bool preview;

        public void Run()
        {
            foreach (var config in this.configs)
            {
                if (!SyncConfiguration.Validate(config))
                {
                    return;
                }
            }

            btnToTemporarilyDisable.Text = "Running...";
            btnToTemporarilyDisable.Enabled = false;
            ThreadPool.QueueUserWorkItem(delegate { RunCopyingOnSeparateThread(); }, null);
        }

        void RunCopyingOnSeparateThread()
        {
            foreach (var config in this.configs)
            {
                CCreateSyncResultsSet results = null;
                string sExceptionOccurred = null;
                try
                {
                    string sLogFilename = RunImplementation.GetLogFilename();
                    RunImplementation.Go(config, sLogFilename, preview, false);
                    results = CCreateSyncResultsSet.ParseFromLogFile(
                        config, sLogFilename, preview);
                }
                catch (Exception e)
                {
                    sExceptionOccurred = e.ToString();
                }

                Action action = delegate () { OnRunComplete(results, sExceptionOccurred); };
                btnToTemporarilyDisable.BeginInvoke(action);
            }

            // Don't restore btn text until all configs were processed
            Action restoreBtnText = delegate ()
            {
                btnToTemporarilyDisable.Text = sPreviousButtonName;
                btnToTemporarilyDisable.Enabled = true;
            };

            btnToTemporarilyDisable.BeginInvoke(restoreBtnText);
        }

        void OnRunComplete(CCreateSyncResultsSet results, string sExceptionOccurred)
        {
            if (sExceptionOccurred != null)
            {
                MessageBox.Show("Exception: " + sExceptionOccurred);
            }

            if (results != null)
            {
                var child = new FormSyncItems(results, globalSettings, preview);
                child.Show();
            }
        }
    }
}

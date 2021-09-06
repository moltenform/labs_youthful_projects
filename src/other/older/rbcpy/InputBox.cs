// Copyright (c) Ben Fisher, 2016.
// Licensed under GPLv3.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace rbcpy
{
    public sealed class InputBoxForm : Form
    {
        Container _components = null;
        Button _btnBrowse;
        Button _btnCancel;
        Button _btnOK;
        ComboBox _comboBox;
        Label _label;

        public InputBoxForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = " ";
            this.AllowDrop = true;
            this._comboBox.Focus();

            // we don't use autocomplete, we just enable it
            // to get the Ctrl+Backspace shortcut
            this._comboBox.AutoCompleteMode = AutoCompleteMode.Append;
        }

        // add MRU history, suggestions, and clipboard contents to the list of examples.
        public static IEnumerable<string> GetInputSuggestions(string currentSuggestion,
            bool useClipboard, bool mustBeDirectory, string[] more)
        {
            List<string> suggestions = new List<string>();
            if (!string.IsNullOrEmpty(currentSuggestion))
            {
                suggestions.Add(currentSuggestion);
            }

            if (useClipboard && !string.IsNullOrEmpty(Utils.GetClipboard()) &&
                Utils.LooksLikePath(Utils.GetClipboard()) == mustBeDirectory)
            {
                // get from clipboard if the right type of string (path vs not path)
                suggestions.Add(Utils.GetClipboard());
            }

            if (more != null)
            {
                suggestions.AddRange(more);
            }

            return suggestions.Where(entry => !mustBeDirectory ||
                FilenameUtils.IsPathRooted(entry));
        }

        // ask user for string input.
        public static string GetStrInput(string mesage, string currentSuggestion = null,
            string[] more = null,
            bool useClipboard = true, bool mustBeDirectory = false, bool taller = false)
        {
            using (InputBoxForm form = new InputBoxForm())
            {
                form._label.Text = mesage;
                form._btnBrowse.Visible = mustBeDirectory;
                form._btnBrowse.Click += (o, e) => form.OnBrowseClick();
                if (taller)
                {
                    form._comboBox.Top += form.Height - 40;
                    form._btnOK.Top += form.Height - 40;
                    form._btnCancel.Top += form.Height - 40;
                    form._label.Height += form.Height - 40;
                    form.Height *= 2;
                }

                // fill combo box with suggested input.
                form._comboBox.Items.Clear();
                var suggestions = GetInputSuggestions(currentSuggestion, 
                    useClipboard, mustBeDirectory, more).ToArray();

                foreach (var s in suggestions)
                {
                    form._comboBox.Items.Add(s);
                }

                form._comboBox.Text = suggestions.Length > 0 ? suggestions[0] : "";
                form.ShowDialog();
                if (form.DialogResult != DialogResult.OK)
                {
                    return null;
                }

                if (mustBeDirectory && !Directory.Exists(form._comboBox.Text))
                {
                    Utils.MessageBox("Directory does not exist");
                    return null;
                }

                return form._comboBox.Text;
            }
        }

        public static int? GetInteger(string message, int defaultInt = 0)
        {
            int fromClipboard = 0;
            var clipboardContainsInt = int.TryParse(Utils.GetClipboard(), out fromClipboard);
            string s = GetStrInput(message, defaultInt.ToString(),
                useClipboard: clipboardContainsInt);

            if (string.IsNullOrEmpty(s) || !int.TryParse(s, out int result))
            {
                return null;
            }
            else
            {
                return result;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                {
                    _components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        // originally based on
        // http://www.java2s.com/Code/CSharp/GUI-Windows-Form/
        // Defineyourowndialogboxandgetuserinput.htm
        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this._label = new System.Windows.Forms.Label();
            this._btnOK = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._comboBox = new System.Windows.Forms.ComboBox();
            this._btnBrowse = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // _label
            this._label.Location = new System.Drawing.Point(12, 8);
            this._label.Name = "_label";
            this._label.Size = new System.Drawing.Size(410, 187);
            this._label.TabIndex = 6;
            this._label.Text = "Type in your message.";

            // _btnOK
            this._btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnOK.Location = new System.Drawing.Point(259, 246);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(70, 24);
            this._btnOK.TabIndex = 2;
            this._btnOK.Text = "OK";

            // _btnCancel
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(335, 246);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(70, 24);
            this._btnCancel.TabIndex = 3;
            this._btnCancel.Text = "Cancel";

            // _comboBox
            this._comboBox.FormattingEnabled = true;
            this._comboBox.Location = new System.Drawing.Point(22, 208);
            this._comboBox.Name = "_comboBox";
            this._comboBox.Size = new System.Drawing.Size(383, 21);
            this._comboBox.TabIndex = 1;

            // _btnBrowse
            this._btnBrowse.Location = new System.Drawing.Point(183, 246);
            this._btnBrowse.Name = "_btnBrowse";
            this._btnBrowse.Size = new System.Drawing.Size(70, 24);
            this._btnBrowse.TabIndex = 3;
            this._btnBrowse.Text = "Browse...";

            // InputBoxForm
            this.AcceptButton = this._btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(434, 287);
            this.ControlBox = false;
            this.Controls.Add(this._comboBox);
            this.Controls.Add(this._btnBrowse);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputBoxForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Input Box Dialog";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.InputBoxForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.InputBoxForm_DragEnter);
            this.ResumeLayout(false);
        }
        #endregion

        private void OnBrowseClick()
        {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _comboBox.Text = dlg.SelectedPath;
            }
        }

        private void InputBoxForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        // if user drags a file onto this form, put the filepath into the combo box.
        private void InputBoxForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                string filePath = filePaths[0];
                if (!string.IsNullOrEmpty(filePath))
                {
                    _comboBox.Text = filePath;
                }
            }
        }
    }
}
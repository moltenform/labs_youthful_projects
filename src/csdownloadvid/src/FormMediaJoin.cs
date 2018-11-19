// Copyright (c) Ben Fisher, 2016.
// Licensed under GPLv3.

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CsDownloadVid
{
    public partial class FormMediaJoin : Form
    {
        RunToolHelper _runner;
        public FormMediaJoin()
        {
            InitializeComponent();
            txtInput.Text = "";
            txtInput.Enabled = false;
            txtInput.BackColor = SystemColors.Control;
            _runner = new RunToolHelper(this.txtStatus, this.lblShortStatus,
                FormAudioFromVideo.GetFfmpegStdoutFilter());
        }

        private void btnGetInput_Click(object sender, EventArgs e)
        {
            var files = Utils.AskOpenFilesDialog("Choose video files...");
            if (files != null)
            {
                txtInput.Text = string.Join(Utils.NL, files);
                txtInput.Enabled = true;
                txtInput.BackColor = SystemColors.Window;
            }
            else
            {
                txtInput.Text = "";
                txtInput.Enabled = false;
                txtInput.BackColor = SystemColors.Control;
            }
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            _runner.RunInThread(() =>
            {
                var lines = GetInputFiles(minExpected: 2);
                var outputFormat = this.tbOutputFormat.Text;
                MediaJoin(lines, outputFormat);
            });
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            RunToolHelper.RunAndCatch(() =>
            {
                _runner.CancelProcess();
            });
        }

        string EscapeStringForFfmpeg(string s)
        {
            return s.Replace("\\", "\\\\").Replace(" ", "\\ ").Replace("'", "\\'");
        }

        void MediaJoin(string[] lines, string outFormat)
        {
            var parentDirs = (from part in lines
                              select Path.GetDirectoryName(part)).Distinct();

            if (parentDirs.Count() != 1)
            {
                throw new CsDownloadVidException("Input files must be in same directory.");
            }

            var fileExts = (from part in lines select Path.GetExtension(part)).Distinct();
            if (fileExts.Count() != 1)
            {
                throw new CsDownloadVidException("Files have different extensions.");
            }

            var tmpList = parentDirs.First() + Utils.Sep + "temp_csdownloadvid_list.txt";
            if (File.Exists(tmpList))
            {
                File.Delete(tmpList);
                Utils.AssertTrue(!File.Exists(tmpList));
            }

            foreach (var part in lines)
            {
                var file = "file " + EscapeStringForFfmpeg(part) + "\n";
                File.AppendAllText(tmpList, file);
            }

            outFormat = outFormat == "auto" ? fileExts.ToArray()[0] : outFormat;
            var output = lines[0] + "_out" + outFormat;
            if (File.Exists(output))
            {
                throw new CsDownloadVidException("File already exists " + output);
            }

            var args = new List<string>();
            args.Add("-nostdin");
            args.Add("-f");
            args.Add("concat");
            args.Add("-safe"); // many versions of ffmpeg think windows full paths are unsafe
            args.Add("0"); // perhaps better to set current directory + use relative paths
            args.Add("-i");
            args.Add(tmpList);
            args.Add("-acodec");
            args.Add("copy");
            args.Add("-vcodec");
            args.Add("copy");
            args.Add(output);
            _runner.Trace("Saving to " + output);

            var info = new ProcessStartInfo();
            info.FileName = CsDownloadVidFilepaths.GetFfmpeg();
            info.Arguments = Utils.CombineProcessArguments(args.ToArray());
            info.CreateNoWindow = true;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            _runner.RunProcessSync(info, "Join Media");

            if (File.Exists(tmpList))
            {
                File.Delete(tmpList);
            }
        }

        string[] GetInputFiles(int minExpected)
        {
            var lines = Utils.SplitLines(txtInput.Text);
            if (lines.Length < minExpected)
            {
                throw new CsDownloadVidException("Expected at least " + minExpected +
                    " input files");
            }

            foreach (var line in lines)
            {
                if (!File.Exists(line))
                    throw new CsDownloadVidException("Input file " + line + " not found");
            }

            return lines;
        }
    }
}

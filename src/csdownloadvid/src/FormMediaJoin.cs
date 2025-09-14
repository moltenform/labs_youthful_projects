// Copyright (c) Ben Fisher, 2016.
// Licensed under GPLv3, refer to LICENSE for details.

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
        public FormMediaJoin(bool showCustom=false)
        {
            InitializeComponent();
            txtInput.Text = "";
            txtInput.Enabled = false;
            txtInput.BackColor = SystemColors.Control;
            _runner = new RunToolHelper(this.txtStatus, this.lblShortStatus,
                FormAudioFromVideo.GetFfmpegStdoutFilter());
            if (showCustom)
            {
                this.Text = "Encode audio or video...";
                this.label2.Text = "Choose some input files...";
                this.label1.Visible = false;
                this.tbOutputFormat.Visible = false;
            }

            this.btnJoin.Visible = !showCustom;
            this.btnMakeAudioLouder.Visible = showCustom;
            this.btnMakeAudioLouder.Visible = showCustom;
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

        private void btnMakeAudioLouder_Click(object sender, EventArgs e)
        {
            var inputs = this.GetInputFiles(1);
            ConfigKeyGetOrAskUserIfNotSet.GetOrAsk(ConfigKey.FilepathM4aEncoder);
            _runner.RunInThread(() =>
            {
                foreach (var input in inputs)
                {
                    if (input.Contains(".madelouder"))
                    {
                        _runner.Trace("skipping" + input + " because it contains .makelouder");
                    }
                    else
                    {
                        this.makeLouderOnes(input);
                    }
                }
            },
                "Making audio louder...");
        }

        private void makeLouderOnes(string input)
        {
            foreach (var scale in new List<string>(){ "2.0", "4.0", "8.0", "16.0", "32.0"})
            {
                var outfile = input + ".makelouder" + scale + ".wav";
                var args = new List<string>();
                args.Add("-nostdin");
                args.Add("-i");
                args.Add(input);
                args.Add("-filter:a");
                args.Add("volume="+scale);
                args.Add(outfile);
                var info = new ProcessStartInfo();
                info.FileName = CsDownloadVidFilepaths.GetFfmpeg();
                info.Arguments = Utils.CombineProcessArguments(args.ToArray());
                info.CreateNoWindow = true;
                info.RedirectStandardError = true;
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                _runner.RunProcessSync(info, "Make Louder");
                if (!File.Exists(outfile))
                {
                    _runner.Trace("expected to see output at " + outfile);
                    throw new Exception("expected to see output at " + outfile);
                }

                var pathOut = RunM4aConversion(outfile, "flac");
                if (new FileInfo(pathOut).Length > 1)
                {
                    File.Delete(outfile);
                }
            }
        }

        public static string RunM4aConversion(string path, string qualitySpec)
        {
            var qualities = new string[] { "16", "24", "96", "128", "144",
                "160", "192", "224", "256", "288", "320", "640", "flac" };
            if (Array.IndexOf(qualities, qualitySpec) == -1)
            {
                throw new CsDownloadVidException("Unsupported bitrate.");
            }
            else if (!path.EndsWith(".wav", StringComparison.Ordinal) &&
                !path.EndsWith(".flac", StringComparison.Ordinal))
            {
                throw new CsDownloadVidException("Unsupported input format.");
            }
            else
            {
                var encoder = Configs.Current.Get(ConfigKey.FilepathM4aEncoder);
                if (!File.Exists(encoder))
                {
                    throw new CsDownloadVidException("M4a encoder not found");
                }

                var pathOutput = Path.GetDirectoryName(path) + Utils.Sep +
                    Path.GetFileNameWithoutExtension(path) +
                    (qualitySpec == "flac" ? ".flac" : ".m4a");
                var script = Path.GetDirectoryName(encoder) + Utils.Sep +
                    "dropq" + qualitySpec + ".py";
                var args = new string[] { path };
                var stderr = Utils.RunPythonScript(
                    script, args, createWindow: false, warnIfStdErr: false);

                if (!File.Exists(pathOutput))
                {
                    Utils.MessageErr("RunM4aConversion failed, " + Utils.FormatPythonError(stderr));
                    return null;
                }
                else
                {
                    return pathOutput;
                }
            }
        }

        private void doCustomEncode(string example, InputBoxHistory key)
        {
            var files = this.GetInputFiles(1);
            var cmd = InputBoxForm.GetStrInput("Command for the ffmpeg encoder:", example,
                key);
            if (String.IsNullOrEmpty(cmd))
            {
                return;
            }
            else if (!cmd.Contains("%in%") && !Utils.AskToConfirm("Did not see '%in%', " +
                "won't process input files. Continue?"))
            {
                return;
            }

            if (!Utils.AskToConfirm("Run the command right now? (or copy the command line to the clipboard)"))
            {
                var s = "";
                foreach (var file in files)
                {
                    s += "\r\n\"" + CsDownloadVidFilepaths.GetFfmpeg() + "\" ";
                    s += cmd.Replace("%in%", files[0]);
                }

                Clipboard.SetText(s);
                MessageBox.Show("Command was copied to the clipboard.");
                return;
            }

            RunToolHelper.RunAndCatch(() =>
            {
                var infos = new List<ProcessStartInfo>();
                foreach (var file in files)
                {
                    var info = new ProcessStartInfo();
                    info.FileName = CsDownloadVidFilepaths.GetFfmpeg();
                    info.Arguments = cmd.Replace("%in%", file);
                    info.CreateNoWindow = true;
                    info.RedirectStandardError = true;
                    info.RedirectStandardOutput = true;
                    info.UseShellExecute = false;
                    infos.Add(info);
                }

                _runner.RunProcesses(infos.ToArray(), "Custom encode");
            });
        }

        private void btnEncodeAv1_Click(object sender, EventArgs e)
        {
            this.doCustomEncode("-i \"%in%\" -crf 20 -vf \"scale=iw/2:ih/2\" -c:v libaom-av1 -b:v 0 -cpu-used 4 -c:a copy \"%in%.mkv\"", InputBoxHistory.CustomEncodeAv1);
        }

        private void btnEncode_Click(object sender, EventArgs e)
        {
            this.doCustomEncode("-i \"%in%\" -c:v libx264 -crf 23 -preset slower -c:a copy \"%in%.mp4\"", InputBoxHistory.CustomEncode);
        }
    }
}

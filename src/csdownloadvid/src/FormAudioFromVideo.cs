// Copyright (c) Ben Fisher, 2016.
// Licensed under GPLv3.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace CsDownloadVid
{
    public partial class FormAudioFromVideo : Form
    {
        RunToolHelper _runner;
        public FormAudioFromVideo()
        {
            InitializeComponent();
            _runner = new RunToolHelper(this.txtStatus, this.lblShortStatus,
                GetFfmpegStdoutFilter());
        }

        private void txtInput_Click(object sender, EventArgs e)
        {
            RunToolHelper.RunAndCatch(() => btnGetInput_Click(sender, e));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            RunToolHelper.RunAndCatch(() => _runner.CancelProcess());
        }

        private void getAudio_Click(object sender, EventArgs e)
        {
            var suggestedFormat = tbOutputFormat.Text;
            RunToolHelper.RunAndCatch(() => GoExtract(true, suggestedFormat));
        }

        private void getVideo_Click(object sender, EventArgs e)
        {
            var suggestedFormat = tbOutputFormat.Text;
            RunToolHelper.RunAndCatch(() => GoExtract(false, suggestedFormat));
        }

        private void btnGetInput_Click(object sender, EventArgs e)
        {
            var files = Utils.AskOpenFilesDialog("Choose video file(s)...");
            if (files != null)
            {
                txtInput.Text = string.Join(";", files);
            }
        }

        private ProcessStartInfo MakeTask(bool audioOrVideo, string suggestedFormat, string file)
        {
            if (suggestedFormat.StartsWith("."))
            {
                // let the user type ".m4a" as well as "m4a"
                suggestedFormat = suggestedFormat.Substring(1);
            }

            Utils.AssertTrue(suggestedFormat.Length > 0, "You did not provide a suggested format.");
            var args = new List<string>();
            args.Add("-nostdin");
            args.Add("-i");
            args.Add(file);
            if (audioOrVideo)
            {
                var format = suggestedFormat == "auto" ? "m4a" : suggestedFormat;
                args.Add("-vn");
                args.Add("-acodec");
                args.Add("copy");
                args.Add(file + "_audio." + format);
            }
            else
            {
                var format = suggestedFormat == "auto" ? "m4v" : suggestedFormat;
                args.Add("-an");
                args.Add("-vcodec");
                args.Add("copy");
                args.Add(file + "_video." + format);
            }

            var info = new ProcessStartInfo();
            info.FileName = CsDownloadVidFilepaths.GetFfmpeg();
            info.Arguments = Utils.CombineProcessArguments(args.ToArray());
            info.CreateNoWindow = true;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            return info;
        }

        private ProcessStartInfo MakeTaskCombineAudioVideo(string audioFile, string videoFile,
            string output)
        {
            var args = new List<string>();
            args.Add("-nostdin");
            args.Add("-i");
            args.Add(videoFile);
            args.Add("-i");
            args.Add(audioFile);
            args.Add("-c:v");
            args.Add("copy");
            args.Add("-c:a");
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
            return info;
        }

        private void GoExtract(bool audioOrVideo, string suggestedFormat)
        {
            var files = Utils.SplitByString(txtInput.Text, ";");
            if (txtInput.Text.Trim() == "" || files.Length == 0)
            {
                MessageBox.Show("No files chosen.");
            }

            List<ProcessStartInfo> tasks = new List<ProcessStartInfo>();
            foreach (var file in files)
            {
                if (!File.Exists(file))
                {
                    MessageBox.Show("Input file " + file + "does not exist");
                    return;
                }

                if (!file.ToLowerInvariant().EndsWith(".m4a") &&
                    !file.ToLowerInvariant().EndsWith(".mp4") &&
                    !file.ToLowerInvariant().EndsWith(".m4v"))
                {
                    if (!Utils.AskToConfirm("Might not work, the file is not a m4a/mp4/m4v" +
                        "... continue?"))
                    {
                        return;
                    }
                }

                ProcessStartInfo task = MakeTask(audioOrVideo, suggestedFormat, file);
                tasks.Add(task);
            }

            _runner.RunProcesses(tasks.ToArray(), "Extract " + (audioOrVideo ? "Audio" :
                "Video"));
        }

        private void btnCombineAudioVideo_Click(object sender, EventArgs e)
        {
            var audio = Utils.AskOpenFileDialog("Choose an audio file (typically m4a or mp3)...");
            if (audio == null)
            {
                return;
            }

            var video = Utils.AskOpenFileDialog("Choose a video file (typically mp4 or mkv)...");
            if (video == null)
            {
                return;
            }

            var outFormat = Path.GetExtension(video);
            outFormat = video.EndsWith(".m4v") ? ".mp4" : outFormat;
            var output = video + "_out" + outFormat;
            if (File.Exists(output))
            {
                MessageBox.Show("File already exists " + output);
                return;
            }

            var task = MakeTaskCombineAudioVideo(audio, video, output);
            _runner.RunProcesses(new ProcessStartInfo[] { task }, "Combine as soundtrack");
        }

        public static Func<string, bool> GetFfmpegStdoutFilter()
        {
            return (s) => !s.StartsWith(" ffmpeg version") &&
                !s.StartsWith("   configuration:") &&
                !s.StartsWith("   lib") &&
                !s.StartsWith("   Metadata:") &&
                !s.StartsWith(" Press [q]") &&
                !s.StartsWith("     major_brand") &&
                !s.StartsWith("     minor_version") &&
                !s.StartsWith("     compatible_brands") &&
                !s.StartsWith("     creation_time") &&
                !s.StartsWith("     encoder ") &&
                !s.StartsWith("     Encoding Params") &&
                !s.StartsWith("     iTunSMPB") &&
                !s.StartsWith("     title ") &&
                !s.StartsWith("     artist ") &&
                !s.StartsWith("     album ") &&
                !s.StartsWith("     description ") &&
                !s.StartsWith("     comment ");
        }
    }
}

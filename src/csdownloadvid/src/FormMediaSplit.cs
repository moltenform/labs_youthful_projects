// Copyright (c) Ben Fisher, 2016.
// Licensed under GPLv3.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CsDownloadVid
{
    public partial class FormMediaSplit : Form
    {
        RunToolHelper _runner;
        public FormMediaSplit()
        {
            InitializeComponent();
            txtSplitpoints.Text = "0:20" + Utils.NL + "1:30";
            _runner = new RunToolHelper(txtStatus, lblShortStatus,
                FormAudioFromVideo.GetFfmpegStdoutFilter());
        }

        private void btnImportAudacity_Click(object sender, EventArgs e)
        {
            var file = Utils.AskOpenFileDialog("Choose Audacity text file " +
                "(File->Export labels as text)", new string[] { "*.txt" });

            if (file != null)
            {
                var lines = File.ReadAllLines(file);
                RunToolHelper.RunAndCatch(() => ImportAudacity(lines));
            }
        }

        private void btnShowSum_Click(object sender, EventArgs e)
        {
            RunToolHelper.RunAndCatch(() =>
            {
                var splitPoints = GetSplitTimes(txtSplitpoints.Text);
                double total = splitPoints.Sum();
                double subseconds = total - (int)total;
                int seconds = ((int)total) % 60;
                int minutes = ((int)total) / 60;
                MessageBox.Show("Sum of all times is " + minutes + ":" +
                    seconds + subseconds.ToString("#.0000"));
            });
        }

        private void btnGetInput_Click(object sender, EventArgs e)
        {
            var file = Utils.AskOpenFileDialog("Choose video file(s)...");
            txtInput.Text = file ?? "";
            txtInput.Visible = true;
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            RunToolHelper.RunAndCatch(() =>
            {
                var input = txtInput.Text;
                var times = GetSplitTimes(txtSplitpoints.Text);
                if (times.Count == 0)
                {
                    throw new CsDownloadVidException("No split points added.");
                }
                else if (!File.Exists(input))
                {
                    throw new CsDownloadVidException("Input file not found");
                }

                if (!(input.EndsWith(".m4a") || input.EndsWith(".m4v")))
                {
                    if (!Utils.AskToConfirm("Warning: designed for .m4a or .m4v files, " +
                        "this might not work. Continue?"))
                    {
                        return;
                    }
                }

                if (checkBoxFadeout.Checked)
                {
                    SplitWithFadeout(input, times, txtFadeLength.Text);
                }
                else
                {
                    SplitMedia(input, times);
                }
            });
        }

        private void ImportAudacity(string[] lines)
        {
            txtSplitpoints.Text = "";
            foreach (var line in lines)
            {
                if (line.Trim().Length > 0)
                {
                    if (line.Contains("\t"))
                    {
                        var beforeTab = Utils.SplitByString(line, "\t")[0];
                        txtSplitpoints.AppendText(Utils.NL + beforeTab);
                    }
                    else
                    {
                        throw new CsDownloadVidException("expected lines to be in the form " +
                            "Number(tab)Label, but got " + line);
                    }
                }
            }
        }

        private void SplitWithFadeout(string inputFile, List<double> splitPoints,
            string sFadeLength)
        {
            const int sampleRate = 44100;
            string outFilename = inputFile + "_fadeout.m4a";
            if (!double.TryParse(sFadeLength, out double fadeLength) || fadeLength <= 0)
            {
                throw new CsDownloadVidException("Invalid fadelength, expected a number of " +
                    "seconds like 4");
            }
            else if (splitPoints.Count == 0)
            {
                throw new CsDownloadVidException("Enter a time, in seconds");
            }
            else if (splitPoints.Count != 1)
            {
                throw new CsDownloadVidException("It looks like you have entered more than " +
                    "one time point. Please enter just one time, in seconds.");
            }
            else if (!inputFile.EndsWith(".m4a"))
            {
                throw new CsDownloadVidException("We currently only support adding fadeout " +
                    "for m4a files (if you have a .mp4 song, please rename it to .m4a first).");
            }
            else if (File.Exists(outFilename))
            {
                throw new CsDownloadVidException("Output file already exists " + outFilename);
            }

            // preemptively make sure we have a path to qaac.
            CsDownloadVidFilepaths.GetQaac();

            // run all in a separate thread, so that UI remains responsive.
            _runner.RunInThread(() =>
            {
                var log = "";
                new AddFadeoutUsingRawAacData().Go(inputFile, sampleRate, splitPoints[0],
                    fadeLength, outFilename, ref log);

                _runner.TraceFiltered(log.Replace("\n", Utils.NL));
                _runner.Trace(File.Exists(outFilename) ? "Successfully saved to " + outFilename :
                    "Error(s) occurred");
            });
        }

        private void SplitMedia(string inputFile, List<double> splitPoints)
        {
            const int maxLenSeconds = 9999;
            splitPoints.Insert(0, 0.0);
            var startingPoints = new List<double>();
            var lengths = new List<double>();
            for (int i = 0; i < splitPoints.Count; i++)
            {
                var start = splitPoints[i];
                double length = 0;
                if (i < splitPoints.Count - 1)
                {
                    var end = splitPoints[i + 1];
                    length = end - start;
                }
                else
                {
                    // just get all the rest
                    length = maxLenSeconds;
                }

                if (start > maxLenSeconds || length > maxLenSeconds)
                {
                    throw new CsDownloadVidException("we don't currently support very long " +
                        "files longer than " + maxLenSeconds + " seconds");
                }

                startingPoints.Add(start);
                lengths.Add(length);
            }

            // run all in a separate thread, so that UI remains responsive.
            _runner.RunInThread(() =>
            {
                var log = "";
                for (int i = 0; i < startingPoints.Count; i++)
                {
                    new AddFadeoutUsingRawAacData().SplitOneFileSynchronous(
                        inputFile, startingPoints[i], lengths[i], i, ref log);
                }

                _runner.TraceFiltered(log);
                _runner.Trace("Done");
            });
        }

        string SecondsToHoursMinutesSeconds(double fSeconds, bool largestIsMinutes)
        {
            if (largestIsMinutes)
            {
                // an easy way to show minutes instead of hours
                fSeconds *= 60;
            }

            Utils.AssertTrue(fSeconds >= 0);
            var ms = fSeconds - (int)fSeconds;
            var msRounded = (int)(1000 * ms);
            int nSeconds = (int)(fSeconds);
            int secsRounded = nSeconds % 60;
            int minRounded = (nSeconds / 60) % 60;
            int hrRounded = (nSeconds / (60 * 60));
            return string.Format("{0}:{1}:{2}.{3}",
                hrRounded.ToString("D2"),
                minRounded.ToString("D2"), secsRounded.ToString("D2"),
                ms.ToString().Replace("0.", ""));
        }

        double ParseTimeFromText(string text)
        {
            var parts = text.Split(new char[] { ':' });
            if (parts.Length == 1)
            {
                return double.Parse(parts[0]);
            }
            else if (parts.Length == 2)
            {
                return 60 * double.Parse(parts[0]) + double.Parse(parts[1]);
            }
            else
            {
                throw new CsDownloadVidException("we currently require either (seconds) or " +
                    "(minutes):(seconds) form.");
            }
        }

        List<double> GetSplitTimes(string text)
        {
            List<double> times = new List<double>();
            foreach (var line in Utils.SplitLines(text))
            {
                times.Add(ParseTimeFromText(line));
                if (times[times.Count - 1] <= 0)
                {
                    throw new CsDownloadVidException("Invalid time, must be > 0.0 seconds");
                }
            }

            return times;
        }

        private void btnToMp3DirectCut_Click(object sender, EventArgs e)
        {
            var output = Utils.AskSaveFileDialog("Save .cue file:", new string[] { "*.cue" });
            if (!string.IsNullOrEmpty(output))
            {
                var times = GetSplitTimes(txtSplitpoints.Text);
                var result = ToCueFile(times);
                File.WriteAllText(output, result);
            }
        }

        string ToCueFile(List<double> times)
        {
            // create a file like this:
            // TITLE "example"
            // FILE "example.mp3" MP3
            //   TRACK 01 AUDIO
            //     TITLE "(Track 01)"
            //     INDEX 01 00:00:00
            var result = new List<string>();
            result.Add("TITLE \"example\"");
            result.Add("FILE \"example.mp3\" MP3");
            result.Add("  TRACK 01 AUDIO");
            result.Add("    TITLE \"(Track 01)\"");
            result.Add("    INDEX 01 00:00:00");
            for (int i = 0; i < times.Count; i++)
            {
                ToCueFileTime(result, i, times[i]);
            }

            return string.Join("\n", result);
        }

        void ToCueFileTime(List<string> result, int i, double time)
        {
            var fmtTime = SecondsToHoursMinutesSeconds(time, largestIsMinutes:true);
            var num = (i + 2).ToString("D2");
            result.Add(string.Format("  TRACK {0} AUDIO", num));
            result.Add(string.Format("    TITLE \"(Track {0})\"", num));
            result.Add(string.Format("    INDEX 01 {0}", fmtTime));
        }
    }
}

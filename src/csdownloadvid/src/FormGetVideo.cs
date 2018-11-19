// Copyright (c) Ben Fisher, 2016.
// Licensed under GPLv3.

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CsDownloadVid
{
    public partial class FormGetVideo : Form
    {
        RunToolHelper _runner;
        Dictionary<string, string> _formatsGeneric = new Dictionary<string, string>
        {
            { "best audio+video", "best" },
            { "worst audio+video", "worst" },
            { "best audio", "bestaudio" },
            { "worst audio", "worstaudio" },
            { "best m4a", "m4a" },
            { "best mp4", "mp4" },
            { "best aac", "aac" },
            { "best webm", "webm" },
        };

        public FormGetVideo()
        {
            InitializeComponent();
            panelAdvanced.Visible = chkShowAdvanced.Checked;
            lblEnterUrlsAdvanced.Visible = chkShowAdvanced.Checked;
            panelChooseQuality.Visible = false;
            panelChooseOutput.Visible = false;
            btnNextStepIsToChooseOutput.Enabled = false;
            lblShortStatus.Text = "";
            txtStatus.Visible = false;

            AddGenericFormatsToListbox();
            txtOutputDir.Text = CsDownloadVidFilepaths.GetDefaultDownloadsDir();
            lblNamePattern.Text = "Filename pattern, see also" + Utils.NL +
                "%(upload_date)s";
            _runner = new RunToolHelper(this.txtStatus, this.lblShortStatus,
                (line) => (!line.Contains("[download]")));

            // pre-emptively ensure that we have paths
            var ensurePath = CsDownloadVidFilepaths.GetPython();
        }

        private void NextStepIsToChooseQuality()
        {
            if (CsDownloadVidFilepaths.GetYtdlPath(cbUsePytube.Checked, 
                required: false) == null)
            {
                return;
            }

            listBoxFmts.Items.Clear();
            AddGenericFormatsToListbox();
            GetOptionsFromUI(out List<string> urlsRet, out int waitBetween,
                out string filenamePattern, out string outDir);

            // look up formats
            txtStatus.Visible = true;
            lblShortStatus.Visible = true;
            panelChooseQuality.Visible = true;
            btnNextStepIsToChooseOutput.Enabled = false;
            btnNextStepIsToChooseOutput.Text = "Looking up formats...";
            var urlToGet = urlsRet[0];
            var info = GetStartInfo(urlToGet, "", true);

            // run all in a separate thread, so that UI remains responsive.
            _runner.RunInThread(() =>
            {
                LoadFormats_StartProc(urlToGet, info);
            });
        }

        private void LoadFormats_StartProc(string urlToGet, ProcessStartInfo info)
        {
            string log = "";
            string stderr = "";
            string stdout = "";
            bool succeeded = false;

            Process p = new Process();
            p.StartInfo = info;
            p.Start();
            p.ErrorDataReceived += (o, eparam) => { stderr += eparam.Data; };
            p.BeginErrorReadLine();
            p.WaitForExit();
            log += "\nRan " + info.FileName + " " + info.Arguments;
            stdout = p.StandardOutput.ReadToEnd();
            if (p.ExitCode == 0)
            {
                succeeded = true;
            }
            else
            {
                log += "\nStdout:" + stdout;
                log += "\nStderr:" + stderr;
            }

            _runner.TraceFiltered(log.Replace("\n", Utils.NL));
            if (succeeded)
            {
                LoadFormats_ToUI(urlToGet, stdout);
            }
            else
            {
                Utils.MessageErr("Could not get formats");
                _runner.Trace("Could not get formats");
            }
        }

        private void LoadFormats_ToUI(string urlToGet, string stdout)
        {
            _runner.Trace("Done getting formats");
            this.Invoke(new Action(() =>
            {
                List<ListBoxItemFormat> items = LoadFormats_Parse(stdout, urlToGet);
                listBoxFmts.Items.Clear();
                if (items != null)
                {
                    listBoxFmts.Items.AddRange(items.ToArray());
                }

                AddGenericFormatsToListbox();
                btnNextStepIsToChooseOutput.Text = "Go to next step";
                btnNextStepIsToChooseOutput.Enabled = true;
            }));
        }

        void AddGenericFormatsToListbox()
        {
            if (!cbUsePytube.Checked)
            {
                var formats = from key in _formatsGeneric.Keys orderby key select key;
                foreach (var format in formats)
                {
                    var item = new ListBoxItemFormat();
                    item._displayText = format;
                    listBoxFmts.Items.Add(item);
                }
            }
        }

        List<ListBoxItemFormat> LoadFormats_Parse(string result, string url)
        {
            var results = new List<ListBoxItemFormat>();
            bool isYoutube = url.Contains("youtube");
            var resultLines = Utils.SplitLines(result);
            var containsAvailFormats = resultLines.Any(
                (s) => s.StartsWith("[info] Available formats for "));
            if (!containsAvailFormats)
            {
                throw new CsDownloadVidException("did not see any line with " +
                    "'[info] Available formats'. result was " + result);
            }

            if (cbUsePytube.Checked)
            {
                foreach (var line in resultLines)
                {
                    if (line.StartsWith("<Stream:"))
                    {
                        ListBoxItemFormat item = new ListBoxItemFormat();
                        item._displayText = line;
                        results.Add(item);
                    }
                }

                return results;
            }

            bool shouldIncludeLine = false;
            foreach (var line in resultLines)
            {
                if (!shouldIncludeLine && line.Replace(" ", "").StartsWith(
                    "format code  extension resolution".Replace(" ", "")))
                {
                    shouldIncludeLine = true;
                }
                else if (shouldIncludeLine)
                {
                    ParseFormat(results, line, isYoutube);
                }
            }

            if (results.Count == 0)
            {
                throw new CsDownloadVidException("Did not see any formats... stdout was " +
                    result);
            }

            results.Sort();
            return results;
        }

        private void ParseFormat(List<ListBoxItemFormat> results, string line, bool isYoutube)
        {
            var parts = line.Split(new string[] { "          " }, count: 2,
                options: StringSplitOptions.None);
            var firstNumber = parts[0];
            if (int.TryParse(firstNumber, out int nOut))
            {
                parts[1] = parts[1].Trim();
                parts[1] = parts[1].Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                if (parts[1].Contains("audio only"))
                    parts[1] = "audio: " + parts[1].Replace("audio only", "");
                else if (parts[1].Contains("video only"))
                    parts[1] = "video: " + parts[1].Replace("video only", "");
                else
                    parts[1] = "a+v: " + parts[1];

                parts[1] = parts[1].Replace(" , ", "");

                ListBoxItemFormat item = new ListBoxItemFormat();
                item._formatNumber = nOut;
                item._displayText = parts[1];
                results.Add(item);
            }
            else if (!isYoutube)
            {
                if (line.Contains("   "))
                {
                    ListBoxItemFormat item = new ListBoxItemFormat();
                    item._displayText = line.Trim();
                    results.Add(item);
                }
            }
            else if (line.Contains("   "))
            {
                _runner.Trace("ignoring line " + line);
            }
        }

        private void chkShowAdvanced_CheckedChanged(object sender, EventArgs e)
        {
            panelAdvanced.Visible = chkShowAdvanced.Checked;
            lblEnterUrlsAdvanced.Visible = chkShowAdvanced.Checked;
        }

        private void btnNextToChooseQuality_Click(object sender, EventArgs e)
        {
            RunToolHelper.RunAndCatch(() => NextStepIsToChooseQuality());
        }

        private void btnSaveTo_Click(object sender, EventArgs e)
        {
            var outDir = Utils.AskSaveFileDialog("Save to?", new string[] { ".mp4" },
                null, txtOutputDir.Text);

            outDir = Path.GetDirectoryName(outDir);
            if (Directory.Exists(outDir))
            {
                txtOutputDir.Text = outDir;
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            RunToolHelper.RunAndCatch(() =>
            {
                if (chkDashToM4a.Checked)
                {
                    CsDownloadVidFilepaths.GetFfmpeg();
                }

                GetOptionsFromUI(out List<string> urlsRet, out int waitBetween,
                    out string filenamePattern, out string outDir);

                var format = GetChosenFormat();
                if (string.IsNullOrEmpty(format))
                {
                    throw new CsDownloadVidException("No format chosen.");
                }
                else if (cbUsePytube.Checked != format.StartsWith("<Stream:"))
                {
                    throw new CsDownloadVidException("It looks like you've clicked " +
                        "'use pytube instead of ytdl' half-way through-- when you change " +
                        "this you have to start over at step 1.");
                }

                Configs.Current.Set(ConfigKey.SaveVideosTo, txtOutputDir.Text);
                _runner.SetWaitBetween(waitBetween);
                List<ProcessStartInfo> listInfos = new List<ProcessStartInfo>();
                foreach (var url in urlsRet)
                {
                    listInfos.Add(GetStartInfo(url, format, false));
                }

                _runner.RunProcesses(listInfos.ToArray(), "downloading");
            });
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            RunToolHelper.RunAndCatch(() => _runner.CancelProcess());
        }

        string GetUrlFromUrlFile(string sFile)
        {
            var lines = File.ReadAllLines(sFile);
            foreach (var line in lines)
            {
                if (line.StartsWith("URL="))
                {
                    return line.Substring("URL=".Length);
                }
            }

            throw new CsDownloadVidException("url not found in file " + sFile);
        }

        private void GetUrlsFromTextFile(string filename, List<string> results)
        {
            var all = File.ReadAllLines(filename);
            foreach (var line in all)
            {
                if (line.StartsWith("http"))
                {
                    results.Add(line);
                }
            }
        }

        public void GetOptionsFromUI(out List<string> urlsRet, out int waitBetween,
            out string filenamePattern, out string outDir)
        {
            urlsRet = new List<string>();
            if (!Directory.Exists(txtOutputDir.Text))
            {
                throw new CsDownloadVidException("Directory does not exist: " +
                    txtOutputDir.Text);
            }

            outDir = txtOutputDir.Text;

            if (!double.TryParse(txtWaitBetween.Text, out double dwaitBetween))
            {
                throw new CsDownloadVidException("Invalid wait between: " +
                    txtWaitBetween.Text);
            }

            waitBetween = (int)(1000 * double.Parse(txtWaitBetween.Text));

            if (!txtFilenamePattern.Text.Contains("%(title)") &&
                !txtFilenamePattern.Text.Contains("%(id)"))
            {
                throw new CsDownloadVidException("filename pattern should have at " +
                    "least %(title) or %(id) but got: " + txtFilenamePattern.Text);
            }

            filenamePattern = txtFilenamePattern.Text;

            var url = txtUrl.Text;
            if (url.StartsWith("http"))
            {
                var urlsTry = url.Split(new char[] { '|' });
                foreach (var one in urlsTry)
                {
                    if (!one.StartsWith("http"))
                    {
                        throw new CsDownloadVidException("Reading multiple urls separated " +
                            "by |. Why does one of the urls not start with http? Got " + one);
                    }
                    urlsRet.Add(one);
                }
            }
            else if (Directory.Exists(url))
            {
                var urlLinks = Directory.GetFiles(url, "*.url");
                if (urlLinks.Length == 0)
                {
                    throw new CsDownloadVidException("no .url files seen in this directory " +
                        url);
                }
                else if (urlLinks.Length > 1 && Utils.AskToConfirm("Found " + urlLinks.Length +
                    " .url files. Continue?"))
                {
                    foreach (var link in urlLinks)
                        urlsRet.Add(GetUrlFromUrlFile(link));
                }
            }
            else if (File.Exists(url) && url.ToLowerInvariant().EndsWith(".url"))
            {
                urlsRet.Add(GetUrlFromUrlFile(url));
            }
            else if (File.Exists(url) && url.ToLowerInvariant().EndsWith(".txt"))
            {
                GetUrlsFromTextFile(url, urlsRet);
            }
            else
            {
                throw new CsDownloadVidException("unrecognized input " + url);
            }
        }

        public string GetChosenFormat()
        {
            if (listBoxFmts.Items.Count == 0 || listBoxFmts.SelectedIndex == -1)
                return null;

            var item = listBoxFmts.SelectedItem as ListBoxItemFormat;
            if (cbUsePytube.Checked)
            {
                return item._displayText;
            }
            if (item._formatNumber == -1)
            {
                if (_formatsGeneric.ContainsKey(item._displayText))
                    return _formatsGeneric[item._displayText];
                else return item._displayText.Split(new string[] { " " },
                    StringSplitOptions.None)[0];
            }
            else
            {
                return item._formatNumber.ToString();
            }
        }

        ProcessStartInfo GetStartInfo(string url, string format, bool listSupportedFormatsOnly)
        {
            var args = new List<string>();
            args.Add("--ignore-config"); // don't look for global config file
            args.Add("--no-mark-watched");
            args.Add("--no-call-home");
            args.Add("--no-mtime"); // don't adjust the lmt of the file, it's confusing
            args.Add("--no-playlist");

            if (listSupportedFormatsOnly)
            {
                args.Add("--list-formats");
                args.Add("--simulate");
            }
            else
            {
                args.Add("--format");
                args.Add(format);

                // post-process, otherwise the m4a won't show correctly in some media players
                if (chkDashToM4a.Checked)
                {
                    args.Add("--ffmpeg-location");
                    args.Add(CsDownloadVidFilepaths.GetFfmpeg());
                }
            }

            if (cbUsePytube.Checked)
            {
                args.Add("--outputdir=" + txtOutputDir.Text);
            }
            else
            {
                var outputTemplate = Path.Combine(txtOutputDir.Text, txtFilenamePattern.Text);
                args.Add("--output");
                args.Add(outputTemplate);
            }

            var sArgs = Utils.CombineProcessArguments(args.ToArray());
            if (!string.IsNullOrWhiteSpace(txtAdditionalArgs.Text))
            {
                sArgs += " " + txtAdditionalArgs.Text + " ";
            }

            sArgs += " " + Utils.CombineProcessArguments(new string[] { url });

            var info = new ProcessStartInfo();
            info.FileName = CsDownloadVidFilepaths.GetYtdlPath(this.cbUsePytube.Checked);
            info.Arguments = sArgs;
            info.CreateNoWindow = true;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;

            if (CsDownloadVidFilepaths.GetYtdlPath(this.cbUsePytube.Checked).EndsWith(".py"))
            {
                info.FileName = CsDownloadVidFilepaths.GetPython();
                info.Arguments = "\"" + CsDownloadVidFilepaths.GetYtdlPath(
                    this.cbUsePytube.Checked) + "\" " + sArgs;
            }

            return info;
        }

        private void btnGetUpdates_Click(object sender, EventArgs e)
        {
            RunToolHelper.RunAndCatch(() =>
            {
                _runner.RunInThread(() =>
                {
                    GetUpdatesImpl();
                },
                "Getting new youtube-dl...");
            });
        }

        private void GetUpdatesImpl()
        {
            _runner.Trace("Getting new youtube-dl...");
            if (cbUsePytube.Checked)
            {
                new DownloadLatestPytube().Go(_runner);
            }
            else
            {
                new DownloadLatestYtdl().Go(_runner);
            }

            _runner.Trace("Done getting new youtube-dl.");
        }

        private void btnNextStepIsToChooseOutput_Click(object sender, EventArgs e)
        {
            panelChooseOutput.Visible = true;
            txtStatus.Visible = true;
        }

        private static void GetPlaylistImpl(string url, string txtpath, RunToolHelper runner)
        {
            bool isPytube = false; // use only ytdl, not pytube
            var args = new List<string>();
            args.Add("--ignore-config"); // don't look for global config file
            args.Add("--no-mark-watched");
            args.Add("--no-call-home");
            args.Add("-j"); // send output in json format
            args.Add("--flat-playlist");
            args.Add("-i"); // continue after errs
            args.Add(url);

            var info = new ProcessStartInfo();
            info.FileName = CsDownloadVidFilepaths.GetYtdlPath(isPytube);
            info.Arguments = Utils.CombineProcessArguments(args.ToArray());
            info.CreateNoWindow = true;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;

            if (CsDownloadVidFilepaths.GetYtdlPath(isPytube).EndsWith(".py"))
            {
                info.FileName = CsDownloadVidFilepaths.GetPython();
                info.Arguments = "\"" + CsDownloadVidFilepaths.GetYtdlPath(isPytube) + "\" " +
                    Utils.CombineProcessArguments(args.ToArray());
            }

            var stdoutGot = "";
            Process p = new Process();
            p.StartInfo = info;
            p.Start();
            p.OutputDataReceived += (o, eparam) => { stdoutGot += eparam.Data; };
            p.BeginOutputReadLine();
            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                throw new CsDownloadVidException("error - non-zero exit code of " + p.ExitCode);
            }

            stdoutGot = stdoutGot.Trim();
            if (!stdoutGot.StartsWith("{"))
            {
                throw new CsDownloadVidException("error - did not get valid json back " +
                    stdoutGot);
            }

            GetPlaylistImplFromJson(runner, url, stdoutGot, txtpath);
        }

        private static void GetPlaylistImplFromJson(RunToolHelper runner, string url,
            string json, string txtPath)
        {
            // we could also use JsonReaderWriterFactory instead
            runner.Trace("process returned successfully. parsing json...");
            var parts = new List<string>(Utils.SplitByString(json, "\"id\": \""));
            parts.RemoveAt(0);
            var linesOut = new List<string>();
            foreach (var part in parts)
            {
                var proposedId = Utils.SplitByString(part, "\"")[0];
                if (proposedId.Length >= 10 && proposedId.Length <= 12 &&
                    !proposedId.Contains(" "))
                {
                    linesOut.Add("https://www.youtube.com/watch?v=" + proposedId);
                    runner.Trace("Found " + "https://www.youtube.com/watch?v=" + proposedId);
                }
            }

            if (linesOut.Count == 0)
            {
                throw new CsDownloadVidException("did not find any videos in this playlist.");
            }

            runner.Trace("Successfully found " + linesOut.Count + " ids.");
            File.WriteAllLines(txtPath, linesOut);
        }

        private void btnGetPlaylist_Click(object sender, EventArgs e)
        {
            var url = txtUrl.Text;
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("no url entered. please enter one of the url with a " +
                    "playlist in the box for 'step 1'.");
                return;
            }
            else if (!Utils.AskToConfirm("Get playlist for " + url + "?"))
            {
                return;
            }

            var txtPath = Utils.AskSaveFileDialog("Save playlist video ids to what text file?",
                new string[] { ".txt" }, new string[] { "Text file" });

            if (string.IsNullOrEmpty(txtPath))
            {
                return;
            }

            RunToolHelper.RunAndCatch(() =>
            {
                _runner.RunInThread(() =>
                {
                    GetPlaylistStart(url, txtPath);
                },
                "Getting playlist...");
            });
        }

        private void GetPlaylistStart(string url, string txtPath)
        {
            _runner.Trace("Getting playlist...");
            _runner.Trace("url=" + url);
            GetPlaylistImpl(url, txtPath, _runner);

            this.Invoke(new Action(() =>
            {
                _runner.Trace("Done getting playlist.");
                MessageBox.Show("We saved the video ids in the playlist to " +
                    txtPath + ". You can now put this path in instead of a URL " +
                    "and hit download.");
                txtUrl.Text = txtPath;
            }));
        }

        private void btnEncode_Click(object sender, EventArgs e)
        {
            var files = Utils.AskOpenFilesDialog("Input file(s):");
            if (files == null || files.Length == 0)
            {
                return;
            }

            var example = "-i \"%in%\" -c:v libx264 -crf 23 -preset slower \"%in%.mp4\"";
            var cmd = InputBoxForm.GetStrInput("Command for the ffmpeg encoder:", example,
                InputBoxHistory.CustomEncode);
            if (String.IsNullOrEmpty(cmd))
            {
                return;
            }
            else if (!cmd.Contains("%in%") && !Utils.AskToConfirm("Did not see '%in%', " +
                "won't process input files. Continue?"))
            {
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
    }

    public class ListBoxItemFormat : IComparable<ListBoxItemFormat>
    {
        // -1 for generic like 'best', 'worst'
        public int _formatNumber = -1;
        public string _displayText;

        public int CompareTo(ListBoxItemFormat other)
        {
            return _displayText.CompareTo(other._displayText);
        }

        public override string ToString()
        {
            if (_formatNumber == -1)
                return _displayText;

            return _displayText.Insert(_displayText.IndexOf(", ") + 2, "(" +
                _formatNumber.ToString() + ") ");
        }
    }
}

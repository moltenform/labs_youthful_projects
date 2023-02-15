// Copyright (c) Ben Fisher, 2016.
// Licensed under GPLv3.

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace CsDownloadVid
{
    // Run tasks in a background thread
    public class RunToolHelper
    {
        TextBox _tb;
        Label _shortLabel;
        readonly Func<string, bool> _filterOutput;
        readonly object _lock = new object();
        int _waitBetweenMs = 0;
        bool _showOutputSooner = false;
        volatile Process _process;
        volatile bool _cancelRequested = false;
        public RunToolHelper(TextBox tb, Label shortlabel,
            Func<string, bool> filter,
            bool showOutputSooner = false)
        {
            _tb = tb;
            _shortLabel = shortlabel;
            _showOutputSooner = showOutputSooner;
            _filterOutput = filter;
        }

        public static string ExceptionToString(Exception e)
        {
            if (e is CsDownloadVidException eOurs)
                return eOurs.Message;
            else if (e == null)
                return "(null)";
            else
                return e.ToString();
        }

        public static void RunAndCatch(Action fn)
        {
            try
            {
                fn.Invoke();
            }
            catch (Exception e)
            {
                Utils.MessageErr(ExceptionToString(e));
            }
        }

        public void Trace(string txt, bool alert = false)
        {
            Utils.AssertTrue(_tb != null, "tb is null");
            _tb.BeginInvoke((MethodInvoker)(() =>
            {
                if (alert)
                {
                    MessageBox.Show(txt);
                }
                else
                {
                    _tb.AppendText(Utils.NL);
                    _tb.AppendText(txt);
                }
            }));
        }

        public void TraceFiltered(string txt)
        {
            // consumer can provide a "filter" callback to determine what is logged
            var lines = Utils.SplitLines(txt);
            foreach (var line in lines)
            {
                if (_filterOutput == null || _filterOutput(line))
                {
                    Trace(line);
                }
            }
        }

        public void SetWaitBetween(int n)
        {
            _waitBetweenMs = n;
        }

        private void _process_OnDataReceived(object o, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                var s = Utils.NL + "output: " + e.Data;
                if (_showOutputSooner)
                {
                    _tb.Invoke((MethodInvoker)(() =>
                    {
                        _tb.AppendText(s);
                    }));
                }
                else
                {
                    _tb.BeginInvoke((MethodInvoker)(() =>
                    {
                        _tb.AppendText(s);
                    }));
                }
            }
        }

        public void RunProcesses(ProcessStartInfo[] infos, string actionName, Action doAfterRun=null)
        {
            RunInThread(() =>
            {
                _cancelRequested = false;
                for (int i = 0; i < infos.Length; i++)
                {
                    _tb.Invoke((MethodInvoker)(() =>
                    {
                        _shortLabel.Text = "Task " + (i + 1) + " of " + infos.Length + "...";
                    }));

                    if (_cancelRequested)
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(_waitBetweenMs);
                        RunProcessSync(infos[i], actionName);
                    }
                }

                _tb.Invoke((MethodInvoker)(() =>
                {
                    doAfterRun?.Invoke();
                    _shortLabel.Text = _cancelRequested ? "Canceled" : "Complete";
                }));
            });
        }

        public void RunProcessSync(ProcessStartInfo info, string actionName,
            bool prioritizeStdErr = true)
        {
            _cancelRequested = false;
            try
            {
                _process = new Process();
                _process.StartInfo = info;
                _process.Start();
                _process.OutputDataReceived += _process_OnDataReceived;
                if (prioritizeStdErr)
                {
                    _process.BeginErrorReadLine();
                }
                else 
                {
                    _process.BeginOutputReadLine();
                }

                _process.WaitForExit();
                Trace("Running: " + info.FileName + " " + info.Arguments);
                if (_process.ExitCode != 0)
                {
                    Trace("warning: exited with code " + _process.ExitCode, true);
                }

                var stm = prioritizeStdErr ? _process.StandardOutput : _process.StandardError;
                using (StreamReader reader = stm)
                {
                    string stdout = reader.ReadToEnd();
                    TraceFiltered(stdout);
                }
            }
            catch (Exception e)
            {
                Utils.MessageErr(ExceptionToString(e));
            }
            finally
            {
                if (_process != null)
                {
                    _process.Close();
                    _process.Dispose();
                    _process = null;
                }
            }

            Trace(actionName + " " + (_cancelRequested ? "Canceled" : "Complete"));
        }

        public void RunProcess(ProcessStartInfo info, string actionName,
            bool prioritizeStdErr = true)
        {
            RunInThread(() =>
            {
                RunProcessSync(info, actionName, prioritizeStdErr);
            });
        }

        public void RunInThread(Action fn, string setStatus = null)
        {
            _cancelRequested = false;
            ThreadPool.QueueUserWorkItem(delegate
            {
                var locker = new LockCloser(_lock);
                using (locker)
                {
                    if (locker.TryEnter())
                    {
                        if (setStatus != null)
                        {
                            this._shortLabel.Invoke((MethodInvoker)
                                (() => this._shortLabel.Text = setStatus));
                        }

                        try
                        {
                            fn.Invoke();
                        }
                        catch (Exception e)
                        {
                            Utils.MessageErr(ExceptionToString(e));
                        }

                        if (setStatus != null)
                        {
                            this._shortLabel.Invoke((MethodInvoker)
                                (() => this._shortLabel.Text = "Done"));
                        }
                    }
                    else
                    {
                        Trace("Can't do this yet, since there's already an operation " +
                            "in progress.", true);
                    }
                }
            });
        }

        public void CancelProcess()
        {
            _cancelRequested = true;
            if (_process != null)
            {
                try
                {
                    _process.Kill();
                    Trace("Process canceled.");
                }
                catch (Exception e)
                {
                    Utils.MessageErr(ExceptionToString(e));
                }
            }
            else
            {
                Trace("Could not cancel process, maybe nothing is running.");
            }
        }
    }

    public static class CsDownloadVidFilepaths
    {
        public static string GetPython()
        {
            var hints = new string[] { @"C:\python33\python.exe", @"C:\python34\python.exe",
                @"C:\python35\python.exe", @"C:\python36\python.exe",
                @"C:\python37\python.exe" };

            return GetPathToBinary(ConfigKey.PathToPython, "Python 3 (python.exe)", hints);
        }

        public static string GetFfmpeg()
        {
            var hints = new string[] { @".\tools\ffmpeg.exe", @".\ffmpeg.exe", @"..\ffmpeg.exe",
                @"..\..\ffmpeg.exe" };

            return GetPathToBinary(ConfigKey.PathToFfmpeg, "FFmpeg (FFmpeg.exe)", hints);
        }

        public static string GetQaac()
        {
            var hints = new string[] { @".\tools\qaac.exe", @".\qaac.exe", @"..\qaac.exe",
                @"..\..\qaac.exe" };

            return GetPathToBinary(ConfigKey.PathToQaac, "Qaac (qaac.exe)", hints);
        }

        private static string GetYtdlImpl(bool usePytube)
        {
            var bin = Utils.Sep + "bin" + Utils.Sep;
            if (Application.ExecutablePath.Contains(bin))
            {
                throw new CsDownloadVidException(bin + " found in path, it's best to run " +
                    "this in its own directory, near the config files, instead of running " +
                    "directly from visual studio.");
            }

            if (usePytube)
            {
                if (!File.Exists("benpytwrapper.py"))
                    throw new CsDownloadVidException("benpytwrapper.py was not found.");
                else if (!File.Exists("./tools/pytubemasterdir/pytube-master/pytube/__init__.py"))
                    throw new CsDownloadVidException("Try clicking the 'Get updates' button in the top " +
                    "right corner. ('pytubemasterdir was not found.')");

                return "benpytwrapper.py";
            }
            else
            {
                var relPath = "tools/ytmasterdir/youtube-dl-master/youtube_dl/__main__.py";
                if (File.Exists("./" + relPath))
                    return "./" + relPath;
                else if (File.Exists("../" + relPath))
                    return "../ " + relPath;
                else if (File.Exists("../../" + relPath))
                    return "../../" + relPath;

                throw new CsDownloadVidException("Try clicking the 'Get updates' button in the top " + 
                    "right corner. ('ytmasterdir was not found.')");
            }
        }

        public static string GetYtdlPath(bool usePytube, bool required = true)
        {
            string ret;
            try
            {
                ret = GetYtdlImpl(usePytube);
            }
            catch (CsDownloadVidException e)
            {
                MessageBox.Show(e.Message);
                if (!required)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            return ret;
        }

        private static string GetPathToBinary(ConfigKey key, string name, string[] hints)
        {
            var current = Configs.Current.Get(key);
            if (File.Exists(current))
            {
                return current;
            }

            foreach (var hint in hints)
            {
                if (File.Exists(hint))
                {
                    SimpleLog.Current.WriteLog("automatically detected location of " + name +
                        Utils.NL + hint);

                    Configs.Current.Set(key, hint);
                    return hint;
                }
            }

            while (true)
            {
                var file = Utils.AskOpenFileDialog("Please find " + name);
                if (file != null)
                {
                    Configs.Current.Set(key, file);
                    return file;
                }
                else if (!Utils.AskToConfirm("Program " + name + " still not found. Continue?"))
                {
                    throw new CsDownloadVidException("Program " + name + " not found.");
                }
            }
        }

        public static string GetDefaultDownloadsDir()
        {
            var downloadsDir = Configs.Current.Get(ConfigKey.SaveVideosTo);
            if (string.IsNullOrEmpty(downloadsDir) || !Directory.Exists(downloadsDir))
            {
                OperatingSystem os = Environment.OSVersion;
                PlatformID plid = os.Platform;
                var isWindows = plid.ToString().Contains("Win");
                if (isWindows)
                {
                    var guid = "{374DE290-123F-4565-9164-39C4925E467B}";
                    var hk = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\" +
                        @"Explorer\Shell Folders";
                    downloadsDir = Registry.GetValue(hk, guid, String.Empty).ToString();
                }
                else
                {
                    downloadsDir = Directory.Exists("~/Downloads") ? "~/Downloads" :
                        "~/downloads";
                }
            }

            return downloadsDir ?? "";
        }
    }

    public abstract class DownloadLatestPyScriptBase
    {
        public void Go(RunToolHelper run)
        {
            var prefix = this.GetPrefix();
            Directory.CreateDirectory("./tools");
            var pathCurrentZip = "./tools/%.zip".Replace("%", prefix);
            var pathCurrentDir = "./tools/%dir".Replace("%", prefix);
            var pathOldZip = "./tools/%-old.zip".Replace("%", prefix);
            var pathOldDir = "./tools/%dir-old".Replace("%", prefix);
            var pathIncomingZip = "./tools/%-incoming.zip".Replace("%", prefix);
            var pathIncomingDir = "./tools/%dir-incoming".Replace("%", prefix);

            // check before calling Delete(), since we want this to work even if
            // dir is currently empty
            run.Trace("Deleting temporary files");
            if (File.Exists(pathIncomingZip))
                File.Delete(pathIncomingZip);

            if (Directory.Exists(pathIncomingDir))
                Directory.Delete(pathIncomingDir, true);

            var url = this.GetUrl();
            run.Trace("Downloading from " + url);
            DownloadFile(url, pathIncomingZip);

            if (!File.Exists(pathIncomingZip))
                throw new CsDownloadVidException("No file was downloaded. " + url);

            if (new FileInfo(pathIncomingZip).Length < 500 * 1024)
                throw new CsDownloadVidException("File downloaded was too small, " +
                    "expect > 500k. " + pathIncomingZip);

            var currentHash = Utils.GetSha512(pathCurrentZip);
            var incomingHash = Utils.GetSha512(pathIncomingZip);
            if (currentHash == incomingHash)
            {
                run.Trace("We seem to have the latest version -- already up to date!");
                return;
            }

            run.Trace("Extracting from zip file...");
            ZipFile.ExtractToDirectory(pathIncomingZip, pathIncomingDir);
            run.Trace("Removing unneeded files...");
            var dir = pathIncomingDir + "/youtube-dl-master/youtube_dl/extractor";

            this.DoPostProcessing(dir, pathIncomingDir);
            run.Trace("Found a newer version!");
            run.Trace("Moving from current to old");
            if (File.Exists(pathOldZip))
                File.Delete(pathOldZip);
            if (Directory.Exists(pathOldDir))
                Directory.Delete(pathOldDir, true);
            if (File.Exists(pathCurrentZip))
                File.Move(pathCurrentZip, pathOldZip);
            if (Directory.Exists(pathCurrentDir))
                Directory.Move(pathCurrentDir, pathOldDir);

            run.Trace("Moving incoming to current");
            File.Move(pathIncomingZip, pathCurrentZip);
            Directory.Move(pathIncomingDir, pathCurrentDir);
        }

        public static void DownloadFile(string url, string dest)
        {
            if (url.StartsWith("https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }

            using (var client = new WebClient())
            {
                client.DownloadFile(url, dest);
            }
        }

        public abstract string GetPrefix();

        public abstract string GetUrl();

        public abstract void DoPostProcessing(string dir, string pathIncomingDir);
    }

    public class DownloadLatestPytube : DownloadLatestPyScriptBase
    {
        public override string GetPrefix()
        {
            return "pytubemaster";
        }

        public override string GetUrl()
        {
            return "https://github.com/pytube/pytube/archive/master.zip";
        }

        public override void DoPostProcessing(string dir, string pathIncomingDir)
        {
            var expectedMain = pathIncomingDir + "/pytube-master/pytube/__main__.py";
            if (!File.Exists(expectedMain))
                throw new CsDownloadVidException("no main.py found, expected at " + expectedMain);
        }
    }

    public class DownloadLatestYtdl : DownloadLatestPyScriptBase
    {
        bool trimRareScripts = false;
        public override string GetPrefix()
        {
            return "ytmaster";
        }

        public override string GetUrl()
        {
            return "https://github.com/rg3/youtube-dl/archive/master.zip";
        }

        public override void DoPostProcessing(string dir, string pathIncomingDir)
        {
            var pathMain = pathIncomingDir + "/youtube-dl-master/youtube_dl/__main__.py";
            if (!File.Exists(pathMain))
                throw new CsDownloadVidException("no main.py found, expected at " + pathMain);

            if (!trimRareScripts)
            {
                return;
            }

            if (Directory.Exists(dir))
            {
                // let's delete most of the python scripts in Ytdl -- there are 100s of scripts for 
                // sites I don't care about
                var allow = @"abc.py
amp.py
abcnews.py
acast.py
adobepass.py
adobetv.py
adultswim.py
adn.py
aljazeera.py
appleconnect.py
appletrailers.py
archiveorg.py
bandcamp.py
bostonglobe.py
common.py
commonmistakes.py
commonprotocols.py
buzzfeed.py
criterion.py
dailymotion.py
espn.py
ebaumsworld.py
facebook.py
extractors.py
giga.py
flickr.py
gfycat.py
go.py
go90.py
huffpost.py
imgur.py
instagram.py
imdb.py
khanacademy.py
kickstarter.py
metacafe.py
myspace.py
nytimes.py
turner.py
patreon.py
pokemon.py
reuters.py
vimeo.py
soundcloud.py
washingtonpost.py
youtube.py
vice.py
once.py
openload.py"; // note: no generic.py since it pulls in a lot. once.py and openload.py added december 2017
                var allowList = Utils.SplitLines(allow);
                var filesInDir = Directory.GetFiles(dir);
                foreach (var fileName in filesInDir)
                {
                    var shortName = Path.GetFileName(fileName);
                    if (shortName.EndsWith(".py") && !shortName.Contains("__") && Array.IndexOf(allowList, shortName) == -1)
                        File.Delete(fileName);
                }

                // comment out any reference to deleted files
                var linesOut = new List<string>();
                foreach (var line in File.ReadAllLines(Path.Combine(dir, "extractors.py")))
                {
                    if (line.StartsWith("from .") && line.Contains(" import "))
                    {
                        var moduleName = Utils.SplitByString(Utils.SplitByString(line, "from .")[1], " import ")[0];
                        if (File.Exists(Path.Combine(dir, moduleName + ".py")))
                            linesOut.Add(line);
                        else
                        {
                            linesOut.Add("if False:");
                            linesOut.Add("    " + line);
                        }
                    }
                    else
                    {
                        linesOut.Add(line);
                    }
                }

                File.WriteAllLines(Path.Combine(dir, "extractors.py"), linesOut.ToArray());

                // does the line reference GenericIE, and not as a string?
                Func<string, bool> hasGenericIE = (s) => s.Contains("GenericIE") &&
                    !s.Contains("'GenericIE'") && !s.Contains("\"GenericIE\"");

                // comment out reference to generic.py, since generic.py references a bunch of others
                var codePrev = File.ReadAllLines(Path.Combine(dir, "__init__.py"));
                var codeNew = from line in codePrev
                              select
                             hasGenericIE(line) ? "# " + line : line;
                File.WriteAllLines(Path.Combine(dir, "__init__.py"), codeNew);
            }
        }
    }

    public class LockCloser : IDisposable
    {
        private readonly object _lock;
        private bool _needToClose;
        public LockCloser(object locker)
        {
            _lock = locker;
        }

        public bool TryEnter()
        {
            Utils.AssertTrue(!this._needToClose, "Cannot enter when already entered.");
            var entered = Monitor.TryEnter(this._lock);
            if (entered)
            {
                this._needToClose = true;
            }

            return entered;
        }

        public void Dispose()
        {
            if (this._needToClose)
            {
                Monitor.Exit(_lock);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rbcpy
{
    public class RunImplementation
    {
        static readonly Random s_random = new Random();
        public static string GetLogFilename()
        {
            return "configs/templog" + s_random.Next() + ".txt";
        }

        public static string GetRandomNumbersInString()
        {
            return "" + s_random.Next();
        }

        public static string Go(SyncConfiguration config, string logfilename, bool previewOnly, bool getCommandOnly)
        {
            if (!getCommandOnly && config.m_isDeleteDuplicates)
            {
                MessageBox.Show("Didn't expect m_isDeleteDuplicates.");
                return "";
            }

            File.Delete(logfilename);
            string args = GetCommandLineArgs(config);
            if (previewOnly)
            {
                args += " /L ";
            }

            args += " /NS "; // no sizes in output
            args += " /FP "; // full paths in logs
            args += " /NP /UNILOG:" + logfilename;
            if (!getCommandOnly)
            {
                RunExeWithArguments("robocopy", args, false, true);
            }

            return args;
        }

        public static void ShowInExplorer(string path)
        {
            if (File.Exists(path))
            {
                string argument = "/select, \"" + path + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
        }

        static void RunExeWithArguments(string sExe, string args, bool createWindow, bool waitForExit)
        {
            Process process = new Process();
            process.StartInfo.CreateNoWindow = !createWindow;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = sExe;
            process.StartInfo.Arguments = args;
            process.Start();
            if (waitForExit)
            {
                process.WaitForExit();
            }
        }
        
        internal static string[] getLines(string slines)
        {
            var lines = slines.Replace("\r\n", "\n").Split(new char[] { '\n' });
            return lines.Where((s) => s.Length > 0).Select((s) => s.Trim()).ToArray();
        }

        internal static string GetCommandLineArgs(SyncConfiguration config)
        {
            string args = "";
            args += " \"" + config.m_src + "\" \"" + config.m_destination + "\" ";
            
            var lines = getLines(config.m_excludeDirs);
            foreach(var line in lines)
            {
                Utils.AssertEq(true, !line.Contains("\""));
                args += " /XD \"" + line + "\" ";
            }

            lines = getLines(config.m_excludeFiles);
            foreach (var line in lines)
            {
                Utils.AssertEq(true, !line.Contains("\""));
                args += " /XF \"" + line + "\" ";
            }
            
            if (config.m_mirror)
                args += " /MIR ";
            if (config.m_copySubDirsAndEmptySubdirs)
                args += " /E "; // " /S /E ";
            if (config.m_copyFlags != "")
                args += " /COPY:"+config.m_copyFlags+" ";
            if (config.m_directoryCopyFlags != "")
                args += " /DCOPY:" + config.m_directoryCopyFlags + " ";
            if (config.m_ipg != "")
                args += " /IPG:" + config.m_ipg + " ";
            if (config.m_nRetries != "")
                args += " /R:" + config.m_nRetries + " ";
            if (config.m_waitBetweenRetries != "")
                args += " /W:" + config.m_waitBetweenRetries + " ";
            if (config.m_nThreads != "" && config.m_nThreads != "0")
                args += " /MT:" + config.m_nThreads + " ";
            if (config.m_custom != "")
                args += " " + config.m_custom + " ";
            if (config.m_symlinkNotTarget)
                args += " /SL ";
            if (config.m_fatTimes)
                args += " /FFT ";
            if (config.m_compensateDst)
                args += " /DST ";

            return args;
        }

        public static void OpenWinmerge(string sWinmerge, string src, string dest, bool recursive)
        {
            if (String.IsNullOrEmpty(sWinmerge))
            {
                MessageBox.Show("First indicate where winmerge is, from the file menu.");
                return;
            }
            
            string sArgs = "/e ";
            if (recursive)
            { 
                sArgs += "/r ";
            }

            sArgs += " \"" + src + "\" \"" + dest + "\" ";
            RunExeWithArguments(sWinmerge, sArgs, true, false);
        }
    }

    public enum CCreateSyncItemStatus
    {
        None,
        AddedInSrc,
        AddedInDest,
        ChangedAndSrcNewer,
        ChangedAndDestNewer,
        DeleteDuplicate,
        Unknown
    }

    public class CCreateSyncItem
    {
        public CCreateSyncItemStatus status = CCreateSyncItemStatus.None;
        public string path = "";
        public bool mirror = false;
        public const int nEmpty = 0;
        public const int nUpdate = 1;
        public const int nAddNew = 2;
        public const int nUpdateWarn = 3;
        public const int nRemove = 4;
        
        public override string ToString()
        {
            string s = "";
            if (status == CCreateSyncItemStatus.AddedInSrc)
                s += "Create\t\t";
            else if (status == CCreateSyncItemStatus.DeleteDuplicate)
                s += "Delete\t\t";
            else if (status == CCreateSyncItemStatus.AddedInDest && mirror)
                s += "Delete\t\t";
            else if (status == CCreateSyncItemStatus.AddedInDest && !mirror)
                s += "(New file)\t\t";
            else if (status == CCreateSyncItemStatus.ChangedAndSrcNewer)
                s += "Update\t\t";
            else if (status == CCreateSyncItemStatus.ChangedAndDestNewer)
                s += "Update()\t\t";

            if (status == CCreateSyncItemStatus.DeleteDuplicate)
            {
                s += path.Substring(0, path.IndexOf('|'));
            }
            else
            {
                s += path;
            }

            return s;
        }

        public ListViewItem GetListItem(bool bPreview)
        {
            int imageIndex = -1;
            string sAction = "";

            if (status == CCreateSyncItemStatus.AddedInSrc)
            {
                imageIndex = nAddNew;
                sAction = bPreview ? "Create" : "Created";
            }
            else if (status == CCreateSyncItemStatus.DeleteDuplicate)
            {
                imageIndex = nRemove;
                sAction = bPreview ? "Delete" : "Deleted";
            }
            else if (status == CCreateSyncItemStatus.AddedInDest && mirror)
            {
                imageIndex = nRemove;
                sAction = bPreview ? "Delete" : "Deleted";
            }
            else if (status == CCreateSyncItemStatus.AddedInDest && !mirror)
            {
                imageIndex = nEmpty;
                sAction = "(New file)";
            }
            else if (status == CCreateSyncItemStatus.ChangedAndSrcNewer)
            {
                imageIndex = nUpdate;
                sAction = bPreview ? "Update" : "Updated";
            }
            else if (status == CCreateSyncItemStatus.ChangedAndDestNewer)
            {
                imageIndex = nUpdateWarn;
                sAction = bPreview ? "Update()" : "Updated()";
            }
            else
            {
                imageIndex = nEmpty;
                sAction = " ";
            }

            string sType = (sAction == " ") ? " " : "File";
            string sDisplayPath;
            if (status == CCreateSyncItemStatus.DeleteDuplicate)
            {
                sDisplayPath = path.Substring(0, path.IndexOf('|'));
            }
            else
            {
                sDisplayPath = path;
            }

            ListViewItem viewItem = new ListViewItem(new string[] { sType, sAction, sDisplayPath });
            viewItem.Tag = this;
            viewItem.ImageIndex = imageIndex;
            return viewItem;
        }

        public string GetLeftPath(SyncConfiguration config)
        {
            if (status == CCreateSyncItemStatus.DeleteDuplicate)
            {
                return path.Substring(0, path.IndexOf('|'));
            }
            else
            {
                var tmpPath = path;
                if (tmpPath.StartsWith("\\"))
                {
                    tmpPath = tmpPath.Substring(1);
                }

                return Path.Combine(config.m_src, tmpPath);
            }
        }
        public string GetRightPath(SyncConfiguration config)
        {
            if (status == CCreateSyncItemStatus.DeleteDuplicate)
            {
                return path.Substring(path.IndexOf('|') + 1);
            }
            else
            {
                var tmppath = path;
                if (tmppath.StartsWith("\\"))
                {
                    tmppath = tmppath.Substring(1);
                }

                return Path.Combine(config.m_destination, tmppath);
            }
        }

        public bool IsInLeft()
        {
            return (status == CCreateSyncItemStatus.AddedInSrc || status == CCreateSyncItemStatus.ChangedAndDestNewer || status == CCreateSyncItemStatus.ChangedAndSrcNewer || status == CCreateSyncItemStatus.DeleteDuplicate);
        }

        public bool IsInRight()
        {
            return (status == CCreateSyncItemStatus.AddedInDest || status == CCreateSyncItemStatus.ChangedAndDestNewer || status == CCreateSyncItemStatus.ChangedAndSrcNewer || status == CCreateSyncItemStatus.DeleteDuplicate);
        }

        public static void SortFromColumnNumber(List<CCreateSyncItem> items, int nSortCol)
        {
            int nMult = nSortCol > 0 ? 1 : -1;
            if (nSortCol == 3 /*path*/ || nSortCol == -3)
            {
                items.Sort(delegate(CCreateSyncItem o1, CCreateSyncItem o2)
                {
                    return nMult * o1.path.CompareTo(o2.path);
                });
            }
            else
            {
                // the distinction is: should Update() compare equal with Update?
                bool bCombineUpdateTypes = (nSortCol == 2 || nSortCol == -2);
                items.Sort(delegate(CCreateSyncItem o1, CCreateSyncItem o2)
                {
                    var o1status = o1.status;
                    var o2status = o2.status;
                    if (bCombineUpdateTypes && o1status == CCreateSyncItemStatus.ChangedAndDestNewer)
                    {
                        o1status = CCreateSyncItemStatus.ChangedAndSrcNewer;
                    }

                    if (bCombineUpdateTypes && o2status == CCreateSyncItemStatus.ChangedAndDestNewer)
                    {
                        o2status = CCreateSyncItemStatus.ChangedAndSrcNewer;
                    }

                    if (o1status == o2status)
                    {
                        return nMult * o1.path.CompareTo(o2.path);
                    } else
                    {
                        return nMult * o1.status.CompareTo(o2.status);
                    }
                });
            }
        }
    }

    public class CCreateSyncResultsSet
    {
        public string sLogFilename;
        public string sSummary;
        public List<CCreateSyncItem> items = new List<CCreateSyncItem>();
        public SyncConfiguration config;
        internal static bool showWarnings = true;

        private static IEnumerable<string> SplitLogItemsToGetPathFragments(SyncConfiguration config, string strSection)
        {
            // use non capturing groups (?:)
            var regex = new Regex("(?:" + Regex.Escape(config.m_src) + ")|(?:" + Regex.Escape(config.m_destination) + ")", RegexOptions.IgnoreCase);
            var segments = regex.Split(strSection);
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].Contains("\r"))
                {
                    segments[i] = segments[i].Substring(0, segments[i].IndexOf("\r"));
                }

                if (segments[i].Contains("\t"))
                {
                    segments[i] = segments[i].Substring(0, segments[i].IndexOf("\t"));
                }

                yield return segments[i].Trim();
            }
        }

        internal static List<CCreateSyncItem> ParseFromLogFileContents(SyncConfiguration config, string sLogFilename, out bool bFailuresSeen, out StringBuilder sbSummary, bool bPreview)
        {
            bFailuresSeen = false;
            sbSummary = new StringBuilder();
            string sLogContents = File.ReadAllText(sLogFilename);
            sLogContents = Regex.Replace(sLogContents, "\r *[0-9]+%", "");
            List<CCreateSyncItem> ret = new List<CCreateSyncItem>();
            var sections = Regex.Split(sLogContents, "\r\n---------------------+\r\n");
            if (sections.Length < 5)
            {
                throw new InvalidDataException("Summary section not found");
            }

            foreach (var line in sections[4].Split(new string[] { "\r\n" }, StringSplitOptions.None))
            {
                var sTrimmed = line.Trim();
                if (!sTrimmed.StartsWith("Times :") && !sTrimmed.StartsWith("Ended :"))
                {
                    sbSummary.AppendLine(line);
                }

                if (sTrimmed.StartsWith("Dirs :") && LookForFailuresInSummary(sTrimmed))
                {
                    bFailuresSeen = true;
                }

                if (sTrimmed.StartsWith("Files :") && LookForFailuresInSummary(sTrimmed))
                {
                    bFailuresSeen = true;
                }
            }

            int nUnknowns = 0;
            foreach (var segment in SplitLogItemsToGetPathFragments(config, sections[3]))
            {
                var left = config.m_src + segment;
                var right = config.m_destination + segment;

                if (segment.Trim() == "")
                {
                    continue;
                }
                else if (segment.EndsWith("\\"))
                {
                    if (bPreview && !Directory.Exists(left) && !Directory.Exists(right))
                        MessageBox.Show("directory does not exist");

                    continue; // we don't track directories
                }

                var status = CCreateSyncItemStatus.Unknown;
                var leftExists = File.Exists(left);
                var rightExists = File.Exists(right);
                if (!leftExists && !rightExists)
                {
                    if (bPreview)
                    {
                        MessageBox.Show("file does not exist");
                    }

                    continue;
                }
                else if (leftExists && !rightExists)
                {
                    status = CCreateSyncItemStatus.AddedInSrc;
                }
                else if (rightExists && !leftExists)
                {
                    status = CCreateSyncItemStatus.AddedInDest;
                }
                else if (rightExists && leftExists)
                {
                    if (new FileInfo(right).LastWriteTime > new FileInfo(left).LastWriteTime)
                    {
                        status = CCreateSyncItemStatus.ChangedAndDestNewer;
                    }
                    else
                    {
                        status = CCreateSyncItemStatus.ChangedAndSrcNewer;
                    }
                }

                ret.Add(new CCreateSyncItem { status = status, path = segment, mirror = config.m_mirror });
            }

            if (showWarnings && nUnknowns > 0)
            {
                MessageBox.Show("note: at least one log entry was not parsed");
            }

            return ret;
        }

        public static CCreateSyncResultsSet ParseFromLogFile(SyncConfiguration config, string sLogFilename, bool bPreview)
        {
            CCreateSyncResultsSet ret = new CCreateSyncResultsSet();
            ret.config = config;
            ret.sLogFilename = sLogFilename;

            if (!File.Exists(sLogFilename))
            {
                ret.sSummary = ("Unknown failure. Log file not found.");
                return ret;
            }

            int nTotalLinesWithDashes = CountSections(sLogFilename);

            if (nTotalLinesWithDashes < 3)
            {
                ret.sSummary = ("Unknown failure. In the log file, less than 3 separator lines.");
                return ret;
            }
            else if (nTotalLinesWithDashes == 3) // looks like errors occurred...
            {
                ret.sSummary = RetrieveTextPastSectionNumber(sLogFilename, 3, "Because there are only 3 separator lines, looks like errors occurred:");
                return ret;
            }
            
            StringBuilder sbSummary;
            bool bFailuresSeen;
            List<CCreateSyncItem> items = ParseFromLogFileContents(config, sLogFilename, out bFailuresSeen, out sbSummary, bPreview);
            ret.items = items;

            if (bFailuresSeen)
            {
                // show the entire results in the summary if a failure occurs
                ret.items.Clear();
                ret.sSummary = RetrieveTextPastSectionNumber(sLogFilename, 3, "Summary indicated failures (can be caused by broken symlinks):");
                return ret;
            }

            if (nTotalLinesWithDashes > 4)
            {
                sbSummary.AppendLine("Note, more than 4 lines with dashes (" + nTotalLinesWithDashes + ")");
            }

            ret.sSummary = sbSummary.ToString();
            ret.sSummary = ret.sSummary.Replace("FAILED", "Failed");
            return ret;
        }

        private static bool LookForFailuresInSummary(string sTrimmed)
        {
            var parts = Regex.Split(sTrimmed, "  +");
            Utils.AssertTrue(parts.Length == 7);
            string sFailed = parts[5];
            return int.Parse(sFailed) != 0;
        }

        private static string RetrieveTextPastSectionNumber(string sLogFilename, int nSection, string sReason)
        {
            string s;
            int nLinesWithDashes = 0;
            StringBuilder sbSummary = new StringBuilder();
            sbSummary.AppendLine(sReason);
            using (StreamReader sr = new StreamReader(sLogFilename))
            {
                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Contains("---------------------") && s.Replace("-", "") == "")
                    {
                        nLinesWithDashes++;
                        continue;
                    }
                    if (nLinesWithDashes >= nSection)
                    {
                        sbSummary.AppendLine(s);
                    }
                }
            }

            return sbSummary.ToString();
        }

        private static int CountSections(string sLogFilename)
        {
            string s;
            int nTotalLinesWithDashes = 0;
            using (StreamReader sr = new StreamReader(sLogFilename))
            {
                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Contains("---------------------") && s.Replace("-", "") == "")
                        nTotalLinesWithDashes++;
                }
            }

            return nTotalLinesWithDashes;
        }
    }
}

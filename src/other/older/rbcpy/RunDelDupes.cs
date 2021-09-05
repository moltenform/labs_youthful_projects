using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace rbcpy
{
    public static class RunDelDupes
    {
        static Dictionary<string, string> GetDestOnes(SyncConfiguration config, out int nSmallFilesInDest)
        {
            nSmallFilesInDest = 0;
            var ret = new Dictionary<string, string>();
            MD5 md5 = MD5.Create();
            foreach (var sFile in Directory.EnumerateFiles(config.m_destination, "*", SearchOption.AllDirectories))
            {
                if (new FileInfo(sFile).Length < 10) { nSmallFilesInDest++;  continue; }
                using (var stream = File.OpenRead(sFile))
                {
                    byte[] bytes = md5.ComputeHash(stream);
                    ret[Convert.ToBase64String(bytes)] = sFile;
                }
            }
            return ret;
        }
        static CCreateSyncResultsSet GetDupes(SyncConfiguration config, Dictionary<string, string> srcs, out int nSmallFilesInSrc)
        {
            nSmallFilesInSrc = 0;
            CCreateSyncResultsSet retDupes = new CCreateSyncResultsSet();
            retDupes.config = config;
            retDupes.sLogFilename = "";
            retDupes.sSummary = "";
            retDupes.items = new List<CCreateSyncItem>();
            MD5 md5 = MD5.Create();
            foreach (var sFile in Directory.EnumerateFiles(config.m_src, "*", SearchOption.AllDirectories))
            {
                if (new FileInfo(sFile).Length < 10) { nSmallFilesInSrc++; continue; }
                using (var stream = File.OpenRead(sFile))
                {
                    byte[] bytes = md5.ComputeHash(stream);
                    var hash = Convert.ToBase64String(bytes);
                    string sOut;
                    if (srcs.TryGetValue(hash, out sOut))
                    {
                        retDupes.items.Add(new CCreateSyncItem { status = CCreateSyncItemStatus.DeleteDuplicate, path = sFile + "|" + sOut, mirror = config.m_mirror });
                    }
                }
            }
            return retDupes;
        }
       
        static CCreateSyncResultsSet FindInternalDuplicates(SyncConfiguration config, out int nSmallFiles)
        {
            nSmallFiles = 0;
            CCreateSyncResultsSet retDupes = new CCreateSyncResultsSet();
            retDupes.config = config;
            retDupes.sLogFilename = "";
            retDupes.sSummary = "";
            retDupes.items = new List<CCreateSyncItem>();
            MD5 md5 = MD5.Create();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            // we'd need to sort to have a consistent ordering, but that would have to be a custom sort that understands # of directories, casing, etc. .ToList();
            var sAllFiles = Directory.EnumerateFiles(config.m_src, "*", SearchOption.AllDirectories);
            //sAllFiles.Sort();
            //sAllFiles.Reverse();
            foreach (var sFile in sAllFiles)
            {
                if (new FileInfo(sFile).Length < 10) { nSmallFiles++; continue; }
                md5 = MD5.Create();
                using (var stream = File.OpenRead(sFile))
                {
                    byte[] bytes = md5.ComputeHash(stream);
                    var hash = Convert.ToBase64String(bytes);
                    string sOut;
                    if (dict.TryGetValue(hash, out sOut))
                    {
                        retDupes.items.Add(new CCreateSyncItem { status = CCreateSyncItemStatus.DeleteDuplicate, path = sFile+"|"+sOut , mirror = config.m_mirror });
                    }
                    else
                    {
                        dict[hash] = sFile;
                    }
                }
            }
            return retDupes;
        }

        public static CCreateSyncResultsSet Run(SyncConfiguration config)
        {
            if (!config.m_mirror)
            {
                System.Windows.Forms.MessageBox.Show("Only mirror supported.");
                return new CCreateSyncResultsSet();
            }

            if (!config.m_isDeleteDuplicates)
            {
                System.Windows.Forms.MessageBox.Show("Expected m_isDeleteDuplicates");
                return new CCreateSyncResultsSet();
            }

            if (config.m_src == config.m_destination)
            {
                int nSmallFiles;
                CCreateSyncResultsSet result = FindInternalDuplicates(config, out nSmallFiles);
                result.sSummary += "Running find internal duplicates. There are " + result.items.Count + " duplicates. \n\nNote: skipped over " + nSmallFiles + " small files.";
                return result;
            }
            else
            {
                int nSmallFilesInDest, nSmallFilesInSrc;
                Dictionary<string, string> dests = GetDestOnes(config, out nSmallFilesInDest);
                CCreateSyncResultsSet result = GetDupes(config, dests, out nSmallFilesInSrc);
                result.sSummary += "There are " + result.items.Count + " duplicates. \n\nNote: skipped over " + nSmallFilesInDest + " small files in dest and " + nSmallFilesInSrc + " small files in src.";

                return result;
            }
        }

        public static CCreateSyncResultsSet ExecuteResultsSet(CCreateSyncResultsSet results)
        {
            if (!results.config.m_mirror)
            {
                System.Windows.Forms.MessageBox.Show("Only mirror supported.");
                return new CCreateSyncResultsSet();
            }

            CCreateSyncResultsSet ret = new CCreateSyncResultsSet();
            ret.items = results.items;
            StringBuilder sbFailures = new StringBuilder();
            int nFailures = 0;
            foreach(var result in results.items)
            {
                if (result.status == CCreateSyncItemStatus.DeleteDuplicate && results.config.m_isDeleteDuplicates)
                {
                    try
                    {
                        File.Delete(result.GetLeftPath(results.config));
                    }
                    catch(Exception e)
                    {
                        nFailures++;
                        sbFailures.AppendLine(e.ToString());
                        sbFailures.Append(result.GetLeftPath(results.config));
                    }
                }
            }

            if (nFailures == 0)
                ret.sSummary = "Successfully deleted " + results.items.Count + " duplicates. \r\n\r\nPrev summary:" + results.sSummary;
            else
                ret.sSummary = "FAILURES: " + nFailures + " failures, \r\n" + sbFailures.ToString() + "\r\n\r\nPrev summary:" + results.sSummary;
            return ret;
        }

        public static void CompareTwoFiles(string s1, string s2)
        {
            var length1 = new FileInfo(s1).Length;
            var length2 = new FileInfo(s2).Length;
            if (length1!=length2)
            {
                System.Windows.Forms.MessageBox.Show("Files are not equal, \nlength of "+s1+" \n="+ length1+"\nlength of "+s2+" \n="+length2+".");
                return;
            }
            var hash1 = Hashing.GetHash(s1);
            var hash2 = Hashing.GetHash(s2);
            if (hash1 != hash2)
            {
                System.Windows.Forms.MessageBox.Show("Hashes are not equal, \nhash of " + s1 + " \n=" + hash1 + "\nhash of " + s2 + " \n=" + hash2 + ".");
                return;
            }
            System.Windows.Forms.MessageBox.Show("Files are nearly certain to be equal.");
        }
    }
}

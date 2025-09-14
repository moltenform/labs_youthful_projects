// Copyright (c) Ben Fisher, 2016.
// Licensed under GPLv3, refer to LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rbcpy
{
    public static class RbcpyTests
    {
        static void TestMethod_SyncConfiguration_Deserialize()
        {
            var config = SyncConfiguration.Deserialize(Testing.GetTestFile("test_cfg_01.xml"));
            Utils.AssertEq(config.m_src, "chg1");
            Utils.AssertEq(config.m_destination, "chg2");
            Utils.AssertEq(config.m_excludeDirs, "chg3");
            Utils.AssertEq(config.m_excludeFiles, "chg4");
            Utils.AssertEq(config.m_mirror, true);
            Utils.AssertEq(config.m_copySubDirsAndEmptySubdirs, false);
            Utils.AssertEq(config.m_copyFlags, "#1");
            Utils.AssertEq(config.m_directoryCopyFlags, "#2");
            Utils.AssertEq(config.m_ipg, "#3");
            Utils.AssertEq(config.m_nRetries, "#4");
            Utils.AssertEq(config.m_waitBetweenRetries, "#5");
            Utils.AssertEq(config.m_nThreads, "#6");
            Utils.AssertEq(config.m_custom, "#7");
            Utils.AssertEq(config.m_symlinkNotTarget, true);
            Utils.AssertEq(config.m_fatTimes, false);
            Utils.AssertEq(config.m_compensateDst, true);
        }

        static void TestMethod_SyncConfiguration_Serialize()
        {
            var config = new SyncConfiguration();
            config.m_src = "chg8";
            config.m_destination = "chg9";
            config.m_excludeDirs = "chg10";
            config.m_excludeFiles = "chg11";
            config.m_mirror = false;
            config.m_copySubDirsAndEmptySubdirs = true;
            config.m_copyFlags = "#1@";
            config.m_directoryCopyFlags = "#2@";
            config.m_ipg = "#3@";
            config.m_nRetries = "#4@";
            config.m_waitBetweenRetries = "#5@";
            config.m_nThreads = "#6@";
            config.m_custom = "#7@";
            config.m_symlinkNotTarget = false;
            config.m_fatTimes = true;
            config.m_compensateDst = false;
            File.Delete(Testing.GetTestTempFile("test_cfg_02_got.xml"));
            SyncConfiguration.Serialize(config, Testing.GetTestTempFile("test_cfg_02_got.xml"));
            string sExpected = File.ReadAllText(Testing.GetTestFile("test_cfg_02.xml"));
            string sGot = File.ReadAllText(Testing.GetTestTempFile("test_cfg_02_got.xml"));
            sGot = sGot.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"");
            sExpected = sExpected.Replace("\r", "").Replace("\n", "").Replace("  <", "<");
            sGot = sGot.Replace("\r", "").Replace("\n", "").Replace("  <", "<");
            Utils.AssertEq(sExpected, sGot);
        }

        static SyncConfiguration GetValidConfig()
        {
            var config = new SyncConfiguration();
            config.m_src = Testing.GetTestDirectory() + "\\testsync\\src";
            config.m_destination = Testing.GetTestDirectory() + "\\testsync\\dest";
            return config;
        }

        static void TestMethod_ValidationShouldSucceed()
        {
            Utils.AssertEq(true, SyncConfiguration.Validate(GetValidConfig()));
        }

        static void TestMethod_ValidationShouldFailBadSrc()
        {
            // make it not validate
            var config = GetValidConfig();
            config.m_src = "notexist";
            Utils.AssertEq(false, SyncConfiguration.Validate(config));
        }

        static void TestMethod_ValidationShouldFailBadDestination()
        {
            // make it not validate
            var config = GetValidConfig();
            config.m_destination = "notexist";
            Utils.AssertEq(false, SyncConfiguration.Validate(config));
        }

        static void TestMethod_ValidationShouldFailSrcEndsWithSlash()
        {
            var config = GetValidConfig();
            config.m_src += "\\";
            Utils.AssertEq(false, SyncConfiguration.Validate(config));
        }

        static void TestMethod_ValidationShouldFailDestEndsWithSlash()
        {
            var config = GetValidConfig();
            config.m_destination += "\\";
            Utils.AssertEq(false, SyncConfiguration.Validate(config));
        }

        static void TestMethod_ValidationShouldFailIfIntersection1()
        {
            var config = GetValidConfig();
            config.m_src = Testing.GetTestDirectory() + "\\testsync\\src";
            config.m_destination = Testing.GetTestDirectory() + "\\TestSync";
            Utils.AssertEq(false, SyncConfiguration.Validate(config));
        }

        static void TestMethod_ValidationShouldFailIfIntersection2()
        {
            var config = GetValidConfig();
            config.m_src = Testing.GetTestDirectory() + "\\testSync";
            config.m_destination = Testing.GetTestDirectory() + "\\testsync\\dest";
            Utils.AssertEq(false, SyncConfiguration.Validate(config));
        }

        static void TestMethod_ValidationNThreadsShouldBeInt()
        {
            // make it not validate
            var config = GetValidConfig();
            config.m_nThreads = "4a";
            Utils.AssertEq(false, SyncConfiguration.Validate(config));
            config.m_nThreads = "5";
            Utils.AssertEq(true, SyncConfiguration.Validate(config));
            config.m_nRetries = "6a";
            Utils.AssertEq(false, SyncConfiguration.Validate(config));
            config.m_nRetries = "7";
            Utils.AssertEq(true, SyncConfiguration.Validate(config));
        }

        static void TestMethod_ValidationQuotesAreDisallowed()
        {
            var config = GetValidConfig();
            config.m_copyFlags = "with\"quote";
            Utils.AssertEq(false, SyncConfiguration.Validate(config));
            config.m_copyFlags = "without quote";
            Utils.AssertEq(true, SyncConfiguration.Validate(config));
            config.m_custom = "ok to have with\"quote";
            Utils.AssertEq(true, SyncConfiguration.Validate(config));
        }

        static void TestMethod_GetCommandLineParameters01()
        {
            var config = SyncConfiguration.Deserialize(Testing.GetTestFile("test_cfg_01.xml"));
            var sArguments = RunImplementation.GetCommandLineArgs(config);
            var sExpected = " \"chg1\" \"chg2\"  /XD \"chg3\"  /XF \"chg4\"  /MIR  /COPY:#1  /DCOPY:#2  /IPG:#3  /R:#4  /W:#5  /MT:#6  #7  /SL  /DST ";
            Utils.AssertEq(sExpected, sArguments);
        }

        static void TestMethod_GetCommandLineParameters02()
        {
            var config = SyncConfiguration.Deserialize(Testing.GetTestFile("test_cfg_02.xml"));
            var sArguments = RunImplementation.GetCommandLineArgs(config);
            var sExpected = " \"chg8\" \"chg9\"  /XD \"chg10\"  /XF \"chg11\"  /E  /COPY:#1@  /DCOPY:#2@  /IPG:#3@  /R:#4@  /W:#5@  /MT:#6@  #7@  /FFT ";
            Utils.AssertEq(sExpected, sArguments);
        }

        static void TestMethod_TestPreviewOfFileSyncAndActualSync()
        {
            string sDirectory = Testing.SetUpSynctestEnvironment();
            TestSyncPreview(sDirectory, 1 /*nThreads*/);
            TestSyncPreview(sDirectory, 4 /*nThreads*/);

            TestActualSync(sDirectory, 1 /*nThreads*/);
            Testing.SetUpSynctestEnvironment();
            TestActualSync(sDirectory, 4 /*nThreads*/);
        }

        static void TestActualSync(string sDirectory, int nThreads)
        {
            var config = GetRealConfig(sDirectory, nThreads);
            string sLogFilename = RunImplementation.GetLogFilename();
            RunImplementation.Go(config, sLogFilename, false /*preview*/, false);
            var results = CCreateSyncResultsSet.ParseFromLogFile(config, sLogFilename, false /*preview*/);
            File.Delete(sLogFilename);
            // the number of skipped dirs is 3 instead of the 2 it used to be, but not important right now
            Utils.AssertEq("Total    Copied   Skipped  Mismatch    Failed    Extras\r\n    Dirs :         5         5         3         0         0         2\r\n   Files :        22         8        14         0         0         2\r\n   Bytes :   632.0 k   126.6 k   505.4 k         0         0    30.7 k", results.sSummary.Trim());

            // check files
            string[] filesExpected = @"..\..\test\testsync\dest
..\..\test\testsync\dest\Images
..\..\test\testsync\dest\Images\a.png
..\..\test\testsync\dest\Images\addempty
..\..\test\testsync\dest\Images\addir
..\..\test\testsync\dest\Images\addir\a.PNG
..\..\test\testsync\dest\Images\b.png
..\..\test\testsync\dest\Images\c.png
..\..\test\testsync\dest\Images\d.png
..\..\test\testsync\dest\Images\DB44-20-x64.jpg
..\..\test\testsync\dest\Images\e.png
..\..\test\testsync\dest\Images\f.gif
..\..\test\testsync\dest\Licenses
..\..\test\testsync\dest\Licenses\.weirdext
..\..\test\testsync\dest\Licenses\Apr-License.txt
..\..\test\testsync\dest\Licenses\Apr-Util-License.txt
..\..\test\testsync\dest\Licenses\BerkeleyDB-License.txt
..\..\test\testsync\dest\Licenses\Cyrus-Sasl-License.txt
..\..\test\testsync\dest\Licenses\GetText-Runtime-License.txt
..\..\test\testsync\dest\Licenses\noext
..\..\test\testsync\dest\Licenses\OpenSsl-License.txt
..\..\test\testsync\dest\Licenses\Serf-License.txt
..\..\test\testsync\dest\Licenses\SharpSvn-License.txt
..\..\test\testsync\dest\Licenses\Subversion-License.txt
..\..\test\testsync\dest\loren.html
..\..\test\testsync\dest\loren.txt
..\..\test\testsync\dest\pic1.png
..\..\test\testsync\src
..\..\test\testsync\src\Images
..\..\test\testsync\src\Images\a.png
..\..\test\testsync\src\Images\addempty
..\..\test\testsync\src\Images\addir
..\..\test\testsync\src\Images\addir\a.PNG
..\..\test\testsync\src\Images\b.png
..\..\test\testsync\src\Images\c.png
..\..\test\testsync\src\Images\d.png
..\..\test\testsync\src\Images\DB44-20-x64.jpg
..\..\test\testsync\src\Images\e.png
..\..\test\testsync\src\Images\f.gif
..\..\test\testsync\src\Licenses
..\..\test\testsync\src\Licenses\.weirdext
..\..\test\testsync\src\Licenses\Apr-License.txt
..\..\test\testsync\src\Licenses\Apr-Util-License.txt
..\..\test\testsync\src\Licenses\BerkeleyDB-License.txt
..\..\test\testsync\src\Licenses\Cyrus-Sasl-License.txt
..\..\test\testsync\src\Licenses\GetText-Runtime-License.txt
..\..\test\testsync\src\Licenses\noext
..\..\test\testsync\src\Licenses\OpenSsl-License.txt
..\..\test\testsync\src\Licenses\Serf-License.txt
..\..\test\testsync\src\Licenses\SharpSvn-License.txt
..\..\test\testsync\src\Licenses\Subversion-License.txt
..\..\test\testsync\src\loren.html
..\..\test\testsync\src\loren.txt
..\..\test\testsync\src\pic1.png".Replace("\r\n", "\n").Replace(@"..\..\test\testsync\", sDirectory + "\\").Split(new char[] { '\n' });
            List<string> filesGot = Directory.GetFileSystemEntries(sDirectory, "*", SearchOption.AllDirectories).ToList();
            filesGot.Sort();
            Testing.AssertStringArrayEqual(filesExpected, filesGot);
            Utils.AssertEq(new FileInfo(sDirectory + "\\src\\Licenses\\Cyrus-Sasl-License.txt").Length, 1861L);
            Utils.AssertEq(new FileInfo(sDirectory + "\\src\\Licenses\\OpenSsl-License.txt").Length, 6286L);
            Utils.AssertEq(new FileInfo(sDirectory + "\\src\\Licenses\\Apr-License.txt").Length, 18324L);
            Utils.AssertEq(new FileInfo(sDirectory + "\\src\\Licenses\\Serf-License.txt").Length, 11562L);
            Utils.AssertEq(new FileInfo(sDirectory + "\\dest\\Licenses\\Cyrus-Sasl-License.txt").Length, 1861L);
            Utils.AssertEq(new FileInfo(sDirectory + "\\dest\\Licenses\\OpenSsl-License.txt").Length, 6286L);
            Utils.AssertEq(new FileInfo(sDirectory + "\\dest\\Licenses\\Apr-License.txt").Length, 18324L);
            Utils.AssertEq(new FileInfo(sDirectory + "\\dest\\Licenses\\Serf-License.txt").Length, 11562L);
        }

        private static void TestSyncPreview(string sDirectory, int nThreads)
        {
            var config = GetRealConfig(sDirectory, nThreads);

            string sLogFilename = RunImplementation.GetLogFilename();
            RunImplementation.Go(config, sLogFilename, true /*preview*/, false);
            var results = CCreateSyncResultsSet.ParseFromLogFile(config, sLogFilename, true /*preview*/);
            // the number of skipped dirs is 3 instead of the 2 it used to be, but not important right now
            Utils.AssertEq("Total    Copied   Skipped  Mismatch    Failed    Extras\r\n    Dirs :         5         5         3         0         0         2\r\n   Files :        22         8        14         0         0         2\r\n   Bytes :   632.0 k   126.6 k   505.4 k         0         0    30.7 k", results.sSummary.Trim());
            File.Delete(sLogFilename);

            {
                CCreateSyncItem.SortFromColumnNumber(results.items, 3); // sort by path ascending
                var resultsFilteredStrings = from item in results.items where item.status != CCreateSyncItemStatus.Unknown select item.ToString();
                var sGot = String.Join("\n", resultsFilteredStrings);
                var sExpected = @"Create		\Images\a.png
Create		\Images\addir\a.PNG
Delete		\Images\new.png
Delete		\Images\remdir\c.png
Create		\Licenses\.weirdext
Update		\Licenses\Apr-License.txt
Update()		\Licenses\Cyrus-Sasl-License.txt
Create		\Licenses\noext
Update()		\Licenses\OpenSsl-License.txt
Update		\Licenses\Serf-License.txt".Replace("\r\n", "\n");
                Utils.AssertEq(sExpected, sGot);
            }

            {
                CCreateSyncItem.SortFromColumnNumber(results.items, -3); // sort by path descending
                var resultsFilteredStrings = from item in results.items where item.status != CCreateSyncItemStatus.Unknown select item.ToString();
                var sGot = String.Join("\n", resultsFilteredStrings);
                var sExpected = @"Update		\Licenses\Serf-License.txt
Update()		\Licenses\OpenSsl-License.txt
Create		\Licenses\noext
Update()		\Licenses\Cyrus-Sasl-License.txt
Update		\Licenses\Apr-License.txt
Create		\Licenses\.weirdext
Delete		\Images\remdir\c.png
Delete		\Images\new.png
Create		\Images\addir\a.PNG
Create		\Images\a.png".Replace("\r\n", "\n");
                Utils.AssertEq(sExpected, sGot);
            }

            {
                CCreateSyncItem.SortFromColumnNumber(results.items, 1); // sort by type where Update() != Update
                var resultsFilteredStrings = from item in results.items where item.status != CCreateSyncItemStatus.Unknown select item.ToString();
                var sGot = String.Join("\n", resultsFilteredStrings);
                var sExpected = @"Create		\Images\a.png
Create		\Images\addir\a.PNG
Create		\Licenses\.weirdext
Create		\Licenses\noext
Delete		\Images\new.png
Delete		\Images\remdir\c.png
Update		\Licenses\Apr-License.txt
Update		\Licenses\Serf-License.txt
Update()		\Licenses\Cyrus-Sasl-License.txt
Update()		\Licenses\OpenSsl-License.txt".Replace("\r\n", "\n");
                Utils.AssertEq(sExpected, sGot);
            }

            {
                CCreateSyncItem.SortFromColumnNumber(results.items, 2); // sort by type where Update() == Update
                var resultsFilteredStrings = from item in results.items where item.status != CCreateSyncItemStatus.Unknown select item.ToString();
                var sGot = String.Join("\n", resultsFilteredStrings);
                var sExpected = @"Create		\Images\a.png
Create		\Images\addir\a.PNG
Create		\Licenses\.weirdext
Create		\Licenses\noext
Delete		\Images\new.png
Delete		\Images\remdir\c.png
Update		\Licenses\Apr-License.txt
Update()		\Licenses\Cyrus-Sasl-License.txt
Update()		\Licenses\OpenSsl-License.txt
Update		\Licenses\Serf-License.txt".Replace("\r\n", "\n");
                Utils.AssertEq(sExpected, sGot);
            }
        }

        static void TestMethod_TestGlobalFailure()
        {
            var config = new SyncConfiguration();
            config.m_src = "notexist1";
            config.m_destination = "notexist2";
            config.m_mirror = true;
            config.m_copySubDirsAndEmptySubdirs = true;

            string sLogFilename = RunImplementation.GetLogFilename();
            RunImplementation.Go(config, sLogFilename, true /*preview*/, false);
            var results = CCreateSyncResultsSet.ParseFromLogFile(config, sLogFilename, true /*preview*/);
            Utils.AssertEq(0, results.items.Count);
            Utils.AssertEq(true, results.sSummary.Contains("looks like errors occurred:"));
            Utils.AssertEq(true, results.sSummary.Contains("The system cannot find the file specified."));
            File.Delete(sLogFilename);
        }

        static void TestMethod_TestIndividualFailures()
        {
            var sDirectory = Testing.SetUpSynctestEnvironment();
            CCreateSyncResultsSet results;
            CCreateSyncResultsSet.showWarnings = false;
            using (Stream iStream2 = File.Open(sDirectory + "\\src\\Licenses\\OpenSsl-License.txt", FileMode.Append, FileAccess.Write, FileShare.None))
            {
                var config = GetRealConfig(sDirectory, 4 /*nThreads*/);
                config.m_nRetries = "1";
                config.m_waitBetweenRetries = "1";
                string sLogFilename = RunImplementation.GetLogFilename();
                RunImplementation.Go(config, sLogFilename, false /*preview*/, false);
                results = CCreateSyncResultsSet.ParseFromLogFile(config, sLogFilename, false /*preview*/);
                File.Delete(sLogFilename);
            }

            CCreateSyncResultsSet.showWarnings = true;

            Utils.AssertEq(0, results.items.Count);
            Utils.AssertEq(true, results.sSummary.Contains("Summary indicated failures"));
            Utils.AssertEq(true, results.sSummary.Contains("*EXTRA File"));
            Utils.AssertEq(true, results.sSummary.Contains("it is being used by another process"));
            Utils.AssertEq(true, results.sSummary.Contains("RETRY LIMIT EXCEEDED"));
            Utils.AssertEq(true, results.sSummary.Contains("Bytes :"));
        }

        private static SyncConfiguration GetRealConfig(string sDirectory, int nThreads)
        {
            var config = new SyncConfiguration();
            config.m_src = Path.GetFullPath(sDirectory + "\\src");
            config.m_destination = Path.GetFullPath(sDirectory + "\\dest");
            config.m_mirror = true;
            config.m_copySubDirsAndEmptySubdirs = true;
            config.m_fatTimes = false;

            if (nThreads > 1)
            {
                config.m_nThreads = "" + nThreads;
            }

            return config;
        }

        public static void RunTests()
        {
            if (!File.Exists(Testing.GetTestDirectory() + @"\testsync\src\Images\b.png"))
            {
                MessageBox.Show("Please unzip testFiles.zip to " + Path.GetFullPath(Testing.GetTestDirectory()) + @"\testsync");
                throw new Exception("Path not found.");
            }

            SyncConfiguration.s_disableMessageBox = true;
            try
            {
                Testing.CallAllTestMethods(typeof(RbcpyTests), null);
            }
            finally
            {
                SyncConfiguration.s_disableMessageBox = false;
            }
        }
    }
}

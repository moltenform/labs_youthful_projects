using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbcpy
{
    public static class RbcpyTests
    {
        static void TestMethod_Asserts_EqualIntsShouldCompareEqual()
        {
            Testing.AssertEqual(1, 1);
        }
        static void TestMethod_Asserts_EqualStringsShouldCompareEqual()
        {
            Testing.AssertEqual("abcd", "abcd");
        }
        static void TestMethod_Asserts_EqualBoolsShouldCompareEqual()
        {
            Testing.AssertEqual(true, true);
            Testing.AssertEqual(false, false);
        }
        static void TestMethod_Asserts_CheckAssertMessage()
        {
            Action fn = delegate() { throw new RbcpyTestException("test123"); };
            Testing.AssertExceptionMessageIncludes(fn, "test123");
        }
        static void TestMethod_Asserts_NonEqualIntsShouldCompareNonEqual()
        {
            Testing.AssertExceptionMessageIncludes(() => Testing.AssertEqual(1, 2), "expected 1 but got 2");
        }
        static void TestMethod_Asserts_NonEqualStrsShouldCompareNonEqual()
        {
            Testing.AssertExceptionMessageIncludes(() => Testing.AssertEqual("abcd", "abce"), "expected abcd but got abce");
        }
        static void TestMethod_Asserts_NonEqualBoolsShouldCompareNonEqual()
        {
            Testing.AssertExceptionMessageIncludes(() => Testing.AssertEqual(true, false), "expected True but got False");
        }
        static void TestMethod_SyncConfiguration_Deserialize()
        {
            var config = SyncConfiguration.Deserialize(Testing.GetTestFile("test_cfg_01.xml"));
            Testing.AssertEqual(config.m_src, "chg1");
            Testing.AssertEqual(config.m_destination, "chg2");
            Testing.AssertEqual(config.m_excludeDirs, "chg3");
            Testing.AssertEqual(config.m_excludeFiles, "chg4");
            Testing.AssertEqual(config.m_mirror, true);
            Testing.AssertEqual(config.m_copySubDirsAndEmptySubdirs, false);
            Testing.AssertEqual(config.m_copyFlags, "#1");
            Testing.AssertEqual(config.m_directoryCopyFlags, "#2");
            Testing.AssertEqual(config.m_ipg, "#3");
            Testing.AssertEqual(config.m_nRetries, "#4");
            Testing.AssertEqual(config.m_waitBetweenRetries, "#5");
            Testing.AssertEqual(config.m_nThreads, "#6");
            Testing.AssertEqual(config.m_custom, "#7");
            Testing.AssertEqual(config.m_symlinkNotTarget, true);
            Testing.AssertEqual(config.m_fatTimes, false);
            Testing.AssertEqual(config.m_compensateDst, true);
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
            Testing.AssertEqual(sExpected, sGot);
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
            Testing.AssertEqual(true, SyncConfiguration.Validate(GetValidConfig()));
        }
        static void TestMethod_ValidationShouldFailBadSrc()
        {
            // make it not validate
            var config = GetValidConfig();
            config.m_src = "notexist";
            Testing.AssertEqual(false, SyncConfiguration.Validate(config));
        }
        static void TestMethod_ValidationShouldFailBadDestination()
        {
            // make it not validate
            var config = GetValidConfig();
            config.m_destination = "notexist";
            Testing.AssertEqual(false, SyncConfiguration.Validate(config));
        }
        static void TestMethod_ValidationShouldFailSrcEndsWithSlash()
        {
            var config = GetValidConfig();
            config.m_src += "\\";
            Testing.AssertEqual(false, SyncConfiguration.Validate(config));
        }
        static void TestMethod_ValidationShouldFailDestEndsWithSlash()
        {
            var config = GetValidConfig();
            config.m_destination += "\\";
            Testing.AssertEqual(false, SyncConfiguration.Validate(config));
        }
        static void TestMethod_ValidationShouldFailIfIntersection1()
        {
            var config = GetValidConfig();
            config.m_src = Testing.GetTestDirectory() + "\\testsync\\src";
            config.m_destination = Testing.GetTestDirectory() + "\\TestSync";
            Testing.AssertEqual(false, SyncConfiguration.Validate(config));
        }
        static void TestMethod_ValidationShouldFailIfIntersection2()
        {
            var config = GetValidConfig();
            config.m_src = Testing.GetTestDirectory() + "\\testSync";
            config.m_destination = Testing.GetTestDirectory() + "\\testsync\\dest";
            Testing.AssertEqual(false, SyncConfiguration.Validate(config));
        }
        static void TestMethod_ValidationNThreadsShouldBeInt()
        {
            // make it not validate
            var config = GetValidConfig();
            config.m_nThreads = "4a";
            Testing.AssertEqual(false, SyncConfiguration.Validate(config));
            config.m_nThreads = "5";
            Testing.AssertEqual(true, SyncConfiguration.Validate(config));
            config.m_nRetries = "6a";
            Testing.AssertEqual(false, SyncConfiguration.Validate(config));
            config.m_nRetries = "7";
            Testing.AssertEqual(true, SyncConfiguration.Validate(config));
        }
        static void TestMethod_ValidationQuotesAreDisallowed()
        {
            var config = GetValidConfig();
            config.m_copyFlags = "with\"quote";
            Testing.AssertEqual(false, SyncConfiguration.Validate(config));
            config.m_copyFlags = "without quote";
            Testing.AssertEqual(true, SyncConfiguration.Validate(config));
            config.m_custom = "ok to have with\"quote";
            Testing.AssertEqual(true, SyncConfiguration.Validate(config));
        }
        static void TestMethod_GetCommandLineParameters01()
        {
            var config = SyncConfiguration.Deserialize(Testing.GetTestFile("test_cfg_01.xml"));
            var sArguments = RunImplementation.GetCommandLineArgs(config);
            var sExpected = " \"chg1\" \"chg2\"  /XD \"chg3\"  /XF \"chg4\"  /MIR  /COPY:#1  /DCOPY:#2  /IPG:#3  /R:#4  /W:#5  /MT:#6  #7  /SL  /DST ";
            Testing.AssertEqual(sExpected, sArguments);
        }
        static void TestMethod_GetCommandLineParameters02()
        {
            var config = SyncConfiguration.Deserialize(Testing.GetTestFile("test_cfg_02.xml"));
            var sArguments = RunImplementation.GetCommandLineArgs(config);
            var sExpected = " \"chg8\" \"chg9\"  /XD \"chg10\"  /XF \"chg11\"  /E  /COPY:#1@  /DCOPY:#2@  /IPG:#3@  /R:#4@  /W:#5@  /MT:#6@  #7@  /FFT ";
            Testing.AssertEqual(sExpected, sArguments);
        }
        static void TestMethod_TestPreviewOfFileSyncAndActualSync()
        {
            string sDirectory = Testing.SetUpSynctestEnvironment();
            TestSyncPreview(sDirectory, 1 /*nThreads*/);
            TestSyncPreview(sDirectory, 4 /*nThreads*/);

            TestActualSync(sDirectory, 1 /*nThreads*/);
            Testing.SetUpSynctestEnvironment();
            TestActualSync(sDirectory, 4 /*nThreads*/);
            TestActualDeleteInternalDuplicates(sDirectory);
        }

        static void TestActualSync(string sDirectory, int nThreads)
        {
            var config = GetRealConfig(sDirectory, nThreads);
            string sLogFilename = RunImplementation.GetLogFilename();
            RunImplementation.Go(config, sLogFilename, false /*preview*/, false);
            var results = CCreateSyncResultsSet.ParseFromLogFile(config, sLogFilename, false /*preview*/);
            File.Delete(sLogFilename);
            Testing.AssertEqual("Total    Copied   Skipped  Mismatch    Failed    Extras\r\n    Dirs :         5         5         2         0         0         2\r\n   Files :        22         8        14         0         0         2\r\n   Bytes :   632.0 k   126.6 k   505.4 k         0         0    30.7 k", results.sSummary.Trim());

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
            Testing.AssertEqual(new FileInfo(sDirectory + "\\src\\Licenses\\Cyrus-Sasl-License.txt").Length, 1861L);
            Testing.AssertEqual(new FileInfo(sDirectory + "\\src\\Licenses\\OpenSsl-License.txt").Length, 6286L);
            Testing.AssertEqual(new FileInfo(sDirectory + "\\src\\Licenses\\Apr-License.txt").Length, 18324L);
            Testing.AssertEqual(new FileInfo(sDirectory + "\\src\\Licenses\\Serf-License.txt").Length, 11562L);
            Testing.AssertEqual(new FileInfo(sDirectory + "\\dest\\Licenses\\Cyrus-Sasl-License.txt").Length, 1861L);
            Testing.AssertEqual(new FileInfo(sDirectory + "\\dest\\Licenses\\OpenSsl-License.txt").Length, 6286L);
            Testing.AssertEqual(new FileInfo(sDirectory + "\\dest\\Licenses\\Apr-License.txt").Length, 18324L);
            Testing.AssertEqual(new FileInfo(sDirectory + "\\dest\\Licenses\\Serf-License.txt").Length, 11562L);
        }

        static void TestMethod_TestActualCheckDuplicatesSync()
        {
            string sDirectory = Testing.SetUpSynctestEnvironment();
            var config = GetRealConfig(sDirectory, 1 /*nThreads*/);
            config.m_isDeleteDuplicates = true;
            File.WriteAllText(config.m_src + "\\short.txt", "a");
            File.WriteAllText(config.m_src + "\\shortbl.txt", "");
            File.WriteAllText(config.m_destination + "\\Images\\short2.txt", "a");
            File.WriteAllText(config.m_destination + "\\shortbl2.dat", "");

            var previewResults = RunDelDupes.Run(config);
            Testing.AssertEqual(false, previewResults.sSummary.Contains("find internal"));
            Testing.AssertEqual(true, previewResults.sSummary.Contains(" 2 small files in dest and 2 small files in src"));
            Testing.AssertEqual(14, previewResults.items.Count);
            var nFilesBefore = Directory.GetFileSystemEntries(sDirectory, "*", SearchOption.AllDirectories).ToList().Count;
            var realResults = RunDelDupes.ExecuteResultsSet(previewResults);
            List<string> filesGot = Directory.GetFileSystemEntries(sDirectory, "*", SearchOption.AllDirectories).ToList();
            var nFilesAfter = filesGot.Count;
            Testing.AssertEqual(false, realResults.sSummary.Contains("find internal"));
            Testing.AssertEqual(true, realResults.sSummary.Contains(" 2 small files in dest and 2 small files in src"));
            Testing.AssertEqual(14, realResults.items.Count);
            Testing.AssertEqual(14, nFilesBefore - nFilesAfter);
            string[] filesExpected = @"..\..\test\testsync\dest
..\..\test\testsync\dest\Images
..\..\test\testsync\dest\Images\b.png
..\..\test\testsync\dest\Images\c.png
..\..\test\testsync\dest\Images\d.png
..\..\test\testsync\dest\Images\DB44-20-x64.jpg
..\..\test\testsync\dest\Images\e.png
..\..\test\testsync\dest\Images\f.gif
..\..\test\testsync\dest\Images\new.png
..\..\test\testsync\dest\Images\remdir
..\..\test\testsync\dest\Images\remdir\c.png
..\..\test\testsync\dest\Images\remempty
..\..\test\testsync\dest\Images\short2.txt
..\..\test\testsync\dest\Licenses
..\..\test\testsync\dest\Licenses\Apr-License.txt
..\..\test\testsync\dest\Licenses\Apr-Util-License.txt
..\..\test\testsync\dest\Licenses\BerkeleyDB-License.txt
..\..\test\testsync\dest\Licenses\Cyrus-Sasl-License.txt
..\..\test\testsync\dest\Licenses\GetText-Runtime-License.txt
..\..\test\testsync\dest\Licenses\OpenSsl-License.txt
..\..\test\testsync\dest\Licenses\Serf-License.txt
..\..\test\testsync\dest\Licenses\SharpSvn-License.txt
..\..\test\testsync\dest\Licenses\Subversion-License.txt
..\..\test\testsync\dest\loren.html
..\..\test\testsync\dest\loren.txt
..\..\test\testsync\dest\pic1.png
..\..\test\testsync\dest\shortbl2.dat
..\..\test\testsync\src
..\..\test\testsync\src\Images
..\..\test\testsync\src\Images\a.png
..\..\test\testsync\src\Images\addempty
..\..\test\testsync\src\Images\addir
..\..\test\testsync\src\Images\addir\a.PNG
..\..\test\testsync\src\Licenses
..\..\test\testsync\src\Licenses\.weirdext
..\..\test\testsync\src\Licenses\Apr-License.txt
..\..\test\testsync\src\Licenses\Cyrus-Sasl-License.txt
..\..\test\testsync\src\Licenses\noext
..\..\test\testsync\src\Licenses\OpenSsl-License.txt
..\..\test\testsync\src\Licenses\Serf-License.txt
..\..\test\testsync\src\short.txt
..\..\test\testsync\src\shortbl.txt".Replace("\r\n", "\n").Replace(@"..\..\test\testsync\", sDirectory + "\\").Split(new char[] { '\n' });
            filesGot.Sort();
            Testing.AssertStringArrayEqual(filesExpected, filesGot);
        }

        private static void TestSyncPreview(string sDirectory, int nThreads)
        {
            var config = GetRealConfig(sDirectory, nThreads);

            string sLogFilename = RunImplementation.GetLogFilename();
            RunImplementation.Go(config, sLogFilename, true /*preview*/, false);
            var results = CCreateSyncResultsSet.ParseFromLogFile(config, sLogFilename, true /*preview*/);
            Testing.AssertEqual("Total    Copied   Skipped  Mismatch    Failed    Extras\r\n    Dirs :         5         5         2         0         0         2\r\n   Files :        22         8        14         0         0         2\r\n   Bytes :   632.0 k   126.6 k   505.4 k         0         0    30.7 k", results.sSummary.Trim());
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
                Testing.AssertEqual(sExpected, sGot);
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
                Testing.AssertEqual(sExpected, sGot);
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
                Testing.AssertEqual(sExpected, sGot);
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
                Testing.AssertEqual(sExpected, sGot);
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
            Testing.AssertEqual(0, results.items.Count);
            Testing.AssertEqual(true, results.sSummary.Contains("looks like errors occurred:"));
            Testing.AssertEqual(true, results.sSummary.Contains("The system cannot find the file specified."));
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

            Testing.AssertEqual(0, results.items.Count);
            Testing.AssertEqual(true, results.sSummary.Contains("Summary indicated failures"));
            Testing.AssertEqual(true, results.sSummary.Contains("*EXTRA File"));
            Testing.AssertEqual(true, results.sSummary.Contains("it is being used by another process"));
            Testing.AssertEqual(true, results.sSummary.Contains("RETRY LIMIT EXCEEDED"));
            Testing.AssertEqual(true, results.sSummary.Contains("Bytes :"));
        }

        static void TestActualDeleteInternalDuplicates(string sDirectory)
        {
            var config = GetRealConfig(sDirectory, 1 /*nThreads*/);
            config.m_isDeleteDuplicates = true;
            config.m_src = config.m_destination;
            File.Copy(config.m_src + "\\loren.txt", config.m_src + "\\Images\\L2");
            File.Copy(config.m_src + "\\loren.txt", config.m_src + "\\Images\\addir\\L3");
            File.Copy(config.m_src + "\\loren.txt", config.m_src + "\\Images\\addir\\L4");
            File.AppendAllText(config.m_src + "\\Images\\addir\\L4", "a");
            File.WriteAllText(config.m_src + "\\short.txt", "a");
            File.WriteAllText(config.m_src + "\\Images\\short2.txt", "a");
            File.WriteAllText(config.m_src + "\\shortbl.txt", "");
            File.WriteAllText(config.m_src + "\\Images\\shortbl2.dat", "");

            var previewResults = RunDelDupes.Run(config);
            Testing.AssertEqual(true, previewResults.sSummary.Contains("find internal"));
            Testing.AssertEqual(true, previewResults.sSummary.Contains(" 4 small files"));
            Testing.AssertEqual(3, previewResults.items.Count);
            var realResults = RunDelDupes.ExecuteResultsSet(previewResults);
            Testing.AssertEqual(true, realResults.sSummary.Contains("find internal"));
            Testing.AssertEqual(true, realResults.sSummary.Contains(" 4 small files"));
            Testing.AssertEqual(3, realResults.items.Count);
            string[] filesExpected = @"..\..\test\testsync\dest
..\..\test\testsync\dest\Images
..\..\test\testsync\dest\Images\a.png
..\..\test\testsync\dest\Images\addempty
..\..\test\testsync\dest\Images\addir
..\..\test\testsync\dest\Images\addir\L4
..\..\test\testsync\dest\Images\b.png
..\..\test\testsync\dest\Images\c.png
..\..\test\testsync\dest\Images\d.png
..\..\test\testsync\dest\Images\DB44-20-x64.jpg
..\..\test\testsync\dest\Images\e.png
..\..\test\testsync\dest\Images\f.gif
..\..\test\testsync\dest\Images\short2.txt
..\..\test\testsync\dest\Images\shortbl2.dat
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
..\..\test\testsync\dest\short.txt
..\..\test\testsync\dest\shortbl.txt
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
                config.m_nThreads = "" + nThreads;
            return config;
        }

        public static void RunTests()
        {
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

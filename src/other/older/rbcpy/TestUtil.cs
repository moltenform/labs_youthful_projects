using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace rbcpy
{
    public class RbcpyTestException : ApplicationException
    {
        public RbcpyTestException(string message) : base(message) { }
    }

    public static class Testing
    {
        public static void AssertEqual(object expected, object actual)
        {
            if (!expected.Equals(actual))
            {
                throw new RbcpyTestException("Assertion failure, expected " + expected + " but got " + actual);
            }
        }

        public static void AssertStringArrayEqual(IList<string> expected, IList<string> actual)
        {
            AssertEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                AssertEqual(expected[i], actual[i]);
            }
        }

        public static void AssertNotNull(object obj)
        {
            if (obj == null)
            {
                throw new RbcpyTestException("Assertion failure, object is null");
            }
        }

        public static void AssertIsNull(object obj)
        {
            if (obj != null)
            {
                throw new RbcpyTestException("Assertion failure, object is not null");
            }
        }

        public static void AssertExceptionMessageIncludes(Action fn, string strExpect)
        {
            string strException = "";
            try
            {
                fn();
            }
            catch (Exception exc)
            {
                strException = exc.ToString();
            }
            if (!strException.Contains(strExpect))
            {
                throw new RbcpyTestException("Testing.AssertExceptionMessageIncludes expected " + strExpect + " but got " + strException + ".");
            }
        }

        public static string GetTestDirectory()
        {
            if (Directory.Exists(@"..\..\test"))
            {
                return @"..\..\test";
            }
            else if (Directory.Exists(@".\test\"))
            {
                return @".\test";
            }
            else if (Directory.Exists(@"..\..\..\test"))
            {
                return @"..\..\..\test";
            }
            else
            {
                throw new RbcpyTestException("test directory not found.");
            }
        }
        public static string GetTestFile(string sFilename)
        {
            string ret = GetTestDirectory() + "\\" + sFilename;
            if (File.Exists(ret))
            {
                return ret;
            }
            else
            {
                throw new RbcpyTestException("test file " + sFilename + " not found.");
            }
        }

        public static string GetTestTempFile(string p)
        {
            if (!Directory.Exists(@".\configs\tmp"))
            {
                Directory.CreateDirectory(@".\configs\tmp");
            }

            return @".\configs\tmp\" + p;
        }

        public static string SetUpSynctestEnvironment()
        {
            var testSyncPath = GetTestTempFile("testsync");
            if (!Directory.Exists(testSyncPath))
            {
                Directory.CreateDirectory(testSyncPath);
            }

            // use rbcpy itself to set up the sync test :)
            var config = new SyncConfiguration();
            config.m_src = GetTestDirectory() + "\\testsync";
            config.m_destination = testSyncPath;
            config.m_mirror = true;
            config.m_copySubDirsAndEmptySubdirs = true;
            config.m_copyFlags = "DA"; // don't copy times

            string sLogFilename = RunImplementation.GetLogFilename();
            RunImplementation.Go(config, sLogFilename, false /*preview*/, false);
            File.Delete(sLogFilename);
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
..\..\test\testsync\src\pic1.png".Replace("\r\n", "\n").Replace(@"..\..\test\testsync\", testSyncPath+"\\").Split(new char[] { '\n' });
            List<string> filesGot = Directory.GetFileSystemEntries(testSyncPath, "*", SearchOption.AllDirectories).ToList();
            filesGot.Sort();
            var sFilesExpected = string.Join("\r\n", filesExpected);
            var sFilesGot = string.Join("\r\n", filesGot);
            Utils.AssertEq(sFilesExpected, sFilesGot);
            Utils.AssertEq(new FileInfo(testSyncPath + "\\src\\Licenses\\Cyrus-Sasl-License.txt").Length, 1861L);
            Utils.AssertEq(new FileInfo(testSyncPath + "\\src\\Licenses\\OpenSsl-License.txt").Length, 6286L);
            Utils.AssertEq(new FileInfo(testSyncPath + "\\src\\Licenses\\Apr-License.txt").Length, 18324L);
            Utils.AssertEq(new FileInfo(testSyncPath + "\\src\\Licenses\\Serf-License.txt").Length, 11562L);
            Utils.AssertEq(new FileInfo(testSyncPath + "\\dest\\Licenses\\Cyrus-Sasl-License.txt").Length, 1865L);
            Utils.AssertEq(new FileInfo(testSyncPath + "\\dest\\Licenses\\OpenSsl-License.txt").Length, 6288L);
            Utils.AssertEq(new FileInfo(testSyncPath + "\\dest\\Licenses\\Apr-License.txt").Length, 18320L);
            Utils.AssertEq(new FileInfo(testSyncPath + "\\dest\\Licenses\\Serf-License.txt").Length, 11558L);

            // adjust file mod times
            MakeFileNewer(testSyncPath + "\\dest\\Licenses\\Cyrus-Sasl-License.txt");
            MakeFileNewer(testSyncPath + "\\dest\\Licenses\\OpenSsl-License.txt");
            MakeFileNewer(testSyncPath + "\\src\\Licenses\\Apr-License.txt");
            MakeFileNewer(testSyncPath + "\\src\\Licenses\\Serf-License.txt");
            return testSyncPath;
        }

        private static void MakeFileNewer(string path)
        {
            var filetime = File.GetLastWriteTime(path);
            filetime.AddSeconds(10);
            File.SetLastWriteTime(path, filetime);
        }

        public static void CallAllTestMethods(Type type, object[] arParams)
        {
            MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
            foreach (MethodInfo methodInfo in methodInfos)
            {
                if (methodInfo.Name.StartsWith("TestMethod_"))
                {
                    Utils.AssertTrue(methodInfo.GetParameters().Length == 0);
                    methodInfo.Invoke(null, arParams);
                }
            }
        }
    }
}

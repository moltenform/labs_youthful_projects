public static class CoordinatePicturesTests
    {
        static void TestMethod_Asserts_EqualIntsShouldCompareEqual()
        {
            TestUtil.IsEq(1, 1);
        }

        static void TestMethod_Asserts_EqualStringsShouldCompareEqual()
        {
            TestUtil.IsEq("abcd", "abcd");
        }

        static void TestMethod_Asserts_EqualBoolsShouldCompareEqual()
        {
            TestUtil.IsEq(true, true);
            TestUtil.IsEq(false, false);
        }

        static void TestMethod_Asserts_IsStringArrayEq()
        {
            TestUtil.IsStringArrayEq(null, null);
            TestUtil.IsStringArrayEq(null, new string[] { });
            TestUtil.IsStringArrayEq("", new string[] { "" });
            TestUtil.IsStringArrayEq("|", new string[] { "", "" });
            TestUtil.IsStringArrayEq("||", new string[] { "", "", "" });
            TestUtil.IsStringArrayEq("aa|bb|cc", new string[] { "aa", "bb", "cc" });
        }

        static void TestMethod_Asserts_CheckAssertMessage()
        {
            Action fn = () =>
            {
                throw new CoordinatePicturesTestException("test123");
            };

            TestUtil.AssertExceptionMessage(fn, "test123");
        }

        static void TestMethod_Asserts_NonEqualIntsShouldCompareNonEqual()
        {
            TestUtil.AssertExceptionMessage(() => TestUtil.IsEq(1, 2),
                "expected 1 but got 2");
        }

        static void TestMethod_Asserts_NonEqualStrsShouldCompareNonEqual()
        {
            TestUtil.AssertExceptionMessage(() => TestUtil.IsEq("abcd", "abce"),
                "expected abcd but got abce");
        }

        static void TestMethod_Asserts_NonEqualBoolsShouldCompareNonEqual()
        {
            TestUtil.AssertExceptionMessage(() => TestUtil.IsEq(true, false),
                "expected True but got False");
        }

        static string PathSep(string s)
        {
            // replace slashes with platform appropriate character
            return s.Replace("/", Utils.Sep);
        }

        static void TestMethod_UtilsSameExceptExtension()
        {
            TestUtil.IsTrue(FilenameUtils.SameExceptExtension(
                "test6.jpg", "test6.jpg"));
            TestUtil.IsTrue(FilenameUtils.SameExceptExtension(
                "test6.jpg", "test6.png"));
            TestUtil.IsTrue(FilenameUtils.SameExceptExtension(
                "test6.jpg", "test6.BMP"));
            TestUtil.IsTrue(!FilenameUtils.SameExceptExtension(
                "test6.jpg", "test6.jpg.jpg"));
            TestUtil.IsTrue(!FilenameUtils.SameExceptExtension(
                "test6a.jpg", "test6.jpg"));
            TestUtil.IsTrue(FilenameUtils.SameExceptExtension(
                "aa.jpg.test6.jpg", "aa.jpg.test6.bmp"));
            TestUtil.IsTrue(!FilenameUtils.SameExceptExtension(
                "aa.jpg.test6.jpg", "aa.bmp.test6.jpg"));

            TestUtil.IsTrue(FilenameUtils.SameExceptExtension(
                PathSep("a/test6.jpg"), PathSep("a/test6.jpg")));
            TestUtil.IsTrue(!FilenameUtils.SameExceptExtension(
                PathSep("a/test6.jpg"), PathSep("b/test6.jpg")));
        }

        static void TestMethod_UtilsIsDigits()
        {
            TestUtil.IsTrue(!Utils.IsDigits(null));
            TestUtil.IsTrue(!Utils.IsDigits(""));
            TestUtil.IsTrue(Utils.IsDigits("0"));
            TestUtil.IsTrue(Utils.IsDigits("0123"));
            TestUtil.IsTrue(Utils.IsDigits("456789"));
            TestUtil.IsTrue(!Utils.IsDigits("456789a"));
            TestUtil.IsTrue(!Utils.IsDigits("a456789a"));
        }

        static void TestMethod_UtilsFirstTwoChars()
        {
            TestUtil.IsEq("", Utils.FirstTwoChars(""));
            TestUtil.IsEq("a", Utils.FirstTwoChars("a"));
            TestUtil.IsEq("ab", Utils.FirstTwoChars("ab"));
            TestUtil.IsEq("ab", Utils.FirstTwoChars("abc"));
        }

        static void TestMethod_UtilsArePathsDistinct()
        {
            TestUtil.IsTrue(!Utils.ArePathsDistinct("", ""));
            TestUtil.IsTrue(!Utils.ArePathsDistinct("a", "a"));
            TestUtil.IsTrue(!Utils.ArePathsDistinct(@"C:\A", @"C:\A"));
            TestUtil.IsTrue(!Utils.ArePathsDistinct(@"C:\A", @"C:\a"));
            TestUtil.IsTrue(!Utils.ArePathsDistinct(@"C:\A\subdir", @"C:\A"));
            TestUtil.IsTrue(!Utils.ArePathsDistinct(@"C:\A", @"C:\A\subdir"));
            TestUtil.IsTrue(Utils.ArePathsDistinct(@"C:\A", @"C:\AA"));
            TestUtil.IsTrue(Utils.ArePathsDistinct(@"C:\abc", @"C:\ABCDE"));
        }

        static void TestMethod_UtilsCombineProcessArguments()
        {
            TestUtil.IsEq("", Utils.CombineProcessArguments(new string[] { }));
            TestUtil.IsEq("\"\"", Utils.CombineProcessArguments(new string[] { "" }));
            TestUtil.IsEq("\"\\\"\"", Utils.CombineProcessArguments(new string[] { "\"" }));
            TestUtil.IsEq("\"\\\"\\\"\"", Utils.CombineProcessArguments(new string[] { "\"\"" }));
            TestUtil.IsEq("\"\\\"a\\\"\"", Utils.CombineProcessArguments(new string[] { "\"a\"" }));
            TestUtil.IsEq("\\", Utils.CombineProcessArguments(new string[] { "\\" }));
            TestUtil.IsEq("\"a b\"", Utils.CombineProcessArguments(new string[] { "a b" }));
            TestUtil.IsEq("a \" b\"", Utils.CombineProcessArguments(new string[] { "a", " b" }));
            TestUtil.IsEq("a\\\\b", Utils.CombineProcessArguments(new string[] { "a\\\\b" }));
            TestUtil.IsEq("\" \\\\\"", Utils.CombineProcessArguments(new string[] { " \\" }));
            TestUtil.IsEq("\" \\\\\\\"\"", Utils.CombineProcessArguments(new string[] { " \\\"" }));
            TestUtil.IsEq("\" \\\\\\\\\"", Utils.CombineProcessArguments(new string[] { " \\\\" }));

            TestUtil.IsEq("\"a\\\\b c\"", Utils.CombineProcessArguments(
                new string[] { "a\\\\b c" }));
            TestUtil.IsEq("\"C:\\Program Files\\\\\"", Utils.CombineProcessArguments(
                new string[] { "C:\\Program Files\\" }));
            TestUtil.IsEq("\"dafc\\\"\\\"\\\"a\"", Utils.CombineProcessArguments(
                new string[] { "dafc\"\"\"a" }));
        }

        static void TestMethod_UtilsFormatPythonError()
        {
            TestUtil.IsEq("", Utils.FormatPythonError(""));
            TestUtil.IsEq("NotError", Utils.FormatPythonError("NotError"));
            TestUtil.IsEq("Not Error: Noterror",
                Utils.FormatPythonError("Not Error: Noterror"));
            TestUtil.IsEq("IsError: Details" + Utils.NL + Utils.NL + Utils.NL +
                "Details: text before IsError: Details",
                Utils.FormatPythonError("text before IsError: Details"));
            TestUtil.IsEq("IsError:2 some words" + Utils.NL + Utils.NL + Utils.NL +
                "Details: text before IsError:1 IsError:2 some words",
                Utils.FormatPythonError("text before IsError:1 IsError:2 some words"));

            var sampleStderr = "test failed, stderr = Traceback (most recent call last): File " +
                "reallylong string reallylong string reallylong string reallylong string " +
                ", line 1234, in test.py, raise RuntimeError(errMsg)RuntimeError: the actual msg";
            TestUtil.IsEq("RuntimeError: the actual msg" + Utils.NL + Utils.NL + Utils.NL +
                "Details: " + sampleStderr,
                Utils.FormatPythonError(sampleStderr));
        }

        static void TestMethod_UtilsGetFirstHttpLink()
        {
            TestUtil.IsEq(null, Utils.GetFirstHttpLink(""));
            TestUtil.IsEq(null, Utils.GetFirstHttpLink("no urls present http none"));
            TestUtil.IsEq("http://www.ok.com", Utils.GetFirstHttpLink("http://www.ok.com"));

            TestUtil.IsEq("http://www.ok.com", Utils.GetFirstHttpLink(
                "http://www.ok.com a b c http://www.second.com"));
            TestUtil.IsEq("http://www.ok.com", Utils.GetFirstHttpLink(
                "a b c http://www.ok.com a b c http://www.second.com"));
        }

        static void TestMethod_UtilsFormatFilesize()
        {
            TestUtil.IsEq(" (2.00mb)", Utils.FormatFilesize((2 * 1024 * 1024) + 25));
            TestUtil.IsEq(" (1.02mb)", Utils.FormatFilesize((1024 * 1024) + 20000));
            TestUtil.IsEq(" (345k)", Utils.FormatFilesize((1024 * 345) + 25));
            TestUtil.IsEq(" (1k)", Utils.FormatFilesize(1025));
            TestUtil.IsEq(" (1k)", Utils.FormatFilesize(1024));
            TestUtil.IsEq(" (1k)", Utils.FormatFilesize(1023));
            TestUtil.IsEq(" (1k)", Utils.FormatFilesize(1));
            TestUtil.IsEq(" (0k)", Utils.FormatFilesize(0));
        }

        static void TestMethod_ArrayAt()
        {
            var arr = new int[] { 1, 2, 3 };
            TestUtil.IsEq(1, Utils.ArrayAt(arr, -100));
            TestUtil.IsEq(1, Utils.ArrayAt(arr, -1));
            TestUtil.IsEq(1, Utils.ArrayAt(arr, 0));
            TestUtil.IsEq(2, Utils.ArrayAt(arr, 1));
            TestUtil.IsEq(3, Utils.ArrayAt(arr, 2));
            TestUtil.IsEq(3, Utils.ArrayAt(arr, 3));
            TestUtil.IsEq(3, Utils.ArrayAt(arr, 100));
        }

        static void TestMethod_LooksLikePath()
        {
            TestUtil.IsTrue(!Utils.LooksLikePath(""));
            TestUtil.IsTrue(!Utils.LooksLikePath("/"));
            TestUtil.IsTrue(!Utils.LooksLikePath("\\"));
            TestUtil.IsTrue(!Utils.LooksLikePath("C:"));
            TestUtil.IsTrue(Utils.LooksLikePath("C:\\"));
            TestUtil.IsTrue(Utils.LooksLikePath("C:\\a\\b\\c"));
            TestUtil.IsTrue(Utils.LooksLikePath("\\test"));
            TestUtil.IsTrue(Utils.LooksLikePath("\\test\\a\\b\\c"));
        }

        static void TestMethod_GetFileAttributes()
        {
            var path = Path.Combine(TestUtil.GetTestWriteDirectory(), "testhash.txt");
            File.WriteAllText(path, "12345678");
            TestUtil.IsEq(File.GetAttributes(path), Utils.GetFileAttributesOrNone(path));

            var pathNotExist = Path.Combine(TestUtil.GetTestWriteDirectory(), "testhash2.txt");
            TestUtil.IsEq(false, File.Exists(pathNotExist));
            TestUtil.IsEq(FileAttributes.Normal, Utils.GetFileAttributesOrNone(pathNotExist));
        }

        static void TestMethod_TestSha512()
        {
            var path = Path.Combine(TestUtil.GetTestWriteDirectory(), "testhash.txt");
            File.WriteAllText(path, "12345678");
            TestUtil.IsEq("filenotfound:", Utils.GetSha512(null));
            TestUtil.IsEq("filenotfound:notexist", Utils.GetSha512("notexist"));
            TestUtil.IsEq("+lhdichR3TOKcNz1Naoqkv7ng23Wr/EiZYPojgmWK" +
                "T8WvACcZSgm4PxccGaVoDzdzjcvE57/TROVnabx9dPqvg==", Utils.GetSha512(path));
        }

        static void TestMethod_SplitByString()
        {
            TestUtil.IsStringArrayEq("", Utils.SplitByString("", "delim"));
            TestUtil.IsStringArrayEq("|", Utils.SplitByString("delim", "delim"));
            TestUtil.IsStringArrayEq("||", Utils.SplitByString("delimdelim", "delim"));
            TestUtil.IsStringArrayEq("a||b", Utils.SplitByString("adelimdelimb", "delim"));
            TestUtil.IsStringArrayEq("a|bb|c", Utils.SplitByString("adelimbbdelimc", "delim"));

            // make sure regex special characters are treated as normal chars
            TestUtil.IsStringArrayEq("a|bb|c", Utils.SplitByString("a**bb**c", "**"));
            TestUtil.IsStringArrayEq("a|bb|c", Utils.SplitByString("a?bb?c", "?"));
        }

        static void TestMethod_SoftDeleteDefaultDir()
        {
            var fakeFile = PathSep("C:/dirtest/test.doc");
            var deleteDir = Utils.GetSoftDeleteDirectory(fakeFile);
            var deleteDest = Utils.GetSoftDeleteDestination(fakeFile);
            TestUtil.IsTrue(deleteDest.StartsWith(deleteDir +
                Utils.Sep + "di_test.doc", StringComparison.Ordinal));
        }

        static void TestMethod_IsExtensionInList()
        {
            var exts = new string[] { ".jpg", ".png" };
            TestUtil.IsTrue(!FilenameUtils.IsExtensionInList("", exts));
            TestUtil.IsTrue(!FilenameUtils.IsExtensionInList("png", exts));
            TestUtil.IsTrue(!FilenameUtils.IsExtensionInList("a.bmp", exts));
            TestUtil.IsTrue(FilenameUtils.IsExtensionInList("a.png", exts));
            TestUtil.IsTrue(FilenameUtils.IsExtensionInList("a.PNG", exts));
            TestUtil.IsTrue(FilenameUtils.IsExtensionInList("a.jpg", exts));
            TestUtil.IsTrue(FilenameUtils.IsExtensionInList("a.bmp.jpg", exts));
            TestUtil.IsTrue(FilenameUtils.IsExt("a.png", ".png"));
            TestUtil.IsTrue(FilenameUtils.IsExt("a.PNG", ".png"));
            TestUtil.IsTrue(!FilenameUtils.IsExt("apng", ".png"));
            TestUtil.IsTrue(!FilenameUtils.IsExt("a.png", ".jpg"));
        }

        static void TestMethod_NumberedPrefix()
        {
            TestUtil.IsEq(PathSep("c:/test/([0000])abc.jpg"),
                FilenameUtils.AddNumberedPrefix(PathSep("c:/test/abc.jpg"), 0));
            TestUtil.IsEq(PathSep("c:/test/([0010])abc.jpg"),
                FilenameUtils.AddNumberedPrefix(PathSep("c:/test/abc.jpg"), 1));
            TestUtil.IsEq(PathSep("c:/test/([1230])abc.jpg"),
                FilenameUtils.AddNumberedPrefix(PathSep("c:/test/abc.jpg"), 123));
            TestUtil.IsEq(PathSep("c:/test/([1230])abc.jpg"),
                FilenameUtils.AddNumberedPrefix(PathSep("c:/test/([1230])abc.jpg"), 123));
            TestUtil.IsEq(PathSep("c:/test/([9999])abc.jpg"),
                FilenameUtils.AddNumberedPrefix(PathSep("c:/test/([9999])abc.jpg"), 123));
            TestUtil.IsEq(PathSep("a.jpg"),
                FilenameUtils.GetFileNameWithoutNumberedPrefix(PathSep("a.jpg")));
            TestUtil.IsEq(PathSep("abc.jpg"),
                FilenameUtils.GetFileNameWithoutNumberedPrefix(PathSep("c:/test/([9999])abc.jpg")));
            TestUtil.IsEq(PathSep("abc.jpg"),
                FilenameUtils.GetFileNameWithoutNumberedPrefix(PathSep("c:/test/([0000])abc.jpg")));
            TestUtil.IsEq(PathSep("abc.jpg"),
                FilenameUtils.GetFileNameWithoutNumberedPrefix(PathSep("c:/test/([1230])abc.jpg")));
        }

        static void TestMethod_UtilsGetCategory()
        {
            var testAdd = FilenameUtils.AddCategoryToFilename(PathSep("c:/dir/test/b b.aaa.jpg"), "mk");
            TestUtil.IsEq(PathSep("c:/dir/test/b b.aaa__MARKAS__mk.jpg"), testAdd);
            testAdd = FilenameUtils.AddCategoryToFilename(PathSep("c:/dir/test/b b.aaa.jpg"), "");
            TestUtil.IsEq(PathSep("c:/dir/test/b b.aaa__MARKAS__.jpg"), testAdd);

            Func<string, string> testGetCategory = (input) =>
            {
                FilenameUtils.GetCategoryFromFilename(input, out string pathWithoutCategory, out string category);
                return pathWithoutCategory + "|" + category;
            };

            TestUtil.IsEq(PathSep("C:/dir/test/file.jpg|123"),
                testGetCategory(PathSep("C:/dir/test/file__MARKAS__123.jpg")));
            TestUtil.IsEq(PathSep("C:/dir/test/file.also.jpg|123"),
                testGetCategory(PathSep("C:/dir/test/file.also__MARKAS__123.jpg")));
            TestUtil.IsEq(PathSep("C:/dir/test/file.jpg|"),
                testGetCategory(PathSep("C:/dir/test/file__MARKAS__.jpg")));

            // check that invalid paths cause exception to be thrown.
            TestUtil.AssertExceptionMessage(() => testGetCategory(
                PathSep("C:/dir/test/dirmark__MARKAS__b/file__MARKAS__123.jpg")), "Directories");
            TestUtil.AssertExceptionMessage(() => testGetCategory(
                PathSep("C:/dir/test/dirmark__MARKAS__b/file.jpg")), "Directories");
            TestUtil.AssertExceptionMessage(() => testGetCategory(
                PathSep("C:/dir/test/file__MARKAS__123__MARKAS__123.jpg")), "exactly 1");
            TestUtil.AssertExceptionMessage(() => testGetCategory(
                PathSep("C:/dir/test/file.jpg")), "exactly 1");
            TestUtil.AssertExceptionMessage(() => testGetCategory(
                PathSep("C:/dir/test/file__MARKAS__123.dir.jpg")), "after the marker");
        }

        static void TestMethod_FindSimilarFilenames()
        {
            var mode = new ModeCategorizeAndRename();
            var extensions = mode.GetFileTypes();
            bool nameHasSuffix;
            string pathWithoutSuffix;
            var filepaths = new string[] {
                PathSep("c:/a/a.png"),
                PathSep("c:/a/b.png"),
                PathSep("c:/a/ab.png"),
                PathSep("c:/a/b_out.png"),
                PathSep("c:/a/a_out.png"),
                PathSep("c:/a/a.png60.jpg"),
                PathSep("c:/a/a.png80.jpg"),
                PathSep("c:/b/a.png90.jpg") };

            // alone with no added suffix
            TestUtil.IsStringArrayEq(null, FindSimilarFilenames.FindSimilarNames(
                PathSep("c:/a/b.png"), extensions, filepaths,
                out nameHasSuffix, out pathWithoutSuffix));
            TestUtil.IsTrue(!nameHasSuffix);
            TestUtil.IsEq(null, pathWithoutSuffix);

            // alone with an added suffix
            TestUtil.IsStringArrayEq(null, FindSimilarFilenames.FindSimilarNames(
                PathSep("c:/b/a.png90.jpg"), extensions, filepaths,
                out nameHasSuffix, out pathWithoutSuffix));
            TestUtil.IsTrue(nameHasSuffix);
            TestUtil.IsEq(PathSep("c:/b/a.jpg"), pathWithoutSuffix);

            // has similar names with no added suffix
            TestUtil.IsStringArrayEq(PathSep("c:/a/a.png60.jpg|c:/a/a.png80.jpg"),
                FindSimilarFilenames.FindSimilarNames(
                    PathSep("c:/a/a.png"), extensions, filepaths,
                    out nameHasSuffix, out pathWithoutSuffix));
            TestUtil.IsTrue(!nameHasSuffix);
            TestUtil.IsEq(null, pathWithoutSuffix);

            // has similar names with an added suffix
            TestUtil.IsStringArrayEq(PathSep("c:/a/a.png|c:/a/a.png80.jpg"),
                FindSimilarFilenames.FindSimilarNames(
                    PathSep("c:/a/a.png60.jpg"), extensions, filepaths,
                    out nameHasSuffix, out pathWithoutSuffix));
            TestUtil.IsTrue(nameHasSuffix);
            TestUtil.IsEq(PathSep("c:/a/a.jpg"), pathWithoutSuffix);
        }

        static void TestMethod_Logging()
        {
            var path = Path.Combine(TestUtil.GetTestWriteDirectory(), "testlog.txt");
            var log = new SimpleLog(path, 1024);

            // write simple log entries
            log.WriteError("test e");
            log.WriteWarning("test w");
            log.WriteLog("test l");
            TestUtil.IsEq("\n[error] test e\n[warning] test w\ntest l",
                File.ReadAllText(path).Replace("\r\n", "\n"));

            // add until over the filesize limit
            for (int i = 0; i < 60; i++)
            {
                log.WriteLog("123456789012345");
            }

            // it's over the limit, but hasn't reached the period yet, so still a large file
            TestUtil.IsTrue(new FileInfo(path).Length > 1024);

            // reach the period, file will be reset
            for (int i = 0; i < 10; i++)
            {
                log.WriteLog("123456789012345");
            }

            // now the file will have been reset
            TestUtil.IsTrue(new FileInfo(path).Length < 1024);
        }

        static void TestMethod_ClassConfigsPersistedCommonUsage()
        {
            string path = Path.Combine(TestUtil.GetTestSubDirectory("testcfg"), "test.ini");
            Configs cfg = new Configs(path);
            cfg.LoadPersisted();

            // unset properties should return empty string
            TestUtil.IsEq("", cfg.Get(ConfigKey.EnablePersonalFeatures));
            TestUtil.IsEq("", cfg.Get(ConfigKey.EnableVerboseLogging));
            TestUtil.IsEq("", cfg.Get(ConfigKey.FilepathDeletedFilesDir));

            // from memory
            cfg.Set(ConfigKey.EnablePersonalFeatures, "data=with=equals=");
            cfg.Set(ConfigKey.EnableVerboseLogging, " data\twith\t tabs");
            TestUtil.IsEq("data=with=equals=", cfg.Get(ConfigKey.EnablePersonalFeatures));
            TestUtil.IsEq(" data\twith\t tabs", cfg.Get(ConfigKey.EnableVerboseLogging));
            TestUtil.IsEq("", cfg.Get(ConfigKey.FilepathDeletedFilesDir));

            // from disk
            cfg = new Configs(path);
            cfg.LoadPersisted();
            TestUtil.IsEq("data=with=equals=", cfg.Get(ConfigKey.EnablePersonalFeatures));
            TestUtil.IsEq(" data\twith\t tabs", cfg.Get(ConfigKey.EnableVerboseLogging));
            TestUtil.IsEq("", cfg.Get(ConfigKey.FilepathDeletedFilesDir));
        }

        static void TestMethod_ClassConfigsPersistedBools()
        {
            string path = Path.Combine(TestUtil.GetTestSubDirectory("testcfg"), "testbools.ini");
            Configs cfg = new Configs(path);

            // read and set bools
            TestUtil.IsEq(false, cfg.GetBool(ConfigKey.EnablePersonalFeatures));
            cfg.SetBool(ConfigKey.EnablePersonalFeatures, true);
            TestUtil.IsEq(true, cfg.GetBool(ConfigKey.EnablePersonalFeatures));
            cfg.SetBool(ConfigKey.EnablePersonalFeatures, false);
            TestUtil.IsEq(false, cfg.GetBool(ConfigKey.EnablePersonalFeatures));
        }

        static void TestMethod_ClassConfigsNewlinesShouldNotBeAccepted()
        {
            string path = Path.Combine(TestUtil.GetTestSubDirectory("testcfg"), "test.ini");
            Configs cfg = new Configs(path);
            TestUtil.AssertExceptionMessage(() => cfg.Set(
                ConfigKey.EnableVerboseLogging, "data\rnewline"), "cannot contain newline");
            TestUtil.AssertExceptionMessage(() => cfg.Set(
                ConfigKey.EnableVerboseLogging, "data\nnewline"), "cannot contain newline");
        }

        static void TestMethod_ClassConfigsInputBoxHistoryShouldHaveCorrespondingConfig()
        {
            // each enum value in InputBoxHistory should have a corresponding MRUvalue
            var checkUniqueness = new HashSet<int>();
            var count = 0;
            foreach (var historyKey in Enum.GetValues(
                typeof(InputBoxHistory)).Cast<InputBoxHistory>())
            {
                if (historyKey != InputBoxHistory.None)
                {
                    TestUtil.IsEq(true, Enum.TryParse<ConfigKey>(
                        "MRU" + historyKey.ToString(), out ConfigKey key));

                    TestUtil.IsTrue(key != ConfigKey.None);
                    checkUniqueness.Add((int)key);
                    count++;
                }
            }

            // check that InputBoxHistory keys are mapped to different ConfigKey keys.
            TestUtil.IsEq(count, checkUniqueness.Count);
        }

        static void TestMethod_ClassConfigsEnumsShouldHaveUniqueValues()
        {
            var listOfInts = Enum.GetValues(typeof(InputBoxHistory)).Cast<int>().ToList();
            var set = new HashSet<int>(listOfInts);
            TestUtil.IsEq(listOfInts.Count, set.Count);

            listOfInts = Enum.GetValues(typeof(ConfigKey)).Cast<int>().ToList();
            set = new HashSet<int>(listOfInts);
            TestUtil.IsEq(listOfInts.Count, set.Count);
        }

        static void TestMethod_FileListNavigation()
        {
            // create files
            var dir = TestUtil.GetTestSubDirectory("filelist");
            File.WriteAllText(Path.Combine(dir, "dd.png"), "content");
            File.WriteAllText(Path.Combine(dir, "cc.png"), "content");
            File.WriteAllText(Path.Combine(dir, "bb.png"), "content");
            File.WriteAllText(Path.Combine(dir, "aa.png"), "content");
            List<string> neighbors = new List<string>(new string[4]);

            { // test gonext, gofirst
                var nav = new FileListNavigation(dir, new string[] { ".png" }, true);
                TestUtil.IsEq(Path.Combine(dir, "aa.png"), nav.Current);

                nav.GoNextOrPrev(true, neighbors, neighbors.Count);
                TestUtil.IsEq(Path.Combine(dir, "bb.png"), nav.Current);
                TestUtil.IsStringArrayEq(
                    "%cc.png|%dd.png|%dd.png|%dd.png".Replace("%", dir + Utils.Sep), neighbors);

                nav.GoNextOrPrev(true, neighbors, neighbors.Count);
                TestUtil.IsEq(Path.Combine(dir, "cc.png"), nav.Current);
                TestUtil.IsStringArrayEq(
                    "%dd.png|%dd.png|%dd.png|%dd.png".Replace("%", dir + Utils.Sep), neighbors);

                nav.GoNextOrPrev(true, neighbors, neighbors.Count);
                TestUtil.IsEq(Path.Combine(dir, "dd.png"), nav.Current);
                TestUtil.IsStringArrayEq(
                    "%dd.png|%dd.png|%dd.png|%dd.png".Replace("%", dir + Utils.Sep), neighbors);

                nav.GoNextOrPrev(true, neighbors, neighbors.Count);
                TestUtil.IsEq(Path.Combine(dir, "dd.png"), nav.Current);
                TestUtil.IsStringArrayEq(
                    "%dd.png|%dd.png|%dd.png|%dd.png".Replace("%", dir + Utils.Sep), neighbors);

                nav.GoFirst();
                TestUtil.IsEq(Path.Combine(dir, "aa.png"), nav.Current);
            }

            { // test golast, goprev
                var nav = new FileListNavigation(dir, new string[] { ".png" }, true);
                TestUtil.IsEq(Path.Combine(dir, "aa.png"), nav.Current);

                nav.GoLast();
                TestUtil.IsEq(Path.Combine(dir, "dd.png"), nav.Current);

                nav.GoNextOrPrev(false, neighbors, neighbors.Count);
                TestUtil.IsEq(Path.Combine(dir, "cc.png"), nav.Current);
                TestUtil.IsStringArrayEq(
                    "%bb.png|%aa.png|%aa.png|%aa.png".Replace("%", dir + Utils.Sep), neighbors);

                nav.GoNextOrPrev(false, neighbors, neighbors.Count);
                TestUtil.IsEq(Path.Combine(dir, "bb.png"), nav.Current);
                TestUtil.IsStringArrayEq(
                    "%aa.png|%aa.png|%aa.png|%aa.png".Replace("%", dir + Utils.Sep), neighbors);

                nav.GoNextOrPrev(false, neighbors, neighbors.Count);
                TestUtil.IsEq(Path.Combine(dir, "aa.png"), nav.Current);
                TestUtil.IsStringArrayEq(
                    "%aa.png|%aa.png|%aa.png|%aa.png".Replace("%", dir + Utils.Sep), neighbors);

                nav.GoNextOrPrev(false, neighbors, neighbors.Count);
                TestUtil.IsEq(Path.Combine(dir, "aa.png"), nav.Current);
                TestUtil.IsStringArrayEq(
                    "%aa.png|%aa.png|%aa.png|%aa.png".Replace("%", dir + Utils.Sep), neighbors);
            }

            { // test gonext when file is missing
                var nav = new FileListNavigation(dir, new string[] { ".png" }, true);
                nav.GoLast();
                nav.TrySetPath(Path.Combine(dir, "().png"), false);
                nav.GoNextOrPrev(true, neighbors, neighbors.Count);
                TestUtil.IsEq(Path.Combine(dir, "aa.png"), nav.Current);
                TestUtil.IsStringArrayEq(
                    "%bb.png|%cc.png|%dd.png|%dd.png".Replace("%", dir + Utils.Sep), neighbors);

                nav.GoLast();
                nav.TrySetPath(Path.Combine(dir, "ab.png"), false);
                nav.GoNextOrPrev(true);
                TestUtil.IsEq(Path.Combine(dir, "bb.png"), nav.Current);

                nav.GoLast();
                nav.TrySetPath(Path.Combine(dir, "bc.png"), false);
                nav.GoNextOrPrev(true);
                TestUtil.IsEq(Path.Combine(dir, "cc.png"), nav.Current);

                nav.GoFirst();
                nav.TrySetPath(Path.Combine(dir, "zz.png"), false);
                nav.GoNextOrPrev(true);
                TestUtil.IsEq(Path.Combine(dir, "dd.png"), nav.Current);
            }

            { // test goprev when file is missing
                var nav = new FileListNavigation(dir, new string[] { ".png" }, true);
                nav.GoLast();
                nav.TrySetPath(Path.Combine(dir, "().png"), false);
                nav.GoNextOrPrev(false);
                TestUtil.IsEq(Path.Combine(dir, "aa.png"), nav.Current);

                nav.GoLast();
                nav.TrySetPath(Path.Combine(dir, "bc.png"), false);
                nav.GoNextOrPrev(false);
                TestUtil.IsEq(Path.Combine(dir, "bb.png"), nav.Current);

                nav.GoLast();
                nav.TrySetPath(Path.Combine(dir, "cd.png"), false);
                nav.GoNextOrPrev(false);
                TestUtil.IsEq(Path.Combine(dir, "cc.png"), nav.Current);

                nav.GoFirst();
                nav.TrySetPath(Path.Combine(dir, "zz.png"), false);
                nav.GoNextOrPrev(false, neighbors, neighbors.Count);
                TestUtil.IsEq(Path.Combine(dir, "dd.png"), nav.Current);
                TestUtil.IsStringArrayEq(
                    "%cc.png|%bb.png|%aa.png|%aa.png".Replace("%", dir + Utils.Sep), neighbors);
            }

            { // gonext and goprev after deleted file
                var nav = new FileListNavigation(dir, new string[] { ".png" }, true);
                nav.GoFirst();
                TestUtil.IsEq(Path.Combine(dir, "aa.png"), nav.Current);
                File.Delete(Path.Combine(dir, "bb.png"));

                // call NotifyFileChanges, the test runs more quickly than event can be received
                nav.NotifyFileChanges();
                nav.GoNextOrPrev(true);
                TestUtil.IsEq(Path.Combine(dir, "cc.png"), nav.Current);
                nav.GoNextOrPrev(true);
                TestUtil.IsEq(Path.Combine(dir, "dd.png"), nav.Current);
                File.Delete(Path.Combine(dir, "cc.png"));
                nav.NotifyFileChanges();
                nav.GoNextOrPrev(false);
                TestUtil.IsEq(Path.Combine(dir, "aa.png"), nav.Current);

                // go down to 1 file
                File.Delete(Path.Combine(dir, "dd.png"));
                nav.NotifyFileChanges();
                nav.GoLast();
                TestUtil.IsEq(Path.Combine(dir, "aa.png"), nav.Current);
                nav.GoNextOrPrev(true);
                TestUtil.IsEq(Path.Combine(dir, "aa.png"), nav.Current);
                nav.GoNextOrPrev(false);
                TestUtil.IsEq(Path.Combine(dir, "aa.png"), nav.Current);

                // go down to no files
                File.Delete(Path.Combine(dir, "aa.png"));
                nav.NotifyFileChanges();
                nav.GoNextOrPrev(true);
                TestUtil.IsEq(null, nav.Current);
                nav.GoNextOrPrev(false);
                TestUtil.IsEq(null, nav.Current);
                nav.GoFirst();
                TestUtil.IsEq(null, nav.Current);
                nav.GoLast();
                TestUtil.IsEq(null, nav.Current);

                // recover from no files
                File.WriteAllText(Path.Combine(dir, "new.png"), "content");
                nav.NotifyFileChanges();
                nav.GoNextOrPrev(true);
                TestUtil.IsEq(Path.Combine(dir, "new.png"), nav.Current);
                nav.GoNextOrPrev(false);
                TestUtil.IsEq(Path.Combine(dir, "new.png"), nav.Current);
            }
        }
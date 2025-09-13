public static void AssertEq(object expected, object actual, string msg = "")
        {
            // use a token to make sure that IsEq(null, null) works.
            expected = expected ?? tokenRepresentingNull;
            actual = actual ?? tokenRepresentingNull;

            if (!expected.Equals(actual))
            {
                throw new CsDownloadVidException(
                    "Assertion failure, " + Utils.NL + Utils.NL + msg +
                    ", expected " + expected + " but got " + actual);
            }
        }

        public static void AssertTrue(bool actual, string msg = "")
        {
            AssertEq(true, actual, msg);
        }
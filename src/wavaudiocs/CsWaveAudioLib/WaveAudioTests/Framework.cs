
//Exceedingly simple test framework.
// Ben Fisher, 2008

using System;
using System.Diagnostics;
namespace WaveAudioTests
{
    static partial class CsWaveAudioTests
    {
        static void asserteqf(double v1, double v2, string strmsg) { asserteqf(v1, v2, 0.001, strmsg); }
        static void asserteqf(double v1, double v2) { asserteqf(v1, v2, 0.001, ""); }
        static void asserteqf(double v1, double v2, double tol) { asserteqf(v1, v2, tol, ""); }
        static void asserteqf(double v1, double v2, double tol, string strmsg)
        {
            if (Math.Abs((double)v1 - (double)v2) <= tol) return;
            Console.Error.WriteLine(strmsg + ": ");
            Console.Error.Write(v1);
            Console.Error.Write("!=");
            Console.Error.Write(v2);
            assert(false);
        }

        static void asserteq<T>(T v1, T v2) { asserteq(v1, v2, ""); }
        static void asserteq<T>(T v1, T v2, string strmsg)
        {
            if (v1.Equals(v2)) return;
            Console.Error.WriteLine(strmsg + ": ");
            Console.Error.Write(v1);
            Console.Error.Write("!=");
            Console.Error.Write(v2);
            assert(false);
        }
        static void assert(bool cond) { assert(cond, ""); }
        static void assert(bool cond, string msg)
        {
            Debug.Assert(cond, msg);
        }
    }
}
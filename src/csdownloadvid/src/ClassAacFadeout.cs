// Copyright (c) Ben Fisher, 2016.
// Licensed under GPLv3, refer to LICENSE for details.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CsDownloadVid
{
    // AddFadeoutUsingRawAacData: fadeout for aac audio.
    // by Ben Fisher, 2016
    // 
    // 1) Losslessly split the input file into 3 audio files
    //    0:00 - Start of fadeout
    //    Start of fadeout - End of fadeout
    //    End of fadeout - End of song
    // 2)
    //    Use ffmpeg's "afade" to fade out the second audio file
    //    Save the result as a .wav file
    // 3)
    //    Use qaac to encode the .wav file into an .m4a
    // 4)
    //    Use ffmpeg to get the raw .aac data from the first audio file in step 1)
    //    Use ffmpeg to get the raw .aac data from the audio file made in step 3)
    // 5)
    //    Splice the two raw .aac data files together to make one combined .aac file,
    //    after stripping the first frames from the second .aac as mentioned below
    // 6)
    //    Use ffmpeg to convert the combined .aac file back into an m4a.
    // 
    // The result is all lossless except for the final seconds of the fade-out itself.
    // Sometimes an artifact is heard right at the transition, in which case it's recommended
    // to try again with a different fade-out time about 1s away.
    // 
    // Alternatives considered:
    //    The -af filter for ffmpeg works, but re-encodes the entire file.
    //    It may be possible to tweak raw aac data to adjust volume levels.
    //    Gluing the 2 pieces of aac with ffmpeg -concat leaves a gap of silence
    //    Gluing the 2 pieces of aac by simply concat'ing aac files leaves a gap of silence
    // 
    // Details:
    //    The raw data aac file is built from frames. The first frames contain priming data,
    //    mentioned in https://developer.apple.com/library/mac/technotes/tn2258/_index.html
    //    We detect frames by searching for the byte sequence 0xfff15080.
    //    The encoder delay for Nero aac is ~2600 samples
    //    The encoder delay for qaac (Apple) is ~2112 samples
    //    we can't break apart a frame without needing to encode, but 2048 is close to 2112.
    //    so, for qaac, deleting the first two 1024-sample frames yields the best results
    public class AddFadeoutUsingRawAacData
    {
        public void Go(string inputFile, int sampleRate, double point, double fadeLength,
            string outFilename, ref string log)
        {
            var tmpFiles = new List<string>();
            try
            {
                GoImpl(tmpFiles, inputFile, sampleRate, point, fadeLength, outFilename,
                    ref log);
            }
            finally
            {
                // clean up temp files
                foreach (var tmpFile in tmpFiles)
                {
                    try
                    {
                        File.Delete(tmpFile);
                    }
                    catch
                    {
                        log += "\nCould not delete " + tmpFile;
                    }
                }
            }
        }

        public void GoImpl(List<string> tmpFiles, string input, int sampleRate,
            double point, double fadeLength, string outFilename, ref string log)
        {
            // round to nearest frame
            point = RoundForM4aframe(sampleRate, point);

            // split file, get the main part
            var firstPiece = SplitOneFileSynchronous(input, 0.0, point, 0, ref log);
            tmpFiles.Add(firstPiece);

            // split file, get the last 4 seconds
            var fadeWav = GetFileWithFadeoutSynchronous(input, point, fadeLength, 0, ref log);
            tmpFiles.Add(fadeWav);

            // convert the last 4 seconds to m4a
            var secondPiece = ConvertWavToM4aSynchronous(fadeWav, "320", ref log);
            tmpFiles.Add(secondPiece);

            // now losslessly concat these pieces together
            ConcatM4aFilesWithoutGap(firstPiece, secondPiece, outFilename, tmpFiles, ref log);
        }

        string ConvertM4aToAacSynchronous(string input, ref string log)
        {
            Utils.AssertTrue(input.EndsWith(".m4a"));
            var outFilename = Path.GetDirectoryName(input) + Utils.Sep +
                Path.GetFileNameWithoutExtension(input) + ".aac";

            var args = new List<string>();
            args.Add("-nostdin");
            args.Add("-i");
            args.Add(input);
            args.Add("-acodec");
            args.Add("copy");
            args.Add(outFilename);
            RunGetStdout(CsDownloadVidFilepaths.GetFfmpeg(),
                Utils.CombineProcessArguments(args.ToArray()),
                "m4a to aac", outFilename, ref log);

            Utils.AssertTrue(File.Exists(outFilename));
            return outFilename;
        }

        string ConvertAacToM4aSynchronous(string input, ref string log)
        {
            Utils.AssertTrue(input.EndsWith(".aac"));
            var outFilename = Path.GetDirectoryName(input) + Utils.Sep +
                Path.GetFileNameWithoutExtension(input) + ".m4a";

            var args = new List<string>();
            args.Add("-nostdin");
            args.Add("-i");
            args.Add(input);
            args.Add("-acodec");
            args.Add("copy");
            args.Add("-bsf:a");
            args.Add("aac_adtstoasc");
            args.Add(outFilename);
            RunGetStdout(CsDownloadVidFilepaths.GetFfmpeg(),
                Utils.CombineProcessArguments(args.ToArray()),
                "aac to m4a", outFilename, ref log);

            Utils.AssertTrue(File.Exists(outFilename));
            return outFilename;
        }

        string GetFileWithFadeoutSynchronous(string input, double start, double fadeLength,
            int i, ref string log)
        {
            var outFilename = input + string.Format("_outfade{0}.wav", i.ToString("D2"));
            var args = new List<string>();
            args.Add("-nostdin");
            args.Add("-i");
            args.Add(input);
            args.Add("-ss");
            args.Add(start.ToString());
            args.Add("-t");
            args.Add(fadeLength.ToString());
            args.Add("-vn");

            // note: hours:min:seconds format does not seem to work in the fade spec.
            args.Add("-af");
            args.Add(string.Format("afade=t=out:st={0}:d={1}",
                start.ToString(),
                fadeLength.ToString()));

            args.Add(outFilename);
            RunGetStdout(CsDownloadVidFilepaths.GetFfmpeg(),
                Utils.CombineProcessArguments(args.ToArray()),
                "get fadeout", outFilename, ref log);

            Utils.AssertTrue(File.Exists(outFilename));
            return outFilename;
        }

        public string SplitOneFileSynchronous(string input, double start, double length,
            int i, ref string log, bool reEncodeAudioFlac=false)
        {
            var outFilename = input + "." + string.Format(
                "{0}", i.ToString("D3")) + Path.GetExtension(input);

            var args = new List<string>();
            args.Add("-nostdin");
            args.Add("-i");
            args.Add(input);

            // important: put the options -ss and -t between -i and output
            args.Add("-ss");
            args.Add(start.ToString());
            args.Add("-t");
            args.Add(length.ToString());
            if (reEncodeAudioFlac)
            {
                args.Add("-acodec");
                args.Add("flac");
            }
            else
            {
                args.Add("-acodec");
                args.Add("copy");
            }
            
            args.Add("-vcodec");
            args.Add("copy");

            args.Add(outFilename);
            RunGetStdout(CsDownloadVidFilepaths.GetFfmpeg(),
                Utils.CombineProcessArguments(args.ToArray()),
                "split", outFilename, ref log);

            Utils.AssertTrue(File.Exists(outFilename));
            return outFilename;
        }

        string ConvertWavToM4aSynchronous(string input, string bitrate, ref string log)
        {
            var outFilename = Path.GetDirectoryName(input) + Utils.Sep +
                Path.GetFileNameWithoutExtension(input) + ".m4a";

            var args = new List<string>();
            args.Add("--quality");
            args.Add("2");
            args.Add("-a");
            args.Add(bitrate);
            args.Add("--rate");
            args.Add("keep");
            args.Add(input);
            args.Add("-d");
            args.Add(Path.GetDirectoryName(input));

            RunGetStdout(CsDownloadVidFilepaths.GetQaac(),
                Utils.CombineProcessArguments(args.ToArray()),
                "convert to m4a", outFilename, ref log);

            Utils.AssertTrue(File.Exists(outFilename));
            return outFilename;
        }

        private void ConcatM4aFilesWithoutGap(string firstPiece, string secondPiece,
            string outFilename, List<string> tmpFilesMade, ref string log)
        {
            var firstPieceAac = ConvertM4aToAacSynchronous(firstPiece, ref log);
            tmpFilesMade.Add(firstPieceAac);
            var secondPieceAac = ConvertM4aToAacSynchronous(secondPiece, ref log);
            tmpFilesMade.Add(secondPieceAac);

            var allOfFirst = File.ReadAllBytes(firstPieceAac);
            var allOfSecond = File.ReadAllBytes(secondPieceAac);
            var offsets = GetFrameOffsets44100(allOfSecond);
            if (offsets.Count == 0)
            {
                offsets = GetFrameOffsets48000(allOfSecond);
            }

            if (offsets.Count < 4)
            {
                throw new CsDownloadVidException(@"Could not find aac frame headers. 
                    We support 44.1khz 16bit 2ch audio and 48khz 16bit 2ch audio,
                    Maybe input is not supported or, length of fade out is too short. " +
                    offsets.Count);
            }

            // for the qaac encoder, this seems to be the best value for removing priming.
            // see comment in the top of class. remove two 1024 sample frames
            var cut = 2;
            var fileJoinedAac = ConcatenateRawAacData(firstPieceAac, allOfFirst, allOfSecond,
                offsets, cut, ref log);

            Utils.AssertTrue(File.Exists(fileJoinedAac));
            tmpFilesMade.Add(fileJoinedAac);

            var finalM4a = ConvertAacToM4aSynchronous(fileJoinedAac, ref log);
            Utils.AssertTrue(File.Exists(finalM4a));
            File.Move(finalM4a, outFilename);
            Utils.AssertTrue(File.Exists(outFilename));
        }

        void RunGetStdout(string exe, string args, string title, string outFilename,
            ref string log)
        {
            if (outFilename != null && File.Exists(outFilename))
            {
                throw new CsDownloadVidException("Failed to '" + title + "' because file " +
                    outFilename + "already exists");
            }

            var info = new ProcessStartInfo();
            string stderr = "";
            info.FileName = exe;
            info.Arguments = args;
            info.CreateNoWindow = true;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;

            // Note: the ErrorDataReceived callback is needed, or deadlocks can occur,
            // since ffmpeg sends out more than 4k (buffer size)
            Process p = new Process();
            p.StartInfo = info;
            p.Start();
            p.ErrorDataReceived += (o, e) => { stderr += e.Data; };
            p.BeginErrorReadLine();
            p.WaitForExit();
            log += "\nRan " + info.FileName + " " + info.Arguments;
            if (p.ExitCode != 0 || !File.Exists(outFilename))
            {
                log += "\nStdout:" + p.StandardOutput.ReadToEnd();
                log += "\nStderr:" + stderr;
            }

            if (outFilename != null && !File.Exists(outFilename))
            {
                throw new CsDownloadVidException("Failed to '" + title + "' expected file " +
                    "created to " + outFilename);
            }
        }

        string ConcatenateRawAacData(string inputName, byte[] firstPiece, byte[] secondPiece,
            List<int> offsets, int cut, ref string log)
        {
            // write a truncated file
            var outFilename = inputName + "trim" + cut.ToString() + ".aac";
            using (var bw = new BinaryWriter(new FileStream(outFilename, FileMode.Create)))
            {
                // write all of the first file aac
                bw.Write(firstPiece);

                // write the truncated second file aac
                bw.Write(secondPiece, offsets[cut], secondPiece.Length - offsets[cut]);
            }

            log += "\nJoined raw aac data.";
            Utils.AssertTrue(File.Exists(outFilename));
            return outFilename;
        }

        double RoundForM4aframe(int sampleRate, double point)
        {
            // we should round to the nearest m4a section, which is usually 1024
            // samples, let's say 30 sections to be sure though, and round down.
            // this is ~0.7 seconds (@ 44.1khz).
            long pointInFrames = (long)(point * sampleRate);
            long roundTo = 30 * 1024;
            long rounded = (pointInFrames / roundTo) * roundTo - 1;
            point = rounded / ((double)sampleRate);
            if (point <= 0)
            {
                throw new CsDownloadVidException("Can't fade-out so close to the start.");
            }

            return point;
        }

        List<int> GetFrameOffsets44100(byte[] aacData)
        {
            var offsets = new List<int>();
            for (int i = 0; i < aacData.Length - 4; i++)
            {
                if (aacData[i] == 0xff &&
                    aacData[i + 1] == 0xf1 &&
                    aacData[i + 2] == 0x50 &&
                    aacData[i + 3] == 0x80)
                {
                    offsets.Add(i);
                }
            }

            return offsets;
        }

        List<int> GetFrameOffsets48000(byte[] aacData)
        {
            var offsets = new List<int>();
            for (int i = 0; i < aacData.Length - 4; i++)
            {
                if (aacData[i] == 0xff &&
                    aacData[i + 1] == 0xf1 &&
                    aacData[i + 2] == 0x4c &&
                    aacData[i + 3] == 0x80)
                {
                    offsets.Add(i);
                }
            }

            return offsets;
        }
    }
}

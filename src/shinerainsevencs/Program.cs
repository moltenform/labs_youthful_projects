// Copyright (c) Ben Fisher, 2016.
// Licensed under LGPLv3.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShineRainSevenCsCommon
{
    static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            var currentExePathD = Utils.GetCurrentExecutableDir();

            SimpleLog.Init(Path.Combine(currentExePathD, "log.txt"));
            SimpleLog.Current.WriteLog("Initializing.");
            Configs.Init(Path.Combine(currentExePathD, "options.ini"));
            Configs.Current.LoadPersisted();
            Configs.Current.Set(ConfigKey.Version, "0.1");
            ConfigKeyGetOrAskUserIfNotSet.RecordMainThread();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // This is where typically there would be a Application.Run(new Form())

            if (Array.IndexOf(args, "-m") == -1) {
                MessageBox.Show("Welcome to ShineRainSevenCsCommon.\r\n\r\n" + 
                    "Example syntax:\r\n" +
                    "ShineRainSevenCsCommon.exe -m \"What is your name?\" -d \"Bob\" ");
                return 0;
            }
            
            var message = getValue(args, "-m") ?? "Message:";
            string outputPath = getValue(args, "-o");
            string defaultVal = getValue(args, "-d") ?? null;
            var whichAppName = getValue(args, "-a");
            var s = "MRUPath";
            if (!Enum.TryParse(s, out InputBoxHistory configKey))
            {
                Console.WriteLine("unknown val");
                return 1;
            }

            var userTyped = InputBoxForm.GetStrInput(message, defaultVal, configKey, null, false, false, false);
            File.WriteAllText(outputPath, userTyped ?? "");
            return 0;
        }

        static string getValue(string[] args, string flag)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == flag)
                {
                    return args[i + 1];
                }
            }
            return null;
        }
    }
}

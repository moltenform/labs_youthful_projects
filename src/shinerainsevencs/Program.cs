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
            var currentExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var currentExePathD = Path.GetDirectoryName(currentExePath);

            SimpleLog.Init(Path.Combine(currentExePathD, "log.txt"));
            SimpleLog.Current.WriteLog("Initializing.");
            Configs.Init(Path.Combine(currentExePathD, "options.ini"));
            Configs.Current.LoadPersisted();
            Configs.Current.Set(ConfigKey.Version, "0.1");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // This is where typically there would be a Application.Run(new Form())
            
            var message = getValue(args, "-m") ?? "Message:";
            string outputPath = getValue(args, "-o");
            string defaultVal = getValue(args, "-d") ?? null;
            var whichNumber = getValue(args, "-n");
            var s = "Path" + whichNumber;
            if (!Enum.TryParse(s, out InputBoxHistory theConfigKey))
            {
                Console.WriteLine("unknown val");
                return 1;
            }

            var userTyped = InputBoxForm.GetStrInput(message, defaultVal, theConfigKey, null, false, false, false);
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

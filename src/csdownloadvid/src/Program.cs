// Copyright (c) Ben Fisher, 2016.
// Licensed under GPLv3.

using System;
using System.Windows.Forms;

namespace CsDownloadVid
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SimpleLog.Init("log.txt");
            SimpleLog.Current.WriteLog("Initializing.");
            Configs.Init("options.ini");
            Configs.Current.LoadPersisted();
            Configs.Current.Set(ConfigKey.Version, "0.1");

            Application.Run(new FormMain());
        }
    }
}

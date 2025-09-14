// Copyright (c) Ben Fisher, 2016.
// Licensed under GPLv3, refer to LICENSE for details.

using System;
using System.Windows.Forms;

namespace rbcpy
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Configs.Init("configkey.ini");
            Configs.Current.LoadPersisted();
            Configs.Current.Set(ConfigKey.Version, "0.1");
            ConfigKeyGetOrAskUserIfNotSet.RecordMainThread();
            SimpleLog.Init("configlog.txt");
            SimpleLog.Current.WriteLog("Initializing.");
            Application.Run(new FormSyncMain());
        }
    }
}

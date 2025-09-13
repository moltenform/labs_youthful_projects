// Copyright (c) Ben Fisher, 2016.
// Licensed under GPLv3. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShineRainSevenCsCommon
{
    // Keys for storing persisted settings.
    // Unlike a C++ enum, numeric values aren't used at all,
    // it's the names that are important. Changing the names
    // here will cause loss of compatibility.
    public enum ConfigKey
    {
        None,
        Version,
        EnablePersonalFeatures,
        EnableVerboseLogging,
        MRUPath1,
        // Directories should end with Dir. 
        FilepathDeletedFilesDir,
        FilepathTempDir,
        FilepathM4aEncoder,
        FilepathPython
    }

    // The inputbox dialog keeps a MRU list of recently used strings.
    // Each item here requires a corresponding item in ConfigKey
    // (which is verified in a test).
    public enum InputBoxHistory
    {
        None,
        MRUPath1,
        FilepathDeletedFilesDir,
        FilepathTempDir
    }

    public static class ConfirmChecksums
    {
        public static void Check(ConfigKey key, string currentVal)
        {
            if (!string.IsNullOrEmpty(currentVal) &&
                key.ToString().StartsWith("Filepath", StringComparison.Ordinal) &&
                    !key.ToString().EndsWith("Dir", StringComparison.Ordinal) &&
                    !key.ToString().Contains("Checksum"))
            {
                var otherKeyName = key.ToString().Replace("Filepath", "FilepathChecksum");
                var otherKey = Enum.Parse(typeof(ConfigKey), otherKeyName);
                VerifyChecksum((ConfigKey)otherKey, currentVal);
            }
        }

        static void VerifyChecksum(ConfigKey sumkey, string current)
        {
            var hash = Utils.GetSha512(current);
            var hashExpected = Configs.Current.Get(sumkey);
            if (hashExpected != hash)
            {
                if (string.IsNullOrEmpty(hashExpected) ||
                    string.IsNullOrEmpty(current) ||
                    Utils.AskToConfirm("Checksum does not match for file " +
                    current + Utils.NL + "was:" + hashExpected + Utils.NL + "now: " + hash +
                    Utils.NL + "Did you recently upgrade or change this program? " +
                    "If so, click Yes. Otherwise, click No to exit."))
                {
                    Configs.Current.Set(sumkey, hash);
                }
                else
                {
                    Environment.Exit(1);
                }
            }
        }
    }

    // Class for storing settings.
    // Persisted settings are saved to a simple ini text file.
    // Currently saves to disk synchronously on every change to a persisted setting
    // which is acceptible for current usage.
    public sealed class Configs
    {
        static Configs _instance;
        string _path;
        Dictionary<ConfigKey, string> _persisted = new Dictionary<ConfigKey, string>();

        internal Configs(string path)
        {
            this._path = path;
        }

        // in-memory non-persisted settings
        public string Directory { get; private set; }
        public bool SuppressDialogs { get; set; }

        public static Configs Current
        {
            get
            {
                return _instance;
            }
        }

        public static ConfigKey ConfigsPersistedKeysFromString(string keyname)
        {
            var key = ConfigKey.None;
            return Enum.TryParse(keyname, out key) ? key : ConfigKey.None;
        }

        public static void Init(string path)
        {
            _instance = new Configs(path);
            _instance.Directory = Path.GetDirectoryName(path);
        }

        public void LoadPersisted()
        {
            if (!File.Exists(_path))
            {
                return;
            }

            var lines = File.ReadAllLines(_path);
            for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                var line = lines[lineNumber];

                // skip sections and comments
                if (line.StartsWith("[", StringComparison.Ordinal) ||
                    line.StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                // split with count=2, to ignore subsequent = characters in the string.
                var split = line.Split(new char[] { '=' }, 2, StringSplitOptions.None);
                if (split.Length != 2)
                {
                    if (!string.IsNullOrEmpty(line.Trim()))
                    {
                        SimpleLog.Current.WriteWarning(
                            "malformed config, missing = on line " + lineNumber);
                    }

                    continue;
                }

                var key = ConfigsPersistedKeysFromString(split[0]);
                if (key == ConfigKey.None)
                {
                    SimpleLog.Current.WriteWarning(
                        "unrecognized config key on line " +
                        lineNumber + ", might occur if using config from future version.");
                    continue;
                }

                _persisted[key] = split[1];
            }
        }

        void SavePersisted()
        {
            var sb = new StringBuilder();
            foreach (var key in from key in _persisted.Keys orderby key select key)
            {
                var value = _persisted[key];
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Contains("\r") || value.Contains("\n"))
                    {
                        throw new ShineRainSevenCsException(
                            "config values cannot contain newline, for key " + key);
                    }

                    sb.AppendLine(key.ToString() + "=" + value);
                }
            }

            File.WriteAllText(_path, sb.ToString());
        }

        public void Set(ConfigKey key, string s)
        {
            var valueWasChanged = !_persisted.TryGetValue(key, out string prev) ||
                prev != s ||
                !File.Exists(_path);
            if (valueWasChanged)
            {
                _persisted[key] = s;
                SavePersisted();
            }
        }

        public void SetBool(ConfigKey key, bool b)
        {
            Set(key, b ? "true" : "");
        }

        public string Get(ConfigKey key)
        {
            var ret = _persisted.TryGetValue(key, out string s) ? s : "";
            ConfirmChecksums.Check(key, ret);
            return ret;
        }

        public bool GetBool(ConfigKey key)
        {
            return !string.IsNullOrEmpty(Get(key));
        }

        public static bool EnableWebp()
        {
            // Webp disabled due to security issues,
            // since the Imazen.Webp library is tied to old insecure webp versions.
            // We'll need to move to another webp library.
            return false;
        }
    }
}

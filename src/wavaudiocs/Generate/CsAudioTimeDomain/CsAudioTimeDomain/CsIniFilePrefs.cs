using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

    public class CsIniSaveHelper
    {
        private IniFileParsing csIniFile;
        private string sSectionName;
        public static readonly string NEWLINEINDICATOR = "$@";
        public CsIniSaveHelper(IniFileParsing csIniFile, string sSectionName)
        {
            this.csIniFile = csIniFile;
            this.sSectionName = sSectionName;
        }

        public void saveDouble(string sParamName, double value)
        {
            csIniFile.WriteValueCheckLength(sSectionName, sParamName, value.ToString(CultureInfo.InvariantCulture));
        }
        public void saveInt(string sParamName, int value)
        {
            csIniFile.WriteValueCheckLength(sSectionName, sParamName, value.ToString(CultureInfo.InvariantCulture));
        }
        public void saveString(string sParamName, string s)
        {
            if (s.Contains(NEWLINEINDICATOR)) throw new IniFileParsingException("The string "+ NEWLINEINDICATOR+" is reserved.");
            s = s.Replace("\r\n", NEWLINEINDICATOR);
            csIniFile.WriteValueCheckLength(sSectionName, sParamName, s);
        }
    }
    public class CsIniLoadHelper
    {
        private IniFileParsing csIniFile;
        private string sSectionName;
        private Dictionary<string, string> dict;
        public CsIniLoadHelper(IniFileParsing csIniFile, string sSectionName)
        {
            this.csIniFile = csIniFile;
            this.sSectionName = sSectionName;
            dict = new Dictionary<string, string>();
            List<KeyValuePair<string, string>> list = csIniFile.GetKeysAndValues(sSectionName);
            foreach (KeyValuePair<string, string> kvp in list)
                dict[kvp.Key] = kvp.Value;
        }

        public double getDouble(string sParamName, bool optional) //defaults to 0.0 if not found
        {
            if (!dict.ContainsKey(sParamName)) 
                if (!optional)
                    throw new IniFileParsingException("Could not find key "+sParamName);
                else 
                    return 0.0;

            double ret;
            if (!double.TryParse(dict[sParamName], out ret)) throw new IniFileParsingException("Not valid number.");
            return ret;
        }
        public double getDouble(string sParamName)
        {
            return getDouble(sParamName, false);
        }
        public int getInt(string sParamName)
        {
            if (!dict.ContainsKey(sParamName)) throw new IniFileParsingException("Could not find key "+sParamName);

            int ret;
            if (!int.TryParse(dict[sParamName], out ret)) throw new IniFileParsingException("Not valid number.");
            return ret;
        }
        public string getString(string sParamName)
        {
            if (!dict.ContainsKey(sParamName)) throw new IniFileParsingException("Could not find key "+sParamName);
            return dict[sParamName].Replace(CsIniSaveHelper.NEWLINEINDICATOR, "\r\n");
        }
    }

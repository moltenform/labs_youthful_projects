using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace CsBifurcation
{
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

        public double getDouble(string sParamName)
        {
            if (!dict.ContainsKey(sParamName)) throw new IniFileParsingException("Could not find key "+sParamName);
            return double.Parse(dict[sParamName]);
        }
        public int getInt(string sParamName)
        {
            if (!dict.ContainsKey(sParamName)) throw new IniFileParsingException("Could not find key "+sParamName);
            return int.Parse(dict[sParamName]);
        }
        public string getString(string sParamName)
        {
            if (!dict.ContainsKey(sParamName)) throw new IniFileParsingException("Could not find key "+sParamName);
            return dict[sParamName].Replace(CsIniSaveHelper.NEWLINEINDICATOR, "\r\n");
        }
    }
}

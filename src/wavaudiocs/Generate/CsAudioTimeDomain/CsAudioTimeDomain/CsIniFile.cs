//worse version at http://www.crowsprogramming.com/archives/95
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;

    public class IniFileParsingException : Exception
    {
        public IniFileParsingException(string str) : base(str) { }
    }

    // class by Ben Fisher
    // inspired by examples like http://www.codeproject.com/KB/cs/cs_ini.aspx
    // my version, though, actually handles errors

    //important: key names are case insensitive
    //currently use winapi. actually works well for handling symbol characters. I think only limitation is 32K file limit. and possibly it might reformat your input; hopefully not losing comments
    //fortunately, although using winapi, there are no handles for us to maintain/close
    //TODO: is there a 32K file size limit? if so, how can we work around this?
    //TODO: also there is a max line limit and so on
    public class IniFileParsing
    {
        private string m_fileName;
        private const int MAXFILESIZE = 32768;
        public const int MAXLINELENGTH = 4096; //used to be 2048
        private const int MAXCATEGORIESLENGTH = 2048;
        private const int MAXKEYSLENGTH = 32768 / 2;
        public IniFileParsing(string sFilename, bool bAssertExists)
        {
            if (bAssertExists) //assert that the file exists
            {
                if (!File.Exists(sFilename)) throw new IniFileParsingException("File does not exist.");
                if ((new FileInfo(sFilename)).Length >= MAXFILESIZE) throw new IniFileParsingException("File is too large. Configuration file must be < 32K in size. We're working on fixing this limitation.");
            }
            this.m_fileName = sFilename;
        }

        [DllImport("KERNEL32.DLL", EntryPoint="GetPrivateProfileStringW",
            SetLastError=true,
            CharSet=CharSet.Unicode, ExactSpelling=true,
            CallingConvention=CallingConvention.StdCall)]
        private static extern int GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            string lpReturnString, //not a stringbuilder. they don't handle '\0' well
            int nBufferSize,
            string lpFilename);
        [DllImport("KERNEL32.DLL", EntryPoint="WritePrivateProfileStringW",
            SetLastError=true,
            CharSet=CharSet.Unicode, ExactSpelling=true,
            CallingConvention=CallingConvention.StdCall)]
        private static extern int WritePrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpString,
            string lpFilename);


        public void WriteValue(string strSection, string strKey, string strValue)
        {
            int res = WritePrivateProfileString(strSection, strKey, strValue, this.m_fileName);
            if (res == 0)
            {
                Win32Exception wexp = new Win32Exception(Marshal.GetLastWin32Error());
                throw new IniFileParsingException("win32:" + wexp.Message);
            }
        }
        public void WriteValueCheckLength(string strSection, string strKey, string strValue)
        {
            if (strValue.Length >= MAXLINELENGTH)
                throw new IniFileParsingException("Length of value is too long.");
            WriteValue(strSection, strKey, strValue);
        }

        public string ReadValue(string strSection, string strKey)
        {
            string temp = new string(' ', MAXLINELENGTH);
            int res = GetPrivateProfileString(strSection, strKey, "", temp, MAXLINELENGTH, this.m_fileName);
            /*if (res == 0)
            {
             * maybe we just wrote 0 characters. TODO. how to distinguish between error case and nothing found case?
                Win32Exception wexp = new Win32Exception(Marshal.GetLastWin32Error());
                throw new IniFileParsingException("win32:" + wexp.Message);
            }*/

            //varified, does throw this when buffer is full. this does occur.
            if (res == MAXLINELENGTH - 1) //see docs: means buffer is full
            {
                throw new IniFileParsingException("line in configuration file is too long. must be <" + MAXLINELENGTH);
            }
            return temp.Split('\0')[0]; //get rid of everything after first null
        }

        public List<string> GetCategories()
        {
            string returnString = new string(' ', MAXCATEGORIESLENGTH);
            int res = GetPrivateProfileString(null, null, null, returnString, MAXCATEGORIESLENGTH, this.m_fileName);
            if (res == 0)
            {
                Win32Exception wexp = new Win32Exception(Marshal.GetLastWin32Error());
                throw new IniFileParsingException("win32:no categories. " + wexp.Message);
            }
            if (res == MAXCATEGORIESLENGTH - 2) //see docs: means buffer is full
            {
                throw new IniFileParsingException("data in categories is too long. must be <" + MAXCATEGORIESLENGTH);
            }
            List<string> result = new List<string>(returnString.Split('\0'));
            result.RemoveRange(result.Count - 2, 2); //removes the last 2 entries. 
            return result;
        }


        public List<KeyValuePair<string, string>> GetKeysAndValues(string category)
        {
            // if category doesn't exist, should return an empty list.
            string returnString = new string(' ', MAXKEYSLENGTH);
            int res = GetPrivateProfileString(category, null, null, returnString, MAXKEYSLENGTH, this.m_fileName);
            /*if (res == 0)
            {
             maybe we just wrote 0 characters. TODO. how to distinguish between error case and nothing found case?
                Win32Exception wexp = new Win32Exception(Marshal.GetLastWin32Error());
                throw new IniFileParsingException("win32:" + wexp.Message);
            }*/
            if (res == MAXKEYSLENGTH - 2) //see docs: means buffer is full
            {
                throw new IniFileParsingException("data in key names is too long. must be <" + MAXKEYSLENGTH);
            }
            List<string> listKeys = new List<string>(returnString.Split('\0'));
            listKeys.RemoveRange(listKeys.Count - 2, 2); //removes the last 2 entries.

            //now let's build the actual list
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            foreach (string sKey in listKeys)
            {
                result.Add(new KeyValuePair<string, string>(sKey, this.ReadValue(category, sKey)));
            }
            return result;
        }

    }

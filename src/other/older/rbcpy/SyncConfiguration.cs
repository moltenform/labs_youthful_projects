using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace rbcpy
{
    public class RbcpyGlobalSettings
    {
        public string m_winMergeDir;
        public string m_directoryForDeletedFiles;
        public static void Serialize(RbcpyGlobalSettings config, string sFilename)
        {
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.NewLineHandling = NewLineHandling.Entitize;
            XmlSerializer serializer = new XmlSerializer(typeof(RbcpyGlobalSettings));
            using (XmlWriter wr = XmlWriter.Create(sFilename, ws))
            {
                serializer.Serialize(wr, config);
            }
        }

        public static RbcpyGlobalSettings Deserialize(string sFilename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(RbcpyGlobalSettings));
            TextReader reader = new StreamReader(sFilename);
            object obj = deserializer.Deserialize(reader);
            reader.Close();
            return (RbcpyGlobalSettings)obj;
        }
    }

    public class SyncConfiguration
    {
        public string m_src = "Enter full path here...";
        public string m_destination = "Enter full path here...";
        public string m_excludeDirs = "";
        public string m_excludeFiles = "";
        public bool m_mirror = true;
        public bool m_copySubDirsAndEmptySubdirs = true;

        public string m_copyFlags = "";
        public string m_directoryCopyFlags = "";
        public string m_ipg = "";
        public string m_nRetries = "3";
        public string m_waitBetweenRetries = "1";

        public string m_nThreads = "8";
        public string m_custom = "";
        public bool m_symlinkNotTarget;
        public bool m_fatTimes = true;
        public bool m_compensateDst;
        internal static bool s_disableMessageBox = false;

        public static bool ShouldBeInt(string sFieldName)
        {
            return (sFieldName == "m_ipg" || sFieldName == "m_nRetries" || sFieldName == "m_waitBetweenRetries" || sFieldName == "m_nThreads");
        }

        public static bool Validate(SyncConfiguration config)
        {
            if (!Directory.Exists(config.m_src) || !config.m_src.Contains("\\"))
            {
                if (!s_disableMessageBox)
                {
                    MessageBox.Show("Source dir does not exist");
                }

                return false;
            }

            if (!Directory.Exists(config.m_destination) || !config.m_destination.Contains("\\"))
            {
                if (!s_disableMessageBox)
                {
                    MessageBox.Show("Dest dir does not exist");
                }

                return false;
            }

            if (config.m_src.EndsWith("\\") || config.m_destination.EndsWith("\\"))
            {
                if (!s_disableMessageBox)
                {
                    MessageBox.Show("Directories should not end with a \\ character.");
                }

                return false;
            }

            
            if (config.m_destination == config.m_src)
            {
                if (!s_disableMessageBox)
                {
                    MessageBox.Show("Src and destination should not be the same.");
                }

                return false;
            }

            string tmpDest = (config.m_destination.ToLower()+"\\"), tmpSrc = (config.m_src.ToLower()+"\\");
            if (tmpDest.Contains(tmpSrc) || tmpSrc.Contains(tmpDest))
            {
                if (!s_disableMessageBox)
                {
                    MessageBox.Show("Src and destination should not intersect.");
                }

                return false;
            }

            Type type = config.GetType();
            FieldInfo[] properties = type.GetFields();
            foreach (FieldInfo property in properties)
            {
                if (ShouldBeInt(property.Name))
                {
                    var str = (string)property.GetValue(config);
                    foreach (char c in str)
                    {
                        if (!"0123456789".Contains(c))
                        {
                            if (!s_disableMessageBox)
                                MessageBox.Show("in field " + property.Name + " needs to be a number, got " + str + " instead");
                            return false;
                        }
                    }
                }
                else if (property.Name != "m_custom")
                {
                    var str = property.GetValue(config) as string;
                    if (str != null && str.Contains("\""))
                    {
                        if (!s_disableMessageBox)
                            MessageBox.Show("in field " + property.Name + " should not contain double-quote, got " + str + " instead");
                        return false;
                    }
                }
            }
            
            return true;
        }

        public static void Serialize(SyncConfiguration config, string sFilename)
        {
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.NewLineHandling = NewLineHandling.Entitize;
            XmlSerializer serializer = new XmlSerializer(typeof(SyncConfiguration));
            using (XmlWriter wr = XmlWriter.Create(sFilename, ws))
            {
                serializer.Serialize(wr, config);
            }
        }

        public static SyncConfiguration Deserialize(string sFilename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(SyncConfiguration));
            TextReader reader = new StreamReader(sFilename);
            object obj = deserializer.Deserialize(reader);
            reader.Close();
            return (SyncConfiguration)obj;
        }
    }
}


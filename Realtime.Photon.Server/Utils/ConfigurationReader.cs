using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Cmune.Util;

namespace Cmune.Realtime.Photon.Server
{
    public static class ConfigurationReader
    {
        public static bool ReadAppSetting<T>(string key, out T val)
        {
            val = default(T);

            string s = ConfigurationManager.AppSettings[key];

            if (!string.IsNullOrEmpty(s))
            {
                if (typeof(T) == typeof(bool))
                {
                    bool b;
                    if (bool.TryParse(s, out b))
                    {
                        val = (T)(object)b;
                    }
                }
                else if (typeof(T) == typeof(int))
                {
                    int b;
                    if (int.TryParse(s, out b))
                    {
                        val = (T)(object)b;
                    }
                }
                else if (typeof(T) == typeof(string))
                {
                    val = (T)(object)s;
                }
                else
                {
                    CmuneDebug.LogError("Unsupported Datatype {0}", typeof(T));
                    return false;
                }

                return true;
            }
            else
            {
                CmuneDebug.LogError("Key Value Pair <{0}, {1}> not defined in Configuration File", key, typeof(T));
                return false;
            }
        }

        public static void ReadSocketServerConfigFile()
        {
            string config = Process.GetCurrentProcess().MainModule.FileName.Replace("PhotonSocketServer.exe", "PhotonServer.config");

            FileInfo file = new FileInfo(config);
            if (file.Exists)
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;
                FileStream fileStream = new FileStream(config, FileMode.Open, FileAccess.Read);
                XmlReader xml = XmlReader.Create(fileStream, settings);
                try
                {
                    while (xml.Read())
                    {
                        if (xml.NodeType == XmlNodeType.Element && xml.Name.Equals("UDPListener"))
                        {
                            ServerSettings.IP = xml.GetAttribute("IPAddress");
                            ServerSettings.Port = int.Parse(xml.GetAttribute("Port"));

                            CmuneDebug.LogInfo("Connection Address {0}", ServerSettings.ConnectionString);
                        }
                    }
                }
                finally
                {
                    xml.Close();
                    fileStream.Close();
                }
            }
            else
            {
                throw CmuneDebug.Exception("Can't find config file at location {0}", config);
            }
        }
    }
}
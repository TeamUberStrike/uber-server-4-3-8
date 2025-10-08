using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Cmune.Realtime.Common;
using ExitGames.Concurrency.Fibers;

namespace Cmune.Realtime.Photon.Server
{
    public class CleanSpeakWebService
    {
        //private event Action<string> FilteredMessage;

        private static PoolFiber _executionFiber;

        private static PoolFiber ExecutionFiber
        {
            get
            {
                if (_executionFiber == null)
                {
                    _executionFiber = new PoolFiber();
                    _executionFiber.Start();
                }
                return _executionFiber;
            }
        }

        public static void FilterMessage(string msg, Action<string> filteredMessage)
        {
            //string message = CreateMessage(msg);
            ////CmuneDebug.LogFormat("CleanSpeakWebService:FilterMessage:\n{0}", message);

            //WebRequest post = WebRequest.Create("http://galileo.inversoft.com:8021/filter");
            //post.Method = "POST";
            //post.ContentType = "text/xml";

            ExecutionFiber.Enqueue(() =>
            {
                try
                {
                    ////We write the parameters into the request
                    //StreamWriter sw = new StreamWriter(post.GetRequestStream());
                    //sw.Write(message);
                    //sw.Close();
                    //FilterCallBack(GetResponse(post), filteredMessage);

                    string cleanMsg = BadWordFilter.CleanMessage(msg);
                    filteredMessage(cleanMsg);
                }
                catch (System.Net.WebException ex)
                {
                    //fallback if webservice call fails
                    filteredMessage(msg);
                    WebserviceError(ex.Message);
                }
                catch (System.UriFormatException ex)
                {
                    //fallback if webservice call fails
                    filteredMessage(msg);
                    WebserviceError(ex.Message);
                }
            }
            );
        }

        private static string GetResponse(WebRequest request)
        {
            // Execute the query
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            StreamReader sr = new StreamReader(response.GetResponseStream());
            return sr.ReadToEnd();
        }

        public static void FilterCallBack(string response, Action<string> filteredMessage)
        {
            CmuneDebug.Log(response);

            XmlReader xml = XmlReader.Create(new StringReader(response));
            if (xml.ReadToFollowing("response"))
            {
                if (xml.AttributeCount > 1)
                {
                    WebserviceError(xml.GetAttribute("errorMessage"));
                }
                else
                {
                    if (xml.ReadToFollowing("replaced"))
                    {
                        filteredMessage(xml.ReadString());
                    }
                }
            }
        }

        public static void WebserviceError(string msg)
        {
            CmuneDebug.LogErrorFormat("CleanSpeakWebService failed with Error: {0}", msg);
        }

        public static string CreateMessage(string message)
        {
            try
            {
                string uri = "http://www.inversoft.com/schemas/cleanspeak/filter-webservice/1.0";
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                StringBuilder b = new StringBuilder();
                XmlWriter xml = XmlWriter.Create(b, settings);

                xml.WriteStartElement("", "request", uri);
                {
                    //xml.WriteAttributeString("xmlns", "http://www.inversoft.com/schemas/cleanspeak/filter-webservice/1.0");

                    xml.WriteStartElement("in");
                    {
                        xml.WriteString(message);
                    }
                    xml.WriteEndElement();

                    //xml.WriteStartElement("", "match", uri);
                    //{
                    //    xml.WriteStartElement("", "cleanspeakdb", uri);
                    //    {
                    //        xml.WriteAttributeString("severity", "medium");
                    //        xml.WriteAttributeString("categories", "Slang Youth");
                    //    }
                    //    xml.WriteEndElement();
                    //}
                    //xml.WriteEndElement();

                    xml.WriteStartElement("", "replace", uri);
                    {
                        xml.WriteAttributeString("withString", "!@#$%^");
                    }
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
                xml.Flush();

                return b.ToString();
            }
            catch (XmlException ex)
            {
                CmuneDebug.Exception("{0}", ex.Message);
                return string.Empty;
            }
        }
    }
}

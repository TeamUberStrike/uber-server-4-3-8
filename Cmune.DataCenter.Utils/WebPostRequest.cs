using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Web;

namespace Cmune.DataCenter.Utils
{
    /// <summary>
    /// Allow us to do a HTTP POST easily
    /// </summary>
    public class WebPostRequest
    {
        WebRequest _TheRequest;
        HttpWebResponse _TheResponse;
        ArrayList _TheQueryData;

        public WebPostRequest(string url)
        {
            _TheRequest = WebRequest.Create(url);
            _TheRequest.Method = "POST";
            _TheQueryData = new ArrayList();
        }

        public void Add(string key, string value)
        {
            _TheQueryData.Add(String.Format("{0}={1}", key, Uri.EscapeDataString(value)));
        }

        public string GetResponse()
        {
            // Set the encoding type
            _TheRequest.ContentType = "application/x-www-form-urlencoded";

            // Build a string containing all the parameters
            string parameters = String.Join("&", (String[])_TheQueryData.ToArray(typeof(string)));
            _TheRequest.ContentLength = parameters.Length;

            // We write the parameters into the request
            using (StreamWriter sw = new StreamWriter(_TheRequest.GetRequestStream()))
            {
                sw.Write(parameters);
                sw.Close();

                // Execute the query
                _TheResponse = (HttpWebResponse)_TheRequest.GetResponse();
                using (StreamReader sr = new StreamReader(_TheResponse.GetResponseStream()))
                {
                    string response = sr.ReadToEnd();
                    sr.Close();
                    return response;
                }
            }
        }
    }
}
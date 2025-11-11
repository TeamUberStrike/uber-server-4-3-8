using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections;
using System.Web;
using System.IO;

namespace Cmune.DataCenter.Utils
{
    /// <summary>
    /// Allow us to do a HTTP GET easily
    /// </summary>
    public class WebGetRequest
    {
        WebRequest _TheRequest;
        HttpWebResponse _TheResponse;
        ArrayList _TheQueryData;
        String _Url;

        public WebGetRequest(string url)
        {
            _Url = url;
            
            _TheQueryData = new ArrayList();
        }

        public WebGetRequest(string url, object parameters)
        {
            _Url = url;

            _TheQueryData = new ArrayList();

            BuildParameters(parameters);
        }

        private void BuildParameters(object parameters)
        {
            foreach(var property in parameters.GetType().GetProperties())
            {
                this.Add(property.Name, property.GetValue(parameters, null).ToString());
            }
        }

        public void Add(string key, string value)
        {
            _TheQueryData.Add(String.Format("{0}={1}", key, HttpUtility.UrlEncode(value)));
        }

        public string GetResponse()
        {
            // Build a string containing all the parameters
            string parameters = String.Join("&", (String[])_TheQueryData.ToArray(typeof(string)));
            if (!parameters.IsNullOrFullyEmpty())
                _Url = _Url.Contains("?") ? (_Url + "&" + parameters) : (_Url + "?" + parameters);
            _TheRequest = WebRequest.Create(_Url);
            _TheRequest.Method = "GET";

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

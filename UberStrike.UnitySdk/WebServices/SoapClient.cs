using System;
using System.Collections;
using System.Text;
using System.Xml;
using UnityEngine;

namespace UberStrike.WebService.Unity
{
    internal static class SoapClient
    {
        private static int _requestId = 0;

        private static void LogRequest(int id, float time, int sizeBytes, string interfaceName, string serviceName, string methodName)
        {
            if (Configuration.RequestLogger != null)
            {
                string size = ((float)sizeBytes / 1000.0f).ToString();
                Configuration.RequestLogger(string.Format("[REQ] ID:{0} Time:{1:N2} Size:{2:N2}Kb Service:{3} Interface:{4} Method:{5}", id, time, size, serviceName, interfaceName, methodName));
            }
        }

        private static void LogResponse(int id, float time, string message, float duration, int sizeBytes)
        {
            if (Configuration.RequestLogger != null)
            {
                string size = (sizeBytes / 1000.0f).ToString();
                Configuration.RequestLogger(string.Format("[RSP] ID:{0} Time:{1:N2} Size:{2:N2}Kb Duration:{3:N2}s Status:{4}", id, time, size, duration, message));
            }
        }

        public static IEnumerator MakeRequest(string interfaceName, string serviceName, string methodName, byte[] data, Action<byte[]> requestCallback, Action<Exception> exceptionHandler)
        {
            int requestId = _requestId++;

            string postData = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""><s:Body><" + methodName + @" xmlns=""http://tempuri.org/""><data>" + Convert.ToBase64String(data) + @"</data></" + methodName + @"></s:Body></s:Envelope>";

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            Hashtable headers = new Hashtable();
            headers.Add("SOAPAction", "\"http://tempuri.org/" + interfaceName + "/" + methodName + "\"");
            headers.Add("Content-type", "text/xml; charset=utf-8");

            XmlDocument doc = new XmlDocument();

            float startTime = Time.realtimeSinceStartup;
            LogRequest(requestId, startTime, data.Length, interfaceName, serviceName, methodName);

            yield return new WaitForEndOfFrame();

            if (WebServiceStatistics.IsEnabled)
                WebServiceStatistics.RecordWebServiceBegin(methodName, byteArray.Length);

            using (WWW request = new WWW(Configuration.WebserviceBaseUrl + serviceName, byteArray, headers))
            {
                yield return request;

                if (WebServiceStatistics.IsEnabled)
                    WebServiceStatistics.RecordWebServiceEnd(methodName, request.bytes.Length, request.isDone && string.IsNullOrEmpty(request.error));

                try
                {
                    if (Configuration.SimulateWebservicesFail)
                    {
                        throw new Exception("Simulated Webservice fail when calling " + interfaceName + "/" + methodName);
                    }

                    byte[] returnData = null;

                    if (request.isDone && String.IsNullOrEmpty(request.error))
                    {
                        if (!string.IsNullOrEmpty(request.text))
                        {
                            try
                            {
                                doc.LoadXml(request.text);
                                XmlNodeList result = doc.GetElementsByTagName(methodName + "Result");
                                if (result.Count > 0)
                                {
                                    returnData = Convert.FromBase64String(result[0].InnerXml);
                                    LogResponse(requestId, Time.realtimeSinceStartup, "OK", Time.realtimeSinceStartup - startTime, request.bytes.Length);
                                }
                                else
                                {
                                    LogResponse(requestId, Time.realtimeSinceStartup, request.text, Time.time - startTime, 0);
                                    throw new Exception("WWW Request to " + Configuration.WebserviceBaseUrl + serviceName + " failed with content" + request.text);
                                }
                            }
                            catch
                            {
                                LogResponse(requestId, Time.time, request.text, Time.realtimeSinceStartup - startTime, 0);
                                throw new Exception("Error reading XML return for method call " + interfaceName + "/" + methodName + ":" + request.text);
                            }
                        }
                    }
                    else
                    {
                        LogResponse(requestId, Time.realtimeSinceStartup, request.error, Time.time - startTime, 0);
                        throw new Exception("WWW Url: " + Configuration.WebserviceBaseUrl + "\nService: " + serviceName + "\nMethod: " + methodName + "\nMessage: " + request.error);
                    }

                    if (requestCallback != null)
                        requestCallback(returnData);
                }
                catch (Exception e)
                {
                    if (exceptionHandler != null) exceptionHandler(e);
                    else throw;
                }
            }
        }
    }
}

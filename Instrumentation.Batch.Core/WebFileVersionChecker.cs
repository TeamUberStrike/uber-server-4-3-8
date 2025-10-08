using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Cmune.DataCenter.Utils;

namespace Cmune.Instrumentation.Batch.Core
{
    public static class WebFileVersionChecker
    {
        /// <summary>
        /// Compare a web version to a local copy to ensure that our copy of files hosted on another service normally are not out of date
        /// </summary>
        /// <param name="url">Should contain the file name at the end</param>
        /// <param name="debugMode"></param>
        /// <returns></returns>
        public static void CheckFile(string url, bool debugMode)
        {
            bool areFilesIdentical = false;
            string fileName = String.Empty;

            // TODO We need to catch all the exceptions to log them

            // we need to download the file

            try
            {
                using (WebClient client = new WebClient())
                using (MemoryStream webStream = new MemoryStream(client.DownloadData(url)))
                {
                    if (debugMode) { Console.WriteLine(String.Format("url {0} was read", url)); }

                    Uri uri = new Uri(url);
                    string[] uriSegments = uri.Segments;

                    // The last fragment is the file name we're looking for

                    if (uriSegments.Length > 0)
                    {
                        fileName = uriSegments[uriSegments.Length - 1];
                    }
                    else
                    {
                        throw new ArgumentException("The url is not properly formed", "url");
                    }

                    // We'll now open the local cache of the file

                    string localCachePath = ConfigurationUtilities.ReadConfigurationManager("LocalCachePath");

                    using (FileStream fileStream = new FileStream(localCachePath + fileName, FileMode.Open, FileAccess.Read))
                    {
                        if (debugMode) { Console.WriteLine(String.Format("local cache {0} was read", localCachePath + fileName)); }

                        areFilesIdentical = StreamEquals(webStream, fileStream);
                    }
                }

                if (!areFilesIdentical)
                {
                    // We need to send an alert
                    CmuneMail.SendEmail(ConfigurationUtilities.ReadConfigurationManager("EmailAlertFrom"),
                                        "Cmune devteam",
                                        ConfigurationUtilities.ReadConfigurationManager("EmailAlertTo"),
                                        "Lee",
                                        String.Format("CmuneBatch: {0} changed", fileName),
                                        String.Format("<ul><li><b>URL:</b> {0}</li><li><b>File name:</b> {1}</li></ul>", url, fileName),
                                        String.Format("URL: {0}\nFile name: {1}", url, fileName));
                }
            }
            catch (Exception ex)
            {
                CmuneLog.LogException(ex, String.Format("url={0}", url));
            }
        }

        /// <summary>
        /// Compares two Streams (might move to Utils)
        /// </summary>
        /// <param name="stream1"></param>
        /// <param name="stream2"></param>
        /// <returns></returns>
        public static bool StreamEquals(Stream stream1, Stream stream2)
        {
            const int bufferSize = 2048;
            byte[] buffer1 = new byte[bufferSize]; //buffer size
            byte[] buffer2 = new byte[bufferSize];

            while (true)
            {
                int count1 = stream1.Read(buffer1, 0, bufferSize);
                int count2 = stream2.Read(buffer2, 0, bufferSize);

                if (count1 != count2)
                    return false;

                if (count1 == 0)
                    return true;

                // You might replace the following with an efficient "memcmp"
                if (!buffer1.Take(count1).SequenceEqual(buffer2.Take(count2)))
                    return false;
            }
        }
    }
}
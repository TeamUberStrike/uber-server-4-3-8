using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Utils;

namespace Cmune.Channels.Instrumentation.Utils
{
    public static class SignInRecord
    {
        const string fileName = "SignInRecord.txt";
        static string recordfilePath = ConfigurationUtilities.ReadConfigurationManager("AdminLogPath") + fileName;

        public static void Write(string userName, string Ip)
        {
            TextWriter tw = new StreamWriter(recordfilePath, true, Encoding.UTF8);

            // write a line of text to the file
            tw.WriteLine(userName + ";" + Ip + ";" + DateTime.Now);
            // close the stream
            tw.Close();
        }

        public static void Refresh(List<string> data)
        {
            TextWriter tw = new StreamWriter(recordfilePath, false, Encoding.UTF8);

            data.ForEach(d => tw.WriteLine(d));
            // close the stream
            tw.Close();
        }

        public static List<string> GetNext20Records()
        {
            string buffer;
            List<string> bufferList = new List<string>();
            List<string> listToReturn = new List<string>();

            TextReader tr = new StreamReader(recordfilePath, Encoding.UTF8);
            for (int i = 0; (buffer = tr.ReadLine()) != null; i++)
            {
                bufferList.Add(buffer);
            }
            tr.Close();

            // get the lastest
            int total = 20;
            for (int i = bufferList.Count - 1; i > 0 && total > 0; i--)
            {
                listToReturn.Add(bufferList[i]);
                total--;
            }

            // if ze have more than 50 logs then we clean the older
            if (bufferList.Count > 50)
            {
                bufferList.RemoveRange(0, bufferList.Count - 50);
                Refresh(bufferList);
            }

            return listToReturn;
        }
    }
}
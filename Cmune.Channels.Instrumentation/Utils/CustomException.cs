using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmune.Channels.Instrumentation.Utils
{
    public class CustomException : Exception
    {
        public string AdditionalInformation { get; set; }
    }
}
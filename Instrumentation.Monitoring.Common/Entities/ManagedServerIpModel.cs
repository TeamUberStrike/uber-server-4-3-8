using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Cmune.Instrumentation.Monitoring.Common.Entities
{
    public class ManagedServerIpModel
    {
        public int Id { set; get; }

        public string Ip { get; set; } 
    }
}

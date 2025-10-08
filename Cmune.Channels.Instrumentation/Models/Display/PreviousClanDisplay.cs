using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmune.Channels.Instrumentation.Models.Display
{
    public class PreviousClanDisplay
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public DateTime DateJoined { get; set; }
        public DateTime DateQuit { get; set; }
    }
}
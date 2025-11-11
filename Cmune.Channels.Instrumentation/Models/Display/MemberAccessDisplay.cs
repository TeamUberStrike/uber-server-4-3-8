using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Cmune.DataCenter.Common.Entities;


namespace Cmune.Channels.Instrumentation.Models.Display
{
    public class MemberAccessDisplay
    {
        public int Cmid { get; set; }
        public string Name { get; set; }
        public MemberAccessLevel AccessLevel { get; set; }

        public MemberAccessDisplay(int cmid, string name, MemberAccessLevel accessLevel)
        {
            this.Cmid = cmid;
            this.Name = name;
            this.AccessLevel = accessLevel;
        }
    }
}

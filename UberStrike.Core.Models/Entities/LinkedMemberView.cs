using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.DataCenter.Common.Entities
{
    public class LinkedMemberView
    {
        #region Properties

        public int Cmid { get; private set; }
        public string Name { get; private set; }

        #endregion

        #region Constructors

        public LinkedMemberView(int cmid, string name)
        {
            this.Cmid = cmid;
            this.Name = name;
        }

        #endregion
    }
}

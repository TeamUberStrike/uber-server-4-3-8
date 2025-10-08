using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmune.Channels.Instrumentation.Models.Display
{
    public struct TestAccountDisplay
    {
         #region Fields

        private int _cmid;
        private string _name;
        private string _email;
        private string _password;

        #endregion Fields

        #region Properties

        public int Cmid
        {
            get { return _cmid; }
            private set { _cmid = value; }
        }

        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }

        public string Email
        {
            get { return _email; }
            private set { _email = value; }
        }

        public string Password
        {
            get { return _password; }
            private set { _password = value; }
        }

        #endregion Properties

        #region Constructors

        public TestAccountDisplay(int cmid, string name, string email, string password)
        {
            _cmid = cmid;
            _name = name;
            _email = email;
            _password = password;
        }

        #endregion Constructors
    }
}
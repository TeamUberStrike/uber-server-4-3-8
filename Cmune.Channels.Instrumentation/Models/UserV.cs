using System;

namespace Cmune.Channels.Instrumentation.Models
{
    public class UserV
    {
        #region Private fields

        private string _login;
        private string _name;
        private string _nbPoints;
        private string _nbCredits;
        private DateTime _lastAliveAck;
        private string _userID;

        #endregion

        #region Properties

        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string NbPoints
        {
            get { return _nbPoints; }
            set { _nbPoints = value; }
        }

        public string NbCredits
        {
            get { return _nbCredits; }
            set { _nbCredits = value; }
        }

        public DateTime LastAliveAck
        {
            get { return _lastAliveAck; }
            set { _lastAliveAck = value; }
        }

        public string UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }

        #endregion

        public UserV(string login, string name, DateTime lastAliveAck, string nbPoints, string nbCredits, string userID)
        {
            this.UserID = userID;
            this.LastAliveAck = lastAliveAck;
            this.Login = login;
            this.Name = name;
            this.NbCredits = nbCredits;
            this.NbPoints = nbPoints;
        }
    }
}

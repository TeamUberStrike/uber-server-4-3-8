using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UberStrike.Channels.Kongregate.Models
{
    public class CompleteAccountModel
    {
        public string NewEmail { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}
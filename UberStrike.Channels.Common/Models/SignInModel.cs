using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Channels.Common.Models
{
    public class SignInModel
    {
        public string KongregateId { get; set; }
        public ChannelType ChannelType { get; set; }
        public string Hash { get; set; }
        public string UserName { get; set; }
        public string Email {get;set;}
        public string Password { get; set; }
    }
}
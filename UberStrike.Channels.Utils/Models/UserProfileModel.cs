using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.DataCenter.DataAccess;
using UberStrike.DataCenter.DataAccess;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Channels.Utils.Models
{
    public class UserProfileModel
    {
        public Member Member { get; set; }
        public User User { get; set; }
        public string UserName { get; set; }
        public FacebookUserModel FacebookUserModel { get; set; }
        public KongregateUserModel KongregateUserModel { get; set; }
        // TODO: to rewrite to allow user from FB, Windows stand alone to login on the portal in a clean way
        public ChannelType ChannelType { get; set; }
    }
}
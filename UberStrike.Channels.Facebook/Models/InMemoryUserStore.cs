using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UberStrike.Channels.Utils.Models;

namespace UberStrike.Channels.Facebook.Models
{
    public class InMemoryUserStore
    {
        private static System.Collections.Concurrent.ConcurrentBag<FacebookUserModel> users = new System.Collections.Concurrent.ConcurrentBag<FacebookUserModel>();

        public static void Add(FacebookUserModel user)
        {
            if (users.SingleOrDefault(u => u.FacebookId == user.FacebookId) != null)
            {
                throw new InvalidOperationException("User already exists.");
            }

            users.Add(user);
        }

        public static FacebookUserModel Get(long facebookId)
        {
            return users.SingleOrDefault(u => u.FacebookId == facebookId);
        }

    }
}
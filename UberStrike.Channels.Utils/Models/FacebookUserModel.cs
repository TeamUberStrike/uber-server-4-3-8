using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UberStrike.Channels.Utils.Models
{
    public class FacebookUserModel
    {
        public long FacebookId { get; set; }
        public string FacebookThirdPartyId { get; set; }
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
        public int Cmid { get; set; }
        public bool IsAuthenticated { get { return Cmid > 0; } }
        /** less important information about user **/
        public string Name { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string PostCode { get; set; }
        public string Locale { get; set; }
        public string Id3PlayerName { get; set; }
        public string PicturePath { get; set; }
        public bool IsAccountComplete { get; set; }
    }
}

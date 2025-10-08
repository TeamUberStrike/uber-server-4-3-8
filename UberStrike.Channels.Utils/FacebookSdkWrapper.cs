using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook;
using Facebook.Web;
using UberStrike.Channels.Utils.Models;
using UberStrike.DataCenter.Common.Entities;
using System.Net;
using System.IO;

namespace UberStrike.Channels.Utils
{
    public class FacebookSdkWrapper
    {
        public long GetFacebookApplicationId()
        {
            return long.Parse(FacebookApplication.Current.AppId);
        }

        public string GetFacebookApplicationUrl()
        {
            return FacebookApplication.Current.CanvasPage;
        }

        public FacebookWebContext FacebookWebContextCurrent
        {
            get
            {
                return FacebookWebContext.Current;
            }
        }

        private bool isValidFbModel(FacebookUserModel fbUserModel, FacebookSession currentSession)
        {
            return !String.IsNullOrEmpty(fbUserModel.AccessToken) && fbUserModel.Expires != null && (fbUserModel.AccessToken == currentSession.AccessToken && fbUserModel.Expires == currentSession.Expires);
        }

        public FacebookUserModel GetFacebookUserModel()
        {
            var currentSession = FacebookWebContext.Current.Session;
            Int64 facebookId = Convert.ToInt64(currentSession.UserId);
            var cacheKey = "Facebook_Channel_FacebookModel_" + facebookId.ToString();
            FacebookUserModel fbUserModel = new FacebookUserModel();
            bool isfbUserModelInCache;
            isfbUserModelInCache = ChannelRuntimeCache.RetrieveFromCache<FacebookUserModel>(cacheKey, ref fbUserModel);
            if (isfbUserModelInCache == false
                || (isfbUserModelInCache == true && !isValidFbModel(fbUserModel, currentSession)))
            {
                var api = new FacebookWebClient();

                dynamic myInfo = api.Get("/me?fields=third_party_id,email,name,first_name,last_name,username,gender,location,locale");

                /** intialisation of value **/
                fbUserModel.FacebookId = Convert.ToInt64(currentSession.UserId);
                fbUserModel.FacebookThirdPartyId = myInfo.third_party_id != null ? myInfo.third_party_id : String.Empty;
                fbUserModel.AccessToken = currentSession.AccessToken;
                fbUserModel.Expires = currentSession.Expires;
                fbUserModel.Name = myInfo.name;
                fbUserModel.Email = myInfo.email != null ? myInfo.email : String.Empty;
                fbUserModel.FirstName = myInfo.first_name;
                fbUserModel.LastName = myInfo.last_name;
                fbUserModel.UserName = myInfo.username;
                fbUserModel.Gender = myInfo.gender;
                fbUserModel.PicturePath = GetUserPicture(fbUserModel.FacebookId);

                if (myInfo.location != null)
                {
                    fbUserModel.City = myInfo.location.name;
                    fbUserModel.Country = myInfo.location.name;
                }

                fbUserModel.Locale = myInfo.locale;
                ChannelRuntimeCache.AddToCache<FacebookUserModel>(cacheKey, fbUserModel, fbUserModel.Expires);
            }

            fbUserModel.Cmid = 0;  // important to reset the cmid to 0  (because it's saved in cache) to unauthenticate the user, we do that to enforce authentication
            return fbUserModel;
        }

        public string GetFirstName(long id)
        {
            var api = new FacebookWebClient();
            bool trashbool;

            dynamic myInfos = api.Get("/" + id + "?fields=first_name");
            if (bool.TryParse(myInfos.ToString(), out trashbool) || myInfos.first_name == null)
                return string.Empty;
            return myInfos.first_name;
        }

        public List<FacebookBasicStatisticView> GetAppUsersFriends()
        {
            List<FacebookBasicStatisticView> facebookFriendsUsingApp = new List<FacebookBasicStatisticView>();

            try
            {
                HttpWebRequest request = WebRequest.Create(string.Format("https://api.facebook.com/method/friends.getAppUsers?access_token={0}&format=json", FacebookWebContext.Current.Session.AccessToken)) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string retVal = reader.ReadToEnd();
                    string[] friendIds = retVal.Replace("[", "").Replace("]", "").Split(',');
                    int i = 1;
                    foreach (var id in friendIds)
                    {
                        long friendId;
                        long.TryParse(id, out friendId);
                        if (friendId > 0)
                        {
                            var fbUserModel = new FacebookBasicStatisticView();
                            fbUserModel.FacebookId = friendId;
                            fbUserModel.FirstName = this.GetFirstName(friendId);
                            fbUserModel.PicturePath = this.GetUserPicture(friendId);
                            facebookFriendsUsingApp.Add(fbUserModel);
                            i++;
                        }
                    }
                }
            }
            catch (FacebookApiException)
            {
                // We swallow the exception as we don't have the right to retrieve the user friends that installed the app
            }

            return facebookFriendsUsingApp;
        }

        public string GetUserPicture(long facebookId)
        {
            return "https://graph.facebook.com/" + facebookId + "/picture";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.Channels.Utils;

namespace UberStrike.Channels.Facebook.Controllers
{
    public class MessageController : Controller
    {
        FacebookSdkWrapper FacebookSDKWrapper;

        //
        // GET: /Message/

        public ActionResult InviteSent()
        {
            FacebookSDKWrapper = new FacebookSdkWrapper();
            bool invitationSent = false;
            try
            {
                if (Request.Browser.Browser == "IE")
                {
                    int i = 0;
                    List<string> ids = new List<string>();
                    for (int j = 0; j < Request.Params.Count; j++)
                    {
                        if (Request.Params["ids[" + i + "]"] != null)
                        {
                            ids.Add(Request.Params["ids[" + i + "]"]);
                            i++;
                        }
                    }
                    if (ids.Count() > 0)
                        invitationSent = true;

                }
               
                if (Request.Params["ids[]"] != null)
                {
                    var ids = Request.Params["ids[]"].ToString().Split(',');
                    if (ids.Count() > 0)
                        invitationSent = true;
                }
            }
            catch (Exception)
            {
            }
            ViewBag.InvitationSent = invitationSent;
            ViewBag.FacebookApplicationUrl = FacebookSDKWrapper.GetFacebookApplicationUrl();
            return View();
        }
    }
}

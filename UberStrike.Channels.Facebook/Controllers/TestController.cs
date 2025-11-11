using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook.Web.Mvc;

namespace UberStrike.Channels.Facebook.Controllers
{
    [CanvasAuthorize(Permissions = "publish_stream,email,publish_actions")]
    public class TestController : BaseController
    {
        #region Constructors
        public TestController()
            : base()
        {
            base.CurrentTab = Utils.Menu.MainTab.Test;
        }
        #endregion

        //
        // GET: /Test/

        public ActionResult Index()
        {
            return View();
        }

    }
}

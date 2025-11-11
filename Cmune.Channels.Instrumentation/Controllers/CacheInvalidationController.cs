using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.DataCenter.Utils;

namespace Cmune.Channels.Instrumentation.Controllers
{
    public class CacheInvalidationController : Controller
    {
        //
        // GET: /CacheInvalidation/

        [ValidateInput(false)]
        public void Index()
        {
            string cacheName = Request.QueryString["name"];
            string secret = Request.QueryString["secret"];

            bool isCacheInvalidated = UberStrikeCacheInvalidation.InvalidateLocalCache(cacheName, secret);

            Response.ContentEncoding = System.Text.Encoding.GetEncoding("iso-8859-1");
            Response.ContentType = "plain/text";
            Response.Write(isCacheInvalidated.ToString());
        }

    }
}

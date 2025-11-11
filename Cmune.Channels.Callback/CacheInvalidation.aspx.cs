using System;
using UberStrike.DataCenter.Utils;

namespace Cmune.Channels.Callback
{
    public partial class CacheInvalidation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
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
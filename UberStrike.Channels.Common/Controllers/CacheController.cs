using System;
using System.Web.Mvc;
using UberStrike.Channels.Utils;

namespace UberStrike.Channels.Common.Controllers
{
    public class CacheResult : ActionResult
    {
        private string _keyname;
        private string _version;
        private string _type;

        public CacheResult(string keyname, string version, string type)
        {
            this._keyname = keyname;
            this._version = version;
            if (type.ToLower().Contains("css"))
            {
                this._type = @"text/css";
            }

            if (type.ToLower().Contains("javascript"))
            {
                this._type = @"text/javascript";
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            ScriptCombiner myCombiner = new ScriptCombiner(this._keyname, this._version, this._type);

            myCombiner.ProcessRequest(context.HttpContext);
        }
    }

    public static class CacheControllerExtensions
    {
        public static CacheResult RenderCacheResult
            (string keyname, string version, string type)
        {
            return new CacheResult(keyname, version, type);
        }
    }

    public class CacheController : Controller
    {
        #region Constructors

        public CacheController()
            : base()
        {
        }

        #endregion

        #region Methods

        public CacheResult CacheContent(string key, string version, string type)
        {
            return CacheControllerExtensions.RenderCacheResult(key, version, type);
        }

        #endregion
    }
}
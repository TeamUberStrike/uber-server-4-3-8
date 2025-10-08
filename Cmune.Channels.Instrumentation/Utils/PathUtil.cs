using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmune.Channels.Instrumentation.Utils
{
    public static class PathUtil
    {
        public static string RelativeToAbsolutePath(string relativePath)
        {
            if (HttpContext.Current == null || HttpContext.Current.Server == null)
                throw new NullReferenceException();
            return HttpContext.Current.Server.MapPath(relativePath);
        }
    }
}
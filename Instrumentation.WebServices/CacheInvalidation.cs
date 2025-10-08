using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UberStrike.DataCenter.Utils;

namespace Cmune.Instrumentation.WebServices
{
    public partial class WebServicesClass : IAllWebServicesInterfaces
    {
        /// <summary>
        /// Invalidates a specific cache
        /// </summary>
        /// <param name="name"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public bool InvalidateCache(string name, string secret)
        {
            bool isCacheInvalidated = UberStrikeCacheInvalidation.InvalidateLocalCache(name, secret);

            return isCacheInvalidated;
        }
    }
}
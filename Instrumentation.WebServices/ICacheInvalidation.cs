using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Cmune.Instrumentation.WebServices
{
    /// <summary>
    /// Cache invalidation part ot the WebServicesClass
    /// </summary>
    [ServiceContract]
    public interface ICacheInvalidation
    {
        /// <summary>
        /// Invalidates a specific cache
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "InvalidateCache?name={name}&secret={secret}")]
        bool InvalidateCache(string name, string secret);
    }
}
using System.ServiceModel;
using System.ServiceModel.Web;
using UberStrike.DataCenter.WebService.Attributes;

namespace UberStrike.DataCenter.WebService.Interfaces
{
    [CmuneWebServiceInterface(UseBinaryProtocol=false)]
    [ServiceContract]
    public interface IServerWebService
    {
        [OperationContract]
        [WebGet(UriTemplate = "InvalidateCache?name={cacheName}&secret={secret}")]
        bool InvalidateCache(string cacheName, string secret);
		
		[OperationContract]
        [WebGet(UriTemplate = "IsAlive")]
        string IsAlive();
    }
}
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml.Linq;
using UberStrike.DataCenter.WebService.Attributes;

namespace UberStrike.DataCenter.WebService.Interfaces
{
    [CmuneWebServiceInterface(UseBinaryProtocol=false)]
    [ServiceContract]
    public interface IDescriptorWebService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "Get")]
        XElement Get();
    }
}

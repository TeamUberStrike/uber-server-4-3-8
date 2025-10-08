using System.ServiceModel;
using System.ServiceModel.Channels;
using Cmune.DataCenter.Common.Utils;

namespace UberStrike.DataCenter.WebService
{
    internal static class WebServiceUtil
    {
        public static long GetCurrentContextNetworkAddress()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties messageProperties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string userHostAddress = endpointProperty.Address;
            return TextUtilities.InetAToN(userHostAddress);
        }
    }
}

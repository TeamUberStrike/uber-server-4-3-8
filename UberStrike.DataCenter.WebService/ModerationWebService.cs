using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Cmune.DataCenter.Business;
using UberStrike.DataCenter.WebService.Interfaces;

namespace UberStrike.DataCenter.WebService
{
    public class ModerationWebService : IModerationWebService
    {
        public bool BanPermanently(int sourceCmid, int targetCmid, int applicationId, string ip)
        {
            OperationContext context = OperationContext.Current;
            MessageProperties messageProperties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string userHostAddress = endpointProperty.Address;

            return CmuneMember.BanAccount(sourceCmid, targetCmid, applicationId, userHostAddress, String.Format("Permanent ban via in game moderation"), null);
        }
    }
}
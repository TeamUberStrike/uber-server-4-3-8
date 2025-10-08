using UberStrike.DataCenter.WebService.Attributes;

namespace UberStrike.DataCenter.WebService.Interfaces
{
    [CmuneWebServiceInterface]
    public interface IModerationWebService
    {
        bool BanPermanently(int sourceCmid, int targetCmid, int applicationId, string ip);
    }
}
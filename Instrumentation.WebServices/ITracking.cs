using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Cmune.Instrumentation.WebServices
{
    [ServiceContract]
    public interface ITracking
    {
        [OperationContract(IsOneWay = false)]
        [WebGet(UriTemplate = "SetInstallTracking?stepId={stepId}&channel={channel}&referrerPartnerId={referrerPartnerId}&isJavaInstall={isJavaInstall}&operatingSystem={operatingSystem}&tracking={tracking}&browsername={browsername}&browserversion={browserversion}")]
        void SetInstallTracking(string stepId,
                                string channel,
                                string referrerPartnerId,
                                string isJavaInstall,
                                string operatingSystem,
                                string tracking,
                                string browsername,
                                string browserversion);

        [OperationContract(IsOneWay = true)]
        [WebGet(UriTemplate = "ReportIssue?type={type}&data={data}")]
        void ReportIssue(string type, string data);
    }
}
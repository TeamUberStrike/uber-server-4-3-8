using System.ServiceModel;

namespace Cmune.Instrumentation.WebServices
{
    [ServiceContract]
    public interface IAllWebServicesInterfaces : ITracking, ICacheInvalidation, IApplication, IServerMonitoring
    {
    }
}
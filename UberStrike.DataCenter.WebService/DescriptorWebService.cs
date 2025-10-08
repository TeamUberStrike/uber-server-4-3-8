using System.IO;
using System.Xml.Linq;
using UberStrike.DataCenter.WebService.Interfaces;

namespace UberStrike.DataCenter.WebService
{
    public class DescriptorWebService : IDescriptorWebService
    {
        public XElement Get()
        {
            return XElement.Load(new StringReader(WebServiceDescriptor.Xml));//"WebServiceDescriptor.xml");
        }
    }
}
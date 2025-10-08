using System.Xml.Linq;
using UberStrike.DataCenter.WebService.Interfaces;

namespace UberStrike.DataCenter.WebService
{
    public class CrossDomainWebService : ICrossDomainWebService
    {
        public XElement Get()
        {
            string crossDomain = @"<?xml version=""1.0""?>
                    <cross-domain-policy>
                    <allow-access-from domain=""*""/>
                    </cross-domain-policy>";

            return XElement.Parse(crossDomain, LoadOptions.None);
        }
    }
}

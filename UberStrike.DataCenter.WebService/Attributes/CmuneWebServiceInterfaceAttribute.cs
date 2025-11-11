using System;

namespace UberStrike.DataCenter.WebService.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class CmuneWebServiceInterfaceAttribute : Attribute
    {
        public bool UseBinaryProtocol { get; set; }

        public CmuneWebServiceInterfaceAttribute(bool useBinaryProtocol = true)
        {
            UseBinaryProtocol = useBinaryProtocol;
        }
    }
}

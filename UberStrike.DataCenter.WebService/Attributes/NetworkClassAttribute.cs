using System;

namespace Cmune.Core.Types.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class NetworkClassAttribute : Attribute
    {
        public NetworkClassAttribute() { }

        public bool Static { get; set; }
    }
}

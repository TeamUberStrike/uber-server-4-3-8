using System;

namespace Cmune.Core.Types.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class NetworkMethodAttribute : Attribute
    {
        public NetworkMethodAttribute() { }

        public bool Unreliable { get; set; }
    }
}

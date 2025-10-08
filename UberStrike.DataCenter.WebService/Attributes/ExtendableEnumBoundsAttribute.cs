using System;

namespace UberStrike.DataCenter.WebService.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExtendableEnumBoundsAttribute : Attribute
    {
        public ExtendableEnumBoundsAttribute(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Min { get; private set; }
        public int Max { get; private set; }
    }
}

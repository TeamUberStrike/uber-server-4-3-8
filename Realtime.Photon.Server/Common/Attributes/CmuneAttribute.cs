using System;

namespace Cmune
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CmuneClassAttribute : CmuneAttribute
    { }

    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
    public class CmuneEnumAttribute : CmuneAttribute
    { }

    public class CmuneAttribute : Attribute
    { }
}

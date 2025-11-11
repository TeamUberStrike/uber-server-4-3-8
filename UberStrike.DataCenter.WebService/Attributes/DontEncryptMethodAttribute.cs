using System;

namespace UberStrike.DataCenter.WebService.Attributes
{
    /// <summary>
    /// Omit this Field in the class serialization process
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class DontEncryptMethodAttribute : Attribute
    { }
}

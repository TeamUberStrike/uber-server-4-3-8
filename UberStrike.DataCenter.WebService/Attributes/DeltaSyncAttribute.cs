using System;

namespace UberStrike.DataCenter.WebService.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DeltaSyncAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagId"></param>
        public DeltaSyncAttribute(int tagId)
        {
            TagId = tagId;
        }

        public int TagId { private set; get; }
        public bool IsTagged { get { return TagId > 0; } }
    }
}
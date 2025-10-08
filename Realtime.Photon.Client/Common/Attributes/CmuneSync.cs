using System;

namespace Cmune.Realtime.Common
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class CMUNESYNC : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagId"></param>
        public CMUNESYNC(int tagId)
        {
            TagId = tagId;
        }

        public int TagId { private set; get; }
        public bool IsTagged { get { return TagId > 0; } }
    }
}
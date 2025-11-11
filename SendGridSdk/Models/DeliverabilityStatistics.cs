// -----------------------------------------------------------------------
// <copyright file="DeliverabilityStatistics.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SendGridSdk.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [DataContract]
    public class DeliverabilityStatistics
    {
        [DataMember(Name = "delivered")]
        public long Delivered { get; private set; }
        [DataMember(Name = "unsubscribes")]
        public long Unsubscribes { get; private set; }
        [DataMember(Name = "invalid_email")]
        public long InvalidEmailAddresses { get; private set; }
        [DataMember(Name = "bounces")]
        public long Bounces { get; private set; }
        [DataMember(Name = "repeat_unsubscribes")]
        public long RepeatUnsubscribes { get; private set; }
        [DataMember(Name = "unique_clicks")]
        public long UniqueClicks { get; private set; }
        [DataMember(Name = "blocked")]
        public long Blocked { get; private set; }
        [DataMember(Name = "spam_drop")]
        public long SpamDrop { get; private set; }
        [DataMember(Name = "repeat_bounces")]
        public long RepeatBounces { get; private set; }
        [DataMember(Name = "repeat_spamreports")]
        public long RepeatSpamReports { get; private set; }
        [DataMember(Name = "date")]
        public DateTime Date { get; private set; }
        [DataMember(Name = "requests")]
        public long Requests { get; private set; }
        [DataMember(Name = "spamreports")]
        public long SpamReports { get; private set; }
        [DataMember(Name = "clicks")]
        public long Clicks { get; private set; }
        [DataMember(Name = "opens")]
        public long Opens { get; private set; }
        [DataMember(Name = "unique_opens")]
        public long UniqueOpens { get; private set; }
        [DataMember(Name = "category")]
        public string Category { get; private set; }

        public DeliverabilityStatistics()
        {
        }
    }
}

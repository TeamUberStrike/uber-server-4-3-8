using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Utils;

namespace Cmune.Channels.Instrumentation.Models.Display
{
    public class MemberPreviousEmailDisplay
    {
        public string PreviousEmail { get; protected set; }
        public DateTime ChangeDate { get; protected set; }
        public string Ip { get; protected set; }

        public MemberPreviousEmailDisplay(string previousEmail, DateTime changeDate, long ip)
        {
            PreviousEmail = previousEmail;
            ChangeDate = changeDate;
            Ip = TextUtilities.InetNToA(ip);
        }
    }

    public class AccountPreviousEmailDisplay : MemberPreviousEmailDisplay
    {
        public string AccountName { get; private set; }
        public int Cmid { get; private set; }

        public AccountPreviousEmailDisplay(string previousEmail, DateTime changeDate, long ip, int cmid, string accountName)
            : base(previousEmail, changeDate, ip)
        {
            Cmid = cmid;
            AccountName = accountName;
        }
    }

    public class MemberPreviousNameDisplay
    {
        public string PreviousName { get; protected set; }
        public DateTime ChangeDate { get; protected set; }
        public string Ip { get; protected set; }

        public MemberPreviousNameDisplay(string previousName, DateTime changeDate, long ip)
        {
            PreviousName = previousName;
            ChangeDate = changeDate;
            Ip = TextUtilities.InetNToA(ip);
        }
    }

    public class AccountPreviousNameDisplay : MemberPreviousNameDisplay
    {
        public string AccountName { get; private set; }
        public int Cmid { get; private set; }

        public AccountPreviousNameDisplay(string previousName, DateTime changeDate, long ip, int cmid, string accountName)
            : base(previousName, changeDate, ip)
        {
            Cmid = cmid;
            AccountName = accountName;
        }
    }
}
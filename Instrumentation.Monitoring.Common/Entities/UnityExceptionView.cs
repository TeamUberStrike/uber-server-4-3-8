using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Instrumentation.Monitoring.Common.Entities
{
    public class UnityExceptionView
    {
        #region Properties

        public int UnityExceptionId { get; private set; }
        public string ExceptionType { get; private set; }
        public string ExceptionMessage { get; private set; }
        public int Cmid { get; private set; }
        public string MemberName { get; private set; }
        public ChannelType Channel { get; private set; }
        public BuildType Build { get; private set; }
        public DateTime ExceptionTime { get; private set; }
        public string Stacktrace { get; private set; }
        public string StacktraceHash { get; private set; }
        public string FaultiveFunction { get; private set; }
        public string ExceptionData { get; private set; }
        public string BuildNumber { get; private set; }

        #endregion

        #region Constructors

        public UnityExceptionView(int unityExceptionId, string exceptionType, string exceptionMessage, int cmid, string memberName, ChannelType channel, BuildType build, DateTime exceptionTime, string stacktrace, string stacktraceHash, string faultiveFunction, string exceptionData, string buildNumber)
        {
            this.UnityExceptionId = unityExceptionId;
            this.ExceptionType = exceptionType;
            this.ExceptionMessage = exceptionMessage;
            this.Cmid = cmid;
            this.MemberName = memberName;
            this.Channel = channel;
            this.Build = build;
            this.ExceptionTime = exceptionTime;
            this.Stacktrace = stacktrace;
            this.StacktraceHash = stacktraceHash;
            this.FaultiveFunction = faultiveFunction;
            this.ExceptionData = exceptionData;
            this.BuildNumber = buildNumber;
        }

        public UnityExceptionView(int unityExceptionId, string exceptionType, string exceptionMessage, int cmid, ChannelType channel, BuildType build, DateTime exceptionTime, string stacktrace, string stacktraceHash, string faultiveFunction, string exceptionData, string buildNumber)
            : this(unityExceptionId, exceptionType, exceptionMessage, cmid, String.Empty, channel, build, exceptionTime, stacktrace, stacktraceHash, faultiveFunction, exceptionData, buildNumber)
        {
        }

        #endregion
    }
}
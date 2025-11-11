// -----------------------------------------------------------------------
// <copyright file="UnityExceptionGroupSummaryView.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Cmune.Instrumentation.Monitoring.Common.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class UnityExceptionGroupSummaryView
    {
        #region Properties

        public string ExceptionType { get; private set; }
        public string ExceptionMessage { get; private set; }
        public string FaultiveFunction { get; private set; }
        public int ExceptionCount { get; private set; }
        public string StacktraceHash { get; private set; }
        public DateTime? LatestException { get; private set; }

        #endregion

        #region Constructors

        public UnityExceptionGroupSummaryView(string exceptionType, string exceptionMessage, string faultiveFunction, int exceptionCount, string stackTraceHash, DateTime? latestException)
        {
            this.ExceptionType = exceptionType;
            this.ExceptionMessage = exceptionMessage;
            this.FaultiveFunction = faultiveFunction;
            this.ExceptionCount = exceptionCount;
            this.StacktraceHash = stackTraceHash;
            this.LatestException = latestException;
        }

        public UnityExceptionGroupSummaryView(string exceptionType, string exceptionMessage, string faultiveFunction, string stackTraceHash)
            : this(exceptionType, exceptionMessage, faultiveFunction, 0, stackTraceHash, null)
        {
        }

        #endregion
    }
}

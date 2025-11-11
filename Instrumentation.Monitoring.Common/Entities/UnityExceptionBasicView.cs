// -----------------------------------------------------------------------
// <copyright file="UnityExceptionBasicView.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Cmune.Instrumentation.Monitoring.Common.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cmune.DataCenter.Common.Entities;

    public class UnityExceptionBasicView
    {
        #region Properties

        public string BuildNumber { get; private set; }
        public DateTime ExceptionTime { get; private set; }
        public int Cmid { get; private set; }
        public ChannelType Channel { get; private set; }
        public int UnityExceptionId { get; private set; }

        #endregion

        #region Constructors

        public UnityExceptionBasicView(string buildNumber, DateTime exceptionTime, int cmid, ChannelType channel, int unityExceptionId)
        {
            this.BuildNumber = buildNumber;
            this.ExceptionTime = exceptionTime;
            this.Cmid = cmid;
            this.Channel = channel;
            this.UnityExceptionId = unityExceptionId;
        }

        #endregion
    }
}

// -----------------------------------------------------------------------
// <copyright file="UnityExceptionGroupDetailedView.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Cmune.Instrumentation.Monitoring.Common.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class UnityExceptionGroupDetailedView
    {
        #region Properties

        public string Stacktrace { get; private set; }
        public UnityExceptionGroupSummaryView Group { get; set; }
        public List<UnityExceptionBasicView> Exceptions { get; private set; }

        #endregion

        #region Constructors

        public UnityExceptionGroupDetailedView(string stacktrace, UnityExceptionGroupSummaryView group, List<UnityExceptionBasicView> exceptions)
        {
            this.Stacktrace = stacktrace;
            this.Group = group;
            this.Exceptions = exceptions;
        }

        #endregion
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CounterLogger.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CounterLogger type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Threading;

using log4net;

namespace Cmune.Realtime.Photon.Server.Diagnostics
{
    /// <summary>
    /// Logs the most intersting counters into a log file
    /// </summary>
    public class CounterLogger
    {
        /// <summary>
        /// Get logger for the counter log file.
        /// </summary>
        private static readonly ILog counterLog = LogManager.GetLogger("PerformanceCounter");

        /// <summary>
        /// Get logger for debug out.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Log interval is set to 5 seconds.
        /// </summary>
        private const int LogIntervalMs = 60 * 1000;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static readonly CounterLogger instance = new CounterLogger();

        /// <summary>
        /// Timer used to trigger log output
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Prevents a default instance of the <see cref="CounterLogger"/> class from being created.
        /// </summary>
        private CounterLogger()
        {
        }

        /// <summary>
        /// Gets an sigelton instance of the <see cref="CounterLogger"/> class.
        /// </summary>
        /// <value>The instance.</value>
        public static CounterLogger Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Starts the log output.
        /// </summary>
        public void Start()
        {
            var callback = new TimerCallback(LogCounter);
            this.timer = new Timer(callback, null, 0, LogIntervalMs);
        }

        /// <summary>
        /// Callback to write counter values to a log counter.
        /// </summary>
        /// <param name="state">State value.</param>
        private static void LogCounter(object state)
        {
            try
            {
                counterLog.InfoFormat(
                    "Games = {0:N2}",
                    Counter.Games.GetNextValue());//global::Photon.SocketServer.Diagnostics.PhotonCounter.SessionCount.GetNextValue());
                counterLog.InfoFormat(
                    "Players = {0:N2}",
                    Counter.Players.GetNextValue());//global::Photon.SocketServer.Diagnostics.PhotonCounter.OperationReceivePerSec.GetNextValue());
                counterLog.InfoFormat(
                    "WebserviceCalls = {0:N2}",
                    Counter.WebserviceCalls.GetNextValue());//global::Photon.SocketServer.Diagnostics.PhotonCounter.OperationResponsePerSec.GetNextValue());
                counterLog.InfoFormat(
                    "WebserviceCallsSetScore = {0:N2}",
                    Counter.WebserviceCallsSetScore.GetNextValue());//global::Photon.SocketServer.Diagnostics.PhotonCounter.EventSentPerSec.GetNextValue());
                counterLog.InfoFormat(
                    "WebserviceExceptions = {0:N2}\n",
                    Counter.WebserviceExceptions.GetNextValue());//global::Photon.SocketServer.Diagnostics.PhotonCounter.AverageOperationExecutionTime.GetNextValue());
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
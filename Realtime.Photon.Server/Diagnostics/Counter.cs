// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Counter.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Counter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using ExitGames.Diagnostics.Counter;
using ExitGames.Diagnostics.Monitoring;

namespace Cmune.Realtime.Photon.Server.Diagnostics
{
    /// <summary>
    /// Counter on application level
    /// </summary>
    public static class Counter
    {
        /// <summary>
        /// Absolute number of games active (in the game cache).
        /// </summary>
        [PublishCounter("Games")]
        public static readonly NumericCounter Games = new NumericCounter("Games");

        /// <summary>
        /// Absolute number of games active (in the game cache).
        /// </summary>
        [PublishCounter("Players")]
        public static readonly NumericCounter Players = new NumericCounter("Players");

        /// <summary>
        /// Absolute number of games active (in the game cache).
        /// </summary>
        [PublishCounter("WebserviceCalls")]
        public static readonly NumericCounter WebserviceCalls = new NumericCounter("WebserviceCalls");

        /// <summary>
        /// Absolute number of games active (in the game cache).
        /// </summary>
        [PublishCounter("WebserviceExceptions")]
        public static readonly NumericCounter WebserviceExceptions = new NumericCounter("WebserviceExceptions");

        /// <summary>
        /// Absolute number of games active (in the game cache).
        /// </summary>
        [PublishCounter("WebserviceCallsSetScore")]
        public static readonly NumericCounter WebserviceCallsSetScore = new NumericCounter("WebserviceCallsSetScore");
    }
}
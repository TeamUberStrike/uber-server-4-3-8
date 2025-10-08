using System;

namespace UberStrike.Helper
{
    public static class SystemTime
    {
        /// <summary>
        /// Environment.TickCount cycles between Int32.MinValue, which is a negative 
        /// number, and Int32.MaxValue once every 49.8 days. This function
        /// removes the sign bit to yield a nonnegative number that cycles 
        /// between zero and Int32.MaxValue once every 24.9 days.
        /// </summary>
        public static int Running
        {
            get { return Environment.TickCount & Int32.MaxValue; }
        }
    }
}
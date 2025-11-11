using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cmune.Util
{
    public static class CmuneDebug
    {
        public static int DebugLevel = 0;
        public static bool DebugFileInfo = false;

        private static List<ICmuneDebug> _debugChannels = new List<ICmuneDebug>();

        public static bool IsDebugEnabled { get { return DebugLevel == 0; } }
        public static bool IsWarningEnabled { get { return DebugLevel <= 1; } }
        public static bool IsErrorEnabled { get { return DebugLevel <= 2; } }

        public static void Assert(bool condition, string message)
        {
            if (!condition) CmuneDebug.LogError("ASSERT: " + message);
        }

        public static void AddDebugChannel(ICmuneDebug channel)
        {
            if (!_debugChannels.Exists(p => p.GetType() == channel.GetType()))
                _debugChannels.Add(channel);
        }

        public static Exception Exception(Exception innerException, string str, params object[] args)
        {

            string t = string.Format(str + "\n" + innerException.Message, args);

            CmuneDebug.LogError(t);

            return new Exception(t);
        }

        public static Exception Exception(string str, params object[] args)
        {
            string t = string.Format(str, args);

            CmuneDebug.LogError(str, args);

            return new Exception(string.Format(str, args));
        }

        public static void Log(string str, params object[] args)
        {
            log(string.Format(str, args), 0);
        }

        public static void LogWarning(string str, params object[] args)
        {
            log("[WARNING] " + string.Format(str, args), 1);
        }

        public static void LogError(string str, params object[] args)
        {
            log("[ERROR] " + string.Format(str, args), 2);
        }

        public static void LogInfo(string str, params object[] args)
        {
            log(string.Format(str, args), 3);
        }

        public static void Log(System.Object t)
        {
            log(t.ToString(), 0);
        }

        public static void LogWarning(System.Object t)
        {
            log("[WARNING] " + t.ToString(), 1);
        }

        public static void LogError(System.Object t)
        {
            log("[ERROR] " + t.ToString(), 2);
        }

        public static void LogInfo(System.Object t)
        {
            log(t.ToString(), 3);
        }

        private static void log(string s, int i)
        {
            if (i >= DebugLevel)
            {
                foreach (ICmuneDebug d in _debugChannels)
                {
                    d.Log(i, s);
                }
            }
        }
    }

    public interface ICmuneDebug
    {
        void Log(int level, string s);
    }

    public class DefaultDebug : ICmuneDebug
    {
        public void Log(int level, string s)
        {
            System.Diagnostics.Debug.Print(s);
        }
    }
}
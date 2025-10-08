using System;
using System.IO;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using NLog;
using System.Text.RegularExpressions;
using System.Text;

namespace Cmune.DataCenter.Utils
{
    /// <summary>
    /// Log type
    /// </summary>
    public enum LogType
    {
        MissingConfigValue = 1,
        UnexpectedReturn = 2,
        Exception = 3,
        Warning = 4,
        Info = 5
    }

    /// <summary>
    /// Log origin
    /// </summary>
    public enum LogSource
    {
        None = 0,
        UberstrikePortal = 1,
        UberstrikeFacebook = 2,
        UberstrikeWebServices = 3,
        Instrumentation = 4,
        InstrumentationWebServices = 5,
        CoreWebServices = 6,
        UberstrikeBatch = 7,
        CoreBatch = 8,
        UberstrikeMySpace = 9,
        ChannelCallback = 10,
        UberstrikeCommon = 11,
        UberStrikeCyworld = 12,
        UberStrikeKongregate = 13,
    }

    public static class CmuneLog
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="logType"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static LogEventInfo InitLog(LogLevel logLevel, LogType logType, string fileName = "")
        {
            LogSource logSource = LogSource.None;
            BuildType buildType = BuildType.Prod;
            ReadConfig(out logSource, out buildType);
            var logEventInfo = new LogEventInfo();
            logEventInfo.Level = logLevel;
            logEventInfo.LoggerName = logType.ToString();
            logEventInfo.Context["logTypeName"] = logType.ToString();
            logEventInfo.Context["logSource"] = logSource.ToString();
            logEventInfo.Context["environment"] = buildType.ToString();
            logEventInfo.Context["logTime"] = DateTime.Now.ToUniversalTime();
            if (!fileName.IsNullOrFullyEmpty())
            {
                var customLog = ConfigurationUtilities.ReadConfigurationManager("CustomLogPath", false);
                logEventInfo.Context["fileName"] = customLog + fileName;
            }
            return logEventInfo;
        }

        private static void FillLog(LogEventInfo logEventInfo, string errorLocation, string error, string incomingDatas)
        {
            logEventInfo.Context["errorLocation"] = errorLocation;
            logEventInfo.Context["error"] = error;
            logEventInfo.Context["incomingDatas"] = incomingDatas;
        }

        private static void FillWarningLog(LogEventInfo logEventInfo, string warningMessage)
        {
            logEventInfo.Context["message"] = warningMessage;
        }

        private static void FillInfoLog(LogEventInfo logEventInfo, string infoMessage)
        {
            logEventInfo.Context["message"] = infoMessage;
        }

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="incomingDatas"></param>
        /// <param name="fileName"></param>
        public static void LogException(Exception ex, string incomingDatas, string fileName = "")
        {
            var logEventInfo = InitLog(LogLevel.Error, LogType.Exception, fileName);

            var location = Regex.Replace(ex.StackTrace, "at ", "<br />at ") + "<br/>(<i>StackTrace</i>)";
            var error = "";
            error += "" + ex.Message + "(<i>Message</i>)<br/>";
            error += "<i>Type</i>: " + ex.GetType().Name + "<br/>";
            error += "<i>Source</i>: " + ex.Source + "<br/>";
            error += "<i>TargetSite</i>: " + (ex.TargetSite != null ? ex.TargetSite.ToString() : string.Empty) + "<br/>";

            StringBuilder userDefinedData = new StringBuilder();

            if (ex.Data != null)
            {
                userDefinedData.AppendLine("<ul>");

                foreach (string key in ex.Data.Keys)
                {
                    userDefinedData.AppendLine(String.Format("<li><i>{0}</i>: {1}</li>", key, ex.Data[key]));
                }

                userDefinedData.AppendLine("</ul>");
            }

            error += "<i>Data</i>: " + userDefinedData.ToString() + "<br/>";
            error += "<i>InnerException</i>:<br />" + (ex.InnerException != null ? Regex.Replace(ex.InnerException.ToString(), "at ", "<br />at ") : string.Empty) + "<br/>";
            error += "<i>HelpLink</i>: " + ex.HelpLink;

            FillLog(logEventInfo, location, error, incomingDatas);
            Logger.Log(logEventInfo);
        }

        /// <summary>
        /// Logs a missing config value when we try to read a config from the app.config or web.config
        /// </summary>
        /// <param name="missingKey"></param>
        /// <param name="fileName"></param>
        public static void LogMissingConfigValue(string missingKey, string fileName = "")
        {
            var logEventInfo = InitLog(LogLevel.Error, LogType.MissingConfigValue);
            FillLog(logEventInfo, string.Empty, missingKey + " config key is missing", string.Empty);
            Logger.Log(logEventInfo);
        }


        /// <summary>
        /// Logs an unexpected return from a function (very useful when a function returns an Enum)
        /// </summary>
        /// <param name="returnedObject"></param>
        /// <param name="error"></param>
        /// <param name="fileName"></param>
        public static void LogUnexpectedReturn(Object returnedObject, string error, string fileName = "")
        {
            var logEventInfo = InitLog(LogLevel.Error, LogType.UnexpectedReturn);
            FillLog(logEventInfo, string.Empty, error, returnedObject.ToString());
            Logger.Log(logEventInfo);
        }


        /// <summary>
        ///  Logs a warning
        /// </summary>
        /// <param name="logString"></param>
        /// <param name="fileName"></param>
        public static void LogWarning(string logString, string fileName = "")
        {
            var logEventInfo = InitLog(LogLevel.Warn, LogType.Warning, fileName);

            var warning = "";
            warning += logString;

            FillWarningLog(logEventInfo, warning);
            Logger.Log(logEventInfo);
        }


        /// <summary>
        /// logs an information
        /// </summary>
        /// <param name="information"></param>
        /// <param name="fileName"></param>
        public static void LogInfo(string information, string fileName = "")
        {
            var logEventInfo = InitLog(LogLevel.Info, LogType.Info, fileName);

            var log = "";
            log += information;

            FillInfoLog(logEventInfo, information);
            Logger.Log(logEventInfo);
        }


        /// <summary>
        /// Writes a string to a file, the log file will be created if missing
        /// </summary>
        /// <param name="logPath">Should be writeable by user running the process</param>
        /// <param name="logContent"></param>
        public static void CustomLog(string logPath, string logContent)
        {
            bool isCustomLogEnable = ConfigurationUtilities.ReadConfigurationManagerBool("CustomLogIsEnable");

            if (isCustomLogEnable)
            {
                var logEventInfo = new LogEventInfo();
                logEventInfo.Level = LogLevel.Info;
                logEventInfo.LoggerName = LogType.Info.ToString();
                logEventInfo.Context["message"] = logContent;
                logEventInfo.Context["logTime"] = DateTime.Now.ToUniversalTime();
                logEventInfo.Context["fileName"] = logPath;

                Logger.Log(logEventInfo);
            }
        }

        /// <summary>
        /// Writes a string to the default path, the log file will be created if missing
        /// </summary>
        /// <param name="logFileName">Contains only the name of the log file</param>
        /// <param name="logContent"></param>
        public static void CustomLogToDefaultPath(string logFileName, string logContent)
        {
            CustomLog(ConfigurationUtilities.ReadConfigurationManager("CustomLogPath") + logFileName, logContent);
        }

        /// <summary>
        /// Allow us to have a nice display on a log file
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string DisplayForLog(int value, int length)
        {
            string ret = value.ToString();

            return ret.PadLeft(length);
        }

        /// <summary>
        /// Gets the full name of a type
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetObjectTypeName(Object obj)
        {
            string objectTypeName = "null";

            if (obj != null)
            {
                objectTypeName = obj.GetType().FullName;
            }

            return objectTypeName;
        }

        /// <summary>
        /// Reads the configuration from the config file (app.config or web.config)
        /// Uses default values if they are missing from the config file
        /// </summary>
        /// <param name="logSource"></param>
        /// <param name="buildSource"></param>
        private static void ReadConfig(out LogSource logSource, out BuildType buildSource)
        {
            logSource = LogSource.None;
            buildSource = BuildType.Prod;

            int logSourceIndex = ConfigurationUtilities.ReadConfigurationManagerInt("NLogSource", false);

            if (Enum.IsDefined(typeof(LogSource), logSourceIndex))
            {
                logSource = (LogSource)logSourceIndex;
            }

            int buildSourceIndex = ConfigurationUtilities.ReadConfigurationManagerInt("NLogBuild", false);
            EnumUtilities.TryParseEnumByValue(buildSourceIndex, out buildSource);
        }
    }
}
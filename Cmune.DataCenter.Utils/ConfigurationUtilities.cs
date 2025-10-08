using System;
using System.Configuration;
using System.Linq;

namespace Cmune.DataCenter.Utils
{
    public static class ConfigurationUtilities
    {
        /// <summary>
        /// Allows us to read an application setting and log it if it's missing
        /// </summary>
        /// <param name="applicationSettingsName">Case sensitive</param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">When the key is missing</exception>
        public static string ReadConfigurationManager(string applicationSettingsName)
        {
            string value = ReadConfigurationManager(applicationSettingsName, true);

            return value;
        }

        /// <summary>
        /// Allows us to read an application setting and log it if it's missing
        /// Calls this override when the config can be missing
        /// </summary>
        /// <param name="applicationSettingsName">Case sensitive</param>
        /// <param name="throwException">Should throw exception if key is missing</param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">When the key is missing and throwException is true</exception>
        public static string ReadConfigurationManager(string applicationSettingsName, bool throwException)
        {
            string value = String.Empty;

            if (ConfigurationManager.AppSettings.AllKeys.Contains(applicationSettingsName))
            {
                value = ConfigurationManager.AppSettings[applicationSettingsName];
            }
            else if (throwException)
            {
                CmuneLog.LogMissingConfigValue(applicationSettingsName);
                throw new ConfigurationErrorsException("Unable to read: " + applicationSettingsName);
            }

            return value;
        }

        /// <summary>
        /// Allows us to read an integer application setting and log it if it's missing
        /// </summary>
        /// <param name="applicationSettingsName"></param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">When the key is missing</exception>
        public static int ReadConfigurationManagerInt(string applicationSettingsName)
        {
            int value = ReadConfigurationManagerInt(applicationSettingsName, true);

            return value;
        }

        /// <summary>
        /// Allows us to read an integer application setting and log it if it's missing
        /// Calls this override when the config can be missing
        /// </summary>
        /// <param name="applicationSettingsName"></param>
        /// <param name="throwException">Should throw exception if key is missing</param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">When the key is missing or is not a Int32 and throwException is true</exception>
        public static int ReadConfigurationManagerInt(string applicationSettingsName, bool throwException)
        {
            int value = 0;
            bool isParsed = false;

            if (!ConfigurationManager.AppSettings[applicationSettingsName].IsNullOrFullyEmpty())
            {
                isParsed = Int32.TryParse(ConfigurationManager.AppSettings[applicationSettingsName], out value);
            }
            else
            {
                isParsed = false;
            }

            if (!isParsed && throwException)
            {
                CmuneLog.LogMissingConfigValue(applicationSettingsName);
                throw new ConfigurationErrorsException("Unable to read: " + applicationSettingsName);
            }

            return value;
        }

        /// <summary>
        /// Allows us to read a long application setting and log it if it's missing
        /// </summary>
        /// <param name="applicationSettingsName"></param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">When the key is missing or is not a Int64</exception>
        public static long ReadConfigurationManagerLong(string applicationSettingsName)
        {
            long value = ReadConfigurationManagerLong(applicationSettingsName, true);

            return value;
        }

        /// <summary>
        /// Allows us to read a long application setting and log it if it's missing
        /// Calls this override when the config can be missing
        /// </summary>
        /// <param name="applicationSettingsName"></param>
        /// <param name="throwException">Should throw exception if key is missing</param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">When the key is missing or is not a Int64 and throwException is true</exception>
        public static long ReadConfigurationManagerLong(string applicationSettingsName, bool throwException)
        {
            long value = 0;
            bool isParsed = false;

            if (!ConfigurationManager.AppSettings[applicationSettingsName].IsNullOrFullyEmpty())
            {
                isParsed = Int64.TryParse(ConfigurationManager.AppSettings[applicationSettingsName], out value);
            }
            else
            {
                isParsed = false;
            }

            if (!isParsed && throwException)
            {
                CmuneLog.LogMissingConfigValue(applicationSettingsName);
                throw new ConfigurationErrorsException("Unable to read: " + applicationSettingsName);
            }

            return value;
        }

        /// <summary>
        /// Allows us to read a boolean application setting and log it if it's missing
        /// </summary>
        /// <param name="applicationSettingsName"></param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">When the key is missing or is not a Boolean</exception>
        public static bool ReadConfigurationManagerBool(string applicationSettingsName)
        {
            bool value = ReadConfigurationManagerBool(applicationSettingsName, true);

            return value;
        }

        /// <summary>
        /// Allows us to read a boolean application setting and log it if it's missing
        /// Calls this override when the config can be missing
        /// </summary>
        /// <param name="applicationSettingsName"></param>
        /// <param name="throwException">Should throw exception if key is missing</param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">When the key is missing or is not a Boolean and throwException is true</exception>
        public static bool ReadConfigurationManagerBool(string applicationSettingsName, bool throwException)
        {
            bool value = false;
            bool isParsed = false;

            if (!ConfigurationManager.AppSettings[applicationSettingsName].IsNullOrFullyEmpty())
            {
                isParsed = Boolean.TryParse(ConfigurationManager.AppSettings[applicationSettingsName], out value);
            }
            else
            {
                isParsed = false;
            }

            if (!isParsed && throwException)
            {
                CmuneLog.LogMissingConfigValue(applicationSettingsName);
                throw new ConfigurationErrorsException("Unable to read: " + applicationSettingsName);
            }

            return value;
        }

        /// <summary>
        /// Allows us to read a float application setting and log it if it's missing
        /// </summary>
        /// <param name="applicationSettingsName"></param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">When the key is missing or is not a Float</exception>
        public static float ReadConfigurationManagerFloat(string applicationSettingsName)
        {
            float value = ReadConfigurationManagerFloat(applicationSettingsName, true);

            return value;
        }

        /// <summary>
        /// Allows us to read a float application setting and log it if it's missing
        /// Calls this override when the config can be missing
        /// </summary>
        /// <param name="applicationSettingsName"></param>
        /// <param name="throwException">Should throw exception if key is missing</param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">When the key is missing or is not a float and throwException is true</exception>
        public static float ReadConfigurationManagerFloat(string applicationSettingsName, bool throwException)
        {
            float value = 0f;
            bool isParsed = false;

            if (!ConfigurationManager.AppSettings[applicationSettingsName].IsNullOrFullyEmpty())
            {
                isParsed = Single.TryParse(ConfigurationManager.AppSettings[applicationSettingsName], out value);
            }
            else
            {
                isParsed = false;
            }

            if (!isParsed && throwException)
            {
                CmuneLog.LogMissingConfigValue(applicationSettingsName);
                throw new ConfigurationErrorsException("Unable to read: " + applicationSettingsName);
            }

            return value;
        }

        /// <summary>
        /// Allows us to read a decimal application setting and log it if it's missing
        /// </summary>
        /// <param name="applicationSettingsName"></param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">When the key is missing or is not a Float</exception>
        public static decimal ReadConfigurationManagerDecimal(string applicationSettingsName)
        {
            decimal value = ReadConfigurationManagerDecimal(applicationSettingsName, true);

            return value;
        }

        /// <summary>
        /// Allows us to read a decimal application setting and log it if it's missing
        /// Calls this override when the config can be missing
        /// </summary>
        /// <param name="applicationSettingsName"></param>
        /// <param name="throwException">Should throw exception if key is missing</param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException">When the key is missing or is not a float and throwException is true</exception>
        public static decimal ReadConfigurationManagerDecimal(string applicationSettingsName, bool throwException)
        {
            decimal value = 0;
            bool isParsed = false;

            if (!ConfigurationManager.AppSettings[applicationSettingsName].IsNullOrFullyEmpty())
            {
                isParsed = Decimal.TryParse(ConfigurationManager.AppSettings[applicationSettingsName], out value);
            }
            else
            {
                isParsed = false;
            }

            if (!isParsed && throwException)
            {
                CmuneLog.LogMissingConfigValue(applicationSettingsName);
                throw new ConfigurationErrorsException("Unable to read: " + applicationSettingsName);
            }

            return value;
        }
    }
}
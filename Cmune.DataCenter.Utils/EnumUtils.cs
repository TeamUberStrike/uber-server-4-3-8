using System;
using System.Globalization;

namespace Cmune.DataCenter.Utils
{
    public static class EnumUtils
    {
        /// <summary>
        /// Transform an Enum to display it nicely
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string PrepareEnumForDisplay(string enumValue)
        {
            string enumDisplayed = String.Empty;

            if (!enumValue.IsNullOrFullyEmpty())
            {
                enumValue = enumValue.Replace('_', ' ').ToLower();

                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                enumDisplayed = myTI.ToTitleCase(enumValue);
            }

            return enumDisplayed;
        }
    }
}
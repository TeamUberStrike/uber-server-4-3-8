using System;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.DataCenter.Common.Utils
{
    public static class EnumUtilities
    {
        /// <summary>
        /// Try to parse an enum by name
        /// </summary>
        /// <typeparam name="T">Should be an enum</typeparam>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseEnumByName<T>(string value, out T result) where T : struct, IConvertible
        {
            return TryParseEnumByName(value, default(T), out result);
        }

        /// <summary>
        /// Try to parse an enum by name
        /// </summary>
        /// <typeparam name="T">Should be an enum</typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseEnumByName<T>(string value, T defaultValue, out T result) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

            bool isEnumParsed = false;

            result = defaultValue;

            try
            {
                string[] names = Enum.GetNames(typeof(T));

                for (int i = 0; i < names.Length; i++)
                {
                    if (names[i].Equals(value))
                    {
                        result = (T)Enum.Parse(typeof(T), value, false);
                        isEnumParsed = true;
                        i = names.Length;
                    }
                }
            }
            catch
            {
            }

            return isEnumParsed;
        }

        /// <summary>
        /// Try to parse an enum by value
        /// </summary>
        /// <typeparam name="T">Should be an enum</typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseEnumByValue<T>(string value, T defaultValue, out T result) where T : struct, IConvertible
        {
            bool isEnumParsed = false;

            result = defaultValue;

            int enumIndex = 0;
            bool isValueInt = Int32.TryParse(value, out enumIndex);

            if (isValueInt)
            {
                isEnumParsed = TryParseEnumByValue(enumIndex, defaultValue, out result);
            }

            return isEnumParsed;
        }

        /// <summary>
        /// Try to parse an enum by value
        /// </summary>
        /// <typeparam name="T">Should be an enum</typeparam>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseEnumByValue<T>(string value, out T result) where T : struct, IConvertible
        {
            return TryParseEnumByValue(value, default(T), out result);
        }

        /// <summary>
        /// Try to parse an enum by value
        /// </summary>
        /// <typeparam name="T">Should be an enum</typeparam>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseEnumByValue<T>(int value, out T result) where T : struct, IConvertible
        {
            return TryParseEnumByValue(value, default(T), out result);
        }

        /// <summary>
        /// Try to parse an enum by value
        /// </summary>
        /// <typeparam name="T">Should be an enum</typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseEnumByValue<T>(int value, T defaultValue, out T result) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

            bool isEnumParsed = false;

            result = defaultValue;

            try
            {
                if (Enum.IsDefined(typeof(T), value))
                {
                    result = (T)Enum.Parse(typeof(T), value.ToString());
                    isEnumParsed = true;
                }
            }
            catch
            {
            }

            return isEnumParsed;
        }

        /// <summary>
        /// Iterates an enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> IterateEnum<T>() where T : struct, IConvertible
        {
            Array enumNames = Enum.GetNames(typeof(T));
            List<T> enumList = new List<T>();

            for (int i = 0; i < enumNames.Length; i++)
            {
                T enumValue = (T)Enum.Parse(typeof(T), enumNames.GetValue(i).ToString());

                enumList.Add(enumValue);
            }

            return enumList;
        }

        public static bool IsCurrentEsns(EsnsType esns)
        {
            return !esns.Equals(EsnsType.WindowsLive) && !esns.Equals(EsnsType.LinkedIn) && !esns.Equals(EsnsType.Gmail) && !esns.Equals(EsnsType.Aol) && !esns.Equals(EsnsType.Yahoo) && !esns.Equals(EsnsType.MySpace);
        }

        public static bool IsCurrentReferrer(ReferrerPartnerType referrer)
        {
            return referrer.Equals(ReferrerPartnerType.Applifier) || referrer.Equals(ReferrerPartnerType.None);
        }

        public static bool IsCurrentChannel(ChannelType channel)
        {
            return !channel.Equals(ChannelType.OSXDashboard) && !channel.Equals(ChannelType.WebMySpace) && !channel.Equals(ChannelType.WindowsStandalone);
        }

        public static bool IsEpinProvider(PaymentProviderType paymentProvider)
        {
            return paymentProvider.Equals(PaymentProviderType.GameSultan) || paymentProvider.Equals(PaymentProviderType.Cmune);
        }
    }
}
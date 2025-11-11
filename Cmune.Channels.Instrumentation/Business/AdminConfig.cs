using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;

namespace Cmune.Channels.Instrumentation.Business
{
    public static class AdminConfig
    {
        #region Chart

        public const string ChartColorChannelAll = "#0078a4";
        public const string ChartColorChannelPortal = "#ffff00";
        public const string ChartColorChannelFacebook = "#0000ff";
        public const string ChartColorChannelMacAppStore = "#e65f05";
        public const string ChartColorChannelCyworld = "#000";
        public const string ChartColorChannelOSXStandalone = "#000";
        public const string ChartColorChannelWindowsStandalone = "#000";
        public const string ChartColorChannelKongregate = "#fb2424";
        public const string ChartColorChannelAndroid = "#000";

        private static readonly Dictionary<ChannelType, string> ChartColorChannels = new Dictionary<ChannelType, string> { 
            { ChannelType.WebPortal, ChartColorChannelPortal} ,
            { ChannelType.WebFacebook, ChartColorChannelFacebook} ,
            { ChannelType.MacAppStore, ChartColorChannelMacAppStore} ,
            { ChannelType.WebCyworld, ChartColorChannelCyworld} ,
            { ChannelType.OSXStandalone , ChartColorChannelOSXStandalone} ,
            { ChannelType.WindowsStandalone , ChartColorChannelWindowsStandalone} ,
            { ChannelType.Kongregate , ChartColorChannelKongregate} ,
            { ChannelType.Android , ChartColorChannelAndroid}
        };

        public static string GetChannelColor(ChannelType channel)
        {
            string color = String.Empty;

            if (!ChartColorChannels.TryGetValue(channel, out color))
            {
                CmuneLog.LogUnexpectedReturn(channel, String.Format("{0} has not a defined color yet", channel));
                color = "#000";
            }

            return color;
        }

        public const string ChartColorPaymentPlaySpan = "#FFBD16";
        public const string ChartColorPaymentSuperRewards = "#7BC90B";
        public const string ChartColorPaymentCyworld = "#0000FF";
        public const string ChartColorPaymentFacebook = "#B3B1AC";
        public const string ChartColorPaymentApple = "#000";
        public const string ChartColorPaymentKongregate = "#13BEEF";
        public const string ChartColorPaymentGameSultan = "#F26522";

        private static readonly Dictionary<PaymentProviderType, string> ChartColorPaymentProviders = new Dictionary<PaymentProviderType, string> { 
            { PaymentProviderType.PlaySpan, ChartColorPaymentPlaySpan} ,
            { PaymentProviderType.SuperRewards, ChartColorPaymentSuperRewards} ,
            { PaymentProviderType.Dotori, ChartColorPaymentCyworld} ,
            { PaymentProviderType.FacebookCredits, ChartColorPaymentFacebook} ,
            { PaymentProviderType.Apple, ChartColorPaymentApple} ,
            { PaymentProviderType.KongregateKreds, ChartColorPaymentKongregate} ,
            { PaymentProviderType.GameSultan, ChartColorPaymentGameSultan}
        };

        public static string GetPaymentProviderColor(PaymentProviderType paymentProvider)
        {
            string color = String.Empty;

            if (!ChartColorPaymentProviders.TryGetValue(paymentProvider, out color))
            {
                CmuneLog.LogUnexpectedReturn(paymentProvider, String.Format("{0} has not a defined color yet", paymentProvider));
                color = "#000";
            }

            return color;
        }

        public const string ChartToolTipBorderColor = "#000";
        public const string ChartToolTipColor = "#0078a4";

        public const string ChartColorCpuUsage = "#0000ff";
        public const string ChartColorRamUsage = "#E00B0B";
        public const string ChartColorDiskSpaceUsage = "#000000";
        public const string ChartColorBandwidthUsage = "#19AF08";

        #endregion

        public static readonly List<ChannelType> ActiveStatsChannels = new List<ChannelType> { ChannelType.WebPortal, ChannelType.WebFacebook, ChannelType.MacAppStore, ChannelType.Kongregate };
        public static readonly List<PaymentProviderType> ActiveStatsPaymentProviders = new List<PaymentProviderType> { PaymentProviderType.PlaySpan, PaymentProviderType.FacebookCredits, PaymentProviderType.SuperRewards, PaymentProviderType.Apple, PaymentProviderType.KongregateKreds, PaymentProviderType.GameSultan };
    }
}
using System.Web.Mvc;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Channels.Common.Helpers
{
    public static class BuyingDurationTypeConverterHelper 
    {
        //
        // GET: /BuyingTypeDurationConverterHelper/

        public static string ConvertBuyingDurationTypeToReadableString(this HtmlHelper html, BuyingDurationType type)
        {
            switch (type)
            {
                case BuyingDurationType.None:
                    return "No duration:";
                case BuyingDurationType.OneDay:
                    return "1 day";
                case BuyingDurationType.SevenDays:
                    return "1 week";
                case BuyingDurationType.ThirtyDays:
                    return "1 month";
                case BuyingDurationType.NinetyDays:
                    return "3 months";
                case BuyingDurationType.Permanent:
                    return "Permanent";
            }
            return "";
        }

    }
}

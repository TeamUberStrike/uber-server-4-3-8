using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.Channels.Utils;
using Cmune.DataCenter.Utils;

namespace UberStrike.Channels.Portal.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string Profile(this UrlHelper helper)
        {
            return helper.Content("~/Profile");
        }

        public static string Account(this UrlHelper helper)
        {
            return helper.Content("~/Account");
        }

        public static string AccountCompletion(this UrlHelper helper)
        {
            return helper.Content("~/AccountCompletion");
        }

        public static string CreditBundle(this UrlHelper helper)
        {
            return helper.Content("~/Bundle")+"?TransactionType="+ TransactionType.Credit;
        }

        public static string ItemAndPointBundle(this UrlHelper helper)
        {
            return helper.Content("~/Bundle") + "?TransactionType=" + TransactionType.ItemAndPoint;
        }

        public static string RedeemEpin(this UrlHelper helper)
        {
            return helper.Content("~/Epin");
        }

        public static string Forum(this UrlHelper helper)
        {
            return ConfigurationUtilities.ReadConfigurationManager("UberstrikeForumUrl");
        }

        public static string Home(this UrlHelper helper)
        {
            return helper.Content("~/Home");
        }

        public static string Play(this UrlHelper helper)
        {
            return helper.Content("~/Play");
        }

        public static string PlayAndRegistration(this UrlHelper helper)
        {
            return helper.Content("~/Play/Registration");
        }

        public static string Promotion(this UrlHelper helper)
        {
            return helper.Content("~/Promotion");
        }

        public static string Ranking(this UrlHelper helper)
        {
            return helper.Content("~/Ranking");
        }

        public static string Download(this UrlHelper helper)
        {
            return helper.Content("~/Download");
        }
    }
}
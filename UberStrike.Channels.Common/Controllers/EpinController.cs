using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Utils.Cryptography;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Utils;

namespace UberStrike.Channels.Common.Controllers
{
    public class EpinController : Controller
    {
        //
        // GET: /Epin/
        public ActionResult Index(string h, ChannelType ch, string gamePage, string epinPage, string faqPage, string lang)
        {
            ViewBag.ChannelType = ch;
            ViewBag.GamePage = gamePage;
            ViewBag.EpinPage = epinPage;
            ViewBag.FaqPage = faqPage;
            ViewBag.H = h;
            ViewBag.Lang = lang;

            // this doesn't scale well, alternative needed if we add more languages
            if (lang == "tr-TR")
            {
                return View("turkishIndex");
            }

            return View();
        }

        [HttpPost]
        public ActionResult ValidateEpin(string h, string pin, ChannelType ch, string gamePage, string epinPage, string faqPage, string lang)
        {
            string cmidFromHash = Crypto.fncRijndaelDecrypt(h, ConfigurationUtilities.ReadConfigurationManager("AuthenticationEncryptPassPhrase"), ConfigurationUtilities.ReadConfigurationManager("AuthenticationEncryptInitVector"));
            int cmid = 0;
            bool isRedemptionBlocked = false;
            bool isSuccess = false;
            int creditAmountAttributed = 0;
            var ip = TextUtilities.InetAToN(HttpContext.Request.UserHostAddress);

          

            if (Int32.TryParse(cmidFromHash, out cmid) && pin.Length < 30)
            {
                if (CmuneEpin.CanRedeem(ip, cmid))
                {
                    EpinTransactionResult epinResult = CmuneEpin.Redeem(CommonConfig.ApplicationIdUberstrike, cmid, pin, ch, out creditAmountAttributed);
                    if (epinResult == EpinTransactionResult.AlreadyRedeemed
                        || epinResult == EpinTransactionResult.InvalidApplication
                        || epinResult == EpinTransactionResult.InvalidData
                        || epinResult == EpinTransactionResult.InvalidPin
                        || epinResult == EpinTransactionResult.Retired
                        || epinResult == EpinTransactionResult.Error)
                    {
                        isSuccess = false;
                        CmuneEpin.RecordFailedAttempt(cmid, ip, pin);
                    }
                    else if (epinResult == EpinTransactionResult.Ok)
                    {
                        isSuccess = true;
                    }
                }
                else {
                    isRedemptionBlocked = true;
                }
            }

            ViewBag.PortalFaqPage = faqPage;
            ViewBag.PortalEpinPage = epinPage;
            ViewBag.PortalGamePage = gamePage;
            ViewBag.FacebookCanvasPage = ConfigurationUtilities.ReadConfigurationManager("FacebookCanvasPage");
            ViewBag.IsRedemptionBlocked = isRedemptionBlocked;
            ViewBag.IsEpinOperationSucessful = isSuccess;
            ViewBag.CreditAmountAttributed = creditAmountAttributed;
            ViewBag.ChannelType = ch;

            // this doesn't scale well, alternative needed if we add more languages
            if (lang == "tr-TR")
            {
                return View("turkishValidateEpin");
            }
            return View();
        }
    }
}

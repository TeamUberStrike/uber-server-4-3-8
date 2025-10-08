using System.Web.Mvc;
using UberStrike.Channels.Utils;

namespace UberStrike.Channels.Portal.Controllers
{
    public class BundleController : Controller
    {
        [Authorize]
        public ActionResult Index(TransactionType transactionType = TransactionType.Credit)
        {
            ViewBag.TransactionType = transactionType;
            return View();
        }
    }
}

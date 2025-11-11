using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.Common.Entities;
using System.Web.Security;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.DataCenter.DataAccess;
using UberStrike.DataCenter.DataAccess;
using Cmune.Channels.Instrumentation.Business;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.Channels.Instrumentation.FilterAttribute;
using Cmune.Instrumentation.DataAccess;
using Cmune.DataCenter.Common.Utils;


namespace Cmune.Channels.Instrumentation.Controllers
{
    public class BaseController : Controller
    {
        protected MembershipUser membershipUser;
        protected List<string>  membershipUserRoles { get; set; }
        protected string CmuneEnvironnement;
        protected string ApplicationPath;
        protected BuildType BuildType;

        public BaseController()
        {
            if (!EnumUtilities.TryParseEnumByName(ConfigurationUtilities.ReadConfigurationManager("Environment", true), out BuildType))
            {
                throw new ConfigurationErrorsException("Environment is not a BuildType type");
            }

            CmuneEnvironnement = ConfigurationUtilities.ReadConfigurationManager(CommonAppSettings.DatabaseDataSource, false).ToLower();
            string paradiseEnvironnement = ConfigurationUtilities.ReadConfigurationManager(UberstrikeAppSettings.DatabaseDataSource, false).ToLower();

            if (!CmuneEnvironnement.IsNullOrFullyEmpty() && !paradiseEnvironnement.IsNullOrFullyEmpty())
            {
                if (!CmuneEnvironnement.Equals(paradiseEnvironnement))
                    throw new ConfigurationErrorsException("This instrumentation is running on 2 different environnements: Cmune: " + CmuneEnvironnement + ", Paradise Paintball:" + paradiseEnvironnement);
            }

            ViewData["EnvironnementLiteral"] = CmuneEnvironnement;
            DateTime now = DateTime.Now;
            ViewData["ServerTimeLiteral"] = now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        protected void InitMembership()
        {
            if (Request.IsAuthenticated)
            {
                membershipUser = Membership.GetUser();
                membershipUserRoles = Roles.Provider.GetRolesForUser(membershipUser.UserName).ToList();
                ViewBag.MembershipUserRoles = membershipUserRoles;
                ViewBag.MembershipUser = membershipUser;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);

            string applicationPath = HttpContext.Request.Url.ToString().Substring(0, HttpContext.Request.Url.ToString().IndexOf('/', 7)) + HttpContext.Request.ApplicationPath + ((HttpContext.Request.ApplicationPath == "/") ? "" : "/");

            ApplicationPath = applicationPath;
            InitMembership();
        }

        protected override void OnActionExecuted(ActionExecutedContext ctx)
        {
            base.OnActionExecuted(ctx);
            // if they are null then set to String.Empty to avoid Null Exception in View
            ViewBag.ApplicationPath = ApplicationPath;
            ViewBag.ImagesRoot = ConfigurationUtilities.ReadConfigurationManager("ImagesRoot");
            ViewBag.IsActiveTabSet = ViewBag.ActiveTab != null;
            ViewBag.IsSubActiveTabSet = ViewBag.SubActiveTab != null;
            ViewBag.Environnement = CmuneEnvironnement;
            ViewBag.DatabaseInfo = ContextHelper<CmuneDataContext>.GetCurrentContext().Connection.ConnectionString;
            ViewBag.CookieDomainName = ConfigurationUtilities.ReadConfigurationManager("CookieDomainName");
            ViewBag.PublishVersion = ConfigurationUtilities.ReadConfigurationManager("PublishVersion");
        }

        protected bool IsAdministrator()
        {
            return membershipUserRoles.Contains(MembershipRoles.Administrator);
        }

        protected void UseProdDataContext()
        {
            ContextHelper<CmuneDataContext>.SetCurrentContext(new CmuneDataContext(ConfigurationUtilities.ReadConfigurationManager("CmuneStatsConnectionString")));
            ContextHelper<UberstrikeDataContext>.SetCurrentContext(new UberstrikeDataContext(ConfigurationUtilities.ReadConfigurationManager("UberStrikeConnectionString")));
            ContextHelper<InstrumentationDataContext>.SetCurrentContext(new InstrumentationDataContext(ConfigurationUtilities.ReadConfigurationManager("InstrumentationConnectionString")));
        }
    }
}

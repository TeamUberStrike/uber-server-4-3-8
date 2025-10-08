using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UberStrike.Channels.Utils;
using UberStrike.Channels.Kongregate.ActionFilters;

namespace UberStrike.Channels.Kongregate
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            var noFilterAccountActions = new List<string>() { "ConnectWith", "LogOut", "CreationError", "DeletedAccount" };
            var noFilterHomeActions = new List<string>() { "", "GetGuestPage", "Index", "Blank", "ProcessUnityInstallFlow", "StartSession", "SafariAllowCookie", "SafariRedirect" };

            filters.Add(new MyHandleErrorAttribute() { View = "Error" });
            var provider = new FilterProvider();
            provider.Add(c => (!(c.RouteData.Values["controller"].ToString() == "Account" && (noFilterAccountActions.Contains(c.RouteData.Values["action"])))
                && !(c.RouteData.Values["controller"].ToString() == "Home" && (noFilterHomeActions.Contains(c.RouteData.Values["action"])))
                && c.RouteData.Values["controller"].ToString() != "CacheInvalidation" && c.RouteData.Values["controller"].ToString() != "Error")
                ? new SessionActionFilter() : null);
            FilterProviders.Providers.Add(provider);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}
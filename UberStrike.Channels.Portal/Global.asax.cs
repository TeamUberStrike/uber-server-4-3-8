using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
//using Uberstrike.Core.Serialization;
using UberStrike.Channels.Portal.ActionFilters;
using System.Configuration;
using UberStrike.Channels.Utils;
using Cmune.DataCenter.Utils;

namespace UberStrike.Channels.Portal
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new MyHandleErrorAttribute() { View = "Error" });
            if (ConfigurationUtilities.ReadConfigurationManagerBool("IsPasswordProtected"))
            {
                var provider2 = new FilterProvider();
                provider2.Add(c => c.RouteData.Values["controller"].ToString() != "CacheInvalidation" ? new FirstAuthenticationFilter() : null);
                FilterProviders.Providers.Add(provider2);
            }
            filters.Add(new InitActionFilter());
            var provider = new FilterProvider();
            provider.Add(c => (c.RouteData.Values["action"].ToString() != "EmailDuplication" && c.RouteData.Values["action"].ToString() != "LinkFacebookAccount") ? new SessionActionFilter() : null);
            FilterProviders.Providers.Add(provider);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "MailingListSuccess",
                "MailingListSuccess.aspx",
                new { controller = "Promotion", action = "MailingListSuccess" }
                );

            routes.MapRoute(
              "MailingListFailure",
              "MailingListFailure.aspx",
              new { controller = "Promotion", action = "MailingListFailure" }
              );

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
           // UberstrikeSerialization.InitSerializationProxies();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UberStrike.Channels.Facebook.FilterAttributes;
using Cmune.DataCenter.Utils;

namespace UberStrike.Channels.Facebook
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ActionAttribute());
            filters.Add(new InitActionFilter());
            filters.Add(new MyHandleErrorAttribute() { View = "Error" });
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Message",
                "Message/{action}",
                new { controller = "Message", action = "InviteSent" }
            );

            routes.MapRoute("Facebook",
                "facebook/{thing1}/{thing2}",
                new { controller = "Error", action = "Reload", thing1 = UrlParameter.Optional, thing2 = UrlParameter.Optional }
            );
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

       
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}
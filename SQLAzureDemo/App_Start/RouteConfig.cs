using System.Web.Mvc;
using System.Web.Routing;

namespace SQLAzureDemo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Throw", "Errors/Throw", new {controller = "Errors", action = "Throw"});

            routes.MapRoute("Errors", "Errors/{resource}/{subResource}",
                new { controller = "Errors", action = "Index", resource = UrlParameter.Optional, subResource = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
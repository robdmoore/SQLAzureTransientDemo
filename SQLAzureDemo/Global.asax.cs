using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac.Integration.Mvc;
using SQLAzureDemo.App_Start;
using SQLAzureDemo.Database.Migrations;
using Serilog;

namespace SQLAzureDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var container = AutofacConfig.BuildContainer(connectionString);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/log.txt"))
                .CreateLogger();
            Migrate.Database(connectionString);
        }
    }
}
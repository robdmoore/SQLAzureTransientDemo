using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac.Integration.Mvc;
using Microsoft.WindowsAzure.Storage;
using SQLAzureDemo.App_Start;
using SQLAzureDemo.App_Start.Serilog;
using SQLAzureDemo.Database.Migrations;
using Serilog;
using StackExchange.Exceptional;
using StackExchange.Exceptional.Stores;

namespace SQLAzureDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
            var azureStorage = ConfigurationManager.ConnectionStrings["AzureStorage"].ConnectionString;
            ErrorStore.Setup("SQLAzureDemo", new SQLErrorStore(connectionString));
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var container = AutofacConfig.BuildContainer(connectionString);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            Log.Logger = new LoggerConfiguration()
                .WriteTo.AzureTable(CloudStorageAccount.Parse(azureStorage))
                .CreateLogger();
            Migrate.Database(connectionString);
        }
    }
}
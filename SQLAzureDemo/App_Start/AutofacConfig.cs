using Autofac;
using Microsoft.WindowsAzure.Storage;
using SQLAzureDemo.App_Start.Autofac;
using Autofac.Integration.Mvc;

namespace SQLAzureDemo.App_Start
{
    public static class AutofacConfig
    {
        public static IContainer BuildContainer(string connectionString, CloudStorageAccount azureStorage)
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterModule(new NHibernateModule(connectionString, azureStorage));
            builder.RegisterModule(new ServicesModule(azureStorage));
            builder.RegisterModule<RepositoryModule>();

            return builder.Build();
        }
    }
}
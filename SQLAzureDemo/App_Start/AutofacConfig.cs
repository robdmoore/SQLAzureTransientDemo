using Autofac;
using SQLAzureDemo.App_Start.Autofac;
using Autofac.Integration.Mvc;

namespace SQLAzureDemo.App_Start
{
    public static class AutofacConfig
    {
        public static IContainer BuildContainer(string connectionString)
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterModule(new NHibernateModule(connectionString));
            builder.RegisterModule<RepositoryModule>();

            return builder.Build();
        }
    }
}
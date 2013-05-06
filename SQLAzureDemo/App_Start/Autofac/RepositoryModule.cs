using Autofac;
using SQLAzureDemo.Database.Repositories;
using Autofac.Integration.Mvc;

namespace SQLAzureDemo.App_Start.Autofac
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MovieRepository>()
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();
        }
    }
}
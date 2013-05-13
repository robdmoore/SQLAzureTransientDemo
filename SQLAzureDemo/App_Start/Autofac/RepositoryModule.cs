using Autofac;
using Autofac.Integration.Mvc;
using NHibernate;
using SQLAzureDemo.Database.Repositories;

namespace SQLAzureDemo.App_Start.Autofac
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new MovieRepository(c.ResolveKeyed<ISession>(NHibernateModule.TransientConnection)))
                .Keyed<IMovieRepository>(NHibernateModule.TransientConnection)
                .InstancePerHttpRequest();

            builder.Register(c => new MovieRepository(c.ResolveKeyed<ISession>(NHibernateModule.ResilientConnection)))
                .Keyed<IMovieRepository>(NHibernateModule.ResilientConnection)
                .InstancePerHttpRequest();
        }
    }
}
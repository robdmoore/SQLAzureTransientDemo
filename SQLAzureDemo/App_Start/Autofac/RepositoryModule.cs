using Autofac;
using Autofac.Integration.Mvc;
using NHibernate;
using SQLAzureDemo.App_Start.EntityFramework;
using SQLAzureDemo.Database.Repositories;

namespace SQLAzureDemo.App_Start.Autofac
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Todo: rename the key constant strings so the repository key registrations don't conflict

            builder.Register(c => new NHibernateMovieRepository(c.ResolveKeyed<ISession>(NHibernateModule.TransientConnection)))
                .Keyed<IMovieRepository>(NHibernateModule.TransientConnection)
                .InstancePerHttpRequest();

            builder.Register(c => new NHibernateMovieRepository(c.ResolveKeyed<ISession>(NHibernateModule.ResilientConnection)))
                .Keyed<IMovieRepository>(NHibernateModule.ResilientConnection)
                .InstancePerHttpRequest();

            builder.Register(c => new EntityFrameworkMovieRepository(c.ResolveKeyed<IModelContext>(EntityFrameworkModule.TransientConnection)))
                .Keyed<IMovieRepository>(EntityFrameworkModule.TransientConnection)
                .InstancePerHttpRequest();

            builder.Register(c => new EntityFrameworkMovieRepository(c.ResolveKeyed<IModelContext>(EntityFrameworkModule.ResilientConnection)))
                .Keyed<IMovieRepository>(EntityFrameworkModule.ResilientConnection)
                .InstancePerHttpRequest();
        }
    }
}
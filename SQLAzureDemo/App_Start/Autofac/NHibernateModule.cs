using Autofac;
using NHibernate;
using NHibernate.Driver;
using SQLAzureDemo.App_Start.NHibernate;
using Autofac.Integration.Mvc;

namespace SQLAzureDemo.App_Start.Autofac
{
    public class NHibernateModule : Module
    {
        private readonly string _connectionString;

        public NHibernateModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new NHibernateConfiguration<Sql2008ClientDriver>(_connectionString).GetSessionFactory())
                .As<ISessionFactory>()
                .SingleInstance();

            builder.Register(c => c.Resolve<ISessionFactory>().OpenSession())
                .As<ISession>()
                .InstancePerHttpRequest();
        }
    }
}
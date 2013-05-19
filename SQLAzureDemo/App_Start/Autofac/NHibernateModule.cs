using System;
using Autofac;
using Microsoft.Practices.TransientFaultHandling;
using NHibernate;
using NHibernate.Driver;
using NHibernate.SqlAzure;
using SQLAzureDemo.App_Start.NHibernate;
using Autofac.Integration.Mvc;
using Serilog;

namespace SQLAzureDemo.App_Start.Autofac
{
    public class NHibernateModule : Module
    {
        public const string TransientConnection = "transient";
        public const string ResilientConnection = "resilient";

        private readonly string _connectionString;

        public NHibernateModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            Register<Sql2008ClientDriver>(builder, TransientConnection);
            Register<LoggingSqlAzureClientDriverWithTimeoutRetries>(builder, ResilientConnection);
        }

        private void Register<TDriver>(ContainerBuilder builder, string key)
            where TDriver: IDriver
        {
            builder.Register(c => new NHibernateConfiguration<TDriver>(_connectionString).GetSessionFactory())
                .Keyed<ISessionFactory>(key)
                .SingleInstance();

            builder.Register(c => c.ResolveKeyed<ISessionFactory>(key).OpenSession())
                .Keyed<ISession>(key)
                .InstancePerHttpRequest();
        }
    }

    public class LoggingSqlAzureClientDriverWithTimeoutRetries : SqlAzureClientDriverWithTimeoutRetries
    {
        protected override EventHandler<RetryingEventArgs> RetryEventHandler()
        {
            return (sender, args) => Log.Logger.Warning(args.LastException,
                "SQLAzureClientDriver Retry - Count:{0}, Delay:{1}, Exception:{2}",
                args.CurrentRetryCount,
                args.Delay,
                args.LastException
            );
        }
    }
}
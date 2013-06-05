using System;
using Autofac;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.SqlAzure;
using Microsoft.Practices.TransientFaultHandling;
using Microsoft.WindowsAzure.Storage;
using NHibernate;
using SQLAzureDemo.App_Start.EntityFramework;
using SQLAzureDemo.App_Start.NHibernate;
using Autofac.Integration.Mvc;

namespace SQLAzureDemo.App_Start.Autofac
{
    public class EntityFrameworkModule : Module
    {
        public const string TransientConnection = "transient";
        public const string ResilientConnection = "resilient";

        private readonly string _connectionString;

        public EntityFrameworkModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new ModelContext(_connectionString))
                .Keyed<IModelContext>(TransientConnection)
                .InstancePerHttpRequest();

            const string incremental = "Incremental Retry Strategy";
            const string backoff = "Backoff Retry Strategy";
            var connectionRetry = new ExponentialBackoff(backoff, 10, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(10), false);
            var commandRetry = new Incremental(incremental, 10, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            var connection = new ReliableSqlConnection(_connectionString,
                new RetryPolicy<SqlAzureTransientErrorDetectionStrategy>(connectionRetry),
                new RetryPolicy<SqlAzureTransientErrorDetectionStrategy>(commandRetry)
            );
            builder.Register(c => new ModelContext(connection))
                .Keyed<IModelContext>(ResilientConnection)
                .InstancePerHttpRequest();
        }
    }
}
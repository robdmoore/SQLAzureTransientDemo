using System;
using System.Data.Common;
using Autofac;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.SqlAzure;
using Microsoft.Practices.TransientFaultHandling;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NHibernate.SqlAzure;
using NHibernate.SqlAzure.RetryStrategies;
using SQLAzureDemo.App_Start.EntityFramework;
using Autofac.Integration.Mvc;
using SQLAzureDemo.App_Start.NHibernate;

namespace SQLAzureDemo.App_Start.Autofac
{
    public class EntityFrameworkModule : Module
    {
        public const string TransientConnection = "eftransient";
        public const string ResilientConnection = "efresilient";

        private readonly string _connectionString;
        private readonly CloudTable _table;

        public EntityFrameworkModule(string connectionString, CloudStorageAccount storageAccount)
        {
            _connectionString = connectionString;

            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(typeof(TransientRetry).Name);
            _table.CreateIfNotExists();

            System.Data.Entity.Database.SetInitializer(new DontPerformCodeFirstMigrations());
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new ModelContext(_connectionString))
                .Keyed<IModelContext>(TransientConnection)
                .InstancePerHttpRequest();

            builder.Register(c => new ModelContext(GetReliableConnection()))
                .Keyed<IModelContext>(ResilientConnection)
                .InstancePerHttpRequest();
        }

        private DbConnection GetReliableConnection()
        {
            var connectionRetry = new ExponentialBackoff("Backoff Retry Strategy", 10, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(10), false);
            var commandRetry = new Incremental("Incremental Retry Strategy", 10, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            var connection = new ReliableSqlConnection(_connectionString,
                new RetryPolicy<SqlAzureTransientErrorDetectionStrategyWithTimeouts>(connectionRetry),
                new RetryPolicy<SqlAzureTransientErrorDetectionStrategyWithTimeouts>(commandRetry)
            );

            connection.CommandRetryPolicy.Retrying += (e, args) => _table.Execute(TableOperation.Insert(new TransientRetry(args)));
            connection.ConnectionRetryPolicy.Retrying += (e, args) => _table.Execute(TableOperation.Insert(new TransientRetry(args)));

            return new ReliableSqlDbConnection(connection);
        }
    }
}
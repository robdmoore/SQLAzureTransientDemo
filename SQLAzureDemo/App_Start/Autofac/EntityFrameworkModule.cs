using System.Data.SqlClient;
using Autofac;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SQLAzureDemo.App_Start.EntityFramework;
using Autofac.Integration.Mvc;
using SQLAzureDemo.App_Start.NHibernate;
using Module = Autofac.Module;

namespace SQLAzureDemo.App_Start.Autofac
{
    public class EntityFrameworkModule : Module
    {
        public const string TransientConnection = "eftransient";
        public const string ResilientConnection = "efresilient";
        public const string ReliableConnectionStringName = "ReliableDatabase";

        private readonly string _connectionString;

        public EntityFrameworkModule(string connectionString, CloudStorageAccount storageAccount)
        {
            _connectionString = connectionString;

            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(typeof(TransientRetry).Name);
            table.CreateIfNotExists();
            ReliableSqlClientProvider.CommandRetry += (sender, args) => table.Execute(TableOperation.Insert(new TransientRetry(args)));
            ReliableSqlClientProvider.ConnectionRetry += (sender, args) => table.Execute(TableOperation.Insert(new TransientRetry(args)));

            System.Data.Entity.Database.SetInitializer(new DontPerformCodeFirstMigrations());
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new ModelContext(_connectionString))
                .Keyed<IModelContext>(TransientConnection)
                .InstancePerHttpRequest();

            builder.Register(c => new ModelContext(ReliableConnectionStringName))
                .Keyed<IModelContext>(ResilientConnection)
                .InstancePerHttpRequest();
        }
    }
}
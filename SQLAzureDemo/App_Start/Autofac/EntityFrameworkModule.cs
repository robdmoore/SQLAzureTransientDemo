using System.Data.SqlClient;
using Autofac;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ReliableDbProvider;
using ReliableDbProvider.SqlAzureWithTimeoutRetries;
using SQLAzureDemo.App_Start.EntityFramework;
using Autofac.Integration.Mvc;
using SQLAzureDemo.App_Start.NHibernate;
using Module = Autofac.Module;

namespace SQLAzureDemo.App_Start.Autofac
{
    public class EntityFrameworkModule : Module
    {
        private readonly string _connectionString;
        public const string TransientConnection = "eftransient";
        public const string ResilientConnection = "efresilient";

        public EntityFrameworkModule(string connectionString, CloudStorageAccount storageAccount)
        {
            _connectionString = connectionString;
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(typeof(TransientRetry).Name);
            table.CreateIfNotExists();
            SqlAzureProvider.CommandRetry += (sender, args) => table.Execute(TableOperation.Insert(new TransientRetry(args)));
            SqlAzureProvider.ConnectionRetry += (sender, args) => table.Execute(TableOperation.Insert(new TransientRetry(args)));

            System.Data.Entity.Database.SetInitializer(new DontPerformCodeFirstMigrations());
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new ModelContext(_connectionString))
                .Keyed<IModelContext>(TransientConnection)
                .InstancePerHttpRequest();

            builder.Register(c => {
                    var reliableConnection = SqlAzureProvider.Instance.CreateConnection();
                    reliableConnection.ConnectionString = _connectionString;
                    return new ModelContext(reliableConnection);
                })
                .Keyed<IModelContext>(ResilientConnection)
                .InstancePerHttpRequest();
        }
    }
}
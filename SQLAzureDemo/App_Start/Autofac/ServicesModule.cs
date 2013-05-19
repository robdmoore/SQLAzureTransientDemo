using Autofac;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SQLAzureDemo.Controllers.Services;

namespace SQLAzureDemo.App_Start.Autofac
{
    public class ServicesModule : Module
    {
        private readonly CloudTable _table;

        public ServicesModule(CloudStorageAccount storageAccount)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(typeof(ControllerOperation).Name);
            _table.CreateIfNotExists();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ControllerOperationLogger>()
                .WithParameter(new TypedParameter(typeof(CloudTable), _table))
                .AsImplementedInterfaces()
                .InstancePerDependency();

            builder.RegisterType<ControllerOperationStatisticsService>()
                .WithParameter(new TypedParameter(typeof(CloudTable), _table))
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
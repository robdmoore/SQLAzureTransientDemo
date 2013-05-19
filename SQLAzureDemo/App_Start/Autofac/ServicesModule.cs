using System;
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
        }
    }

    public class ControllerOperation : TableEntity
    {
        private static readonly long TicksInOneMinute = TimeSpan.FromMinutes(1).Ticks;

        public ControllerOperation() {}

        public ControllerOperation(string type, string url, bool failed)
        {
            OperationFailed = failed;
            Url = url;
            OperationType = type;
            RowKey = type;
            PartitionKey = (DateTime.UtcNow.Ticks % TicksInOneMinute).ToString();
        }

        public bool OperationFailed { get; set; }
        public string OperationType { get; set; }
        public string Url { get; set; }
    }
}
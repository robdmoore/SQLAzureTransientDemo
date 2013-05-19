using System;
using Microsoft.Practices.TransientFaultHandling;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NHibernate.SqlAzure;

namespace SQLAzureDemo.App_Start.NHibernate
{
    public class LoggingSqlAzureClientDriverWithTimeoutRetries : SqlAzureClientDriverWithTimeoutRetries
    {
        private static CloudTable _table;
        public static void Initialise(CloudStorageAccount storageAccount)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(typeof(TransientRetry).Name);
            _table.CreateIfNotExists();
        }

        protected override EventHandler<RetryingEventArgs> RetryEventHandler()
        {
            return (sender, args) => _table.Execute(TableOperation.Insert(new TransientRetry(args)));
        }
    }
}
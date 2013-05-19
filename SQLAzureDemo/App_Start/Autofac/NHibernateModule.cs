using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Autofac;
using Microsoft.Practices.TransientFaultHandling;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NHibernate;
using NHibernate.Driver;
using NHibernate.SqlAzure;
using SQLAzureDemo.App_Start.NHibernate;
using Autofac.Integration.Mvc;
using SQLAzureDemo.App_Start.Serilog;

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

        public class TransientRetry : TableEntity
        {
            public TransientRetry() {}

            public TransientRetry(RetryingEventArgs eventArgs)
            {
                var lastException = eventArgs.LastException;

                Delay = eventArgs.Delay.ToString();
                RetryCount = eventArgs.CurrentRetryCount;
                Exception = lastException.TraceInformation();
                ExceptionMessage = lastException.Message;
                
                Url = HttpContext.Current.Request.RawUrl;
                Server = Environment.MachineName;

                var sqlException = lastException as SqlException;
                if (sqlException != null)
                    SqlErrorNumbers = string.Join(",", sqlException.Errors.Cast<SqlError>().Select(e => e.Number));

                PartitionKey = string.Format("0{0}", DateTime.UtcNow.Ticks);
                RowKey = string.Format("{0}|{1}", SqlErrorNumbers, RetryCount);
            }

            public string SqlErrorNumbers { get; set; }
            public string Delay { get; set; }
            public int RetryCount { get; set; }
            public string ExceptionMessage { get; set; }
            public string Exception { get; set; }
            public string Url { get; set; }
            public string Server { get; set; }
        }
    }
}
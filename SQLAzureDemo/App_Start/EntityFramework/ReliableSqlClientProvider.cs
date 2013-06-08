using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.SqlAzure;
using Microsoft.Practices.TransientFaultHandling;
using NHibernate.SqlAzure.RetryStrategies;

namespace SQLAzureDemo.App_Start.EntityFramework
{
    public class ReliableSqlClientProvider : DbProviderFactory, IServiceProvider
    {
        public static readonly ReliableSqlClientProvider Instance = new ReliableSqlClientProvider();

        public override DbConnection CreateConnection()
        {
            var connectionRetry = new ExponentialBackoff("Backoff Retry Strategy", 10, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(10), false);
            var commandRetry = new Incremental("Incremental Retry Strategy", 10, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            var connection = new ReliableSqlConnection("",
                new RetryPolicy<SqlAzureTransientErrorDetectionStrategyWithTimeouts>(connectionRetry),
                new RetryPolicy<SqlAzureTransientErrorDetectionStrategyWithTimeouts>(commandRetry)
            );

            connection.CommandRetryPolicy.Retrying += CommandRetry;
            connection.ConnectionRetryPolicy.Retrying += ConnectionRetry;

            return new ReliableSqlDbConnection(connection);
        }

        public object GetService(Type serviceType)
        {
            return serviceType == typeof(DbProviderServices)
                ? ReliableDbProviderServices.Instance
                : null;
        }

        public override DbCommand CreateCommand()
        {
            // If I return null then EF grabs the command using the connection, which allows for it to be wrapped
            return null;
        }

        public static event EventHandler<RetryingEventArgs> CommandRetry;
        public static event EventHandler<RetryingEventArgs> ConnectionRetry;

        #region NotSupported

        public override bool CanCreateDataSourceEnumerator
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override DbCommandBuilder CreateCommandBuilder()
        {
            throw new NotSupportedException();
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            throw new NotSupportedException();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            throw new NotSupportedException();
        }

        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            throw new NotSupportedException();
        }

        public override DbParameter CreateParameter()
        {
            throw new NotSupportedException();
        }

        public override CodeAccessPermission CreatePermission(PermissionState state)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
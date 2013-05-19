using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.Practices.TransientFaultHandling;
using Microsoft.WindowsAzure.Storage.Table;
using SQLAzureDemo.App_Start.Serilog;

namespace SQLAzureDemo.App_Start.NHibernate
{
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
                
            Url = HttpContext.Current.Request.Url.ToString();
            Server = Environment.MachineName;

            var sqlException = lastException as SqlException;
            if (sqlException != null)
                SqlErrorNumbers = String.Join(",", sqlException.Errors.Cast<SqlError>().Select(e => e.Number));

            PartitionKey = String.Format("0{0}", DateTime.UtcNow.Ticks);
            RowKey = String.Format("{0}|{1}", SqlErrorNumbers, RetryCount);
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
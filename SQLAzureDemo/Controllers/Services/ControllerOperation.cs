using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace SQLAzureDemo.Controllers.Services
{
    public class ControllerOperation : TableEntity
    {
        public const string Transient = "transient";
        public const string Resilient = "resilient";
        public const string NHibernate = "nhibernate";
        public const string EntityFramework = "entityframework";

        public ControllerOperation() {}

        public ControllerOperation(string type, string framework, string url, bool failed, TimeSpan elapsed)
        {
            SecondsElapsed = elapsed.TotalSeconds;
            OperationFailed = failed;
            Url = url;
            OperationType = type;
            Framework = framework;
            RowKey = string.Format("{0}|{1:D10}", type, elapsed.Ticks);
            PartitionKey = string.Format("0{0}", DateTime.UtcNow.Ticks);
        }

        public bool OperationFailed { get; set; }
        public string OperationType { get; set; }
        public string Url { get; set; }
        public double SecondsElapsed { get; set; }
        public string Framework { get; set; }
    }
}
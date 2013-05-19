using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace SQLAzureDemo.Controllers.Services
{
    public class ControllerOperation : TableEntity
    {
        public static readonly long TicksInOneMinute = TimeSpan.FromMinutes(1).Ticks;

        public ControllerOperation() {}

        public ControllerOperation(string type, string url, bool failed)
        {
            OperationFailed = failed;
            Url = url;
            OperationType = type;
            RowKey = type;
            PartitionKey = (DateTime.UtcNow.Ticks / TicksInOneMinute).ToString();
        }

        public bool OperationFailed { get; set; }
        public string OperationType { get; set; }
        public string Url { get; set; }
    }
}
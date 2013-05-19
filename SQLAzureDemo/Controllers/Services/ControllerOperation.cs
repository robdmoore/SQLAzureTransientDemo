using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace SQLAzureDemo.Controllers.Services
{
    public class ControllerOperation : TableEntity
    {
        public ControllerOperation() {}

        public ControllerOperation(string type, string url, bool failed)
        {
            OperationFailed = failed;
            Url = url;
            OperationType = type;
            RowKey = type;
            PartitionKey = string.Format("0{0}", DateTime.UtcNow.Ticks);
        }

        public bool OperationFailed { get; set; }
        public string OperationType { get; set; }
        public string Url { get; set; }
    }
}
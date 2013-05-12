using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace SQLAzureDemo.App_Start.Serilog
{
    public class AzureTableStorageSink : ILogEventSink
    {
        private readonly CloudTable _table;

        public AzureTableStorageSink(CloudStorageAccount storageAccount)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(typeof(LogEventEntity).Name);
            _table.CreateIfNotExists();
        }

        public void Emit(LogEvent logEvent)
        {
            // todo: Use batch insert operation via timer like the Mongo and Couch sinks
            _table.Execute(TableOperation.Insert(new LogEventEntity(logEvent)));
        }
    }

    public class LogEventEntity : TableEntity
    {
        public LogEventEntity() {}

        public LogEventEntity(LogEvent log)
        {
            Timestamp = log.TimeStamp.ToUniversalTime().DateTime;
            PartitionKey = string.Format("0{0}", Timestamp.Ticks);
            RowKey = string.Format("{0}|{1}", log.Level, log.MessageTemplate.Text);
            MessageTemplate = log.MessageTemplate.Text;
            Level = log.Level.ToString();
            Exception = log.Exception.TraceInformation();
            RenderedMessage = log.RenderedMessage;
            var s = new StringWriter();
            new SimpleJsonFormatter().Format(log, s);
            Data = s.ToString();
        }

        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public string Exception { get; set; }
        public string RenderedMessage { get; set; }
        public string Data { get; set; }
    }
}
using System;
using Microsoft.WindowsAzure.Storage;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace SQLAzureDemo.App_Start.Serilog
{
    public static class LoggerConfigurationAzureTableStorageExtensions
    {
        public static LoggerConfiguration AzureTable(this LoggerSinkConfiguration loggerConfiguration,
            CloudStorageAccount storageAccount, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Minimum)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (storageAccount == null) throw new ArgumentNullException("storageAccount");
            return loggerConfiguration.Sink(new AzureTableStorageSink(storageAccount), restrictedToMinimumLevel);
        }
    }
}
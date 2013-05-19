using System;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

namespace SQLAzureDemo.Controllers.Services
{
    public interface IControllerOperationStatisticsService
    {
        OperationStatistics GetStatistics();
    }

    public class OperationStatistics
    {
        public int TotalResilientRequests { get; set; }
        public int FailedResilientRequests { get; set; }
        public int TotalTransientRequests { get; set; }
        public int FailedTransientRequests { get; set; }
    }

    public class ControllerOperationStatisticsService : IControllerOperationStatisticsService
    {
        private readonly CloudTable _table;

        public ControllerOperationStatisticsService(CloudTable table)
        {
            _table = table;
        }

        public OperationStatistics GetStatistics()
        {
            var fiveMinsAgo = DateTime.UtcNow.AddMinutes(-5).Ticks / ControllerOperation.TicksInOneMinute;
            var stats = _table.ExecuteQuery(
                new TableQuery<ControllerOperation>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, fiveMinsAgo.ToString()))
            ).ToList();

            return new OperationStatistics
            {
                TotalResilientRequests = stats.Count(s => s.OperationType == "resilient"),
                FailedResilientRequests = stats.Count(s => s.OperationType == "resilient" && s.OperationFailed),
                TotalTransientRequests = stats.Count(s => s.OperationType == "transient"),
                FailedTransientRequests = stats.Count(s => s.OperationType == "transient" && s.OperationFailed)
            };
        }
    }
}
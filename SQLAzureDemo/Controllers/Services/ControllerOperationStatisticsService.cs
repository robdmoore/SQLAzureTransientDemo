using System;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

namespace SQLAzureDemo.Controllers.Services
{
    public interface IControllerOperationStatisticsService
    {
        OperationStatistics GetStatistics(string framework);
    }

    public class OperationStatistics
    {
        public int TotalResilientRequests { get; set; }
        public int FailedResilientRequests { get; set; }
        public double AverageResilientRequests { get; set; }
        public int TotalTransientRequests { get; set; }
        public int FailedTransientRequests { get; set; }
        public double AverageTransientRequests { get; set; }
    }

    public class ControllerOperationStatisticsService : IControllerOperationStatisticsService
    {
        private readonly CloudTable _table;

        public ControllerOperationStatisticsService(CloudTable table)
        {
            _table = table;
        }

        public OperationStatistics GetStatistics(string framework)
        {
            var fiveMinsAgo = DateTime.UtcNow.AddMinutes(-5).Ticks;
            var stats = _table.ExecuteQuery(
                new TableQuery<ControllerOperation>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, string.Format("0{0}", fiveMinsAgo)))
            ).ToList();

            var successfulResilient = stats.Where(s => s.OperationType == ControllerOperation.Resilient && s.Framework == framework && !s.OperationFailed).ToList();
            var successfulTransient = stats.Where(s => s.OperationType == ControllerOperation.Transient && s.Framework == framework && !s.OperationFailed).ToList();

            return new OperationStatistics
            {
                TotalResilientRequests = stats.Count(s => s.OperationType == ControllerOperation.Resilient && s.Framework == framework),
                FailedResilientRequests = stats.Count(s => s.OperationType == ControllerOperation.Resilient && s.Framework == framework && s.OperationFailed),
                AverageResilientRequests = successfulResilient.Any() ? successfulResilient.Average(s => s.SecondsElapsed) : 0,
                TotalTransientRequests = stats.Count(s => s.OperationType == ControllerOperation.Transient && s.Framework == framework),
                FailedTransientRequests = stats.Count(s => s.OperationType == ControllerOperation.Transient && s.Framework == framework && s.OperationFailed),
                AverageTransientRequests = successfulTransient.Any() ? successfulTransient.Average(s => s.SecondsElapsed) : 0,
            };
        }
    }
}
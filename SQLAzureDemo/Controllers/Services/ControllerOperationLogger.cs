using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace SQLAzureDemo.Controllers.Services
{
    public interface IControllerOperationLogger : IDisposable {}

    public class ControllerOperationLogger : IControllerOperationLogger
    {
        private readonly CloudTable _table;
        private readonly Stopwatch _stopwatch;

        public ControllerOperationLogger(CloudTable table)
        {
            _table = table;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _table.Execute(TableOperation.Insert(new ControllerOperation(
                HttpContext.Current.Request.RawUrl.ToLower().Contains("transient") ? ControllerOperation.Transient : ControllerOperation.Resilient,
                HttpContext.Current.Request.RawUrl.ToLower().Contains("nhibernate") ? ControllerOperation.NHibernate : ControllerOperation.EntityFramework,
                HttpContext.Current.Request.Url.ToString(),
                OperationFailed(),
                _stopwatch.Elapsed
            )));
        }

        // http://stackoverflow.com/questions/2830073/detecting-a-dispose-from-an-exception-inside-using-block
        private static bool OperationFailed()
        {
            return Marshal.GetExceptionPointers() != IntPtr.Zero
                || Marshal.GetExceptionCode() != 0;
        }
    }
}
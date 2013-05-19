using System;
using System.Runtime.InteropServices;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace SQLAzureDemo.Controllers.Services
{
    public interface IControllerOperationLogger : IDisposable {}

    public class ControllerOperationLogger : IControllerOperationLogger
    {
        private readonly CloudTable _table;

        public ControllerOperationLogger(CloudTable table)
        {
            _table = table;
        }

        public void Dispose()
        {
            _table.Execute(TableOperation.Insert(new ControllerOperation(
                HttpContext.Current.Request.RawUrl.ToLower().Contains("transient") ? "transient" : "resilient",
                HttpContext.Current.Request.Url.ToString(),
                OperationFailed()
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
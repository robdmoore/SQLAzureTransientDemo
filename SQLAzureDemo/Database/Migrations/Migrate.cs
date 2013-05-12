using System.Reflection;
using DbUp;
using DbUp.Engine.Output;
using Serilog;

namespace SQLAzureDemo.Database.Migrations
{
    public static class Migrate
    {
        public static void Database(string connectionString)
        {
            var result = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsAndCodeEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogTo(new SerilogUpgradeLog())
                .Build()
                .PerformUpgrade();

            if (!result.Successful)
                throw result.Error;
        }

        class SerilogUpgradeLog : IUpgradeLog
        {
            public void WriteInformation(string format, params object[] args)
            {
                Log.Logger.Information(format, args);
            }

            public void WriteError(string format, params object[] args)
            {
                Log.Logger.Error(format, args);
            }

            public void WriteWarning(string format, params object[] args)
            {
                Log.Logger.Warning(format, args);
            }
        }
    }
}
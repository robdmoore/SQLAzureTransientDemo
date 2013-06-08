using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.SqlAzure;

namespace SQLAzureDemo.App_Start.EntityFramework
{
    public class ReliableSqlDbConnection : global::NHibernate.SqlAzure.ReliableSqlDbConnection
    {
        public ReliableSqlDbConnection(ReliableSqlConnection connection) : base(connection) {}

        protected override DbProviderFactory DbProviderFactory
        {
            get
            {
                return ReliableSqlClientProvider.Instance;
            }
        }
    }
}
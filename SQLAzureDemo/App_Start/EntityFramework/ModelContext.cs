using System.Data.Entity;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.SqlAzure;
using SQLAzureDemo.Database.Models;

namespace SQLAzureDemo.App_Start.EntityFramework
{
    public interface IModelContext
    {
        DbSet<Movie> Movies { get; set; }
    }

    public class ModelContext : DbContext, IModelContext
    {
        public ModelContext(string connectionString) : base(connectionString) { }

        public ModelContext(ReliableSqlConnection reliableSqlConnection) : base(reliableSqlConnection.Current, true) { }

        public DbSet<Movie> Movies { get; set; }
    }
}
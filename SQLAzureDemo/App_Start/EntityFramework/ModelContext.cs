using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<Movie> Movies { get; set; }
    }

    public class DontPerformCodeFirstMigrations : IDatabaseInitializer<ModelContext>
    {
        public void InitializeDatabase(ModelContext context) {}
    }
}
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
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
        public ModelContext(DbConnection dbConnection) : base(dbConnection, true) { }

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
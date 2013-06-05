using System.Linq;
using SQLAzureDemo.App_Start.EntityFramework;
using SQLAzureDemo.Database.Models;

namespace SQLAzureDemo.Database.Repositories
{
    public class EntityFrameworkMovieRepository : MovieRepository
    {
        private readonly IModelContext _modelContext;

        public EntityFrameworkMovieRepository(IModelContext modelContext)
        {
            _modelContext = modelContext;
        }

        protected override IQueryable<Movie> MovieAsQueryable()
        {
            return _modelContext.Movies;
        }
    }
}
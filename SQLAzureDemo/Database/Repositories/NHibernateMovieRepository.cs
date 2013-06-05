using System.Linq;
using NHibernate;
using NHibernate.Linq;
using SQLAzureDemo.Database.Models;

namespace SQLAzureDemo.Database.Repositories
{
    public class NHibernateMovieRepository : MovieRepository
    {
        private readonly ISession _session;

        public NHibernateMovieRepository(ISession session)
        {
            _session = session;
        }

        protected override IQueryable<Movie> MovieAsQueryable()
        {
            return _session.Query<Movie>();
        }
    }
}
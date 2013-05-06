using NHibernate;

namespace SQLAzureDemo.Database.Repositories
{
    public interface IMovieRepository
    {
        MovieSearchResult Search(string searchText);
    }

    public class MovieRepository : IMovieRepository
    {
        private readonly ISession _session;

        public MovieRepository(ISession session)
        {
            _session = session;
        }

        public MovieSearchResult Search(string searchText)
        {
            return new MovieSearchResult {AverageYearOfCreation = 2005, NumberOfMovies = 100, SearchTerm = searchText};
        }
    }

    public class MovieSearchResult
    {
        public string SearchTerm { get; set; }
        public int AverageYearOfCreation { get; set; }
        public int NumberOfMovies { get; set; }
    }
}
using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using SQLAzureDemo.Database.Models;

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
            var count = _session.Query<Movie>()
                .Count(m => m.Title.Contains(searchText));

            if (count == 0)
                return new MovieSearchResult {SearchTerm = searchText};

            var avg = _session.Query<Movie>()
                .Where(m => m.Title.Contains(searchText))
                .Average(m => m.Year);

            return new MovieSearchResult {AverageYearOfCreation = Convert.ToInt32(avg), NumberOfMovies = count, SearchTerm = searchText};
        }
    }

    public class MovieSearchResult
    {
        public string SearchTerm { get; set; }
        public int AverageYearOfCreation { get; set; }
        public int NumberOfMovies { get; set; }
    }
}
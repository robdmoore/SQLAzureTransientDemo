using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using SQLAzureDemo.Database.Models;

namespace SQLAzureDemo.Database.Repositories
{
    public interface IMovieRepository
    {
        MovieSearchResult Search(string searchText, int page, int numPerPage);
    }

    public class MovieRepository : IMovieRepository
    {
        private readonly ISession _session;

        public MovieRepository(ISession session)
        {
            _session = session;
        }

        public MovieSearchResult Search(string searchText, int page, int numPerPage)
        {
            var count = _session.Query<Movie>()
                .Count(m => m.Title.Contains(searchText));

            if (count == 0)
                return new MovieSearchResult {SearchTerm = searchText};

            var avg = _session.Query<Movie>()
                .Where(m => m.Title.Contains(searchText))
                .Average(m => m.Year);

            var totalPages = (int) Math.Ceiling((double) count/numPerPage);
            if (count < numPerPage*page)
                page = totalPages;

            var movies = _session.Query<Movie>()
                .Where(m => m.Title.Contains(searchText))
                .OrderBy(m => m.Title)
                .Skip(numPerPage*(page - 1))
                .Take(numPerPage)
                .ToList();

            return new MovieSearchResult
            {
                SearchTerm = searchText,
                Page = page,
                ResultsPerPage = numPerPage,
                AbsoluteAverageYearOfCreation = Convert.ToInt32(avg),
                TotalCount = count,
                Movies = movies,
                TotalPages = totalPages
            };
        }
    }

    public class MovieSearchResult
    {
        public string SearchTerm { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int ResultsPerPage { get; set; }
        public int AbsoluteAverageYearOfCreation { get; set; }
        public int TotalCount { get; set; }
        public IList<Movie> Movies { get; set; }
    }
}
using System;
using System.Linq;
using SQLAzureDemo.Database.Models;

namespace SQLAzureDemo.Database.Repositories
{
    public interface IMovieRepository
    {
        MovieSearchResult Search(string searchText, int page, int numPerPage);
    }

    public abstract class MovieRepository : IMovieRepository
    {
        protected abstract IQueryable<Movie> MovieAsQueryable();

        public MovieSearchResult Search(string searchText, int page, int numPerPage)
        {
            var count = MovieAsQueryable()
                .Count(m => m.Title.Contains(searchText));

            if (count == 0)
                return new MovieSearchResult { SearchTerm = searchText };

            var avg = MovieAsQueryable()
                .Where(m => m.Title.Contains(searchText))
                .Average(m => m.Year);

            var totalPages = (int)Math.Ceiling((double)count / numPerPage);
            if (count < numPerPage * page)
                page = totalPages;

            var movies = MovieAsQueryable()
                .Where(m => m.Title.Contains(searchText))
                .OrderBy(m => m.Title)
                .Skip(numPerPage * (page - 1))
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
}
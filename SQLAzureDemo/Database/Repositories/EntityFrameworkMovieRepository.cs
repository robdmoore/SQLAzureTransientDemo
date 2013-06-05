using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using SQLAzureDemo.App_Start.EntityFramework;
using SQLAzureDemo.Database.Models;

namespace SQLAzureDemo.Database.Repositories
{
    public class EntityFrameworkMovieRepository : IMovieRepository
    {
        private readonly IModelContext _modelContext;

        public EntityFrameworkMovieRepository(IModelContext modelContext)
        {
            _modelContext = modelContext;
        }

        public MovieSearchResult Search(string searchText, int page, int numPerPage)
        {
            //todo: refactor using IQueryable with the other Repository?
            var count = _modelContext.Movies
                .Count(m => m.Title.Contains(searchText));

            if (count == 0)
                return new MovieSearchResult {SearchTerm = searchText};

            var avg = _modelContext.Movies
                .Where(m => m.Title.Contains(searchText))
                .Average(m => m.Year);

            var totalPages = (int) Math.Ceiling((double) count/numPerPage);
            if (count < numPerPage*page)
                page = totalPages;

            var movies = _modelContext.Movies
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
}
using System.Collections.Generic;

namespace SQLAzureDemo.Database.Models
{
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
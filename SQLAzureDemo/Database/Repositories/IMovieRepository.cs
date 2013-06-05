using SQLAzureDemo.Database.Models;

namespace SQLAzureDemo.Database.Repositories
{
    public interface IMovieRepository
    {
        MovieSearchResult Search(string searchText, int page, int numPerPage);
    }
}
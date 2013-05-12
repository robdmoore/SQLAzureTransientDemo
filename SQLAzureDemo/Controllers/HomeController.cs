using System.Web.Mvc;
using SQLAzureDemo.Database.Repositories;

namespace SQLAzureDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMovieRepository _repository;

        public HomeController(IMovieRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Transient(string q)
        {
            return View(!string.IsNullOrEmpty(q) ? _repository.Search(q) : null);
        }
    }
}
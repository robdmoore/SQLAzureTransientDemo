using System.Web.Mvc;
using Autofac;
using SQLAzureDemo.App_Start.Autofac;
using SQLAzureDemo.Database.Repositories;

namespace SQLAzureDemo.Controllers
{
    public class TransientController : Controller
    {
        private readonly IMovieRepository _repository;

        public TransientController(IComponentContext scope)
        {
            // Note: you would normally resolve the IMovieRepository into the controller, but this is needed for this demo to enable use of ResolveKeyed
            _repository = scope.ResolveKeyed<IMovieRepository>(NHibernateModule.TransientConnection);
        }

        public ActionResult Index(string q, int page = 1)
        {
            return View(!string.IsNullOrEmpty(q) ? _repository.Search(q, page, 500) : null);
        }
    }
}
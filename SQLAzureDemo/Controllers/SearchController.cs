using System;
using System.Web.Mvc;
using Autofac.Features.OwnedInstances;
using SQLAzureDemo.Controllers.Services;
using SQLAzureDemo.Database.Repositories;

namespace SQLAzureDemo.Controllers
{
    public abstract class SearchController : Controller
    {
        private readonly IMovieRepository _repository;
        private readonly Func<Owned<IControllerOperationLogger>> _operationLoggerFactory;

        public SearchController(IMovieRepository repository, Func<Owned<IControllerOperationLogger>> operationLoggerFactory)
        {
            _repository = repository;
            _operationLoggerFactory = operationLoggerFactory;
        }

        public ActionResult Index(string q, int page = 1)
        {
            if (string.IsNullOrEmpty(q))
                return View();
            using (_operationLoggerFactory())
            {
                return View(_repository.Search(q, page, 500));
            }
        }
    }
}
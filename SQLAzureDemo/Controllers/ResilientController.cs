using System;
using System.Web.Mvc;
using Autofac;
using Autofac.Features.OwnedInstances;
using SQLAzureDemo.App_Start.Autofac;
using SQLAzureDemo.Controllers.Services;
using SQLAzureDemo.Database.Repositories;

namespace SQLAzureDemo.Controllers
{
    public class ResilientController : Controller
    {
        private readonly Func<Owned<IControllerOperationLogger>> _operationLoggerFactory;
        private readonly IMovieRepository _repository;

        //Todo: add endpoint for EF

        public ResilientController(IComponentContext scope, Func<Owned<IControllerOperationLogger>> operationLoggerFactory)
        {
            _operationLoggerFactory = operationLoggerFactory;
            // Note: you would normally resolve the IMovieRepository into the controller, but this is needed for this demo to enable use of ResolveKeyed
            _repository = scope.ResolveKeyed<IMovieRepository>(NHibernateModule.ResilientConnection);
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

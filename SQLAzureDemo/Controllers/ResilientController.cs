using System;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Web.Mvc;
using Autofac;
using SQLAzureDemo.App_Start.Autofac;
using SQLAzureDemo.Database.Repositories;
using Serilog;

namespace SQLAzureDemo.Controllers
{
    public class ResilientController : Controller
    {
        private readonly IMovieRepository _repository;

        public ResilientController(IComponentContext scope)
        {
                // Note: you would normally resolve the IMovieRepository into the controller, but this is needed for this demo to enable use of ResolveKeyed
                _repository = scope.ResolveKeyed<IMovieRepository>(NHibernateModule.ResilientConnection);
        }

        public ActionResult Index(string q, int page = 1)
        {
            try
            {
                return View(!string.IsNullOrEmpty(q) ? _repository.Search(q, page, 500) : null);
            }
            catch (Exception e)
            {
                CheckException(e);
                throw;
            }
        }

        private static void CheckException(Exception e)
        {
            var exception = e as SqlException;
            if (exception != null)
            {
                Log.Logger.Warning(exception, "SQL Exception with error nos. {0}",
                    string.Join(",", exception.Errors.Cast<SqlError>().Select(error => error.Number.ToString()).ToArray()));
            }
            if (e.InnerException != null)
            {
                CheckException(e.InnerException);
            }
        }
    }
}
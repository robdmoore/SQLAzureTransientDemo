using System.Web.Mvc;
using SQLAzureDemo.Controllers.Services;

namespace SQLAzureDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IControllerOperationStatisticsService _statisticsService;

        public HomeController(IControllerOperationStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public ActionResult Index()
        {
            return View(new StatisticsViewModel
                {
                    NHibernate = _statisticsService.GetStatistics(ControllerOperation.NHibernate),
                    EntityFramework = _statisticsService.GetStatistics(ControllerOperation.EntityFramework)
                }
            );
        }
    }

    public class StatisticsViewModel
    {
        public OperationStatistics NHibernate { get; set; }
        public OperationStatistics EntityFramework { get; set; }
    }
}
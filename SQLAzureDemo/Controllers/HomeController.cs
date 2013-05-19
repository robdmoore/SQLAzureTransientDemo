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
            return View(_statisticsService.GetStatistics());
        }
    }
}
using System;
using System.Web.Mvc;
using StackExchange.Exceptional;

namespace SQLAzureDemo.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult Index()
        {
            var context = System.Web.HttpContext.Current;
            var factory = new HandlerFactory();

            var page = factory.GetHandler(context, Request.RequestType, Request.Url.ToString(), Request.PathInfo);
            page.ProcessRequest(context);

            return null;
        }

        public ActionResult Throw()
        {
            throw new Exception("test");
        }
    }
}
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using AlexaTVInfoSkill.Service;

namespace AlexaTVInfoSkill.Controllers
{
    public class TestController : Controller
    {
        public ActionResult Index()
        {
            if (!Request.IsLocal)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));

            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Run(string value)
        {
            if (!Request.IsLocal)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var model = TvMaze.FindShow(value);

            return View(model);
        }
    }
}
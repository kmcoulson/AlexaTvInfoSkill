using System.Web.Mvc;
using AlexaTVInfoSkill.Service;

namespace AlexaTVInfoSkill.Controllers
{
    public class ShowsController : Controller
    {
        public ActionResult Index()
        {
            var model = TvMaze.GetShows();

            return View(model);
        }
    }
}
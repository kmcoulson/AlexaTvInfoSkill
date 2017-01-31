using System.Web.Mvc;
using AlexaTVInfoSkill.Service;

namespace AlexaTVInfoSkill.Controllers
{
    public class RefreshController : Controller
    {
        public ActionResult Index()
        {
            TvInfoService.StoreShows();

            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AlexaTVInfoSkill.Service;
using TVMazeAPI;

namespace AlexaTVInfoSkill.Controllers
{
    public class ShowsController : Controller
    {
        public ActionResult Index()
        {
            var model = TvMaze.GetShows();

            return View(model);
        }

        public ActionResult Search(string term)
        {
            var model = TvMaze.FindShow(term);

            return View(model);
        }
    }
}
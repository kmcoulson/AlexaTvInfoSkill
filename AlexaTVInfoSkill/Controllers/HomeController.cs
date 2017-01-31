using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AlexaTVInfoSkill.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            var model = GetReleaseNotes();

            return View(model);
        }

        private Dictionary<DateTime, string> GetReleaseNotes()
        {
            var releaseNotes = new Dictionary<DateTime, string>
            {
                {DateTime.Parse("25JAN2017 00:00"), "Initial release."},
                {DateTime.Parse("26JAN2017 23:00"), "Updated show name algorithm to improve show name matching."},
                {
                    DateTime.Parse("30JAN2017 23:00"),
                    "UK air dates now returned for UK requests.<br/>Massive improvements to the show matching algorithm.<br/>Performance improvements.<br/>Bug fixes."
                }
            };




            return releaseNotes;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using AlexaTVInfoSkill.Service;
using AlexaTVInfoSkill.Service.Model;

namespace AlexaTVInfoSkill.Controllers
{
    public class TestController : Controller
    {
        public ActionResult Index()
        {
            if (!Request.IsLocal)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var model = new List<Tuple<DateTime, string, string, string>>();

            var logs = Task.Run(DocumentRepository<RequestLog>.GetItemsAsync).Result;

            foreach (var requestLog in logs.Where(x => x.ShowName != "-").OrderByDescending(x => x.TimeStamp))
            {
                var match = TvInfoService.FindShow(requestLog.ShowName, "en-GB", true);

                model.Add(new Tuple<DateTime, string, string, string>(requestLog.TimeStamp, requestLog.ShowName, match != null ? match.Name : "Unmatched", requestLog.ResponseText));
            }

            return View(model);
        }
    }
}
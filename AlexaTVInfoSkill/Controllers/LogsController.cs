using System.Threading.Tasks;
using System.Web.Mvc;
using AlexaTVInfoSkill.Service;
using AlexaTVInfoSkill.Service.Model;

namespace AlexaTVInfoSkill.Controllers
{
    public class LogsController : Controller
    {
        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var logs = await DocumentRepository<RequestLog>.GetItemsAsync();
            return View(logs);
        }
    }
}
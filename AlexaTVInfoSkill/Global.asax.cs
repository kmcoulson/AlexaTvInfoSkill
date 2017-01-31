using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AlexaTVInfoSkill.Service;
using AlexaTVInfoSkill.Service.Model;
using AlexaTVInfoSkill.Service.Model.TvMaze;
using AlexaTVInfoSkill.Service.Scheduler;

namespace AlexaTVInfoSkill
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DocumentRepository<RequestLog>.Initialize("TVInfo");
            DocumentRepository<TvShow>.Initialize("TVShows");

            SchedulingHost.Start();
        }
    }
}

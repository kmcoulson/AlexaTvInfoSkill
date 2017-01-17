using System.Web.Http;
using AlexaTVInfoSkill.Service;
using AlexaTVInfoSkill.Service.Model;

namespace AlexaTVInfoSkill.Controllers
{
    public class AlexaController : ApiController
    {
        private readonly AlexaRequestHandler _requestHandler;

        public AlexaController(AlexaRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        public AlexaController() : this(new AlexaRequestHandler()) { }

        [HttpPost, Route("api/alexa/tvinfo")]
        public AlexaResponse TvInfo(AlexaRequest request)
        {
            return _requestHandler.Process(request);
        }
    }
}
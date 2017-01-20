using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AlexaTVInfoSkill.Service;
using AlexaTVInfoSkill.Service.Model;

namespace AlexaTVInfoSkill.Controllers
{
    public class AlexaController : ApiController
    {
        private const string ApplicationId = "amzn1.ask.skill.4a117fa6-d67b-461b-a6bb-2d7315ecec2c";
        private readonly AlexaRequestHandler _requestHandler;

        public AlexaController(AlexaRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        public AlexaController() : this(new AlexaRequestHandler()) { }

        [HttpPost, Route("api/alexa/tvinfo")]
        public AlexaResponse TvInfo(AlexaRequest request)
        {
            if (request.Session.Application.ApplicationId != ApplicationId)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var totalSeconds = (DateTime.UtcNow - request.Request.Timestamp).TotalSeconds;
            if (!Request.IsLocal() && (totalSeconds <= 0 || totalSeconds > 150))
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));

            return _requestHandler.Process(request);
        }
    }
}
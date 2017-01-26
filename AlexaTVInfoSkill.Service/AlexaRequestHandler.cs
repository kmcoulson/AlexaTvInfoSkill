using System;
using System.Linq;
using AlexaTVInfoSkill.Service.Model;

namespace AlexaTVInfoSkill.Service
{
    public class AlexaRequestHandler
    {
        private readonly RequestLogService _requestLogService;

        public AlexaRequestHandler(RequestLogService requestLogService)
        {
            _requestLogService = requestLogService;
        }

        public AlexaRequestHandler() : this(new RequestLogService()) { }

        public AlexaResponse Process(AlexaRequest request)
        {
            AlexaResponse response;

            switch (request.Request.Type)
            {
                case "LaunchRequest":
                    response = Launch();
                    break;
                case "IntentRequest":
                    response = Intent(request);
                    break;
                case "SessionEndedRequest":
                    response = GetSessionEndedResponse();
                    break;
                default:
                    response = null;
                    break;
            }

            try
            {
                SaveLog(request, response);
            }
            catch (Exception e)
            {
            }

            return response;
        }

        private void SaveLog(AlexaRequest request, AlexaResponse response)
        {
            var slots = request.Request.Intent.GetSlots().Select(intentSlot => new RequestSlotLog
            {
                Name = intentSlot.Key, Value = intentSlot.Value
            }).ToList();

            var log = new RequestLog
            {
                RequestId = request.Request.RequestId,
                UserId = request.Session.User.UserId,
                Type = request.Request.Type,
                Intent = request.Request.Intent.Name,
                TimeStamp = request.Request.Timestamp,
                ResponseText = response.Response.OutputSpeech.Type == "PlainText" ? response.Response.OutputSpeech.Text : response.Response.OutputSpeech.Ssml,
                Slots = slots
            };
            
            var task = _requestLogService.Add(log);
        }

        private AlexaResponse Launch()
        {
            return new AlexaResponse("Welcome to the TV Info skill, you can use this skill to ask about TV Shows. What would you like to know?", false)
            {
                Response =
                {
                    Card = new AlexaResponse.ResponseAttributes.CardAttributes
                    {
                        Title = "Launched",
                        Content = "Welcome to the TV Info skill."
                    },
                    Reprompt = new AlexaResponse.ResponseAttributes.RepromptAttributes
                    {
                        OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes
                        {
                            Text = "What would you like to know?"
                        }
                    }
                }
            };
        }

        private static AlexaResponse Intent(AlexaRequest request)
        {
            AlexaResponse response = null;

            switch (request.Request.Intent.Name)
            {
                case "ShowSynopsisIntent":
                case "ShowStartIntent":
                case "ShowNextEpisodeIntent":
                case "ShowCastIntent":
                case "ShowPersonIntent":
                case "ShowCharacterIntent":
                    response = AlexaTvShowIntentHandler.GetTvShowResponse(request.Request.Intent.Name, request);
                    break;
                case "AMAZON.CancelIntent":
                case "AMAZON.StopIntent":
                    response = GetSessionEndedResponse();
                    break;
                case "AMAZON.HelpIntent":
                    response = GetHelpResponse();
                    break;
            }

            return response;
        }


        private static AlexaResponse GetHelpResponse()
        {
            return new AlexaResponse("You can ask TV Info: what your favourite show is about, when your show is next on, who is in your show or who plays your favourite character in a show. What would you like to know?", false);
        }

        private static AlexaResponse GetSessionEndedResponse()
        {
            return new AlexaResponse("Thanks for using the TV Info skill");
        }
    }
}

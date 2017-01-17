using System;
using System.Linq;
using AlexaTVInfoSkill.Service.Model;

namespace AlexaTVInfoSkill.Service
{
    public class AlexaRequestHandler
    {
        public AlexaResponse Process(AlexaRequest request)
        {
            switch (request.Request.Type)
            {
                case "LaunchRequest":
                    return Launch();
                case "IntentRequest":
                    return Intent(request);
                case "SessionEndedRequest":
                    return GetSessionEndedResponse();
                default:
                    return null;
            }
        }


        private AlexaResponse Launch()
        {
            return new AlexaResponse("Welcome to the TV Info skill, you can use this skill to ask about TV Shows.", false)
            {
                Response =
                {
                    Card = new AlexaResponse.ResponseAttributes.CardAttributes
                    {
                        Title = "Launched",
                        Content = "Welcome to the TV Info skill."
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
            return new AlexaResponse("You can ask TV Info...");
        }

        private static AlexaResponse GetSessionEndedResponse()
        {
            return new AlexaResponse("Thanks for using the TV Info skill");
        }
    }
}

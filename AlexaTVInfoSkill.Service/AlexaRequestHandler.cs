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

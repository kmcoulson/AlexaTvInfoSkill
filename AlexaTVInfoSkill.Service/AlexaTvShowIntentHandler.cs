using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlexaTVInfoSkill.Service.Model;

namespace AlexaTVInfoSkill.Service
{
    public class AlexaTvShowIntentHandler
    {
        public static AlexaResponse GetTvShowResponse(string intent, AlexaRequest request)
        {
            var showName = GetSlotValue(request, "showtitle");
            if (string.IsNullOrEmpty(showName)) return new AlexaResponse("Sorry, I didn't hear a show name.", "Show not found");


            var show = TvInfoService.FindShow(showName, request.Request.Locale);
            if (show == null) return new AlexaResponse($"Sorry, I wasn't able to find the show '{showName}', please try again.", "Show not found");

            string showId;
            ITvService tvService;
            if (request.Request.Locale == "en-GB")
            {
                showId = show.ExternalIds.ImdbId;
                tvService = new ImdbService();
            }
            else
            {
                showId = show.Id;
                tvService = new TvMazeService();
            }

            using (var tvInfoService = new TvInfoService(tvService))
            {
                switch (intent)
                {
                    case "ShowSynopsisIntent":
                        return GetSynopsis(show.Name, show.Summary);

                    case "ShowStartIntent":
                        {
                            var originalAirDate = tvInfoService.GetStartDate(showId);
                            return GetStartDate(show.Name, originalAirDate);
                        }

                    case "ShowNextEpisodeIntent":
                        {
                            var episode = show.Status == "Ended"
                                ? tvInfoService.GetLastEpisodeAirDate(showId)
                                : tvInfoService.GetNextEpisodeAirDate(showId);
                            return GetNextEpisode(show.Name, show.Status, episode);
                        }

                    case "ShowCastIntent":
                        {
                            var personName = GetSlotValue(request, "person");
                            if (!string.IsNullOrEmpty(personName))
                            {
                                var person = tvInfoService.GetCharacter(showId, personName);
                                return GetPerson(show.Name, personName, person);
                            }

                            var characterName = GetSlotValue(request, "character");
                            if (!string.IsNullOrEmpty(characterName))
                            {
                                var person = tvInfoService.GetActor(showId, characterName);
                                return GetCharacter(show.Name, characterName, person);
                            }

                            var cast = tvInfoService.GetCast(showId);
                            return GetCast(show.Name, cast);
                        }
                }

                return new AlexaResponse("Sorry, I don't know how to respond to your request.", "Unknown Intent");
            }
        }

        private static string GetSlotValue(AlexaRequest request, string slotName)
        {
            var slots = request.Request.Intent.GetSlots();
            return slots.All(x => x.Key != slotName) ? null : slots.First(x => x.Key == slotName).Value;
        }

        private static AlexaResponse GetSynopsis(string name, string synopsis)
        {
            return new AlexaResponse(synopsis, $"Synopsis for {name}");
        }

        private static AlexaResponse GetStartDate(string name, DateTime? originalAirDate)
        {
            var cardTitle = $"Premiere date for {name}";

            if (!originalAirDate.HasValue)
                return new AlexaResponse($"Sorry, I wasn't able to find the premiere date for {name}.", cardTitle);

            var outputSpeechText = $"{name} premiered on <say-as interpret-as='date'>{originalAirDate.Value:yyyyMMdd}</say-as>.";
            var cardContent = $"{name} premiered on {originalAirDate.Value:dd MMM yyyy}.";

            return new AlexaResponse(outputSpeechText, cardTitle, cardContent);
        }

        private static AlexaResponse GetNextEpisode(string name, string status, ITvEpisode episode)
        {
            var cardTitle = $"Next episode of {name}";

            if (status == "Ended")
            {
                if (!episode.OriginalAirDate.HasValue)
                    return new AlexaResponse($"{name} has ended.", cardTitle);

                return new AlexaResponse($"{name} ended on <say-as interpret-as='date'>{episode.OriginalAirDate.Value:yyyyMMdd}</say-as>.", cardTitle, $"{name} ended on {episode.OriginalAirDate.Value:dd MMM yyyy}.");
            }

            if (!episode.OriginalAirDate.HasValue)
                return new AlexaResponse($"Sorry, the air date for the next episode of {name} hasn't been released yet.", cardTitle);

            var episodeName = episode.Name == "TBA" || episode.Name == $"Episode #{episode.Season}.{episode.Number}" ? "" : $", called {episode.Name},";
            var airTime = episode.OriginalAirDate.Value.ToString("HHmm") != "0000" ? $" at <say-as interpret-as='time'>{episode.OriginalAirDate.Value:HH:mm}</say-as>" : "";
            return new AlexaResponse(
                $"Season {episode.Season} episode {episode.Number} of {name}{episodeName} airs on <say-as interpret-as='date'>{episode.OriginalAirDate.Value:yyyyMMdd}</say-as>{airTime}.",
                cardTitle,
                $"Season {episode.Season} - Episode {episode.Number}\n{episodeName}\nAirs {episode.OriginalAirDate.Value:dd MMM yyyy}{airTime}.");
        }

        private static AlexaResponse GetCast(string name, List<CastMember> cast)
        {
            var cardTitle = $"Cast of {name}";
            
            if (cast == null || !cast.Any()) return new AlexaResponse($"Sorry, I wasn't able to find any cast information for {name}.", cardTitle);

            var resultList = new StringBuilder();
            foreach (var castMember in cast)
            {
                if (castMember != cast.First()) resultList.Append(", ");
                if (castMember == cast.Last()) resultList.Append("and ");

                resultList.Append(castMember.Person.Name != castMember.Character.Name
                    ? $"{castMember.Person.Name} as {castMember.Character.Name}"
                    : castMember.Person.Name);
            }

            var outputSpeechText = $"{name} stars: {resultList}";
            var cardContent = resultList.ToString().Replace(", ", "\n").Replace(" and ", "\n");

            return new AlexaResponse(outputSpeechText, cardTitle, cardContent);
        }

        private static AlexaResponse GetPerson(string showName, string personQuery, CastMember person)
        {
            var cardTitle = $"{personQuery.ToCamelCase()} in {showName}";

            if (person == null)
                return new AlexaResponse($"Sorry, I was unable to find an actor called '{personQuery}' starring in {showName}", cardTitle);

            var outputSpeechText = $"{person.Person.Name} played {person.Character.Name} in {showName}.";

            return new AlexaResponse(outputSpeechText, cardTitle);
        }

        private static AlexaResponse GetCharacter(string showName, string characterQuery, CastMember person)
        {
            var cardTitle = $"{characterQuery.ToCamelCase()} in {showName}";

            if (person == null)
                return new AlexaResponse($"Sorry, I was unable to find a character called '{characterQuery}' on {showName}", cardTitle);

            var outputSpeechText = $"{person.Character.Name} was played by {person.Person.Name} in {showName}.";

            return new AlexaResponse(outputSpeechText, cardTitle);
        }
    }
}

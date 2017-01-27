using System;
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

            var show = TvMaze.FindShow(showName);
            if (show == null) return new AlexaResponse($"Sorry, I wasn't able to find the show '{showName}', please try again.", "Show not found");
            
            switch (intent)
            {
                case "ShowSynopsisIntent":
                    return GetSynopsis(show);

                case "ShowStartIntent":
                    return GetStartDate(show);

                case "ShowNextEpisodeIntent":
                    return GetNextEpisode(show);

                case "ShowCastIntent":
                {
                    AlexaResponse response = null;

                    var personName = GetSlotValue(request, "person");
                    if (!string.IsNullOrEmpty(personName)) response = GetPerson(show, personName);

                        var characterName = GetSlotValue(request, "character");
                    if (!string.IsNullOrEmpty(characterName)) response = GetCharacter(show, characterName);

                    return response ?? GetCast(show);
                }
            }

            return new AlexaResponse("Sorry, I don't know how to respond to your request.", "Unknown Intent");
        }

        private static string GetSlotValue(AlexaRequest request, string slotName)
        {
            var slots = request.Request.Intent.GetSlots();
            return slots.All(x => x.Key != slotName) ? null : slots.First(x => x.Key == slotName).Value;
        }

        private static AlexaResponse GetSynopsis(TvShow show)
        {
            return new AlexaResponse(show.Summary, $"Synopsis for {show.Name}");
        }

        private static AlexaResponse GetStartDate(TvShow show)
        {
            var cardTitle = $"Premiere date for {show.Name}";

            DateTime startDate;
            if (string.IsNullOrEmpty(show.Premiered) || !DateTime.TryParse(show.Premiered, out startDate))
                return new AlexaResponse($"Sorry, I wasn't able to find the premiere date for {show.Name}.", cardTitle);

            var outputSpeechText = $"{show.Name} premiered on <say-as interpret-as='date'>{startDate:yyyyMMdd}</say-as>.";
            var cardContent = $"{show.Name} premiered on {startDate:dd MMM yyyy}.";

            return new AlexaResponse(outputSpeechText, cardTitle, cardContent);
        }

        private static AlexaResponse GetNextEpisode(TvShow show)
        {
            var cardTitle = $"Next episode of {show.Name}";

            if (show.Status == "Ended")
            {
                if (show.Links.PreviousEpisode?.Id == null)
                    return new AlexaResponse($"{show.Name} has ended.", cardTitle);

                var lastEpisode = TvMaze.GetEpisode(show.Links.PreviousEpisode.Id.Value);
                DateTime endDate;
                if (!string.IsNullOrEmpty(lastEpisode.AirDate) && DateTime.TryParse(lastEpisode.AirDate, out endDate))
                    return new AlexaResponse($"{show.Name} ended on <say-as interpret-as='date'>{endDate:yyyyMMdd}</say-as>.", cardTitle, $"{show.Name} ended on {endDate:dd MMM yyyy}.");

                return new AlexaResponse($"{show.Name} has ended.", cardTitle);
            }

            if (show.Links.NextEpisode?.Id == null)
                return new AlexaResponse($"Sorry, the air date for the next episode of {show.Name} hasn't been released yet.", cardTitle);

            var nextEpisode = TvMaze.GetEpisode(show.Links.NextEpisode.Id.Value);
            DateTime airDate;
            if (!string.IsNullOrEmpty(nextEpisode.AirDate) && DateTime.TryParse(nextEpisode.AirDate, out airDate))
            {
                var episodeName = nextEpisode.Name == "TBA" ? "" : $", called {nextEpisode.Name},";
                return new AlexaResponse(
                    $"Season {nextEpisode.Season} episode {nextEpisode.Number} of {show.Name}{episodeName} airs on <say-as interpret-as='date'>{airDate:yyyyMMdd}</say-as>.",
                    cardTitle,
                    $"Season {nextEpisode.Season} - Episode {nextEpisode.Name}\n{nextEpisode.Name}\nAirs {airDate:dd MMM yyyy}.");
            }

            return new AlexaResponse($"Sorry, the air date for the next episode of {show.Name} hasn't been released yet.", cardTitle);
        }

        private static AlexaResponse GetCast(TvShow show)
        {
            var cardTitle = $"Cast of {show.Name}";

            var cast = TvMaze.GetShowCast(show.Id);
            if (cast == null || !cast.Any()) return new AlexaResponse($"Sorry, I wasn't able to find any cast information for {show.Name}.", cardTitle);

            var resultList = new StringBuilder();
            foreach (var castMember in cast)
            {
                if (castMember != cast.First()) resultList.Append(", ");
                if (castMember == cast.Last()) resultList.Append("and ");
                resultList.Append($"{castMember.Person.Name} as {castMember.Character.Name}");
            }

            var outputSpeechText = $"{show.Name} stars: {resultList}";
            var cardContent = resultList.ToString().Replace(", ", "\n").Replace(" and ", "\n");

            return new AlexaResponse(outputSpeechText, cardTitle, cardContent);
        }

        private static AlexaResponse GetPerson(TvShow show, string personName)
        {
            if (personName == null)
                return null;

            var cardTitle = $"{personName.ToCamelCase()} in {show.Name}";

            var cast = TvMaze.GetShowCast(show.Id);
            if (cast == null || !cast.Any()) return null;

            var personNameParts = personName.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

            var people = cast.Where(x => personNameParts.All(c => x.Person.Name.ToLower().Contains(c.ToLower()))).ToList();
            var person = people.FirstOrDefault();
            if (person == null) return null;
            
            var outputSpeechText = new StringBuilder($"{person.Person.Name} played {person.Character.Name} in {show.Name}");
            if (people.Count > 1)
            {
                outputSpeechText.Append(" In addition: ");
                foreach (var castMember in people)
                {
                    if (castMember == people.First()) continue;
                    if (castMember == people.Last()) outputSpeechText.Append("and ");
                    var lineEnd = castMember != people.Last() ? ", " : "";

                    outputSpeechText.Append($"{castMember.Person.Name} played {castMember.Character.Name}{lineEnd}");
                }
            }
            outputSpeechText.Append(".");

            return new AlexaResponse(outputSpeechText.ToString(), cardTitle);
        }

        private static AlexaResponse GetCharacter(TvShow show, string characterName)
        {
            if (characterName == null)
                return null;

            var cardTitle = $"{characterName.ToCamelCase()} in {show.Name}";

            var cast = TvMaze.GetShowCast(show.Id);
            if (cast == null || !cast.Any()) return null;

            var characterNameParts = characterName.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var people = cast.Where(x => characterNameParts.All(c => x.Character.Name.ToLower().Contains(c.ToLower()))).ToList();
            var person = people.FirstOrDefault();
            if (person == null) return null;

            var outputSpeechText = new StringBuilder($"{person.Character.Name} was played by {person.Person.Name} in {show.Name}");
            if (people.Count > 1)
            {
                outputSpeechText.Append(" In addition: ");
                foreach (var castMember in people)
                {
                    if (castMember == people.First()) continue;
                    if (castMember == people.Last()) outputSpeechText.Append("and ");
                    var lineEnd = castMember != people.Last() ? ", " : "";

                    outputSpeechText.Append($"{castMember.Character.Name} was played by {castMember.Person.Name}{lineEnd}");
                }
            }
            outputSpeechText.Append(".");

            return new AlexaResponse(outputSpeechText.ToString(), cardTitle);
        }
    }
}

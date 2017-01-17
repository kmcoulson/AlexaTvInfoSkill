﻿using System;
using System.Linq;
using System.Text;
using AlexaTVInfoSkill.Service.Model;

namespace AlexaTVInfoSkill.Service
{
    public class AlexaTvShowIntentHandler
    {
        public static AlexaResponse GetTvShowResponse(string intent, AlexaRequest request)
        {
            var showName = GetShowNameFromSlot(request);
            if (string.IsNullOrEmpty(showName)) return new AlexaResponse("Sorry, I didn't hear a show name.", "Show not found");

            var show = TvMaze.FindShow(showName);
            if (show == null) return new AlexaResponse("Sorry, I wasn't able to find the show you asked for, please try again.", "Show not found");
            
            switch (intent)
            {
                case "ShowSynopsisIntent":
                    return GetSynopsis(show);

                case "ShowStartIntent":
                    return GetStartDate(show);

                case "ShowNextEpisodeIntent":
                    return GetNextEpisode(show);

                case "ShowCastIntent":
                    return GetCast(show);
            }

            return new AlexaResponse("Sorry, I don't know how to respond to your request.", "Unknown Intent");
        }

        private static string GetShowNameFromSlot(AlexaRequest request)
        {
            var slots = request.Request.Intent.GetSlots();
            return slots.All(x => x.Key != "showtitle") ? null : slots.First(x => x.Key == "showtitle").Value;
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
                return new AlexaResponse($"Sorry, I wasn't able to find the premiere date for {show.Name}", cardTitle);

            var outputSpeechText = $"{show.Name} premiered on <say-as interpret-as='date'>{startDate:yyyyMMdd}</say-as>.";
            var cardContent = $"{show.Name} premiered on {startDate:dd MMM yyyy}.";

            return new AlexaResponse(outputSpeechText, cardTitle, cardContent);
        }

        private static AlexaResponse GetNextEpisode(TvShow show)
        {
            var cardTitle = $"Next episode of {show.Name}";

            if (show.Status == "Ended")
            {
                if (!show.Links.PreviousEpisode.Id.HasValue)
                    return new AlexaResponse($"{show.Name} has ended.", cardTitle);

                var lastEpisode = TvMaze.GetEpisode(show.Links.PreviousEpisode.Id.Value);
                DateTime endDate;
                if (!string.IsNullOrEmpty(lastEpisode.AirDate) && DateTime.TryParse(lastEpisode.AirDate, out endDate))
                    return new AlexaResponse($"{show.Name} ended on {endDate:yyyyMMdd}.", cardTitle, $"{show.Name} ended on <say-as interpret-as='date'>{endDate:dd MMM yyyy}</say-as>.");

                return new AlexaResponse($"{show.Name} has ended.", cardTitle);
            }

            if (!show.Links.NextEpisode.Id.HasValue)
                return new AlexaResponse($"Sorry, I don't know when {show.Name} will continue yet.", cardTitle);

            var nextEpisode = TvMaze.GetEpisode(show.Links.NextEpisode.Id.Value);
            DateTime airDate;
            if (!string.IsNullOrEmpty(nextEpisode.AirDate) && DateTime.TryParse(nextEpisode.AirDate, out airDate))
                return new AlexaResponse(
                    $"Season {nextEpisode.Season} episode {nextEpisode.Number} of {show.Name}, called {nextEpisode.Name}, airs on <say-as interpret-as='date'>{airDate:yyyyMMdd}</say-as>.", 
                    cardTitle,
                    $"Season {nextEpisode.Season} - Episode {nextEpisode.Name}\n{nextEpisode.Name}\nAirs {airDate:dd MMM yyyy}");

            return new AlexaResponse($"Sorry, I don't know when {show.Name} will continue yet.", cardTitle);
        }

        private static AlexaResponse GetCast(TvShow show)
        {
            var cardTitle = $"Cast of {show.Name}";

            var cast = TvMaze.GetShowCast(show.Id);
            if (cast == null || !cast.Any()) return new AlexaResponse($"Sorry, I wasn't able to find any cast information for {show.Name}", cardTitle);

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
    }
}
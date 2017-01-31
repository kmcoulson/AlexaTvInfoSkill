using System;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using AlexaTVInfoSkill.Service.Model;
using AlexaTVInfoSkill.Service.Model.Imdb;

namespace AlexaTVInfoSkill.Service
{
    public class ImdbService : ITvService
    {
        private const string ImdbApi = "http://www.myapifilms.com/imdb/idIMDB";
        private static readonly string ApiKey = Config.Imdb.ApiKey;

        public static TvShow GetResponse(string imdbId)
        {
            if (AppCache.Contains(imdbId))
                return AppCache.Get<TvShow>(imdbId);

            var url = $"{ImdbApi}?idIMDB={imdbId}&token={ApiKey}&format=json&language=en-gb&seasons=1";

            var request = WebRequest.Create(url) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Server error (HTTP {response.StatusCode}: {response.StatusDescription}).");

                var jsonSerializer = new DataContractJsonSerializer(typeof(ImdbResponse));
                var imdbResponse = (ImdbResponse)jsonSerializer.ReadObject(response.GetResponseStream());

                var show = imdbResponse.Data.Shows.FirstOrDefault();
                if (show != null) AppCache.Add(imdbId, show, DateTime.Now.AddHours(12));
                return show;
            }
        }

        public ITvShow GetShow(string id)
        {
            return GetResponse(id);
        }

        public ITvEpisode GetNextEpisode(string showId)
        {
            var show = GetResponse(showId);
            var futureEpisodes = show.Seasons.SelectMany(s => s.Episodes).Where(e => e.OriginalAirDate.HasValue && e.OriginalAirDate.Value > DateTime.Now);
            var nextEpisode = futureEpisodes.OrderBy(e => e.OriginalAirDate).FirstOrDefault();
            if (nextEpisode == null) return null;
            {
                var season = show.Seasons.First(s => s.Episodes.Any(e => e.Id == nextEpisode.Id));
                nextEpisode.Season = season.Number;
                return nextEpisode;
            }
        }

        public ITvEpisode GetLastEpisode(string showId)
        {
            var show = GetResponse(showId);
            var pastEpisodes = show.Seasons.SelectMany(s => s.Episodes).Where(e => e.OriginalAirDate.HasValue && e.OriginalAirDate.Value < DateTime.Now);
            var lastEpisode = pastEpisodes.OrderByDescending(e => e.OriginalAirDate).FirstOrDefault();
            if (lastEpisode == null) return null;
            {
                var season = show.Seasons.First(s => s.Episodes.Any(e => e.Id == lastEpisode.Id));
                lastEpisode.Season = season.Number;
                return lastEpisode;
            }
        }
    }
}

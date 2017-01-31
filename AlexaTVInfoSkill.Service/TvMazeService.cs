using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using AlexaTVInfoSkill.Service.Model;
using AlexaTVInfoSkill.Service.Model.TvMaze;

namespace AlexaTVInfoSkill.Service
{
    public class TvMazeService : ITvService
    {
        private const string TvMazeApi = "http://api.tvmaze.com";

        private enum Type
        {
            AllShows,
            Shows,
            SearchShows,
            Episodes,
            Cast
        }

        private static T GetResponse<T>(Type type, string paramater)
        {
            string url;
            switch (type)
            {
                case Type.AllShows:
                    url = $"{TvMazeApi}/shows/?page={paramater}";
                    break;
                case Type.Shows:
                    url = $"{TvMazeApi}/shows/{paramater}";
                    break;
                case Type.SearchShows:
                    url = $"{TvMazeApi}/search/shows?q={paramater}";
                    break;
                case Type.Episodes:
                    url = $"{TvMazeApi}/episodes/{paramater}";
                    break;
                case Type.Cast:
                    url = $"{TvMazeApi}/shows/{paramater}/cast";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            };

            var request = WebRequest.Create(url) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Server error (HTTP {response.StatusCode}: {response.StatusDescription}).");

                var jsonSerializer = new DataContractJsonSerializer(typeof(T));
                return (T)jsonSerializer.ReadObject(response.GetResponseStream());
            }
        }

        public static IEnumerable<SearchContainer> FindShow(string query)
        {
            return GetResponse<List<SearchContainer>>(Type.SearchShows, query);
        }

        public static List<ITvShow> GetShows(string page)
        {
            var response = GetResponse<List<TvShow>>(Type.SearchShows, page);
            return response.Select(x => (ITvShow)x).ToList();
        }

        public static List<CastMember> GetCast(string id)
        {
            var response = GetResponse<List<CastMember>>(Type.Cast, id);
            return response;
        }


        public ITvShow GetShow(string id)
        {
            var response = GetResponse<TvShow>(Type.Shows, id);
            return response;
        }

        public ITvEpisode GetEpisode(string id)
        {
            var response = GetResponse<TvEpisode>(Type.Episodes, id);
            return response;
        }

        public ITvEpisode GetNextEpisode(string showId)
        {
            var show = GetShow(showId);
            return show?.Links?.NextEpisode == null ? null : GetEpisode(show.Links.NextEpisode.Id);
        }

        public ITvEpisode GetLastEpisode(string showId)
        {
            var show = GetShow(showId);
            return show?.Links?.PreviousEpisode == null ? null : GetEpisode(show.Links.PreviousEpisode.Id);
        }
    }
}

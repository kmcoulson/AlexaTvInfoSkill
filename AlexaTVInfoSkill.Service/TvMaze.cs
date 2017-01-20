using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using AlexaTVInfoSkill.Service.Model;

namespace AlexaTVInfoSkill.Service
{
    public class TvMaze
    {
        private static T GetResponse<T>(string endPoint)
        {
            const string apiUrl = "http://api.tvmaze.com";
            var url = endPoint.StartsWith(apiUrl) ? endPoint : $"{apiUrl}{endPoint}";

            var request = WebRequest.Create(url) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Server error (HTTP {response.StatusCode}: {response.StatusDescription}).");

                var jsonSerializer = new DataContractJsonSerializer(typeof(T));
                return (T)jsonSerializer.ReadObject(response.GetResponseStream());
            }
        }

        public static List<TvShow> GetShows()
        {
            var response = GetResponse<List<TvShow>>("/shows");
            return response;
        }

        public static TvShow GetShow(int id)
        {
            var endPoint = string.Concat("/search/shows/", id);
            var response = GetResponse<TvShow>(endPoint);
            return response;
        }

        public static TvShow FindShow(string query)
        {
            var endPoint = string.Concat("/search/shows?q=", query);
            var response = GetResponse<List<ShowSearchContainer>>(endPoint);
            return response.Any() ? response.OrderByDescending(x => x.Score).First().Show : null;
        }

        public static List<CastMember> GetShowCast(int id)
        {
            var endPoint = string.Concat("/shows/", id, "/cast");
            var response = GetResponse<List<CastMember>>(endPoint);
            return response;
        }

        public static TvEpisode GetEpisode(int id)
        {
            var endPoint = string.Concat("/episodes/", id);
            var response = GetResponse<TvEpisode>(endPoint);
            return response;
        }
    }
}

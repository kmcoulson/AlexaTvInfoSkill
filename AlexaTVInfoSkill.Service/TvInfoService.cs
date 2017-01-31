using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AlexaTVInfoSkill.Service.Model;
using AlexaTVInfoSkill.Service.Model.TvMaze;
using Microsoft.Win32.SafeHandles;

namespace AlexaTVInfoSkill.Service
{
    public class TvInfoService : IDisposable
    {
        private readonly ITvService _tvService;

        public TvInfoService(ITvService tvService)
        {
            _tvService = tvService;
        }

        public static void StoreShows()
        {
            var tasks = new List<Task>();
            for (var i = 0; i < int.MaxValue; i++)
            {
                try
                {
                    var response = TvMazeService.GetShows(i.ToString());
                    tasks.AddRange(response.Where(x => x.Language == "English").Select(x => DocumentRepository<TvShow>.CreateItemAsync((TvShow)x)));
                }
                catch (Exception)
                {
                    break;
                }
            }

            Task.Run(() => tasks);
        }

        public static IEnumerable<TvShow> GetShows()
        {
            if (AppCache.Contains("tvshows"))
            {
                return AppCache.Get<List<TvShow>>("tvshows");
            }

            var tvShows = Task.Run(DocumentRepository<TvShow>.GetItemsAsync).Result;
            AppCache.Add("tvshows", tvShows, DateTime.Now.AddHours(12));
            return tvShows;
        }
        
        public static ITvShow FindShow(string query, string locale, bool localOnly = false)
        {
            query = FixQuery(query);
            var searchResults = FindShowLocally(query).ToList();
            if (!localOnly)
                searchResults = searchResults.Concat(TvMazeService.FindShow(query)).ToList();
            searchResults.ForEach(x => x.Score = CalcLevenshteinDistance(x.Show.Name.ToLower(), query.ToLower()));
            var result = searchResults.Any() ? searchResults.OrderBy(x => x.Score).First().Show : null;

            return result;
        }

        private static string FixQuery(string query)
        {
            return query.ToLower().TrimWords(new[] { "tv info", "the next time", "the next", "the tv show", "will be", "right now" });
        }

        private static IEnumerable<SearchContainer> FindShowLocally(string query)
        {
            var queryParts = query.RemoveStopwords().ToLower().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var shows = GetShows().ToList();

            var nameMatches = shows.Where(s => s.Name.ToLower() == query || s.Name.ToLower().StartsWith(query) || s.Name.ToLower().EndsWith(query)).ToList();
            foreach (var queryPart in queryParts)
            {
                var partMatches = shows.Where(s => s.NameParts.Contains(queryPart));
                nameMatches.AddRange(partMatches);

                if (!queryPart.Contains("'") && !queryPart.Contains("'s")) continue;
                
                var noCommaMatches = shows.Where(s => s.NameParts.Contains(queryPart.Replace("'", "")));
                nameMatches.AddRange(noCommaMatches);
                var singularMatches = shows.Where(s => s.NameParts.Contains(queryPart.Replace("'s", "")));
                nameMatches.AddRange(singularMatches);
                
            }

            return nameMatches.Select(match => new SearchContainer { Show = match });
        }

        private static decimal CalcLevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0;

            var lengthA = a.Length;
            var lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (var i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (var j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (var i = 1; i <= lengthA; i++)
                for (var j = 1; j <= lengthB; j++)
                {
                    var cost = b[j - 1] == a[i - 1] ? 0 : 1;
                    distances[i, j] = Math.Min
                        (
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                        );
                }
            return distances[lengthA, lengthB];
        }

        public string GetSynopsis(string id)
        {
            var show = _tvService.GetShow(id);
            return show?.Summary;
        }

        public string GetStatus(string id)
        {
            var show = id.StartsWith("tt")
                ? GetShows().FirstOrDefault(s => s.ExternalIds.ImdbId == id)
                : GetShows().FirstOrDefault(s => s.Id == id);
            return show?.Status;
        }

        public DateTime? GetStartDate(string id)
        {
            var show = _tvService.GetShow(id);
            return show?.StartDate;
        }

        public ITvEpisode GetNextEpisodeAirDate(string id)
        {
            return _tvService.GetNextEpisode(id);
        }

        public ITvEpisode GetLastEpisodeAirDate(string id)
        {
            return _tvService.GetLastEpisode(id);
        }

        public List<CastMember> GetCast(string id)
        {
            var show = id.StartsWith("tt")
                ? GetShows().FirstOrDefault(s => s.ExternalIds.ImdbId == id)
                : GetShows().FirstOrDefault(s => s.Id == id);
            return show != null ? TvMazeService.GetCast(show.Id) : null;
        }

        public CastMember GetActor(string showId, string characterName)
        {
            var cast = GetCast(showId);
            var scores = cast.ToDictionary(x => x, x => CalcLevenshteinDistance(x.Character.Name, characterName));
            return scores.Any(s => s.Value < 5) ? scores.Where(s => s.Value < 5).OrderBy(x => x.Value).First().Key : null;
        }

        public CastMember GetCharacter(string showId, string actorName)
        {
            var cast = GetCast(showId);
            var scores = cast.ToDictionary(x => x, x => CalcLevenshteinDistance(x.Person.Name, actorName));
            return scores.Any(s => s.Value < 5) ? scores.Where(s => s.Value < 5).OrderBy(x => x.Value).First().Key : null;
        }

        #region Dispose

        // Flag: Has Dispose already been called?
        bool _disposed;
        // Instantiate a SafeHandle instance.
        readonly SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _handle.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            _disposed = true;
        }

        #endregion
    }
}

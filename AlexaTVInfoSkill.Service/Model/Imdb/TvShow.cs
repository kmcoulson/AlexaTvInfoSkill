using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AlexaTVInfoSkill.Service.Model.TvMaze;

namespace AlexaTVInfoSkill.Service.Model.Imdb
{

    [DataContract]
    public class TvShow : ITvShow
    {
        [DataMember(Name = "idIMDB")]
        public string Id { get; set; }

        [DataMember(Name = "title")]
        public string Name { get; set; }

        public string[] NameParts => Name.ToLower().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        
        public string Language {
            get { return Languages.FirstOrDefault(); }
            set { Languages = new[] {value}; }
        }

        [DataMember(Name = "languages")]
        public string[] Languages { get; set; }

        [DataMember(Name = "genres")]
        public string[] Genres { get; set; }

        public string Status { get; set; }

        [DataMember(Name = "runtime")]
        public string Runtime { get; set; }

        [DataMember(Name = "releaseDate")]
        public string Released { get; set; }

        public DateTime? StartDate => ImdbHelper.ParseDateTime(Released);

        [DataMember(Name = "plot")]
        public string Summary { get; set; }

        [DataMember(Name = "seasons")]
        public List<Season> Seasons { get; set; }

        public Links Links { get; set; }
        public Externals ExternalIds { get; set; }
    }

    [DataContract]
    public class Season
    {
        [DataMember(Name = "numSeason")]
        public int Number { get; set; }

        [DataMember(Name = "episodes")]
        public List<TvEpisode> Episodes { get; set; }
    }
}

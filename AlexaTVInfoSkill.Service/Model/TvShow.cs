using System;
using System.Linq;
using System.Runtime.Serialization;

namespace AlexaTVInfoSkill.Service.Model
{
    [DataContract]
    public class TvShow
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "url")]
        public Uri Url { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "language")]
        public string Language { get; set; }
        [DataMember(Name = "genres")]
        public string[] Genres { get; set; }
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "runtime")]
        public int? Runtime { get; set; }
        [DataMember(Name = "premiered")]
        public string Premiered { get; set; }

        private string _summary;
        [DataMember(Name = "summary")]
        public string Summary {
            get { return _summary; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _summary = value
                        .Replace("<br>", Environment.NewLine)
                        .Replace("<br />", Environment.NewLine)
                        .Replace("<br/>", Environment.NewLine)
                        .Replace("<p>", Environment.NewLine)
                        .Replace("</p>", "")
                        .Replace("<i>", "")
                        .Replace("</i>", "")
                        .Replace("<b>", "")
                        .Replace("</b>", "")
                        .Replace("<ul>", Environment.NewLine)
                        .Replace("</ul>", Environment.NewLine)
                        .Replace("<li>", "")
                        .Replace("</li>", Environment.NewLine)
                        .Replace("<div>", "")
                        .Replace("<em>", "")
                        .Replace("</em>", "")
                        .Replace("<em/>", "")
                        .Replace("<strong>", "")
                        .Replace("</strong>", "");
                }
            }
        }

        [DataMember(Name = "_links")]
        public Links Links { get; set; }
    }

    [DataContract]
    public class Links
    {
        [DataMember(Name = "self")]
        public Link Self { get; set; }

        [DataMember(Name = "previousepisode")]
        public Link PreviousEpisode { get; set; }

        [DataMember(Name = "nextepisode")]
        public Link NextEpisode { get; set; }
    }

    [DataContract]
    public class Link
    {
        public int? Id => GetIdFromUrl(Href);

        [DataMember(Name = "href")]
        public string Href { get; set; }

        private static int? GetIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            var urlSplit = url.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            var idString = urlSplit.LastOrDefault();
            int id;
            if (int.TryParse(idString, out id)) return id;
            return null;
        }
    }
}

using System;
using AlexaTVInfoSkill.Service.Model.TvMaze;

namespace AlexaTVInfoSkill.Service.Model
{
    public interface ITvShow
    {
        string Id { get; set; }
        string Name { get; set; }
        string[] NameParts { get; }
        string Language { get; set; }
        string[] Genres { get; set; }
        string Status { get; set; }
        string Runtime { get; set; }
        DateTime? StartDate { get; }
        string Summary { get; set; }
        Links Links { get; set; }
        Externals ExternalIds { get; set; }
    }
}
namespace Announcarr.Clients.Sonarr.Responses;

public class AlternateTitleResource
{
    public string? Title { get; set; }
    public int? SeasonNumber { get; set; }
    public int? SceneSeasonNumber { get; set; }
    public string? SceneOrigin { get; set; }
    public string? Comment { get; set; }
}
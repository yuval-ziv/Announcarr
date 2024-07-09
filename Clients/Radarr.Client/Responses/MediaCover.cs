namespace Announcarr.Clients.Radarr.Responses;

public class MediaCover
{
    public MediaCoverTypes CoverType { get; set; }
    public string? Url { get; set; }
    public string? RemoteUrl { get; set; }
}
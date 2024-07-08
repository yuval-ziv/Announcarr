namespace Announcer.Clients.Radarr.Responses;

public class AddMovieOptions
{
    public bool IgnoreEpisodesWithFiles { get; set; }
    public bool IgnoreEpisodesWithoutFiles { get; set; }
    public MonitorTypes Monitor { get; set; }
    public bool SearchForMovie { get; set; }
    public AddMovieMethod AddMethod { get; set; }
}
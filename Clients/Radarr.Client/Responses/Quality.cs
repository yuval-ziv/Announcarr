namespace Announcer.Clients.Radarr.Responses;

public class Quality
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public QualitySource Source { get; set; }
    public int Resolution { get; set; }
    public Modifier Modifier { get; set; }
}
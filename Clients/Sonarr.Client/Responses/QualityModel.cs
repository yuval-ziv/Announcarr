namespace Announcer.Clients.Sonarr.Responses;

public class QualityModel
{
    public Quality Quality { get; set; } = new();
    public Revision Revision { get; set; } = new();
    public QualitySource Source { get; set; }
    public int Resolution { get; set; }
}
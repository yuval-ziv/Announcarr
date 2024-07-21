namespace Announcarr.Clients.Radarr.Responses;

public class QualityModel
{
    public Quality Quality { get; set; } = new();
    public Revision Revision { get; set; } = new();
}
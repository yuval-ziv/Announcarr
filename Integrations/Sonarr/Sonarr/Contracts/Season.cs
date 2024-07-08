namespace Announcer.Integrations.Sonarr.Contracts;

public class Season
{
    public int SeasonNumber { get; set; }
    public List<Episode> Episodes { get; set; } = [];
}
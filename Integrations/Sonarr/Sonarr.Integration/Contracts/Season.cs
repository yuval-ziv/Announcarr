namespace Announcarr.Integrations.Sonarr.Integration.Contracts;

public class Season
{
    public int SeasonNumber { get; set; }
    public List<Episode> Episodes { get; set; } = [];
}
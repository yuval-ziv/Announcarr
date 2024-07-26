using Announcarr.Abstractions.Contracts.Contracts;

namespace Announcarr.Integrations.Sonarr.Integration.Contracts;

public class SonarrCalendarItem : BaseCalendarItem
{
    public string? SeriesName { get; set; }
    public List<Season> Seasons { get; set; } = [];

    public override string GetCaption(string dateTimeFormat)
    {
        return
            $"{SeriesName}:{Environment.NewLine}{string.Join(Environment.NewLine, Seasons.Select(GetCaption))}{(ReleaseDate is not null ? $" - airs {ReleaseDate?.ToString(dateTimeFormat)}" : string.Empty)}";
    }

    private static string GetCaption(Season season)
    {
        return $"Season {season.SeasonNumber} - Episode{(season.Episodes.Count > 1 ? "s" : string.Empty)}: {string.Join(" + ", season.Episodes.Select(GetCaption))}";
    }

    private static string GetCaption(Episode episode)
    {
        return $"{episode.EpisodeTitle ?? "TBA"} ({episode.EpisodeNumber:00})";
    }
}
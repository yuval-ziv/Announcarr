using Announcarr.Abstractions.Contracts.Contracts;

namespace Announcarr.Integrations.Sonarr.Integration.Contracts;

public class NewlyMonitoredSeries : NewlyMonitoredItem
{
    public string? SeriesName { get; set; }
    public List<SeasonEpisodeCount> SeasonToAvailableEpisodesCount { get; set; } = [];

    public override string GetCaption(string dateTimeFormat)
    {
        return $"Started monitoring new series '{SeriesName}'{(StartedMonitoring is null ? string.Empty : $" on {StartedMonitoring?.ToString(dateTimeFormat)}")}{Environment.NewLine}" +
               GetAvailableEpisodesCaption(dateTimeFormat);
    }

    private string GetAvailableEpisodesCaption(string dateTimeFormat)
    {
        return string.Join(Environment.NewLine, SeasonToAvailableEpisodesCount.Where(x => x.HasAnyEpisodesAvailable).Select(x => x.GetCaption(dateTimeFormat)));
    }
}

public class SeasonEpisodeCount : ICaptionableItem
{
    public int SeasonNumber { get; set; }
    public int AvailableEpisodesCount { get; set; }
    public int TotalSeasonEpisodesCount { get; set; }
    public bool HasAnyEpisodesAvailable => AvailableEpisodesCount > 0;
    public bool HasAllEpisodesAvailable => AvailableEpisodesCount > 0 && AvailableEpisodesCount == TotalSeasonEpisodesCount;

    public string? GetCaption(string dateTimeFormat)
    {
        if (HasAllEpisodesAvailable)
        {
            return $"Season {SeasonNumber:00} has all {TotalSeasonEpisodesCount} episodes available";
        }

        if (HasAnyEpisodesAvailable)
        {
            return $"Season {SeasonNumber:00} is partially available with {AvailableEpisodesCount} episodes out of a total of {TotalSeasonEpisodesCount}";
        }

        return $"Season {SeasonNumber:00} is not available - none of a total of {TotalSeasonEpisodesCount} episodes is available";
    }
}
using Announcarr.Abstractions.Contracts;

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
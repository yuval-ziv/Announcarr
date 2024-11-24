using Announcarr.Abstractions.Contracts;

namespace Announcarr.Integrations.Sonarr.Integration.Contracts;

public class SeasonEpisodeCount : ICaptionableItem
{
    public int SeasonNumber { get; set; }
    public int AvailableEpisodesCount { get; set; }
    public int TotalSeasonEpisodesCount { get; set; }
    public bool HasAnyEpisodesAvailable => AvailableEpisodesCount > 0;
    public bool HasAllEpisodesAvailable => AvailableEpisodesCount > 0 && AvailableEpisodesCount == TotalSeasonEpisodesCount;

    public string GetCaption(string dateTimeFormat)
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
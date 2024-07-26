using Announcarr.Integrations.Abstractions.Responses;
using EnumsNET;

namespace Announcarr.Integrations.Radarr.Integration.Contracts;

public class NewlyMonitoredMovie : NewlyMonitoredItem
{
    public string? MovieName { get; set; }
    public required DateTimeOffset? ReleaseDate { get; set; }
    public ReleaseDateType ReleaseDateType { get; set; }
    public bool IsAvailable { get; set; }

    public override string GetCaption(string dateTimeFormat) =>
        $"Started monitoring new movie '{MovieName}'{(StartedMonitoring is null ? string.Empty : $" on {StartedMonitoring?.ToString(dateTimeFormat)}")}{Environment.NewLine}" +
        $"{(ReleaseDate is not null ? $" - {ReleaseDateType.AsString(EnumFormat.Description)} on {ReleaseDate?.ToString(dateTimeFormat)}" : string.Empty)}{Environment.NewLine}" +
        (IsAvailable ? "The movie is already available for streaming!" : "the movie is not available for streaming yet :(");
}
using Announcarr.Abstractions.Contracts;

namespace Announcarr.Services;

public interface IAnnouncarrService
{
    Task<ForecastContract> GetAllForecastItemsAsync(DateTimeOffset? start, DateTimeOffset? end, bool? export = false, CancellationToken cancellationToken = default);
    Task<SummaryContract> GetAllSummaryItemsAsync(DateTimeOffset? start, DateTimeOffset? end, bool? export = false, CancellationToken cancellationToken = default);
}
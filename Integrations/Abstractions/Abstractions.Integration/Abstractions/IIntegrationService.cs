using Announcarr.Abstractions.Contracts;

namespace Announcarr.Integrations.Abstractions.Integration.Abstractions;

public interface IIntegrationService
{
    bool IsEnabled { get; }
    string Name { get; }
    Task<ForecastContract> GetForecastAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    Task<SummaryContract> GetSummaryAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
}
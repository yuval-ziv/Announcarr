namespace Announcarr.Services;

public interface ITestExporterService
{
    Task<(bool IsSuccessful, string Message)> TestExportersAsync(string? exporterName, bool enabledOnly = true, CancellationToken cancellationToken = default);
}
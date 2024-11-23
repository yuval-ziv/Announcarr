using Announcarr.Exporters.Abstractions.Exporter.Interfaces;
using Announcarr.Utils.Extensions.String;

namespace Announcarr.Services;

public class TestExporterService : ITestExporterService
{
    private readonly List<IExporterService> _exporterServices;

    public TestExporterService(IEnumerable<IExporterService> exporterServices)
    {
        _exporterServices = exporterServices.ToList();
    }

    public async Task<(bool IsSuccessful, string Message)> TestExportersAsync(string? exporterName, bool enabledOnly = true, CancellationToken cancellationToken = default)
    {
        if (exporterName.IsNullOrEmpty())
        {
            await Task.WhenAll(_exporterServices.Where(exporter => !enabledOnly || exporter.IsEnabled).Select(exporterService => exporterService.TestExporterAsync(cancellationToken)));
            return (true, $"Ran a total of {_exporterServices.Count(exporter => !enabledOnly || exporter.IsEnabled)} exporters.");
        }

        IExporterService? selectedExporter = _exporterServices.FirstOrDefault(exporter => exporter.Name == exporterName);

        if (selectedExporter is null)
        {
            return (false, $"Couldn't find exporter named {exporterName}");
        }

        await selectedExporter.TestExporterAsync(cancellationToken);
        return (true, $"Ran exporter named {selectedExporter.Name}");
    }
}
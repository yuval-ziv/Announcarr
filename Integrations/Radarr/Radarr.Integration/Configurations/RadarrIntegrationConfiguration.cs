using Announcarr.Integrations.Abstractions.Interfaces;

namespace Announcarr.Integrations.Radarr.Integration.Configurations;

public class RadarrIntegrationConfiguration : BaseIntegrationConfiguration
{
    public bool IsEnabled { get; set; } = false;
    public string Url { get; set; } = "http://localhost:7878";
    public string? ApiKey { get; set; }
    public bool IgnoreCertificateValidation { get; set; } = false;
}
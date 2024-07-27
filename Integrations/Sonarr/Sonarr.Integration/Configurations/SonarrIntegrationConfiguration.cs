using Announcarr.Integrations.Abstractions.Integration.Abstractions;

namespace Announcarr.Integrations.Sonarr.Integration.Configurations;

public class SonarrIntegrationConfiguration : BaseIntegrationConfiguration
{
    public bool IsEnabled { get; set; } = false;
    public string Url { get; set; } = "http://localhost:8989";
    public string? ApiKey { get; set; }
    public bool IgnoreCertificateValidation { get; set; } = false;
    public bool IgnoreSeasonZero { get; set; } = true;
}
namespace Announcarr.Integrations.Sonarr.Integration.Configurations;

public class SonarrIntegrationConfiguration
{
    public bool IsEnabled { get; set; } = false;
    public string? Name { get; set; } = null;
    public string Url { get; set; } = "http://localhost:8989";
    public string? ApiKey { get; set; }
    public bool IgnoreCertificateValidation { get; set; } = false;
    public bool IgnoreSeasonZero { get; set; } = true;
}
namespace Announcarr.Integrations.Sonarr.Integration.Configurations;

public class SonarrIntegrationConfiguration
{
    public string Url { get; set; } = "http://localhost:8989";
    public string? ApiKey { get; set; }
    public bool IgnoreCertificateValidation { get; set; } = false;
}
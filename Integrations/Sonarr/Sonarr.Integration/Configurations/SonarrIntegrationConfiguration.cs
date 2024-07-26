﻿using Announcarr.Integrations.Abstractions.Interfaces;

namespace Announcarr.Integrations.Sonarr.Integration.Configurations;

public class SonarrIntegrationConfiguration : IIntegrationConfiguration
{
    public bool IsEnabled { get; set; } = false;
    public bool IsGetCalendarEnabled { get; set; } = true;
    public bool IsGetRecentlyAddedEnabled { get; set; } = true;
    public string? Name { get; set; } = null;
    public string Url { get; set; } = "http://localhost:8989";
    public string? ApiKey { get; set; }
    public bool IgnoreCertificateValidation { get; set; } = false;
    public bool IgnoreSeasonZero { get; set; } = true;
}
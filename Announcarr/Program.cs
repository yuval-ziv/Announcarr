using Announcarr.Configurations;
using Announcarr.Configurations.Validations;
using Announcarr.Exporters.Telegram.Configurations;
using Announcarr.Exporters.Telegram.Services;
using Announcarr.Exporters.Abstractions.Extensions.DependencyInjection;
using Announcarr.Integrations.Abstractions.Extensions.DependencyInjection;
using Announcarr.Integrations.Abstractions.Responses;
using Announcarr.Integrations.Radarr.Extensions.DependencyInjection.Validations;
using Announcarr.Integrations.Radarr.Integration.Configurations;
using Announcarr.Integrations.Radarr.Integration.Services;
using Announcarr.Integrations.Sonarr.Extensions.DependencyInjection.Validations;
using Announcarr.Integrations.Sonarr.Integration.Configurations;
using Announcarr.Integrations.Sonarr.Integration.Services;
using Announcarr.JsonConverters;
using Announcarr.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IValidateOptions<AnnouncarrConfiguration>, AnnouncarrConfigurationValidator>();
builder.Services.AddSingleton<IValidateOptions<SonarrIntegrationConfiguration>, SonarrServiceIntegrationConfigurationValidator>();
builder.Services.AddSingleton<IValidateOptions<RadarrIntegrationConfiguration>, RadarrServiceIntegrationConfigurationValidator>();

builder.Services.Configure<AnnouncarrConfiguration>(builder.Configuration.GetSection(AnnouncarrConfiguration.SectionName));

IConfigurationSection integrationsConfigurationSection = builder.Configuration.GetSection("Integrations");
builder.Services.Configure<SonarrIntegrationConfiguration>(integrationsConfigurationSection.GetSection("Sonarr.Integration"));
builder.Services.Configure<RadarrIntegrationConfiguration>(integrationsConfigurationSection.GetSection("Radarr.Integration"));

builder.Services.AddIntegration<SonarrIntegrationService>().WithIntegrationConfiguration<SonarrIntegrationConfiguration>();
builder.Services.AddIntegration<RadarrIntegrationService>().WithIntegrationConfiguration<RadarrIntegrationConfiguration>();

builder.Services.AddSingleton<ICalendarService, CalendarService>();

builder.Services.ConfigureHttpJsonOptions(options => { options.SerializerOptions.Converters.Add(new PolymorphicConverter<BaseCalendarItem>()); });

WebApplication app = builder.Build();

app.MapGet("/calendar",
    async (ICalendarService calendarService, [FromQuery(Name = "start")] DateTimeOffset? start, [FromQuery(Name = "end")] DateTimeOffset? end) =>
    Results.Ok((object?)await calendarService.GetAllCalendarItemsAsync(start, end)));

app.Run();
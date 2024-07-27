using Announcarr.Abstractions.Contracts.Contracts;
using Announcarr.Configurations;
using Announcarr.Configurations.Validations;
using Announcarr.Exporters.Abstractions.Exporter.Extensions.DependencyInjection;
using Announcarr.Exporters.Abstractions.Exporter.Interfaces;
using Announcarr.Exporters.Telegram.Exporter.Configurations;
using Announcarr.Exporters.Telegram.Exporter.Services;
using Announcarr.Integrations.Abstractions.Integration.Extensions.DependencyInjection;
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
using Telegram.Extensions.DependencyInjection.Validations;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IValidateOptions<AnnouncarrConfiguration>, AnnouncarrConfigurationValidator>();
builder.Services.AddSingleton<IValidateOptions<List<SonarrIntegrationConfiguration>>, SonarrServiceIntegrationConfigurationValidator>();
builder.Services.AddSingleton<IValidateOptions<List<RadarrIntegrationConfiguration>>, RadarrServiceIntegrationConfigurationValidator>();
builder.Services.AddSingleton<IValidateOptions<List<TelegramExporterConfiguration>>, TelegramExporterConfigurationValidator>();

builder.Services.Configure<AnnouncarrConfiguration>(builder.Configuration.GetSection(AnnouncarrConfiguration.SectionName));

IConfigurationSection integrationsConfigurationSection = builder.Configuration.GetSection("Integrations");
builder.Services.Configure<List<SonarrIntegrationConfiguration>>(integrationsConfigurationSection.GetSection("Sonarr"));
builder.Services.Configure<List<RadarrIntegrationConfiguration>>(integrationsConfigurationSection.GetSection("Radarr"));

IConfigurationSection exportersConfigurationSection = builder.Configuration.GetSection("Exporters");
builder.Services.Configure<List<TelegramExporterConfiguration>>(exportersConfigurationSection.GetSection("Telegram"));

builder.Services.AddIntegrations<SonarrIntegrationService, SonarrIntegrationConfiguration>();
builder.Services.AddIntegrations<RadarrIntegrationService, RadarrIntegrationConfiguration>();

builder.Services.AddExporters<TelegramExporterService, TelegramExporterConfiguration>();

builder.Services.AddSingleton<ICalendarService, CalendarService>();

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new PolymorphicConverter<BaseCalendarItem>()));
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new PolymorphicConverter<NewlyMonitoredItem>()));

WebApplication app = builder.Build();

app.MapGet("/calendar",
    async (ICalendarService calendarService, [FromQuery(Name = "start")] DateTimeOffset? start, [FromQuery(Name = "end")] DateTimeOffset? end, [FromQuery(Name = "export")] bool? export) =>
    Results.Ok((object?)await calendarService.GetAllCalendarItemsAsync(start, end, export)));

app.MapGet("/recentlyAdded",
    async (ICalendarService calendarService, [FromQuery(Name = "start")] DateTimeOffset? start, [FromQuery(Name = "end")] DateTimeOffset? end, [FromQuery(Name = "export")] bool? export) =>
    Results.Ok((object?)await calendarService.GetAllRecentlyAddedItemsAsync(start, end, export)));

app.MapGet("/testExporters", async (IEnumerable<IExporterService> exporterServices) => await Task.WhenAll(exporterServices.Select(exporterService => exporterService.TestExporterAsync())));

app.Run();
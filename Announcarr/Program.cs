using Announcarr.Configurations;
using Announcarr.Configurations.Validations;
using Announcarr.JsonConverters;
using Announcarr.Services;
using Announcer.Integrations.Abstractions.Extensions.DependencyInjection;
using Announcer.Integrations.Abstractions.Responses;
using Announcer.Integrations.Radarr.Configurations;
using Announcer.Integrations.Radarr.Extensions.DependencyInjection.Validations;
using Announcer.Integrations.Radarr.Services;
using Announcer.Integrations.Sonarr.Configurations;
using Announcer.Integrations.Sonarr.Extensions.DependencyInjection.Validations;
using Announcer.Integrations.Sonarr.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IValidateOptions<AnnouncarrConfiguration>, AnnouncarrConfigurationValidator>();
builder.Services.AddSingleton<IValidateOptions<SonarrIntegrationConfiguration>, SonarrServiceIntegrationConfigurationValidator>();
builder.Services.AddSingleton<IValidateOptions<RadarrIntegrationConfiguration>, RadarrServiceIntegrationConfigurationValidator>();

builder.Services.Configure<AnnouncarrConfiguration>(builder.Configuration.GetSection(AnnouncarrConfiguration.SectionName));

IConfigurationSection serviceIntegrationConfigurationSection = builder.Configuration.GetSection("Integrations");
builder.Services.Configure<SonarrIntegrationConfiguration>(serviceIntegrationConfigurationSection.GetSection("Sonarr"));
builder.Services.Configure<RadarrIntegrationConfiguration>(serviceIntegrationConfigurationSection.GetSection("Radarr"));

builder.Services.AddIntegration<SonarrIntegrationService>().WithConfiguration<SonarrIntegrationConfiguration>();
builder.Services.AddIntegration<RadarrIntegrationService>().WithConfiguration<RadarrIntegrationConfiguration>();

builder.Services.AddSingleton<ICalendarService, CalendarService>();

builder.Services.ConfigureHttpJsonOptions(options => { options.SerializerOptions.Converters.Add(new PolymorphicConverter<BaseCalendarItem>()); });

WebApplication app = builder.Build();

app.MapGet("/calendar",
    async (ICalendarService calendarService, [FromQuery(Name = "start")] DateTimeOffset? start, [FromQuery(Name = "end")] DateTimeOffset? end) =>
    Results.Ok((object?)await calendarService.GetAllCalendarItemsAsync(start, end)));

app.Run();
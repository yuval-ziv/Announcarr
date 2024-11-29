using System.Diagnostics;
using Announcarr.Abstractions.Contracts;
using Announcarr.Configurations;
using Announcarr.Configurations.Validations;
using Announcarr.Exporters.Abstractions.Exporter.Extensions.DependencyInjection;
using Announcarr.Exporters.Telegram.Exporter.Configurations;
using Announcarr.Exporters.Telegram.Exporter.Services;
using Announcarr.HostedServices;
using Announcarr.Integrations.Abstractions.Integration.Extensions.DependencyInjection;
using Announcarr.Integrations.Radarr.Extensions.DependencyInjection.Validations;
using Announcarr.Integrations.Radarr.Integration.Configurations;
using Announcarr.Integrations.Radarr.Integration.Services;
using Announcarr.Integrations.Sonarr.Extensions.DependencyInjection.Validations;
using Announcarr.Integrations.Sonarr.Integration.Configurations;
using Announcarr.Integrations.Sonarr.Integration.Services;
using Announcarr.JsonConverters;
using Announcarr.Scheduler;
using Announcarr.Services;
using Announcarr.Webhooks.Overseerr.Extensions.Configurations;
using Announcarr.Webhooks.Overseerr.Extensions.DependencyInjection;
using Announcarr.Webhooks.Overseerr.Extensions.DependencyInjection.Validations;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Extensions.DependencyInjection.Validations;

namespace Announcarr.Utils.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddAnnouncarrServices(this WebApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;
        ConfigurationManager configuration = builder.Configuration;

        AddSerilogLogging(builder);
        AddControllers(services);
        AddProblemDetails(services);
        AddValidations(services);
        AddConfigurations(services, configuration);
        AddServices(services);
        AddAnnouncarrLogic(services);
        services.AddHostedService<AnnouncarrHostedService>();
    }

    private static void AddSerilogLogging(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
    }

    private static void AddControllers(IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new PolymorphicConverter<BaseItem>()));
        services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new PolymorphicConverter<NewlyMonitoredItem>()));
        services.AddControllers();
        services.AddOpenApi();
    }

    private static void AddProblemDetails(IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;

                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            };
        });
    }

    private static void AddValidations(IServiceCollection services)
    {
        services.AddSingleton<IValidateOptions<AnnouncarrConfiguration>, AnnouncarrConfigurationValidator>();
        services.AddSingleton<IValidateOptions<List<SonarrIntegrationConfiguration>>, SonarrServiceIntegrationConfigurationValidator>();
        services.AddSingleton<IValidateOptions<List<RadarrIntegrationConfiguration>>, RadarrServiceIntegrationConfigurationValidator>();
        services.AddSingleton<IValidateOptions<List<TelegramExporterConfiguration>>, TelegramExporterConfigurationValidator>();
        services.AddSingleton<IValidateOptions<List<OverseerrConfiguration>>, OverseerrConfigurationValidator>();
    }

    private static void AddConfigurations(IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<AnnouncarrConfiguration>(configuration.GetSection(AnnouncarrConfiguration.SectionName));

        IConfigurationSection integrationsConfigurationSection = configuration.GetSection("Integrations");
        services.Configure<List<SonarrIntegrationConfiguration>>(integrationsConfigurationSection.GetSection("Sonarr"));
        services.Configure<List<RadarrIntegrationConfiguration>>(integrationsConfigurationSection.GetSection("Radarr"));

        IConfigurationSection exportersConfigurationSection = configuration.GetSection("Exporters");
        services.Configure<List<TelegramExporterConfiguration>>(exportersConfigurationSection.GetSection("Telegram"));

        IConfigurationSection webhooksConfigurationSection = configuration.GetSection("Webhooks");
        services.Configure<List<OverseerrConfiguration>>(webhooksConfigurationSection.GetSection("Overseerr"));
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IAnnouncarrService, AnnouncarrService>();
        services.AddSingleton<ITestExporterService, TestExporterService>();
        services.AddSingleton<IAnnouncarrScheduler, AnnouncarrScheduler>();
    }

    private static void AddAnnouncarrLogic(IServiceCollection services)
    {
        services.AddIntegrations<SonarrIntegrationService, SonarrIntegrationConfiguration>();
        services.AddIntegrations<RadarrIntegrationService, RadarrIntegrationConfiguration>();

        services.AddExporters<TelegramExporterService, TelegramExporterConfiguration>();

        services.AddDefaultOverseerrWebhookHandlers();
    }
}
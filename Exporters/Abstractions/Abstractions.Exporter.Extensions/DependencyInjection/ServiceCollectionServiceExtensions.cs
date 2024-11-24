using System.Diagnostics.CodeAnalysis;
using Announcarr.Exporters.Abstractions.Exporter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Announcarr.Exporters.Abstractions.Exporter.Extensions.DependencyInjection;

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection AddExporters<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation, TConfiguration>(this IServiceCollection services)
        where TImplementation : class, IExporterService
        where TConfiguration : BaseExporterConfiguration
    {
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        List<TConfiguration> list = serviceProvider.GetService<IOptions<List<TConfiguration>>>()?.Value ?? [];

        return services.AddExporters<TImplementation, TConfiguration>(list);
    }

    public static IServiceCollection AddExporters<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation, TConfiguration>(this IServiceCollection services,
        List<TConfiguration>? configurations)
        where TImplementation : class, IExporterService
        where TConfiguration : BaseExporterConfiguration
    {
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger<TImplementation>>();

        foreach (TConfiguration configuration in configurations ?? Enumerable.Empty<TConfiguration>())
        {
            services.AddExporter<TImplementation, TConfiguration>(configuration, logger);
        }

        return services;
    }

    private static void AddExporter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation, TConfiguration>(this IServiceCollection services,
        TConfiguration configuration, bool includeLogger = true)
        where TImplementation : class, IExporterService
        where TConfiguration : BaseExporterConfiguration
    {
        if (!includeLogger)
        {
            services.AddSingleton<IExporterService, TImplementation>(_ => (TImplementation)Activator.CreateInstance(typeof(TImplementation), configuration)!);
        }
        else
        {
            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<TImplementation>>();
            services.AddSingleton<IExporterService, TImplementation>(_ => (TImplementation)Activator.CreateInstance(typeof(TImplementation), logger, configuration)!);
        }
    }

    private static void AddExporter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation, TConfiguration>(this IServiceCollection services,
        TConfiguration configuration, ILogger<TImplementation>? logger)
        where TImplementation : class, IExporterService
        where TConfiguration : BaseExporterConfiguration
    {
        if (logger is null)
        {
            services.AddSingleton<IExporterService, TImplementation>(_ => (TImplementation)Activator.CreateInstance(typeof(TImplementation), configuration)!);
        }
        else
        {
            services.AddSingleton<IExporterService, TImplementation>(_ => (TImplementation)Activator.CreateInstance(typeof(TImplementation), logger, configuration)!);
        }
    }
}
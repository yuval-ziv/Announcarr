using System.Diagnostics.CodeAnalysis;
using Announcarr.Exporters.Abstractions.Exporter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Announcarr.Exporters.Abstractions.Exporter.Extensions.DependencyInjection;

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection AddExporters<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation, TConfiguration>(this IServiceCollection services)
        where TImplementation : class, IExporterService
        where TConfiguration : class, IExporterConfiguration
    {
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        List<TConfiguration> list = serviceProvider.GetService<IOptions<List<TConfiguration>>>()?.Value ?? [];

        return services.AddExporters<TImplementation, TConfiguration>(list);
    }

    public static IServiceCollection AddExporters<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation, TConfiguration>(this IServiceCollection services,
        List<TConfiguration>? configurations)
        where TImplementation : class, IExporterService
        where TConfiguration : class, IExporterConfiguration
    {
        foreach (TConfiguration configuration in configurations ?? Enumerable.Empty<TConfiguration>())
        {
            services.AddExporter<TImplementation, TConfiguration>(configuration);
        }

        return services;
    }

    private static IServiceCollection AddExporter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation, TConfiguration>(this IServiceCollection services,
        TConfiguration configuration)
        where TImplementation : class, IExporterService
        where TConfiguration : class, IExporterConfiguration
    {
        return services.AddSingleton<IExporterService, TImplementation>(_ => (TImplementation)Activator.CreateInstance(typeof(TImplementation), configuration)!);
    }
}
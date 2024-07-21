using System.Diagnostics.CodeAnalysis;
using Announcarr.Exporters.Abstractions.Exporter.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Announcarr.Exporters.Abstractions.Exporter.Extensions.DependencyInjection;

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection AddExporter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection services)
        where TImplementation : class, IExporterService
    {
        services.AddSingleton<IExporterService, TImplementation>();
        return services;
    }

    public static IServiceCollection WithExporterConfiguration<TConfiguration>(this IServiceCollection services) where TConfiguration : class, new()
    {
        services.WithExporterConfiguration(provider => provider.GetService<IOptions<TConfiguration>>()?.Value ?? new TConfiguration());
        return services;
    }

    public static IServiceCollection WithExporterConfiguration<TConfiguration>(this IServiceCollection services, TConfiguration configuration) where TConfiguration : class
    {
        services.AddSingleton(configuration);
        return services;
    }

    public static IServiceCollection WithExporterConfiguration<TConfiguration>(this IServiceCollection services, Func<IServiceProvider, TConfiguration> configurationFactory)
        where TConfiguration : class
    {
        services.AddSingleton(typeof(TConfiguration), configurationFactory);
        return services;
    }
}
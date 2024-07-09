using System.Diagnostics.CodeAnalysis;
using Announcarr.Integrations.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Announcarr.Integrations.Abstractions.Integration.Extensions.DependencyInjection;

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection AddIntegration<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection services)
        where TImplementation : class, IIntegrationService
    {
        services.AddSingleton<IIntegrationService, TImplementation>();
        return services;
    }

    public static IServiceCollection WithIntegrationConfiguration<TConfiguration>(this IServiceCollection services) where TConfiguration : class, new()
    {
        services.WithIntegrationConfiguration(provider => provider.GetService<IOptions<TConfiguration>>()?.Value ?? new TConfiguration());
        return services;
    }

    public static IServiceCollection WithIntegrationConfiguration<TConfiguration>(this IServiceCollection services, TConfiguration configuration) where TConfiguration : class
    {
        services.AddSingleton(configuration);
        return services;
    }

    public static IServiceCollection WithIntegrationConfiguration<TConfiguration>(this IServiceCollection services, Func<IServiceProvider, TConfiguration> configurationFactory)
        where TConfiguration : class
    {
        services.AddSingleton(typeof(TConfiguration), configurationFactory);
        return services;
    }
}
using System.Diagnostics.CodeAnalysis;
using Announcer.Integrations.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Announcer.Integrations.Abstractions.Extensions.DependencyInjection;

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection AddIntegration<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection services)
        where TImplementation : class, IIntegrationService
    {
        services.AddSingleton<IIntegrationService, TImplementation>();
        return services;
    }

    public static IServiceCollection WithConfiguration<TConfiguration>(this IServiceCollection services) where TConfiguration : class, new()
    {
        services.WithConfiguration(provider => provider.GetService<IOptions<TConfiguration>>()?.Value ?? new TConfiguration());
        return services;
    }

    public static IServiceCollection WithConfiguration<TConfiguration>(this IServiceCollection services, TConfiguration configuration) where TConfiguration : class
    {
        services.AddSingleton(configuration);
        return services;
    }

    public static IServiceCollection WithConfiguration<TConfiguration>(this IServiceCollection services, Func<IServiceProvider, TConfiguration> configurationFactory)
        where TConfiguration : class
    {
        services.AddSingleton(typeof(TConfiguration), configurationFactory);
        return services;
    }
}
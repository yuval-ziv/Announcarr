using System.Diagnostics.CodeAnalysis;
using Announcarr.Integrations.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Announcarr.Integrations.Abstractions.Integration.Extensions.DependencyInjection;

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection AddIntegrations<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation, TConfiguration>(this IServiceCollection services)
        where TImplementation : class, IIntegrationService
        where TConfiguration : class, IIntegrationConfiguration
    {
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        List<TConfiguration> list = serviceProvider.GetService<IOptions<List<TConfiguration>>>()?.Value ?? [];

        return services.AddIntegrations<TImplementation, TConfiguration>(list);
    }

    public static IServiceCollection AddIntegrations<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation, TConfiguration>(this IServiceCollection services,
        List<TConfiguration>? configurations)
        where TImplementation : class, IIntegrationService
        where TConfiguration : class, IIntegrationConfiguration
    {
        foreach (TConfiguration configuration in configurations ?? Enumerable.Empty<TConfiguration>())
        {
            services.AddIntegration<TImplementation, TConfiguration>(configuration);
        }

        return services;
    }

    private static void AddIntegration<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation, TConfiguration>(this IServiceCollection services,
        TConfiguration configuration)
        where TImplementation : class, IIntegrationService
        where TConfiguration : class, IIntegrationConfiguration
    {
        services.AddSingleton<IIntegrationService, TImplementation>(_ => (TImplementation)Activator.CreateInstance(typeof(TImplementation), configuration)!);
    }
}
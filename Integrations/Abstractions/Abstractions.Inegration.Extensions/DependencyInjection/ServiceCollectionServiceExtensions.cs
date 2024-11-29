using System.Diagnostics.CodeAnalysis;
using Announcarr.Integrations.Abstractions.Integration.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Announcarr.Integrations.Abstractions.Integration.Extensions.DependencyInjection;

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection AddIntegrations<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation, TConfiguration>(this IServiceCollection services)
        where TImplementation : class, IIntegrationService
        where TConfiguration : BaseIntegrationConfiguration
    {
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        List<TConfiguration> list = serviceProvider.GetService<IOptions<List<TConfiguration>>>()?.Value ?? [];

        return services.AddIntegrations<TImplementation, TConfiguration>(list);
    }

    public static IServiceCollection AddIntegrations<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation, TConfiguration>(this IServiceCollection services,
        List<TConfiguration>? configurations)
        where TImplementation : class, IIntegrationService
        where TConfiguration : BaseIntegrationConfiguration
    {
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger<TImplementation>>();

        foreach (TConfiguration configuration in configurations ?? Enumerable.Empty<TConfiguration>())
        {
            services.AddIntegration<TImplementation, TConfiguration>(configuration, logger);
        }

        return services;
    }

    private static void AddIntegration<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation, TConfiguration>(this IServiceCollection services,
        TConfiguration configuration, bool includeLogger = true)
        where TImplementation : class, IIntegrationService
        where TConfiguration : BaseIntegrationConfiguration
    {
        if (!includeLogger)
        {
            services.AddSingleton<IIntegrationService, TImplementation>(_ => (TImplementation)Activator.CreateInstance(typeof(TImplementation), configuration)!);
        }
        else
        {
            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<TImplementation>>();
            services.AddSingleton<IIntegrationService, TImplementation>(_ => (TImplementation)Activator.CreateInstance(typeof(TImplementation), logger, configuration)!);
        }
    }

    private static void AddIntegration<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation, TConfiguration>(this IServiceCollection services,
        TConfiguration configuration, ILogger<TImplementation>? logger)
        where TImplementation : class, IIntegrationService
        where TConfiguration : BaseIntegrationConfiguration
    {
        if (logger is null)
        {
            services.AddSingleton<IIntegrationService, TImplementation>(_ => (TImplementation)Activator.CreateInstance(typeof(TImplementation), configuration)!);
        }
        else
        {
            services.AddSingleton<IIntegrationService, TImplementation>(_ => (TImplementation)Activator.CreateInstance(typeof(TImplementation), logger, configuration)!);
        }
    }
}
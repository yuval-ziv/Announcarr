using Announcarr.Webhooks.Overseerr.Extensions.Configurations;
using Announcarr.Webhooks.Overseerr.Extensions.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Announcarr.Webhooks.Overseerr.Extensions.DependencyInjection;

public static class ApplicationBuilderExtensions
{
    public static void UseOverseerrWebhooks(this IApplicationBuilder app, string path)
    {
        app.UseOverseerrWebhooks(new OverseerrConfiguration { Path = path });
    }

    public static void UseOverseerrWebhooks(this IApplicationBuilder app, HttpMethod method)
    {
        app.UseOverseerrWebhooks(new OverseerrConfiguration { Method = method.Method });
    }

    public static void UseOverseerrWebhooks(this IApplicationBuilder app, HttpMethod method, string path)
    {
        app.UseOverseerrWebhooks(new OverseerrConfiguration { Method = method.Method, Path = path });
    }

    public static void UseOverseerrWebhooks(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        List<OverseerrConfiguration>? configurations = scope.ServiceProvider.GetRequiredService<IOptions<List<OverseerrConfiguration>>>().Value;

        if (configurations is null)
        {
            OverseerrConfiguration overseerrConfiguration = scope.ServiceProvider.GetRequiredService<IOptions<OverseerrConfiguration>>().Value;

            if (overseerrConfiguration is not null)
            {
                configurations = [overseerrConfiguration];
            }
        }

        app.UseOverseerrWebhooks(configurations);
    }

    public static void UseOverseerrWebhooks(this IApplicationBuilder app, List<OverseerrConfiguration>? configurations)
    {
        configurations?.ForEach(app.UseOverseerrWebhooks);
    }

    public static void UseOverseerrWebhooks(this IApplicationBuilder app, OverseerrConfiguration configuration)
    {
        app.UseMiddleware<OverseerrMiddleware>(configuration);
    }
}
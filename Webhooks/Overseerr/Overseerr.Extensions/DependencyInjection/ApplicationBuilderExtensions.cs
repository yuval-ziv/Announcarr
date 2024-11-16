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
        OverseerrConfiguration configuration;
        using (IServiceScope scope = app.ApplicationServices.CreateScope())
        {
            configuration = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<OverseerrConfiguration>>().Value;
        }

        app.UseOverseerrWebhooks(configuration);
    }

    public static void UseOverseerrWebhooks(this IApplicationBuilder app, OverseerrConfiguration configuration)
    {
        app.UseMiddleware<OverseerrMiddleware>(configuration);
    }
}
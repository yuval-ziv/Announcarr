using Announcarr.Middlewares;
using Announcarr.Utils.Extensions;
using Announcarr.Webhooks.Overseerr.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddAnnouncarrServices();

WebApplication app = builder.Build();

app.UseOverseerrWebhooks();
app.UseMiddleware<RequestLoggingMiddleware>();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
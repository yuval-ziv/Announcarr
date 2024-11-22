using System.Net;
using System.Text;
using Announcarr.Utils.Extensions.String;
using Announcarr.Webhooks.Overseerr.Extensions.Configurations;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Services;
using Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Announcarr.Webhooks.Overseerr.Extensions.Middlewares;

public class OverseerrMiddleware
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy(),
        },
        Converters = [new CaseAndHumpInsensitiveStringEnumConverter()],
    };

    private readonly OverseerrConfiguration _configuration;
    private readonly ILogger<OverseerrMiddleware> _logger;
    private readonly RequestDelegate _next;

    public OverseerrMiddleware(RequestDelegate next, ILogger<OverseerrMiddleware> logger, OverseerrConfiguration? configuration = null)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration ?? new OverseerrConfiguration();

        if (_configuration.AuthorizationHeader.IsNullOrEmpty())
        {
            _logger.LogDebug("Starting Overseerr webhook middleware on method {HttpMethod} and path {Path}", _configuration.Method, _configuration.Path);
            return;
        }

        string obfuscatedAuthorizationHeader = GetObfuscatedAuthorizationHeader(_configuration.AuthorizationHeader);

        _logger.LogDebug("Starting Overseerr webhook middleware on method {HttpMethod} and path {Path} with authorization header {AuthorizationHeader}", _configuration.Method, _configuration.Path,
            obfuscatedAuthorizationHeader);
    }

    private string GetObfuscatedAuthorizationHeader(string authorizationHeader)
    {
        var amountOfCharactersToKeepInTheEnd = Convert.ToInt32(Math.Min(authorizationHeader.Length * 0.2, 6));

        return authorizationHeader.Obfuscate(keepLastCharacters: amountOfCharactersToKeepInTheEnd);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);
        if (ShouldNotHandleRequest(context))
        {
            return;
        }

        var webhookService = context.RequestServices.GetService<IWebhookService>();

        if (webhookService is null)
        {
            _logger.LogWarning("Got request to overseerr webhook on {HttpMethod} and path {Path}, but no webhook service registered", _configuration.Method, _configuration.Path);
            return;
        }

        if (!_configuration.AuthorizationHeader.IsNullOrEmpty() && context.Request.Headers["Authorization"] != _configuration.AuthorizationHeader)
        {
            _logger.LogWarning("Got request to overseerr webhook on {HttpMethod} and path {Path} with invalid authorization header", _configuration.Method, _configuration.Path);
            await SetResponseToUnauthorized(context);
            return;
        }

        string requestBody = await ReadRequestBody(context.Request);
        string normalizedRequestBody = NormalizeRequestFieldsToCamelCase(requestBody);
        var contract = JsonConvert.DeserializeObject<OverseerrWebhookContract>(normalizedRequestBody, JsonSerializerSettings);

        if (contract is null)
        {
            _logger.LogError("Got overseerr webhook request on {HttpMethod} and path {Path}, but body was either empty or malformed", _configuration.Method, _configuration.Path);
            return;
        }

        _logger.LogDebug("Handling overseerr webhook request on {HttpMethod} and path {Path} with notification type {NotificationType} ", _configuration.Method, _configuration.Path,
            contract.NotificationType);

        NotificationTypeConfiguration notificationTypeConfiguration = _configuration.NotificationTypeToConfiguration.GetValueOrDefault(contract.NotificationType, new NotificationTypeConfiguration());

        bool handlingResult = await webhookService.HandleAsync(contract, notificationTypeConfiguration.Tags, _configuration.OverseerrUrl, _configuration.Name,
            _configuration.IsEnabled && notificationTypeConfiguration.IsEnabled);

        await SetResponseToSuccess(context);

        _logger.LogDebug("Finished handling overseerr webhook request on {HttpMethod} and path {Path} with notification type {NotificationType} {Result}", _configuration.Method, _configuration.Path,
            contract.NotificationType, handlingResult ? "successfully" : "unsuccessfully");
    }

    /// <summary>
    ///     Not handling the request if either of the following conditions are met:
    ///     <list type="number">
    ///         <item>
    ///             <description>Webhook listener is not enabled</description>
    ///         </item>
    ///         <item>
    ///             <description>Request is from a remote ip and only local requests are allowed</description>
    ///         </item>
    ///         <item>
    ///             <description>Request doesn't match webhook method and path</description>
    ///         </item>
    ///     </list>
    /// </summary>
    /// <param name="context">the current http request's context</param>
    /// <returns>true if the request should be ignored; false otherwise.</returns>
    private bool ShouldNotHandleRequest(HttpContext context)
    {
        return !_configuration.EnableWebhookListener || (!_configuration.AllowRemoteRequests && !IPAddress.IsLoopback(context.Connection.RemoteIpAddress)) ||
               !RequestingOverseerrWebhook(context.Request);
    }

    private bool RequestingOverseerrWebhook(HttpRequest request)
    {
        if (request.Method != _configuration.Method)
        {
            return false;
        }

        if (request.Path != _configuration.Path)
        {
            return false;
        }

        return true;
    }

    private static async Task SetResponseToUnauthorized(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Invalid Authorization Header");
    }

    private static async Task<string> ReadRequestBody(HttpRequest request)
    {
        request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadExactlyAsync(buffer);
        string requestBody = Encoding.UTF8.GetString(buffer);
        request.Body.Position = 0;
        return requestBody;
    }

    private static string NormalizeRequestFieldsToCamelCase(string requestBody)
    {
        JObject jObject = JObject.Parse(requestBody);

        NormalizeProperties(jObject);

        return jObject.ToString();
    }

    private static void NormalizeProperties(JObject jObject)
    {
        foreach (JProperty property in jObject.Properties().ToList())
        {
            RenamePropertyIfNeeded(jObject, property);
            HandleNestedProperties(property);
        }
    }

    private static void RenamePropertyIfNeeded(JObject jObject, JProperty property)
    {
        string newPropertyName = property.Name.ToCamelCase();
        if (newPropertyName != property.Name)
        {
            RenameProperty(jObject, property, newPropertyName);
        }
    }

    private static void RenameProperty(JObject jObject, JProperty property, string newName)
    {
        jObject.Add(newName, jObject.GetValue(property.Name));
        jObject.Remove(property.Name);
    }

    private static void HandleNestedProperties(JProperty property)
    {
        switch (property.Value)
        {
            case JObject nestedObject:
                NormalizeProperties(nestedObject);
                break;
            case JArray array:
                array.OfType<JObject>().ToList().ForEach(NormalizeProperties);
                break;
        }
    }

    private static async Task SetResponseToSuccess(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsync("Handled webhook request correctly");
    }
}
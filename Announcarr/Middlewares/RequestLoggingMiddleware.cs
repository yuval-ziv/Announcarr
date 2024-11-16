using System.Text;

namespace Announcarr.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string requestBody = await ReadRequestBody(context.Request);
        _logger.LogInformation("Request: {RequestPath} ({RequestMethod})\r\nBody: {RequestBody}", context.Request.Path, context.Request.Method, requestBody);

        await _next(context);
    }

    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadExactlyAsync(buffer);
        string requestBody = Encoding.UTF8.GetString(buffer);
        request.Body.Position = 0;
        return requestBody;
    }
}
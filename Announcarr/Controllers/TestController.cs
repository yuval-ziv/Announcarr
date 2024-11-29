using Announcarr.Abstractions.Contracts;
using Announcarr.Services;
using Microsoft.AspNetCore.Mvc;

namespace Announcarr.Controllers;

[Route("api/test")]
[ApiController]
public class TestController : Controller
{
    private readonly IAnnouncarrService _announcarrService;
    private readonly ITestExporterService _testExporterService;

    public TestController(IAnnouncarrService announcarrService, ITestExporterService testExporterService)
    {
        _announcarrService = announcarrService;
        _testExporterService = testExporterService;
    }

    [HttpGet("forecast")]
    public async Task<IActionResult> GetForecast([FromQuery(Name = "start")] DateTimeOffset? start, [FromQuery(Name = "end")] DateTimeOffset? end, [FromQuery(Name = "export")] bool? export)
    {
        ForecastContract result = await _announcarrService.GetAllForecastItemsAsync(start, end, export);
        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery(Name = "start")] DateTimeOffset? start, [FromQuery(Name = "end")] DateTimeOffset? end, [FromQuery(Name = "export")] bool? export)
    {
        SummaryContract result = await _announcarrService.GetAllSummaryItemsAsync(start, end, export);
        return Ok(result);
    }

    [HttpPost("exporters")]
    public async Task<IActionResult> TestAllExporters([FromQuery(Name = "exporterName")] string? exporterName, [FromQuery(Name = "enabledOnly")] bool enabledOnly = true)
    {
        (bool isSuccessful, string? message) = await _testExporterService.TestExportersAsync(exporterName, enabledOnly);

        return isSuccessful ? Ok(message) : BadRequest(message);
    }
}
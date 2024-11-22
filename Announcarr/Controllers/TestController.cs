using Announcarr.Abstractions.Contracts;
using Announcarr.Services;
using Microsoft.AspNetCore.Mvc;

namespace Announcarr.Controllers;

[Route("api/test")]
[ApiController]
public class TestController : Controller
{
    private readonly ICalendarService _calendarService;
    private readonly ITestExporterService _testExporterService;

    public TestController(ICalendarService calendarService, ITestExporterService testExporterService)
    {
        _calendarService = calendarService;
        _testExporterService = testExporterService;
    }

    [HttpGet("calendar")]
    public async Task<IActionResult> GetCalendar([FromQuery(Name = "start")] DateTimeOffset? start, [FromQuery(Name = "end")] DateTimeOffset? end, [FromQuery(Name = "export")] bool? export)
    {
        CalendarContract result = await _calendarService.GetAllCalendarItemsAsync(start, end, export);
        return Ok(result);
    }

    [HttpGet("recentlyAdded")]
    public async Task<IActionResult> GetRecentlyAdded([FromQuery(Name = "start")] DateTimeOffset? start, [FromQuery(Name = "end")] DateTimeOffset? end, [FromQuery(Name = "export")] bool? export)
    {
        RecentlyAddedContract result = await _calendarService.GetAllRecentlyAddedItemsAsync(start, end, export);
        return Ok(result);
    }

    [HttpPost("exporters")]
    public async Task<IActionResult> TestAllExporters([FromQuery(Name = "exporterName")] string? exporterName, [FromQuery(Name = "enabledOnly")] bool enabledOnly = true)
    {
        (bool isSuccessful, string? message) = await _testExporterService.TestExportersAsync(exporterName, enabledOnly);

        return isSuccessful ? Ok(message) : BadRequest(message);
    }
}
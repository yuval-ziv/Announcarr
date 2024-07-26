using Announcarr.Exporters.Abstractions.Exporter.AbstractImplementations;
using Announcarr.Exporters.Telegram.Configurations;
using Announcarr.Integrations.Abstractions.Responses;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Announcarr.Exporters.Telegram.Services;

public class TelegramExporterService : BaseExporterService
{
    private const string DefaultThumbnailNotAvailableUri = "https://thumbs.dreamstime.com/b/ning%C3%BAn-icono-disponible-de-la-imagen-plano-ejemplo-del-vector-132482953.jpg";

    private readonly TelegramExporterConfiguration _configuration;
    private readonly TelegramBotClient _bot;
    private readonly List<ChatId> _chatIds;

    public TelegramExporterService(TelegramExporterConfiguration configuration)
    {
        _configuration = configuration;
        _bot = new TelegramBotClient(_configuration.Bot?.Token ?? "");
        _chatIds = _configuration.Bot?.ChatIds.Select(chatId => new ChatId(chatId)).ToList() ?? [];
    }

    public override bool IsEnabled => _configuration.IsEnabled;

    public override string GetName => _configuration.Name ?? "Telegram";
    public override bool IsTestExporterEnabled => _configuration.IsTestExporterEnabled;

    protected override async Task TestExporterLogicAsync(CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendTextMessageAsync(chatId, "This is a test message.", cancellationToken: cancellationToken));
    }

    public override bool IsExportCalendarEnabled => _configuration.IsExportCalendarEnabled;

    protected override async Task ExportCalendarLogicAsync(CalendarResponse calendarResponse, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendTextMessageAsync(chatId,
            $"The calendar for {startDate.ToString(_configuration.DateTimeFormat)} to {endDate.ToString(_configuration.DateTimeFormat)} is:", cancellationToken: cancellationToken));
        await Task.WhenAll(calendarResponse.CalendarItems.Select(calendarItem => SendCalendarItemToAllChatsAsync(calendarItem, cancellationToken)));
    }

    public override bool IsExportRecentlyAddedEnabled => _configuration.IsExportRecentlyAddedEnabled;

    protected override async Task ExportRecentlyAddedLogicAsync(RecentlyAddedResponse recentlyAddedResponse, DateTimeOffset startDate, DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendTextMessageAsync(chatId,
            $"The recently monitored items for {startDate.ToString(_configuration.DateTimeFormat)} to {endDate.ToString(_configuration.DateTimeFormat)} are:", cancellationToken: cancellationToken));
        await Task.WhenAll(recentlyAddedResponse.NewlyMonitoredItems.Select(newMonitoredItem => SendNewMonitoredItemToAllChatsAsync(newMonitoredItem, cancellationToken)));
        await SendToAllChatsAsync(chatId => _bot.SendTextMessageAsync(chatId,
            $"The recently added items for {startDate.ToString(_configuration.DateTimeFormat)} to {endDate.ToString(_configuration.DateTimeFormat)} is:", cancellationToken: cancellationToken));
        await Task.WhenAll(recentlyAddedResponse.NewItems.Select(calendarItem => SendCalendarItemToAllChatsAsync(calendarItem, cancellationToken)));
    }

    private async Task SendCalendarItemToAllChatsAsync(BaseCalendarItem calendarItem, CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendPhotoAsync(chatId, new InputFileUrl(calendarItem.ThumbnailUrl ?? DefaultThumbnailNotAvailableUri),
            caption: calendarItem.GetCaption(_configuration.DateTimeFormat),
            cancellationToken: cancellationToken));
    }

    private async Task SendNewMonitoredItemToAllChatsAsync(NewlyMonitoredItem newlyMonitoredItem, CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendPhotoAsync(chatId, new InputFileUrl(newlyMonitoredItem.ThumbnailUrl ?? DefaultThumbnailNotAvailableUri),
            caption: newlyMonitoredItem.GetCaption(_configuration.DateTimeFormat),
            cancellationToken: cancellationToken));
    }

    private async Task SendToAllChatsAsync(Func<ChatId, Task<Message>> sendFunction)
    {
        await Task.WhenAll(_chatIds.Select(sendFunction));
    }
}
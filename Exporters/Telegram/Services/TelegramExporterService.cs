using Announcarr.Exporters.Abstractions.Exporter.Interfaces;
using Announcarr.Exporters.Telegram.Configurations;
using Announcarr.Integrations.Abstractions.Responses;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Announcarr.Exporters.Telegram.Services;

public class TelegramExporterService : IExporterService
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

    public bool IsEnabled() => _configuration.IsEnabled;

    public string GetName() => _configuration.Name ?? "Telegram";

    public async Task TestExporterAsync(CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendTextMessageAsync(chatId, "This is a test message.", cancellationToken: cancellationToken));
    }

    public async Task ExportCalendarAsync(CalendarResponse calendarResponse, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendTextMessageAsync(chatId,
            $"The calendar for {startDate.ToString(_configuration.DateTimeFormat)} to {endDate.ToString(_configuration.DateTimeFormat)} is:", cancellationToken: cancellationToken));
        await Task.WhenAll(calendarResponse.CalendarItems.Select(calendarItem => SendCalendarItemToAllChatsAsync(calendarItem, cancellationToken)));
    }

    private async Task SendCalendarItemToAllChatsAsync(BaseCalendarItem calendarItem, CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendPhotoAsync(chatId, new InputFileUrl(calendarItem.ThumbnailUrl ?? DefaultThumbnailNotAvailableUri),
            caption: calendarItem.GetCaption(_configuration.DateTimeFormat),
            cancellationToken: cancellationToken));
    }

    private async Task SendToAllChatsAsync(Func<ChatId, Task<Message>> sendFunction)
    {
        await Task.WhenAll(_chatIds.Select(sendFunction));
    }
}
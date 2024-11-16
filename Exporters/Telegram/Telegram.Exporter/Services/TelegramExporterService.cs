using Announcarr.Abstractions.Contracts;
using Announcarr.Exporters.Abstractions.Exporter.AbstractImplementations;
using Announcarr.Exporters.Abstractions.Exporter.Resolvers;
using Announcarr.Exporters.Telegram.Exporter.Configurations;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Announcarr.Exporters.Telegram.Exporter.Services;

public class TelegramExporterService : BaseExporterService<TelegramExporterConfiguration>
{
    private const string DefaultThumbnailNotAvailableUri = "https://thumbs.dreamstime.com/b/ning%C3%BAn-icono-disponible-de-la-imagen-plano-ejemplo-del-vector-132482953.jpg";
    private readonly TelegramBotClient _bot;
    private readonly List<ChatId> _chatIds;


    public TelegramExporterService(TelegramExporterConfiguration configuration) : base(configuration)
    {
        _bot = new TelegramBotClient(Configuration.Bot?.Token ?? "");
        _chatIds = Configuration.Bot?.ChatIds.Select(chatId => new ChatId(chatId)).ToList() ?? [];
    }

    public override bool IsEnabled => Configuration.IsEnabled;

    public override string Name => Configuration.Name ?? "Telegram";
    public override bool? ExportOnEmptyContract { get; set; }
    public override string? CustomMessageOnEmptyContract { get; set; }

    protected override async Task TestExporterLogicAsync(CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendMessage(chatId, "This is a test message.", cancellationToken: cancellationToken));
    }

    protected override async Task ExportCalendarLogicAsync(CalendarContract calendarContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendMessage(chatId,
            $"The calendar for {startDate.ToString(Configuration.DateTimeFormat)} to {endDate.ToString(Configuration.DateTimeFormat)} is:", cancellationToken: cancellationToken));
        await Task.WhenAll(calendarContract.CalendarItems.Select(calendarItem => SendCalendarItemToAllChatsAsync(calendarItem, cancellationToken)));
    }

    protected override async Task ExportEmptyCalendarLogicAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        string text = TextMessageResolver.ResolveTextMessage(CustomMessageOnEmptyContract, AnnouncementType.Calendar, startDate, endDate,
            Configuration.DateTimeFormat);
        await SendToAllChatsAsync(chatId => _bot.SendMessage(chatId, text, cancellationToken: cancellationToken));
    }

    protected override async Task ExportRecentlyAddedLogicAsync(RecentlyAddedContract recentlyAddedContract, DateTimeOffset startDate, DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendMessage(chatId,
            $"The recently monitored items for {startDate.ToString(Configuration.DateTimeFormat)} to {endDate.ToString(Configuration.DateTimeFormat)} are:", cancellationToken: cancellationToken));
        await Task.WhenAll(recentlyAddedContract.NewlyMonitoredItems.Select(newMonitoredItem => SendNewMonitoredItemToAllChatsAsync(newMonitoredItem, cancellationToken)));
        await SendToAllChatsAsync(chatId => _bot.SendMessage(chatId,
            $"The recently added items for {startDate.ToString(Configuration.DateTimeFormat)} to {endDate.ToString(Configuration.DateTimeFormat)} is:", cancellationToken: cancellationToken));
        await Task.WhenAll(recentlyAddedContract.NewItems.Select(calendarItem => SendCalendarItemToAllChatsAsync(calendarItem, cancellationToken)));
    }

    protected override async Task ExportEmptyRecentlyAddedLogicAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken)
    {
        string text = TextMessageResolver.ResolveTextMessage(CustomMessageOnEmptyContract, AnnouncementType.Calendar, startDate, endDate,
            Configuration.DateTimeFormat);
        await SendToAllChatsAsync(chatId => _bot.SendMessage(chatId, text, cancellationToken: cancellationToken));
    }

    protected override Task ExportAnnouncementLogicAsync(CustomAnnouncement message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private async Task SendCalendarItemToAllChatsAsync(BaseCalendarItem calendarItem, CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendPhoto(chatId, new InputFileUrl(calendarItem.ThumbnailUrl ?? DefaultThumbnailNotAvailableUri),
            calendarItem.GetCaption(Configuration.DateTimeFormat),
            cancellationToken: cancellationToken));
    }

    private async Task SendNewMonitoredItemToAllChatsAsync(NewlyMonitoredItem newlyMonitoredItem, CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendPhoto(chatId, new InputFileUrl(newlyMonitoredItem.ThumbnailUrl ?? DefaultThumbnailNotAvailableUri),
            newlyMonitoredItem.GetCaption(Configuration.DateTimeFormat),
            cancellationToken: cancellationToken));
    }

    private async Task SendToAllChatsAsync(Func<ChatId, Task<Message>> sendFunction)
    {
        await Task.WhenAll(_chatIds.Select(sendFunction));
    }
}
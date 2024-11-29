using System.Text;
using Announcarr.Abstractions.Contracts;
using Announcarr.Exporters.Abstractions.Exporter.AbstractImplementations;
using Announcarr.Exporters.Abstractions.Exporter.Resolvers;
using Announcarr.Exporters.Telegram.Exporter.Configurations;
using Announcarr.Utils.Extensions.String;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Announcarr.Exporters.Telegram.Exporter.Services;

public class TelegramExporterService : BaseExporterService<TelegramExporterConfiguration>
{
    private const string DefaultThumbnailNotAvailableUri = "https://thumbs.dreamstime.com/b/ning%C3%BAn-icono-disponible-de-la-imagen-plano-ejemplo-del-vector-132482953.jpg";
    private const int ThirtyMinutesInSeconds = 1800;
    private readonly TelegramBotClient _bot;
    private readonly List<ChatId> _chatIds;


    public TelegramExporterService(TelegramExporterConfiguration configuration) : this(null, configuration)
    {
    }

    public TelegramExporterService(ILogger<TelegramExporterService>? logger, TelegramExporterConfiguration configuration) : base(logger, configuration)
    {
        string? customTelegramBotApiServer = Configuration.Bot.CustomTelegramBotApiServer.IsNullOrWhiteSpace() ? null : Configuration.Bot.CustomTelegramBotApiServer;
        var telegramBotClientOptions = new TelegramBotClientOptions(Configuration.Bot.Token, customTelegramBotApiServer)
        {
            RetryCount = int.MaxValue,
            RetryThreshold = ThirtyMinutesInSeconds,
        };
        _bot = new TelegramBotClient(telegramBotClientOptions);
        _chatIds = Configuration.Bot.ChatIds.Select(chatId => new ChatId(chatId)).ToList();
    }

    public override bool IsEnabled => Configuration.IsEnabled;

    public override string Name => Configuration.Name ?? "Telegram";
    public override bool? ExportOnEmptyContract { get; set; }
    public override string? CustomMessageOnEmptyContract { get; set; }

    protected override async Task TestExporterLogicAsync(CancellationToken cancellationToken = default)
    {
        if (Configuration.IsEnabledByAnnouncementType(AnnouncementType.Test))
        {
            await ExportToAllChatsAsync(chatId => _bot.SendMessage(chatId, "This is a test message.", cancellationToken: cancellationToken));
        }
    }

    protected override async Task ExportForecastLogicAsync(ForecastContract forecastContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        var startDateString = startDate.ToString(Configuration.DateTimeFormat);
        var endDateString = endDate.ToString(Configuration.DateTimeFormat);

        await ExportToAllChatsAsync(chatId => _bot.SendMessage(chatId, $"The forecast for {startDateString} to {endDateString} is:", cancellationToken: cancellationToken));
        await ExportItemsAsync(forecastContract.Items, cancellationToken);
    }

    protected override async Task ExportEmptyForecastLogicAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        string text = TextMessageResolver.ResolveTextMessage(CustomMessageOnEmptyContract, AnnouncementType.Forecast, startDate, endDate, Configuration.DateTimeFormat);

        await ExportToAllChatsAsync(chatId => _bot.SendMessage(chatId, text, cancellationToken: cancellationToken));
    }

    protected override async Task ExportSummaryLogicAsync(SummaryContract summaryContract, DateTimeOffset startDate, DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        var startDateString = startDate.ToString(Configuration.DateTimeFormat);
        var endDateString = endDate.ToString(Configuration.DateTimeFormat);

        await ExportToAllChatsAsync(chatId => _bot.SendMessage(chatId, $"The summary of newly monitored items for {startDateString} to {endDateString} are:", cancellationToken: cancellationToken));
        await ExportItemsAsync(summaryContract.NewlyMonitoredItems, cancellationToken);

        await ExportToAllChatsAsync(chatId => _bot.SendMessage(chatId, $"The summary of new items for {startDateString} to {endDateString} is:", cancellationToken: cancellationToken));
        await ExportItemsAsync(summaryContract.NewItems, cancellationToken);
    }

    protected override Task ExportEmptySummaryLogicAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        string text = TextMessageResolver.ResolveTextMessage(CustomMessageOnEmptyContract, AnnouncementType.Forecast, startDate, endDate, Configuration.DateTimeFormat);
        return ExportToAllChatsAsync(chatId => _bot.SendMessage(chatId, text, cancellationToken: cancellationToken));
    }

    protected override Task ExportAnnouncementLogicAsync(CustomAnnouncement message, CancellationToken cancellationToken = default)
    {
        var messageBuilder = new StringBuilder();
        if (message.Title is not null)
        {
            string title = Markdown.Escape(message.Title);
            messageBuilder.AppendLine($"*{title}*");
        }

        if (message.Message is not null)
        {
            messageBuilder.AppendLine(Markdown.Escape(message.Message));
        }

        if (message.Link is not null && message.Link.IsValidUri())
        {
            messageBuilder.AppendLine();
            messageBuilder.Append(GetLinkText(message));
        }

        var text = messageBuilder.ToString();

        if (message.Image.IsNullOrWhiteSpace())
        {
            Logger?.LogDebug("Exporting custom announcement without photo");
            return ExportToAllChatsAsync(chatId => _bot.SendMessage(chatId, text, ParseMode.MarkdownV2, cancellationToken: cancellationToken));
        }

        var image = new InputFileUrl(message.Image);
        Logger?.LogDebug("Exporting custom announcement with photo");
        return ExportToAllChatsAsync(chatId => _bot.SendPhoto(chatId, image, text, ParseMode.MarkdownV2, cancellationToken: cancellationToken));
    }

    private async Task ExportItemsAsync<T>(List<T> items, CancellationToken cancellationToken = default) where T : ICaptionableItem, IThumbnailItem
    {
        Logger?.LogDebug("Exporting {AmountOfItems} items on exporter {ExporterName}", items.Count, Name);
        foreach (T[] itemsChunk in items.Chunk(10))
        {
            await ExportItemsChunkAsync(itemsChunk, cancellationToken);
        }
    }

    private async Task ExportItemsChunkAsync<T>(T[] itemsChunk, CancellationToken cancellationToken = default) where T : ICaptionableItem, IThumbnailItem
    {
        if (itemsChunk.Length == 1)
        {
            Logger?.LogDebug("Exporting chunk with only one item on exporter {ExporterName}, exporting as a captioned photo", Name);
            var image = new InputFileUrl(itemsChunk[0].GetThumbnailUri() ?? DefaultThumbnailNotAvailableUri);
            string? caption = itemsChunk[0].GetCaption(Configuration.DateTimeFormat);
            await ExportToAllChatsAsync(async chatId => await _bot.SendPhoto(chatId, image, caption: caption, cancellationToken: cancellationToken));

            return;
        }

        Logger?.LogDebug("Exporting chunk with {AmountOfItems} items on exporter {ExporterName}, exporting as captioned media group followed by full caption of chunk", itemsChunk.Length, Name);
        (List<InputMediaPhoto> newItemsMedia, string newItemsCaption) = GetMediaGroupAndCaption(itemsChunk);
        await ExportToAllChatsAsync(async chatId => await _bot.SendMediaGroup(chatId, newItemsMedia, cancellationToken: cancellationToken));
        await ExportToAllChatsAsync(async chatId => await _bot.SendMessage(chatId, newItemsCaption, cancellationToken: cancellationToken));
    }

    private (List<InputMediaPhoto> MediaGroup, string Caption) GetMediaGroupAndCaption<T>(T[] items) where T : ICaptionableItem, IThumbnailItem
    {
        List<InputMediaPhoto> mediaGroup = items.Select(ToInputMediaPhoto).ToList();
        string caption = string.Join(Environment.NewLine, items.Select(item => item.GetCaption(Configuration.DateTimeFormat)));

        return (mediaGroup, caption);
    }

    private InputMediaPhoto ToInputMediaPhoto<T>(T item) where T : ICaptionableItem, IThumbnailItem
    {
        return new InputMediaPhoto(new InputFileUrl(item.GetThumbnailUri() ?? DefaultThumbnailNotAvailableUri))
        {
            Caption = item.GetCaption(Configuration.DateTimeFormat),
        };
    }

    private static string GetLinkText(CustomAnnouncement message)
    {
        if (message.Link.IsUriWithPortNumber())
        {
            return $"See more here - {message.Link}";
        }

        return $"[See more here]({message.Link})";
    }

    private async Task ExportToAllChatsAsync(Func<ChatId, Task> exportFunction)
    {
        await Task.WhenAll(_chatIds.Select(exportFunction));
    }
}
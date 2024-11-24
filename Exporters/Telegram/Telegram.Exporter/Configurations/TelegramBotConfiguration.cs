namespace Announcarr.Exporters.Telegram.Exporter.Configurations;

public class TelegramBotConfiguration
{
    public required string Token { get; set; }
    public required List<long> ChatIds { get; set; } = [];
    public string? CustomTelegramBotApiServer { get; set; } = null; //https://github.com/tdlib/telegram-bot-api
}
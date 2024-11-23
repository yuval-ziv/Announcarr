namespace Announcarr.Exporters.Telegram.Exporter.Configurations;

public class TelegramBotConfiguration
{
    public required string Token { get; set; }
    public required List<long> ChatIds { get; set; } = [];
}
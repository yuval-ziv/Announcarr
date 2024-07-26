namespace Announcarr.Exporters.Telegram.Configurations;

public class TelegramExporterConfiguration
namespace Announcarr.Exporters.Telegram.Exporter.Configurations;

{
    public bool IsEnabled { get; set; } = false;
    public bool IsTestExporterEnabled { get; set; } = true;
    public bool IsExportCalendarEnabled { get; set; } = true;
    public bool IsExportRecentlyAddedEnabled { get; set; } = true;
    public string? Name { get; set; } = null;
    public string DateTimeFormat { get; set; } = "dd/MM/yyyy";
    public TelegramBotConfiguration? Bot { get; set; }
}

public class TelegramBotConfiguration
{
    public required string Token { get; set; }
    public required List<long> ChatIds { get; set; } = [];
}
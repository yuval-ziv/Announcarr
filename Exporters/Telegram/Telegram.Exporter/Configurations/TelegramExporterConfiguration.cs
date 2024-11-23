using Announcarr.Exporters.Abstractions.Exporter.Interfaces;

namespace Announcarr.Exporters.Telegram.Exporter.Configurations;

public class TelegramExporterConfiguration : BaseExporterConfiguration
{
    public bool IsEnabled { get; set; } = false;
    public bool IsTestExporterEnabled { get; set; } = true;
    public string DateTimeFormat { get; set; } = "dd/MM/yyyy";
    public required TelegramBotConfiguration Bot { get; set; }
}
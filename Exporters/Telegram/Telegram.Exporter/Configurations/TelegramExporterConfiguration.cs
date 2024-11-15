﻿using Announcarr.Exporters.Abstractions.Exporter.Interfaces;

namespace Announcarr.Exporters.Telegram.Exporter.Configurations;

public class TelegramExporterConfiguration : BaseExporterConfiguration
{
    public bool IsEnabled { get; set; } = false;
    public bool IsTestExporterEnabled { get; set; } = true;
    public string DateTimeFormat { get; set; } = "dd/MM/yyyy";
    public TelegramBotConfiguration? Bot { get; set; }
}

public class TelegramBotConfiguration
{
    public required string Token { get; set; }
    public required List<long> ChatIds { get; set; } = [];
}
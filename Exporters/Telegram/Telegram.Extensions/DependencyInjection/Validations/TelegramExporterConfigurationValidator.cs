using Announcarr.Exporters.Abstractions.Exporter.Extensions.DependencyInjection.Validations;
using Announcarr.Exporters.Telegram.Exporter.Configurations;
using Announcarr.Utils.Extensions.String;
using Microsoft.Extensions.Options;

namespace Announcarr.Exporters.Telegram.Extensions.DependencyInjection.Validations;

public class TelegramExporterConfigurationValidator : BaseExporterConfigurationValidator<TelegramExporterConfiguration>
{
    public override ValidateOptionsResult Validate(string? name, List<TelegramExporterConfiguration> allOptions)
    {
        TelegramExporterConfiguration? configurationWithoutBotToken = allOptions.FirstOrDefault(options => options.Bot?.Token.IsNullOrWhiteSpace() ?? false);
        if (configurationWithoutBotToken is not null)
        {
            return ValidateOptionsResult.Fail($"{nameof(TelegramExporterConfiguration.Bot.Token)} is required and cannot be empty or white space only (failed for {configurationWithoutBotToken.Name}");
        }

        return base.Validate(name, allOptions);
    }
}
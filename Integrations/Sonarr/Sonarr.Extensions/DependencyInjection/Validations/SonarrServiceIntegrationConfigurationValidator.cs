using Announcarr.Integrations.Abstractions.Integration.Extensions.DependencyInjection.Validations;
using Announcarr.Integrations.Sonarr.Integration.Configurations;
using Announcarr.Utils.Extensions.String;
using Microsoft.Extensions.Options;

namespace Announcarr.Integrations.Sonarr.Extensions.DependencyInjection.Validations;

public class SonarrServiceIntegrationConfigurationValidator : BaseServiceIntegrationConfigurationValidator<SonarrIntegrationConfiguration>
{
    public override ValidateOptionsResult Validate(string? name, List<SonarrIntegrationConfiguration> allOptions)
    {
        if (allOptions.Any(options => options.ApiKey.IsNullOrWhiteSpace()))
        {
            return ValidateOptionsResult.Fail($"{nameof(SonarrIntegrationConfiguration.ApiKey)} is required and cannot be empty or white space only");
        }

        return base.Validate(name, allOptions);
    }
}
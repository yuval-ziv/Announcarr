using Announcarr.Integrations.Abstractions.Integration.Extensions.DependencyInjection.Validations;
using Announcarr.Integrations.Radarr.Integration.Configurations;
using Announcarr.Utils.Extensions.String;
using Microsoft.Extensions.Options;

namespace Announcarr.Integrations.Radarr.Extensions.DependencyInjection.Validations;

public class RadarrServiceIntegrationConfigurationValidator : BaseServiceIntegrationConfigurationValidator<RadarrIntegrationConfiguration>
{
    public override ValidateOptionsResult Validate(string? name, List<RadarrIntegrationConfiguration> allOptions)
    {
        if (allOptions.Any(options => options.ApiKey.IsNullOrWhiteSpace()))
        {
            return ValidateOptionsResult.Fail($"{nameof(RadarrIntegrationConfiguration.ApiKey)} is required and cannot be empty or white space only");
        }

        return base.Validate(name, allOptions);
    }
}
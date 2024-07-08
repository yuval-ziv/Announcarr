using Announcer.Integrations.Radarr.Configurations;
using Microsoft.Extensions.Options;

namespace Announcer.Integrations.Radarr.Extensions.DependencyInjection.Validations;

public class RadarrServiceIntegrationConfigurationValidator : IValidateOptions<RadarrIntegrationConfiguration>
{
    public ValidateOptionsResult Validate(string? name, RadarrIntegrationConfiguration options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.ApiKey)} is required and cannot be empty or white space only");
        }

        return ValidateOptionsResult.Success;
    }
}
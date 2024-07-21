using Announcarr.Integrations.Sonarr.Integration.Configurations;
using Microsoft.Extensions.Options;

namespace Announcarr.Integrations.Sonarr.Extensions.DependencyInjection.Validations;

public class SonarrServiceIntegrationConfigurationValidator : IValidateOptions<SonarrIntegrationConfiguration>
{
    public ValidateOptionsResult Validate(string? name, SonarrIntegrationConfiguration options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.ApiKey)} is required and cannot be empty or white space only");
        }


        return ValidateOptionsResult.Success;
    }
}
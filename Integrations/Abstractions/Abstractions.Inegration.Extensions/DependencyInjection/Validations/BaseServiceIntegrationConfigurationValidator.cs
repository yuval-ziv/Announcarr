using Announcarr.Integrations.Abstractions.Integration.Abstractions;
using Announcarr.Utils.Extensions.String;
using Microsoft.Extensions.Options;

namespace Announcarr.Integrations.Abstractions.Integration.Extensions.DependencyInjection.Validations;

public abstract class BaseServiceIntegrationConfigurationValidator<TConfiguration> : IValidateOptions<List<TConfiguration>> where TConfiguration : BaseIntegrationConfiguration
{
    public virtual ValidateOptionsResult Validate(string? name, List<TConfiguration> allOptions)
    {
        if (allOptions.Count <= 1)
        {
            return ValidateOptionsResult.Success;
        }

        if (allOptions.Any(configuration => configuration.Name.IsNullOrEmpty()))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(BaseIntegrationConfiguration.Name)} is required when there are multiple configurations of the same type, and cannot be empty or whitespace only");
        }

        if (allOptions.Select(configuration => configuration.Name).Distinct().Count() < allOptions.Count)
        {
            return ValidateOptionsResult.Fail("all configuration names must be distinct when there are multiple configurations of the same type");
        }

        return ValidateOptionsResult.Success;
    }
}
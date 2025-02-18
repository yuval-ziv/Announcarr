using Announcarr.Utils.Extensions.String;
using Announcarr.Webhooks.Overseerr.Extensions.Configurations;
using Microsoft.Extensions.Options;

namespace Announcarr.Webhooks.Overseerr.Extensions.DependencyInjection.Validations;

public class OverseerrConfigurationValidator : IValidateOptions<List<OverseerrConfiguration>>
{
    public ValidateOptionsResult Validate(string? name, List<OverseerrConfiguration> allConfigurations)
    {
        if (allConfigurations.Count <= 1)
        {
            return ValidateOptionsResult.Success;
        }

        if (allConfigurations.Any(configuration => configuration.Name.IsNullOrEmpty()))
        {
            return ValidateOptionsResult.Fail($"{nameof(OverseerrConfiguration.Name)} is required when there are multiple configurations of the same type, and cannot be empty or whitespace only");
        }

        if (allConfigurations.Select(configuration => configuration.Name).Distinct().Count() < allConfigurations.Count)
        {
            return ValidateOptionsResult.Fail("all configuration names must be distinct when there are multiple configurations of the same type");
        }

        if (allConfigurations.DistinctBy(configuration => configuration.Path).Count() < allConfigurations.Count)
        {
            return ValidateOptionsResult.Fail("all configuration paths must be distinct when there are multiple configurations of the same type");
        }

        return ValidateOptionsResult.Success;
    }
}
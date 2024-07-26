using Announcarr.Exporters.Abstractions.Exporter.Interfaces;
using Announcarr.Integrations.Abstractions.Interfaces;
using Announcarr.Utils.Extensions.String;
using Microsoft.Extensions.Options;

namespace Announcarr.Exporters.Abstractions.Exporter.Extensions.DependencyInjection.Validations;

public abstract class BaseExporterConfigurationValidator<TConfiguration> : IValidateOptions<List<TConfiguration>> where TConfiguration : IExporterConfiguration
{
    public virtual ValidateOptionsResult Validate(string? name, List<TConfiguration> allOptions)
    {
        if (allOptions.Count <= 1)
        {
            return ValidateOptionsResult.Success;
        }

        if (allOptions.Any(configuration => configuration.Name.IsNullOrEmpty()))
        {
            return ValidateOptionsResult.Fail($"{nameof(IIntegrationConfiguration.Name)} is required when there are multiple configurations of the same type, and cannot be empty or whitespace only");
        }

        if (allOptions.Select(configuration => configuration.Name).Distinct().Count() < allOptions.Count)
        {
            return ValidateOptionsResult.Fail("all configuration names must be distinct when there are multiple configurations of the same type");
        }

        return ValidateOptionsResult.Success;
    }
}
using Microsoft.Extensions.Options;

namespace Announcarr.Extensions;

public static class ValidateOptionsResultExtensions
{
    public static ValidateOptionsResult Merge(this ValidateOptionsResult first, ValidateOptionsResult second)
    {
        if (first.Failed || second.Failed)
        {
            return ValidateOptionsResult.Fail((first.Failures ?? []).Concat(second.Failures ?? []));
        }

        if (first.Skipped || second.Skipped)
        {
            return ValidateOptionsResult.Skip;
        }

        return ValidateOptionsResult.Success;
    }
}
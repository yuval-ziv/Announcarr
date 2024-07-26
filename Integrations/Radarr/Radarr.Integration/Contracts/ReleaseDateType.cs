using System.ComponentModel;

namespace Announcarr.Integrations.Radarr.Integration.Contracts;

public enum ReleaseDateType
{
    [Description("physical release")] PhysicalRelease,
    [Description("digital release")] DigitalRelease,
    [Description("in cinemas")] InCinemas,
}
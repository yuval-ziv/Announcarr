using System.ComponentModel;

namespace Announcarr.Abstractions.Contracts;

public enum AnnouncementType
{
    Test,
    Calendar,
    [Description("Recently Added")] RecentlyAdded,
}
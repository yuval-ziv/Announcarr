using System.ComponentModel;

namespace Announcarr.Abstractions.Contracts;

public enum AnnouncementType
{
    Calendar,
    [Description("Recently Added")] RecentlyAdded,
}
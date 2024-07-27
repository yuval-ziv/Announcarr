using System.ComponentModel;

namespace Announcarr.Abstractions.Contracts.Contracts;

public enum AnnouncementType
{
    Calendar,
    [Description("Recently Added")] RecentlyAdded,
}
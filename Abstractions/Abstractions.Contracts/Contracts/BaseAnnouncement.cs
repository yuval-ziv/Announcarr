namespace Announcarr.Abstractions.Contracts;

public abstract class BaseAnnouncement
{
    public virtual bool IsEmpty { get; }
    public virtual AnnouncementType AnnouncementType { get; }
    public virtual HashSet<string> Tags { get; set; } = [];
}
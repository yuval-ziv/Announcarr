namespace Announcarr.Abstractions.Contracts;

public abstract class BaseAnnouncement
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public virtual bool IsEmpty { get; }
    public virtual AnnouncementType AnnouncementType { get; }
    public virtual HashSet<string> Tags { get; set; } = [];
}
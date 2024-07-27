namespace Announcarr.Abstractions.Contracts;

public interface IAnnouncement
{
    public bool IsEmpty { get; }
    public AnnouncementType AnnouncementType { get; }
    public List<string> Tags { get; set; }
}
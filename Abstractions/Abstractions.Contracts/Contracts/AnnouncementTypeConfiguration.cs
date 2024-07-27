namespace Announcarr.Abstractions.Contracts;

public class AnnouncementTypeConfiguration
{
    public bool IsEnabled { get; set; }
    public HashSet<string> Tags { get; set; } = [];
}
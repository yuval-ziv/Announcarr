namespace Announcarr.Abstractions.Contracts;

public class AnnouncementTypeConfiguration
{
    public bool IsEnabled { get; set; }
    public List<string> Tags { get; set; } = [];
}
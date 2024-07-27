namespace Announcarr.Integrations.Abstractions.Interfaces;

public class AnnouncementTypeConfiguration
{
    public bool IsEnabled { get; set; }
    public List<string> Tags { get; set; } = [];
}
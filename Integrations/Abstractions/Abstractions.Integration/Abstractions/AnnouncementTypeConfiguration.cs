namespace Announcarr.Integrations.Abstractions.Integration.Abstractions;

public class AnnouncementTypeConfiguration
{
    public bool IsEnabled { get; set; }
    public List<string> Tags { get; set; } = [];
}
using Announcarr.Abstractions.Contracts;

namespace Announcarr.Exporters.Abstractions.Exporter.Interfaces;

public abstract class BaseExporterConfiguration
{
    public string? Name { get; set; }
    public Dictionary<AnnouncementType, AnnouncementTypeConfiguration> AnnouncementTypeToConfiguration { get; set; } = [];

    public virtual bool IsEnabledByAnnouncementType(AnnouncementType announcementType, bool defaultValue = true)
    {
        return AnnouncementTypeToConfiguration.TryGetValue(announcementType, out AnnouncementTypeConfiguration? value) ? value.IsEnabled : defaultValue;
    }

    public virtual List<string> GetTagsByAnnouncementType(AnnouncementType announcementType)
    {
        return GetTagsByAnnouncementType(announcementType, []);
    }

    public virtual List<string> GetTagsByAnnouncementType(AnnouncementType announcementType, List<string> defaultTags)
    {
        return AnnouncementTypeToConfiguration.TryGetValue(announcementType, out AnnouncementTypeConfiguration? value) ? value.Tags : defaultTags;
    }
}
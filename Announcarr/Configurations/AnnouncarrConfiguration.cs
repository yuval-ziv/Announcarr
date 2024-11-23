namespace Announcarr.Configurations;

public class AnnouncarrConfiguration
{
    public const string SectionName = "Announcarr";
    public required AnnouncarrIntervalConfiguration Interval { get; set; }
    public required AnnouncarrEmptyContractFallbackConfiguration EmptyContractFallback { get; set; }
}
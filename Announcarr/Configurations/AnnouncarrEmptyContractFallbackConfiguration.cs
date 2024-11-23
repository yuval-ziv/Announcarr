namespace Announcarr.Configurations;

public class AnnouncarrEmptyContractFallbackConfiguration
{
    public bool ExportOnEmptyContract { get; set; } = false;
    public string CustomMessageOnEmptyContract { get; set; } = "There is nothing to announce for {announcementType}";
}
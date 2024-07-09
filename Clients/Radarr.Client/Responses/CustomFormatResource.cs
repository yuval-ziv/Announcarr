namespace Announcarr.Clients.Radarr.Responses;

public class CustomFormatResource
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool? IncludeCustomFormatWhenRenaming { get; set; }
    public List<CustomFormatSpecificationSchema>? Specifications { get; set; }
}
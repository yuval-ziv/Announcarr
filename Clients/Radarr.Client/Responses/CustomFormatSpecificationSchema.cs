namespace Announcer.Clients.Radarr.Responses;

public class CustomFormatSpecificationSchema
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Implementation { get; set; }
    public string? ImplementationName { get; set; }
    public string? InfoLink { get; set; }
    public bool Negate { get; set; }
    public bool Required { get; set; }
    public List<Field>? Fields { get; set; }
    public List<CustomFormatSpecificationSchema>? Presets { get; set; }
}
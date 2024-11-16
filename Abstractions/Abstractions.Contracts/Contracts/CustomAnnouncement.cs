using Announcarr.Utils.Extensions.String;

namespace Announcarr.Abstractions.Contracts;

public class CustomAnnouncement : BaseAnnouncement
{
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? Image { get; set; }
    public string? Link { get; set; }
    public override bool IsEmpty => Title.IsNullOrEmpty() && Message.IsNullOrEmpty();
}
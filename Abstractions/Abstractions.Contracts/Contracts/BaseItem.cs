namespace Announcarr.Abstractions.Contracts;

public abstract class BaseItem : ICaptionableItem, IThumbnailItem
{
    public required string ItemSource { get; set; }
    public required DateTimeOffset? ReleaseDate { get; set; }
    public string? ThumbnailUrl { get; set; }
    public abstract string? GetCaption(string dateTimeFormat);
    public string? GetThumbnailUri() => ThumbnailUrl;
}
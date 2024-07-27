namespace Announcarr.Abstractions.Contracts;

public interface ICaptionableItem
{
    string? GetCaption(string dateTimeFormat);
}
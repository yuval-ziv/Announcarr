namespace Announcarr.Abstractions.Contracts.Contracts;

public interface ICaptionableItem
{
    string? GetCaption(string dateTimeFormat);
}
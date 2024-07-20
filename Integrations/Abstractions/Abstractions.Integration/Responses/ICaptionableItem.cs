namespace Announcarr.Integrations.Abstractions.Responses;

public interface ICaptionableItem
{
    string? GetCaption(string dateTimeFormat);
}
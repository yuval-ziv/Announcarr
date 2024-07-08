namespace Announcer.Clients.Sonarr.Responses;

public class MediaInfoResource
{
    public int Id { get; set; }
    public long AudioBitrate { get; set; }
    public double AudioChannels { get; set; }
    public string? AudioCodec { get; set; }
    public string? AudioLanguages { get; set; }
    public int AudioStreamCount { get; set; }
    public int VideoBitDepth { get; set; }
    public long VideoBitrate { get; set; }
    public string? VideoCodec { get; set; }
    public double VideoFps { get; set; }
    public string? VideoDynamicRange { get; set; }
    public string? VideoDynamicRangeType { get; set; }
    public string? Resolution { get; set; }
    public string? RunTime { get; set; }
    public string? ScanType { get; set; }
    public string? Subtitles { get; set; }
}
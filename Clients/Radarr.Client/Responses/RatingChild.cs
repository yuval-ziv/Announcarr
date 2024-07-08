namespace Announcer.Clients.Radarr.Responses;

public class RatingChild
{
    public int Votes { get; set; }
    public double Value { get; set; }
    public RatingType Type { get; set; }
}
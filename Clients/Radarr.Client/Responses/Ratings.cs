namespace Announcarr.Clients.Radarr.Responses;

public class Ratings
{
    public RatingChild Imdb { get; set; } = new();
    public RatingChild Tmdb { get; set; } = new();
    public RatingChild Metacritic { get; set; } = new();
    public RatingChild RottenTomatoes { get; set; } = new();
}
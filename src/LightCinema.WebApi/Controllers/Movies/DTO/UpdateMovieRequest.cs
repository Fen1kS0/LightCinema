namespace LightCinema.WebApi.Controllers.Movies.DTO;

public sealed class UpdateMovieRequest
{
    public required string Name { get; set; }
    public required string Descriptions { get; set; }
    public required string ImageLink { get; set; }
    public required string PosterLink { get; set; }
    public required int? Year { get; set; }
    public required int? AgeLimit { get; set; }
    public required IReadOnlyCollection<int> Genres { get; set; }
    public required IReadOnlyCollection<int> Countries { get; set; }
}
namespace LightCinema.WebApi.Controllers.Movies.DTO;

public class GetMovieByIdResponse
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required int CreatedYear { get; set; }
    public required int AgeLimit { get; set; }
    public required string ImageLink { get; set; }
    public required string PosterLink { get; set; }
    public required IEnumerable<GetCountryDto> Countries { get; set; }
    public required IEnumerable<GetGenreDto> Genres { get; set; }
    public required IEnumerable<GetMovieByIdSessionDto> Sessions { get; set; }
}

public sealed class GetMovieByIdSessionDto
{
    public required int Id { get; set; }
    public required string DateTime { get; set; }
    public required int MinPrice { get; set; }
}
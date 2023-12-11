namespace LightCinema.WebApi.Controllers.Movies.DTO;

public sealed class GetMoviesResponse
{
    public required IEnumerable<GetMovieDto> Movies { get; set; }
}

public sealed class GetMovieDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required IEnumerable<string> Genres { get; set; }
    public required string PosterLink { get; set; }
    public IEnumerable<GetMovieSessionDto>? Sessions { get; set; }
}

public sealed class GetMovieSessionDto
{
    public required int Id { get; set; }
    public required string Time { get; set; }
    public required int MinPrice { get; set; }
}
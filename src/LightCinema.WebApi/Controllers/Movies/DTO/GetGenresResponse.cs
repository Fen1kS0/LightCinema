namespace LightCinema.WebApi.Controllers.Movies.DTO;

public sealed class GetGenresResponse
{
    public required IEnumerable<GetGenreDto> Genres { get; set; }
}

public sealed class GetGenreDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}
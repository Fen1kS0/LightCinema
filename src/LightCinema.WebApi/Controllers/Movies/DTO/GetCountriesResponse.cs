namespace LightCinema.WebApi.Controllers.Movies.DTO;

public sealed class GetCountriesResponse
{
    public required IEnumerable<GetCountryDto> Countries { get; set; }
}

public sealed class GetCountryDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}
namespace LightCinema.WebApi.Controllers.Sessions.DTO;

public sealed class GetSeatsBySessionResponse
{
    public required IEnumerable<GetSeatDto> Seats { get; set; }
}

public sealed class GetSeatDto
{
    public required int Id { get; set; }
    public required int Row { get; set; }
    public required int Number { get; set; }
    public required int Price { get; set; }
    public required bool Reserved { get; set; }
}
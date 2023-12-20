namespace LightCinema.WebApi.Controllers.Halls.DTO;

public sealed class CreateHallRequest
{
    public required int Number { get; set; }

    public required IEnumerable<CreateSeatDto> Seats { get; set; }
}

public sealed class CreateSeatDto
{
    
    public required int Row { get; set; }
    public required int Number { get; set; }
    public required bool IsIncreasedPrice { get; set; }
}
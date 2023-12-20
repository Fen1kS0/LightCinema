namespace LightCinema.WebApi.Controllers.Halls.DTO;

public sealed class GetHallsResponse
{
    public IEnumerable<GetHallDto> Halls { get; set; }
}

public sealed class GetHallDto
{
    public int Number { get; set; }
    public int Rows { get; set; }
    public int SeatsInRow { get; set; }
    public int VipSeats { get; set; }
}
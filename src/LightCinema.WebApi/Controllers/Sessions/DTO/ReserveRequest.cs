namespace LightCinema.WebApi.Controllers.Sessions.DTO;

public sealed class ReserveRequest
{
    public required IEnumerable<int> Seats { get; set; }
}
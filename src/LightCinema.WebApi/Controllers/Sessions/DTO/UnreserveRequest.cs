namespace LightCinema.WebApi.Controllers.Sessions.DTO;

public sealed class UnreserveRequest
{
    public required ICollection<int> Seats { get; set; }
}
namespace LightCinema.WebApi.Controllers.Sessions.DTO;

public sealed class CreateSessionRequest
{
    public int MovieId { get; set; }
    public DateTime DateTime { get; set; }
    public int HallNumber { get; set; }
    public int Price { get; set; }
    public int IncreasedPrice { get; set; }
}
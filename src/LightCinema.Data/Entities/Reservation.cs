namespace LightCinema.Data.Entities;

public sealed class Reservation
{
    public required User User { get; set; }
    public int UserId { get; set; }
    
    public required Session Session { get; set; }
    public int SessionId { get; set; }
    
    public required Place Place { get; set; }
    public int PlaceId { get; set; }
}
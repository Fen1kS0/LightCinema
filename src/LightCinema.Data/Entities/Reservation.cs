namespace LightCinema.Data.Entities;

public sealed class Reservation
{
    public User User { get; set; } = null!;
    public string UserLogin { get; set; }
    public Session Session { get; set; } = null!;
    public int SessionId { get; set; }
    
    public Seat Seat { get; set; } = null!;
    public int SeatId { get; set; }
}
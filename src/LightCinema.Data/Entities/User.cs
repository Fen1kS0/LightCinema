namespace LightCinema.Data.Entities;

public sealed class User
{
    public required string Login { get; set; }
    
    public required string Password { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
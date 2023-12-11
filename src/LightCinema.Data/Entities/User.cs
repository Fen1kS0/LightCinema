namespace LightCinema.Data.Entities;

public sealed class User
{
    public string Login { get; set; }
    
    public string Password { get; set; }
    
    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
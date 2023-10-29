namespace LightCinema.Data.Entities;

public sealed class Session
{
    public int Id { get; set; }

    public required Movie Movie { get; set; }
    public int MovieId { get; set; }

    public DateTime Start { get; set; }

    public int HallNumber { get; set; }

    public int Price { get; set; }
    public int IncreasedPrice { get; set; }
    
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
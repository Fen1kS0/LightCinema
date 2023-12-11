namespace LightCinema.Data.Entities;

public sealed class Seat
{
    public int Id { get; set; }

    public int Hall { get; set; }
    
    public int Row { get; set; }

    public int Number { get; set; }

    public bool IsIncreasedPrice { get; set; }
    
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
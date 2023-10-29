namespace LightCinema.Data.Entities;

public sealed class Place
{
    public int Id { get; set; }

    public int HallNumber { get; set; }
    
    public int RowNumber { get; set; }

    public int PlaceNumber { get; set; }

    public bool IsIncreasedPrice { get; set; }
    
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
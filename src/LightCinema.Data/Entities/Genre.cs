namespace LightCinema.Data.Entities;

public sealed class Genre
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
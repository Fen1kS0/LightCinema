namespace LightCinema.Data.Entities;

public sealed class Country
{
    public int Id { get; set; }

    public string Name { get; set; }
    
    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
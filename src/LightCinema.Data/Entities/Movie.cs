namespace LightCinema.Data.Entities;

public sealed class Movie
{
    public int Id { get; set; }
    
    public string Name { get; set; }

    public string Descriptions { get; set; }

    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    
    public ICollection<Country> Countries { get; set; } = new List<Country>();

    public int Year { get; set; }

    public int AgeLimit { get; set; }

    public string PosterLink { get; set; }
    
    public string ImageLink { get; set; }
    
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}
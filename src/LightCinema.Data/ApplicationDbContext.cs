using System.Reflection;
using LightCinema.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LightCinema.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Seat> Places { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Country> Countries { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
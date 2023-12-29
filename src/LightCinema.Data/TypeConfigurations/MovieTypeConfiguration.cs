using LightCinema.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightCinema.Data.TypeConfigurations;

public class MovieTypeConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("Movie");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name).HasMaxLength(250).IsRequired();
        builder.Property(c => c.Descriptions).IsRequired();
        builder.Property(c => c.PosterLink).IsRequired();
        builder.Property(c => c.ImageLink).IsRequired();
        builder.Property(c => c.AgeLimit).HasMaxLength(18);
        builder.Property(c => c.Year).HasMaxLength(DateTime.Now.AddYears(1).Year);
        
        builder
            .HasMany(m => m.Genres)
            .WithMany(g => g.Movies)
            .UsingEntity("GenreMovie");
        
        builder
            .HasMany(c => c.Countries)
            .WithMany(g => g.Movies)
            .UsingEntity("CountryMovie");
    }
}
using LightCinema.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightCinema.Data.TypeConfigurations;

public class SeatTypeConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("Seats");
        builder.HasKey(c => c.Id);
    }
}
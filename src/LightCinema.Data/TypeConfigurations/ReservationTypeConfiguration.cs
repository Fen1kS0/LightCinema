using LightCinema.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightCinema.Data.TypeConfigurations;

public class ReservationTypeConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(r => new { r.SessionId, r.UserLogin, PlaceId = r.SeatId });
        
        builder.HasOne(r => r.Session)
            .WithMany(s => s.Reservations)
            .HasForeignKey(r => r.SessionId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        
        builder.HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserLogin)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        
        builder.HasOne(r => r.Seat)
            .WithMany(p => p.Reservations)
            .HasForeignKey(r => r.SeatId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
using LightCinema.Data.Entities;

namespace LightCinema.WebApi.Controllers.Users.DTO;

public sealed class UserResponse
{
    public required string Login { get; set; }
    public required ICollection<Reservation> Reservations { get; set; }
}
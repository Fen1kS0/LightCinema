namespace LightCinema.WebApi.Controllers.Users.DTO;

public sealed class ProfileResponse
{
    public required string Login { get; set; }

    public required IEnumerable<ReservationProfile> Reserves { get; set; }
}

public sealed class ReservationProfile
{
    public required int SessionId { get; set; }
    public required int SeatId { get; set; }
    public required string MovieName { get; set; }
    public required int Hall { get; set; }
    public required int Row { get; set; }
    public required int Number { get; set; }
    public required string DateTime { get; set; }
    public required bool CanUnreserve { get; set; }
}
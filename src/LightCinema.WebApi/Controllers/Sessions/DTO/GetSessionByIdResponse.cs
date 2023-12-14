namespace LightCinema.WebApi.Controllers.Sessions.DTO;

public sealed class GetSessionByIdResponse
{
    public required int MovieId { get; set; }
    public required string MovieName { get; set; }
    public required string SessionsDate { get; set; }
    public required IEnumerable<OtherSessionDto> Sessions { get; set; }
}

public sealed class OtherSessionDto
{
    public required int Id { get; set; }
    public required string Time { get; set; }
    public required int MinPrice { get; set; }
}
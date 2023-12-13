namespace LightCinema.WebApi.Controllers.Users.DTO;

public sealed class AuthResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required string Login { get; set; }
    public required string Role { get; set; }
}
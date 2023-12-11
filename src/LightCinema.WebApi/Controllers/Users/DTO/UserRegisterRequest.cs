namespace LightCinema.WebApi.Controllers.Users.DTO;

public sealed class UserRegisterRequest
{
    public required string Login { get; set; }
    public required string Password { get; set; }
}
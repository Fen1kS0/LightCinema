using System.ComponentModel.DataAnnotations;

namespace LightCinema.WebApi.Controllers.Users.DTO;

public sealed class RefreshRequest
{
    public required string AccessToken { get; set; }
    
    [Required]
    public required string RefreshToken { get; set; }
}
namespace LightCinema.WebApi.Application.Configs;

public sealed class JwtConfig
{
    public string Issuer { get; set; } = null!;
    
    public string Audience { get; set; } = null!;
    
    public string SecretKey { get; set; } = null!;
    
    public int AccessTokenExpiryInMinutes { get; set; }
    
    public int RefreshTokenExpiryInDays { get; set; }
}
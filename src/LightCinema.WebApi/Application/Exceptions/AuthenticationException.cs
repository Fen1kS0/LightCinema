namespace LightCinema.WebApi.Application.Exceptions;

public sealed class AuthenticationException : BaseException
{
    public AuthenticationException(string message) : base(message, 401)
    {
    }
}
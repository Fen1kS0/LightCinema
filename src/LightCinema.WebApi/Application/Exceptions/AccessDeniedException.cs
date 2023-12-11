namespace LightCinema.WebApi.Application.Exceptions;

public sealed class AccessDeniedException : BaseException
{
    public AccessDeniedException(string message) : base(message, 403)
    {
    }
}
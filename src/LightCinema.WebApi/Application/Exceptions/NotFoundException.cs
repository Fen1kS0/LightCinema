namespace LightCinema.WebApi.Application.Exceptions;

public sealed class NotFoundException : BaseException
{
    public NotFoundException(string message) : base(message, 404)
    {
    }
}
namespace LightCinema.WebApi.Application.Exceptions;

public sealed class BusinessException : BaseException
{
    public BusinessException(string message) : base(message, 400)
    {
    }
}
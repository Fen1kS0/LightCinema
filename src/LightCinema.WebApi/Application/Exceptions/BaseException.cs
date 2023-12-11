namespace LightCinema.WebApi.Application.Exceptions;

public abstract class BaseException : Exception
{
    protected BaseException(string message) : base(message)
    {
        StatusCode = 500;
    }

    protected BaseException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
    
    public int StatusCode { get; set; }
}
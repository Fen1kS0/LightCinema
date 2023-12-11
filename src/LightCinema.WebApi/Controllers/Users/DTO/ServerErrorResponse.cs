namespace LightCinema.WebApi.Controllers.Users.DTO;

public sealed class ServerErrorResponse
{
    public ServerErrorResponse(int statusCode, string errorMessage, string? stackTrace)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
        StackTrace = stackTrace;
    }

    public int StatusCode { get; set; }
    public string ErrorMessage { get; set; }
    public string? StackTrace { get; set; }
}
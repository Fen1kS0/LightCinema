namespace LightCinema.WebApi.Controllers.Users.DTO;

public sealed class ClientErrorResponse
{
    public ClientErrorResponse(int statusCode, string errorMessage)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }

    public int StatusCode { get; set; }
    public string ErrorMessage { get; set; }
}
using LightCinema.WebApi.Application.Exceptions;
using LightCinema.WebApi.Controllers.Users.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LightCinema.WebApi.Application.Filters;

public class ExceptionFilter : ExceptionFilterAttribute
{
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger;
    }

    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is BaseException exception)
        {
            var response = new ClientErrorResponse(exception.StatusCode, exception.Message);

            context.HttpContext.Response.StatusCode = exception.StatusCode;
            context.Result = new JsonResult(response);
            
            _logger.LogWarning($"{exception.StatusCode} {exception.Message}");
        }
        else
        {
            var response = new ServerErrorResponse(500, context.Exception.Message, context.Exception.StackTrace);

            context.HttpContext.Response.StatusCode = 500;
            context.Result = new JsonResult(response);
            
            _logger.LogError($"{context.Exception.Message} {context.Exception.Message} {context.Exception.StackTrace}");
        }

        await base.OnExceptionAsync(context);
    }
}
using System.Security.Claims;
using LightCinema.WebApi.Application.Auth;
using Microsoft.AspNetCore.Mvc;

namespace LightCinema.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BaseController : ControllerBase
{
    protected int UserId => User.Identity is { IsAuthenticated: false }
        ? default
        : int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    
    protected bool IsAdmin => User.Identity is { IsAuthenticated: false }
        ? default
        : User.FindFirst(ClaimTypes.Role)?.Value == RoleNames.Admin;
}
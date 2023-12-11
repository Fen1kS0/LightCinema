using System.Security.Claims;
using LightCinema.WebApi.Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightCinema.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = PolicyNames.RequireVisitorRole)]
public class BaseController : ControllerBase
{
    protected string? UserLogin => User.Identity is { IsAuthenticated: false }
        ? default
        : User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    
    protected bool IsAdmin => User.Identity is { IsAuthenticated: false }
        ? default
        : User.FindFirst(ClaimTypes.Role)?.Value == RoleNames.Admin;
}
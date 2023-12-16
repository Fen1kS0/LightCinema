using System.Security.Claims;
using LightCinema.Data;
using LightCinema.Data.Entities;
using LightCinema.WebApi.Application.Auth;
using LightCinema.WebApi.Application.Configs;
using LightCinema.WebApi.Application.Exceptions;
using LightCinema.WebApi.Application.Services;
using LightCinema.WebApi.Controllers.Users.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace LightCinema.WebApi.Controllers.Users;

public class UsersController : BaseController
{
    private readonly ApplicationDbContext _dbContext;
    private readonly JwtService _jwtService;
    private readonly JwtConfig _jwtConfig;
    
    public UsersController(ApplicationDbContext dbContext, JwtService jwtService, JwtConfig jwtConfig)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
        _jwtConfig = jwtConfig;
    }

    [HttpGet("{login}")]
    [Authorize(Policy = PolicyNames.RequireAdministratorRole)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserById([FromRoute] string login)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(x => x.Reservations)
            .FirstOrDefaultAsync(x => x.Login == login);

        if (user is null)
        {
            throw new NotFoundException("Пользователь не найден");
        }
        
        return Ok(new UserResponse
        {
            Login = user.Login,
            Reservations = user.Reservations
        });
    }
    
    [HttpPost("sign-in")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> LoginUser([FromBody] UserLoginRequest userLoginRequest)
    {
        try
        {
            return Ok(GetAdminCredentials(userLoginRequest.Login, userLoginRequest.Password));
        }
        catch (Exception)
        {
            // ignored
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Login == userLoginRequest.Login);

        if (user is null)
        {
            throw new NotFoundException("Пользователь не найден");
        }

        if (userLoginRequest.Password != user.Password)
        {
            throw new AuthenticationException("Неверный пароль");
        }
        
        var claims = GenerateUserClaims(user);

        var accessToken = _jwtService.GenerateAccessToken(claims);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryInDays);
        await _dbContext.SaveChangesAsync();
        
        return Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Login = user.Login,
            Role = RoleNames.Visitor
        });
    }
    
    [HttpPost("sign-up")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> RegisterUser([FromBody] UserRegisterRequest userRegisterRequest)
    {
        if (userRegisterRequest.Login == "admin")
        {
            throw new BusinessException("Админ не может быть зарегистрирован");
        }
        
        var existsUser = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Login == userRegisterRequest.Login);

        if (existsUser is not null)
        {
            throw new BusinessException("Такой пользователь уже существует");
        }
        
        var user = new User
        {
            Login = userRegisterRequest.Login,
            Password = userRegisterRequest.Password
        };
        
        var claims = GenerateUserClaims(user);
        
        var accessToken = _jwtService.GenerateAccessToken(claims);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryInDays);
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        
        return Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Login = user.Login,
            Role = RoleNames.Visitor
        });
    }
    
    [HttpPost("refreshToken")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshRequest refreshRequest)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(refreshRequest.AccessToken);
        var userLogin = principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        
        if (userLogin == "admin")
        {
            throw new BusinessException("Админ не может обновить токен");
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Login == userLogin);

        if (user is null || user.RefreshToken != refreshRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new BusinessException("Invalid refresh token request");
        }

        var newAccessToken = _jwtService.GenerateAccessToken(principal.Claims);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryInDays);
        await _dbContext.SaveChangesAsync();

        return Ok(new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            Login = user.Login,
            Role = RoleNames.Visitor
        });
    }
    
    [HttpPost("revokeToken")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeToken()
    {
        if (IsAdmin)
        {
            throw new BusinessException("Админ не может сделать revoke");
        }
        
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Login == UserLogin);

        if (user is null)
        {
            throw new NotFoundException("Пользователь не найден");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _dbContext.SaveChangesAsync();

        return Accepted();
    }
    
    [HttpGet("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile()
    {
        if (IsAdmin)
        {
            throw new BusinessException("У админа нет профиля");
        }
        
        var user = await _dbContext.Users
            .AsNoTracking()
            .AsSingleQuery()
            .FirstOrDefaultAsync(x => x.Login == UserLogin);

        if (user is null)
        {
            throw new NotFoundException("Пользователь не найден");
        }

        var reservations = await _dbContext.Reservations
            .AsNoTracking()
            .AsSingleQuery()
            .Include(x => x.Session)
            .ThenInclude(x => x.Movie)
            .Include(x => x.Seat)
            .Where(x => x.UserLogin == UserLogin)
            .OrderByDescending(x => x.Session.Start)
            .Select(x => new ReservationProfile
            {
                SessionId = x.SessionId,
                SeatId = x.SeatId,
                MovieName = x.Session.Movie.Name,
                DateTime = x.Session.Start.AddHours(4).ToString("yyyy-MM-dd HH:mm"),
                Hall = x.Seat.Hall,
                Row = x.Seat.Row,
                Number = x.Seat.Number,
                CanUnreserve = x.Session.Start > DateTimeOffset.UtcNow
            })
            .ToListAsync();
        
        return Ok(new ProfileResponse
        {
            Login = user.Login,
            Reserves = reservations
        });
    }

    private AuthResponse GetAdminCredentials(string login, string password)
    {
        if (login != "admin" || password != "admin")
        {
            throw new NotFoundException("Пользователь не найден");
        }

        var accessToken = _jwtService.GenerateAccessToken( new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, login),
            new(ClaimTypes.Role, RoleNames.Admin)
        });
        
        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = string.Empty,
            Login = login,
            Role = RoleNames.Admin
        };
    }
    
    private IEnumerable<Claim> GenerateUserClaims(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Login),
            new(ClaimTypes.Role, RoleNames.Visitor)
        };

        return claims;
    }
}
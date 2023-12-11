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
            .SingleOrDefaultAsync(x => x.Login == login);

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
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Login == userLoginRequest.Login);

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
            RefreshToken = refreshToken
        });
    }
    
    [HttpPost("sign-up")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> RegisterUser([FromBody] UserRegisterRequest userRegisterRequest)
    {
        var existsUser = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Login == userRegisterRequest.Login);

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
            RefreshToken = refreshToken
        });
    }
    
    [HttpPost("refreshToken")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshRequest refreshRequest)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(refreshRequest.AccessToken);
        var userLogin = principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Login == userLogin);

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
            RefreshToken = newRefreshToken
        });
    }
    
    [HttpPost("revokeToken")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeToken()
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Login == UserLogin);

        if (user is null)
        {
            throw new NotFoundException("Пользователь не найден");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _dbContext.SaveChangesAsync();

        return Accepted();
    }
    
    [HttpPost("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .AsSingleQuery()
            .SingleOrDefaultAsync(x => x.Login == UserLogin);

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
            .Select(x => new ReservationProfile
            {
                SessionId = x.SessionId,
                SeatId = x.SeatId,
                MovieName = x.Session.Movie.Name,
                DateTime = x.Session.Start.ToString("yyyy-MM-dd HH:mm"),
                Hall = x.Seat.Hall,
                Row = x.Seat.Row,
                Number = x.Seat.Number,
            })
            .ToListAsync();
        
        return Ok(new ProfileResponse
        {
            Login = user.Login,
            Reserves = reservations
        });
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
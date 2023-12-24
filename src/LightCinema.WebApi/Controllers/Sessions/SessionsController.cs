using LightCinema.Data;
using LightCinema.Data.Entities;
using LightCinema.WebApi.Application.Auth;
using LightCinema.WebApi.Application.Exceptions;
using LightCinema.WebApi.Controllers.Sessions.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LightCinema.WebApi.Controllers.Sessions;

public class SessionsController : BaseController
{
    private readonly ApplicationDbContext _dbContext;

    public SessionsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetSessionByIdResponse>> GetSessionById([FromRoute] int id)
    {
        var session = await _dbContext.Sessions
            .AsNoTracking()
            .AsSingleQuery()
            .Include(x => x.Movie)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (session is null)
        {
            throw new NotFoundException("Сессия не найдена");
        }

        var sessionDateTime = session.Start.UtcDateTime.AddHours(4);
        
        var start = new DateTimeOffset(DateOnly.FromDateTime(sessionDateTime).ToDateTime(new TimeOnly()).AddHours(-4), TimeSpan.Zero);
        if (start < DateTimeOffset.UtcNow)
        {
            start = DateTimeOffset.UtcNow;
        }
        
        var end = new DateTimeOffset(DateOnly.FromDateTime(sessionDateTime.AddDays(1)).ToDateTime(new TimeOnly()).AddHours(-4), TimeSpan.Zero);
        
        var otherSessions = await _dbContext.Sessions
            .AsNoTracking()
            .AsSingleQuery()
            .Where(x => x.MovieId == session.MovieId && start < x.Start && x.Start < end)
            .Select(x => new OtherSessionDto
            {
                Id = x.Id,
                MinPrice = x.Price,
                Time = x.Start.AddHours(4).ToString("HH:mm")
            })
            .ToListAsync();

        return Ok(new GetSessionByIdResponse
        {
            MovieId = session.MovieId,
            MovieName = session.Movie.Name,
            AgeLimit = session.Movie.AgeLimit,
            SessionsDate = session.Start.AddHours(4).ToString("yyyy-MM-dd"),
            Sessions = otherSessions
        });
    }

    [HttpGet("{id}/seats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetSeatsBySessionResponse>> GetSeatsBySession([FromRoute] int id)
    {
        var session = await _dbContext.Sessions
            .AsNoTracking()
            .AsSingleQuery()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (session is null)
        {
            throw new NotFoundException("Сессия не найдена");
        }

        var reservedSeatIds = await _dbContext.Reservations
            .AsNoTracking()
            .AsSingleQuery()
            .Where(x => x.SessionId == id)
            .Select(x => x.SeatId)
            .ToListAsync();

        var seats = await _dbContext.Seats
            .AsNoTracking()
            .AsSingleQuery()
            .Where(x => x.Hall == session.Hall)
            .OrderBy(x => x.Row)
            .ThenBy(x => x.Number)
            .Select(x => new GetSeatDto
            {
                Id = x.Id,
                Row = x.Row,
                Number = x.Number,
                Price = x.IsIncreasedPrice ? session.IncreasedPrice : session.Price,
                Reserved = reservedSeatIds.Contains(x.Id)
            })
            .ToListAsync();

        return Ok(new GetSeatsBySessionResponse
        {
            Seats = seats
        });
    }

    [HttpPost("{id}/reserve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Reserve([FromRoute] int id, [FromBody] ReserveRequest request)
    {
        var session = await _dbContext.Sessions
            .AsNoTracking()
            .AsSingleQuery()
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (session is null || session.Start < DateTimeOffset.UtcNow)
        {
            throw new NotFoundException("Сессия не найдена");
        }

        var reservedSeatIds = await _dbContext.Reservations
            .AsNoTracking()
            .AsSingleQuery()
            .Where(x => x.SessionId == id)
            .Select(x => x.SeatId)
            .ToListAsync();

        if (request.Seats.Any(x => reservedSeatIds.Contains(x)))
        {
            throw new BusinessException("Бронь не возможна, т.к. одно из мест уже занято");
        }

        var reservations = request.Seats.Select(seatId => new Reservation
        {
            SeatId = seatId,
            SessionId = id,
            UserLogin = UserLogin!
        });
        
        await _dbContext.Reservations.AddRangeAsync(reservations);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }
    
    [HttpPost("{id}/unreserve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Unreserve([FromRoute] int id, [FromBody] UnreserveRequest request)
    {
        var session = await _dbContext.Sessions
            .AsNoTracking()
            .AsSingleQuery()
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (session is null || session.Start < DateTimeOffset.UtcNow)
        {
            throw new NotFoundException("Сессия не найдена");
        }

        var reservations = await _dbContext.Reservations
            .AsSingleQuery()
            .Where(x => x.UserLogin == UserLogin! && x.SessionId == id && x.SeatId == request.SeatId)
            .SingleOrDefaultAsync();

        if (reservations is null)
        {
            throw new BusinessException("Бронь снять не возможно, т.к. она уже снята");
        }
        
        _dbContext.Reservations.RemoveRange(reservations);
        await _dbContext.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = PolicyNames.RequireAdministratorRole)]
    public async Task<IActionResult> Create([FromBody] CreateSessionRequest request)
    {
        var seat = await _dbContext.Seats
                .AsSingleQuery()
                .FirstOrDefaultAsync(x => x.Hall == request.HallNumber);
            
        if (seat is null)
        {
            throw new BusinessException("Такого зала не существует");
        }

        if (request.DateTime < DateTimeOffset.UtcNow.DateTime.AddHours(4))
        {
            throw new BusinessException("Время и дата не могут быть в прошлом");
        }

        if (request.Price < 0)
        {
            throw new BusinessException("Цена на сеанс не может быть отрицательной");
        }
        
        if (request.IncreasedPrice < request.Price)
        {
            throw new BusinessException("Повышенная цена должна быть больше обычной");
        }

        var movie = _dbContext.Movies
            .AsNoTracking()
            .AsSingleQuery()
            .SingleOrDefaultAsync(x => x.Id == request.MovieId);

        if (movie is null)
        {
            throw new BusinessException("Фильм не найден");
        }

        var session = new Session
        {
            MovieId = request.MovieId,
            Hall = request.HallNumber,
            Start = new DateTimeOffset(request.DateTime.AddHours(-4), TimeSpan.Zero),
            Price = request.Price,
            IncreasedPrice = request.IncreasedPrice
        };
        
        await _dbContext.Sessions.AddAsync(session);
        await _dbContext.SaveChangesAsync();
        
        return Ok();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = PolicyNames.RequireAdministratorRole)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var session = await _dbContext.Sessions
            .AsSingleQuery()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (session is null)
        {
            throw new NotFoundException("Сессия не найдена");
        }

        _dbContext.Sessions.Remove(session);
        await _dbContext.SaveChangesAsync();
        
        return Ok();
    }
}
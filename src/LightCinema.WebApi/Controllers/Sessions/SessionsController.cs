﻿using LightCinema.Data;
using LightCinema.Data.Entities;
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

    [HttpGet("{id}/seats")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetSeatsBySessionResponse>> GetSeatsBySession([FromRoute] int id)
    {
        var session = await _dbContext.Sessions
            .AsNoTracking()
            .AsSingleQuery()
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

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

        var seats = await _dbContext.Places
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

        if (session is null)
        {
            throw new NotFoundException("Сессия не найдена");
        }

        var reservations = await _dbContext.Reservations
            .AsSingleQuery()
            .Where(x => x.UserLogin == UserLogin! && x.SessionId == id && request.Seats.Contains(x.SeatId))
            .ToListAsync();

        if (request.Seats.Count != reservations.Count)
        {
            throw new BusinessException("Бронь снять не возможно, т.к. она уже снята");
        }
        
        _dbContext.Reservations.RemoveRange(reservations);
        await _dbContext.SaveChangesAsync();
        
        return Ok();
    }
}
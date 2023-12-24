using LightCinema.Data;
using LightCinema.Data.Entities;
using LightCinema.WebApi.Application.Auth;
using LightCinema.WebApi.Application.Exceptions;
using LightCinema.WebApi.Controllers.Halls.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LightCinema.WebApi.Controllers.Halls;

[Authorize(Policy = PolicyNames.RequireAdministratorRole)]
public class HallsController : BaseController
{
    private readonly ApplicationDbContext _dbContext;

    public HallsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<GetHallsResponse>> GetHalls()
    {
        var halls = await _dbContext.Seats
            .AsNoTracking()
            .AsSingleQuery()
            .GroupBy(x => x.Hall)
            .Select(x => new GetHallDto
            {
                Number = x.Key,
                Rows = x.Max(s => s.Row),
                SeatsInRow = x.Max(s => s.Number),
                VipSeats = x.Count(s => s.IsIncreasedPrice)
            })
            .ToListAsync();

        return Ok(new GetHallsResponse
        {
            Halls = halls
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateHall([FromBody] CreateHallRequest request)
    {
        try
        {
            await _dbContext.Seats
                .Select(x => x.Hall)
                .SingleAsync(x => x == request.Number);
            
            throw new BusinessException("Такой зал уже создан");
        }
        catch (Exception)
        {
            // ignore
        }
        
        var seats = request.Seats.Select(x => new Seat
        {
            Hall = request.Number,
            Row = x.Row,
            Number = x.Number,
            IsIncreasedPrice = x.IsIncreasedPrice
        });

        await _dbContext.Seats.AddRangeAsync(seats);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{number}")]
    public async Task<IActionResult> DeleteHall(int number)
    {
        var seats = await _dbContext.Seats
            .AsSingleQuery()
            .Where(x => x.Hall == number)
            .ToListAsync();

        if (!seats.Any())
        {
            throw new BusinessException("Такого зала нет");
        }

        var sessions = await _dbContext.Sessions
            .AsSingleQuery()
            .Where(x => x.Hall == number)
            .ToListAsync();

        _dbContext.Seats.RemoveRange(seats);
        _dbContext.Sessions.RemoveRange(sessions);
        await _dbContext.SaveChangesAsync();
        
        return NoContent();
    }
}
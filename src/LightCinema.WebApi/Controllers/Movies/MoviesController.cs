﻿using LightCinema.Data;
using LightCinema.WebApi.Controllers.Movies.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LightCinema.WebApi.Controllers.Movies;

public class MoviesController : BaseController
{
    private readonly ApplicationDbContext _dbContext;

    public MoviesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetMoviesResponse>> GetMovies(
        [FromQuery(Name = "date")] DateType dateType,
        [FromQuery(Name = "withSessions")] bool? withSessions = true)
    {
        var now = DateTimeOffset.UtcNow.AddHours(4);
        var (start, end) = dateType switch
        {
            DateType.Today => (new DateTime(now.Year, now.Month, now.Day),
                new DateTime(now.Year, now.Month, now.Day + 1)),
            DateType.Tomorrow => (new DateTime(now.Year, now.Month, now.Day + 1),
                new DateTime(now.Year, now.Month, now.Day + 2)),
            DateType.Soon => (new DateTime(now.Year, now.Month, now.Day + 2), DateTime.MaxValue),
            _ => throw new ArgumentOutOfRangeException(nameof(dateType), dateType, "DateType not found")
        };

        var startOffset = new DateTimeOffset(start, TimeSpan.Zero);
        var endOffset = new DateTimeOffset(end, TimeSpan.Zero);

        var query = _dbContext.Movies
            .AsSingleQuery()
            .AsNoTracking()
            .Include(x => x.Genres)
            .Include(x => x.Sessions)
            .Where(x => x.Sessions.Any(s => startOffset < s.Start && s.Start < endOffset));

        var movies = await query.Select(x => new GetMovieDto()
        {
            Id = x.Id,
            Name = x.Name,
            PosterLink = x.PosterLink,
            Genres = x.Genres.Select(g => g.Name),
            Sessions = x.Sessions
                .Where(s => startOffset < s.Start && s.Start < endOffset)
                .OrderBy(s => s.Start)
                .Select(s => new GetMovieSessionDto
                {
                    Id = s.Id,
                    MinPrice = s.Price,
                    Time = s.Start.ToString("HH:mm")
                })
        }).ToListAsync();

        if (withSessions is true)
        {
            foreach (var movie in movies)
            {
                movie.Sessions = null;
            }
        }

        return Ok(new GetMoviesResponse()
        {
            Movies = movies
        });
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetMoviesResponse>> GetMoviesById([FromRoute] int id)
    {

        var movie = await _dbContext.Movies
            .AsSingleQuery()
            .AsNoTracking()
            .Include(x => x.Genres)
            .Include(x => x.Sessions)
            .Where(x => x.Id == id)
            .Select(x => new GetMovieByIdResponse()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Descriptions,
                ImageLink = x.PosterLink,
                CreatedYear = x.Year,
                AgeLimit = x.AgeLimit,
                Countries = x.Countries.Select(c => c.Name),
                Genres = x.Genres.Select(g => g.Name),
                Sessions = x.Sessions
                    .OrderBy(s => s.Start)
                    .Select(s => new GetMovieByIdSessionDto()
                    {
                        Id = s.Id,
                        MinPrice = s.Price,
                        DateTime = s.Start.ToString("yyyy-MM-dd HH:mm")
                    })
            })
            .SingleOrDefaultAsync();
        
        return Ok(movie);
    }
}
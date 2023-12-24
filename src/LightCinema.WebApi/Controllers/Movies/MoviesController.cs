using LightCinema.Data;
using LightCinema.Data.Entities;
using LightCinema.WebApi.Application.Auth;
using LightCinema.WebApi.Application.Exceptions;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetMoviesResponse>> GetMovies(
        [FromQuery(Name = "date")] DateType dateType,
        [FromQuery(Name = "withSessions")] bool? withSessions = true)
    {
        var now = DateTimeOffset.UtcNow.UtcDateTime.AddHours(4);
        var (start, end) = dateType switch
        {
            DateType.Today => (
                now,
                DateOnly.FromDateTime(now.AddDays(1)).ToDateTime(new TimeOnly())),
            DateType.Tomorrow => (
                DateOnly.FromDateTime(now.AddDays(1)).ToDateTime(new TimeOnly()),
                DateOnly.FromDateTime(now.AddDays(2)).ToDateTime(new TimeOnly())),
            DateType.Soon => (
                DateOnly.FromDateTime(now.AddDays(2)).ToDateTime(new TimeOnly()),
                DateTime.MaxValue),
            _ => throw new ArgumentOutOfRangeException(nameof(dateType), dateType, "DateType not found")
        };

        var startOffset = new DateTimeOffset(start.AddHours(-4), TimeSpan.Zero);
        var endOffset = new DateTimeOffset(end.AddHours(-4), TimeSpan.Zero);

        var query = _dbContext.Movies
            .AsSingleQuery()
            .AsNoTracking()
            .Include(x => x.Genres)
            .Include(x => x.Sessions)
            .Where(x => x.Sessions.Any(s => startOffset < s.Start && s.Start < endOffset));

        var movies = await query.Select(x => new GetMovieDto
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
                    Time = s.Start.AddHours(4).ToString("HH:mm")
                })
        }).ToListAsync();

        if (withSessions is false)
        {
            foreach (var movie in movies)
            {
                movie.Sessions = null;
            }
        }

        return Ok(new GetMoviesResponse
        {
            Movies = movies
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetMovieByIdResponse>> GetMoviesById([FromRoute] int id)
    {
        var movie = await _dbContext.Movies
            .AsSingleQuery()
            .AsNoTracking()
            .Include(x => x.Genres)
            .Include(x => x.Sessions)
            .Where(x => x.Id == id)
            .Select(x => new GetMovieByIdResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Descriptions,
                ImageLink = x.ImageLink,
                PosterLink = x.PosterLink,
                CreatedYear = x.Year,
                AgeLimit = x.AgeLimit,
                Countries = x.Countries.Select(c => new GetCountryDto
                {
                    Id = c.Id,
                    Name = c.Name
                }),
                Genres = x.Genres.Select(g => new GetGenreDto
                {
                    Id = g.Id,
                    Name = g.Name
                }),
                Sessions = x.Sessions
                    .Where(s => s.Start > DateTimeOffset.UtcNow)
                    .OrderBy(s => s.Start)
                    .Select(s => new GetMovieByIdSessionDto
                    {
                        Id = s.Id,
                        MinPrice = s.Price,
                        DateTime = s.Start.AddHours(4).ToString("yyyy-MM-dd HH:mm")
                    })
            })
            .SingleOrDefaultAsync();

        return Ok(movie);
    }

    [HttpGet("genres")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetGenresResponse>> GetGenres()
    {
        var genres = await _dbContext.Genres
            .AsNoTracking()
            .AsSingleQuery()
            .Select(x => new GetGenreDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        return Ok(new GetGenresResponse
        {
            Genres = genres
        });
    }

    [HttpGet("countries")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetCountriesResponse>> GetCountries()
    {
        var countries = await _dbContext.Countries
            .AsNoTracking()
            .AsSingleQuery()
            .Select(x => new GetCountryDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        return Ok(new GetCountriesResponse
        {
            Countries = countries
        });
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = PolicyNames.RequireAdministratorRole)]
    public async Task<IActionResult> CreateMovie([FromBody] CreateMovieRequest request)
    {
        if (request.Year > DateTimeOffset.UtcNow.Year)
        {
            throw new BusinessException("Год создания фильма не может быть в будущем");
        }

        if (request.AgeLimit is < 0 or > 18)
        {
            throw new BusinessException("Возрастное ограничение может быть от 0 до 18 лет");
        }

        var genres = await _dbContext.Genres
            .AsSingleQuery()
            .Where(x => request.Genres.Contains(x.Id))
            .ToListAsync();

        if (genres.Count != request.Genres.Count)
        {
            throw new NotFoundException("Найдены не все жанры");
        }

        var countries = await _dbContext.Countries
            .AsSingleQuery()
            .Where(x => request.Countries.Contains(x.Id))
            .ToListAsync();

        if (countries.Count != request.Countries.Count)
        {
            throw new NotFoundException("Найдены не все страны");
        }

        var movie = new Movie
        {
            Name = request.Name,
            Descriptions = request.Descriptions,
            Genres = genres,
            Countries = countries,
            Year = request.Year!.Value,
            AgeLimit = request.AgeLimit!.Value,
            PosterLink = request.PosterLink,
            ImageLink = request.ImageLink
        };

        await _dbContext.Movies.AddAsync(movie);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = PolicyNames.RequireAdministratorRole)]
    public async Task<IActionResult> UpdateMovie([FromRoute] int id, [FromBody] UpdateMovieRequest request)
    {
        var movie = await _dbContext.Movies
            .AsSingleQuery()
            .Include(x => x.Genres)
            .Include(x => x.Countries)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (movie is null)
        {
            throw new BusinessException("Фильм не найден");
        }
        
        if (request.Year > DateTimeOffset.UtcNow.Year)
        {
            throw new BusinessException("Год создания фильма не может быть в будущем");
        }

        if (request.AgeLimit is < 0 or > 18)
        {
            throw new BusinessException("Возрастное ограничение может быть от 0 до 18 лет");
        }

        var genres = await _dbContext.Genres
            .AsSingleQuery()
            .Where(x => request.Genres.Contains(x.Id))
            .ToListAsync();

        if (genres.Count != request.Genres.Count)
        {
            throw new NotFoundException("Найдены не все жанры");
        }

        var countries = await _dbContext.Countries
            .AsSingleQuery()
            .Where(x => request.Countries.Contains(x.Id))
            .ToListAsync();

        if (countries.Count != request.Countries.Count)
        {
            throw new NotFoundException("Найдены не все страны");
        }

        movie.Countries = new List<Country>();
        movie.Genres = new List<Genre>();
        _dbContext.Movies.Update(movie);
        await _dbContext.SaveChangesAsync();
        
        movie.Name = request.Name;
        movie.Descriptions = request.Descriptions;
        movie.Genres = genres;
        movie.Countries = countries;
        movie.Year = request.Year!.Value;
        movie.AgeLimit = request.AgeLimit!.Value;
        movie.PosterLink = request.PosterLink;
        movie.ImageLink = request.ImageLink;
        
        _dbContext.Movies.Update(movie);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize(Policy = PolicyNames.RequireAdministratorRole)]
    public async Task<IActionResult> DeleteMovie([FromRoute] int id)
    {
        var movie = await _dbContext.Movies.AsSingleQuery().SingleOrDefaultAsync(x => x.Id == id);

        if (movie is null)
        {
            throw new BusinessException("Фильм не найден");
        }

        _dbContext.Movies.Remove(movie);
        await _dbContext.SaveChangesAsync();
        
        return NoContent();
    }
}
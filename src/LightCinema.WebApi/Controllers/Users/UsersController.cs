using LightCinema.Core.Movies.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LightCinema.WebApi.Controllers.Users;

public class UsersController : BaseController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("/{id}")]
    public async Task<IActionResult> GetMoviesByIdAsync(int id)
    {
        var result = await _mediator.Send(new GetMovieByIdQuery(id));
        
        return Ok();
    }
}
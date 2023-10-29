using MediatR;

namespace LightCinema.Core.Movies.Queries;

public sealed record GetMovieByIdQuery(int Id) : IRequest<GetMovieByIdQueryResult>;

public sealed record GetMovieByIdQueryResult;

public sealed class GetMovieByIdQueryHandler : IRequestHandler<GetMovieByIdQuery, GetMovieByIdQueryResult>
{
    public async Task<GetMovieByIdQueryResult> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
    {
        return new GetMovieByIdQueryResult();
    }
}
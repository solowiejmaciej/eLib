using eLib.DAL.Repositories;
using eLib.Models.Dtos;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Queries;

public record GetAllAuthors() : IRequest<Result<IEnumerable<AuthorDto>, Error>>;

public sealed class GetAllAuthorsHandler : IRequestHandler<GetAllAuthors, Result<IEnumerable<AuthorDto>, Error>>
{
    private readonly IAuthorRepository _authorRepository;

    public GetAllAuthorsHandler(
        IAuthorRepository authorRepository
        )
    {
        _authorRepository = authorRepository;
    }

    public async Task<Result<IEnumerable<AuthorDto>, Error>> Handle(GetAllAuthors request, CancellationToken cancellationToken)
    {
        var allAuthors = await _authorRepository.GetAllWithDetails(cancellationToken);
        var authorDtos = allAuthors.Select(x => x.MapToDto());
        return Result<IEnumerable<AuthorDto>, Error>.FromEnumerable(authorDtos);
    }
}
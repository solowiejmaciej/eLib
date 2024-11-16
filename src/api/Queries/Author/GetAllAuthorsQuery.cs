using eLib.DAL.Repositories;
using eLib.Models.Dtos;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Queries.Author;

public record GetAllAuthorsQuery() : IResultQuery<IEnumerable<AuthorDto>>;

public sealed class GetAllAuthorsHandler : IResultQueryHandler<GetAllAuthorsQuery, IEnumerable<AuthorDto>>
{
    private readonly IAuthorRepository _authorRepository;

    public GetAllAuthorsHandler(
        IAuthorRepository authorRepository
        )
    {
        _authorRepository = authorRepository;
    }

    public async Task<Result<IEnumerable<AuthorDto>, Error>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
    {
        var allAuthors = await _authorRepository.GetAllWithDetails(cancellationToken);
        var authorDtos = allAuthors.Select(x => x.MapToDto());
        return Result<IEnumerable<AuthorDto>, Error>.FromEnumerable(authorDtos);
    }
}
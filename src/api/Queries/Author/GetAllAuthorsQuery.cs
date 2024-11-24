using eLib.Common.Dtos;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories;
using eLib.Models.Results.Base;
namespace eLib.Queries.Author;

public record GetAllAuthorsQuery(PaginationParameters PaginationParameters) : IResultQuery<PaginationResult<AuthorDto>>;

public sealed class GetAllAuthorsHandler : IResultQueryHandler<GetAllAuthorsQuery, PaginationResult<AuthorDto>>
{
    private readonly IAuthorRepository _authorRepository;

    public GetAllAuthorsHandler(
        IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<Result<PaginationResult<AuthorDto>, Error>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
    {
        var allAuthors = await _authorRepository.GetAllPaginatedWithDetailsAsync(request.PaginationParameters, cancellationToken);
        return allAuthors.MapToDto(x => x.MapToDto());
    }
}
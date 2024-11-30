using eLib.Commands;
using eLib.Common.Dtos;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories;
using eLib.Models.Results.Base;
using FluentValidation;

namespace eLib.Queries.Book;

public record GetBooksByAuthorIdQuery(Guid AuthorId, PaginationParameters PaginationParameters)
    : IResultQuery<PaginationResult<BookDto>>;

public class GetBooksByAuthorIdQueryValidator : AbstractValidator<GetBooksByAuthorIdQuery>
{
    public GetBooksByAuthorIdQueryValidator()
    {
        RuleFor(x => x.AuthorId).NotEmpty();
    }
}

public class GetBooksByAuthorIdQueryHandler : IResultQueryHandler<GetBooksByAuthorIdQuery, PaginationResult<BookDto>>
{
    private readonly IBookRepository _bookRepository;

    public GetBooksByAuthorIdQueryHandler(
        IBookRepository bookRepository
        )
    {
        _bookRepository = bookRepository;
    }


    public async Task<Result<PaginationResult<BookDto>, Error>> Handle(GetBooksByAuthorIdQuery request, CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetAllPaginatedByAuthorId(request.AuthorId, request.PaginationParameters, cancellationToken);
        return books.MapToDto(x => x.MapToDto());
    }
}
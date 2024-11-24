using eLib.Common.Dtos;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Queries.Book;

public record GetAllBooksQuery(PaginationParameters PaginationParameters) : IResultQuery<PaginationResult<BookDto>>;

public class GetAllBooksQueryHandler : IResultQueryHandler<GetAllBooksQuery, PaginationResult<BookDto>>
{
    private readonly IBookRepository _bookRepository;

    public GetAllBooksQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }


    public async Task<Result<PaginationResult<BookDto>, Error>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var allAuthors = await _bookRepository.GetAllPaginatedWithDetailsAsync(request.PaginationParameters, cancellationToken);
        return allAuthors.MapToDto(x => x.MapToDto());
    }
}
using eLib.DAL.Repositories;
using eLib.Models.Dtos;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Queries.Book;

public record GetAllBooksQuery : IResultQuery<IEnumerable<BookDto>>;

public class GetAllBooksQueryHandler : IResultQueryHandler<GetAllBooksQuery, IEnumerable<BookDto>>
{
    private readonly IBookRepository _bookRepository;

    public GetAllBooksQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }


    public async Task<Result<IEnumerable<BookDto>, Error>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var allBooks = await _bookRepository.GetAllWithDetailsAsync(cancellationToken);
        var bookDtos = allBooks.Select(x => x.MapToDto());
        return Result<IEnumerable<BookDto>, Error>.FromEnumerable(bookDtos);

    }
}
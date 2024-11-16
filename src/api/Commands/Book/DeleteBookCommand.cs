using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Commands.Book;

public record DeleteBookCommand(Guid Id) : IResultCommand<Unit>;

public class DeleteBookCommandHandler : IResultCommandHandler<DeleteBookCommand, Unit>
{
    private readonly IBookRepository _bookRepository;

    public DeleteBookCommandHandler(
        IBookRepository bookRepository
        )
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<Unit, Error>> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken);
        if (book == null)
        {
            return BookErrors.NotFound;
        }
        await _bookRepository.DeleteAsync(book.Id, cancellationToken);
        await _bookRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
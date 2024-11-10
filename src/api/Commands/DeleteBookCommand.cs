using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Commands;

public record DeleteBookCommand(Guid Id) : IRequest<Result<Unit, Error>>;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, Result<Unit, Error>>
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
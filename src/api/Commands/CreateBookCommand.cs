using eLib.DAL.Entities;
using eLib.DAL.Repositories;
using MediatR;

namespace eLib.Commands;

public record CreateBookCommand() : IRequest<int>;

public sealed class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, int>
{
    private readonly IBookRepository _bookRepository;

    public CreateBookCommandHandler(
        IBookRepository bookRepository
        )
    {
        _bookRepository = bookRepository;
    }
    public async Task<int> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = Book.Create("Title", Guid.Empty, "Description", "www.example.com");
        await _bookRepository.AddAsync(book, cancellationToken);
        return await _bookRepository.SaveChangesAsync(cancellationToken);
    }
}
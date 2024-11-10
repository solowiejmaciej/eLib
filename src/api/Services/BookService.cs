using eLib.Commands;
using eLib.DAL.Entities;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;

    public BookService(
        IBookRepository bookRepository,
        IAuthorRepository authorRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
    }
    public async Task<Result<Guid, Error>> AddBookWithDetails(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetByIdAsync(command.AuthorId, cancellationToken);
        if (author == null)
        {
            return AuthorErrors.NotFound;
        }
        var book = Book.Create(command);
        var bookId = await _bookRepository.AddAsync(book, cancellationToken);
        await _bookRepository.SaveChangesAsync(cancellationToken);
        return bookId;
    }

    public async Task<Result<Unit, Error>> UpdateBookWithDetails(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var bookToUpdate = await _bookRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (bookToUpdate == null)
        {
            return BookErrors.NotFound;
        }

        var author = await _authorRepository.GetByIdAsync(request.AuthorId, cancellationToken);
        if (author == null)
        {
            return AuthorErrors.NotFound;
        }

        bookToUpdate.Update(request);

        await _bookRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public interface IBookService
{
    Task<Result<Guid, Error>> AddBookWithDetails(CreateBookCommand command, CancellationToken cancellationToken);
    Task<Result<Unit, Error>> UpdateBookWithDetails(UpdateBookCommand request, CancellationToken cancellationToken);
}
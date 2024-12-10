using eLib.Auth.Providers;
using eLib.DAL.Entities;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.ReadingList;

public record AddBookToReadingListCommand(Guid BookId) : IResultCommand<Unit>;

public class AddBookToReadingListCommandValidator : AbstractValidator<AddBookToReadingListCommand>
{
    public AddBookToReadingListCommandValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
    }
}

public class AddBookToReadingListCommandHandler : IResultCommandHandler<AddBookToReadingListCommand, Unit>
{
    private readonly IReadingListEntryRepository _readingListEntryRepository;
    private readonly IUserInfoProvider _userInfoProvider;

    public AddBookToReadingListCommandHandler(
        IReadingListEntryRepository readingListEntryRepository,
        IUserInfoProvider userInfoProvider)
    {
        _readingListEntryRepository = readingListEntryRepository;
        _userInfoProvider = userInfoProvider;
    }

    public async Task<Result<Unit, Error>> Handle(AddBookToReadingListCommand request, CancellationToken cancellationToken)
    {
        var userId = _userInfoProvider.GetCurrentUserID();

        var existsAsync = await _readingListEntryRepository.ExistsAsync(request.BookId, userId, cancellationToken);
        if (existsAsync)
        {
            return ReadingListErrors.AlreadyExists;
        }

        var readingListEntry = ReadingListEntry.Create(request.BookId, userId, 0, false);
        await _readingListEntryRepository.AddAsync(readingListEntry, cancellationToken);
        await _readingListEntryRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
using eLib.Auth.Providers;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.ReadingList;

public record RemoveBookFromReadingListCommand(Guid BookId) : IResultCommand<Unit>;

public class RemoveBookFromReadingListCommandValidator : AbstractValidator<RemoveBookFromReadingListCommand>
{
    public RemoveBookFromReadingListCommandValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
    }
}

public class RemoveBookFromReadingListCommandHandler : IResultCommandHandler<RemoveBookFromReadingListCommand, Unit>
{
    private readonly IReadingListEntryRepository _readingListEntryRepository;
    private readonly IUserInfoProvider _userInfoProvider;

    public RemoveBookFromReadingListCommandHandler(
        IReadingListEntryRepository readingListEntryRepository,
        IUserInfoProvider userInfoProvider)
    {
        _readingListEntryRepository = readingListEntryRepository;
        _userInfoProvider = userInfoProvider;
    }

    public async Task<Result<Unit, Error>> Handle(RemoveBookFromReadingListCommand request, CancellationToken cancellationToken)
    {
        var userId = _userInfoProvider.GetCurrentUserID();
        var readingListEntry = await _readingListEntryRepository.GetByBookIdAndUserIdAsync(request.BookId, userId, cancellationToken);
        if (readingListEntry is null)
        {
            return ReadingListErrors.NotFound;
        }

        await _readingListEntryRepository.DeleteAsync(readingListEntry.Id, cancellationToken);
        await _readingListEntryRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
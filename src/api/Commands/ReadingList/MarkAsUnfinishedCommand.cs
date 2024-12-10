using eLib.Auth.Providers;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.ReadingList;

public record MarkAsUnfinishedCommand(Guid BookId) : IResultCommand<Unit>;

public class MarkAsUnfinishedCommandValidator : AbstractValidator<MarkAsUnfinishedCommand>
{
    public MarkAsUnfinishedCommandValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
    }
}

public class MarkAsUnfinishedCommandHandler : IResultCommandHandler<MarkAsUnfinishedCommand, Unit>
{
    private readonly IReadingListEntryRepository _readingListEntryRepository;
    private readonly IUserInfoProvider _userInfoProvider;

    public MarkAsUnfinishedCommandHandler(
        IReadingListEntryRepository readingListEntryRepository,
        IUserInfoProvider userInfoProvider)
    {
        _readingListEntryRepository = readingListEntryRepository;
        _userInfoProvider = userInfoProvider;
    }

    public async Task<Result<Unit, Error>> Handle(MarkAsUnfinishedCommand request, CancellationToken cancellationToken)
    {
        var userId = _userInfoProvider.GetCurrentUserID();
        var readingListEntry = await _readingListEntryRepository.GetByBookIdAndUserIdAsync(request.BookId, userId, cancellationToken);
        if (readingListEntry is null)
        {
            return ReadingListErrors.NotFound;
        }

        var result = readingListEntry.MarkAsUnfinished();
        if (result.HasError())
            return result.Error;

        await _readingListEntryRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
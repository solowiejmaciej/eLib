using eLib.Auth.Providers;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.ReadingList;

public record MarkAsFinishedCommand(Guid BookId) : IResultCommand<Unit>;

public class MarkAsFinishedCommandValidator : AbstractValidator<MarkAsFinishedCommand>
{
    public MarkAsFinishedCommandValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
    }
}

public class MarkAsFinishedCommandHandler : IResultCommandHandler<MarkAsFinishedCommand, Unit>
{
    private readonly IReadingListEntryRepository _readingListEntryRepository;
    private readonly IUserInfoProvider _userInfoProvider;

    public MarkAsFinishedCommandHandler(
        IReadingListEntryRepository readingListEntryRepository,
        IUserInfoProvider userInfoProvider)
    {
        _readingListEntryRepository = readingListEntryRepository;
        _userInfoProvider = userInfoProvider;
    }

    public async Task<Result<Unit, Error>> Handle(MarkAsFinishedCommand request, CancellationToken cancellationToken)
    {
        var userId = _userInfoProvider.GetCurrentUserID();
        var readingListEntry = await _readingListEntryRepository.GetByBookIdAndUserIdAsync(request.BookId, userId, cancellationToken);
        if (readingListEntry is null)
        {
            return ReadingListErrors.NotFound;
        }

        var result = readingListEntry.MarkAsFinished();
        if (result.HasError())
            return result.Error;

        await _readingListEntryRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
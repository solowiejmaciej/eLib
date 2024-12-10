using System.Text.Json.Serialization;
using eLib.Auth.Providers;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.ReadingList;

public record UpdateProgressCommand(int Progress, Guid BookId) : IResultCommand<Unit>
{
    [JsonIgnore]
    public Guid BookId { get; set; } = BookId;
    public int Progress { get; set; } = Progress;
}

public class UpdateProgressCommandValidator : AbstractValidator<UpdateProgressCommand>
{
    public UpdateProgressCommandValidator()
    {
        RuleFor(x => x.Progress).InclusiveBetween(0, 100);
    }
}

public class UpdateProgressCommandHandler : IResultCommandHandler<UpdateProgressCommand, Unit>
{
    private readonly IReadingListEntryRepository _readingListRepository;
    private readonly IUserInfoProvider _userInfoProvider;

    public UpdateProgressCommandHandler(
        IReadingListEntryRepository readingListRepository,
        IUserInfoProvider userInfoProvider)
    {
        _readingListRepository = readingListRepository;
        _userInfoProvider = userInfoProvider;
    }

    public async Task<Result<Unit, Error>> Handle(UpdateProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = _userInfoProvider.GetCurrentUserID();

        var readingListEntry = await _readingListRepository.GetByBookIdAndUserIdAsync(request.BookId, userId, cancellationToken);

        if (readingListEntry is null)
        {
            return ReadingListErrors.NotFound;
        }

        var result = readingListEntry.UpdateProgress(request.Progress);
        if(result.HasError())
            return result.Error;

        await _readingListRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}


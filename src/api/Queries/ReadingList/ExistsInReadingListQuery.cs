using eLib.Auth.Providers;
using eLib.DAL.Repositories;
using eLib.Models.Results.Base;
using FluentValidation;

namespace eLib.Queries.ReadingList;

public record ExistsInReadingListQuery(Guid BookId) : IResultQuery<bool>;

public class ExistsInReadingListQueryValidator : AbstractValidator<ExistsInReadingListQuery>
{
    public ExistsInReadingListQueryValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
    }
}

public class ExistsInReadingListQueryQueryHandler : IResultQueryHandler<ExistsInReadingListQuery, bool>
{
    private readonly IReadingListEntryRepository _readingListEntryRepository;
    private readonly IUserInfoProvider _userInfoProvider;

    public ExistsInReadingListQueryQueryHandler(
        IReadingListEntryRepository readingListEntryRepository,
        IUserInfoProvider userInfoProvider)
    {
        _readingListEntryRepository = readingListEntryRepository;
        _userInfoProvider = userInfoProvider;
    }

    public async Task<Result<bool, Error>> Handle(ExistsInReadingListQuery request, CancellationToken cancellationToken)
    {
        var userId = _userInfoProvider.GetCurrentUserID();
        var existsAsync = await _readingListEntryRepository.ExistsAsync(request.BookId, userId, cancellationToken);
        return existsAsync;
    }
}
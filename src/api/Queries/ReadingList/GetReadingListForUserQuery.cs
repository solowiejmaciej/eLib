using eLib.Common.Dtos;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories;
using eLib.Models.Results.Base;
using FluentValidation;

namespace eLib.Queries.ReadingList;

public record GetReadingListForUserQuery(Guid UserId, PaginationParameters PaginationParameters) : IResultQuery<PaginationResult<ReadingListEntryBookDto>>;

public class GetReadingListForUserQueryValidator : AbstractValidator<GetReadingListForUserQuery>
{
    public GetReadingListForUserQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PaginationParameters).NotNull();
    }
}

public class GetReadingListForUserQueryHandler : IResultQueryHandler<GetReadingListForUserQuery, PaginationResult<ReadingListEntryBookDto>>
{
    private readonly IReadingListEntryRepository _readingListEntryRepository;

    public GetReadingListForUserQueryHandler(IReadingListEntryRepository readingListEntryRepository)
    {
        _readingListEntryRepository = readingListEntryRepository;
    }

    public async Task<Result<PaginationResult<ReadingListEntryBookDto>, Error>> Handle(GetReadingListForUserQuery request, CancellationToken cancellationToken)
        => await _readingListEntryRepository.GetPaginatedReadingListEntriesWithBookAsync(request.UserId, cancellationToken, request.PaginationParameters);
}
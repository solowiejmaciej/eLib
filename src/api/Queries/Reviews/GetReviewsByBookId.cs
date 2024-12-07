using eLib.Common.Dtos;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories;
using eLib.Models.Results.Base;
using FluentValidation;

namespace eLib.Queries.Reviews;

public record GetReviewsByBookId(Guid BookId, PaginationParameters PaginationParameters) : IResultQuery<PaginationResult<ReviewUserDto>>;

public class GetReviewsByBookIdValidator : AbstractValidator<GetReviewsByBookId>
{
    public GetReviewsByBookIdValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty();
    }
}

public class GetReviewsByBookIdHandler : IResultQueryHandler<GetReviewsByBookId, PaginationResult<ReviewUserDto>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetReviewsByBookIdHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<PaginationResult<ReviewUserDto>, Error>> Handle(GetReviewsByBookId request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetPaginatedByBookIdAsync(request.BookId, request.PaginationParameters, cancellationToken);

        return reviews;
    }
}
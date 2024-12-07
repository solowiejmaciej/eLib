using eLib.Common.Dtos;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories;
using eLib.Models.Results.Base;

namespace eLib.Queries.Reviews;

public record GetReviewsByUserId(Guid UserId, PaginationParameters PaginationParameters) : IResultQuery<PaginationResult<ReviewDto>>;

public class GetReviewsByUserIdHandler : IResultQueryHandler<GetReviewsByUserId, PaginationResult<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetReviewsByUserIdHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<PaginationResult<ReviewDto>, Error>> Handle(GetReviewsByUserId request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetPaginatedByUserIdAsync(request.UserId, request.PaginationParameters, cancellationToken);
        return reviews.MapToDto(x => x.MapToDto());
    }
}
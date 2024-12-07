using eLib.Common.Dtos;
using eLib.DAL.Entities;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories.Base;
using eLib.Models.Results.Base;
using eLib.Services;
using Microsoft.EntityFrameworkCore;

namespace eLib.DAL.Repositories;

public class ReviewsRepository : RepositoryBase<Review>, IReviewRepository
{
    private readonly LibraryDbContext _context;
    private readonly IPaginationService _paginationService;

    public ReviewsRepository(
        LibraryDbContext context,
        IPaginationService paginationService) : base(context, paginationService)
    {
        _context = context;
        _paginationService = paginationService;
    }

    public override async Task<PaginationResult<Review>> GetAllPaginatedAsync(
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = GetQueryable();

        if (!string.IsNullOrEmpty(paginationParameters.SearchFraze))
        {
            var searchTerm = paginationParameters.SearchFraze.ToLower();
            query = query.Where(b =>
                EF.Functions.Like(b.Content.ToLower(), $"%{searchTerm}%"));
        }
        return await _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);
    }

    public async Task<PaginationResult<ReviewUserDto>> GetPaginatedByBookIdAsync(
        Guid bookId,
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = GetQueryable().
            Where(x => x.BookId == bookId).
            Include(x => x.User).
            Select(x => new ReviewUserDto
            {
                Id = x.Id,
                BookId = x.BookId,
                Rating = x.Rating,
                Content = x.Content,
                Name = x.User.Name,
                Surname = x.User.Surname,
                CreatedAt = x.CreatedAt
            });
        return await _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);
    }

    public Task<PaginationResult<Review>> GetPaginatedByUserIdAsync(Guid requestUserId, PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = GetQueryable().Where(x => x.UserId == requestUserId);

        if (!string.IsNullOrEmpty(paginationParameters.SearchFraze))
        {
            var searchTerm = paginationParameters.SearchFraze.ToLower();
            query = query.Where(b =>
                EF.Functions.Like(b.Content.ToLower(), $"%{searchTerm}%"));
        }

        return _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);
    }
}


public interface IReviewRepository : IRepositoryBase<Review>
{
    Task<PaginationResult<ReviewUserDto>> GetPaginatedByBookIdAsync(Guid bookId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
    Task<PaginationResult<Review>> GetPaginatedByUserIdAsync(Guid requestUserId, PaginationParameters requestPaginationParameters, CancellationToken cancellationToken);
}
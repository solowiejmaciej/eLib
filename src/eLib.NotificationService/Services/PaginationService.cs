using eLib.NotificationService.DAL.Pagination;
using eLib.NotificationService.Models;
using Microsoft.EntityFrameworkCore;

namespace eLib.NotificationService.Services
{
    public class PaginationService : IPaginationService
    {
        public async Task<PaginationResult<TEntity>> GetPaginatedResultAsync<TEntity>(IQueryable<TEntity> query, PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            var count = query.Count();
            var items = await query
                .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .ToListAsync(cancellationToken);
            return new PaginationResult<TEntity>(items, count, paginationParameters.PageNumber, paginationParameters.PageSize);
        }
    }

    public interface IPaginationService
    {
       Task<PaginationResult<TEntity>> GetPaginatedResultAsync<TEntity>(
           IQueryable<TEntity> query,
           PaginationParameters paginationParameters,
           CancellationToken cancellationToken);
    }
}
using eLib.DAL.Entities;
using eLib.DAL.Pagination;
using eLib.Models.Results.Base;

namespace eLib.DAL.Repositories.Base;

public interface IRepositoryWithDetailsBase<T, TDetails>
    where T : Entity
    where TDetails : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<T?>> GetAllAsync(CancellationToken cancellationToken);
    Task<Guid> AddAsync(T entity, CancellationToken cancellationToken);
    Task UpdateAsync(T entity, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<T>> GetAllWithDetailsAsync(CancellationToken cancellationToken);
    Task<T?> GetByIdWithDetailsAsync(Guid requestId, CancellationToken cancellationToken);
    Task<TDetails?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PaginationResult<T>> GetAllPaginatedAsync(PaginationParameters paginationParameters,
        CancellationToken cancellationToken);
    Task<PaginationResult<T>> GetAllPaginatedWithDetailsAsync(PaginationParameters paginationParameters,
        CancellationToken cancellationToken);
}
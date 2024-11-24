using System.Linq.Expressions;
using eLib.DAL;
using eLib.DAL.Entities;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories.Base;
using eLib.Models.Results.Base;
using eLib.Services;
using Microsoft.EntityFrameworkCore;

public class RepositoryWithDetailsBase<T, TDetails> : IRepositoryWithDetailsBase<T, TDetails>
    where T : Entity
    where TDetails : Entity
{
    private readonly LibraryDbContext _context;
    private readonly DbSet<T> _dbSet;
    private readonly Expression<Func<T, TDetails>> _detailsSelector;
    private readonly IPaginationService _paginationService;

    public RepositoryWithDetailsBase(
        LibraryDbContext context,
        Expression<Func<T, TDetails>> detailsSelector,
        IPaginationService paginationService)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _detailsSelector = detailsSelector;
        _paginationService = paginationService;
    }

    protected IQueryable<T> GetQueryable() => _dbSet.AsQueryable();

    protected IQueryable<T> GetQueryableWithDetails()
        => _dbSet.Include(_detailsSelector);

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public virtual async Task<IEnumerable<T?>> GetAllAsync(CancellationToken cancellationToken)
        => await _dbSet.ToListAsync(cancellationToken);

    public virtual async Task<T?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
        => await GetQueryableWithDetails()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public virtual async Task<IEnumerable<T>> GetAllWithDetailsAsync(CancellationToken cancellationToken)
        => await GetQueryableWithDetails()
            .ToListAsync(cancellationToken);

    public virtual async Task<Guid> AddAsync(T entity, CancellationToken cancellationToken)
    {
        var createdEntity = await _dbSet.AddAsync(entity, cancellationToken);
        return createdEntity.Entity.Id;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        _dbSet.Update(entity);
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbSet.FindAsync(id, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<PaginationResult<T>> GetAllPaginatedAsync(PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = GetQueryable();

        return await _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);
    }

    public virtual async Task<PaginationResult<T>> GetAllPaginatedWithDetailsAsync(
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = GetQueryableWithDetails();

        return await _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);
    }

    public virtual async Task<TDetails?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken)
        => await GetQueryableWithDetails()
            .Where(e => e.Id == id)
            .Select(_detailsSelector)
            .FirstOrDefaultAsync(cancellationToken);
}
using eLib.DAL;
using eLib.DAL.Entities;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories.Base;
using eLib.Models.Results.Base;
using eLib.Services;
using Microsoft.EntityFrameworkCore;

public class RepositoryBase<T> : IRepositoryBase<T> where T : Entity
{
    private readonly LibraryDbContext _context;
    private readonly DbSet<T> _dbSet;
    private readonly IPaginationService _paginationService;

    public RepositoryBase(
        LibraryDbContext context,
        IPaginationService paginationService)
    {
        _context = context;
        _paginationService = paginationService;
        _dbSet = context.Set<T>();
    }

    protected IQueryable<T> GetQueryable() => _dbSet.AsQueryable();

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<T?>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Guid> AddAsync(T entity, CancellationToken cancellationToken)
    {
        var createdEntity = await _dbSet.AddAsync(entity, cancellationToken);
        return createdEntity.Entity.Id;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        _dbSet.Update(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbSet.FindAsync(id, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<PaginationResult<T>> GetAllPaginatedAsync(PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(paginationParameters.SearchFraze))
        {
            var searchTerm = paginationParameters.SearchFraze.ToLower();
            query = query.Where(entity => entity.ToString().ToLower().Contains(searchTerm));
        }
        return await _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);
    }
}
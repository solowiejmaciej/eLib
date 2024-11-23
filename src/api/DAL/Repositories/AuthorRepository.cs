using eLib.DAL.Entities;
using eLib.DAL.Repositories.Base;
using eLib.Models.Results.Base;
using Microsoft.EntityFrameworkCore;

namespace eLib.DAL.Repositories;

public class AuthorRepository : RepositoryBase<Author>, IAuthorRepository
{
    private readonly LibraryDbContext _context;

    public AuthorRepository(LibraryDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Author>> GetAllWithDetails(CancellationToken cancellationToken)
        => await _context.Authors
            .Include(x => x.Details)
            .ToListAsync(cancellationToken);

    public Task<Author?> GetByIdWithDetails(Guid requestId, CancellationToken cancellationToken)
        => _context.Authors
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == requestId, cancellationToken);
}

public interface IAuthorRepository : IRepositoryBase<Author>
{
    Task<IEnumerable<Author>> GetAllWithDetails(CancellationToken cancellationToken);
    Task<Author?> GetByIdWithDetails(Guid requestId, CancellationToken cancellationToken);
}
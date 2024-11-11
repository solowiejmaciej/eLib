using eLib.DAL.Entities;
using eLib.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace eLib.DAL.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    private readonly LibraryDbContext _context;

    public UserRepository(LibraryDbContext context) : base(context)
    {
        _context = context;
    }


    public async Task<IEnumerable<User>> GetAllWithDetailsAsync(CancellationToken cancellationToken)
        => await _context.Users
            .Include(x => x.Details)
            .ToListAsync(cancellationToken);

    public async Task<User?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
        => await _context.Users
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}

public interface IUserRepository : IRepositoryBase<User>
{
    Task<IEnumerable<User>> GetAllWithDetailsAsync(CancellationToken cancellationToken);
    Task<User?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
}
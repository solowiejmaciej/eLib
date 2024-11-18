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

    public async Task<User?> GetByEmailWithDetailsAsync(string email, CancellationToken cancellationToken)
        => await _context.Users
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public async Task<User?> GetByPhoneNumberWithDetailsAsync(string phoneNumber, CancellationToken cancellationToken)
        => await _context.Users
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);

    public async Task<bool> IsEmailUnique(string requestEmail, CancellationToken cancellationToken)
        => await _context.Users
            .AllAsync(x => x.Email != requestEmail, cancellationToken);

    public Task<bool> IsPhoneNumberUnique(string requestPhoneNumber, CancellationToken cancellationToken)
        => _context.Users
            .AllAsync(x => x.PhoneNumber != requestPhoneNumber, cancellationToken);
}

public interface IUserRepository : IRepositoryBase<User>
{
    Task<IEnumerable<User>> GetAllWithDetailsAsync(CancellationToken cancellationToken);
    Task<User?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByEmailWithDetailsAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByPhoneNumberWithDetailsAsync(string phoneNumber, CancellationToken cancellationToken);
    Task<bool> IsEmailUnique(string requestEmail, CancellationToken cancellationToken);
    Task<bool> IsPhoneNumberUnique(string requestPhoneNumber, CancellationToken cancellationToken);
}
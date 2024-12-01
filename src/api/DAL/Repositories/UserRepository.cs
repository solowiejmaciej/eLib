using eLib.DAL.Entities;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories.Base;
using eLib.Models.Results.Base;
using eLib.Services;
using Microsoft.EntityFrameworkCore;

namespace eLib.DAL.Repositories;

public class UserRepository : RepositoryWithDetailsBase<User, UserDetails>, IUserRepository
{
    private readonly LibraryDbContext _context;
    private readonly IPaginationService _paginationService;

    public UserRepository(LibraryDbContext context, IPaginationService paginationService) : base(context, user => user.Details, paginationService)
    {
        _context = context;
        _paginationService = paginationService;
    }

    public override async Task<PaginationResult<User>> GetAllPaginatedWithDetailsAsync(
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = GetQueryableWithDetails();

        if (!string.IsNullOrEmpty(paginationParameters.SearchFraze))
        {
            var searchTerm = paginationParameters.SearchFraze.ToLower();
            query = query.Where(b =>
                EF.Functions.Like(b.Email.ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like(b.Surname.ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like(b.Name.ToLower(), $"%{searchTerm}%"));
        }
        return await _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);
    }

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

public interface IUserRepository : IRepositoryWithDetailsBase<User, UserDetails>
{
    Task<User?> GetByEmailWithDetailsAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByPhoneNumberWithDetailsAsync(string phoneNumber, CancellationToken cancellationToken);
    Task<bool> IsEmailUnique(string requestEmail, CancellationToken cancellationToken);
    Task<bool> IsPhoneNumberUnique(string requestPhoneNumber, CancellationToken cancellationToken);
}
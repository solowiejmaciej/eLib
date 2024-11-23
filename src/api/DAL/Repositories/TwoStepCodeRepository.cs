using eLib.DAL.Entities;
using eLib.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace eLib.DAL.Repositories;

public class TwoStepCodeRepository : RepositoryBase<TwoStepCode>, ITwoStepCodeRepository
{
    private readonly LibraryDbContext _context;

    public TwoStepCodeRepository(LibraryDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<TwoStepCode?> GetByCodeAsync(string code, CancellationToken cancellationToken)
        => await _context.TwoStepCodes
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
}

public interface ITwoStepCodeRepository : IRepositoryBase<TwoStepCode>
{
    Task<TwoStepCode?> GetByCodeAsync(string code, CancellationToken cancellationToken);
}
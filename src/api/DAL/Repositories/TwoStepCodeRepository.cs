using eLib.DAL.Entities;
using eLib.DAL.Repositories.Base;
using eLib.Services;
using Microsoft.EntityFrameworkCore;

namespace eLib.DAL.Repositories;

public class TwoStepCodeRepository : RepositoryBase<TwoStepCode>, ITwoStepCodeRepository
{
    private readonly LibraryDbContext _context;
    private readonly IPaginationService _paginationService;

    public TwoStepCodeRepository(
        LibraryDbContext context,
        IPaginationService paginationService) : base(context, paginationService)
    {
        _context = context;
        _paginationService = paginationService;
    }

    public async Task<TwoStepCode?> GetByCodeAsync(string code, CancellationToken cancellationToken)
        => await _context.TwoStepCodes
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
}

public interface ITwoStepCodeRepository : IRepositoryBase<TwoStepCode>
{
    Task<TwoStepCode?> GetByCodeAsync(string code, CancellationToken cancellationToken);
}
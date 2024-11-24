using eLib.DAL.Entities;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories.Base;
using eLib.Models.Results.Base;
using eLib.Services;
using Microsoft.EntityFrameworkCore;

namespace eLib.DAL.Repositories;

public class ReservationRepository : RepositoryBase<Reservation>, IReservationRepository
{
    private readonly LibraryDbContext _context;
    private readonly IPaginationService _paginationService;

    public ReservationRepository(
        LibraryDbContext context,
        IPaginationService paginationService) : base(context, paginationService)
    {
        _context = context;
        _paginationService = paginationService;
    }

    public async Task<PaginationResult<Reservation>> GetPaginatedReservationsByUserId(Guid userId, PaginationParameters paginationParameters, CancellationToken cancellationToken)
    {
        var query = _context.Reservations
            .Where(x => x.UserId == userId)
            .AsNoTracking();

        return await _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);
    }

}

public interface IReservationRepository : IRepositoryBase<Reservation>
{
    Task<PaginationResult<Reservation>> GetPaginatedReservationsByUserId(Guid userId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
}
using eLib.DAL.Entities;
using eLib.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace eLib.DAL.Repositories;

public class ReservationRepository : RepositoryBase<Reservation>, IReservationRepository
{
    private readonly LibraryDbContext _context;

    public ReservationRepository(LibraryDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByUserId(Guid userId, CancellationToken cancellationToken)
        => await _context.Reservations
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

}

public interface IReservationRepository : IRepositoryBase<Reservation>
{
    Task<IEnumerable<Reservation>> GetReservationsByUserId(Guid userId, CancellationToken cancellationToken);
}
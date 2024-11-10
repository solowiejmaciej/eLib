using eLib.DAL.Entities;
using eLib.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace eLib.DAL.Repositories;

public class BookRepository : RepositoryBase<Book>, IBookRepository
{
    private readonly LibraryDbContext _context;

    public BookRepository(LibraryDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Book?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
        => await _context.Books
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<Book>> GetAllWithDetailsAsync(CancellationToken cancellationToken)
        => await _context.Books
            .Include(x => x.Details)
            .ToListAsync(cancellationToken);
}

public interface IBookRepository : IRepositoryBase<Book>
{
    Task<Book?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Book>> GetAllWithDetailsAsync(CancellationToken cancellationToken);
}
using eLib.Common.Dtos;
using eLib.DAL.Entities;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories.Base;
using eLib.Models.Results.Base;
using eLib.Services;
using Microsoft.EntityFrameworkCore;

namespace eLib.DAL.Repositories;

public class ReadingListEntryRepository : RepositoryBase<ReadingListEntry>, IReadingListEntryRepository
{
    private readonly LibraryDbContext _context;
    private readonly IPaginationService _paginationService;

    public ReadingListEntryRepository(LibraryDbContext context, IPaginationService paginationService) : base(context, paginationService)
    {
        _context = context;
        _paginationService = paginationService;
    }

    public Task<bool> ExistsAsync(Guid bookId, Guid userId, CancellationToken cancellationToken)
        => _context.ReadingListEntries
            .AnyAsync(readingListEntry => readingListEntry.BookId == bookId && readingListEntry.UserId == userId, cancellationToken);

    public async Task<PaginationResult<ReadingListEntryBookDto>> GetPaginatedReadingListEntriesWithBookAsync(Guid userId, CancellationToken cancellationToken, PaginationParameters paginationParameters)
    {
        var query = _context.ReadingListEntries
            .Where(r => r.UserId == userId)
            .Join(
                _context.Books,
                readingListEntry => readingListEntry.BookId,
                book => book.Id,
                (readingListEntry, book) => new { readingListEntry, book }
            )
            .Join(
                _context.BookDetails,
                combined => combined.book.Id,
                bookDetails => bookDetails.BookId,
                (combined, bookDetails) => new { combined.readingListEntry, combined.book, bookDetails }
            )
            .Join(
                _context.Authors,
                combined => combined.book.AuthorId,
                author => author.Id,
                (combined, author) => new ReadingListEntryBookDto
                {
                    BookId = combined.readingListEntry.BookId,
                    UserId = combined.readingListEntry.UserId,
                    Progress = combined.readingListEntry.Progress,
                    IsFinished = combined.readingListEntry.IsFinished,
                    DateAdded = combined.readingListEntry.DateAdded,
                    Title = combined.book.Title,
                    AuthorName = $"{author.Name} {author.Surname}",
                    AuthorId = author.Id,
                    CoverUrl = combined.bookDetails.CoverUrl,
                }
            )
            .OrderByDescending(readingListEntryBookDto => readingListEntryBookDto.DateAdded);

        return await _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);
    }

    public async Task<ReadingListEntry?> GetByBookIdAndUserIdAsync(Guid bookId, Guid userId, CancellationToken cancellationToken)
        => await _context.ReadingListEntries
            .Where(readingListEntry => readingListEntry.BookId == bookId && readingListEntry.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
}


public interface IReadingListEntryRepository : IRepositoryBase<ReadingListEntry>
{
    Task<bool> ExistsAsync(Guid bookId, Guid userId, CancellationToken cancellationToken);

    Task<PaginationResult<ReadingListEntryBookDto>> GetPaginatedReadingListEntriesWithBookAsync(Guid userId,
        CancellationToken cancellationToken, PaginationParameters paginationParameters);

    Task<ReadingListEntry?> GetByBookIdAndUserIdAsync(Guid bookId, Guid userId, CancellationToken cancellationToken);
}
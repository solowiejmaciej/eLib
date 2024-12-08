using eLib.DAL.Entities;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories.Base;
using eLib.Models.Results.Base;
using eLib.Services;
using Microsoft.EntityFrameworkCore;

namespace eLib.DAL.Repositories;

public class BookRepository : RepositoryWithDetailsBase<Book, BookDetails>, IBookRepository
{
    private readonly LibraryDbContext _context;
    private readonly IPaginationService _paginationService;

    public BookRepository(
        LibraryDbContext context,
        IPaginationService paginationService) : base(context, book => book.Details, paginationService)
    {
        _context = context;
        _paginationService = paginationService;
    }

    public override async Task<PaginationResult<Book>> GetAllPaginatedWithDetailsAsync(
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = GetQueryableWithDetails();

        if (!string.IsNullOrEmpty(paginationParameters.SearchFraze))
        {
            var searchTerm = paginationParameters.SearchFraze.ToLower();
            query = query.Where(b =>
                EF.Functions.Like(b.Title.ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like(b.Details.Description.ToLower(), $"%{searchTerm}%"));
        }
        return await _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);
    }

    public async Task<PaginationResult<Book>> GetAllPaginatedWithDetailsByAuthorId(
        Guid authorId,
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = GetQueryableWithDetails();
        query = query.Where(b => b.AuthorId == authorId);

        if (!string.IsNullOrEmpty(paginationParameters.SearchFraze))
        {
            var searchTerm = paginationParameters.SearchFraze.ToLower();
            query = query.Where(b =>
                EF.Functions.Like(b.Title.ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like(b.Details.Description.ToLower(), $"%{searchTerm}%"));
        }

        return await _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);
    }
}

public interface IBookRepository : IRepositoryWithDetailsBase<Book, BookDetails>
{
    Task<PaginationResult<Book>> GetAllPaginatedWithDetailsByAuthorId(Guid authorId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
}
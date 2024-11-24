using eLib.DAL.Entities;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories.Base;
using eLib.Models.Results.Base;
using eLib.Services;
using Microsoft.EntityFrameworkCore;

namespace eLib.DAL.Repositories;

public class AuthorRepository : RepositoryWithDetailsBase<Author, AuthorDetails>, IAuthorRepository
{
    private readonly IPaginationService _paginationService;

    public AuthorRepository(LibraryDbContext context, IPaginationService paginationService) : base(context, author => author.Details, paginationService)
    {
        _paginationService = paginationService;
    }

    public override async Task<PaginationResult<Author>> GetAllPaginatedWithDetailsAsync(
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = GetQueryableWithDetails();

        if (!string.IsNullOrEmpty(paginationParameters.SearchFraze))
        {
            var searchTerm = paginationParameters.SearchFraze.ToLower();
            query = query.Where(b =>
                EF.Functions.Like(b.Name.ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like(b.Details.Biography.ToLower(), $"%{searchTerm}%"));
        }
        return await _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);
    }
}

public interface IAuthorRepository : IRepositoryWithDetailsBase<Author, AuthorDetails>
{

}
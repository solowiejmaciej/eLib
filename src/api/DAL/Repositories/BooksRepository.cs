using eLib.DAL.Entities;
using eLib.DAL.Repositories.Base;

namespace eLib.DAL.Repositories
{
    public class BookRepository : RepositoryBase<Book>, IBookRepository
    {
        public BookRepository(LibraryDbContext context) : base(context)
        {

        }
    }

    public interface IBookRepository : IRepositoryBase<Book>
    {
    }
}
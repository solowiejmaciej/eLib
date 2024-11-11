using Microsoft.EntityFrameworkCore;
using eLib.DAL.Entities;
using eLib.DomainEvents;
using MediatR;

namespace eLib.DAL
{
    public class LibraryDbContext : DbContext
    {
        private readonly IMediator _mediator;

        public LibraryDbContext(
            DbContextOptions<LibraryDbContext> options,
            IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookDetails> BookDetails { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorDetails> AuthorDetails { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Details)
                .WithOne()
                .HasForeignKey<BookDetails>(bd => bd.BookId);

            modelBuilder.Entity<Author>()
                .HasOne(a => a.Details)
                .WithOne()
                .HasForeignKey<AuthorDetails>(ad => ad.AuthorId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Details)
                .WithOne()
                .HasForeignKey<UserDetails>(ud => ud.UserId);

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchDomainEventsAsync();
            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }

        private async Task DispatchDomainEventsAsync()
        {
            var eventsToPublish = ChangeTracker.Entries<AggregateRoot>()
                .Select(aggregateRoot => aggregateRoot.Entity)
                .SelectMany(aggregateRoot =>
                {
                    var domainEvents = aggregateRoot.GetDomainEvents();
                    aggregateRoot.ClearDomainEvents();
                    return domainEvents;
                }).ToList();


            var tasks = eventsToPublish
                .Select(async (domainEvent) =>
                {
                    await _mediator.Publish(domainEvent);
                });

            await Task.WhenAll(tasks);
        }
    }
}